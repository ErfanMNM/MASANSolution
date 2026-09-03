using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private e_PrinterStatus _printerStatus = e_PrinterStatus.DISCONNECT;

        private DataTable sentCodesL1= new DataTable();

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        private string WH = "150;150";
        private string LR = "4;4";
        private string ClearPrinter = "AAA";

        private string code = "\u001b10104680825412637215TGkBR(b;RunM\u001b191EE11\u001b192VFnOdvOodJm/5wO2JBj+9U0dq253icpdAd8Tx3E3vUE=";
        private string MainCode = "\u001bA\u001bA3V+00000H+0000\u001bCS4\u001b#F7\u001bA1V00300H0300\u001b%0\u001bH0045\u001bV00044\u001b2D51,06,06,000,000\u001bDN0089,\u001b10104680825412637215TGkBR(b;RunM\u001b191EE11\u001b192VFnOdvOodJm/5wO2JBj+9U0dq253icpdAd8Tx3E3vUE=\u001bQ1\u001bZ\u0003\u001bZ\u001b";
        public page_SATO()
        {
            InitializeComponent();
            InitUI();
            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                AppendLog($"Connect failed: {ex.Message}");
                Disconnect();
            }

            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            if(backgroundWorker.IsBusy == false)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!backgroundWorker.CancellationPending)
            {
                switch (Globals.Production_State)
                {
                    case e_Production_State.Running:
                        if (sentCodesL1.Rows.Count <= 0)
                        {
                            GetPrinterJobAll();
                        }
                        else
                        {


                            if (Globals.Printer_Job < 10)
                            {
                                //SentToPrinter();
                            }
                        }
                        break;
                    case e_Production_State.Printer_Loading:
                        break;

                }
                Thread.Sleep(100); // Tạm dừng 1 giây trước khi gửi tiếp
            }
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
                opStatus.Text= "ĐANG KẾT NỐI";
                opStatus.BackColor = System.Drawing.Color.Green;
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
                opStatus.Text = "CHƯA KẾT NỐI";
                opStatus.BackColor = System.Drawing.Color.Red;
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

        private void btnGetPrinterJob_Click(object sender, EventArgs e)
        {
            //lấy danh sách code vào bảng chờ
            var a = Globals.ProductionData.getDataPO.Get_Codes_Printer(Globals.ProductionData.orderNo);
            sentCodesL1 = a.Codes;
            AppendLog($"Get Codes: {sentCodesL1.Rows.Count} rows");

        }

        private void GetPrinterJobAll()
        {
            //lấy danh sách code vào bảng chờ
            var a = Globals.ProductionData.getDataPO.Get_Codes_Printer(Globals.ProductionData.orderNo);
            sentCodesL1 = a.Codes;
            AppendLog($"Get Codes: {sentCodesL1.Rows.Count} rows");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (networkStream == null || !networkStream.CanWrite)
            {
                AppendLog("Not connected.");
                return;
            }

            string text = ipClearPrinter.Text;
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

        private void btnSentToPrinter_Click(object sender, EventArgs e)
        {
            SentToPrinter();
        }

        private void SentToPrinter()
        {
            WH= uiTextBox2.Text;
            LR= uiTextBox1.Text;
            //lấy các dòng code từ điểm bắt đầu
            int stopIndex = Globals.Printer_Counter+5;
            int startIndex = Globals.Printer_Counter;
            for (int i = startIndex; i < stopIndex; i++)
            {
                if (i < sentCodesL1.Rows.Count)
                {
                    string codeS = sentCodesL1.Rows[i]["Code"].ToString();
                    codeS = codeS.Replace("\u001d", "\u001b1").Replace("\u001D", "\u001b1"); ;
                    string test = "\u001bA\u001bA3V+00000H+0000\u001bCS4\u001b#F7\u001bA1V00200H0200\u001b%0\u001bH0055\u001bV00005\u001b2D51,06,06,000,000\u001bDN0041,\u001b10104630024630332215eAUgrLL&anYm\u001b193fKvI\u001bQ1\u001bZ\u0003\u001bZ\u001b";
                    string testcode = $"\u001bA\u001bA3V+00000H+0000\u001bCS4\u001b#F7\u001bA1V00200H0200\u001b%0\u001bH0055\u001bV00005\u001b2D51,06,06,000,000\u001bDN0041,\u001b1{codeS}\u001bQ1\u001bZ\u0003\u001bZ\u001b";
                    string codeToSend = $"\u001bA\u001bA3V+00000H+0000\u001bCS4\u001b#F7\u001bA1V{WH.Split(";")[0]}H{WH.Split(";")[1]}\u001b%0\u001bH{LR.Split(";")[0]}\u001bV{LR.Split(";")[1]}\u001b2D51,06,06,000,000\u001bDN0041,\u001b1{codeS}\u001bQ1\u001bZ\u0003\u001bZ\u001b";
                    try
                    {
                        byte[] data = Encoding.ASCII.GetBytes(codeToSend);
                        networkStream.Write(data, 0, data.Length);
                        AppendLog($"SEND: {codeToSend}");

                        Globals.Printer_Counter++; 
                        Globals.Printer_Job++ ;
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"Send failed: {ex.Message}");
                        Disconnect();
                        break;
                    }
                }

                Thread.Sleep(100); // Tạm dừng 1 giây trước khi gửi tiếp
            }


            if (networkStream == null || !networkStream.CanWrite)
            {
                AppendLog("Not connected.");
                return;
            }
            if (string.IsNullOrEmpty(code)) return;
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(code);
                networkStream.Write(data, 0, data.Length);
                AppendLog($"SEND: {code}");
            }
            catch (Exception ex)
            {
                AppendLog($"Send failed: {ex.Message}");
                Disconnect();
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {

        }
    }

    public enum e_PrinterStatus
    {
        CONNECT = 1,
        DISCONNECT = 0,
    }
}
