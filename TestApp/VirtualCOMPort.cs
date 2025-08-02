using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Threading;
using System.Text;
using System.ComponentModel;

namespace TestApp
{
    public class VirtualCOMPort : IDisposable
    {
        private string _portName;
        private NamedPipeServerStream _pipeServer;
        private BackgroundWorker _worker;
        private volatile bool _isRunning = false;
        private Queue<byte[]> _incomingData = new Queue<byte[]>();
        private Queue<byte[]> _outgoingData = new Queue<byte[]>();
        private readonly object _lockObject = new object();

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<string> StatusChanged;

        public bool IsOpen => _isRunning && _pipeServer?.IsConnected == true;
        public string PortName => _portName;

        public VirtualCOMPort(string portName)
        {
            _portName = portName;
            InitializeWorker();
        }

        private void InitializeWorker()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_Completed;
        }

        public bool Open()
        {
            try
            {
                if (_isRunning) return true;

                _pipeServer = new NamedPipeServerStream(
                    $"VirtualCOM_{_portName}",
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                _isRunning = true;
                _worker.RunWorkerAsync();

                OnStatusChanged($"Virtual {_portName} đã mở");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Lỗi mở {_portName}: {ex.Message}");
                return false;
            }
        }

        public void Close()
        {
            _isRunning = false;
            
            if (_worker?.IsBusy == true)
            {
                _worker.CancelAsync();
            }

            _pipeServer?.Close();
            _pipeServer?.Dispose();
            _pipeServer = null;

            OnStatusChanged($"Virtual {_portName} đã đóng");
        }

        public void Write(byte[] data)
        {
            if (!IsOpen || data == null || data.Length == 0) return;

            lock (_lockObject)
            {
                _outgoingData.Enqueue(data);
            }
        }

        public void Write(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Write(Encoding.UTF8.GetBytes(text));
            }
        }

        public byte[] Read()
        {
            lock (_lockObject)
            {
                if (_incomingData.Count > 0)
                {
                    return _incomingData.Dequeue();
                }
            }
            return null;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            byte[] buffer = new byte[1024];

            try
            {
                // Đợi client kết nối
                OnStatusChanged($"Đang đợi ứng dụng kết nối đến {_portName}...");
                _pipeServer.WaitForConnection();
                OnStatusChanged($"Ứng dụng đã kết nối đến {_portName}!");

                while (!worker.CancellationPending && _isRunning && _pipeServer.IsConnected)
                {
                    // Đọc dữ liệu từ client
                    if (_pipeServer.CanRead)
                    {
                        try
                        {
                            int bytesRead = _pipeServer.Read(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                byte[] receivedData = new byte[bytesRead];
                                Array.Copy(buffer, receivedData, bytesRead);
                                
                                lock (_lockObject)
                                {
                                    _incomingData.Enqueue(receivedData);
                                }

                                OnDataReceived(receivedData);
                            }
                        }
                        catch (IOException) { /* Pipe disconnected */ }
                    }

                    // Gửi dữ liệu đến client
                    lock (_lockObject)
                    {
                        while (_outgoingData.Count > 0 && _pipeServer.CanWrite)
                        {
                            try
                            {
                                byte[] dataToSend = _outgoingData.Dequeue();
                                _pipeServer.Write(dataToSend, 0, dataToSend.Length);
                                _pipeServer.Flush();
                            }
                            catch (IOException) { /* Pipe disconnected */ }
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Lỗi {_portName}: {ex.Message}");
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                OnStatusChanged($"{_portName} worker đã bị hủy");
            }
            else if (e.Error != null)
            {
                OnStatusChanged($"{_portName} worker lỗi: {e.Error.Message}");
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data));
        }

        protected virtual void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }

        public void Dispose()
        {
            Close();
            _worker?.Dispose();
        }
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public string Text => Encoding.UTF8.GetString(Data);

        public DataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
    }
}