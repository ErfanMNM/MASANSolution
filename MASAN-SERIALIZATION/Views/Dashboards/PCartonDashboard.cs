using DATALOGIC_SCAN;
using Google.Apis.Storage.v1.Data;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using MASAN_SERIALIZATION.Views.Database;
using SpT.Logs;
using Sunny.UI;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public partial class PCartonDashboard : UIPage
    {

        private const int WM_INPUT = 0x00FF;

        private const uint RIM_TYPEKEYBOARD = 1;

        private const uint RID_INPUT = 0x10000003;
        private const uint RIDI_DEVICENAME = 0x20000007;
        // Nhận input ngay cả khi Form không có focus / bị ẩn
        private const int RIDEV_INPUTSINK = 0x00000100;


        #region RAW INPUT STRUCT

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public int dwFlags;
            public IntPtr hwndTarget;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;

            // Dùng offset Header size phù hợp x86/x64
            [FieldOffset(24)]
            public RAWKEYBOARD keyboard;
        }

        #endregion


        #region DLL IMPORT

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterRawInputDevices(
            RAWINPUTDEVICE[] pRawInputDevices,
            uint uiNumDevices,
            uint cbSize
        );


        [DllImport("user32.dll")]
        private static extern uint GetRawInputData(
            IntPtr hRawInput,
            uint uiCommand,
            IntPtr pData,
            ref uint pcbSize,
            uint cbSizeHeader
        );


        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetRawInputDeviceInfo(
            IntPtr hDevice,
            uint uiCommand,
            StringBuilder pData,
            ref uint pcbSize
        );

        #endregion


        // Lưu danh sách các scanner đã nhận được
        private readonly Dictionary<IntPtr, string> scanners =
            new Dictionary<IntPtr, string>();

        // ============================================================
        // HID BLUETOOTH - GÁN DEVICE VÀO LANE
        // ============================================================
        // Để trống nếu muốn tự động:
        // Thiết bị đầu tiên gửi dữ liệu -> Lane 01
        // Thiết bị thứ hai gửi dữ liệu -> Lane 02
        //
        // Sau khi chạy, Device Path sẽ hiện trên opLane01/opLane02.
        // Copy Device Path vào 2 biến dưới để cố định thiết bị.
        private string FixedDeviceIdLane01 = "";
        private string FixedDeviceIdLane02 = "";

        private string _deviceIdLane01 = "";
        private string _deviceIdLane02 = "";

        private readonly Dictionary<IntPtr, int> _deviceLaneMap =
            new Dictionary<IntPtr, int>();

        private readonly Dictionary<string, int> _devicePathLaneMap =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private readonly object _deviceLock = new object();

        // ============================================================
        // HID BUFFER - GOM MỘT LẦN SCAN THÀNH 1 CHUỖI
        // ============================================================
        private readonly StringBuilder _bufferLane01 =
            new StringBuilder();

        private readonly StringBuilder _bufferLane02 =
            new StringBuilder();

        private readonly object _bufferLockLane01 =
            new object();

        private readonly object _bufferLockLane02 =
            new object();

        private System.Threading.Timer _timerLane01;
        private System.Threading.Timer _timerLane02;

        // Nếu handheld không gửi Enter, sau 200ms không có phím mới
        // thì coi như scan đã hoàn tất.
        private const int SCAN_TIMEOUT_MS = 200;

        private bool IsTestModeEnabled => AppConfigs.Current.TestMode;

        Connection _ScanConection01 = new Connection();
        Connection _ScanConection02 = new Connection();

        BackgroundWorker _bw_update_ui = new BackgroundWorker();
        BackgroundWorker _bw_http_server = new BackgroundWorker();

        private HttpListener _httpListener;
        private readonly int _httpPort = 9999;
        private bool _httpServerRunning = false;

        //tạo file log 
        private LogHelper<e_LogType> PCLog;

        public PCartonDashboard()
        {
            InitializeComponent();
            PCLog = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "Pages", "PClog.ptl"));
        }

        #region Các hàm khởi tạo

        public void INIT()
        {

            RegisterKeyboardRawInput();
            if (AppConfigs.Current.cartonScanerMode != 1)
            {
                _ScanConection01.SERIALPORT = serialPort1;
                _ScanConection01.EVENT += _ScanConection01_EVENT;
                _ScanConection01.LOAD();
                _ScanConection01.CONNECT(AppConfigs.Current.HandScanCOM01);

                _ScanConection02.SERIALPORT = serialPort2;
                _ScanConection02.EVENT += _ScanConection02_EVENT;
                _ScanConection02.LOAD();
                _ScanConection02.CONNECT(AppConfigs.Current.HandScanCOM02);


            }
            else
            {
                tcpClient1.IP = AppConfigs.Current.cartonScanerTCP_IP;
                tcpClient1.Port = AppConfigs.Current.cartonScanerTCP_Port;

                tcpClient1.Connect();
            }

            _bw_update_ui.WorkerSupportsCancellation = true;
            _bw_update_ui.DoWork += bw_update_ui;

            if (!_bw_update_ui.IsBusy)
            {
                _bw_update_ui.RunWorkerAsync();
            }
            else
            {
                this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
            }

            // Khởi động HTTP Server BackgroundWorker
            _bw_http_server.WorkerSupportsCancellation = true;
            _bw_http_server.DoWork += bw_http_server_DoWork;
            if (!_bw_http_server.IsBusy)
            {
                _bw_http_server.RunWorkerAsync();
            }


        }


        #region Các hàm máy BL


        #region REGISTER RAW INPUT

        private void RegisterKeyboardRawInput()
        {
            RAWINPUTDEVICE[] devices = new RAWINPUTDEVICE[1];

            devices[0].usUsagePage = 0x01;
            devices[0].usUsage = 0x06; // Keyboard

            // Nhận input khi Form không focus
            devices[0].dwFlags = RIDEV_INPUTSINK;

            // Window nhận WM_INPUT
            devices[0].hwndTarget = this.Handle;


            bool result = RegisterRawInputDevices(
                devices,
                (uint)devices.Length,
                (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE))
            );


            if (!result)
            {
                int error = Marshal.GetLastWin32Error();

                MessageBox.Show(
                    "Không thể đăng ký Raw Input.\nError: " + error
                );
            }
        }


        // ============================================================
        // WINDOWS GỬI WM_INPUT VÀO FORM KỂ CẢ KHI FORM KHÔNG FOCUS
        // ============================================================
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_INPUT)
            {
                try
                {
                    ProcessRawInput(m.LParam);
                }
                catch (Exception ex)
                {
                    PCLog?.WriteLogAsync(
                        Globals.CurrentUser.Username,
                        e_LogType.Error,
                        "HID WndProc Error: " + ex.Message
                    );
                }
            }

            base.WndProc(ref m);
        }


        // ============================================================
        // ĐỌC RAW INPUT TỪ HANDHELD
        // ============================================================
        private void ProcessRawInput(IntPtr hRawInput)
        {
            uint size = 0;

            uint resultSize = GetRawInputData(
                hRawInput,
                RID_INPUT,
                IntPtr.Zero,
                ref size,
                (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))
            );

            if (resultSize == uint.MaxValue || size == 0)
                return;

            IntPtr buffer = Marshal.AllocHGlobal((int)size);

            try
            {
                uint result = GetRawInputData(
                    hRawInput,
                    RID_INPUT,
                    buffer,
                    ref size,
                    (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))
                );

                if (result == uint.MaxValue || result == 0)
                    return;

                RAWINPUT raw =
                    Marshal.PtrToStructure<RAWINPUT>(buffer);

                // Chỉ nhận keyboard HID
                if (raw.header.dwType != RIM_TYPEKEYBOARD)
                    return;

                // Chỉ nhận KeyDown
                bool keyDown =
                    raw.keyboard.Message == 0x0100 ||
                    raw.keyboard.Message == 0x0104;

                if (!keyDown)
                    return;

                IntPtr deviceHandle = raw.header.hDevice;

                if (deviceHandle == IntPtr.Zero)
                    return;

                // Xác định thiết bị là Lane 01 hay Lane 02
                int lane = GetDeviceLane(deviceHandle);

                if (lane != 1 && lane != 2)
                    return;

                Keys key = (Keys)raw.keyboard.VKey;

                // ====================================================
                // ENTER = KẾT THÚC MỘT LẦN SCAN
                // ====================================================
                if (key == Keys.Enter || key == Keys.Return)
                {
                    if (lane == 1)
                        CompleteLane01();
                    else
                        CompleteLane02();

                    return;
                }

                // ====================================================
                // PHÍM BÌNH THƯỜNG -> ĐƯA VÀO BUFFER
                // ====================================================
                string value = GetKeyValue(key);

                if (string.IsNullOrEmpty(value))
                    return;

                if (lane == 1)
                    AddDataToLane01(value);
                else
                    AddDataToLane02(value);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }


        // ============================================================
        // XÁC ĐỊNH HANDHELD NÀO LÀ LANE NÀO
        // ============================================================
        private int GetDeviceLane(IntPtr deviceHandle)
        {
            lock (_deviceLock)
            {
                // Handle đã được xác định trước đó
                if (_deviceLaneMap.TryGetValue(
                    deviceHandle,
                    out int oldLane))
                {
                    return oldLane;
                }

                // Lấy Device Path
                string devicePath = GetDeviceName(deviceHandle);

                if (string.IsNullOrWhiteSpace(devicePath))
                    return 0;

                // Device Path đã được map
                if (_devicePathLaneMap.TryGetValue(
                    devicePath,
                    out int knownLane))
                {
                    _deviceLaneMap[deviceHandle] = knownLane;
                    return knownLane;
                }

                // ====================================================
                // ID CỐ ĐỊNH LANE 01
                // ====================================================
                if (!string.IsNullOrWhiteSpace(FixedDeviceIdLane01) &&
                    devicePath.IndexOf(
                        FixedDeviceIdLane01.Trim(),
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    RegisterHidDevice(
                        deviceHandle,
                        devicePath,
                        1
                    );

                    return 1;
                }

                // ====================================================
                // ID CỐ ĐỊNH LANE 02
                // ====================================================
                if (!string.IsNullOrWhiteSpace(FixedDeviceIdLane02) &&
                    devicePath.IndexOf(
                        FixedDeviceIdLane02.Trim(),
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    RegisterHidDevice(
                        deviceHandle,
                        devicePath,
                        2
                    );

                    return 2;
                }

                // ====================================================
                // CHƯA CẤU HÌNH ID:
                // MÁY ĐẦU TIÊN -> LANE 01
                // MÁY THỨ HAI -> LANE 02
                // ====================================================
                if (string.IsNullOrWhiteSpace(_deviceIdLane01))
                {
                    RegisterHidDevice(
                        deviceHandle,
                        devicePath,
                        1
                    );

                    return 1;
                }

                if (string.IsNullOrWhiteSpace(_deviceIdLane02))
                {
                    RegisterHidDevice(
                        deviceHandle,
                        devicePath,
                        2
                    );

                    return 2;
                }

                // Có thiết bị thứ 3 -> không xử lý
                return 0;
            }
        }


        // ============================================================
        // LƯU DEVICE ID + HIỆN RA LISTBOX
        // ============================================================
        private void RegisterHidDevice(
            IntPtr deviceHandle,
            string devicePath,
            int lane)
        {
            _deviceLaneMap[deviceHandle] = lane;
            _devicePathLaneMap[devicePath] = lane;

            if (lane == 1)
            {
                _deviceIdLane01 = devicePath;

                this.InvokeIfRequired(() =>
                {
                    opLane01.Items.Insert(
                        0,
                        "[HID] DEVICE ID LANE 01: " + devicePath
                    );
                });

                PCLog?.WriteLogAsync(
                    Globals.CurrentUser.Username,
                    e_LogType.Info,
                    "HID gán Lane 01: " + devicePath
                );
            }
            else
            {
                _deviceIdLane02 = devicePath;

                this.InvokeIfRequired(() =>
                {
                    opLane02.Items.Insert(
                        0,
                        "[HID] DEVICE ID LANE 02: " + devicePath
                    );
                });

                PCLog?.WriteLogAsync(
                    Globals.CurrentUser.Username,
                    e_LogType.Info,
                    "HID gán Lane 02: " + devicePath
                );
            }
        }


        // ============================================================
        // LẤY DEVICE PATH
        // ============================================================
        private string GetDeviceName(IntPtr hDevice)
        {
            uint size = 0;

            uint result = GetRawInputDeviceInfo(
                hDevice,
                RIDI_DEVICENAME,
                null,
                ref size
            );

            if (result == uint.MaxValue || size == 0)
                return "";

            StringBuilder deviceName =
                new StringBuilder((int)size);

            result = GetRawInputDeviceInfo(
                hDevice,
                RIDI_DEVICENAME,
                deviceName,
                ref size
            );

            if (result == uint.MaxValue)
                return "";

            return deviceName.ToString();
        }


        // ============================================================
        // CHUYỂN KEYBOARD HID THÀNH KÝ TỰ
        // ENTER KHÔNG ĐƯA VÀO ĐÂY
        // ============================================================
        private string GetKeyValue(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z)
                return key.ToString();

            if (key >= Keys.D0 && key <= Keys.D9)
                return (
                    (int)key - (int)Keys.D0
                ).ToString();

            if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                return (
                    (int)key - (int)Keys.NumPad0
                ).ToString();

            switch (key)
            {
                case Keys.OemMinus:
                    return "-";

                case Keys.Oemplus:
                    return "+";

                case Keys.OemPeriod:
                    return ".";

                case Keys.Oemcomma:
                    return ",";

                case Keys.OemQuestion:
                    return "/";

                case Keys.OemSemicolon:
                    return ";";

                case Keys.OemQuotes:
                    return "'";

                case Keys.OemOpenBrackets:
                    return "[";

                case Keys.OemCloseBrackets:
                    return "]";

                case Keys.OemPipe:
                    return "\\";

                case Keys.Space:
                    return " ";

                default:
                    return "";
            }
        }


        // ============================================================
        // THÊM KÝ TỰ VÀO BUFFER LANE 01
        // ============================================================
        private void AddDataToLane01(string value)
        {
            lock (_bufferLockLane01)
            {
                _bufferLane01.Append(value);

                if (_timerLane01 == null)
                {
                    _timerLane01 =
                        new System.Threading.Timer(
                            Lane01ScanCompleted,
                            null,
                            SCAN_TIMEOUT_MS,
                            System.Threading.Timeout.Infinite
                        );
                }
                else
                {
                    _timerLane01.Change(
                        SCAN_TIMEOUT_MS,
                        System.Threading.Timeout.Infinite
                    );
                }
            }
        }


        // ============================================================
        // THÊM KÝ TỰ VÀO BUFFER LANE 02
        // ============================================================
        private void AddDataToLane02(string value)
        {
            lock (_bufferLockLane02)
            {
                _bufferLane02.Append(value);

                if (_timerLane02 == null)
                {
                    _timerLane02 =
                        new System.Threading.Timer(
                            Lane02ScanCompleted,
                            null,
                            SCAN_TIMEOUT_MS,
                            System.Threading.Timeout.Infinite
                        );
                }
                else
                {
                    _timerLane02.Change(
                        SCAN_TIMEOUT_MS,
                        System.Threading.Timeout.Infinite
                    );
                }
            }
        }


        // ============================================================
        // ENTER HOẶC TIMEOUT -> HOÀN TẤT LANE 01
        // SAU ĐÓ ĐƯA THẲNG VÀO HandScan01_Process()
        // ============================================================
        private void CompleteLane01()
        {
            string code = "";

            lock (_bufferLockLane01)
            {
                if (_bufferLane01.Length == 0)
                    return;

                code = _bufferLane01
                    .ToString()
                    .Trim();

                _bufferLane01.Clear();

                _timerLane01?.Change(
                    System.Threading.Timeout.Infinite,
                    System.Threading.Timeout.Infinite
                );
            }

            if (string.IsNullOrWhiteSpace(code))
                return;

            this.InvokeIfRequired(() =>
            {
                opLane01.Items.Insert(
                    0,
                    "[HID 01] " + code
                );

                if (opLane01.Items.Count > 200)
                {
                    opLane01.Items.RemoveAt(
                        opLane01.Items.Count - 1
                    );
                }

                // ĐƯA MÃ ĐÃ GOM THẲNG VÀO HANDSCAN
                HandScan01_Process(code);
            });
        }


        // ============================================================
        // ENTER HOẶC TIMEOUT -> HOÀN TẤT LANE 02
        // SAU ĐÓ ĐƯA THẲNG VÀO HandScan02_Process()
        // ============================================================
        private void CompleteLane02()
        {
            string code = "";

            lock (_bufferLockLane02)
            {
                if (_bufferLane02.Length == 0)
                    return;

                code = _bufferLane02
                    .ToString()
                    .Trim();

                _bufferLane02.Clear();

                _timerLane02?.Change(
                    System.Threading.Timeout.Infinite,
                    System.Threading.Timeout.Infinite
                );
            }

            if (string.IsNullOrWhiteSpace(code))
                return;

            this.InvokeIfRequired(() =>
            {
                opLane02.Items.Insert(
                    0,
                    "[HID 02] " + code
                );

                if (opLane02.Items.Count > 200)
                {
                    opLane02.Items.RemoveAt(
                        opLane02.Items.Count - 1
                    );
                }

                // ĐƯA MÃ ĐÃ GOM THẲNG VÀO HANDSCAN
                HandScan02_Process(code);
            });
        }


        // ============================================================
        // TIMEOUT LANE 01
        // ============================================================
        private void Lane01ScanCompleted(object state)
        {
            CompleteLane01();
        }


        // ============================================================
        // TIMEOUT LANE 02
        // ============================================================
        private void Lane02ScanCompleted(object state)
        {
            CompleteLane02();
        }

        #endregion



        #endregion


        string lastWarning = string.Empty;
        private void bw_update_ui(object sender, DoWorkEventArgs e)
        {
            while (!_bw_update_ui.CancellationPending)
            {
                try
                {
                    int lastID = Globals.ProductionData.counter.cartonID - 1;
                    int nextID = Globals.ProductionData.counter.cartonID + 1;
                    this.InvokeIfRequired(() =>
                    {
                        uiLabel1.Text = Globals.test.ToString();
                        uiLabel2.Text = Globals.test2.ToString();

                        opCartonMaxID.Text = Globals.ProductionData.counter.cartonID.ToString();
                        opCartonCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData) ? cartonData.cartonCode : "Chưa có mã thùng";
                        opcartonPackCount.Text = Globals.ProductionData.counter.carton_Packing_Count.ToString();


                        opLastActive.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData1) ? cartonData1.Activate_Datetime : "Chưa có thời gian kích hoạt";
                        opLastID.Text = (Globals.ProductionData.counter.cartonID - 1).ToString();
                        opLastCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData2) ? cartonData2.cartonCode : "Chưa có mã thùng";

                        opnextID.Text = (Globals.ProductionData.counter.cartonID + 1).ToString();
                        opnextCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData3) ? cartonData3.cartonCode : "Chưa có mã thùng";
                        opnextStart.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData4) ? cartonData4.Start_Datetime : "Chưa có thời gian kích hoạt";

                        uiLabel3.Text = Globals.HandScan01_Connected.ToString() + "/" + Globals.HandScan02_Connected;

                        if (Globals.Canhbao != lastWarning)
                        {
                            lastWarning = Globals.Canhbao;
                            opWarning.Items.Insert(0, Globals.Canhbao);
                        }
                    });

                }
                catch (Exception ex)
                {
                    PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, " Lỗi PC01 :" + ex.Message);
                }

                Thread.Sleep(500); // Đợi 1 giây trước khi lặp lại
            }
        }

        #endregion

        #region HTTP Server

        // Classes for JSON serialization
        public class CartonScanRequest
        {
            public string machineName { get; set; }
            public string cartonCode { get; set; }
            public string scannedAt { get; set; }
            public string mode { get; set; }
        }

        public class CartonScanResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
            public string status { get; set; }
            public int cartonIndex { get; set; }
            public string orderNo { get; set; }
            public int productCount { get; set; }
            public string activateDate { get; set; }
        }

        private void bw_http_server_DoWork(object sender, DoWorkEventArgs e)
        {
            _httpServerRunning = true;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://+:{_httpPort}/");

            try
            {
                _httpListener.Start();
                LogHttpServerStatus("HTTP Server started on port " + _httpPort);

                while (!_bw_http_server.CancellationPending)
                {
                    try
                    {
                        var context = _httpListener.GetContext();
                        HandleHttpRequests(context);
                    }
                    catch (HttpListenerException) when (_bw_http_server.CancellationPending)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!_bw_http_server.CancellationPending)
                        {
                            PCLog.WriteLogAsync("System", e_LogType.Error, "HTTP Request Error: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PCLog.WriteLogAsync("System", e_LogType.Error, "HTTP Server Error: " + ex.Message);
            }
            finally
            {
                _httpServerRunning = false;
                if (_httpListener != null && _httpListener.IsListening)
                {
                    _httpListener.Stop();
                    _httpListener.Close();
                }
                LogHttpServerStatus("HTTP Server stopped");
            }
        }

        private void HandleHttpRequests(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                string method = context.Request.HttpMethod;

                if (path == "/health" && method == "GET")
                {
                    HandleHealthCheck(context);
                }
                else if (path == "/carton/scan" && method == "POST")
                {
                    HandleCartonRequest(context);
                }
                else
                {
                    SendJsonResponse(context, 404, false, "Not Found", null);
                }
            }
            catch (Exception ex)
            {
                PCLog.WriteLogAsync("System", e_LogType.Error, "HandleHttpRequests Error: " + ex.Message);
                SendJsonResponse(context, 500, false, "Internal Server Error: " + ex.Message, null);
            }
        }

        private void HandleHealthCheck(HttpListenerContext context)
        {
            var response = new
            {
                status = "ok",
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            SendJsonResponse(context, 200, true, "OK", response);
        }

        private void HandleCartonRequest(HttpListenerContext context)
        {
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                string jsonBody = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(jsonBody))
                {
                    SendJsonResponse(context, 400, false, "Request body is empty", null);
                    return;
                }

                CartonScanRequest request;
                try
                {
                    request = Newtonsoft.Json.JsonConvert.DeserializeObject<CartonScanRequest>(jsonBody);
                }
                catch
                {
                    // Fallback: try parsing as pipe-separated format for backward compatibility
                    string[] parts = jsonBody.Split('|');
                    if (parts.Length >= 2)
                    {
                        request = new CartonScanRequest
                        {
                            cartonCode = parts[0].Trim(),
                            machineName = parts[1].Trim()
                        };
                    }
                    else
                    {
                        SendJsonResponse(context, 400, false, "Invalid format. Expected JSON or <Mã thùng>|<Tên máy>", null);
                        return;
                    }
                }

                if (request == null || string.IsNullOrEmpty(request.cartonCode))
                {
                    SendJsonResponse(context, 400, false, "Invalid request: cartonCode is required", null);
                    return;
                }

                string cartonCode = request.cartonCode.Trim();
                string machineName = request.machineName ?? "";

                PCLog.WriteLogAsync("HTTP", e_LogType.DataChange, $"Received carton: {cartonCode} from {machineName}");

                // Xử lý carton dựa trên tên máy
                if (machineName.Contains("Lane01") || machineName.Contains("Line01") || machineName.Contains("SC0"))
                {
                    HandScan01_Process(cartonCode);
                }
                else if (machineName.Contains("Lane02") || machineName.Contains("Line02") || machineName.Contains("SC1"))
                {
                    HandScan02_Process(cartonCode);
                }
                else
                {

                    //kiểm tra xem thùng đang chạy có code chưa

                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDatah1))
                    {
                        //kiểm tra xem chẵn hay lẻ 
                        if (Globals.ProductionData.counter.cartonID % 2 == 0)
                        {
                            //đúng là chẵn (thùng 2)
                            if (cartonDatah1.cartonCode == "0")
                            {
                                HandScan02_Process(cartonCode);
                            }
                            else
                            {
                                HandScan01_Process(cartonCode);
                            }
                        }
                        else
                        {
                            //đúng là lẻ (thùng 1)
                            if (cartonDatah1.cartonCode == "0")
                            {
                                HandScan01_Process(cartonCode);
                            }
                            else
                            {
                                HandScan02_Process(cartonCode);
                            }
                        }

                    }

                }

                // Lấy thông tin carton sau khi xử lý
                int currentCartonID = Globals.ProductionData.counter.cartonID;
                string currentCartonCode = "";
                string currentStatus = "OK";
                string currentOrderNo = Globals.ProductionData.orderNo ?? "";
                int productCount = Globals.ProductionData.counter.carton_Packing_Count;
                string activateDate = "";

                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(currentCartonID, out ProductionCartonData cartonData))
                {
                    currentCartonCode = cartonData.cartonCode;
                    activateDate = cartonData.Activate_Datetime ?? "";
                    if (cartonData.Production_Datetime != "0")
                    {
                        currentStatus = "COMPLETED";
                    }
                    else if (cartonData.Start_Datetime != "0")
                    {
                        currentStatus = "IN_PROGRESS";
                    }
                    else
                    {
                        currentStatus = "PENDING";
                    }
                }

                var response = new CartonScanResponse
                {
                    success = true,
                    message = "Carton scanned successfully",
                    status = currentStatus,
                    cartonIndex = currentCartonID,
                    orderNo = currentOrderNo,
                    productCount = productCount,
                    activateDate = activateDate
                };
                SendCartonScanResponse(context, 200, response);
            }
        }

        private void SendCartonScanResponse(HttpListenerContext context, int statusCode, CartonScanResponse response)
        {
            try
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                PCLog.WriteLogAsync("System", e_LogType.Error, "SendCartonScanResponse Error: " + ex.Message);
            }
        }

        private void SendJsonResponse(HttpListenerContext context, int statusCode, bool success, string message, object data)
        {
            try
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var jsonResponse = new
                {
                    success = success,
                    message = message,
                    data = data
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResponse);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                PCLog.WriteLogAsync("System", e_LogType.Error, "SendJsonResponse Error: " + ex.Message);
            }
        }

        private void LogHttpServerStatus(string message)
        {
            try
            {
                this.InvokeIfRequired(() =>
                {
                    opLane01.Items.Insert(0, $"[HTTP] {message}");
                });
            }
            catch { }
        }

        private void StopHttpServer()
        {
            if (_bw_http_server.IsBusy)
            {
                _bw_http_server.CancelAsync();
            }
            PCLog.WriteLogAsync("System", e_LogType.Info, "HTTP Server stop requested");
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            try
            {
                _timerLane01?.Dispose();
                _timerLane02?.Dispose();
            }
            catch
            {
            }

            StopHttpServer();
            if (_bw_update_ui.IsBusy)
            {
                _bw_update_ui.CancelAsync();
            }
            base.Dispose(disposing);
        }

        //scan chẵn
        private void _ScanConection02_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    if (!Globals.HandScan02_Connected)
                    {
                        Globals.HandScan02_Connected = true;
                    }
                    break;
                case e_Serial.Disconnected:
                    if (Globals.HandScan02_Connected)
                    {
                        Globals.HandScan02_Connected = false;
                    }
                    break;
                case e_Serial.Recive:

                    this.InvokeIfRequired(() =>
                    {
                        opLane02.Items.Insert(0, s.Trim());
                    });

                    if (AppConfigs.Current.cartonScaner_Only_Once)
                    {
                        break;
                    }
                    HandScan02_Process(s);
                    break;
            }
        }

        string saveLine = AppConfigs.Current.cartonCode_Line01;
        private void _ScanConection01_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    if (!Globals.HandScan01_Connected)
                    {
                        Globals.HandScan01_Connected = true;
                    }
                    break;
                case e_Serial.Disconnected:

                    if (Globals.HandScan01_Connected)
                    {
                        Globals.HandScan01_Connected = false;
                    }
                    break;
                case e_Serial.Recive:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, s.Trim());
                        opLane01.Items.Insert(0, saveLine);
                    });

                    if (AppConfigs.Current.cartonScaner_Only_Once)
                    {
                        if (s.Contains(AppConfigs.Current.cartonCode_Line01) || s.Contains(AppConfigs.Current.cartonCode_Line02))
                        {

                            saveLine = s.Trim();
                            this.InvokeIfRequired(() =>
                            {

                                opLane01.Items.Insert(0, "Đổi Line " + saveLine);
                            });
                        }
                        else
                        {
                            if (saveLine.Contains(AppConfigs.Current.cartonCode_Line01))
                            {
                                this.InvokeIfRequired(() =>
                                {

                                    opLane01.Items.Insert(0, "Xử lý Lane 01" + saveLine);
                                });
                                HandScan01_Process(s.Trim());
                            }
                            else if (saveLine.Contains(AppConfigs.Current.cartonCode_Line02))
                            {
                                this.InvokeIfRequired(() =>
                                {

                                    opLane01.Items.Insert(0, "Xử lý Lane 02" + saveLine);
                                });
                                HandScan02_Process(s.Trim());
                            }
                        }
                    }
                    else
                    {
                        HandScan01_Process(s);
                    }

                    break;
            }


        }

        private void btnSend1_Click(object sender, EventArgs e)
        {
            string test1 = ipTest1.Text.Split('-')[2];
            int test1Int = Convert.ToInt32(test1);
            test1Int++;
            ipTest1.Text = ipTest1.Text.Split('-')[0] + "-" + ipTest1.Text.Split('-')[1] + "-" + test1Int.ToString();
            HandScan01_Process(ipTest1.Text);
        }

        private void HandScan02_Process(string s)
        {
            //lưu log scan
            PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.DataChange, "Scan 02" + s.Trim(), s.Trim());
            //kiểm tra xem thùng đang chạy chẵn hay lẻ
            if (Globals.ProductionData.counter.cartonID % 2 != 0)
            {
                //thùng lẻ
                //kiểm tra xem thùng tiếp theo có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData))
                {
                    //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                    if (cartonData.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (!IsTestModeEnabled && Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonData.cartonCode = s.Trim();
                            cartonData.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }

            }
            else
            {

                //thùng chẵn tức là thùng đang chạy
                //kiểm tra xem thùng chẵn có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDataL))
                {
                    //nếu thùng đã có mã thì không làm gì cả
                    if (cartonDataL.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (!IsTestModeEnabled && Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);
                        }
                    }
                }
            }
        }

        private void HandScan01_Process(string s)
        {
            PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.DataChange, "Scan 01" + s.Trim(), s.Trim());

            this.InvokeIfRequired(() =>
            {
                opLane01.Items.Insert(0, s.Trim());
            });
            //kiểm tra xem thùng đang chạy chẵn hay lẻ
            if (Globals.ProductionData.counter.cartonID % 2 == 0)
            {
                //thùng chẵn
                //kiểm tra xem thùng tiếp theo có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData))
                {
                    //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                    if (cartonData.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (!IsTestModeEnabled && Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonData.cartonCode = s.Trim();
                            cartonData.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }
            }
            else
            {
                //thùng lẻ tức là thùng đang chạy
                //kiểm tra xem thùng lẻ có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDataL))
                {
                    //nếu thùng đã có mã thì không làm gì cả
                    if (cartonDataL.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (!IsTestModeEnabled && Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);

                            ////kết thúc thùng chẵn cũ
                            //cartonData.cartonCode = s.Trim();
                            //cartonData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }
            }
        }

        private void btnSend2_Click(object sender, EventArgs e)
        {
            string test2 = ipTest2.Text.Split('-')[2];
            int test2Int = Convert.ToInt32(test2);
            test2Int++;
            ipTest2.Text = ipTest2.Text.Split('-')[0] + "-" + ipTest2.Text.Split('-')[1] + "-" + test2Int.ToString();
            HandScan02_Process(ipTest2.Text);
        }

        private void tcpClient1_ClientCallBack(SpT.Communications.TCP.enumClient state, string data)
        {
            switch (state)
            {
                case SpT.Communications.TCP.enumClient.CONNECTED:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Connected");
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.CONNECTED;
                    break;
                case SpT.Communications.TCP.enumClient.DISCONNECTED:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Disconnected");
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.DISCONNECTED;
                    break;
                case SpT.Communications.TCP.enumClient.RECEIVED:
                    //phân tách ký tự <p> để lấy thùng bên nào ví dụ SC1<p>123456789 là thùng bên 1

                    string lane = data.Split(new string[] { "<p>" }, StringSplitOptions.None)[0];
                    string codeContent = data.Split(new string[] { "<p>" }, StringSplitOptions.None)[1];

                    if (lane != "SC1")
                    {
                        HandScan01_Process(codeContent);
                    }
                    else
                    {
                        HandScan02_Process(codeContent);
                    }

                    break;
                case SpT.Communications.TCP.enumClient.RECONNECT:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Reconnect");
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.RECONNECTING;
                    break;
            }
        }

        private void btnNextCarton_Click(object sender, EventArgs e)
        {

            if (ipTest1.Text != "secrettantien512")
            {
                this.ShowErrorDialog("Vui lòng không nhấn thử");
                return;
            }
            //kiểm tra thùng hiện tại đã có mã chưa
            if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData))
            {
                if (cartonData.cartonCode == "0")
                {
                    this.ShowErrorDialog("Thùng hiện tại chưa có mã thùng. Vui lòng quét mã thùng trước khi chuyển sang thùng mới.");
                    return;
                }
            }


            Globals.ProductionData.counter.cartonID += 1;
            Globals.ProductionData.counter.carton_Packing_Count = 0;

            //thêm thùng mới vào csdl
            ProductionCartonData cartonData1 = new ProductionCartonData();
            // chọn ID mới theo max ID hiện có để tránh lệch khi đã tạo/xóa trước đó
            cartonData1.cartonID = Globals_Database.Dictionary_ProductionCarton_Data.Keys.DefaultIfEmpty(0).Max() + 1;
            cartonData1.cartonCode = "0";
            cartonData1.Activate_Datetime = "0";
            cartonData1.Activate_User = "0";
            cartonData1.Production_Datetime = "0";
            cartonData1.orderNo = Globals.ProductionData.orderNo;
            cartonData1.Start_Datetime = "0";
            Globals_Database.Dictionary_ProductionCarton_Data.Add(cartonData1.cartonID, cartonData1);
            //thêm cái thùng mới vào cuối để đủ thùng
            Globals.ProductionData.setDB.Insert_Carton(cartonData1, cartonData1.orderNo);


        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            // Kiểm tra và kích hoạt thùng liên tục trước đó nếu chưa được kích hoạt
            CheckAndActivatePreviousCarton();
        }

        /// <summary>
        /// Kiểm tra thùng liên tục trước đó, nếu chưa kích hoạt thì tự động kích hoạt
        /// </summary>
        private void CheckAndActivatePreviousCarton()
        {
            try
            {
                Globals.Log.WriteLogAsync("System", e_LogType.Info, "Bắt đầu kiểm tra thùng liên tục chưa kích hoạt");

                // Chờ một chút để đảm bảo dữ liệu đã được load
                Thread.Sleep(2000);

                // Kiểm tra xem có đơn hàng nào đang hoạt động không
                if (string.IsNullOrEmpty(Globals.ProductionData.orderNo))
                {
                    Globals.Log.WriteLogAsync("System", e_LogType.Info, "Không có đơn hàng đang hoạt động, bỏ qua kiểm tra thùng");
                    return;
                }

                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData))
                { 
                    this.ShowInfoDialog($"Kiểm tra thùng ID={cartonData.cartonID}, Code={cartonData.cartonCode}, Start={cartonData.Start_Datetime}, Production date={cartonData.Production_Datetime}");

                    if (cartonData.cartonCode != "0" &&
                        cartonData.Start_Datetime != "0" &&
                        cartonData.Production_Datetime == "0")
                    { 
                        // Thêm vào hàng đợi kích hoạt
                        Globals_Database.Activate_Carton.Enqueue(cartonData.cartonCode);
                        Globals.Log.WriteLogAsync("System", e_LogType.Info,
                            $"Tự động kích hoạt thùng ID={cartonData.cartonID}, Code={cartonData.cartonCode}");
                        this.ShowSuccessDialog($"Đã tự động kích hoạt thùng ID={cartonData.cartonID}, Code={cartonData.cartonCode}");
                    }
                }

            }
            catch (Exception ex)
            {
                Globals.Log.WriteLogAsync("System", e_LogType.Error, "Lỗi khi kiểm tra và kích hoạt thùng tự động: " + ex.Message);
            }
        }

        private void uiSymbolButton2_Click(object sender, EventArgs e)
        {
            if(AppConfigs.Current.APP_TEST_MODE2)
            {
                Globals.TestComand = ipTest1.Text;
            }
            else
            {
                this.ShowErrorDialog("Chức năng này chỉ dành cho chế độ test");
            }
        }
    }
}
