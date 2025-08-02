using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    public class VirtualCOMBridge : IDisposable
    {
        private VirtualCOMPort _port1;
        private VirtualCOMPort _port2;
        private bool _isConnected = false;

        public event EventHandler<string> StatusChanged;
        public event EventHandler<string> DataTransferred;

        public bool IsConnected => _isConnected;
        public string Port1Name => _port1?.PortName;
        public string Port2Name => _port2?.PortName;

        public bool CreateBridge(string port1Name, string port2Name)
        {
            try
            {
                // Tạo 2 virtual COM ports
                _port1 = new VirtualCOMPort(port1Name);
                _port2 = new VirtualCOMPort(port2Name);

                // Đăng ký events
                _port1.DataReceived += Port1_DataReceived;
                _port1.StatusChanged += Port_StatusChanged;
                
                _port2.DataReceived += Port2_DataReceived;
                _port2.StatusChanged += Port_StatusChanged;

                // Mở ports
                bool port1Opened = _port1.Open();
                bool port2Opened = _port2.Open();

                if (port1Opened && port2Opened)
                {
                    _isConnected = true;
                    OnStatusChanged($"✅ Bridge được tạo: {port1Name} ↔ {port2Name}");
                    OnStatusChanged($"📱 App 1 có thể kết nối: \\\\.\\pipe\\VirtualCOM_{port1Name}");
                    OnStatusChanged($"📱 App 2 có thể kết nối: \\\\.\\pipe\\VirtualCOM_{port2Name}");
                    return true;
                }
                else
                {
                    OnStatusChanged("❌ Không thể tạo bridge");
                    Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Lỗi tạo bridge: {ex.Message}");
                Dispose();
                return false;
            }
        }

        private void Port1_DataReceived(object sender, DataReceivedEventArgs e)
        {
            // Chuyển tiếp dữ liệu từ Port1 sang Port2
            _port2?.Write(e.Data);
            OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] {Port1Name} → {Port2Name}: {e.Text.Trim()}");
        }

        private void Port2_DataReceived(object sender, DataReceivedEventArgs e)
        {
            // Chuyển tiếp dữ liệu từ Port2 sang Port1
            _port1?.Write(e.Data);
            OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] {Port2Name} → {Port1Name}: {e.Text.Trim()}");
        }

        private void Port_StatusChanged(object sender, string status)
        {
            OnStatusChanged(status);
        }

        public void SendTestData(string data, bool toPort1 = true)
        {
            if (!_isConnected) return;

            if (toPort1 && _port1 != null)
            {
                _port1.Write(data);
                OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] Test → {Port1Name}: {data}");
            }
            else if (!toPort1 && _port2 != null)
            {
                _port2.Write(data);
                OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] Test → {Port2Name}: {data}");
            }
        }

        public void CloseBridge()
        {
            _isConnected = false;
            
            _port1?.Close();
            _port2?.Close();
            
            OnStatusChanged("🔌 Bridge đã ngắt kết nối");
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
            
            _port1?.Dispose();
            _port2?.Dispose();
            
            _port1 = null;
            _port2 = null;
        }

        public static List<string> GetAvailableVirtualPorts()
        {
            // Gợi ý tên virtual ports
            return new List<string>
            {
                "VCOM1", "VCOM2", "VCOM3", "VCOM4",
                "VCOM5", "VCOM6", "VCOM7", "VCOM8",
                "VAPP1", "VAPP2", "VDEV1", "VDEV2"
            };
        }
    }
}