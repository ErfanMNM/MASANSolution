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
    public class CameraInstance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public TcpListener TcpListener { get; set; }
        public List<TcpClient> ConnectedClients { get; set; }
        public bool ServerRunning { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public BackgroundWorker SendWorker { get; set; }
        public string[] FileLines { get; set; }
        public int CurrentLineIndex { get; set; }
        public int SendCount { get; set; }
        public int MaxCount { get; set; }
        public int DelayMs { get; set; }
        public bool IsSending { get; set; }
        internal readonly object ClientsGate = new object();
        internal readonly object SendGate = new object();
        internal Task AcceptTask = Task.CompletedTask;
        internal string[] SendingLines;
        
        public CameraInstance(int id, string name, int port)
        {
            Id = id;
            Name = name;
            Port = port;
            ConnectedClients = new List<TcpClient>();
            ServerRunning = false;
            SendWorker = new BackgroundWorker();
            SendWorker.WorkerReportsProgress = true;
            SendWorker.WorkerSupportsCancellation = true;
            CurrentLineIndex = 0;
            SendCount = 0;
            IsSending = false;
        }
    }

    // No UI or device dependencies: every listener generation owns its clients.
    internal static class CameraSimulatorCore
    {
        internal static int ClientCount(CameraInstance camera)
        {
            lock (camera.ClientsGate) return camera.ConnectedClients.Count;
        }

        internal static void Start(CameraInstance camera, int port, Action<string> log, Action changed, IPAddress address = null)
        {
            lock (camera.ClientsGate)
            {
                if (camera.ServerRunning) return;
                var listener = new TcpListener(address ?? IPAddress.Any, port);
                try { listener.Start(); }
                catch { listener.Stop(); throw; }
                var stop = new CancellationTokenSource();
                camera.Port = ((IPEndPoint)listener.LocalEndpoint).Port;
                camera.TcpListener = listener;
                camera.CancellationTokenSource = stop;
                camera.ServerRunning = true;
                camera.AcceptTask = Task.Run(() => Accept(camera, listener, stop, log, changed));
            }
        }

        private static async Task Accept(CameraInstance camera, TcpListener listener,
            CancellationTokenSource stop, Action<string> log, Action changed)
        {
            var token = stop.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    bool accepted;
                    lock (camera.ClientsGate)
                    {
                        accepted = camera.ServerRunning && !token.IsCancellationRequested && ReferenceEquals(camera.TcpListener, listener);
                        if (accepted) camera.ConnectedClients.Add(client);
                    }
                    if (!accepted) { client.Close(); break; }
                    log("Client kết nối");
                    changed();
                }
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested) log("Lỗi chấp nhận client: " + ex.Message);
            }
            finally
            {
                TcpClient[] clients = new TcpClient[0];
                lock (camera.ClientsGate)
                {
                    if (ReferenceEquals(camera.TcpListener, listener))
                    {
                        camera.ServerRunning = false;
                        camera.TcpListener = null;
                        camera.CancellationTokenSource = null;
                        clients = camera.ConnectedClients.ToArray();
                        camera.ConnectedClients.Clear();
                    }
                }
                foreach (var client in clients) client.Close();
                listener.Stop();
                stop.Dispose();
                changed();
            }
        }

        internal static void Stop(CameraInstance camera)
        {
            TcpListener listener;
            TcpClient[] clients;
            lock (camera.ClientsGate)
            {
                camera.ServerRunning = false;
                camera.CancellationTokenSource?.Cancel();
                camera.CancellationTokenSource = null;
                listener = camera.TcpListener;
                camera.TcpListener = null;
                clients = camera.ConnectedClients.ToArray();
                camera.ConnectedClients.Clear();
            }
            listener?.Stop();
            foreach (var client in clients) client.Close();
        }

        internal static Task<int> BroadcastAsync(CameraInstance camera, string message, Action<TcpClient, byte[]> writer = null)
        {
            TcpListener listener;
            lock (camera.ClientsGate) listener = camera.TcpListener;
            return Task.Run(() => Broadcast(camera, message, writer, listener));
        }

        internal static int Broadcast(CameraInstance camera, string message, Action<TcpClient, byte[]> writer = null)
        {
            TcpListener listener;
            lock (camera.ClientsGate) listener = camera.TcpListener;
            return Broadcast(camera, message, writer, listener);
        }

        private static int Broadcast(CameraInstance camera, string message, Action<TcpClient, byte[]> writer, TcpListener listener)
        {
            // Serialize sends without holding the client-list gate over network I/O.
            lock (camera.SendGate)
            {
                TcpClient[] clients;
                lock (camera.ClientsGate)
                {
                    if (!ReferenceEquals(camera.TcpListener, listener)) return 0;
                    clients = camera.ConnectedClients.ToArray();
                }
                byte[] data = Encoding.UTF8.GetBytes(message);
                int sent = 0;
                foreach (var client in clients)
                {
                    try
                    {
                        if (writer != null) writer(client, data);
                        else
                        {
                            client.SendTimeout = 1000;
                            client.GetStream().Write(data, 0, data.Length);
                        }
                        sent++;
                    }
                    catch
                    {
                        lock (camera.ClientsGate) camera.ConnectedClients.Remove(client);
                        client.Close();
                    }
                }
                return sent;
            }
        }

        internal static string[] ReadCsvFirstColumn(string path, bool hasHeader)
        {
            var lines = new List<string>();
            using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(path, Encoding.UTF8, true))
            {
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = false;
                if (hasHeader && !parser.EndOfData) parser.ReadFields();
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields != null && fields.Length > 0 && !string.IsNullOrWhiteSpace(fields[0]))
                        lines.Add(fields[0].Trim());
                }
            }
            return lines.ToArray();
        }
    }

    public partial class CameraSimulatorForm : UIPage
    {
        private Dictionary<int, CameraInstance> cameras;
        private int selectedCameraId;
        private const int MAX_CAMERAS = 3;
        private string currentDatabasePath;

        private void PostUI(Action action)
        {
            if (IsDisposed || Disposing) return;
            try
            {
                if (InvokeRequired) BeginInvoke(new Action(() => { if (!IsDisposed && !Disposing) action(); }));
                else action();
            }
            catch (InvalidOperationException) { }
        }

        private void StartServer(CameraInstance camera, int port)
        {
            CameraSimulatorCore.Start(camera, port,
                message => PostUI(() => uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} {message}\n")),
                () => PostUI(() => { if (camera.Id == selectedCameraId) UpdateCameraSelection(); }));
        }

        public CameraSimulatorForm()
        {
            InitializeComponent();

            cameras = new Dictionary<int, CameraInstance>();
            selectedCameraId = 1;
            InitializeCameras();
            InitializeUI();
        }
        
        private void InitializeCameras()
        {
            for (int i = 1; i <= MAX_CAMERAS; i++)
            {
                var camera = new CameraInstance(i, $"Camera {i}", 8080 + i - 1);
                camera.SendWorker.DoWork += SendWorker_DoWork;
                camera.SendWorker.ProgressChanged += SendWorker_ProgressChanged;
                camera.SendWorker.RunWorkerCompleted += SendWorker_RunWorkerCompleted;
                cameras[i] = camera;
            }
        }

        private void InitializeUI()
        {
            this.Text = "🖥️ Camera TCP Server Simulator Pro - Multi Instance Manager";
            this.BackColor = System.Drawing.Color.FromArgb(245, 246, 247);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            
            // Start server button
            uiButton_StartServer.Text = "▶️ Khởi động";
            uiButton_StartServer.Click += BtnStartServer_Click;
            
            // Stop server button
            uiButton_StopServer.Text = "⏹️ Dừng";
            uiButton_StopServer.Click += BtnStopServer_Click;
            uiButton_StopServer.Enabled = false;
            
            // Send single code button
            uiButton_SendSingle.Text = "📤 Gửi";
            uiButton_SendSingle.Click += BtnSendSingle_Click;
            uiButton_SendSingle.Enabled = false;
            
            // Load file button
            uiButton_LoadFile.Text = "📄 TXT";
            uiButton_LoadFile.Click += BtnLoadFile_Click;
            
            // Load CSV button
            uiButton_LoadCSV.Text = "📈 CSV";
            uiButton_LoadCSV.Click += BtnLoadCSV_Click;
            
            // Data Wizard button
            uiButton_DataWizard.Text = "🧝‍♂️ Wizard";
            uiButton_DataWizard.Click += BtnDataWizard_Click;
            
            // Send range button
            uiButton_SendRange.Text = "📦 Dải mã";
            uiButton_SendRange.Click += BtnSendRange_Click;
            uiButton_SendRange.Enabled = false;
            
            // Start continuous send button
            uiButton_StartContinuous.Text = "▶️ Bắt đầu";
            uiButton_StartContinuous.Click += BtnStartContinuous_Click;
            uiButton_StartContinuous.Enabled = false;
            
            // Stop continuous send button
            uiButton_StopContinuous.Text = "⏹️ Dừng";
            uiButton_StopContinuous.Click += BtnStopContinuous_Click;
            uiButton_StopContinuous.Enabled = false;
            
            // Load SQLite button
            uiButton_LoadSQLite.Text = "🗄 Chọn DB";
            uiButton_LoadSQLite.Click += BtnLoadSQLite_Click;
            
            // Load Column Data button
            uiButton_LoadColumnData.Text = "⬇️ Tải";
            uiButton_LoadColumnData.Click += BtnLoadColumnData_Click;
            uiButton_LoadColumnData.Enabled = false;
            
            // Start/Stop all cameras buttons
            uiButton_StartAllCameras.Text = "▶️ Tất cả";
            uiButton_StartAllCameras.Click += BtnStartAllCameras_Click;
            
            uiButton_StopAllCameras.Text = "⏹️ Tất cả";
            uiButton_StopAllCameras.Click += BtnStopAllCameras_Click;
            
            // Initialize camera selection dropdown
            uiComboBox_CameraSelect.Items.Clear();
            foreach (var camera in cameras.Values)
            {
                uiComboBox_CameraSelect.Items.Add($"Camera {camera.Id} (Port {camera.Port})");
            }
            uiComboBox_CameraSelect.SelectedIndex = 0;
            
            // Default values
            uiIntegerUpDown_Count.Value = 10;
            uiIntegerUpDown_Delay.Value = 1000;
            
            // Update UI to show current camera
            UpdateCameraSelection();
        }
        
        private void UpdateCameraSelection()
        {
            var camera = cameras[selectedCameraId];
            uiTextBox_Port.Text = camera.Port.ToString();
            uiLabel_ClientCount.Text = $"Camera {camera.Id} - Clients: {CameraSimulatorCore.ClientCount(camera)}";
            
            uiButton_StartServer.Enabled = !camera.ServerRunning;
            uiButton_StopServer.Enabled = camera.ServerRunning;
            uiButton_SendSingle.Enabled = camera.ServerRunning;
            uiButton_StartContinuous.Enabled = camera.ServerRunning && camera.FileLines != null && camera.FileLines.Length > 0 && !camera.IsSending;
            uiButton_StopContinuous.Enabled = camera.IsSending;
            uiButton_SendRange.Enabled = camera.ServerRunning && !string.IsNullOrEmpty(uiTextBox_FromCode.Text) && !string.IsNullOrEmpty(uiTextBox_ToCode.Text);
            
            // Update file status label if camera has data
            if (camera.FileLines != null && camera.FileLines.Length > 0)
            {
                if (string.IsNullOrEmpty(uiLabel_FileStatus.Text) || !uiLabel_FileStatus.Text.Contains($"Camera {camera.Id}"))
                {
                    uiLabel_FileStatus.Text = $"Camera {camera.Id}: {camera.FileLines.Length} dòng dữ liệu";
                }
            }
            else
            {
                if (camera.Id == selectedCameraId)
                {
                    uiLabel_FileStatus.Text = $"Camera {camera.Id}: Chưa có dữ liệu";
                }
            }
        }

        private void BtnStartServer_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            try
            {
                int port = int.Parse(uiTextBox_Port.Text);
                StartServer(camera, port);
                UpdateCameraSelection();
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Server đã khởi động trên cổng {camera.Port}\n");
            }
            catch (Exception ex)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi khởi động Camera {camera.Id} server: {ex.Message}\n");
            }
        }

        private void BtnStopServer_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            StopServer(camera);
        }

        private void StopServer(CameraInstance camera)
        {
            CameraSimulatorCore.Stop(camera);
            StopContinuousSending(camera);
            uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Server đã dừng\n");
            if (camera.Id == selectedCameraId) UpdateCameraSelection();
        }

        private async void BtnSendSingle_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            string code = uiTextBox_SingleCode.Text.Trim();
            if (!string.IsNullOrEmpty(code)) await SendToAllClientsAsync(camera, code);
        }

        private void BtnLoadFile_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        camera.FileLines = File.ReadAllLines(ofd.FileName);
                        camera.CurrentLineIndex = 0;
                        uiLabel_FileStatus.Text = $"Camera {camera.Id} File đã tải: {camera.FileLines.Length} dòng";
                        UpdateCameraSelection();
                    }
                    catch (Exception ex)
                    {
                        uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi đọc file cho Camera {camera.Id}: {ex.Message}\n");
                    }
                }
            }
        }

        private void BtnStartContinuous_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            if (camera.FileLines != null && camera.FileLines.Length > 0)
            {
                int count = (int)uiIntegerUpDown_Count.Value;
                int delay = (int)uiIntegerUpDown_Delay.Value;
                
                StartContinuousSending(camera, count, delay);
            }
        }

        private void StartContinuousSending(CameraInstance camera, int count, int delayMs)
        {
            if (camera.SendWorker.IsBusy)
                return;
                
            if (count <= 0 || delayMs < 0 || camera.FileLines == null || camera.FileLines.Length == 0) return;
            camera.SendingLines = (string[])camera.FileLines.Clone();
            camera.MaxCount = count;
            camera.DelayMs = delayMs;
            camera.SendCount = 0;
            camera.CurrentLineIndex = 0;
            camera.IsSending = true;
            
            camera.SendWorker.RunWorkerAsync(camera);
            
            if (camera.Id == selectedCameraId)
            {
                UpdateCameraSelection();
                uiLabel_SendStatus.Text = $"Camera {camera.Id} Đang gửi...";
            }
        }

        private void BtnStopContinuous_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            StopContinuousSending(camera);
        }

        private void StopContinuousSending(CameraInstance camera)
        {
            if (camera.SendWorker.IsBusy)
            {
                camera.SendWorker.CancelAsync();
            }
            
            camera.IsSending = false;
            
            if (camera.Id == selectedCameraId)
            {
                UpdateCameraSelection();
                uiLabel_SendStatus.Text = $"Camera {camera.Id} Đã dừng";
            }
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
                        currentDatabasePath = dbPath;
                        uiButton_LoadColumnData.Enabled = uiComboBox_Columns.Items.Count > 0;
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
                    uiButton_LoadColumnData.Enabled = true;
                }
                else
                {
                    uiButton_LoadColumnData.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi tải cột: {ex.Message}\n");
            }
        }

        private void UiComboBox_Tables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uiComboBox_Tables.SelectedItem != null && !string.IsNullOrEmpty(currentDatabasePath))
            {
                string connectionString = $"Data Source={currentDatabasePath};Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    LoadTableColumns(connection, uiComboBox_Tables.SelectedItem.ToString());
                }
            }
        }

        private async Task SendToAllClientsAsync(CameraInstance camera, string message)
        {
            int sent = await CameraSimulatorCore.BroadcastAsync(camera, message).ConfigureAwait(false);
            PostUI(() =>
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Gửi tới {sent} client: {message}\n");
                if (camera.Id == selectedCameraId)
                    uiLabel_ClientCount.Text = $"Camera {camera.Id} - Clients: {CameraSimulatorCore.ClientCount(camera)}";
            });
        }

        private void SendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var camera = (CameraInstance)e.Argument;
            var worker = (BackgroundWorker)sender;
            var lines = camera.SendingLines;
            int index = 0;
            while (camera.SendCount < camera.MaxCount && index < lines.Length)
            {
                if (worker.CancellationPending) { e.Cancel = true; break; }
                SendToAllClientsAsync(camera, lines[index++]).GetAwaiter().GetResult();
                camera.CurrentLineIndex = index;
                camera.SendCount++;
                worker.ReportProgress((int)((double)camera.SendCount / camera.MaxCount * 100), camera);
                for (int remaining = camera.DelayMs; remaining > 0 && !worker.CancellationPending;)
                {
                    int pause = Math.Min(remaining, 50);
                    Thread.Sleep(pause);
                    remaining -= pause;
                }
            }
            if (worker.CancellationPending) e.Cancel = true;
        }

        private void SendWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var camera = (CameraInstance)e.UserState;
            
            if (camera.Id == selectedCameraId)
            {
                uiLabel_SendStatus.Text = $"Camera {camera.Id} Đã gửi: {camera.SendCount}/{camera.MaxCount}";
            }
        }
        
        private void SendWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var camera = cameras.Values.FirstOrDefault(c => c.SendWorker == worker);
            
            if (camera != null)
            {
                camera.IsSending = false;
                
                if (camera.Id == selectedCameraId)
                {
                    UpdateCameraSelection();
                    if (e.Error != null)
                    {
                        uiLabel_SendStatus.Text = $"Camera {camera.Id} Lỗi gửi: {e.Error.Message}";
                    }
                    else if (e.Cancelled)
                    {
                        uiLabel_SendStatus.Text = $"Camera {camera.Id} Đã hủy";
                    }
                    else
                    {
                        uiLabel_SendStatus.Text = $"Camera {camera.Id} Hoàn thành: {camera.SendCount}/{camera.MaxCount}";
                    }
                }
            }
        }
        
        private void UiComboBox_CameraSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCameraId = uiComboBox_CameraSelect.SelectedIndex + 1;
            UpdateCameraSelection();
        }
        
        private void BtnStartAllCameras_Click(object sender, EventArgs e)
        {
            foreach (var camera in cameras.Values)
            {
                if (camera.ServerRunning) continue;
                try
                {
                    StartServer(camera, camera.Port);
                    uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Server đã khởi động trên cổng {camera.Port}\n");
                }
                catch (Exception ex)
                {
                    uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi khởi động Camera {camera.Id} server: {ex.Message}\n");
                }
            }
            UpdateCameraSelection();
        }

        private void BtnStopAllCameras_Click(object sender, EventArgs e)
        {
            foreach (var camera in cameras.Values)
            {
                if (camera.ServerRunning)
                {
                    StopServer(camera);
                }
            }
            
            UpdateCameraSelection();
            uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Đã dừng tất cả cameras\n");
        }
        
        private void BtnLoadColumnData_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            
            if (string.IsNullOrEmpty(currentDatabasePath) || 
                uiComboBox_Tables.SelectedItem == null || 
                uiComboBox_Columns.SelectedItem == null)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Vui lòng chọn DB, bảng và cột\n");
                return;
            }
            
            try
            {
                string connectionString = $"Data Source={currentDatabasePath};Version=3;";
                string tableName = uiComboBox_Tables.SelectedItem.ToString();
                string columnName = uiComboBox_Columns.SelectedItem.ToString();
                
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    
                    // Load data from selected column
                    string query = $"SELECT [{columnName}] FROM [{tableName}] WHERE [{columnName}] IS NOT NULL AND [{columnName}] != ''";
                    using (var command = new SQLiteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        var dataList = new List<string>();
                        while (reader.Read())
                        {
                            string value = reader.GetValue(0)?.ToString();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                dataList.Add(value.Trim());
                            }
                        }
                        
                        if (dataList.Count > 0)
                        {
                            camera.FileLines = dataList.ToArray();
                            camera.CurrentLineIndex = 0;
                            uiLabel_FileStatus.Text = $"Camera {camera.Id} SQLite: {dataList.Count} bản ghi từ {columnName}";
                            uiLabel_RecordCount.Text = $"Đã tải {dataList.Count} bản ghi";
                            UpdateCameraSelection();
                            
                            uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Đã tải {dataList.Count} bản ghi từ cột {columnName}\n");
                        }
                        else
                        {
                            uiLabel_RecordCount.Text = "Không có dữ liệu";
                            uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Không tìm thấy dữ liệu trong cột {columnName}\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Lỗi tải dữ liệu SQLite: {ex.Message}\n");
                uiLabel_RecordCount.Text = $"Lỗi: {ex.Message}";
            }
        }
        
        private void BtnLoadCSV_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var header = MessageBox.Show(this, "Dòng đầu tiên có phải là tiêu đề CSV?", "Nạp CSV",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (header == DialogResult.Cancel) return;
                        camera.FileLines = CameraSimulatorCore.ReadCsvFirstColumn(ofd.FileName, header == DialogResult.Yes);
                        camera.CurrentLineIndex = 0;
                        uiLabel_FileStatus.Text = $"Camera {camera.Id} CSV đã tải: {camera.FileLines.Length} dòng";
                        UpdateCameraSelection();
                    }
                    catch (Exception ex)
                    {
                        uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Lỗi đọc CSV cho Camera {camera.Id}: {ex.Message}\n");
                    }
                }
            }
        }
        
        private void BtnDataWizard_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            
            // Simple data wizard dialog
            using (var wizard = new DataWizardForm())
            {
                if (wizard.ShowDialog() == DialogResult.OK)
                {
                    camera.FileLines = wizard.GeneratedData;
                    camera.CurrentLineIndex = 0;
                    uiLabel_FileStatus.Text = $"Camera {camera.Id} Wizard: {camera.FileLines.Length} dòng";
                    UpdateCameraSelection();
                }
            }
        }
        
        private void BtnSendRange_Click(object sender, EventArgs e)
        {
            var camera = cameras[selectedCameraId];
            string fromCode = uiTextBox_FromCode.Text.Trim();
            string toCode = uiTextBox_ToCode.Text.Trim();
            
            if (string.IsNullOrEmpty(fromCode) || string.IsNullOrEmpty(toCode))
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Vui lòng nhập mã bắt đầu và kết thúc\n");
                return;
            }
            
            try
            {
                // Try to parse as numbers first
                if (long.TryParse(fromCode, out long fromNum) && long.TryParse(toCode, out long toNum))
                {
                    if (fromNum > toNum)
                    {
                        uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Mã bắt đầu phải nhỏ hơn mã kết thúc\n");
                        return;
                    }
                    
                    var rangeData = new List<string>();
                    for (long i = fromNum; i <= toNum; i++)
                    {
                        rangeData.Add(i.ToString().PadLeft(fromCode.Length, '0'));
                    }
                    
                    camera.FileLines = rangeData.ToArray();
                    camera.CurrentLineIndex = 0;
                    uiLabel_FileStatus.Text = $"Camera {camera.Id} Dải số: {fromCode}-{toCode} ({camera.FileLines.Length} mã)";
                    UpdateCameraSelection();
                    
                    uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Đã tạo dải mã từ {fromCode} đến {toCode}\n");
                }
                else
                {
                    // Handle as string range (alphabetical)
                    uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Chưa hỗ trợ dải chuỗi, vui lòng sử dụng số\n");
                }
            }
            catch (Exception ex)
            {
                uiRichTextBox_Log.AppendText($"[{DateTime.Now}] Camera {camera.Id} Lỗi tạo dải mã: {ex.Message}\n");
            }
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (var camera in cameras.Values)
            {
                StopServer(camera);
            }
            base.OnFormClosing(e);
        }
    }
}
