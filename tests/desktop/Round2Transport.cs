using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Concurrent;

internal static class Round2Transport
{
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Until(Func<bool> condition, string message)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (!condition()) { if (timer.ElapsedMilliseconds > 5000) throw new TimeoutException(message); Thread.Sleep(10); }
    }
    private static void Receive(Stream stream, byte expected)
    {
        var bytes = new byte[1]; var task = stream.ReadAsync(bytes, 0, 1);
        if (!task.Wait(5000)) { stream.Dispose(); throw new TimeoutException("bridge read stalled"); }
        Check(task.Result == 1 && bytes[0] == expected, "bridge wrong payload/EOF");
    }
    private static NamedPipeClientStream Pipe(string name)
    {
        var client = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
        try { client.Connect(5000); return client; } catch { client.Dispose(); throw; }
    }
    internal static void Run()
    {
        string name = "FIXTURE_" + Guid.NewGuid().ToString("N");
        using (var port = new TestApp.VirtualCOMPort(name))
        {
            port.StatusChanged += (s, text) => { throw new Exception("status observer fixture"); };
            int packets = 0;
            port.DataReceived += (s, data) => { throw new Exception("payload observer fixture"); };
            port.DataReceived += (s, data) => Interlocked.Increment(ref packets);
            for (int generation = 0; generation < 2; generation++)
            {
                Check(port.Open(), "pipe open failed");
                Check(port.Read() == null, "pipe carried stale incoming data after reopen");
                using (var peer = Pipe("VirtualCOM_" + name))
                {
                    Until(() => port.IsOpen, "pipe did not connect");
                    port.Write(new byte[] { 31 }); Receive(peer, 31); // peer has not sent first
                    peer.WriteByte(32); byte[] incoming = null;
                    Until(() => (incoming = port.Read()) != null, "pipe inbound stalled"); Check(incoming.Single() == 32, "pipe inbound mismatch");
                    int before = packets; peer.WriteByte(33); Until(() => packets > before, "throwing observer disconnected pipe");
                    port.Write(new byte[] { 34 }); Receive(peer, 34); // Leave packet 33 unread for Close/reopen.
                }
                port.Close(); Check(port.Completion.Wait(5000), "pipe worker left running");
            }
        }
        using (var port = new TestApp.VirtualSerialPort(name))
        {
            int packets = 0;
            port.DataReceived += (s, data) => { throw new Exception("serial payload observer fixture"); };
            port.DataReceived += (s, data) => Interlocked.Increment(ref packets);
            for (int generation = 0; generation < 2; generation++)
            {
                Check(port.Open(), "serial pipe reopen failed");
                using (var peer = Pipe("COM_Bridge_" + name))
                {
                    // Wait for the server accept to publish before its first write.
                    Thread.Sleep(30); port.Write(new byte[] { 41 }); Receive(peer, 41);
                    int before = packets; peer.WriteByte(42); Until(() => packets > before, "serial observer skipped healthy subscriber");
                    port.Write(new byte[] { 43 }); Receive(peer, 43);
                }
                port.Close(); Check(port.Completion.Wait(5000), "serial pipe worker left running");
            }
        }
        using (var bridge = new TestApp.VirtualCOMBridge())
        {
            bridge.StatusChanged += (s, text) => { throw new Exception("status observer fixture"); };
            bridge.DataTransferred += (s, text) => { throw new Exception("transfer observer fixture"); };
            int transfers = 0; bridge.DataTransferred += (s, text) => Interlocked.Increment(ref transfers);
            Check(bridge.CreateBridge(name + "A", name + "B"), "named-pipe bridge create failed");
            var oldPort = (TestApp.VirtualCOMPort)typeof(TestApp.VirtualCOMBridge).GetField("_port1", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bridge);
            var eventField = typeof(TestApp.VirtualCOMPort).GetField("DataReceived", BindingFlags.NonPublic | BindingFlags.Instance);
            var previousHandler = (EventHandler<TestApp.DataReceivedEventArgs>)eventField.GetValue(oldPort);
            using (var a = Pipe("VirtualCOM_" + name + "A"))
            using (var b = Pipe("VirtualCOM_" + name + "B"))
            {
                Thread.Sleep(30); a.WriteByte(51); Receive(b, 51); b.WriteByte(52); Receive(a, 52);
                for (int i = 0; i < 30; i++) { a.WriteByte(53); Receive(b, 53); }
                Check(oldPort.Read() == null, "event-only bridge buffered forwarded packets indefinitely");
                bridge.CloseBridge();
            }
            Check(eventField.GetValue(oldPort) == null, "old bridge delegate was not removed");
            Check(bridge.CreateBridge(name + "A", name + "B"), "named-pipe bridge recreate failed");
            int beforeStale = transfers; previousHandler(oldPort, new TestApp.DataReceivedEventArgs(new byte[] { 99 }));
            Check(transfers == beforeStale, "stale bridge callback logged after replacement");
            bridge.CloseBridge();
            Parallel.For(0, 4, i => Check(bridge.CreateBridge(name + "C" + i, name + "D" + i), "concurrent pipe create failed"));
            bridge.CloseBridge();
            for (int i = 0; i < 4; i++) using (var a = new TestApp.VirtualCOMPort(name + "C" + i)) using (var b = new TestApp.VirtualCOMPort(name + "D" + i))
            { Check(a.Open() && b.Open(), "concurrent pipe replacement leaked binding"); a.Close(); b.Close(); Check(a.Completion.Wait(5000) && b.Completion.Wait(5000), "pipe rebind workers leaked"); }
        }
        var blocker = new TcpListener(IPAddress.Loopback, 0);
        try
        {
            blocker.Start(); int occupied = ((IPEndPoint)blocker.LocalEndpoint).Port;
            var first = new TcpListener(IPAddress.Loopback, 0); first.Start(); int available = ((IPEndPoint)first.LocalEndpoint).Port; first.Stop();
            using (var bridge = new TestApp.TCPCOMBridge()) Check(!bridge.CreateBridge(available, occupied), "TCP second bind should fail");
            var retry = new TcpListener(IPAddress.Loopback, available); retry.Start(); retry.Stop();
        }
        finally { blocker.Stop(); }
        using (var bridge = new TestApp.TCPCOMBridge())
        {
            bridge.StatusChanged += (s, text) => { throw new Exception("status observer fixture"); };
            bridge.DataTransferred += (s, text) => { throw new Exception("transfer observer fixture"); };
            Check(bridge.CreateBridge(0, 0), "TCP bridge create failed");
            using (var onlySecond = new TcpClient()) { onlySecond.Connect(IPAddress.Loopback, bridge.Port2); bridge.CloseBridge(); Check(bridge.Completion.Wait(5000), "pending accept left worker"); }
            Check(bridge.CreateBridge(0, 0), "TCP bridge reopen failed");
            using (var a = new TcpClient()) using (var b = new TcpClient())
            {
                a.Connect(IPAddress.Loopback, bridge.Port1); b.Connect(IPAddress.Loopback, bridge.Port2); Until(() => bridge.IsConnected, "TCP pair not published");
                a.GetStream().WriteByte(61); Receive(b.GetStream(), 61); b.GetStream().WriteByte(62); Receive(a.GetStream(), 62);
                a.Close(); Check(bridge.Completion.Wait(5000), "TCP EOF did not stop both directions"); Check(!bridge.IsConnected, "TCP connected stale after EOF");
            }
            // Every caller supplies distinct fixed ports so all replaced listeners can be checked.
            var reservations = Enumerable.Range(0, 8).Select(i => new TcpListener(IPAddress.Loopback, 0)).ToArray();
            foreach (var listener in reservations) listener.Start();
            int[] ports = reservations.Select(listener => ((IPEndPoint)listener.LocalEndpoint).Port).ToArray();
            foreach (var listener in reservations) listener.Stop();
            Parallel.For(0, 4, i => { Check(bridge.CreateBridge(ports[i * 2], ports[i * 2 + 1]), "concurrent TCP create failed"); });
            bridge.CloseBridge(); Check(bridge.Completion.Wait(5000), "concurrent TCP final worker leaked");
            foreach (int port in ports) { var listener = new TcpListener(IPAddress.Loopback, port); listener.Start(); listener.Stop(); }
        }
    }
}
