using MASAN_SERIALIZATION.Utils;
using Sunny.UI;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MASAN_SERIALIZATION.Views.Printer
{
    public partial class page_SATO : UIPage
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private CancellationTokenSource ctsRead;

        public page_SATO()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            btnSend.Click += BtnSend_Click;
            btnConnect.Click += BtnConnect_Click;

            btnConnect.Text = "Connect";
            btnSend.Text = "Send";
            btnSend.Enabled = false;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (tcpClient == null || !tcpClient.Connected)
                Connect();
            else
                Disconnect();
        }

        private void Connect()
        {
            string ip = ipIP.Text.Trim();
            if (!int.TryParse(ipPort.Text.Trim(), out int port))
            {
                AppendLog("Invalid port.");
                return;
            }

            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(ip, port);
                networkStream = tcpClient.GetStream();

                ctsRead = new CancellationTokenSource();
                _ = Task.Run(() => ReadLoop(ctsRead.Token));

                btnConnect.Text = "Disconnect";
                btnSend.Enabled = true;
                ipIP.Enabled = false;
                ipPort.Enabled = false;
                AppendLog($"Connected to {ip}:{port}");
            }
            catch (Exception ex)
            {
                AppendLog($"Connect failed: {ex.Message}");
                Disconnect();
            }
        }

        private void Disconnect()
        {
            try
            {
                ctsRead?.Cancel();
                networkStream?.Close();
                tcpClient?.Close();
            }
            catch { /* ignore */ }
            finally
            {
                networkStream = null;
                tcpClient = null;
                ctsRead = null;
                btnConnect.Text = "Connect";
                btnSend.Enabled = false;
                ipIP.Enabled = true;
                ipPort.Enabled = true;
                AppendLog("Disconnected.");
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (networkStream == null || !networkStream.CanWrite)
            {
                AppendLog("Not connected.");
                return;
            }

            string text = ipContent.Text;
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                byte[] data = Encoding.ASCII.GetBytes(text);
                networkStream.Write(data, 0, data.Length);
                AppendLog($"SEND: {text}");
            }
            catch (Exception ex)
            {
                AppendLog($"Send failed: {ex.Message}");
                Disconnect();
            }
        }

        private void ReadLoop(CancellationToken token)
        {
            byte[] buffer = new byte[4096];

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (networkStream == null)
                    {
                        Thread.Sleep(50);
                        continue;
                    }

                    if (!networkStream.DataAvailable)
                    {
                        Thread.Sleep(20);
                        continue;
                    }

                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string msg = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        AppendLog($"RECV: {msg.TrimEnd()}");
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                }
                catch (Exception ex)
                {
                    AppendLog($"Read error: {ex.Message}");
                    this.InvokeIfRequired(() => Disconnect());
                    break;
                }
            }
        }

        private void AppendLog(string msg)
        {
            if (IsDisposed || Disposing) return;

            this.InvokeIfRequired(() =>
            {
                string line = $"{DateTime.Now:HH:mm:ss} {msg}";
                opConsole.Items.Add(line);
                if (opConsole.Items.Count > 0)
                    opConsole.SelectedIndex = opConsole.Items.Count - 1;
            });
        }
    }
}
