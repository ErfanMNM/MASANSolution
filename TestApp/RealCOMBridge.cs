using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp
{
    public class RealCOMBridge : IDisposable
    {
        private SerialPort _port1;
        private SerialPort _port2;
        private bool _isConnected = false;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _bridgeTask1;
        private Task _bridgeTask2;

        public event EventHandler<string> StatusChanged;
        public event EventHandler<string> DataTransferred;

        public bool IsConnected => _isConnected;
        public string Port1Name => _port1?.PortName;
        public string Port2Name => _port2?.PortName;

        public bool CreateBridge(string comPort1, string comPort2)
        {
            try
            {
                // Kiểm tra COM ports có tồn tại không
                if (!IsPortExists(comPort1))
                {
                    OnStatusChanged($"❌ {comPort1} không tồn tại!");
                    return false;
                }

                if (!IsPortExists(comPort2))
                {
                    OnStatusChanged($"❌ {comPort2} không tồn tại!");
                    return false;
                }

                _cancellationTokenSource = new CancellationTokenSource();

                // Tạo và cấu hình SerialPorts
                _port1 = new SerialPort(comPort1);
                _port2 = new SerialPort(comPort2);

                ConfigurePort(_port1);
                ConfigurePort(_port2);

                // Mở ports
                _port1.Open();
                OnStatusChanged($"✅ {comPort1} opened");

                _port2.Open();
                OnStatusChanged($"✅ {comPort2} opened");

                _isConnected = true;

                // Bắt đầu bridge tasks
                _bridgeTask1 = Task.Run(() => BridgeData(_port1, _port2, comPort1, comPort2));
                _bridgeTask2 = Task.Run(() => BridgeData(_port2, _port1, comPort2, comPort1));

                OnStatusChanged($"🎯 COM Bridge được tạo: {comPort1} ↔ {comPort2}");
                OnStatusChanged($"📱 Hercules có thể kết nối đến {comPort1} và {comPort2}");
                OnStatusChanged($"🔄 Dữ liệu tự động chuyển tiếp 2 chiều!");

                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Lỗi tạo COM bridge: {ex.Message}");
                CloseBridge();
                return false;
            }
        }

        private void ConfigurePort(SerialPort port)
        {
            port.BaudRate = 9600;
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.ReadTimeout = 100;
            port.WriteTimeout = 100;
        }

        private async Task BridgeData(SerialPort fromPort, SerialPort toPort, string fromName, string toName)
        {
            byte[] buffer = new byte[1024];

            while (!_cancellationTokenSource.Token.IsCancellationRequested && _isConnected)
            {
                try
                {
                    if (fromPort.IsOpen && fromPort.BytesToRead > 0)
                    {
                        int bytesRead = fromPort.Read(buffer, 0, buffer.Length);
                        
                        if (bytesRead > 0 && toPort.IsOpen)
                        {
                            toPort.Write(buffer, 0, bytesRead);
                            
                            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] {fromName} → {toName}: {data.Trim()}");
                        }
                    }

                    await Task.Delay(1, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (_isConnected)
                    {
                        OnStatusChanged($"❌ Bridge error: {ex.Message}");
                        break;
                    }
                }
            }
        }

        public void SendTestData(string data, bool toPort1 = true)
        {
            try
            {
                if (!_isConnected) return;

                byte[] bytes = Encoding.UTF8.GetBytes(data + "\r\n");

                if (toPort1 && _port1?.IsOpen == true)
                {
                    _port1.Write(bytes, 0, bytes.Length);
                    OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] Test → {Port1Name}: {data}");
                }
                else if (!toPort1 && _port2?.IsOpen == true)
                {
                    _port2.Write(bytes, 0, bytes.Length);
                    OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] Test → {Port2Name}: {data}");
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Send error: {ex.Message}");
            }
        }

        public void CloseBridge()
        {
            try
            {
                _isConnected = false;
                _cancellationTokenSource?.Cancel();

                // Wait for bridge tasks to complete
                Task.WaitAll(new[] { _bridgeTask1, _bridgeTask2 }.Where(t => t != null).ToArray(), 2000);

                // Close ports
                if (_port1?.IsOpen == true)
                {
                    _port1.Close();
                    OnStatusChanged($"🔌 {Port1Name} closed");
                }

                if (_port2?.IsOpen == true)
                {
                    _port2.Close();
                    OnStatusChanged($"🔌 {Port2Name} closed");
                }

                _port1?.Dispose();
                _port2?.Dispose();
                _port1 = null;
                _port2 = null;

                OnStatusChanged("✅ COM Bridge đã đóng");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Close error: {ex.Message}");
            }
        }

        private bool IsPortExists(string portName)
        {
            try
            {
                return SerialPort.GetPortNames().Contains(portName);
            }
            catch
            {
                return false;
            }
        }

        protected virtual void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }

        protected virtual void OnDataTransferred(string data)
        {
            DataTransferred?.Invoke(this, data);
        }

        public void Dispose()
        {
            CloseBridge();
            _cancellationTokenSource?.Dispose();
        }

        public static List<string> GetAvailableCOMPorts()
        {
            try
            {
                return SerialPort.GetPortNames().ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static List<string> GetSuggestedCOMPairs()
        {
            // Gợi ý các cặp COM port thường dùng
            return new List<string>
            {
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6",
                "COM7", "COM8", "COM9", "COM10", "COM11", "COM12"
            };
        }
    }
}