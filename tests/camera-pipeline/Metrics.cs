using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace CameraPipeline.QA
{
    internal static class Json
    {
        public static void Write(string path, object value)
        {
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            File.WriteAllText(path, serializer.Serialize(value), new UTF8Encoding(false));
        }
    }

    internal sealed class Sample
    {
        public int Id;
        public string Camera, Code, Status, PlcValue, PlcAddress, Error, RecordOwner, RecordStatus;
        public long Scheduled, Ingress, Worker, ProcessStart, PrePlc, CallbackReturn, Terminal;
        public int PrePlcCount, ResultCount, WorkerCount;
        public int CallbackThread;
        public bool InlineResult;
        public int RecordCount;
        public int ExpectedHmiResults = 1;
    }

    internal sealed class Measurement : IDisposable
    {
        public readonly ConcurrentDictionary<int, Sample> Samples = new ConcurrentDictionary<int, Sample>();
        public readonly ConcurrentQueue<string> Failures = new ConcurrentQueue<string>();
        public readonly AsyncLocal<Sample> Current = new AsyncLocal<Sample>();
        public readonly string Name;
        public long Started = Stopwatch.GetTimestamp(), Finished;
        public bool Passed { get; private set; }
        private int outstanding, peakOutstanding;
        private long peakWorkingSet, peakManaged;
        private long sampledProductionPendingMax;
        private volatile bool observeProduction;
        private readonly object queueMetricsLock = new object();
        private readonly Dictionary<string, int> queueMax = new Dictionary<string, int>();
        private readonly Timer memoryTimer;
        private readonly object samplingStopLock = new object();
        private bool samplingStopped;
        private readonly int gc0 = GC.CollectionCount(0), gc1 = GC.CollectionCount(1), gc2 = GC.CollectionCount(2);
        private readonly Process process = Process.GetCurrentProcess();
        public Measurement(string name)
        {
            Name = name;
            ObserveMemory(null);
            memoryTimer = new Timer(ObserveMemory, null, 100, 100);
        }
        public Sample Admit(int id, string camera, string code, long scheduled)
        {
            var sample = new Sample { Id = id, Camera = camera, Code = code, Scheduled = scheduled, Ingress = Stopwatch.GetTimestamp(), CallbackThread = Thread.CurrentThread.ManagedThreadId };
            if (!Samples.TryAdd(id, sample)) throw new InvalidOperationException("Duplicate offered ID");
            int pending = Interlocked.Increment(ref outstanding);
            int old;
            do { old = peakOutstanding; if (pending <= old) break; }
            while (Interlocked.CompareExchange(ref peakOutstanding, pending, old) != old);
            Current.Value = sample;
            return sample;
        }
        public void WorkerStart(Sample sample)
        {
            if (sample == null) { Failures.Enqueue("Worker has no event correlation"); return; }
            Interlocked.Increment(ref sample.WorkerCount);
            Interlocked.CompareExchange(ref sample.Worker, Stopwatch.GetTimestamp(), 0);
            Current.Value = sample;
        }
        public void BeforePlc(string address, string value)
        {
            var s = Current.Value;
            if (s == null) { Failures.Enqueue("PLC send has no event correlation"); return; }
            Interlocked.Increment(ref s.PrePlcCount);
            if (Interlocked.CompareExchange(ref s.PrePlc, Stopwatch.GetTimestamp(), 0) == 0)
            { s.PlcAddress = address; s.PlcValue = value; }
        }
        public void ProcessStarted()
        {
            var s = Current.Value;
            if (s == null) { Failures.Enqueue("Processing has no event correlation"); return; }
            Interlocked.CompareExchange(ref s.ProcessStart, Stopwatch.GetTimestamp(), 0);
        }
        public void Result(string status)
        {
            var s = Current.Value;
            if (s == null) { Failures.Enqueue("Result has no event correlation"); return; }
            s.Status = status;
            if (s.CallbackThread == Thread.CurrentThread.ManagedThreadId && s.CallbackReturn == 0) s.InlineResult = true;
            Interlocked.Increment(ref s.ResultCount);
        }
        public void Complete(Sample s, Exception error = null)
        {
            if (s == null) { Failures.Enqueue("Completion has no event correlation"); return; }
            if (error != null) { s.Error = error.GetType().Name + ": " + error.Message; Failures.Enqueue(s.Error); }
            if (Interlocked.CompareExchange(ref s.Terminal, Stopwatch.GetTimestamp(), 0) == 0)
                Interlocked.Decrement(ref outstanding);
        }
        public bool Wait(int timeoutMs)
        {
            var sw = Stopwatch.StartNew();
            while (Volatile.Read(ref outstanding) != 0 && sw.ElapsedMilliseconds < timeoutMs) Thread.Sleep(2);
            Finished = Stopwatch.GetTimestamp();
            ObserveMemory(null);
            return Volatile.Read(ref outstanding) == 0;
        }
        public static double Ms(long ticks) { return ticks * 1000d / Stopwatch.Frequency; }
        public void BeginProductionObservation()
        { observeProduction = true; ObserveMemory(null); }
        public void ObservePending()
        { if (observeProduction) Max(ref sampledProductionPendingMax, Volatile.Read(ref MASAN_SERIALIZATION.Globals.PendingCameraRequests)); }
        private void ObserveMemory(object unused)
        {
            try
            {
                process.Refresh(); Max(ref peakWorkingSet, process.WorkingSet64); Max(ref peakManaged, GC.GetTotalMemory(false));
                ObservePending();
                if (observeProduction)
                {
                    lock (queueMetricsLock)
                    {
                        QueueMax("sqlite", MASAN_SERIALIZATION.Globals_Database.Update_Product_To_SQLite_Queue.Count);
                        QueueMax("mainRecords", MASAN_SERIALIZATION.Globals_Database.Insert_Product_To_Record_Queue.Count);
                        QueueMax("subRecords", MASAN_SERIALIZATION.Globals_Database.Insert_Product_To_Record_CS_Queue.Count);
                        QueueMax("cartonRecords", MASAN_SERIALIZATION.Globals_Database.Update_Product_To_Record_Carton_Queue.Count);
                        QueueMax("cartonActivation", MASAN_SERIALIZATION.Globals_Database.Activate_Carton.Count);
                        QueueMax("awsReceived", MASAN_SERIALIZATION.Globals_Database.aWS_Recive_Datas.Count);
                        QueueMax("awsSend", MASAN_SERIALIZATION.Globals_Database.aWS_Send_Datas.Count);
                    }
                }
            }
            catch (InvalidOperationException) { }
        }
        // Called by the scenario owner, never by the timer callback. Wait until
        // queued samples finish before saving peaks or disposing their Process.
        private void StopSampling()
        {
            lock (samplingStopLock)
            {
                if (samplingStopped) return;
                using (var completed = new ManualResetEvent(false))
                {
                    if (memoryTimer.Dispose(completed)) completed.WaitOne();
                }
                samplingStopped = true;
            }
        }
        private void QueueMax(string key, int value)
        { int previous; queueMax.TryGetValue(key, out previous); queueMax[key] = Math.Max(previous, value); }
        private static void Max(ref long field, long candidate)
        { long old; do { old = Interlocked.Read(ref field); if (candidate <= old) return; } while (Interlocked.CompareExchange(ref field, candidate, old) != old); }
        private static object Distribution(IEnumerable<double> values)
        {
            double[] v = values.OrderBy(x => x).ToArray();
            Func<double, double?> p = q => v.Length == 0 ? (double?)null : v[Math.Max(0, (int)Math.Ceiling(q * v.Length) - 1)];
            return new { count = v.Length, p50 = p(.50), p95 = p(.95), p99 = p(.99), max = p(1) };
        }
        private static object Timing(Sample[] s)
        {
            return new {
                schedulerLatenessMs = Distribution(s.Select(x => Ms(x.Ingress - x.Scheduled))),
                callbackWallMs = Distribution(s.Where(x => x.CallbackReturn > 0).Select(x => Ms(x.CallbackReturn - x.Ingress))),
                schedulingWaitMs = Distribution(s.Where(x => x.Worker > 0).Select(x => Ms(x.Worker - x.Ingress))),
                serializationWaitMs = Distribution(s.Where(x => x.ProcessStart > 0 && x.Worker > 0).Select(x => Ms(x.ProcessStart - x.Worker))),
                processToPrePlcMs = Distribution(s.Where(x => x.ProcessStart > 0 && x.PrePlc > 0).Select(x => Ms(x.PrePlc - x.ProcessStart))),
                ingressToPrePlcMs = Distribution(s.Where(x => x.PrePlc > 0).Select(x => Ms(x.PrePlc - x.Ingress))),
                scheduledToPrePlcMs = Distribution(s.Where(x => x.PrePlc > 0).Select(x => Ms(x.PrePlc - x.Scheduled))),
                ingressToTerminalMs = Distribution(s.Where(x => x.Terminal > 0).Select(x => Ms(x.Terminal - x.Ingress)))
            };
        }
        public object Save(string root, object settings, IEnumerable<string> assertions, bool requirePrePlc)
        {
            StopSampling();
            ObserveMemory(null);
            Dictionary<string, int> queuePeaks;
            lock (queueMetricsLock) queuePeaks = new Dictionary<string, int>(queueMax);
            var samples = Samples.Values.OrderBy(x => x.Id).ToArray();
            var assertionsArray = assertions.ToArray();
            int missing = samples.Count(x => x.Terminal == 0 || (x.ExpectedHmiResults >= 0 && x.ResultCount != x.ExpectedHmiResults) || x.RecordCount != 1 || (requirePrePlc && x.PrePlcCount == 0));
            int repeated = samples.Count(x => x.PrePlcCount > 1 || x.ResultCount > 1 || x.RecordCount > 1 || x.WorkerCount > 1);
            var violations = assertionsArray.Concat(Failures).ToList();
            if (missing != 0) violations.Add("Missing terminal/result/prePLC events: " + missing);
            if (repeated != 0) violations.Add("Repeated result/prePLC/worker events: " + repeated);
            if (samples.Any(x => x.Worker > 0 && x.Worker < x.Ingress || x.PrePlc > 0 && x.PrePlc < x.Ingress || x.Terminal > 0 && x.Terminal < x.PrePlc))
                violations.Add("Nonmonotonic event timestamps");
            Passed = violations.Count == 0;
            string path = Path.Combine(root, Name);
            Directory.CreateDirectory(path);
            using (var w = new StreamWriter(Path.Combine(path, "events.csv"), false, new UTF8Encoding(false)))
            {
                w.WriteLine("id,camera,status,plcValue,plcAddress,scheduledTicks,ingressTicks,workerTicks,processStartTicks,prePlcTicks,callbackReturnTicks,terminalTicks,prePlcCount,resultCount,workerCount,recordCount,recordStatus,recordOwner,error");
                foreach (var x in samples)
                    w.WriteLine(string.Join(",", new[] { x.Id.ToString(), x.Camera, x.Status, x.PlcValue, x.PlcAddress,
                        x.Scheduled.ToString(), x.Ingress.ToString(), x.Worker.ToString(), x.ProcessStart.ToString(), x.PrePlc.ToString(),
                        x.CallbackReturn.ToString(), x.Terminal.ToString(), x.PrePlcCount.ToString(), x.ResultCount.ToString(), x.WorkerCount.ToString(), x.RecordCount.ToString(), x.RecordStatus, Quote(x.RecordOwner), Quote(x.Error) }));
            }
            long lastOffer = samples.Length == 0 ? Started : samples.Max(x => x.Ingress);
            long lastPre = samples.Where(x => x.PrePlc > 0).Select(x => x.PrePlc).DefaultIfEmpty(Started).Max();
            var result = new {
                scenario = Name, settings, passed = violations.Count == 0, failures = violations,
                offered = samples.Length, callbackReturned = samples.Count(x => x.CallbackReturn > 0),
                prePlc = samples.Count(x => x.PrePlc > 0), terminal = samples.Count(x => x.Terminal > 0),
                recordEvents = samples.Sum(x => x.RecordCount), hmiResultEvents = samples.Sum(x => x.ResultCount),
                missing, repeated, exceptions = samples.Count(x => x.Error != null),
                offeredEventsPerSecond = samples.Length / Math.Max(.000001, Ms(lastOffer - Started) / 1000),
                prePlcEventsPerSecond = samples.Count(x => x.PrePlc > 0) / Math.Max(.000001, Ms(lastPre - Started) / 1000),
                terminalEventsPerSecond = samples.Count(x => x.Terminal > 0) / Math.Max(.000001, Ms(Finished - Started) / 1000),
                offerWindowMs = Ms(lastOffer - Started), drainMs = Ms(Finished - lastOffer),
                peakOutstanding, peakWorkingSetBytes = peakWorkingSet, peakManagedBytes = peakManaged,
                sampledProductionPendingMax, sampledPersistenceQueueMaxDepth = queuePeaks,
                gcCollections = new[] { GC.CollectionCount(0) - gc0, GC.CollectionCount(1) - gc1, GC.CollectionCount(2) - gc2 },
                all = Timing(samples), byCamera = samples.GroupBy(x => x.Camera).ToDictionary(x => x.Key, x => Timing(x.ToArray())),
                byOutcome = samples.GroupBy(x => x.Camera + "/" + (x.Status ?? "missing")).ToDictionary(x => x.Key, x => new { count = x.Count(), timing = Timing(x.ToArray()) }),
                observedPrePlcArrivalInversions = samples.Where(x => x.PrePlc > 0).GroupBy(x => x.Camera).ToDictionary(x => x.Key, x => Inversions(x.OrderBy(y => y.PrePlc).Select(y => y.Ingress))),
                note = "Outstanding is callback ingress to worker/busy-branch terminal. Production pending is sampled at callback return/100ms; persistence queue depths at 100ms/final drain and retained (no disk consumer). Baseline has no production pending counter, so that sampled field is unavailable/0. Scheduling wait is actual dispatch. No product latency SLA asserted."
            };
            Json.Write(Path.Combine(path, "summary.json"), result);
            return result;
        }
        private static string Quote(string value) { return "\"" + (value ?? "").Replace("\"", "\"\"") + "\""; }
        private static int Inversions(IEnumerable<long> ingressTicks)
        { long highest = -1; int inversions = 0; foreach (long tick in ingressTicks) { if (tick < highest) inversions++; highest = Math.Max(highest, tick); } return inversions; }
        public void Dispose() { StopSampling(); process.Dispose(); }
    }
}
