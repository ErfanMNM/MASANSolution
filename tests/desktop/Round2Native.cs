using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Links the production lifetime code, replacing only the native resource boundary.
internal static class Round2Native
{
    private sealed class FakeConnection : TestApp.COMPortEmulator.IConnection
    {
        internal readonly ManualResetEventSlim Started = new ManualResetEventSlim();
        internal readonly ManualResetEventSlim Release = new ManualResetEventSlim();
        internal readonly ConcurrentQueue<byte[]> Written = new ConcurrentQueue<byte[]>();
        internal byte[] Payload;
        internal bool HoldAfterDispose, FailWrite;
        internal int Reads, Disposals;
        private int closed;
        public int Read(byte[] buffer)
        {
            Interlocked.Increment(ref Reads); Started.Set();
            if (!Release.Wait(5000)) throw new TimeoutException("fake read not released");
            var data = Interlocked.Exchange(ref Payload, null);
            if (data == null) { Thread.Sleep(1); return 0; }
            Array.Copy(data, buffer, data.Length); return data.Length;
        }
        public void Write(byte[] data)
        {
            if (FailWrite) throw new IOException("fixture write failure");
            if (Volatile.Read(ref closed) != 0) throw new ObjectDisposedException("fake connection");
            Written.Enqueue((byte[])data.Clone());
        }
        public void Dispose()
        {
            if (Interlocked.Exchange(ref closed, 1) != 0) return;
            Interlocked.Increment(ref Disposals);
            if (!HoldAfterDispose) Release.Set();
        }
    }
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    internal static void Run()
    {
        var old = new FakeConnection { Payload = new byte[] { 1 }, HoldAfterDispose = true };
        var fresh = new FakeConnection { Payload = new byte[] { 2 } };
        var resources = new ConcurrentQueue<FakeConnection>(new[] { old, fresh });
        int opens = 0, received = 0;
        using (var currentReceived = new ManualResetEventSlim())
        using (var port = new TestApp.COMPortEmulator("OFFLINE_ONLY", name =>
        {
            Check(name == "OFFLINE_ONLY", "native factory name changed");
            Interlocked.Increment(ref opens); FakeConnection next;
            Check(resources.TryDequeue(out next), "unexpected resource open"); return next;
        }))
        {
            port.StatusChanged += (s, text) => { throw new Exception("observer fixture"); };
            port.DataReceived += (s, data) => { Interlocked.Add(ref received, data.Single()); currentReceived.Set(); };
            try
            {
                Check(port.Open() && port.Open() && opens == 1, "open not idempotent");
                Check(old.Started.Wait(5000), "old native reader absent"); Task previous = port.Completion;
                port.Close(); Check(old.Disposals == 1 && !port.IsOpen && !previous.IsCompleted, "close waited for retained reader/resource not released");
                Check(port.Open() && opens == 2 && fresh.Started.Wait(5000), "native reopen did not create fresh reader");
                port.Write(new byte[] { 9 }); Check(fresh.Written.Single().Single() == 9 && old.Written.IsEmpty, "write used prior connection");
                old.Release.Set(); Check(previous.Wait(5000) && received == 0 && port.IsOpen, "old reader reached new session or emitted stale data");
                fresh.Release.Set(); Check(currentReceived.Wait(5000) && received == 2, "new reader canceled/stale delivery");
                port.Close(); Check(port.Completion.Wait(5000), "fresh reader did not stop");
                port.Close(); port.Dispose(); Check(old.Disposals == 1 && fresh.Disposals == 1 && !port.Open(), "dispose/reopen contract failed");
            }
            finally { old.Release.Set(); fresh.Release.Set(); }
        }
        var callback = new FakeConnection { Payload = new byte[] { 3 } };
        using (var callbackDone = new ManualResetEventSlim())
        using (var port = new TestApp.COMPortEmulator("OFFLINE_ONLY", name => callback))
        {
            long closeMs = -1;
            port.DataReceived += (s, data) => { var timer = System.Diagnostics.Stopwatch.StartNew(); port.Close(); closeMs = timer.ElapsedMilliseconds; callbackDone.Set(); };
            Check(port.Open(), "callback fixture open failed"); callback.Release.Set();
            Check(callbackDone.Wait(5000) && port.Completion.Wait(5000) && closeMs < 500, "callback close blocked waiting on itself");
        }
        var retry = new FakeConnection(); int attempts = 0, errors = 0;
        using (var port = new TestApp.COMPortEmulator("OFFLINE_ONLY", name =>
        {
            if (++attempts == 1) throw new IOException("fixture configuration failure"); return retry;
        }))
        {
            port.StatusChanged += (s, text) => { if (text.Contains("Error opening") || text.Contains("Write error")) errors++; };
            Check(!port.Open() && !port.IsOpen && port.Open(), "failed native open poisoned retry");
            retry.FailWrite = true; port.Write(new byte[] { 1 }); Check(errors == 2, "native I/O errors were hidden");
            port.Close(); Check(port.Completion.Wait(5000) && retry.Disposals == 1, "native failed-write resource leaked");
        }
        var blocked = new FakeConnection { Payload = new byte[] { 4 } };
        using (var entered = new ManualResetEventSlim())
        using (var finish = new ManualResetEventSlim())
        using (var port = new TestApp.COMPortEmulator("OFFLINE_ONLY", name => blocked))
        {
            port.DataReceived += (s, data) => { entered.Set(); finish.Wait(5000); };
            try
            {
                Check(port.Open(), "blocked subscriber open failed"); blocked.Release.Set();
                Check(entered.Wait(5000), "blocked subscriber absent");
                Check(Task.Run(() => port.Close()).Wait(500), "close blocked behind a UI subscriber");
            }
            finally { finish.Set(); }
            Check(port.Completion.Wait(5000), "blocked subscriber reader leaked");
        }
    }
}
