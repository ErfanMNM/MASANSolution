using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CartonRegression
{
    enum e_Production_State { Ready, Saving, Editing, Loading }
    enum e_LogType { DataChange }
    sealed class Counter { public int cartonID; }
    sealed class Production { public string orderNo = "PO-A"; public Counter counter = new Counter(); }
    sealed class User { public string Username = "synthetic-user"; }
    static class Globals
    {
        public static readonly object PoTransitionLock = new object();
        public static e_Production_State Production_State;
        public static Production ProductionData = new Production();
        public static User CurrentUser = new User();
    }

    // The fake dictionary snapshots its values and can align two unguarded readers.
    // This only schedules a legal race; code/owner/target decisions stay extracted.
    sealed class CartonDictionary : Dictionary<int, ProductionCartonData>
    {
        public Barrier UnprotectedReadBarrier;
        public new IEnumerable<ProductionCartonData> Values
        {
            get
            {
                var snapshot = base.Values.Select(value => new ProductionCartonData
                {
                    cartonID = value.cartonID, orderNo = value.orderNo,
                    cartonCode = value.cartonCode, Start_Datetime = value.Start_Datetime
                }).ToArray();
                if (UnprotectedReadBarrier != null && !Monitor.IsEntered(Globals.PoTransitionLock))
                    Program.Check(UnprotectedReadBarrier.SignalAndWait(TimeSpan.FromSeconds(5)), "unguarded-reader barrier timed out");
                return snapshot;
            }
        }
    }
    static partial class Globals_Database
    {
        public static CartonDictionary Dictionary_ProductionCarton_Data = new CartonDictionary();
        public static ConcurrentQueue<ProductionCartonData> Update_Product_To_Record_Carton_Queue = new ConcurrentQueue<ProductionCartonData>();
    }
    sealed class FakeLog
    {
        public Action OnWrite;
        public void WriteLogAsync(string user, e_LogType type, string text, string code)
        {
            Program.Check(!Monitor.IsEntered(Globals.PoTransitionLock), "log invoked while lifecycle lock held");
            OnWrite?.Invoke();
        }
    }
    sealed class FakeList { public readonly FakeItems Items = new FakeItems(); }
    sealed class FakeItems { public void Insert(int index, string text) { Program.Check(!Monitor.IsEntered(Globals.PoTransitionLock), "UI list invoked under lock"); } }

    partial class CartonHarness
    {
        private readonly FakeLog PCLog = new FakeLog();
        private readonly FakeList opLane01 = new FakeList();
        public int Notifications;
        public void SetLogHook(Action callback) { PCLog.OnWrite = callback; }
        public void Scan(bool firstLane, string code)
        {
            if (firstLane) HandScan01_Process(code); else HandScan02_Process(code);
        }
        private void InvokeIfRequired(Action action)
        {
            Program.Check(!Monitor.IsEntered(Globals.PoTransitionLock), "UI dispatcher invoked while lifecycle lock held");
            action();
        }
        private void ShowErrorNotifier(string text, bool modal, int duration)
        {
            Program.Check(!Monitor.IsEntered(Globals.PoTransitionLock), "UI notification invoked while lifecycle lock held");
            Interlocked.Increment(ref Notifications);
        }
    }

    static class Program
    {
        static readonly List<object> Results = new List<object>();
        static int Passed, ExpectedDefects;
        static bool Before;
        public static void Check(bool value, string message) { if (!value) throw new Exception(message); }
        static void Reset(int id = 1)
        {
            Globals.Production_State = e_Production_State.Ready;
            Globals.ProductionData = new Production();
            Globals.ProductionData.counter.cartonID = id;
            Globals_Database.Dictionary_ProductionCarton_Data = new CartonDictionary();
            Globals_Database.Update_Product_To_Record_Carton_Queue = new ConcurrentQueue<ProductionCartonData>();
            foreach (int cartonID in new[] { id, id + 1 })
                Globals_Database.Dictionary_ProductionCarton_Data.Add(cartonID, new ProductionCartonData
                {
                    cartonID = cartonID, orderNo = "PO-A", cartonCode = "0",
                    Start_Datetime = "0", Activate_Datetime = "0", Production_Datetime = "0", Activate_User = "0"
                });
        }
        static void Run(string name, Action action, bool expectedDefect = false)
        {
            action();
            if (expectedDefect) ExpectedDefects++; else Passed++;
            Results.Add(new { name, disposition = expectedDefect ? "expected_defect_reproduced" : "passed" });
            Console.WriteLine((expectedDefect ? "EXPECTED_DEFECT " : "PASS ") + name);
        }
        static void Parity()
        {
            foreach (int id in new[] { 1, 2 }) foreach (bool first in new[] { true, false })
            {
                Reset(id);
                new CartonHarness().Scan(first, "  CARTON-ONE  ");
                int wanted = id + (((first && id % 2 == 0) || (!first && id % 2 != 0)) ? 1 : 0);
                ProductionCartonData record;
                Check(Globals_Database.Update_Product_To_Record_Carton_Queue.TryDequeue(out record), "normal scan missing queue record");
                Check(record.cartonID == wanted && record.cartonCode == "CARTON-ONE" && record.orderNo == "PO-A", "lane/parity/code/owner changed");
                Check(record.Start_Datetime != "0" && Globals_Database.Update_Product_To_Record_Carton_Queue.IsEmpty, "normal scan queue/time invalid");
            }
        }
        static void UniqueCodes()
        {
            Reset(); var harness = new CartonHarness();
            harness.Scan(true, "CODE-1"); harness.Scan(false, "CODE-2");
            Check(Globals_Database.Dictionary_ProductionCarton_Data.Values.Count(x => x.cartonCode != "0") == 2, "two distinct scans not assigned");
            Check(Globals_Database.Update_Product_To_Record_Carton_Queue.Count == 2 && harness.Notifications == 0, "distinct scans queue/notification mismatch");
        }
        static void ConcurrentSameCode(int id)
        {
            int repetitions = Before ? 1 : 64;
            for (int repeat = 0; repeat < repetitions; repeat++)
            {
                Reset(id); var harness = new CartonHarness();
                using (var logBarrier = new Barrier(2)) using (var readBarrier = new Barrier(2))
                {
                    harness.SetLogHook(() => Check(logBarrier.SignalAndWait(TimeSpan.FromSeconds(5)), "scan-admission barrier timed out"));
                    Globals_Database.Dictionary_ProductionCarton_Data.UnprotectedReadBarrier = readBarrier;
                    var first = Task.Run(() => harness.Scan(true, "SAME-CARTON"));
                    var second = Task.Run(() => harness.Scan(false, "SAME-CARTON"));
                    try { Check(Task.WaitAll(new[] { first, second }, TimeSpan.FromSeconds(10)), "concurrent handlers did not complete"); }
                    finally { Globals_Database.Dictionary_ProductionCarton_Data.UnprotectedReadBarrier = null; }
                }
                int assigned = Globals_Database.Dictionary_ProductionCarton_Data.Values.Count(x => x.cartonCode == "SAME-CARTON");
                int queued = Globals_Database.Update_Product_To_Record_Carton_Queue.Count;
                Check(assigned == (Before ? 2 : 1) && queued == assigned, "same code assignment/queue count=" + assigned + "/" + queued);
                Check(harness.Notifications == (Before ? 0 : 1), "duplicate notification changed");
                Check(Globals_Database.Update_Product_To_Record_Carton_Queue.All(x => x.orderNo == "PO-A"), "queued owner changed");
            }
        }
        static void StaleOwner()
        {
            foreach (bool first in new[] { true, false })
            {
                Reset(); var harness = new CartonHarness();
                harness.SetLogHook(() =>
                {
                    lock (Globals.PoTransitionLock)
                    {
                        Globals.ProductionData.orderNo = "PO-B";
                        foreach (var value in Globals_Database.Dictionary_ProductionCarton_Data.Values)
                            Globals_Database.Dictionary_ProductionCarton_Data[value.cartonID].orderNo = "PO-B";
                    }
                });
                harness.Scan(first, "STALE-A");
                Check(Globals_Database.Dictionary_ProductionCarton_Data.Values.All(x => x.cartonCode == "0"), "stale scan mutated PO-B");
                Check(Globals_Database.Update_Product_To_Record_Carton_Queue.IsEmpty, "stale scan queued PO-B record");
            }
        }
        static void TransitionAfterCapture()
        {
            foreach (var state in new[] { e_Production_State.Saving, e_Production_State.Editing, e_Production_State.Loading })
                foreach (bool first in new[] { true, false })
                {
                    Reset(); var harness = new CartonHarness();
                    harness.SetLogHook(() => Globals.Production_State = state);
                    harness.Scan(first, "TRANSITION");
                    Check(Globals_Database.Update_Product_To_Record_Carton_Queue.IsEmpty && Globals_Database.Dictionary_ProductionCarton_Data.Values.All(x => x.cartonCode == "0"), "state changed after capture but assignment proceeded");
                }
        }
        static void TargetGuards()
        {
            foreach (string value in new[] { "occupied", "wrong-owner", "missing" })
            {
                Reset(); var harness = new CartonHarness();
                if (value == "occupied") Globals_Database.Dictionary_ProductionCarton_Data[1].cartonCode = "EXISTING";
                if (value == "wrong-owner") Globals_Database.Dictionary_ProductionCarton_Data[1].orderNo = "OTHER";
                if (value == "missing") Globals_Database.Dictionary_ProductionCarton_Data.Remove(1);
                harness.Scan(true, "NEW-CODE");
                Check(Globals_Database.Update_Product_To_Record_Carton_Queue.IsEmpty, "invalid target queued a record");
            }
        }
        static void ImmutableSnapshot()
        {
            Reset(); new CartonHarness().Scan(true, "OWNER-A-CODE");
            ProductionCartonData record;
            Check(Globals_Database.Update_Product_To_Record_Carton_Queue.TryPeek(out record), "snapshot missing");
            var model = Globals_Database.Dictionary_ProductionCarton_Data[1];
            model.cartonCode = "MUTATED"; model.orderNo = "PO-B"; model.Start_Datetime = "changed";
            Globals.ProductionData.orderNo = "PO-B";
            Check(record.cartonCode == "OWNER-A-CODE" && record.orderNo == "PO-A" && record.Start_Datetime != "changed", "queue snapshot aliases mutable carton");
        }
        public static int Main(string[] args)
        {
            if (args.Length != 2) return 64;
            Before = args[0] == "before";
            try
            {
                Run("lane_parity_and_normal_snapshot", Parity);
                Run("different_codes_two_lanes", UniqueCodes);
                Run("concurrent_same_code_odd", () => ConcurrentSameCode(1), Before);
                Run("concurrent_same_code_even", () => ConcurrentSameCode(2), Before);
                if (!Before)
                {
                    Run("captured_owner_cannot_mutate_new_po", StaleOwner);
                    Run("transition_after_capture_blocks_assignment", TransitionAfterCapture);
                    Run("occupied_wrong_owner_missing_target", TargetGuards);
                    Run("queue_owner_and_values_immutable", ImmutableSnapshot);
                }
                File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new { mode = args[0], passedGroups = Passed, expectedDefects = ExpectedDefects, unexpectedFailures = 0, productConformant = !Before, results = Results }));
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(new { mode = args[0], passedGroups = Passed, expectedDefects = ExpectedDefects, unexpectedFailures = 1, productConformant = false, error = ex.ToString(), results = Results }));
                return 1;
            }
        }
    }
}
