using MFI_Service;
using QR_MASAN_01.Auth;
using QR_MASAN_01.Mid;
using QR_MASAN_01.Views;
using QR_MASAN_01.Views.Printers;
using QR_MASAN_01.Views.Scada;
using QR_MASAN_01.Views.Settings;
using SpT;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading;
using static QR_MASAN_01.SystemLogs;
using System.Windows.Forms;
using System.Net.Http;
using System.Globalization;



namespace QR_MASAN_01
{
    public partial class FMainQR01 : UIForm2
    {
       //F1Printer _F1Printer = new F1Printer();
        F1Dashboard _F1Dashboard = new F1Dashboard();
        ///F1Cloudv2 _f1Cloudv2 = new F1Cloudv2();
        MFI_Service_Form _FMFI = new MFI_Service_Form();
        //F1Cloud _f1Cloud = new F1Cloud();
       //F1Data _F1Data = new F1Data();
        ScanQR scanQR = new ScanQR();
       //F1PLC _f1PLC = new F1PLC();
       FormTest FormTest = new FormTest();
       Settings _setings = new Settings();
        MyLanPrinter _myLanPrinter = new MyLanPrinter();
        Printer_V7 _printer_V7 = new Printer_V7();
        FStatistics _FStatistics = new FStatistics();
        FSystemlogs FSystemlogs = new FSystemlogs();
        F1PLC _f1PLC = new F1PLC();
        LoginForm loginForm = new LoginForm();

        public FMainQR01()
        {
            //khởi động phần mềm
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_EVENT, "Phần mềm khởi động", "System", "Bắt đầu khởi động");
            SystemLogs.LogQueue.Enqueue(systemLogs);

            try
            {
                InitializeComponent();
                UIStyles.CultureInfo = CultureInfos.en_US;
                this.MainTabControl = uiTabControl1;
                uiNavMenu1.TabControl = uiTabControl1;
                WKCheck.RunWorkerAsync();
                //set mặc định timeprinter là giờ hiện tại
                Globalvariable.TimeUnixPrinter = DateTimeOffset.Now.ToUnixTimeSeconds();

                _setings.LoadSettings("C:/Phan_Mem/Configs.xlsx");

                //GoogleSheetConfigHelper.Init(
                //                            "credentials.json",
                //                            "1V2xjY6AA4URrtcwUorQE54Ud5KyI7Ev2hpDPMMcXVTI",
                //                            "PLC!A1:C100"
                //                        );

                PLCAddress.Init(
                                "credentials.json",
                                "1V2xjY6AA4URrtcwUorQE54Ud5KyI7Ev2hpDPMMcXVTI",
                                "PLC!A1:C100"
                            );


            }
            catch (Exception ex)
            {
                // Ghi log lỗi vào hàng đợi
                SystemLogs.LogQueue.Enqueue(new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.ERROR, "Lỗi khởi động phần mềm", "System", ex.Message));
                // Hiển thị thông báo lỗi cho người dùng
                this.ShowErrorDialog("Lỗi khởi động phần mềm", ex.Message, UIStyle.Red);
            }
                
        }

        private void btnAppClose_Click(object sender, EventArgs e)
        {
            // Ghi log sự kiện đóng ứng dụng
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_EVENT, "Đóng ứng dụng", "System", "Người dùng đã đóng ứng dụng");
            SystemLogs.LogQueue.Enqueue(systemLogs);
            ClockWK.CancelAsync();
            
            Environment.Exit(0);
        }
        private void RenderControlForm()
        {
            
            uiNavMenu1.CreateNode(AddPage(_F1Dashboard, 1001));
            uiNavMenu1.CreateNode(AddPage(_FMFI, 1003));
            uiNavMenu1.CreateNode(AddPage(scanQR, 1004));
            //uiNavMenu1.CreateNode(AddPage(FormTest, 1998));
            uiNavMenu1.CreateNode(AddPage(_f1PLC, 1009));
            uiNavMenu1.CreateNode(AddPage(_FStatistics, 1002));
            uiNavMenu1.CreateNode(AddPage(FSystemlogs, 1005));
            uiNavMenu1.SelectPage(1001);

            _F1Dashboard.INIT();
            _FMFI.FMFI_INIT();
           scanQR.INIT();
           

            //kiểm soát máy in

            switch (GlobalSettings.Get("PRINTER"))
            {
                case "ML":
                    uiNavMenu1.CreateNode(AddPage(_myLanPrinter, 1011));
                    break;
                case "V7":
                    uiNavMenu1.CreateNode(AddPage(_printer_V7, 1012));
                    break;
                case "UC22":
                    //uiNavMenu1.CreateNode(AddPage(new F1Printer(), 1004));
                    break;
                case "NONE":
                    // Không làm gì cả
                    break;
                default:
                    //không làm gì cả
                    break;
            }
        }
        private void ToggleFullScreen()
        {

            if (this.WindowState != FormWindowState.Maximized)
            {
                // Lưu trữ trạng thái và kích thước hiện tại để có thể khôi phục lại sau
                this.Tag = this.WindowState;
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }

            else
            {
                // Khôi phục trạng thái và kích thước form trước khi vào chế độ toàn màn hình
                this.WindowState = (FormWindowState)this.Tag;
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            ToggleFullScreen();
            WK_LaserPrinterTime.RunWorkerAsync();
            ClockWK.RunWorkerAsync();
        }
        //kiểm tra mấy thứ linh tinh
        bool InternetConnection = false;
        double InternetSpeed = 0;

        private void WKCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            int demso = 0;

            while (!WKCheck.CancellationPending)
            {
                Thread.Sleep(1000);
                //internet

                    InternetConnection = Internet.IsOK();
                    InternetSpeed = Internet.GetInternetSpeed();

                if (InternetConnection)
                {
                    lblInternet.Text = $"Internet:{InternetSpeed:F1} KBps";
                    lblInternet.FillColor = Color.FromArgb(243, 249, 255);
                }
                else
                {
                    demso++;
                    if (demso > 10)
                    {
                        //ghi log lỗi
                        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.ERROR, "Lỗi kết nối Internet", "System", "Không thể kết nối Internet trong 10 giây");
                        SystemLogs.LogQueue.Enqueue(systemLogs);
                        demso = 0;
                        lblInternet.Text = "Internet: Lỗi";
                        lblInternet.FillColor = Color.Red;
                    }

                }


                if(Globalvariable.AllReady)
                {

                    lblAllStatus.Text = "Hệ thống sẵn sàng";
                    lblAllStatus.FillColor = Color.Green;
                    lblAllStatus.ForeColor = Color.White;
                }
                else
                {
                    lblAllStatus.Text = "Hệ thống chưa sẵn sàng";
                    lblAllStatus.FillColor = Color.Red;
                    lblAllStatus.ForeColor = Color.White;
                }
            }
        }

        private void ClockWK_DoWork(object sender, DoWorkEventArgs e)
        {
            //đồng hồ
            while(!ClockWK.CancellationPending)
            {
                try
                {
                    // Kiểm tra hàng đợi và thêm bản ghi vào SQLite
                    if (LogQueue.Count > 0)
                    {
                        InsertToSQLite(LogQueue.Dequeue());
                    }

                }
                catch (Exception ex)
                {
                    // Ghi log lỗi vào hàng đợi
                    this.ShowErrorDialog("Lỗi ghi log vào SQLite", ex.Message, UIStyle.Red);
                }

                if (Globalvariable.CurrentUser.Username == string.Empty)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (uiNavMenu1.Nodes.Count <= 0)
                        {
                            uiNavMenu1.CreateNode(AddPage(loginForm, 1999));
                            uiNavMenu1.SelectPage(1999);
                        }

                    }));

                }
                else
                {
                    //nếu đã đăng nhập thì render các control form
                    this.Invoke(new Action(() =>
                    {
                        if (uiNavMenu1.Nodes.Count == 1)
                        {
                            opUser.Text = Globalvariable.CurrentUser.Username;
                            switch (Globalvariable.CurrentUser.Role)
                            {
                                case "Admin":
                                    opUser.ForeColor = Color.Red;
                                    break;
                                case "Operator":
                                    opUser.ForeColor = Color.Blue;
                                    break;
                                case "Worker":
                                    opUser.ForeColor = Color.Green;
                                    break;
                                default:
                                    opUser.ForeColor = Color.Gray;
                                    break;
                            }
                            uiNavMenu1.Nodes[0].Remove();
                            RenderControlForm();
                        }
                    }));

                }
                lblClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                Thread.Sleep(100);
            }
            
        }

        private void Logo_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnMini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void uiTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        int logs = 11;
        private async void WK_LaserPrinterTime_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_LaserPrinterTime.CancellationPending)
            {
                try
                {
                    HttpClient HttpClient = new HttpClient();

                    // Lấy chuỗi thời gian từ URL máy in
                    string timeString = await HttpClient.GetStringAsync(GlobalSettings.Get("LASER_PRINTER_URL"));
                    timeString = timeString.Trim();

                    // Parse datetime từ chuỗi nhận được (giả sử đúng format)
                   if( DateTime.TryParse(timeString, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateTime))
                    {
                        Globalvariable.TimeUnixPrinter = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

                        this.Invoke(new Action(() =>
                        {
                            opLaserPrinterTime.Text = $"{dateTime.ToString("dd-MM-yyyy HH:mm:ss")}";
                            opLaserPrinterTime.ForeColor = Color.Green;
                        }));
                    }
                    else
                    {
                        Globalvariable.TimeUnixPrinter++;
                        this.Invoke(new Action(() =>
                        {
                            //chuyển số giây Unix sang định dạng ngày giờ hiện lên texbox
                            timeString = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).ToString("dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US"));
                            opLaserPrinterTime.Text = $"{timeString}";
                            opLaserPrinterTime.ForeColor = Color.Red;

                        }));
                    }
                }
                catch (Exception ex)
                {
                    Globalvariable.TimeUnixPrinter++;
                    this.Invoke(new Action(() =>
                    {
                        opLaserPrinterTime.Text = $"{DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).ToString("dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US"))}";
                        opLaserPrinterTime.ForeColor = Color.Red;
                    }));
                    logs++;
                    if (logs > 10)
                    {
                        logs = 0;
                        //ghi log lỗi
                        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.ERROR, "Lỗi kết nối máy in laser", "System", ex.Message);
                        SystemLogs.LogQueue.Enqueue(systemLogs);
                    }
                    
                }

                Thread.Sleep(1000);
            }
        }
    }
}
