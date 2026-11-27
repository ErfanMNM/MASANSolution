using System;
using System.Threading.Tasks;
namespace TestApp
{
    public class VirtualSerialPort : IDisposable
    {
        private readonly VirtualCOMPort port;
        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<string> StatusChanged;
        public string PortName => port.PortName;
        public bool IsOpen => port.IsListening;
        public Task Completion => port.Completion;
        public VirtualSerialPort(string portName)
        {
            port = new VirtualCOMPort(portName, "COM_Bridge_", false);
            port.DataReceived += (sender, data) => OnDataReceived(data.Data);
            port.StatusChanged += (sender, status) => OnStatusChanged(status);
        }
        public bool Open() { return port.Open(); }
        public void Close() { port.Close(); }
        public void Write(byte[] data) { port.Write(data); }
        public void Write(string text) { port.Write(text); }
        protected virtual void OnDataReceived(byte[] data)
        {
            var observers = DataReceived; if (observers == null) return;
            foreach (EventHandler<byte[]> observer in observers.GetInvocationList()) try { observer(this, data); } catch (Exception) { }
        }
        protected virtual void OnStatusChanged(string status)
        {
            var observers = StatusChanged; if (observers == null) return;
            foreach (EventHandler<string> observer in observers.GetInvocationList()) try { observer(this, status); } catch (Exception) { }
        }
        public void Dispose() { port.Dispose(); }
    }
}
