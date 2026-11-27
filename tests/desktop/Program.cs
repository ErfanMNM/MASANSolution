using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpT.Logs;

internal static class Program
{
    private enum ActionCode { Save }
    private static int passed;
    private static readonly string Root = Path.Combine(Path.GetTempPath(), "masan-desktop-" + Guid.NewGuid().ToString("N"));

    private static int Main()
    {
        Directory.CreateDirectory(Root);
        try
        {
            Run("audit roundtrip and graceful drain", AuditRoundTrip);
            Run("audit relative database path", AuditRelativePath);
            Run("audit event timestamps", AuditTimestamps);
            Run("chart replacement metacharacters", ChartReplacement);
            Run("legacy three-loader millisecond reload", LegacyLoaders);
            Run("printer resume offset", PrinterResume);
            Round2Tests.Run(Root, Run);
            Run("SPMS1 TCP lifetime", () => TcpLifetime(false));
            Run("SpT TCP lifetime", () => TcpLifetime(true));
            Console.WriteLine("RESULT passed=" + passed + " failed=0");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally
        {
            SQLiteConnection.ClearAllPools();
            try { Directory.Delete(Root, true); } catch { }
        }
    }

    private static void Run(string name, Action action) { action(); passed++; Console.WriteLine("PASS " + name); }
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static void Until(Func<bool> condition, string message, int timeout = 7000)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (!condition()) { if (timer.ElapsedMilliseconds > timeout) throw new TimeoutException(message); Thread.Sleep(10); }
    }
    private static T Await<T>(Task<T> task, string message)
    {
        Check(task.Wait(8000), message);
        return task.GetAwaiter().GetResult();
    }

    private static void AuditRoundTrip()
    {
        var db = Path.Combine(Root, "audit.db");
        var logger = new LogHelper<ActionCode>(db);
        Parallel.For(0, 1000, i => logger.WriteLogAsync("fixture", ActionCode.Save, "entry-" + i, "{}", "CODE-" + i).GetAwaiter().GetResult());
        var dispose = Task.Run(() => logger.Dispose());
        Check(dispose.Wait(15000), "audit Dispose did not drain in time");
        logger.Dispose(); // IDisposable may be called repeatedly by owners/containers.
        using (var read = new LogHelper<ActionCode>(db))
        {
            var entries = read.GetLogs(2000);
            Check(entries.Count == 1000, "accepted audit entries lost during Dispose");
            Check(entries.Select(e => e.Code).Distinct().Count() == 1000, "Code not persisted/read");
            Check(entries.All(e => e.Code == "CODE-" + e.Description.Substring(6)), "Code/entry mismatch");
            Check(read.GetLogsByAction(ActionCode.Save).Count == 1000, "action read mismatch");
        }
    }
    private static void AuditRelativePath()
    {
        string cwd = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(Root);
            using (var logger = new LogHelper<ActionCode>("relative.db"))
                logger.WriteLogAsync("fixture", ActionCode.Save, "relative", Code: "REL").GetAwaiter().GetResult();
            using (var read = new LogHelper<ActionCode>("relative.db")) Check(read.GetLogs().Single().Code == "REL", "relative path roundtrip failed");
        }
        finally { Directory.SetCurrentDirectory(cwd); }
    }
    private static void AuditTimestamps()
    {
        const long timestamp = 1795730000;
        var active = new QR_MASAN_01.ActiveLogs("fixture", timestamp, QR_MASAN_01.ActiveLogs.e_ActiveLogType.CONNECT, "test");
        var aws = new QR_MASAN_01.AWSLogs("fixture", timestamp, QR_MASAN_01.AWSLogs.e_AWSLogType.CONNECT, "test");
        Check(active.TimeUnix == timestamp && aws.TimeUnix == timestamp, "constructor lost timestamp");
    }
    private static void ChartReplacement()
    {
        string path = Path.Combine(Root, "chart.html");
        string label = "literal $& $` $' $$";
        File.WriteAllText(path, "<script>option = {}; after();</script>");
        var option = new EChartsOption { xAxis = new List<XAxis> { new XAxis { data = new List<string> { label } } } };
        EChartsInjector.InjectEChartsOption(path, option);
        string html = File.ReadAllText(path);
        Check(html == "<script>option = " + JsonConvert.SerializeObject(option, Formatting.Indented, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }) + "; after();</script>", "JSON interpreted as regex replacement syntax");
    }

    private static void LegacyLoaders()
    {
        var table = new DataTable();
        table.Columns.Add("ProductID", typeof(int)); table.Columns.Add("ProductQR", typeof(string));
        table.Columns.Add("Active", typeof(int)); table.Columns.Add("TimestampActive", typeof(string));
        table.Columns.Add("TimestampPrinted", typeof(string)); table.Columns.Add("TimeUnixActive", typeof(long));
        table.Columns.Add("TimeUnixPrinted", typeof(long));
        table.Rows.Add(7, "fixture-code", 1, "active", "printed", 1795730000123L, 1795730000L);
        DesktopFixtures.LoaderMain.Load(table);
        DesktopFixtures.Loader_C1.Load(table);
        DesktopFixtures.Loader_C2.Load(table);
        foreach (var dictionary in new[] { DesktopFixtures.Globalvariable.Main_Content_Dictionary, DesktopFixtures.Globalvariable.C1_Content_Dictionary, DesktopFixtures.Globalvariable.C2_Content_Dictionary })
            Check(dictionary.Count == 1 && dictionary["fixture-code"].TimeUnixActive == 1795730000123L, "loader lost millisecond data or used wrong camera dictionary");
    }
    private static void PrinterResume()
    {
        var uno = new DesktopFixtures.PrinterUnoV7();
        var plus = new DesktopFixtures.PrinterUnoplus();
        foreach (var printer in new DesktopFixtures.PrinterBase[] { uno, plus })
        {
            printer.DATA.AddRange(Enumerable.Range(0, 100).Select(i => "CODE-" + i));
            printer.Old = 50; printer.INDEX = 30;
        }
        uno.Send1Data(); plus.Send1Data();
        foreach (var printer in new DesktopFixtures.PrinterBase[] { uno, plus })
        {
            Check(printer.sendPrinter.Sent.Single() == "CODE-80", "resume refill ignored offset");
            printer.INDEX = 50; printer.BUFFER = 0;
        }
        uno.Send1Data(); plus.Send1Data();
        Check(uno.sendPrinter.Sent.Count == 1 && plus.sendPrinter.Sent.Count == 1, "resume bounds exceeded actual data");
    }

    private sealed class Transport : IDisposable
    {
        public Func<bool> Connected;
        public Action Connect, Disconnect, Cleanup;
        public Action<string> Send;
        public Func<Task> Worker;
        public void Dispose() { Cleanup(); }
    }
    private static Transport Create(bool modern, int port, StringBuilder received, object gate)
    {
        if (modern)
        {
            var c = new SpT.Communications.TCP.TCPClient { IP = "127.0.0.1", Port = port };
            c.ClientCallBack += (state, data) => { if (state == SpT.Communications.TCP.enumClient.DISCONNECTED) throw new InvalidOperationException("fixture subscriber"); };
            c.ClientCallBack += (state, data) => { if (state == SpT.Communications.TCP.enumClient.RECEIVED) lock (gate) received.Append(data); };
            return new Transport { Connected = () => c.Connected, Connect = c.Connect, Disconnect = c.Disconnect, Cleanup = c.Dispose, Send = c.Send, Worker = () => Worker(c) };
        }
        else
        {
            var c = new SPMS1.TCPClient { IP = "127.0.0.1", Port = port };
            c.ClientCallBack += (state, data) => { if (state == SPMS1.enumClient.DISCONNECTED) throw new InvalidOperationException("fixture subscriber"); };
            c.ClientCallBack += (state, data) => { if (state == SPMS1.enumClient.RECEIVED) lock (gate) received.Append(data); };
            return new Transport { Connected = () => c.Connected, Connect = c.Connect, Disconnect = c.Disconnect, Cleanup = c.Dispose, Send = c.Send, Worker = () => Worker(c) };
        }
    }
    private static Task Worker(object component) { return (Task)component.GetType().GetField("reconnectTask", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(component); }
    private static void TcpLifetime(bool modern)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var text = new StringBuilder(); var gate = new object();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        try
        {
            using (var c = Create(modern, port, text, gate))
            {
                var accept = listener.AcceptTcpClientAsync();
                c.Connect();
                using (var server = Await(accept, "initial connect failed"))
                {
                    Until(c.Connected, "Connected state not published");
                    var worker = c.Worker();
                    for (int i = 0; i < 20; i++) c.Connect();
                    Check(ReferenceEquals(worker, c.Worker()), "repeated Connect created workers");
                    byte[] payload = Encoding.UTF8.GetBytes(modern ? "A€B" : "ABC");
                    foreach (byte b in payload) { server.GetStream().WriteByte(b); Thread.Sleep(10); }
                    Until(() => { lock (gate) return text.ToString() == (modern ? "A€B" : "ABC"); }, "receive chunks corrupted");
                    c.Send("outgoing");
                    var bytes = new byte[8]; int count = 0;
                    while (count < bytes.Length)
                    {
                        int size = Await(server.GetStream().ReadAsync(bytes, count, bytes.Length - count), "send stalled");
                        Check(size > 0, "unexpected EOF during send");
                        count += size;
                    }
                    Check(Encoding.ASCII.GetString(bytes) == "outgoing", "send payload mismatch");
                }
                using (var server = Await(listener.AcceptTcpClientAsync(), "remote EOF did not reconnect after subscriber error"))
                {
                    Until(c.Connected, "reconnect state failed");
                    var worker = c.Worker();
                    c.Disconnect();
                    Check(worker.Wait(7000) && !worker.IsFaulted, "disconnect worker did not stop cleanly");
                    Check(!c.Connected(), "disconnect state true");
                    var unwanted = listener.AcceptTcpClientAsync();
                    Thread.Sleep(3300);
                    Check(!unwanted.IsCompleted, "manual Disconnect reconnected");
                    c.Connect();
                    using (var newest = Await(unwanted, "explicit reconnect failed"))
                    {
                        Until(c.Connected, "new lifetime not connected");
                        c.Dispose();
                        Check(c.Worker().Wait(7000), "Dispose left worker running");
                        Check(!c.Connected(), "Dispose left Connected true");
                    }
                }
            }
            using (var container = new Container())
            {
                var plain = modern ? (Component)new SpT.Communications.TCP.TCPClient(container) { IP = "127.0.0.1", Port = port } : new SPMS1.TCPClient(container) { IP = "127.0.0.1", Port = port };
                plain.GetType().GetMethod("Connect").Invoke(plain, null);
                using (var server = Await(listener.AcceptTcpClientAsync(), "no-subscriber connect failed"))
                {
                    Until(() => (bool)plain.GetType().GetProperty("Connected").GetValue(plain), "Connected depends on subscriber");
                    container.Dispose();
                    Check(Worker(plain).Wait(7000), "container Dispose left worker");
                }
            }
        }
        finally { listener.Stop(); }
    }
}
