using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPMS1
{
    public enum enumClient { CONNECTED, DISCONNECTED, RECEIVED, RECONNECT }

    public partial class TCPClient : Component
    {
        public delegate void EventForClient(enumClient state, string data);
        public event EventForClient ClientCallBack;
        public string IP { get; set; }
        public int Port { get; set; }
        private volatile bool connected;
        public bool Connected { get { return connected; } private set { connected = value; } }

        private readonly object stateGate = new object();
        private readonly object sendGate = new object();
        private Socket client;
        private CancellationTokenSource cts;
        private Task reconnectTask;
        private bool disposed;

        public TCPClient() { InitializeComponent(); }
        public TCPClient(IContainer container) { container.Add(this); InitializeComponent(); }

        public void Connect()
        {
            lock (stateGate)
            {
                if (disposed) throw new ObjectDisposedException(nameof(TCPClient));
                if (cts != null) return;
                var lifetime = new CancellationTokenSource();
                cts = lifetime;
                reconnectTask = Task.Run(() => RunConnectionAsync(lifetime));
            }
        }

        private async Task RunConnectionAsync(CancellationTokenSource lifetime)
        {
            var token = lifetime.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    string error = "Server closed connection";
                    try
                    {
                        lock (stateGate)
                        {
                            if (token.IsCancellationRequested || !ReferenceEquals(cts, lifetime)) break;
                            // Publish before connecting so Disconnect can abort a pending connect.
                            client = socket;
                        }
                        Notify(enumClient.RECONNECT, "Reconnecting...");
                        await socket.ConnectAsync(new IPEndPoint(IPAddress.Parse(IP), Port)).ConfigureAwait(false);
                        lock (stateGate)
                        {
                            if (token.IsCancellationRequested || !ReferenceEquals(client, socket)) break;
                            Connected = true;
                        }
                        Notify(enumClient.CONNECTED, "Connected");
                        var buffer = new byte[512];
                        var chars = new char[Encoding.ASCII.GetMaxCharCount(buffer.Length)];
                        var decoder = Encoding.ASCII.GetDecoder();
                        while (!token.IsCancellationRequested)
                        {
                            int count = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None).ConfigureAwait(false);
                            if (count == 0) break;
                            int length = decoder.GetChars(buffer, 0, count, chars, 0);
                            if (length > 0 && !token.IsCancellationRequested)
                                Notify(enumClient.RECEIVED, new string(chars, 0, length));
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                    finally
                    {
                        bool notify = false;
                        lock (stateGate)
                        {
                            if (ReferenceEquals(cts, lifetime) && ReferenceEquals(client, socket))
                            {
                                client = null;
                                Connected = false;
                                notify = !token.IsCancellationRequested;
                            }
                        }
                        socket.Dispose();
                        if (notify) Notify(enumClient.DISCONNECTED, error);
                    }
                    await Task.Delay(3000, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            finally
            {
                lock (stateGate)
                {
                    if (ReferenceEquals(cts, lifetime)) cts = null;
                }
                lifetime.Dispose();
            }
        }

        public void Send(string data)
        {
            if (data == null) return;
            Socket socket;
            lock (stateGate) socket = client;
            if (socket == null || !Connected) return;
            try
            {
                var bytes = Encoding.ASCII.GetBytes(data);
                lock (sendGate)
                {
                    int offset = 0;
                    while (offset < bytes.Length)
                    {
                        int sent = socket.Send(bytes, offset, bytes.Length - offset, SocketFlags.None);
                        if (sent == 0) throw new SocketException((int)SocketError.ConnectionReset);
                        offset += sent;
                    }
                }
            }
            catch (Exception)
            {
                // Close only the captured socket. Its receive loop owns state and reconnect.
                socket.Dispose();
            }
        }

        private void Notify(enumClient state, string data)
        {
            var handlers = ClientCallBack;
            if (handlers == null) return;
            foreach (EventForClient handler in handlers.GetInvocationList())
            {
                try { handler(state, data); }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("TCP callback failed: {0}", ex.GetType().Name);
                }
            }
        }

        public void Disconnect() { StopConnection(false); }

        private void StopConnection(bool disposing)
        {
            Socket socket;
            lock (stateGate)
            {
                if (disposing) disposed = true;
                cts?.Cancel();
                cts = null;
                socket = client;
                client = null;
                Connected = false;
            }
            socket?.Dispose();
            if (!disposing) Notify(enumClient.DISCONNECTED, "Disconnected");
        }
    }
}
