using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace ViewRegression
{
    // UI adapter only. Generated production methods own all trimming decisions.
    public sealed class FakeList
    {
        public List<string> Items = new List<string>();
    }

    public partial class ReceiveView
    {
        public FakeList opRecive = new FakeList();
        public FakeList opNotiboardAndSend = new FakeList();
        public void Add(string message) { AddReceiveNotification(message); }
    }

    // Exact production code still uses DateTime.Now/new/Parse/AddHours. This
    // deterministic value adapter supplies only the clock/date operations.
    public struct TestDateTime
    {
        private readonly DateTime value;
        public static DateTime Clock;
        private TestDateTime(DateTime current) { value = current; }
        public TestDateTime(int y, int m, int d, int h, int min, int sec)
        { value = new DateTime(y, m, d, h, min, sec); }
        public static TestDateTime Now { get { return new TestDateTime(Clock); } }
        public int Year { get { return value.Year; } }
        public int Month { get { return value.Month; } }
        public int Day { get { return value.Day; } }
        public int Hour { get { return value.Hour; } }
        public TestDateTime AddHours(double amount) { return new TestDateTime(value.AddHours(amount)); }
        public static TestDateTime Parse(string text)
        { return new TestDateTime(DateTime.Parse(text, CultureInfo.InvariantCulture)); }
        public string ToString(string format) { return value.ToString(format, CultureInfo.InvariantCulture); }
        public static TimeSpan operator -(TestDateTime a, TestDateTime b) { return a.value - b.value; }
    }

    // SQLite boundary is fake: it records actual SQL and bound timestamps and
    // supplies declared query-result rows. It never opens any database.
    public static class QueryFixture
    {
        public static List<Dictionary<string, object>> Rows = new List<Dictionary<string, object>>();
        public static Dictionary<string, object> Parameters;
        public static string Sql;
        public static string Connection;
        public static int OpenCount;
        public static void Reset()
        {
            Rows.Clear(); Parameters = null; Sql = null; Connection = null; OpenCount = 0;
        }
    }
    public sealed class SQLiteConnection : IDisposable
    {
        public SQLiteConnection(string connection) { QueryFixture.Connection = connection; }
        public void Open() { QueryFixture.OpenCount++; }
        public void Dispose() { }
    }
    public sealed class FakeParameters : Dictionary<string, object>
    {
        public void AddWithValue(string key, object value) { Add(key, value); }
    }
    public sealed class SQLiteCommand : IDisposable
    {
        public FakeParameters Parameters = new FakeParameters();
        public SQLiteCommand(string sql, SQLiteConnection connection) { QueryFixture.Sql = sql; }
        public FakeReader ExecuteReader()
        {
            QueryFixture.Parameters = new Dictionary<string, object>(Parameters);
            return new FakeReader(QueryFixture.Rows);
        }
        public void Dispose() { }
    }
    public sealed class FakeReader : IDisposable
    {
        private readonly List<Dictionary<string, object>> rows;
        private int index = -1;
        public FakeReader(List<Dictionary<string, object>> input) { rows = input; }
        public bool Read() { index++; return index < rows.Count; }
        public object this[string field] { get { return rows[index][field]; } }
        public void Dispose() { }
    }

    internal static class Program
    {
        private static readonly List<object> Results = new List<object>();
        private static int failed;
        private static void Assert(bool value, string message)
        { if (!value) throw new InvalidOperationException(message); }
        private static void Group(string name, Action test)
        {
            try { test(); Results.Add(new { name, passed = true }); Console.WriteLine("PASS " + name); }
            catch (Exception ex)
            { failed++; Results.Add(new { name, passed = false, error = ex.Message }); Console.WriteLine("FAIL " + name + ": " + ex.Message); }
        }
        private static ReceiveView Backlog(int count)
        {
            var view = new ReceiveView();
            for (int i = 0; i < count; i++) view.opRecive.Items.Add("old-" + i);
            view.opNotiboardAndSend.Items.Add("send-sentinel");
            return view;
        }
        private static void SendUnchanged(ReceiveView view)
        { Assert(view.opNotiboardAndSend.Items.SequenceEqual(new[] { "send-sentinel" }), "Receive trim changed send list"); }
        private static void AddHour(DateTime hour, int id)
        {
            QueryFixture.Rows.Add(new Dictionary<string, object> {
                { "HourBlock", hour.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                { "Total", 100 + id }, { "Good", 70 + id }, { "Bad", 20 }, { "Other", 10 }
            });
        }
        private static void CheckChart(DateTime now, bool rows)
        {
            QueryFixture.Reset(); TestDateTime.Clock = now;
            var end = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            var start = end.AddHours(-5);
            if (rows)
            {
                AddHour(start.AddHours(-1), 800);
                for (int i = 0; i < 6; i++) AddHour(start.AddHours(i), i);
                AddHour(end.AddHours(1), 900);
            }
            var chart = StatisticsView.GetLast6Hours("synthetic-in-memory-adapter.db");
            Assert(chart.Labels.Length == 6 && chart.Tong.Length == 6 && chart.Tot.Length == 6 && chart.Loi.Length == 6 && chart.Khac.Length == 6,
                "All labels and value arrays must have six elements");
            Assert(chart.Labels.SequenceEqual(Enumerable.Range(0, 6).Select(i => start.AddHours(i).ToString("H:00", CultureInfo.InvariantCulture))), "Label window is wrong");
            Assert(chart.Labels[5] == end.ToString("H:00", CultureInfo.InvariantCulture), "Final label must be current hour");
            Assert((string)QueryFixture.Parameters["@start"] == start.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), "Bound start must be five hours before current hour");
            Assert((string)QueryFixture.Parameters["@end"] == end.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), "Bound end must be current hour");
            Assert(QueryFixture.Sql.Contains(">= datetime(@start)") && QueryFixture.Sql.Contains("<= datetime(@end, '+59 minutes', '+59 seconds')"), "SQL window bindings changed");
            Assert(QueryFixture.OpenCount == 1, "Expected one fake connection");
            for (int i = 0; i < 6; i++)
            {
                Assert(chart.Tong[i] == (rows ? 100 + i : 0), "Total bucket mismatch " + i);
                Assert(chart.Tot[i] == (rows ? 70 + i : 0), "Good bucket mismatch " + i);
                Assert(chart.Loi[i] == (rows ? 20 : 0), "Bad bucket mismatch " + i);
                Assert(chart.Khac[i] == (rows ? 10 : 0), "Other bucket mismatch " + i);
            }
        }
        private static int Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Group("receive-first-event", () => { var v = Backlog(0); v.Add("new"); Assert(v.opRecive.Items.SequenceEqual(new[] { "new" }), "First receive missing"); SendUnchanged(v); });
            Group("receive-exact-limit", () => { var v = Backlog(49); v.Add("new"); Assert(v.opRecive.Items.Count == 50 && v.opRecive.Items[49] == "old-48", "50-item boundary wrong"); SendUnchanged(v); });
            Group("receive-overflow-oldest", () => { var v = Backlog(50); v.Add("new"); Assert(v.opRecive.Items.Count == 50 && v.opRecive.Items[0] == "new" && v.opRecive.Items[49] == "old-48", "Oldest receive not removed"); SendUnchanged(v); });
            Group("receive-burst-bounded-per-event", () => { var v = Backlog(0); for (int i = 0; i < 1000; i++) { v.Add("event-" + i); Assert(v.opRecive.Items.Count <= 50, "Per-event cap exceeded"); } Assert(v.opRecive.Items[0] == "event-999" && v.opRecive.Items[49] == "event-950", "Latest receive order wrong"); SendUnchanged(v); });
            Group("receive-prior-backlog-cleared", () => { var v = Backlog(200); v.Add("new"); Assert(v.opRecive.Items.Count == 50 && v.opRecive.Items[0] == "new", "Old backlog not capped"); SendUnchanged(v); });
            Group("periodic-trims-only-receive", () => { var v = Backlog(200); v.PeriodicTrim(); Assert(v.opRecive.Items.Count == 50 && v.opRecive.Items[0] == "old-0" && v.opRecive.Items[49] == "old-49", "Periodic receive cap wrong"); SendUnchanged(v); });
            Group("six-hour-day-shape-and-all-buckets", () => CheckChart(new DateTime(2026, 11, 27, 14, 27, 31), true));
            Group("six-hour-midnight-wrap", () => CheckChart(new DateTime(2026, 11, 27, 2, 7, 13), true));
            Group("six-hour-empty-result-shape", () => CheckChart(new DateTime(2026, 11, 27, 0, 0, 0), false));
            var result = new { status = failed == 0 ? "passed" : "failed", passedGroups = Results.Count - failed, failedGroups = failed, groups = Results,
                coverage = "Exact production receive helper, exact periodic receive trim block and exact GetLast6Hours/class; fake list, deterministic DateTime adapter and fake SQLite result/binding adapters. No live UI, SQL execution, DB, network or app startup." };
            string json = new JavaScriptSerializer().Serialize(result);
            if (args.Length != 1) throw new ArgumentException("Expected isolated result JSON path");
            File.WriteAllText(args[0], json);
            Console.WriteLine(json);
            return failed == 0 ? 0 : 1;
        }
    }
}
