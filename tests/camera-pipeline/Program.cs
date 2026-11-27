using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using MASAN_SERIALIZATION;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Views.Dashboards;

namespace CameraPipeline.QA
{
    internal static class Program
    {
        private static Fixture fixture;
        private static string output;
        private static bool baseline;
        private static int failures;
        private static readonly List<object> reports = new List<object>();
        private static readonly List<string> failedScenarios = new List<string>();
        private static int Main(string[] args)
        {
            try
            {
                if ((args.Length != 2 && args.Length != 3) || (args[1] != "current" && args[1] != "baseline") || (args.Length == 3 && args[2] != "smoke"))
                    throw new ArgumentException("Usage: CameraPipeline.Bench.exe <fresh-artifact-directory> current|baseline [smoke]");
                bool smoke = args.Length == 3;
                output = Path.GetFullPath(args[0]); baseline = args[1] == "baseline";
                if (Directory.Exists(output)) throw new ArgumentException("Output directory must not exist");
                if (output.StartsWith(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), StringComparison.OrdinalIgnoreCase) ||
                    output.StartsWith(@"C:\MasanSerialization", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Output must be a distinct sandbox directory");
                Directory.CreateDirectory(output);
                Json.Write(Path.Combine(output, "environment.json"), new {
                    utc = DateTime.UtcNow.ToString("o"), local = DateTimeOffset.Now.ToString("o"),
                    runtime = Environment.Version.ToString(), os = Environment.OSVersion.ToString(),
                    cpuCount = Environment.ProcessorCount, process64bit = Environment.Is64BitProcess,
                    stopwatchFrequency = Stopwatch.Frequency, commandLine = Environment.CommandLine,
                    mode = args[1], processId = Process.GetCurrentProcess().Id,
                    boundaries = "Exact production callback/decision methods with test timing probes. Fake UI Invoke is serialized/delayed, console is count-only; file PO loaded into separate Main/Sub DTO instances. No actual WinForms render, TCP/camera optics, production DB or PLC transport. Real Task.Run/BackgroundWorker/ContinueWith scheduling. Cross-PO injected; this suite makes no real SQLite measurement. Persistence enqueue production code, no disk consumer."
                });
                fixture = Fixture.Create(Path.Combine(output, "fake-po"));
                // Warm both dispatch paths on disjoint state; no warmup included in reports.
                Warmup();
                Classifications();
                DuplicateRace();
                if (!baseline) { PoFenceTruthTable(); OwnershipSnapshots(); StaleCallbackOwner(); }
                if (!smoke)
                {
                Bulk("steady-main", 1000, 1000, new BoundaryOptions(), false);
                Bulk("slow-ui-main", 200, 1000, new BoundaryOptions { UiDelayMs = 5 }, false);
                Bulk("burst-main-sample", 5000, 0, new BoundaryOptions(), false);
                Bulk("slow-crosspo-main", 200, 1000, new BoundaryOptions { CrossPoDelayMs = 2 }, false);
                Bulk("slow-read-sub", 200, 1000, new BoundaryOptions { PlcReadDelayMs = 5 }, true);
                Bulk("slow-transport-sub", 200, 1000, new BoundaryOptions { PlcSendDelayMs = 5 }, true);
                BothCameras();
                if (!baseline) Bulk("full-po-200000", Fixture.Population, 0, new BoundaryOptions(), false);
                }
                Json.Write(Path.Combine(output, "suite.json"), new {
                    mode = args[1], phase = smoke ? "smoke" : "full", fixturePopulation = Fixture.Population, fixtureCodesSha256 = fixture.CodesSha256,
                    passed = failures == 0, failedScenarios, scenarioCount = reports.Count, scenarios = reports,
                    note = "Baseline failures characterize old source; current must pass. Saturation ordering is reported, not an asserted FIFO/SLA. Busy Sub rejects are explicit Error decisions, not losses."
                });
                Console.WriteLine("MODE=" + args[1] + " SCENARIOS=" + reports.Count + " FAILED=" + failures + " OUTPUT=" + output);
                return failures == 0 ? 0 : 2;
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                if (output != null && Directory.Exists(output)) Json.Write(Path.Combine(output, "fatal.json"), new { type = error.GetType().Name, error = error.ToString() });
                return 3;
            }
        }

        private static FDashboard Reset(Measurement measurement, BoundaryOptions options)
        {
            if (Globals.PendingCameraRequests != 0) throw new InvalidOperationException("Cannot reset fixture while production-admitted requests remain pending");
            QaHooks.Reset(measurement, options);
            Globals.ProductionData = new ProductionOrder(); // Test-only class, never application constructor.
            Globals.productionData_Cs = new ProductionOrder();
            Globals.Production_State = e_Production_State.Running;
            Globals.CurrentPLCID = 1; Globals.currentPLCStatus = 0;
            Globals.PendingCameraRequests = 0;
            Globals_Database.Dictionary_ProductionCode_Data.Clear();
            Globals_Database.Dictionary_ProductionCode_CameraSub_Data.Clear();
            Globals_Database.Dictionary_ProductionCarton_Data.Clear();
            ClearQueues();
            for (int i = 0; i < fixture.Codes.Length; i++)
            {
                Globals_Database.Dictionary_ProductionCode_Data.Add(fixture.Codes[i], new ProductionCodeData { orderNo = Globals.ProductionData.orderNo, codeID = i, Code = fixture.Codes[i], Main_Camera_Status = "0", Sub_Camera_Status = "0", cartonCode = "0" });
                Globals_Database.Dictionary_ProductionCode_CameraSub_Data.Add(fixture.Codes[i], new ProductionCodeData { orderNo = Globals.ProductionData.orderNo, codeID = i, Code = fixture.Codes[i], Main_Camera_Status = "0", Sub_Camera_Status = "0", cartonCode = "0" });
            }
            for (int i = 0; i <= Fixture.Population / options.CartonPack + 2; i++)
                Globals_Database.Dictionary_ProductionCarton_Data.Add(i, new ProductionCartonData { orderNo = Globals.ProductionData.orderNo, cartonID = i, cartonCode = "QA_CARTON_" + i });
            measurement.Started = Stopwatch.GetTimestamp(); // Exclude fixture dictionary/carton setup.
            measurement.BeginProductionObservation();
            return new FDashboard();
        }
        private static void ClearQueues()
        {
            if (baseline)
            {
                // Actual baseline Queue<T> races can corrupt its internal state,
                // making Clear throw after the measured scenario already failed.
                // All workers have drained: replace only sandbox state between
                // scenarios, retaining the production queue type and failure data.
                foreach (string name in new[] { "Insert_Product_To_Record_Queue", "Insert_Product_To_Record_CS_Queue", "Update_Product_To_SQLite_Queue", "Update_Product_To_Record_Carton_Queue", "Activate_Carton", "aWS_Recive_Datas", "aWS_Send_Datas" })
                {
                    var field = typeof(Globals_Database).GetField(name, BindingFlags.Public | BindingFlags.Static);
                    if (field == null || field.IsInitOnly) throw new InvalidOperationException("Cannot isolate baseline sandbox queue: " + name);
                    field.SetValue(null, Activator.CreateInstance(field.FieldType));
                }
                return;
            }
            Globals_Database.Insert_Product_To_Record_Queue.Clear();
            Globals_Database.Insert_Product_To_Record_CS_Queue.Clear();
            Globals_Database.Update_Product_To_SQLite_Queue.Clear();
            Globals_Database.Update_Product_To_Record_Carton_Queue.Clear();
            Globals_Database.Activate_Carton.Clear();
            Globals_Database.aWS_Recive_Datas.Clear();
            Globals_Database.aWS_Send_Datas.Clear();
        }
        private static object Property(object value, string name)
        { return value.GetType().GetProperty(name).GetValue(value); }
        private static bool Pending()
        { return (bool)typeof(Globals_Database).GetProperty("HasPendingWork").GetValue(null); }
        private static bool TryEditing(e_Production_State expected)
        {
            var type = typeof(Program).Assembly.GetType("MASAN_SERIALIZATION.Views.ProductionInfo.PPOInfo", true);
            return (bool)type.GetMethod("TryEnterEditing", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Activator.CreateInstance(type), new object[] { expected });
        }
        private static void PoFenceTruthTable()
        {
            using (var m = new Measurement("po-transition-fence"))
            {
                var o = new BoundaryOptions { Coverage = "actual extracted TryEnterEditing and HasPendingWork unit regression; not callback performance" };
                Reset(m, o); var errors = new List<string>();
                string[] queues = { "Update_Product_To_SQLite_Queue", "Insert_Product_To_Record_Queue", "Insert_Product_To_Record_CS_Queue", "Update_Product_To_Record_Carton_Queue", "Activate_Carton", "aWS_Recive_Datas", "aWS_Send_Datas" };
                foreach (var state in new[] { e_Production_State.Ready, e_Production_State.Completed })
                {
                    Globals.Production_State = state;
                    if (Pending() || !TryEditing(state) || Globals.Production_State != e_Production_State.Editing) errors.Add("Empty guard must allow " + state);
                    Globals.Production_State = state; Globals.PendingCameraRequests = 1;
                    if (!Pending() || TryEditing(state) || Globals.Production_State != state) errors.Add("Pending worker must fence " + state);
                    Globals.PendingCameraRequests = 0;
                    foreach (string name in queues)
                    {
                        object queue = typeof(Globals_Database).GetField(name).GetValue(null);
                        Type itemType = queue.GetType().GetGenericArguments()[0];
                        queue.GetType().GetMethod("Enqueue").Invoke(queue, new[] { Activator.CreateInstance(itemType) });
                        if (!Pending() || TryEditing(state) || Globals.Production_State != state) errors.Add(name + " must fence " + state);
                        ClearQueues();
                        if (Pending()) errors.Add(name + " remains pending after drain");
                    }
                    if (!TryEditing(state) || Globals.Production_State != e_Production_State.Editing) errors.Add("Guard must be retryable after drain " + state);
                }
                Globals.Production_State = e_Production_State.Running;
                if (TryEditing(e_Production_State.Ready) || Globals.Production_State != e_Production_State.Running) errors.Add("Wrong expected state must not enter Editing");
                if (Globals.ProductionData.orderNo != "QA_SANDBOX_200000") errors.Add("Guard changed PO identity");
                Save(m, o, errors, false);
            }
        }
        private static void OwnershipSnapshots()
        {
            using (var m = new Measurement("po-ownership-snapshots"))
            {
                var o = new BoundaryOptions { Coverage = "actual extracted ownership/snapshot helpers; no persistence consumer/DB" };
                var d = Reset(m, o); var errors = new List<string>();
                Globals.ProductionData.orderNo = "QA_PO_A";
                typeof(Globals_Database).GetMethod("EnqueueCartonActivation").Invoke(null, new object[] { "CARTON_A", "QA_PO_A", "2026-11-27", 24 });
                object activation = Globals_Database.Activate_Carton.First();
                Globals.ProductionData.orderNo = "QA_PO_B"; Globals.ProductionData.productionDate = "2026-11-28"; o.CartonPack = 12;
                if ((string)Property(activation, "orderNo") != "QA_PO_A" || (string)Property(activation, "cartonCode") != "CARTON_A" || (string)Property(activation, "productionDate") != "2026-11-27" || (int)Property(activation, "cartonPack") != 24) errors.Add("Activation reread mutable current PO/config");
                var original = new ProductionCodeData { orderNo = "QA_PO_A", Code = fixture.Codes[9], Main_Camera_Status = "1", Sub_Camera_Status = "0", cartonCode = "CARTON_A", Activate_Datetime = "DATE_A", Production_Datetime = "PROD_A", Sub_Camera_Activate_Datetime = "SUB_A" };
                typeof(FDashboard).GetMethod("Enqueue_Product_To_SQLite", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(d, new object[] { original.Code, original, false, "QA_PO_A" });
                original.orderNo = "QA_PO_B"; original.Main_Camera_Status = "-1"; original.cartonCode = "CARTON_B"; original.Activate_Datetime = "DATE_B"; original.Sub_Camera_Activate_Datetime = "SUB_B";
                var snapshot = Globals_Database.Update_Product_To_SQLite_Queue.First().data;
                if (ReferenceEquals(snapshot, original) || snapshot.orderNo != "QA_PO_A" || snapshot.Main_Camera_Status != "1" || snapshot.cartonCode != "CARTON_A" || snapshot.Activate_Datetime != "DATE_A" || snapshot.Sub_Camera_Activate_Datetime != "SUB_A") errors.Add("SQLite snapshot shares mutable PO/DTO data");
                var carton = new ProductionCartonData { orderNo = "QA_PO_A", cartonID = 7, cartonCode = "CARTON_A", Production_Datetime = "PROD_A" };
                var cartonSnapshot = (ProductionCartonData)typeof(Globals_Database).GetMethod("SnapshotCarton").Invoke(null, new object[] { carton });
                carton.orderNo = "QA_PO_B"; carton.cartonCode = "CARTON_B";
                if (ReferenceEquals(cartonSnapshot, carton) || cartonSnapshot.orderNo != "QA_PO_A" || cartonSnapshot.cartonCode != "CARTON_A" || cartonSnapshot.Production_Datetime != "PROD_A") errors.Add("Carton snapshot shares mutable fields");
                ClearQueues(); Save(m, o, errors, false);
            }
        }
        private static void StaleCallbackOwner()
        {
            using (var m = new Measurement("po-stale-callback-owner"))
            using (var gate = new ManualResetEventSlim(false))
            {
                var o = new BoundaryOptions { Coverage = "actual callbacks capture A; forced pointer swap to B behind test dispatch gate is defense-in-depth, not a permitted production PO transition" };
                var d = Reset(m, o); var errors = new List<string>();
                Globals.ProductionData.orderNo = "QA_PO_A"; QaHooks.DispatchGate = gate;
                Sample main = m.Admit(0, "Main", fixture.Codes[13], Stopwatch.GetTimestamp()); main.ExpectedHmiResults = 0; d.Feed(main);
                Sample sub = m.Admit(1, "Sub", fixture.Codes[13], Stopwatch.GetTimestamp()); sub.ExpectedHmiResults = 0; d.Feed(sub);
                try
                {
                    Globals.Production_State = e_Production_State.Ready;
                    if (!Pending() || TryEditing(e_Production_State.Ready)) errors.Add("Actual guard allowed transition with callback admitted");
                    // Intentionally bypass guard to verify captured owner survives stale worker execution.
                    Globals.ProductionData = new ProductionOrder { orderNo = "QA_PO_B", productionDate = "2026-11-28" };
                    Globals.productionData_Cs = new ProductionOrder { orderNo = "QA_PO_B" };
                    Globals.Production_State = e_Production_State.Running;
                    foreach (var pair in Globals_Database.Dictionary_ProductionCode_Data) pair.Value.orderNo = "QA_PO_B";
                    foreach (var pair in Globals_Database.Dictionary_ProductionCode_CameraSub_Data) pair.Value.orderNo = "QA_PO_B";
                }
                finally { gate.Set(); }
                Drain(m, d); QaHooks.DispatchGate = null;
                foreach (var sample in m.Samples.Values)
                    if (sample.PrePlcCount != 1 || sample.PlcValue != "0" || sample.RecordCount != 1 || sample.RecordStatus != "Error" || sample.RecordOwner != "QA_PO_A") errors.Add("Stale callback did not reject/record original owner exactly once: " + sample.Camera);
                if (Globals_Database.Dictionary_ProductionCode_Data[fixture.Codes[13]].Main_Camera_Status != "0" || Globals_Database.Dictionary_ProductionCode_CameraSub_Data[fixture.Codes[13]].Sub_Camera_Status != "0" || Globals_Database.Update_Product_To_SQLite_Queue.Any() || Globals_Database.Activate_Carton.Any() || Globals.ProductionData.counter.carton_Packing_Count != 0) errors.Add("Stale callback mutated new PO dictionary/activation state");
                if (Globals.ProductionData.counter.totalCount != 0 || Globals.ProductionData.counter.errorCount != 0 || Globals.productionData_Cs.counter.totalCount != 0 || Globals.productionData_Cs.counter.errorCount != 0) errors.Add("Stale callback mutated new PO counters/HMI results");
                if (Globals.PendingCameraRequests != 0) errors.Add("Production pending count did not drain");
                Save(m, o, errors);
            }
        }
        private static Sample Feed(Measurement m, FDashboard dashboard, int id, string camera, string code, long? scheduled = null)
        {
            Sample sample = m.Admit(id, camera, code, scheduled ?? Stopwatch.GetTimestamp());
            dashboard.Feed(sample);
            return sample;
        }
        private static void Drain(Measurement m, FDashboard dashboard)
        {
            if (!m.Wait(120000)) throw new TimeoutException("Outstanding callback workers did not drain; abort process before next scenario");
            dashboard.WaitBackgroundWorker();
        }
        private static Sample Serial(Measurement m, FDashboard d, int id, string camera, string code)
        { var s = Feed(m, d, id, camera, code); Drain(m, d); return s; }
        private static void Check(Sample s, string status, string value, List<string> errors)
        {
            if (s.Status != status || s.PlcValue != value)
                errors.Add("ID=" + s.Id + " " + s.Camera + " expected=" + status + "/" + value + " actual=" + s.Status + "/" + s.PlcValue);
        }
        private static void Save(Measurement m, BoundaryOptions options, IEnumerable<string> errors, bool requirePrePlc = true)
        {
            var errorList = errors.ToList();
            if (!m.Wait(1)) errorList.Add("Undrained at save");
            reports.Add(m.Save(output, options, errorList, requirePrePlc));
            bool failed = !m.Passed;
            if (failed) { failures++; failedScenarios.Add(m.Name); }
            Console.WriteLine(m.Name + ": " + (failed ? "FAIL" : "PASS") + " offered=" + m.Samples.Count);
        }
        private static void Warmup()
        {
            using (var m = new Measurement("warmup"))
            {
                var d = Reset(m, new BoundaryOptions());
                Serial(m, d, 0, "Main", fixture.Codes[1]); Serial(m, d, 1, "Sub", fixture.Codes[1]);
            }
        }
        private static void Classifications()
        {
            using (var m = new Measurement("classifications"))
            {
                var o = new BoundaryOptions(); var d = Reset(m, o); var errors = new List<string>(); int id = 0;
                foreach (var camera in new[] { "Main", "Sub" })
                {
                    Check(Serial(m, d, id++, camera, null), "Error", "0", errors);
                    Check(Serial(m, d, id++, camera, ""), "Error", "0", errors);
                    Check(Serial(m, d, id++, camera, "FAIL"), "ReadFail", "0", errors);
                    Check(Serial(m, d, id++, camera, "NOREAD"), "NotFound", "0", errors);
                    Check(Serial(m, d, id++, camera, "QA_ABSENT"), "NotFound", "0", errors);
                }
                Check(Serial(m, d, id++, "Sub", fixture.Codes[1]), "ReadFail", "0", errors);
                Check(Serial(m, d, id++, "Main", fixture.Codes[1]), "Pass", "1", errors);
                Check(Serial(m, d, id++, "Main", fixture.Codes[1]), "Duplicate", "0", errors);
                o.DuplicateReject = false;
                Check(Serial(m, d, id++, "Main", fixture.Codes[1]), "Duplicate", "1", errors);
                o.DuplicateReject = true;
                Check(Serial(m, d, id++, "Sub", fixture.Codes[1]), "Pass", "1", errors); // carton0 even
                Check(Serial(m, d, id++, "Sub", fixture.Codes[1]), "Duplicate", "0", errors);
                string tokenized = fixture.Codes[0].Replace("\u001D", "<GS>").Replace("\u001E", "<RS>").Replace("\u001F", "<US>");
                Check(Serial(m, d, id++, "Main", tokenized), "Pass", "1", errors);
                Check(Serial(m, d, id++, "Sub", tokenized), "Pass", "1", errors);
                // Real duplicate uses false + populated TResult.data. Service failure is false + null.
                o.CrossPoAllowed = false;
                Check(Serial(m, d, id++, "Main", fixture.Codes[2]), "Duplicate", "0", errors);
                o.DuplicateReject = false;
                Check(Serial(m, d, id++, "Main", fixture.Codes[2]), "Pass", "1", errors);
                o.CrossPoError = true;
                Check(Serial(m, d, id++, "Main", fixture.Codes[3]), "Error", "0", errors);
                o.CrossPoAllowed = true; o.CrossPoError = false; o.DuplicateReject = true;
                o.SendSuccess = false;
                Check(Serial(m, d, id++, "Main", fixture.Codes[4]), "Error", "1", errors);
                Check(Serial(m, d, id++, "Main", "MISSING_SEND_FAIL"), "NotFound", "0", errors);
                Check(Serial(m, d, id++, "Sub", "MISSING_SEND_FAIL"), "NotFound", "0", errors);
                if (Globals_Database.Insert_Product_To_Record_Queue.Last().PLCStatus || Globals_Database.Insert_Product_To_Record_CS_Queue.Last().PLCStatus)
                    errors.Add("Failed PLC send must not be recorded as successful");
                o.SendSuccess = true; o.DuoMode = true;
                Check(Serial(m, d, id++, "Sub", fixture.Codes[4]), "ReadFail", "0", errors);
                Check(Serial(m, d, id++, "Main", fixture.Codes[5]), "Pass", "1", errors);
                Globals.ProductionData.counter.cartonID = 1;
                var duo = Serial(m, d, id++, "Sub", fixture.Codes[5]); Check(duo, "Pass", "2", errors);
                if (duo.PlcAddress != "PLC2/PLC2_Reject_DM_C1") errors.Add("Sub duo route mismatch");
                o.TimeoutChecking = true;
                Check(Serial(m, d, id++, "Main", fixture.Codes[6]), "Pass", "1", errors);
                Check(Serial(m, d, id++, "Sub", fixture.Codes[6]), "Pass", "2", errors);
                if (!baseline)
                {
                    Check(Serial(m, d, id++, "Main", fixture.Codes[7]), "Pass", "1", errors);
                    o.PlcStatus = 5;
                    Check(Serial(m, d, id++, "Sub", fixture.Codes[7]), "Error", "2", errors);
                    o.PlcStatus = 0;
                }
                Check(Serial(m, d, id++, "Main", fixture.Codes[8]), "Pass", "1", errors);
                Globals.ProductionData.counter.cartonID = 0;
                Globals.ProductionData.counter.carton_Packing_Count = o.CartonPack - 1;
                Check(Serial(m, d, id++, "Sub", fixture.Codes[8]), "Pass", "1", errors);
                if (Globals.ProductionData.counter.cartonID != 1 || Globals.ProductionData.counter.carton_Packing_Count != 0) errors.Add("Carton full boundary did not roll to next carton");
                Globals.Production_State = e_Production_State.Pause;
                Check(Serial(m, d, id++, "Sub", fixture.Codes[9]), "Error", "0", errors);
                Globals.Production_State = e_Production_State.Running;
                if (ReferenceEquals(Globals_Database.Dictionary_ProductionCode_Data[fixture.Codes[1]], Globals_Database.Dictionary_ProductionCode_CameraSub_Data[fixture.Codes[1]]))
                    errors.Add("Fixture incorrectly shares Main/Sub DTO reference");
                Save(m, o, errors);
            }
        }
        private static void DuplicateRace()
        {
            using (var m = new Measurement("duplicate-race"))
            {
                var o = new BoundaryOptions { CrossPoDelayMs = 2, PlcSendDelayMs = 2 }; var d = Reset(m, o);
                const int count = 128;
                System.Threading.Tasks.Parallel.For(0, count, i => Feed(m, d, i, "Main", fixture.Codes[42]));
                Drain(m, d);
                var errors = new List<string>();
                if (m.Samples.Values.Count(x => x.Status == "Pass") != 1 || m.Samples.Values.Count(x => x.Status == "Duplicate") != count - 1)
                    errors.Add("Concurrent same-code Main scans must activate exactly once");
                if (Globals.ProductionData.counter.totalCount != count || Globals.ProductionData.counter.passCount != 1)
                    errors.Add("Main counters lost concurrent ingress or duplicate activation");
                Save(m, o, errors);
            }
        }
        private static void Bulk(string name, int count, int rate, BoundaryOptions o, bool sub)
        {
            using (var m = new Measurement(name))
            {
                var d = Reset(m, o);
                if (sub) foreach (string code in fixture.Codes) Globals_Database.Dictionary_ProductionCode_Data[code].Main_Camera_Status = "1";
                long start = Stopwatch.GetTimestamp(); m.Started = start;
                for (int i = 0; i < count; i++)
                {
                    long scheduled = rate == 0 ? start : start + (long)(i * (double)Stopwatch.Frequency / rate);
                    while (Stopwatch.GetTimestamp() < scheduled) { if (Measurement.Ms(scheduled - Stopwatch.GetTimestamp()) > 2) Thread.Sleep(1); else Thread.SpinWait(50); }
                    Feed(m, d, i, sub ? "Sub" : "Main", fixture.Codes[i], scheduled);
                }
                Drain(m, d);
                var errors = new List<string>();
                if (!sub && m.Samples.Values.Any(x => x.Status != "Pass")) errors.Add("Unique valid Main population misclassified");
                if (!sub && (Globals.ProductionData.counter.passCount != count || Globals.ProductionData.counter.totalCount != count)) errors.Add("Main counters do not conserve complete unique ingestion");
                if (!sub && count == Fixture.Population && Globals.Production_State != e_Production_State.Waiting_Stop) errors.Add("Full PO did not transition to Waiting_Stop at exact orderQty");
                if (!sub && (Globals_Database.Insert_Product_To_Record_Queue.Count != count || Globals_Database.Update_Product_To_SQLite_Queue.Count != count)) errors.Add("Production persistence enqueue counts do not conserve Main population");
                if (!sub && !baseline && !m.Samples.Values.OrderBy(x => x.PrePlc).Select(x => x.Id).SequenceEqual(Enumerable.Range(0, count))) errors.Add("Current single-producer Main prePLC order differs from callback arrival order");
                if (sub && m.Samples.Values.Any(x => x.Status != "Pass" && x.Status != "Error")) errors.Add("Unexpected Sub result under busy saturation");
                if (sub && !m.Samples.Values.Any(x => x.Status == "Pass")) errors.Add("Healthy Sub workload produced no successful worker decision");
                if (sub && m.Samples.Values.Any(x => x.Worker > 0 && (x.Status != "Pass" || x.PlcValue == "0"))) errors.Add("Worker-admitted valid Sub must Pass and route to a lane");
                if (sub && m.Samples.Values.Any(x => x.InlineResult && (x.Status != "Error" || x.PlcValue != "0"))) errors.Add("Inline Sub busy decision must Error/reject0");
                if (sub && Globals.productionData_Cs.counter.totalCount != count) errors.Add("Sub ingress count mismatch");
                if (sub && (Globals_Database.Insert_Product_To_Record_CS_Queue.Count != count || Globals.productionData_Cs.counter.passCount != m.Samples.Values.Count(x => x.Status == "Pass") || Globals.productionData_Cs.counter.errorCount != m.Samples.Values.Count(x => x.Status == "Error"))) errors.Add("Sub records/counters do not conserve outcomes");
                Save(m, o, errors);
            }
        }
        private static void BothCameras()
        {
            using (var m = new Measurement("both-cameras-burst"))
            {
                var o = new BoundaryOptions { PlcReadDelayMs = 2 }; var d = Reset(m, o);
                for (int i = 0; i < 500; i++) Globals_Database.Dictionary_ProductionCode_Data[fixture.Codes[i]].Main_Camera_Status = "1";
                System.Threading.Tasks.Parallel.Invoke(
                    () => { for (int i = 0; i < 500; i++) Feed(m, d, i, "Sub", fixture.Codes[i]); },
                    () => { for (int i = 500; i < 1000; i++) Feed(m, d, i, "Main", fixture.Codes[i]); });
                Drain(m, d);
                var errors = new List<string>();
                if (m.Samples.Values.Any(x => x.Camera == "Main" && x.Status != "Pass")) errors.Add("Main changed under two-camera load");
                if (m.Samples.Values.Any(x => x.Camera == "Sub" && x.Status != "Pass" && x.Status != "Error")) errors.Add("Unexpected Sub classification under two-camera load");
                if (!m.Samples.Values.Any(x => x.Camera == "Sub" && x.Status == "Pass")) errors.Add("Two-camera workload has no successful Sub");
                if (m.Samples.Values.Any(x => x.Camera == "Sub" && x.Worker > 0 && (x.Status != "Pass" || x.PlcValue == "0"))) errors.Add("Two-camera admitted Sub must Pass/lane");
                if (!baseline && !m.Samples.Values.Where(x => x.Camera == "Main").OrderBy(x => x.PrePlc).Select(x => x.Id).SequenceEqual(Enumerable.Range(500, 500))) errors.Add("Two-camera single-producer Main order mismatch");
                Save(m, o, errors);
            }
        }
    }
}
