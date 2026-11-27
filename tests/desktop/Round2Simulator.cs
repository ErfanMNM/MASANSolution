using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestApp;

internal static class Round2Simulator
{
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static void Until(Func<bool> value, string message)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (!value()) { if (timer.ElapsedMilliseconds > 5000) throw new TimeoutException(message); Thread.Sleep(10); }
    }
    internal static void Run(string root)
    {
        Csv(root);
        LifetimeAndQueuedSend();
        SharedClientsAndOffThreadSend();
    }

    private static void Csv(string root)
    {
        string file = Path.Combine(root, "camera-first-column-" + Guid.NewGuid().ToString("N") + ".csv");
        try
        {
            File.WriteAllText(file, "ABC001,first\r\n12345,second\r\n\"A,B\",third\r\n\"line1\r\nline2\",fourth\r\n", Encoding.UTF8);
            var values = CameraSimulatorCore.ReadCsvFirstColumn(file, false);
            Check(values.SequenceEqual(new[] { "ABC001", "12345", "A,B", "line1\r\nline2" }), "camera CSV dropped alphanumeric first row or split quoted fields");
            File.WriteAllText(file, "Code,Description\r\nABC001,first\r\n", Encoding.UTF8);
            Check(CameraSimulatorCore.ReadCsvFirstColumn(file, true).SequenceEqual(new[] { "ABC001" }), "camera explicit CSV header retained");
            File.WriteAllText(file, "123,456\r\n789,012\r\n", Encoding.UTF8);
            Check(CameraSimulatorCore.ReadCsvFirstColumn(file, false).SequenceEqual(new[] { "123", "789" }), "camera numeric multicolumn first row lost");
        }
        finally { File.Delete(file); }
    }

    private static void LifetimeAndQueuedSend()
    {
        var camera = new CameraInstance(1, "fixture", 0);
        var release = new ManualResetEventSlim(false);
        var entered = new ManualResetEventSlim(false);
        Task<int> first = null, stale = null;
        TcpClient oldPeer = null, newPeer = null;
        Task oldAccept = null;
        int staleWrites = 0;
        try
        {
            CameraSimulatorCore.Start(camera, 0, message => { }, () => { }, IPAddress.Loopback);
            oldAccept = camera.AcceptTask;
            oldPeer = new TcpClient(); oldPeer.Connect(IPAddress.Loopback, camera.Port);
            Until(() => CameraSimulatorCore.ClientCount(camera) == 1, "simulator did not accept first loopback client");
            Check(CameraSimulatorCore.Broadcast(camera, "X") == 1, "simulator loopback send failed");
            var one = new byte[1]; var read = oldPeer.GetStream().ReadAsync(one, 0, 1);
            Check(read.Wait(5000) && read.Result == 1 && one[0] == (byte)'X', "simulator wrong payload");
            first = CameraSimulatorCore.BroadcastAsync(camera, "old", (client, data) => { entered.Set(); Check(release.Wait(5000), "release blocked send"); });
            Check(entered.Wait(5000), "simulator async send did not start");
            stale = CameraSimulatorCore.BroadcastAsync(camera, "queued-old", (client, data) => Interlocked.Increment(ref staleWrites));
            CameraSimulatorCore.Stop(camera); // Must not wait on SendGate or the blocked writer.
            CameraSimulatorCore.Start(camera, 0, message => { }, () => { }, IPAddress.Loopback);
            newPeer = new TcpClient(); newPeer.Connect(IPAddress.Loopback, camera.Port);
            Until(() => CameraSimulatorCore.ClientCount(camera) == 1, "simulator did not accept replacement client");
            release.Set();
            Check(first.Wait(5000) && stale.Wait(5000) && stale.Result == 0 && staleWrites == 0, "queued old send reached replacement generation");
            Check(oldAccept.Wait(5000), "old simulator accept left running");
            Check(camera.ServerRunning && CameraSimulatorCore.ClientCount(camera) == 1, "old accept cleanup damaged replacement generation");
            var latest = camera.AcceptTask;
            CameraSimulatorCore.Stop(camera);
            Check(latest.Wait(5000) && CameraSimulatorCore.ClientCount(camera) == 0, "simulator stop left accept/client running");
            for (int i = 0; i < 3; i++)
            {
                CameraSimulatorCore.Start(camera, 0, message => { }, () => { }, IPAddress.Loopback);
                var accept = camera.AcceptTask;
                CameraSimulatorCore.Stop(camera);
                Check(accept.Wait(5000), "simulator immediate start/stop did not finish");
            }
        }
        finally
        {
            release.Set(); CameraSimulatorCore.Stop(camera); oldPeer?.Close(); newPeer?.Close();
            try { first?.Wait(5000); stale?.Wait(5000); oldAccept?.Wait(5000); camera.AcceptTask.Wait(5000); } catch { }
            release.Dispose(); entered.Dispose(); camera.SendWorker.Dispose();
        }
    }

    private static void SharedClientsAndOffThreadSend()
    {
        var camera = new CameraInstance(2, "fake-writer", 0);
        var bad = new TcpClient(); var good = new TcpClient();
        using (var entered = new ManualResetEventSlim(false))
        using (var release = new ManualResetEventSlim(false))
        {
            Task<int> send = null;
            try
            {
                lock (camera.ClientsGate) { camera.ConnectedClients.Add(bad); camera.ConnectedClients.Add(good); }
                int caller = Thread.CurrentThread.ManagedThreadId, writerThread = caller;
                send = CameraSimulatorCore.BroadcastAsync(camera, "payload", (client, data) =>
                {
                    writerThread = Thread.CurrentThread.ManagedThreadId;
                    entered.Set(); Check(release.Wait(5000), "fake writer release timeout");
                    if (ReferenceEquals(client, bad)) throw new IOException("fixture disconnect");
                    Check(Encoding.UTF8.GetString(data) == "payload", "simulator changed protocol payload");
                });
                Check(entered.Wait(5000) && !send.IsCompleted && writerThread != caller, "network write blocked caller thread");
                var add = Task.Run(() => { for (int i = 0; i < 100; i++) lock (camera.ClientsGate) camera.ConnectedClients.Add(new TcpClient()); });
                Check(add.Wait(5000), "client-list gate held across blocked network send");
                release.Set(); Check(send.Wait(5000) && send.Result == 1, "failed client prevented healthy delivery");
                Check(CameraSimulatorCore.ClientCount(camera) == 101, "simulator concurrent client list lost updates");
                Parallel.For(0, 30, i => CameraSimulatorCore.Broadcast(camera, "race", (client, data) => { }));
            }
            finally
            {
                release.Set(); try { send?.Wait(5000); } catch { }
                CameraSimulatorCore.Stop(camera); camera.SendWorker.Dispose();
            }
        }
    }
}
