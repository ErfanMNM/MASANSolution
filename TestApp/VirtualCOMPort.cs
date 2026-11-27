using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp
{
    public class VirtualCOMPort : IDisposable
    {
        private sealed class Session
        {
            internal readonly CancellationTokenSource Stop = new CancellationTokenSource();
            internal readonly ConcurrentQueue<byte[]> Outgoing = new ConcurrentQueue<byte[]>();
            internal readonly SemaphoreSlim Ready = new SemaphoreSlim(0);
            internal NamedPipeServerStream Pipe;
        }
        private readonly object gate = new object();
        private readonly string prefix;
        private readonly bool bufferIncoming;
        private Session current;
        private bool disposed;
        private readonly ConcurrentQueue<byte[]> incoming = new ConcurrentQueue<byte[]>();
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<string> StatusChanged;
        public string PortName { get; }
        public bool IsListening { get { lock (gate) return current != null; } }
        public bool IsOpen { get { lock (gate) return current?.Pipe?.IsConnected == true; } }
        public Task Completion { get; private set; } = Task.CompletedTask;
        public VirtualCOMPort(string portName) : this(portName, "VirtualCOM_") { }
        internal VirtualCOMPort(string portName, string pipePrefix, bool bufferIncoming = true) { PortName = portName; prefix = pipePrefix; this.bufferIncoming = bufferIncoming; }
        private NamedPipeServerStream NewPipe() { return new NamedPipeServerStream(prefix + PortName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous); }
        public bool Open()
        {
            try
            {
                lock (gate)
                {
                    if (disposed) throw new ObjectDisposedException(nameof(VirtualCOMPort));
                    if (current != null) return true;
                    var session = new Session { Pipe = NewPipe() };
                    current = session;
                    Completion = Task.Run(() => Run(session));
                }
                OnStatusChanged("Opened " + PortName); return true;
            }
            catch (Exception ex) { OnStatusChanged(ex.Message); return false; }
        }
        private async Task Run(Session session)
        {
            var token = session.Stop.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    NamedPipeServerStream pipe;
                    lock (gate) { if (!ReferenceEquals(current, session)) break; pipe = session.Pipe; }
                    try
                    {
                        await pipe.WaitForConnectionAsync(token).ConfigureAwait(false);
                        OnStatusChanged("Connected " + PortName);
                        using (var connectionStop = CancellationTokenSource.CreateLinkedTokenSource(token))
                        {
                            var read = ReadLoop(session, pipe, connectionStop.Token);
                            var write = WriteLoop(session, pipe, connectionStop.Token);
                            await Task.WhenAny(read, write).ConfigureAwait(false);
                            connectionStop.Cancel(); pipe.Dispose();
                            try { await Task.WhenAll(read, write).ConfigureAwait(false); } catch (Exception) { }
                        }
                    }
                    catch (Exception) when (token.IsCancellationRequested) { break; }
                    catch (IOException) { }
                    finally { pipe.Dispose(); }
                    lock (gate)
                    {
                        if (!ReferenceEquals(current, session) || token.IsCancellationRequested) break;
                        while (session.Outgoing.TryDequeue(out var ignored)) { }
                        session.Pipe = NewPipe();
                    }
                }
            }
            catch (Exception ex) { if (!token.IsCancellationRequested) OnStatusChanged(ex.Message); }
            finally
            {
                lock (gate) { if (ReferenceEquals(current, session)) current = null; }
                session.Pipe?.Dispose(); session.Stop.Dispose(); session.Ready.Dispose();
            }
        }
        private async Task ReadLoop(Session session, NamedPipeServerStream pipe, CancellationToken token)
        {
            var buffer = new byte[1024];
            while (!token.IsCancellationRequested)
            {
                int count = await pipe.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);
                if (count == 0) break;
                var data = new byte[count]; Array.Copy(buffer, data, count);
                lock (gate) { if (!ReferenceEquals(current, session)) break; if (bufferIncoming) incoming.Enqueue(data); }
                OnDataReceived(data);
            }
        }
        private async Task WriteLoop(Session session, NamedPipeServerStream pipe, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await session.Ready.WaitAsync(token).ConfigureAwait(false);
                while (session.Outgoing.TryDequeue(out var data))
                    await pipe.WriteAsync(data, 0, data.Length, token).ConfigureAwait(false);
                await pipe.FlushAsync(token).ConfigureAwait(false);
            }
        }
        public void Write(byte[] data)
        {
            if (data == null || data.Length == 0) return;
            lock (gate)
            {
                if (current?.Pipe?.IsConnected != true) return;
                current.Outgoing.Enqueue((byte[])data.Clone()); current.Ready.Release();
            }
        }
        public void Write(string text) { if (!string.IsNullOrEmpty(text)) Write(Encoding.UTF8.GetBytes(text)); }
        public byte[] Read() { return incoming.TryDequeue(out var data) ? data : null; }
        public void Close()
        {
            Session old;
            lock (gate)
            {
                old = current; current = null; old?.Stop.Cancel();
                old?.Pipe?.Dispose();
                while (incoming.TryDequeue(out var ignored)) { }
            }
        }
        protected virtual void OnDataReceived(byte[] data)
        {
            var observers = DataReceived; if (observers == null) return;
            var args = new DataReceivedEventArgs(data);
            foreach (EventHandler<DataReceivedEventArgs> observer in observers.GetInvocationList())
                try { observer(this, args); } catch (Exception) { }
        }
        protected virtual void OnStatusChanged(string status)
        {
            var observers = StatusChanged;
            if (observers == null) return;
            foreach (EventHandler<string> observer in observers.GetInvocationList())
                try { observer(this, status); } catch (Exception) { }
        }
        public void Dispose() { lock (gate) disposed = true; Close(); }
    }
    public class DataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public string Text => Encoding.UTF8.GetString(Data);
        public DataReceivedEventArgs(byte[] data) { Data = data; }
    }
}
