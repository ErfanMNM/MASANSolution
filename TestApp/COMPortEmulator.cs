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

        private string _portName;
        private SafeFileHandle _portHandle;
        private bool _isOpen = false;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _readerTask;

        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<string> StatusChanged;

        public bool IsOpen => _isOpen;
        public string PortName => _portName;

        public COMPortEmulator(string portName)
        {
            _portName = portName;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public bool Open()
        {
            try
            {
                if (_isOpen) return true;

                // Thử mở COM port với Windows API
                string comPath = $"\\\\.\\{_portName}";
                
                _portHandle = CreateFile(
                    comPath,
                    GENERIC_READ | GENERIC_WRITE,
                    0, // Exclusive access
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    FILE_ATTRIBUTE_NORMAL,
                    IntPtr.Zero);

                if (_portHandle.IsInvalid)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Cannot open {_portName}. Error: {error}. Port may not exist or already in use.");
                }

                // Configure COM port
                ConfigurePort();

                _isOpen = true;
                
                // Start reading thread
                _readerTask = Task.Run(ReadDataAsync);

                OnStatusChanged($"✅ {_portName} opened successfully!");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Error opening {_portName}: {ex.Message}");
                return false;
            }
        }

        private void ConfigurePort()
        {
            // Set timeouts
            COMMTIMEOUTS timeouts = new COMMTIMEOUTS
            {
                ReadIntervalTimeout = 50,
                ReadTotalTimeoutMultiplier = 0,
                ReadTotalTimeoutConstant = 100,
                WriteTotalTimeoutMultiplier = 0,
                WriteTotalTimeoutConstant = 100
            };
            SetCommTimeouts(_portHandle, ref timeouts);

            // Set DCB (Data Control Block)
            DCB dcb = new DCB();
            dcb.DCBlength = (uint)Marshal.SizeOf(dcb);
            
            if (GetCommState(_portHandle, ref dcb))
            {
                dcb.BaudRate = 9600;
                dcb.ByteSize = 8;
                dcb.Parity = 0; // None
                dcb.StopBits = 0; // 1 stop bit
                SetCommState(_portHandle, ref dcb);
            }
        }

        public void Close()
        {
            try
            {
                _isOpen = false;
                _cancellationTokenSource?.Cancel();

                _readerTask?.Wait(1000);

                _portHandle?.Close();
                _portHandle?.Dispose();
                _portHandle = null;

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
                if (!_isOpen || _portHandle?.IsInvalid != false) return;

                uint bytesWritten;
                bool success = WriteFile(_portHandle, data, (uint)data.Length, out bytesWritten, IntPtr.Zero);
                
                if (!success)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Write failed. Error: {error}");
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

        private async Task ReadDataAsync()
        {
            byte[] buffer = new byte[1024];

            while (!_cancellationTokenSource.Token.IsCancellationRequested && _isOpen)
            {
                try
                {
                    uint bytesRead;
                    bool success = ReadFile(_portHandle, buffer, (uint)buffer.Length, out bytesRead, IntPtr.Zero);

                    if (success && bytesRead > 0)
                    {
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(buffer, receivedData, bytesRead);
                        OnDataReceived(receivedData);
                    }

                    await Task.Delay(1, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnStatusChanged($"❌ Read error on {_portName}: {ex.Message}");
                    break;
                }
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        protected virtual void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }

        public void Dispose()
        {
            Close();
            _cancellationTokenSource?.Dispose();
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