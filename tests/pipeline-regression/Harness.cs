using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace PipelineRenderRegression
{
    // Only the extracted RenderOrderInfo method is production code. Every other
    // type in this assembly is an explicit fake; no app assembly is referenced.
    internal sealed partial class PPOInfo
    {
        private bool IsRender = false;
        internal bool BusyForTest { get { return IsRender; } }
        internal readonly FakeText ipOrderNO = new FakeText { Text = "SYNTHETIC-PO-V07" };
        internal readonly FakeDate ipProductionDate = new FakeDate();
        internal readonly FakeLogger _pageLogger = new FakeLogger();

        private void InvokeIfRequired(Action action)
        {
            CaseContext.Current.Touch("invoke");
            action();
        }

        private void ShowErrorDialog(string message)
        {
            CaseContext.Current.Touch("dialog");
            Check.That(message == "Lỗi PP06: synthetic MES unavailable", "Error dialog changed.");
        }

        private void ValidateDatabaseFile(DataTable table)
        {
            CaseContext.Current.Touch("validate");
            Check.That(ReferenceEquals(table, CaseContext.Current.OrderResult.data), "Validation data changed.");
        }

        private TResult GetCodeCount()
        {
            CaseContext.Current.Touch("count");
            return CaseContext.Current.CountResult;
        }

        private void UpdateOrderInfoUI(TResult order, TResult count)
        {
            CaseContext.Current.Touch("update");
            Check.That(ReferenceEquals(order, CaseContext.Current.OrderResult), "Order result changed.");
            Check.That(ReferenceEquals(count, CaseContext.Current.CountResult), "Count result changed.");
        }

        private void LoadCountersAsync() { CaseContext.Current.Touch("load-counters"); }
    }

    internal static class Check
    {
        internal static void That(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("ASSERTION: " + message);
        }
    }

    internal sealed class TResult
    {
        internal bool issuccess = true;
        internal string message = "synthetic MES unavailable";
        internal DataTable data = new DataTable();
    }

    internal sealed class HistoryResult
    {
        internal bool issucess = true;
        internal DataTable logPO = new DataTable();
    }

    internal sealed class FakeText { internal string Text; }
    internal sealed class FakeDate
    {
        private DateTime storedValue;
        internal DateTime Value
        {
            get { return storedValue; }
            set { CaseContext.Current.Touch("date.set"); storedValue = value; }
        }
    }

    internal enum e_LogType { Error }
    internal sealed class FakeLogger
    {
        internal void WriteLogAsync(string user, e_LogType kind, string message)
        {
            CaseContext.Current.Touch("log");
            Check.That(user == "synthetic-user" && kind == e_LogType.Error, "Log context changed.");
            Check.That(message == "Lấy thông tin đơn hàng thất bại: synthetic MES unavailable", "Error log changed.");
        }
    }

    internal static class Globals
    {
        internal static readonly FakeProduction ProductionData = new FakeProduction();
        internal static readonly FakeUser CurrentUser = new FakeUser();
    }

    internal sealed class FakeUser { internal string Username = "synthetic-user"; }
    internal sealed class FakeProduction
    {
        internal readonly FakeMes getfromMES = new FakeMes();
        internal readonly FakeHistory getDataPO = new FakeHistory();
    }

    internal sealed class FakeMes
    {
        internal TResult ProductionOrder_Detail(string orderNo)
        {
            CaseContext.Current.Touch("mes");
            Check.That(orderNo == "SYNTHETIC-PO-V07", "MES owner changed.");
            return CaseContext.Current.OrderResult;
        }
    }

    internal sealed class FakeHistory
    {
        internal HistoryResult Get_PO_Run_History_By_OrderNo(string orderNo)
        {
            CaseContext.Current.Touch("history");
            Check.That(orderNo == "SYNTHETIC-PO-V07", "History owner changed.");
            return CaseContext.Current.History;
        }
    }

    internal sealed class InjectedFailure : Exception
    {
        internal InjectedFailure(string stage) : base("synthetic failure at " + stage) { }
    }

    internal sealed class CaseContext
    {
        internal static CaseContext Current;
        internal PPOInfo Owner;
        internal readonly List<string> Calls = new List<string>();
        internal readonly TResult OrderResult = new TResult();
        internal readonly TResult CountResult = new TResult();
        internal readonly HistoryResult History = new HistoryResult();
        internal string ThrowAt;
        internal InjectedFailure Injected;

        internal void Touch(string stage)
        {
            Calls.Add(stage);
            Check.That(Owner.BusyForTest, "IsRender must be true during " + stage);
            if (stage == ThrowAt) throw Injected;
        }
    }

    internal sealed class Scenario
    {
        internal string Name;
        internal string ThrowAt;
        internal bool FailedMes;
        internal bool MalformedDate;
        internal string[] ExpectedCalls;
    }

    internal static class Program
    {
        private static readonly string[] NormalCalls =
            { "mes", "history", "invoke", "date.set", "validate", "count", "update", "load-counters" };

        private static Scenario Throwing(string stage)
        {
            int stop = Array.IndexOf(NormalCalls, stage);
            Check.That(stop >= 0, "Unknown scenario stage.");
            return new Scenario { Name = stage + "-throws", ThrowAt = stage,
                ExpectedCalls = NormalCalls.Take(stop + 1).ToArray() };
        }

        private static Dictionary<string, object> Run(Scenario scenario, bool baseline)
        {
            var state = new CaseContext();
            state.Owner = new PPOInfo();
            state.ThrowAt = scenario.ThrowAt;
            state.Injected = scenario.ThrowAt == null ? null : new InjectedFailure(scenario.ThrowAt);
            state.OrderResult.issuccess = !scenario.FailedMes;
            state.History.logPO.Columns.Add("productionDate", typeof(string));
            state.History.logPO.Rows.Add(scenario.MalformedDate ? "not-a-persisted-date" : "2026-11-01T00:00:00");
            CaseContext.Current = state;

            Exception observed = null;
            try { state.Owner.RenderOrderInfo(); }
            catch (Exception error) { observed = error; }

            bool expectsException = scenario.ThrowAt != null || scenario.MalformedDate;
            bool expectedBusy = baseline && expectsException;
            var failures = new List<string>();
            if (state.Owner.BusyForTest != expectedBusy)
                failures.Add("IsRender expected " + expectedBusy + " after method, actual " + state.Owner.BusyForTest);
            if (scenario.MalformedDate)
            {
                if (!(observed is FormatException)) failures.Add("Expected original FormatException to propagate.");
            }
            else if (scenario.ThrowAt != null)
            {
                if (!ReferenceEquals(observed, state.Injected)) failures.Add("Injected exception identity was not preserved.");
            }
            else if (observed != null) failures.Add("Unexpected exception: " + observed);
            if (!state.Calls.SequenceEqual(scenario.ExpectedCalls))
                failures.Add("Call order mismatch. Actual: " + string.Join(",", state.Calls));
            if (!scenario.MalformedDate && (scenario.ThrowAt == null || Array.IndexOf(NormalCalls, scenario.ThrowAt) > 3))
                if (state.Owner.ipProductionDate.Value != new DateTime(2026, 11, 1))
                    failures.Add("Persisted productionDate result changed.");

            string outcome = failures.Count != 0 ? "failed" : baseline
                ? (expectsException ? "expected_defect_reproduced" : "control_behavior_matched") : "passed";
            Console.WriteLine(outcome.ToUpperInvariant() + " " + scenario.Name);
            foreach (string failure in failures) Console.WriteLine("  " + failure);
            return new Dictionary<string, object> {
                { "name", scenario.Name }, { "outcome", outcome }, { "calls", state.Calls },
                { "is_render_after", state.Owner.BusyForTest },
                { "exception_type", observed == null ? null : observed.GetType().FullName },
                { "injected_exception_identity_preserved", scenario.ThrowAt == null ? (object)null : ReferenceEquals(observed, state.Injected) },
                { "failures", failures }
            };
        }

        private static int Main(string[] args)
        {
            if (args.Length != 2 || (args[1] != "current" && args[1] != "baseline")) return 2;
            bool baseline = args[1] == "baseline";
            var scenarios = new List<Scenario> {
                new Scenario { Name = "normal", ExpectedCalls = NormalCalls },
                new Scenario { Name = "failed-mes-early-return", FailedMes = true,
                    ExpectedCalls = new[] { "mes", "history", "invoke", "date.set", "log", "dialog" } },
                new Scenario { Name = "malformed-persisted-production-date", MalformedDate = true,
                    ExpectedCalls = new[] { "mes", "history", "invoke" } },
                Throwing("invoke"), Throwing("validate"), Throwing("count"),
                Throwing("update"), Throwing("load-counters"), Throwing("mes"), Throwing("history")
            };
            var cases = scenarios.Select(s => Run(s, baseline)).ToArray();
            int failed = cases.Count(c => (string)c["outcome"] == "failed");
            var result = new Dictionary<string, object> {
                { "kind", baseline ? "expected-defect-control" : "current-product-regression" },
                { "product_pass_count", baseline ? 0 : cases.Length - failed },
                { "expected_defect_count", cases.Count(c => (string)c["outcome"] == "expected_defect_reproduced") },
                { "case_count", cases.Length }, { "failed_count", failed },
                { "control_assertions_satisfied", baseline ? (object)(failed == 0) : null },
                { "utc", DateTime.UtcNow.ToString("o") }, { "cases", cases }
            };
            File.WriteAllText(args[0], new JavaScriptSerializer().Serialize(result));
            Console.WriteLine("RESULT cases=" + cases.Length + " failed=" + failed + " kind=" + result["kind"]);
            return failed == 0 ? 0 : 1;
        }
    }
}
