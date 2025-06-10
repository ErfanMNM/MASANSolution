using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SupportApp
{
    public partial class FMain : Form
    {
        private TcpListener tcpServer;
        private TcpClient connectedClient;
        private List<string> codesToSend = new List<string>();
        private CancellationTokenSource cts; // để dừng gửi
        private int sentCount = 0;



        public FMain()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    codesToSend.Clear();
                    opConsole.Items.Clear();

                    foreach (var line in lines)
                    {
                        var code = line.Trim();
                        if (!string.IsNullOrWhiteSpace(code))
                        {
                            codesToSend.Add(code+";");
                        }
                    }

                    opConsole.Items.Add("Số lượng code: " +codesToSend.Count);


                }
            }
        }


        private void StartTcpServer()
        {
            if (tcpServer != null)
                tcpServer.Stop();

            tcpServer = new TcpListener(IPAddress.Any, 6969);
            tcpServer.Start();
            opConsole.Items.Add("TCP Server started on port 9000. Waiting for client...");

            Thread serverThread = new Thread(() =>
            {
                connectedClient = tcpServer.AcceptTcpClient();

                opConsole.Invoke((MethodInvoker)(() =>
                {
                    opConsole.Items.Add("Client connected: " + connectedClient.Client.RemoteEndPoint);
                }));

                cts = new CancellationTokenSource();
                SendCodesToClient(cts.Token);
            });

            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void SendCodesToClient(CancellationToken token)
        {
            try
            {
                using (NetworkStream stream = connectedClient.GetStream())
                {
                    foreach (var code in codesToSend)
                    {
                        if (token.IsCancellationRequested)
                        {
                            opConsole.Invoke((MethodInvoker)(() => opConsole.Items.Add("Sending stopped by user.")));
                            break;
                        }

                        byte[] data = Encoding.UTF8.GetBytes(code + "\n");
                        stream.Write(data, 0, data.Length);

                        sentCount++;
                        lblCount.Invoke((MethodInvoker)(() => lblCount.Text = $"Sent: {sentCount}"));

                        Thread.Sleep(20); // gửi cách nhau 1 giây
                    }

                    opConsole.Invoke((MethodInvoker)(() =>
                    {
                        if (!token.IsCancellationRequested)
                            opConsole.Items.Add("All codes sent.");
                    }));
                }
            }
            catch (Exception ex)
            {
                opConsole.Invoke((MethodInvoker)(() =>
                    opConsole.Items.Add("Error: " + ex.Message)
                ));
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            StartTcpServer();
        }

        private void btnDung_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                opConsole.Items.Add("Stop requested.");
            }
        }
    }
}
