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
using System.Data.SQLite;
using Sunny.UI;

namespace TestApp
{
    public partial class CameraSimulatorForm : UIPage
    {
        private TcpListener tcpListener;
        private List<TcpClient> connectedClients;
        private bool serverRunning;
        private CancellationTokenSource cancellationTokenSource;
        private System.Threading.Timer sendTimer;
        private string[] fileLines;
        private int currentLineIndex;
        private string selectedColumn;

        public CameraSimulatorForm()
        {
            InitializeComponent();
            connectedClients = new List<TcpClient>();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Camera TCP Server Simulator";
            
            // Start server button
            uiButton_StartServer.Text = "Khởi động Server";
            uiButton_StartServer.Click += BtnStartServer_Click;
            
            // Stop server button
            uiButton_StopServer.Text = "Dừng Server";
            uiButton_StopServer.Click += BtnStopServer_Click;
            uiButton_StopServer.Enabled = false;
            
            // Send single code button
            uiButton_SendSingle.Text = "Gửi mã đơn";
            uiButton_SendSingle.Click += BtnSendSingle_Click;
            uiButton_SendSingle.Enabled = false;
            
            // Load file button
            uiButton_LoadFile.Text = "Chọn file";
            uiButton_LoadFile.Click += BtnLoadFile_Click;
            
            // Start continuous send button
            uiButton_StartContinuous.Text = "Gửi liên tục";
            uiButton_StartContinuous.Click += BtnStartContinuous_Click;
            uiButton_StartContinuous.Enabled = false;
            
            // Stop continuous send button
            uiButton_StopContinuous.Text = "Dừng gửi";
            uiButton_StopContinuous.Click += BtnStopContinuous_Click;
            uiButton_StopContinuous.Enabled = false;
            
            // Load SQLite button
            uiButton_LoadSQLite.Text = "Chọn SQLite DB";
            uiButton_LoadSQLite.Click += BtnLoadSQLite_Click;
            
            // Default values
            uiTextBox_Port.Text = "8080";
            uiIntegerUpDown_Count.Value = 10;
            uiIntegerUpDown_Delay.Value = 1000;
        }

        private async void BtnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(uiTextBox_Port.Text);
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                serverRunning = true;
                cancellationTokenSource = new CancellationTokenSource();
                
                uiButton_StartServer.Enabled = false;
                uiButton_StopServer.Enabled = true;
                uiButton_SendSingle.Enabled = true;
                
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Server đã khởi động trên cổng {port}\n");
                
                await AcceptClientsAsync(cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi khởi động server: {ex.Message}\n");
            }
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && serverRunning)
            {
                try
                {
                    var client = await AcceptTcpClientAsync(tcpListener, cancellationToken);
                    if (client != null)
                    {
                        connectedClients.Add(client);
                        uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Client kết nối: {client.Client.RemoteEndPoint}\n");
                    }
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi chấp nhận client: {ex.Message}\n");
                    }
                }
            }
        }

        private async Task<TcpClient> AcceptTcpClientAsync(TcpListener listener, CancellationToken cancellationToken)
        {
            try
            {
                return await Task.Run(() =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        if (listener.Pending())
                        {
                            return listener.AcceptTcpClient();
                        }
                        Thread.Sleep(100);
                    }
                    return null;
                }, cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        private void BtnStopServer_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            try
            {
                serverRunning = false;
                cancellationTokenSource?.Cancel();
                
                foreach (var client in connectedClients.ToList())
                {
                    client?.Close();
                }
                connectedClients.Clear();
                
                tcpListener?.Stop();
                
                uiButton_StartServer.Enabled = true;
                uiButton_StopServer.Enabled = false;
                uiButton_SendSingle.Enabled = false;
                
                StopContinuousSending();
                
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Server đã dừng\n");
            }
            catch (Exception ex)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi dừng server: {ex.Message}\n");
            }
        }

        private void BtnSendSingle_Click(object sender, EventArgs e)
        {
            string code = uiTextBox_SingleCode.Text.Trim();
            if (!string.IsNullOrEmpty(code))
            {
                SendToAllClients(code);
            }
        }

        private void BtnLoadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        fileLines = File.ReadAllLines(ofd.FileName);
                        currentLineIndex = 0;
                        uiLabel_FileStatus.Text = $"File đã tải: {fileLines.Length} dòng";
                        uiButton_StartContinuous.Enabled = serverRunning && fileLines.Length > 0;
                    }
                    catch (Exception ex)
                    {
                        uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi đọc file: {ex.Message}\n");
                    }
                }
            }
        }

        private void BtnStartContinuous_Click(object sender, EventArgs e)
        {
            if (fileLines != null && fileLines.Length > 0)
            {
                int count = (int)uiIntegerUpDown_Count.Value;
                int delay = (int)uiIntegerUpDown_Delay.Value;
                
                StartContinuousSending(count, delay);
            }
        }

        private void StartContinuousSending(int count, int delayMs)
        {
            sendTimer = new System.Threading.Timer();
            sendTimer.Interval = delayMs;
            
            int sentCount = 0;
            int maxCount = count;
            
            sendTimer.Tick += (s, e) =>
            {
                if (sentCount >= maxCount || currentLineIndex >= fileLines.Length)
                {
                    StopContinuousSending();
                    return;
                }
                
                string line = fileLines[currentLineIndex];
                SendToAllClients(line);
                
                currentLineIndex++;
                sentCount++;
                
                uiLabel_SendStatus.Text = $"Đã gửi: {sentCount}/{maxCount}";
            };
            
            sendTimer.Start();
            uiButton_StartContinuous.Enabled = false;
            uiButton_StopContinuous.Enabled = true;
            uiLabel_SendStatus.Text = "Đang gửi...";
        }

        private void BtnStopContinuous_Click(object sender, EventArgs e)
        {
            StopContinuousSending();
        }

        private void StopContinuousSending()
        {
            sendTimer?.Stop();
            sendTimer?.Dispose();
            sendTimer = null;
            
            uiButton_StartContinuous.Enabled = serverRunning && fileLines != null && fileLines.Length > 0;
            uiButton_StopContinuous.Enabled = false;
            uiLabel_SendStatus.Text = "Đã dừng";
        }

        private void BtnLoadSQLite_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadSQLiteDatabase(ofd.FileName);
                }
            }
        }

        private void LoadSQLiteDatabase(string dbPath)
        {
            try
            {
                string connectionString = $"Data Source={dbPath};Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    
                    var tables = GetTableNames(connection);
                    if (tables.Count > 0)
                    {
                        uiComboBox_Tables.Items.Clear();
                        uiComboBox_Tables.Items.AddRange(tables.ToArray());
                        uiComboBox_Tables.SelectedIndex = 0;
                        
                        LoadTableColumns(connection, tables[0]);
                        uiLabel_DBStatus.Text = $"DB đã tải: {tables.Count} bảng";
                    }
                }
            }
            catch (Exception ex)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi tải SQLite: {ex.Message}\n");
            }
        }

        private List<string> GetTableNames(SQLiteConnection connection)
        {
            var tables = new List<string>();
            using (var command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }
            return tables;
        }

        private void LoadTableColumns(SQLiteConnection connection, string tableName)
        {
            try
            {
                uiComboBox_Columns.Items.Clear();
                using (var command = new SQLiteCommand($"PRAGMA table_info({tableName})", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uiComboBox_Columns.Items.Add(reader.GetString(1));
                    }
                }
                
                if (uiComboBox_Columns.Items.Count > 0)
                {
                    uiComboBox_Columns.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi tải cột: {ex.Message}\n");
            }
        }

        private void UiComboBox_Tables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uiComboBox_Tables.SelectedItem != null)
            {
                string dbPath = uiLabel_DBStatus.Text.Contains("DB đã tải") ? 
                    uiLabel_DBStatus.Tag?.ToString() : "";
                
                if (!string.IsNullOrEmpty(dbPath))
                {
                    string connectionString = $"Data Source={dbPath};Version=3;";
                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        LoadTableColumns(connection, uiComboBox_Tables.SelectedItem.ToString());
                    }
                }
            }
        }

        private void SendToAllClients(string message)
        {
            var clientsToRemove = new List<TcpClient>();
            
            foreach (var client in connectedClients.ToList())
            {
                try
                {
                    if (client.Connected)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                        client.GetStream().Write(data, 0, data.Length);
                    }
                    else
                    {
                        clientsToRemove.Add(client);
                    }
                }
                catch
                {
                    clientsToRemove.Add(client);
                }
            }
            
            foreach (var client in clientsToRemove)
            {
                connectedClients.Remove(client);
                client?.Close();
            }
            
            uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Gửi tới {connectedClients.Count} client: {message}\n");
            uiLabel_ClientCount.Text = $"Clients kết nối: {connectedClients.Count}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopServer();
            base.OnFormClosing(e);
        }
    }
}