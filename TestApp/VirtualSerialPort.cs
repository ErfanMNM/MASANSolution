using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace TestApp
{
    public class VirtualSerialPort : IDisposable
    {
        // Windows API để tạo COM port ảo
        [DllImport("kernel32.dll", SetLastError = true)]
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

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        private string _portName;
        private NamedPipeServerStream _pipeServer;
        private Task _listenerTask;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isOpen = false;

        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<string> StatusChanged;

        public bool IsOpen => _isOpen;
        public string PortName => _portName;

        public VirtualSerialPort(string portName)
        {
            _portName = portName;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public bool Open()
        {
            try
            {
                if (_isOpen) return true;

                // Tạo Named Pipe Server để mô phỏng COM port
                string pipeName = $"COM_Bridge_{_portName}";
                _pipeServer = new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                // Bắt đầu listen cho connections
                _listenerTask = Task.Run(async () => await ListenForConnections());

                _isOpen = true;
                OnStatusChanged($"Virtual {_portName} opened - Pipe: \\\\.\\pipe\\COM_Bridge_{_portName}");
                
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error opening {_portName}: {ex.Message}");
                return false;
            }
        }

        public void Close()
        {
            try
            {
                _isOpen = false;
                _cancellationTokenSource?.Cancel();
                
                _pipeServer?.Close();
                _pipeServer?.Dispose();
                
                _listenerTask?.Wait(1000);
                
                OnStatusChanged($"Virtual {_portName} closed");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error closing {_portName}: {ex.Message}");
            }
        }

        public void Write(byte[] data)
        {
            try
            {
                if (_isOpen && _pipeServer?.IsConnected == true)
                {
                    _pipeServer.Write(data, 0, data.Length);
                    _pipeServer.Flush();
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Write error on {_portName}: {ex.Message}");
            }
        }

        public void Write(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Write(Encoding.UTF8.GetBytes(text));
            }
        }

        private async Task ListenForConnections()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    OnStatusChanged($"Waiting for connection to {_portName}...");
                    
                    await _pipeServer.WaitForConnectionAsync(_cancellationTokenSource.Token);
                    
                    OnStatusChanged($"Client connected to {_portName}!");
                    
                    // Handle data while connected
                    await HandleClientData();
                    
                    _pipeServer.Disconnect();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnStatusChanged($"Connection error on {_portName}: {ex.Message}");
                    await Task.Delay(1000); // Wait before retry
                }
            }
        }

        private async Task HandleClientData()
        {
            byte[] buffer = new byte[1024];
            
            while (_pipeServer.IsConnected && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    int bytesRead = await _pipeServer.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token);
                    
                    if (bytesRead > 0)
                    {
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(buffer, receivedData, bytesRead);
                        OnDataReceived(receivedData);
                    }
                }
                catch (IOException)
                {
                    // Client disconnected
                    break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnStatusChanged($"Data handling error on {_portName}: {ex.Message}");
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
    }
}