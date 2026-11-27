using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace TestApp
{
    public class COMPortEmulator : IDisposable
    {
        // Windows API để tạo và quản lý COM ports
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCommTimeouts(
            SafeFileHandle hFile,
            ref COMMTIMEOUTS lpCommTimeouts);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCommState(
            SafeFileHandle hFile,
            ref DCB lpDCB);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCommState(
            SafeFileHandle hFile,
            ref DCB lpDCB);

        [StructLayout(LayoutKind.Sequential)]
        private struct COMMTIMEOUTS
        {
            public uint ReadIntervalTimeout;
            public uint ReadTotalTimeoutMultiplier;
            public uint ReadTotalTimeoutConstant;
            public uint WriteTotalTimeoutMultiplier;
            public uint WriteTotalTimeoutConstant;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DCB
        {
            public uint DCBlength;
            public uint BaudRate;
            public uint flags;
            public ushort wReserved;
            public ushort XonLim;
            public ushort XoffLim;
            public byte ByteSize;
            public byte Parity;
            public byte StopBits;
            public byte XonChar;
            public byte XoffChar;
            public byte ErrorChar;
            public byte EofChar;
            public byte EvtChar;
            public ushort wReserved1;
        }

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private const uint FILE_FLAG_OVERLAPPED = 0x40000000;

        internal interface IConnection : IDisposable
        {
            int Read(byte[] buffer);
            void Write(byte[] data);
        }

        private sealed class NativeConnection : IConnection
        {
            private readonly SafeFileHandle handle;
            private NativeConnection(SafeFileHandle handle) { this.handle = handle; }
            internal static IConnection Open(string portName)
            {
                var handle = CreateFile($"\\\\.\\{portName}", GENERIC_READ | GENERIC_WRITE,
                    0, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
                try
                {
                    if (handle.IsInvalid) throw new IOException("Cannot open " + portName + ": " + Marshal.GetLastWin32Error());
                    var timeouts = new COMMTIMEOUTS { ReadIntervalTimeout = 50, ReadTotalTimeoutConstant = 100, WriteTotalTimeoutConstant = 100 };
                    if (!SetCommTimeouts(handle, ref timeouts)) throw new IOException("SetCommTimeouts failed: " + Marshal.GetLastWin32Error());
                    var dcb = new DCB(); dcb.DCBlength = (uint)Marshal.SizeOf(dcb);
                    if (!GetCommState(handle, ref dcb)) throw new IOException("GetCommState failed: " + Marshal.GetLastWin32Error());
                    dcb.BaudRate = 9600; dcb.ByteSize = 8; dcb.Parity = 0; dcb.StopBits = 0;
                    if (!SetCommState(handle, ref dcb)) throw new IOException("SetCommState failed: " + Marshal.GetLastWin32Error());
                    return new NativeConnection(handle);
                }
                catch { handle.Dispose(); throw; }
            }
            public int Read(byte[] buffer)
            {
                uint count;
                if (!ReadFile(handle, buffer, (uint)buffer.Length, out count, IntPtr.Zero))
                    throw new IOException("Read failed: " + Marshal.GetLastWin32Error());
                return checked((int)count);
            }
            public void Write(byte[] data)
            {
                uint count;
                if (!WriteFile(handle, data, (uint)data.Length, out count, IntPtr.Zero) || count != data.Length)
                    throw new IOException("Write failed or incomplete: " + Marshal.GetLastWin32Error());
            }
            public void Dispose() { handle.Dispose(); }
        }

        private sealed class Session
        {
            internal readonly IConnection Connection;
            internal readonly CancellationTokenSource Stop = new CancellationTokenSource();
            internal Session(IConnection connection) { Connection = connection; }
        }
        private readonly object gate = new object();
        private readonly string _portName;
        private readonly Func<string, IConnection> openConnection;
        private Session current;
        private bool disposed;

        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<string> StatusChanged;

        public bool IsOpen { get { lock (gate) return current != null; } }
        public string PortName => _portName;
        public Task Completion { get; private set; } = Task.CompletedTask;

        public COMPortEmulator(string portName) : this(portName, NativeConnection.Open) { }
        internal COMPortEmulator(string portName, Func<string, IConnection> openConnection)
        {
            _portName = portName;
            this.openConnection = openConnection ?? throw new ArgumentNullException(nameof(openConnection));
        }

        public bool Open()
        {
            try
            {
                lock (gate)
                {
                    if (disposed) return false;
                    if (current != null) return true;
                    var session = new Session(openConnection(_portName));
                    current = session;
                    Completion = Task.Run(() => ReadDataAsync(session));
                }

                OnStatusChanged($"✅ {_portName} opened successfully!");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Error opening {_portName}: {ex.Message}");
                return false;
            }
        }

        public void Close()
        {
            try
            {
                Session old;
                lock (gate)
                {
                    old = current; current = null;
                    old?.Stop.Cancel();
                    // Closing a native resource never waits on this reader task.
                    old?.Connection.Dispose();
                }

                OnStatusChanged($"🔌 {_portName} closed");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Error closing {_portName}: {ex.Message}");
            }
        }

        public void Write(byte[] data)
        {
            try
            {
                if (data == null || data.Length == 0) return;
                lock (gate)
                {
                    current?.Connection.Write(data);
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Write error on {_portName}: {ex.Message}");
            }
        }

        public void Write(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Write(Encoding.UTF8.GetBytes(text));
            }
        }

        private async Task ReadDataAsync(Session session)
        {
            var token = session.Stop.Token;
            byte[] buffer = new byte[1024];
            try
            {
                while (!token.IsCancellationRequested)
                {
                    int bytesRead = session.Connection.Read(buffer);
                    lock (gate)
                    {
                        if (!ReferenceEquals(current, session) || token.IsCancellationRequested) break;
                    }
                    if (bytesRead > 0)
                    {
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(buffer, receivedData, bytesRead);
                        // An already-dispatched callback may finish after Close; it must not hold
                        // the lifecycle gate while synchronously invoking a UI subscriber.
                        OnDataReceived(receivedData);
                    }
                    await Task.Delay(1, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { if (!token.IsCancellationRequested) OnStatusChanged($"❌ Read error on {_portName}: {ex.Message}"); }
            finally
            {
                lock (gate) { if (ReferenceEquals(current, session)) current = null; }
                try { session.Connection.Dispose(); } finally { session.Stop.Dispose(); }
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            var observers = DataReceived;
            if (observers != null) foreach (EventHandler<byte[]> observer in observers.GetInvocationList())
                try { observer(this, data); } catch (Exception) { }
        }

        protected virtual void OnStatusChanged(string status)
        {
            var observers = StatusChanged;
            if (observers != null) foreach (EventHandler<string> observer in observers.GetInvocationList())
                try { observer(this, status); } catch (Exception) { }
        }

        public void Dispose()
        {
            lock (gate) disposed = true;
            Close();
        }

        // Static method để check COM port có tồn tại không
        public static bool IsPortAvailable(string portName)
        {
            try
            {
                string comPath = $"\\\\.\\{portName}";
                var handle = CreateFile(
                    comPath,
                    GENERIC_READ | GENERIC_WRITE,
                    0,
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    FILE_ATTRIBUTE_NORMAL,
                    IntPtr.Zero);

                bool available = !handle.IsInvalid;
                handle?.Close();
                handle?.Dispose();
                
                return available;
            }
            catch
            {
                return false;
            }
        }
    }
}
