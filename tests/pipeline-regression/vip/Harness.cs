using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace VipRegression
{
    // ProcessScannedCode and btnfind_Click are extracted verbatim. The original
    // Task.Run is used; all UI/data/global types below are explicitly safe fakes.
    internal sealed partial class CheckVIP
    {
        private int scan;
        internal int Gate { get { return Volatile.Read(ref scan); } }
        internal readonly TextControl ipCode = new TextControl();
        internal readonly TextControl opCodeInfo = new TextControl();
        internal readonly TextControl opCaseCode = new TextControl();
        internal readonly FakeGrid uiDataGridView1 = new FakeGrid();
        internal void ClickFind() { btnfind_Click(this, EventArgs.Empty); }

        private void InvokeIfRequired(Action action)
        {
            var state = State.Current;
            int number = Interlocked.Increment(ref state.UiCalls);
            InvokeFake(action, number == 1 ? "initial-ui" : "clear-ui");
            if (number == 1 && state.BlockInitialUi)
            {
                state.InitialUiEntered.Set();
                Check.That(state.ReleaseInitialUi.Wait(5000), "Initial UI gate timed out.");
            }
        }

        private void Invoke(Action action) { InvokeFake(action, "result-ui"); }
        private void InvokeFake(Action action, string stage)
        {
            State.Current.Touch(stage);
            FakeUi.Depth++;
            try { action(); }
            finally { FakeUi.Depth--; }
        }
    }

    internal static class Check
    {
        internal static void That(bool value, string message)
        { if (!value) throw new InvalidOperationException("ASSERTION: " + message); }
    }

    internal static class FakeUi { [ThreadStatic] internal static int Depth; }
    internal sealed class TextControl { internal string Text = ""; }
    internal sealed class FakeGrid
    {
        internal object DataSource;
        internal int ClearCount;
        internal void ClearAll()
        {
            Check.That(FakeUi.Depth > 0, "Grid ClearAll must run inside fake UI invocation.");
            State.Current.Touch("clear");
            ClearCount++;
            DataSource = null;
        }
    }

    internal sealed class TResult
    {
        internal bool issuccess;
        internal DataTable data;
    }

    internal static class Globals
    {
        internal static readonly object PoTransitionLock = new object();
        internal static readonly FakeProduction ProductionData = new FakeProduction();
    }

    internal sealed class FakeProduction
    {
        internal string orderNo = "PO-A";
        internal readonly FakeData getDataPO = new FakeData();
    }

    internal sealed class FakeData
    {
        internal TResult getCodeInfo(string code, string order)
        {
            var state = State.Current;
            state.CodeArguments.Add(code + "|" + order);
            state.Touch("lookup");
            return new TResult { issuccess = !state.NotFound, data = state.CodeData };
        }

        internal TResult getCodeInfoWithCartonCode(string order, string carton)
        {
            var state = State.Current;
            state.CartonArguments.Add(order + "|" + carton);
            state.Touch("carton-lookup");
            return new TResult { issuccess = true, data = state.CartonData };
        }
    }

    internal sealed class State
    {
        internal static State Current;
        internal CheckVIP Owner = new CheckVIP();
        internal bool NotFound;
        internal bool BlockInitialUi;
        internal string ThrowAt;
        internal readonly Exception Injected = new ApplicationException("synthetic VIP failure");
        internal readonly ManualResetEventSlim InitialUiEntered = new ManualResetEventSlim();
        internal readonly ManualResetEventSlim ReleaseInitialUi = new ManualResetEventSlim();
        internal readonly List<string> CodeArguments = new List<string>();
        internal readonly List<string> CartonArguments = new List<string>();
        internal readonly DataTable CodeData = new DataTable();
        internal readonly DataTable CartonData = new DataTable();
        internal int UiCalls;

        internal State()
        {
            CodeData.Columns.Add("Code"); CodeData.Columns.Add("Status");
            CodeData.Columns.Add("ActivateDate"); CodeData.Columns.Add("cartonCode");
            CodeData.Rows.Add("CODE-A", "1", "synthetic-time", "CARTON-A");
        }

        internal void Touch(string stage)
        {
            Check.That(!Monitor.IsEntered(Globals.PoTransitionLock), "UI/DB under lifecycle lock at " + stage);
            Check.That(Owner.Gate == 1, "Gate not acquired at " + stage);
            if (ThrowAt == stage) throw Injected;
        }

        internal void WaitIdle()
        { Check.That(SpinWait.SpinUntil(() => Owner.Gate == 0, 5000), "Worker did not release gate."); }

        internal void AssertSuccess()
        {
            Check.That(Owner.opCodeInfo.Text == "MÃ: CODE-A | Trạng thái :1 | Thời gian kích hoạt : synthetic-time", "Success text changed.");
            Check.That(Owner.opCaseCode.Text == "MÃ THÙNG: CARTON-A", "Carton text changed.");
            Check.That(ReferenceEquals(Owner.uiDataGridView1.DataSource, CartonData), "Carton result not displayed.");
        }
    }

    internal static class Program
    {
        private static State NewState()
        {
            var state = new State(); State.Current = state;
            lock (Globals.PoTransitionLock) Globals.ProductionData.orderNo = "PO-A";
            return state;
        }

        private static void RaceAndCapture()
        {
            var state = NewState(); state.BlockInitialUi = true;
            Exception firstError = null, secondError = null;
            var first = new Thread(() => { try { state.Owner.ProcessScannedCode("  CODE-A  "); } catch (Exception ex) { firstError = ex; } });
            first.IsBackground = true; first.Start();
            bool reached = state.InitialUiEntered.Wait(5000);
            if (!reached) { state.ReleaseInitialUi.Set(); throw new InvalidOperationException("First callback did not reach UI gate."); }
            bool secondReturned;
            int callsBeforeRelease;
            var second = new Thread(() => { try { state.Owner.ProcessScannedCode(" CODE-B "); } catch (Exception ex) { secondError = ex; } });
            second.IsBackground = true;
            try
            {
                Check.That(state.Owner.Gate == 1, "Gate must be acquired before Task.Run.");
                lock (Globals.PoTransitionLock) Globals.ProductionData.orderNo = "PO-B";
                state.Owner.ipCode.Text = "OVERWRITTEN-AFTER-CAPTURE";
                second.Start(); secondReturned = second.Join(2000);
                callsBeforeRelease = Volatile.Read(ref state.UiCalls);
            }
            finally { state.ReleaseInitialUi.Set(); }
            Check.That(first.Join(5000), "First callback stuck.");
            Check.That(second.Join(5000), "Second callback stuck.");
            state.WaitIdle();
            Check.That(firstError == null && secondError == null, "Callback threw unexpectedly.");
            Check.That(secondReturned && callsBeforeRelease == 1, "Busy callback must return before first UI/dispatch is released.");
            Check.That(state.CodeArguments.SequenceEqual(new[] { "CODE-A|PO-A" }), "Must accept one trimmed captured-code lookup only.");
            Check.That(state.CartonArguments.SequenceEqual(new[] { "PO-A|CARTON-A" }), "Second lookup lost captured PO.");
            state.AssertSuccess();
        }

        private static void ThrowThenRetry(string stage)
        {
            var state = NewState(); state.ThrowAt = stage;
            Exception observed = null;
            try { state.Owner.ProcessScannedCode(" CODE-A "); }
            catch (Exception error) { observed = error; }
            state.WaitIdle();
            if (stage == "initial-ui") Check.That(ReferenceEquals(observed, state.Injected), "Initial UI exception changed.");
            else Check.That(observed == null, "Worker exception changed synchronous callback behavior.");
            state.ThrowAt = null;
            // The second invocation must be admitted and complete after any failure.
            state.Owner.ProcessScannedCode(" CODE-A "); state.WaitIdle();
            state.AssertSuccess();
            Check.That(state.CodeArguments.Last() == "CODE-A|PO-A", "Retry used wrong context.");
        }

        private static void NotFound()
        {
            var state = NewState(); state.NotFound = true;
            state.Owner.ProcessScannedCode(" CODE-A "); state.WaitIdle();
            Check.That(state.CodeArguments.SequenceEqual(new[] { "CODE-A|PO-A" }), "Notfound lookup changed.");
            Check.That(state.CartonArguments.Count == 0 && state.Owner.uiDataGridView1.ClearCount == 1, "Notfound branch changed.");
            Check.That(state.Owner.opCodeInfo.Text == "Không tìm thấy code CODE-A trong PO PO-A" && state.Owner.opCaseCode.Text == "", "Notfound text changed.");
        }

        private static void ButtonDelegate()
        {
            var state = NewState(); state.Owner.ipCode.Text = " CODE-A ";
            state.Owner.ClickFind(); state.WaitIdle(); state.AssertSuccess();
            Check.That(state.CodeArguments.SequenceEqual(new[] { "CODE-A|PO-A" }), "Button did not use shared captured lookup.");
        }

        private static int Main(string[] args)
        {
            if (args.Length != 1) return 2;
            var tests = new KeyValuePair<string, Action>[] {
                new KeyValuePair<string, Action>("busy-before-dispatch-trim-and-captured-po", RaceAndCapture),
                new KeyValuePair<string, Action>("first-lookup-throw-retry", () => ThrowThenRetry("lookup")),
                new KeyValuePair<string, Action>("carton-lookup-throw-retry", () => ThrowThenRetry("carton-lookup")),
                new KeyValuePair<string, Action>("grid-ui-throw-retry", () => ThrowThenRetry("clear")),
                new KeyValuePair<string, Action>("result-ui-throw-retry", () => ThrowThenRetry("result-ui")),
                new KeyValuePair<string, Action>("initial-ui-throw-retry", () => ThrowThenRetry("initial-ui")),
                new KeyValuePair<string, Action>("notfound-preserves-message", NotFound),
                new KeyValuePair<string, Action>("manual-button-delegates", ButtonDelegate)
            };
            var results = new List<Dictionary<string, object>>();
            foreach (var test in tests)
            {
                string error = null;
                try { test.Value(); } catch (Exception ex) { error = ex.ToString(); }
                results.Add(new Dictionary<string, object> { { "name", test.Key }, { "passed", error == null }, { "error", error } });
                Console.WriteLine((error == null ? "PASS " : "FAIL ") + test.Key);
                if (error != null) Console.WriteLine(error);
                // Avoid switching global fakes while a failed worker remains active.
                if (State.Current != null && State.Current.Owner.Gate != 0) break;
            }
            int passed = results.Count(r => (bool)r["passed"]);
            var report = new Dictionary<string, object> { { "case_count", results.Count }, { "passed", passed },
                { "failed", results.Count - passed }, { "cases", results }, { "utc", DateTime.UtcNow.ToString("o") } };
            File.WriteAllText(args[0], new JavaScriptSerializer().Serialize(report));
            return results.Count == tests.Length && passed == tests.Length ? 0 : 1;
        }
    }
}
