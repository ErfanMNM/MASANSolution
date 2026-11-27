using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    public class VirtualCOMBridge : IDisposable
    {
        private VirtualCOMPort _port1;
        private VirtualCOMPort _port2;
        private readonly object gate = new object();
        private EventHandler<DataReceivedEventArgs> firstHandler, secondHandler;
        private bool _isConnected = false;
        private bool disposed;

        public event EventHandler<string> StatusChanged;
        public event EventHandler<string> DataTransferred;

        public bool IsConnected { get { lock (gate) return _isConnected; } }
        public string Port1Name { get { lock (gate) return _port1?.PortName; } }
        public string Port2Name { get { lock (gate) return _port2?.PortName; } }

        public bool CreateBridge(string port1Name, string port2Name)
        {
            string error = null;
            lock (gate)
            {
              if (disposed) return false;
              CloseBridge();
              try
              {
                // Tạo 2 virtual COM ports
                _port1 = new VirtualCOMPort(port1Name, "VirtualCOM_", false);
                _port2 = new VirtualCOMPort(port2Name, "VirtualCOM_", false);

                // Đăng ký events
                var first = _port1; var second = _port2;
                firstHandler = (source, data) =>
                {
                    lock (gate)
                    {
                        if (!_isConnected || !ReferenceEquals(_port1, first) || !ReferenceEquals(_port2, second)) return;
                        second.Write(data.Data);
                    }
                    if (IsCurrent(first, second)) OnDataTransferred(first.PortName + " → " + second.PortName + ": " + data.Text);
                };
                first.DataReceived += firstHandler;
                
                secondHandler = (source, data) =>
                {
                    lock (gate)
                    {
                        if (!_isConnected || !ReferenceEquals(_port1, first) || !ReferenceEquals(_port2, second)) return;
                        first.Write(data.Data);
                    }
                    if (IsCurrent(first, second)) OnDataTransferred(second.PortName + " → " + first.PortName + ": " + data.Text);
                };
                second.DataReceived += secondHandler;

                // Mở ports
                bool port1Opened = _port1.Open();
                bool port2Opened = _port2.Open();

                if (port1Opened && port2Opened)
                {
                    _isConnected = true;
                    _port1.StatusChanged += Port_StatusChanged;
                    _port2.StatusChanged += Port_StatusChanged;
                }
                else
                {
                    error = "❌ Không thể tạo bridge";
                    CloseBridge();
                }
              }
              catch (Exception ex)
              {
                error = $"❌ Lỗi tạo bridge: {ex.Message}";
                CloseBridge();
              }
            }
            OnStatusChanged(error ?? $"✅ Bridge được tạo: {port1Name} ↔ {port2Name}");
            return error == null;
        }

        private bool IsCurrent(VirtualCOMPort first, VirtualCOMPort second)
        {
            lock (gate) return _isConnected && ReferenceEquals(_port1, first) && ReferenceEquals(_port2, second);
        }

        private void Port_StatusChanged(object sender, string status)
        {
            lock (gate) if (!ReferenceEquals(sender, _port1) && !ReferenceEquals(sender, _port2)) return;
            OnStatusChanged(status);
        }

        public void SendTestData(string data, bool toPort1 = true)
        {
            string message = null;
            lock (gate)
            {
            if (!_isConnected) return;

            if (toPort1 && _port1 != null)
            {
                _port1.Write(data);
                message = $"[{DateTime.Now:HH:mm:ss}] Test → {Port1Name}: {data}";
            }
            else if (!toPort1 && _port2 != null)
            {
                _port2.Write(data);
                message = $"[{DateTime.Now:HH:mm:ss}] Test → {Port2Name}: {data}";
            }
            }
            if (message != null) OnDataTransferred(message);
        }

        public void CloseBridge()
        {
          lock (gate)
          {
            _isConnected = false;
            var first = _port1; var second = _port2; _port1 = null; _port2 = null;
            if (first != null) { first.DataReceived -= firstHandler; first.StatusChanged -= Port_StatusChanged; }
            if (second != null) { second.DataReceived -= secondHandler; second.StatusChanged -= Port_StatusChanged; }
            firstHandler = null; secondHandler = null;
            try { first?.Dispose(); } finally { second?.Dispose(); }
          }
        }

        protected virtual void OnStatusChanged(string status)
        {
            var observers = StatusChanged;
            if (observers != null) foreach (EventHandler<string> observer in observers.GetInvocationList())
                try { observer(this, status); } catch (Exception) { }
        }

        protected virtual void OnDataTransferred(string data)
        {
            var observers = DataTransferred;
            if (observers != null) foreach (EventHandler<string> observer in observers.GetInvocationList())
                try { observer(this, data); } catch (Exception) { }
        }

        public void Dispose() { lock (gate) { if (disposed) return; disposed = true; CloseBridge(); } }

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
