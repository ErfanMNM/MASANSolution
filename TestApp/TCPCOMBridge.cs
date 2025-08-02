using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestApp
{
    public class TCPCOMBridge : IDisposable
    {
        private TcpListener _server1;
        private TcpListener _server2;
        private TcpClient _client1;
        private TcpClient _client2;
        private NetworkStream _stream1;
        private NetworkStream _stream2;
        private bool _isRunning = false;
        private int _port1;
        private int _port2;

        public event EventHandler<string> StatusChanged;
        public event EventHandler<string> DataTransferred;

        public bool IsConnected => _isRunning && _client1?.Connected == true && _client2?.Connected == true;
        public int Port1 => _port1;
        public int Port2 => _port2;

        public bool CreateBridge(int tcpPort1, int tcpPort2)
        {
            try
            {
                _port1 = tcpPort1;
                _port2 = tcpPort2;

                // Tạo TCP servers
                _server1 = new TcpListener(IPAddress.Any, _port1);
                _server2 = new TcpListener(IPAddress.Any, _port2);

                _server1.Start();
                _server2.Start();

                _isRunning = true;

                OnStatusChanged($"✅ TCP Bridge được tạo!");
                OnStatusChanged($"📱 App 1 kết nối: localhost:{_port1} (Raw TCP)");
                OnStatusChanged($"📱 App 2 kết nối: localhost:{_port2} (Raw TCP)");
                OnStatusChanged($"💡 Hercules: TCP Client → localhost:{_port1} và localhost:{_port2}");

                // Bắt đầu listen cho connections
                _ = Task.Run(ListenForConnections);

                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Lỗi tạo TCP bridge: {ex.Message}");
                return false;
            }
        }

        private async Task ListenForConnections()
        {
            try
            {
                OnStatusChanged("🔄 Đang đợi 2 ứng dụng kết nối...");

                // Đợi 2 clients kết nối
                var task1 = _server1.AcceptTcpClientAsync();
                var task2 = _server2.AcceptTcpClientAsync();

                _client1 = await task1;
                _client2 = await task2;

                _stream1 = _client1.GetStream();
                _stream2 = _client2.GetStream();

                OnStatusChanged("🎯 Cả 2 ứng dụng đã kết nối! Bridge hoạt động!");

                // Bắt đầu bridge data
                var bridgeTask1 = Task.Run(() => BridgeData(_stream1, _stream2, "App1", "App2"));
                var bridgeTask2 = Task.Run(() => BridgeData(_stream2, _stream1, "App2", "App1"));

                await Task.WhenAny(bridgeTask1, bridgeTask2);
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Lỗi kết nối: {ex.Message}");
            }
        }

        private async Task BridgeData(NetworkStream fromStream, NetworkStream toStream, string fromApp, string toApp)
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (_isRunning && fromStream.CanRead && toStream.CanWrite)
                {
                    int bytesRead = await fromStream.ReadAsync(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        await toStream.WriteAsync(buffer, 0, bytesRead);
                        await toStream.FlushAsync();

                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] {fromApp} → {toApp}: {data.Trim()}");
                    }
                    else
                    {
                        // Connection closed
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Bridge error: {ex.Message}");
            }
        }

        public void SendTestData(string data, bool toPort1 = true)
        {
            try
            {
                if (!IsConnected) return;

                byte[] bytes = Encoding.UTF8.GetBytes(data + "\r\n");
                
                if (toPort1 && _stream1?.CanWrite == true)
                {
                    _stream1.Write(bytes, 0, bytes.Length);
                    _stream1.Flush();
                    OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] Test → App1: {data}");
                }
                else if (!toPort1 && _stream2?.CanWrite == true)
                {
                    _stream2.Write(bytes, 0, bytes.Length);
                    _stream2.Flush();
                    OnDataTransferred($"[{DateTime.Now:HH:mm:ss}] Test → App2: {data}");
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
                _isRunning = false;

                _stream1?.Close();
                _stream2?.Close();
                _client1?.Close();
                _client2?.Close();
                _server1?.Stop();
                _server2?.Stop();

                OnStatusChanged("🔌 TCP Bridge đã đóng");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"❌ Close error: {ex.Message}");
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
        }

        public static List<int> GetAvailableTCPPorts()
        {
            return new List<int>
            {
                8001, 8002, 8003, 8004, 8005, 8006,
                9001, 9002, 9003, 9004, 9005, 9006,
                10001, 10002, 10003, 10004, 10005, 10006
            };
        }
    }
}