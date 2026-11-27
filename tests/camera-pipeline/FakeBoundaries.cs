// Test-only boundaries. No project/assembly reference to the application; no sockets,
// database providers, WinForms constructors, PLC driver or production initializer.
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Threading;
using CameraPipeline.QA;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;

namespace CameraPipeline.QA
{
    internal sealed class BoundaryOptions
    {
        public int UiDelayMs, CrossPoDelayMs, PlcReadDelayMs, PlcSendDelayMs;
        public int LogDelayMs = 0;
        public bool SendSuccess = true, ReadSuccess = true, CrossPoAllowed = true, CrossPoError, TimeoutChecking;
        public int PlcStatus = 0;
        public int AckTimeoutMs = 50;
        public bool DuplicateReject = true, DuoMode;
        public int CartonPack = 24;
        public string Coverage = "production callback ingress";
    }

    internal static class QaHooks
    {
        public static Measurement Measurement;
        public static BoundaryOptions Options;
        public static readonly ConcurrentDictionary<object, Sample> Sub = new ConcurrentDictionary<object, Sample>();
        public static ManualResetEventSlim DispatchGate;
        public static void BindSub(object request)
        {
            var sample = Measurement.Current.Value;
            if (sample == null || !Sub.TryAdd(request, sample)) Measurement.Failures.Enqueue("Sub request correlation missing/duplicate");
        }
        public static void ProcessMain() { Measurement.ProcessStarted(); }
        public static void ProcessSub(object request)
        {
            Sample sample;
            if (!Sub.TryGetValue(request, out sample)) throw new InvalidOperationException("No Sub request correlation");
            Measurement.Current.Value = sample;
            Measurement.ProcessStarted();
        }
        public static void Result(string status) { Measurement.Result(status); }
        public static void RecordEnqueued(MASAN_SERIALIZATION.ProductionCodeData_Record record)
        {
            var sample = Measurement.Current.Value;
            if (sample == null) { Measurement.Failures.Enqueue("Record has no correlation"); return; }
            Interlocked.Increment(ref sample.RecordCount);
            sample.RecordStatus = record.status.ToString();
            var owner = record.GetType().GetProperty("orderNo");
            sample.RecordOwner = owner == null ? null : (string)owner.GetValue(record);
            if (sample.Status == null) sample.Status = sample.RecordStatus;
        }
        public static void Reset(Measurement measurement, BoundaryOptions options)
        { Sub.Clear(); DispatchGate = null; Measurement = measurement; Options = options; }
        public static void WaitForDispatchGate()
        { var gate = DispatchGate; if (gate != null && !gate.Wait(5000)) throw new TimeoutException("QA dispatch gate not released"); }
        public static void Delay(int milliseconds) { if (milliseconds > 0) Thread.Sleep(milliseconds); }
    }

    internal static class StringExtensions
    {
        // Explicit adapter assumption for well-formed numeric fixture values only.
        public static bool IsNullOrEmpty(this string value) { return string.IsNullOrEmpty(value); }
        public static int ToInt32(this string value) { return int.Parse(value, CultureInfo.InvariantCulture); }
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        { T discarded; while (queue.TryDequeue(out discarded)) { } }
    }
    public sealed class OperateResult<T>
    {
        public bool IsSuccess;
        public T Content;
        public int ErrorCode;
    }
    public sealed class FakeOmron
    {
        public readonly FakePlc plc = new FakePlc();
    }
    public sealed class FakePlc
    {
        public OperateResult<int[]> ReadInt32(string address, int count)
        {
            if (address != "PLC_CurrentID_DM_C2" || count != 2) throw new InvalidOperationException("Unexpected PLC read boundary");
            QaHooks.Delay(QaHooks.Options.PlcReadDelayMs);
            return new OperateResult<int[]> { IsSuccess = QaHooks.Options.ReadSuccess, Content = new[] { 1, QaHooks.Options.PlcStatus }, ErrorCode = -100 };
        }
    }
    public sealed class FakeConsole
    {
        public readonly FakeItems Items = new FakeItems();
        public int SelectedIndex { get; set; }
    }
    public sealed class FakeItems
    {
        private int count;
        public int Count { get { return Volatile.Read(ref count); } }
        // Count only: display rendering/list memory growth intentionally excluded.
        public void Add(string unused) { Interlocked.Increment(ref count); }
    }
    public sealed class FakePageLog
    {
        public void WriteLogAsync(object user, object type, string message, string detail = null)
        { QaHooks.Delay(QaHooks.Options.LogDelayMs); }
    }
}

namespace MASAN_SERIALIZATION
{
    public static class Globals
    {
        public static e_Camera_State CameraMain_State, CameraSub_State;
        public static e_Production_State Production_State;
        public static ProductionOrder ProductionData, productionData_Cs;
        public static readonly FakeUser CurrentUser = new FakeUser();
        public static int CurrentPLCID = 1, currentPLCStatus = 0;
        public static int PendingCameraRequests;
        public static readonly object PoTransitionLock = new object();
        public static string Canhbao;
    }
    public sealed class FakeUser { public string Username = "QA_SANDBOX"; }
}

namespace MASAN_SERIALIZATION.Production
{
    public partial class ProductionOrder
    {
        public Product_Counter counter = new Product_Counter();
        public string orderNo = "QA_SANDBOX_200000", productionDate = "2026-11-27", orderQty = "200000";
        public readonly FakeCrossPo getDataPO = new FakeCrossPo();
    }
    public sealed class FakeCrossPo
    {
        public TResult CheckCodeActivatedInOtherPO(string code, string currentOrderNo)
        {
            QaHooks.Delay(QaHooks.Options.CrossPoDelayMs);
            DataTable table = null;
            if (!QaHooks.Options.CrossPoAllowed && !QaHooks.Options.CrossPoError)
            { table = new DataTable(); table.Columns.Add("Code"); table.Rows.Add(code); }
            return new TResult(QaHooks.Options.CrossPoAllowed && !QaHooks.Options.CrossPoError, "QA injected cross-PO boundary", 0, table);
        }
    }
}

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public enum enumClient { CONNECTED, DISCONNECTED, RECEIVED, RECONNECT }
    internal static class AppConfigs
    {
        public static FakeConfig Current = new FakeConfig();
    }
    internal sealed class FakeConfig
    {
        public bool CameraMain_DuplicateReject_Enabled { get { return QaHooks.Options.DuplicateReject; } }
        public bool PLC_Duo_Mode { get { return QaHooks.Options.DuoMode; } }
        public bool CameraSub_Timeout_Enabled { get { return QaHooks.Options.TimeoutChecking; } }
        public int CameraSub_Timeout_Ms { get { return QaHooks.Options.AckTimeoutMs; } }
        public int cartonPack { get { return QaHooks.Options.CartonPack; } }
    }
    internal static class PLCAddress { public static string Get(string key) { return key; } }

    // This shim preserves real ThreadPool Task.Run; wraps ONLY timing/error collection.
    internal sealed class Task
    {
        private readonly System.Threading.Tasks.Task task;
        private Task(System.Threading.Tasks.Task task) { this.task = task; }
        public bool IsFaulted { get { return task.IsFaulted; } }
        public AggregateException Exception { get { return task.Exception; } }
        public static Task CompletedTask { get { return new Task(System.Threading.Tasks.Task.CompletedTask); } }
        public static Task Run(Action action)
        {
            var measurement = QaHooks.Measurement;
            var sample = measurement.Current.Value;
            return new Task(System.Threading.Tasks.Task.Run(() => {
                measurement.WorkerStart(sample);
                try { QaHooks.WaitForDispatchGate(); action(); measurement.Complete(sample); }
                catch (Exception e) { measurement.Complete(sample, e); throw; }
            }));
        }
        public Task ContinueWith(Action<Task> action, CancellationToken cancellation, System.Threading.Tasks.TaskContinuationOptions options, System.Threading.Tasks.TaskScheduler scheduler)
        {
            var measurement = QaHooks.Measurement;
            var sample = measurement.Current.Value;
            return new Task(task.ContinueWith(previous => {
                measurement.WorkerStart(sample);
                try { QaHooks.WaitForDispatchGate(); action(new Task(previous)); measurement.Complete(sample); }
                catch (Exception e) { measurement.Complete(sample, e); throw; }
            }, cancellation, options, scheduler));
        }
    }
    public partial class FDashboard
    {
        private readonly BackgroundWorker subpr = new BackgroundWorker();
        private readonly FakeConsole ipConsole = new FakeConsole();
        private readonly FakeOmron OMRON_PLC = new FakeOmron();
        private static readonly FakePageLog DashboardPageLog = new FakePageLog();
        private readonly object fakeUiLock = new object();

        public FDashboard()
        {
            subpr.DoWork += (sender, args) => {
                var request = (CameraSubRequest)args.Argument;
                Sample sample;
                if (!QaHooks.Sub.TryGetValue(request, out sample)) throw new InvalidOperationException("Missing BGW sample");
                var measurement = QaHooks.Measurement;
                measurement.WorkerStart(sample);
                try { subpr_DoWork(sender, args); measurement.Complete(sample); }
                catch (Exception error) { measurement.Complete(sample, error); }
            };
            subpr.RunWorkerCompleted += (sender, args) => { if (args.Error != null) QaHooks.Measurement.Failures.Enqueue(args.Error.ToString()); };
        }
        internal void Feed(Sample sample)
        {
            try
            {
                if (sample.Camera == "Main") CameraMain_ClientCallBack(enumClient.RECEIVED, sample.Code);
                else CameraSub_ClientCallBack(enumClient.RECEIVED, sample.Code);
                // Busy/state reject completes in callback. Async workers complete via shim/BGW.
                if (sample.InlineResult) QaHooks.Measurement.Complete(sample);
            }
            catch (Exception error) { QaHooks.Measurement.Complete(sample, error); }
            finally { sample.CallbackReturn = System.Diagnostics.Stopwatch.GetTimestamp(); QaHooks.Measurement.ObservePending(); QaHooks.Measurement.Current.Value = null; }
        }
        public void WaitBackgroundWorker()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (subpr.IsBusy && sw.ElapsedMilliseconds < 5000) Thread.Sleep(1);
            if (subpr.IsBusy) throw new InvalidOperationException("BackgroundWorker failed to drain");
        }
        private void InvokeIfRequired(Action action)
        {
            lock (fakeUiLock) { QaHooks.Delay(QaHooks.Options.UiDelayMs); action(); }
        }
        private string ExceptionToJson(Exception error) { return error.GetType().Name + ": " + error.Message; }
        public bool Send_To_PLC(string address, string value)
        { QaHooks.Measurement.BeforePlc(address, value); QaHooks.Delay(QaHooks.Options.PlcSendDelayMs); return QaHooks.Options.SendSuccess; }
        public bool Send_To_PLC_2(string address, string value)
        { QaHooks.Measurement.BeforePlc("PLC2/" + address, value); QaHooks.Delay(QaHooks.Options.PlcSendDelayMs); return QaHooks.Options.SendSuccess; }
    }
}
