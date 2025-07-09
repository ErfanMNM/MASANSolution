using MFI_Service;
using QR_MASAN_01.Auth;
using QR_MASAN_01.Mid;
using QR_MASAN_01.Views;
using QR_MASAN_01.Views.MFI_Service;
using QR_MASAN_01.Views.Printers;
using QR_MASAN_01.Views.Scada;
using QR_MASAN_01.Views.Settings;
using SpT;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static QR_MASAN_01.ActiveLogs;
using static QR_MASAN_01.SystemLogs;



namespace QR_MASAN_01
{
    public partial class FMainQR01 : UIForm2
    {
        MFI_Service_Form _FMFI = new MFI_Service_Form();
        FDashboard _FDashboard = new FDashboard();
        ScanQR scanQR = new ScanQR();
        MyLanPrinter _myLanPrinter = new MyLanPrinter();
        Printer_V7 _printer_V7 = new Printer_V7();
        FStatistics _FStatistics = new FStatistics();
        FSystemlogs FSystemlogs = new FSystemlogs();
        F1PLC _f1PLC = new F1PLC();
        LoginForm loginForm = new LoginForm();
        DeActive deActive = new DeActive();
        FPI_Service fPI_Service = new FPI_Service();
        //FDashboard_XK FDashboard_XK = new FDashboard_XK();
        FormTest fTest = new FormTest();
        FAppSetting fAppSetting = new FAppSetting();

        public static e_Render_State Render_State = e_Render_State.LOGIN;
        public static e_App_State App_State = e_App_State.LOGIN;
        public FMainQR01()
        {
            //khởi động phần mềm
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_EVENT, "Phần mềm khởi động", "System", "Bắt đầu khởi động");
            SystemLogs.LogQueue.Enqueue(systemLogs);

            try
            {
                InitializeComponent();
                UIStyles.CultureInfo = CultureInfos.en_US;
                UIStyles.GlobalFont = true;
                UIStyles.GlobalFontName = "Tahoma";

                this.MainTabControl = uiTabControl1;
                uiNavMenu1.TabControl = uiTabControl1;

                WKCheck.RunWorkerAsync();

                //set mặc định timeprinter là giờ hiện tại
                Globalvariable.TimeUnixPrinter = DateTimeOffset.Now.ToUnixTimeSeconds();

                PLCAddress.Init(
                                "credentials.json",
                                "1V2xjY6AA4URrtcwUorQE54Ud5KyI7Ev2hpDPMMcXVTI",
                                "PLC!A1:C100"
                            );
                //load Setting
                Setting.Current.Load();
                //Setting.Current.Save();
                //đọc file sqlite đưa vào datatable


                //using (var conn = new SQLiteConnection($@"Data Source=C:\Users\THUC\source\repos\ErfanMNM\MASANSolution\Server_Service\codes\08936086140878010725BMIP01.db;Version=3;"))
                //{
                //    string query = $@"SELECT ""_rowid_"",* FROM ""main"".""UniqueCodes""";
                //    var adapter = new SQLiteDataAdapter(query, conn);
                //    var table = new DataTable();
                //    adapter.Fill(table);
                //}

            }
            catch (Exception ex)
            {
                // Ghi log lỗi vào hàng đợi
                SystemLogs.LogQueue.Enqueue(new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_ERROR, "Lỗi khởi động phần mềm", "System", ex.Message));
                // Hiển thị thông báo lỗi cho người dùng
                this.ShowErrorDialog("Lỗi khởi động phần mềm", ex.Message, UIStyle.Red);
            }
                
        }

        private void btnAppClose_Click(object sender, EventArgs e)
        {
            // Ghi log sự kiện đóng ứng dụng
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_EVENT, "Đóng ứng dụng", "System", "Người dùng đã đóng ứng dụng");
            SystemLogs.LogQueue.Enqueue(systemLogs);
            ClockWK.CancelAsync();
            
            Environment.Exit(0);
        }
        private void RenderControlForm()
        {
            uiNavMenu1.Nodes.Clear();
            // Thêm các trang vào menu điều hướng
            uiNavMenu1.CreateNode(AddPage(_FDashboard, 1001));
            //uiNavMenu1.CreateNode(AddPage(FDashboard_XK, 1006));
            //uiNavMenu1.CreateNode(AddPage(_FMFI, 1003));
            uiNavMenu1.CreateNode(AddPage(fTest, 1007)); // Thêm trang Test
            uiNavMenu1.CreateNode(AddPage(fPI_Service, 1003));
            uiNavMenu1.CreateNode(AddPage(scanQR, 1004));
            uiNavMenu1.CreateNode(AddPage(_f1PLC, 1009));
            uiNavMenu1.CreateNode(AddPage(_FStatistics, 1002));
            uiNavMenu1.CreateNode(AddPage(FSystemlogs, 1005));
            uiNavMenu1.CreateNode(AddPage(fAppSetting, 1008)); // Thêm trang Cài đặt ứng dụng
            uiNavMenu1.CreateNode(AddPage(loginForm, 1999));
            uiNavMenu1.CreateNode(AddPage(deActive, 1998));

            uiNavMenu1.SelectPage(1999); //chọn trang Dashboard mặc định
            uiNavMenu1.Nodes[uiNavMenu1.Nodes.Count - 1].Remove();

            uiNavMenu1.Visible = false; //ẩn menu ban đầu
            uiNavMenu1.Enabled = false; //vô hiệu hóa menu ban đầu

            Render_State = e_Render_State.LOGIN; //đặt trạng thái render ban đầu là LOGIN

            _FDashboard.INIT();
            //FDashboard_XK.INIT();
            //_FMFI.FMFI_INIT();
            scanQR.INIT();
            fPI_Service.INIT();
            fAppSetting.FAppSetting_Load();

            //kiểm soát máy in

            switch (Setting.Current.Printer_name)
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
            RenderControlForm();
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
                        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_ERROR, "Lỗi kết nối Internet", "System", "Không thể kết nối Internet trong 10 giây");
                        SystemLogs.LogQueue.Enqueue(systemLogs);
                        demso = 0;
                        lblInternet.Text = "Internet: Lỗi";
                        lblInternet.FillColor = Color.Red;
                    }

                }

                    lblStatus.Text = $"{Globalvariable.All_Ready}|{GV.Production_Status.ToString()}";

                if ((GV.Production_Status == e_Production_Status.READY || GV.Production_Status == e_Production_Status.RUNNING) && Globalvariable.FDashBoard_Ready)
                {
                    Globalvariable.All_Ready = true; //đặt trạng thái sẵn sàng của hệ thống là true
                }

                else
                {
                    Globalvariable.All_Ready = false; //đặt trạng thái sẵn sàng của hệ thống là false
                }

                if (Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.READY)
                {

                    lblAllStatus.Text = "Đang dừng sản xuất";
                    lblAllStatus.FillColor = Color.Yellow;
                    lblAllStatus.ForeColor = Color.White;
                }
                else if (Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.RUNNING)
                {
                    lblAllStatus.Text = "Đang sản xuất";
                    lblAllStatus.FillColor = Color.Green;
                    lblAllStatus.ForeColor = Color.White;
                }
                else if (!Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.EDITING)
                {
                    lblAllStatus.Text = "Đang chỉnh PO";
                    lblAllStatus.FillColor = Color.Blue;
                    lblAllStatus.ForeColor = Color.Black;
                }
                else if (Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.UNKNOWN)
                {
                    lblAllStatus.Text = "Chưa chọn PO";
                    lblAllStatus.FillColor = Color.Orange;
                    lblAllStatus.ForeColor = Color.Black;
                }
                else if (GCamera.Camera_Status != e_Camera_Status.CONNECTED && GCamera.Camera_Status_02 != e_Camera_Status.CONNECTED && !Globalvariable.PLCConnect)
                {
                    lblAllStatus.Text = "Thiết bị đang lỗi";
                    lblAllStatus.FillColor = Color.Red;
                    lblAllStatus.ForeColor = Color.Black;
                }
                else
                {
                    lblAllStatus.Text = "Hệ thống chưa sẵn sàng";
                    lblAllStatus.FillColor = Color.Red;
                    lblAllStatus.ForeColor = Color.White;
                }
            }
        }

        /// <summary>
        /// TUI THÍCH GHI UNACTIVE LÀ DEACTIVE CHỨ NÓ KHÔNG CÓ Ý NGHĨA GÌ ĐÂU NHA
        /// </summary>

        bool login_rendered = false; // Biến để kiểm tra xem đã render trang đăng nhập hay chưa
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

                    if(ActiveLogQueue.Count > 0)
                    {
                        ActiveInsertToSQLite(ActiveLogQueue.Dequeue());
                    }

                }
                catch (Exception ex)
                {
                    // Ghi log lỗi vào hàng đợi
                    this.ShowErrorDialog("Lỗi ghi log vào SQLite", ex.Message, UIStyle.Red);
                }

                switch (App_State)
                {
                    case e_App_State.LOGIN:

                        //kiểm tra xem đã render trang đăng nhập chưa
                        if (Render_State != e_Render_State.LOGIN)
                        {
                            Render_State = e_Render_State.LOGIN; //đặt trạng thái render là LOGIN
                            this.Invoke(new Action(() =>
                            {

                                uiNavMenu1.Nodes[uiNavMenu1.Nodes.Count - 1].Remove(); // xóa trang đăng nhập nếu đã đăng nhập
                                uiNavMenu1.CreateNode("DM", 1999); // thêm trang đăng nhập vào menu
                                uiNavMenu1.SelectPage(1999); // chọn trang đăng nhập
                                uiNavMenu1.Enabled = false; //vô hiệu hóa menu
                                uiNavMenu1.Visible = false; //ẩn menu
                            }));
                        }
                        //kiểm tra xem đã đăng nhập chưa
                        if (Globalvariable.CurrentUser.Username != string.Empty)
                        {
                            //hiện thị thông tin user 
                            this.Invoke(new Action(() =>
                            {
                                
                                switch(Globalvariable.CurrentUser.Role)
                                {
                                    case "ADMIN":
                                        opUser.Text = $"[ADMIN] {Globalvariable.CurrentUser.Username}";
                                        opUser.ForeColor = Color.Red;
                                        break;
                                    case "OPERATOR":
                                        //màu vàng cho vận hành
                                        opUser.Text = $"[OPERATOR] {Globalvariable.CurrentUser.Username}";
                                        opUser.ForeColor = Color.Yellow;

                                        break;
                                    case "WORKER":
                                        //màu xanh
                                        opUser.Text = $"[WORKER] {Globalvariable.CurrentUser.Username}";
                                        opUser.ForeColor = Color.Green;
                                        break;
                                    default:
                                        opUser.Text = "Không xác định";
                                        break;
                                }
                            }));
                            if (Globalvariable.ACTIVE)
                            {
                                //nếu đã đăng nhập và ACTIVE thì chuyển sang trạng thái ACTIVE
                                App_State = e_App_State.ACTIVE;
                            }
                            else
                            {
                                //nếu đã đăng nhập nhưng không ACTIVE thì chuyển sang trạng thái UNACTIVE
                                App_State = e_App_State.DEACTIVE;
                            }
                        }
                        else
                        {
                            //nếu chưa đăng nhập thì vẫn ở trạng thái LOGIN
                            App_State = e_App_State.LOGIN;
                        }


                        break;
                    case e_App_State.ACTIVE:
                        //kiểm tra xem đã render trang ACTIVE chưa
                        if (Render_State != e_Render_State.ACTIVE)
                        {
                            Render_State = e_Render_State.ACTIVE; //đặt trạng thái render là ACTIVE
                            this.Invoke(new Action(() =>
                            {
                                btnDeActive.Enabled = true;
                                uiNavMenu1.Nodes[uiNavMenu1.Nodes.Count - 1].Remove(); // xóa trang cuối nếu đã đăng nhập
                                uiNavMenu1.SelectPage(1003); // chọn trang Dashboard
                                uiNavMenu1.Enabled = true; //bật menu
                                uiNavMenu1.Visible = true; //hiện menu
                            }));
                        }

                        //kiểm tra xem người dùng đã đăng nhập chưa
                        if (Globalvariable.CurrentUser.Username == string.Empty)
                        {
                            //nếu chưa đăng nhập thì chuyển sang trạng thái LOGIN
                            App_State = e_App_State.LOGIN;
                        }
                        else
                        {

                            if (Globalvariable.ACTIVE == false)
                            {
                                //nếu ACTIVE = false thì chuyển sang trạng thái DEACTIVE
                                App_State = e_App_State.DEACTIVE;
                            }
                            else
                            {
                                //nếu ACTIVE = true thì vẫn ở trạng thái ACTIVE
                                App_State = e_App_State.ACTIVE;
                            }
                        }
                        break;
                    case e_App_State.DEACTIVE:
                        //kiểm tra xem đã render trang DEACTIVE chưa
                        if (Render_State != e_Render_State.DEACTIVE)
                        {
                            
                            this.Invoke(new Action(() =>
                            {
                                
                                if (Render_State == e_Render_State.LOGIN)
                                {
                                    uiNavMenu1.Nodes[uiNavMenu1.Nodes.Count - 1].Remove(); // xóa trang cuối nếu đã đăng nhập
                                }
                                btnDeActive.Enabled = false; 
                                uiNavMenu1.CreateNode("DMA", 1998); // thêm trang DEACTIVE vào menu
                                uiNavMenu1.SelectPage(1998); // chọn trang DEACTIVE
                                uiNavMenu1.Enabled = false; //vô hiệu hóa menu
                                uiNavMenu1.Visible = false; //ẩn menu
                            }));

                            Render_State = e_Render_State.DEACTIVE; //đặt trạng thái render là DEACTIVE
                        }
                        //kiểm tra xem người dùng đã đăng nhập chưa
                        if (Globalvariable.CurrentUser.Username == string.Empty)
                        {
                            //nếu chưa đăng nhập thì chuyển sang trạng thái LOGIN
                            App_State = e_App_State.LOGIN;
                        }
                        else
                        {
                            //nếu đã đăng nhập thì vẫn ở trạng thái DEACTIVE
                            if (!Globalvariable.ACTIVE)
                            {
                                App_State = e_App_State.DEACTIVE; //nếu ACTIVE = false thì vẫn ở trạng thái DEACTIVE
                            }
                            else
                            {
                                App_State = e_App_State.ACTIVE; //nếu ACTIVE = true thì chuyển sang trạng thái ACTIVE
                            }
                        }
                        break;
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
                //try
                //{
                //    HttpClient HttpClient = new HttpClient();

                //    // Lấy chuỗi thời gian từ URL máy in
                //    string timeString = await HttpClient.GetStringAsync(Setting.Current.Laser_printer_server_url);
                //    timeString = timeString.Trim();

                //    // Parse datetime từ chuỗi nhận được (giả sử đúng format)
                //   if( DateTime.TryParse(timeString, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateTime))
                //    {
                //        Globalvariable.TimeUnixPrinter = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

                //        this.Invoke(new Action(() =>
                //        {
                //            opLaserPrinterTime.Text = $"{dateTime.ToString("dd-MM-yyyy HH:mm:ss")}";
                //            opLaserPrinterTime.ForeColor = Color.Green;
                //        }));
                //    }
                //    else
                //    {
                //        Globalvariable.TimeUnixPrinter++;
                //        this.Invoke(new Action(() =>
                //        {
                //            //chuyển số giây Unix sang định dạng ngày giờ hiện lên texbox
                //            timeString = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).ToString("dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US"));
                //            opLaserPrinterTime.Text = $"{timeString}";
                //            opLaserPrinterTime.ForeColor = Color.Red;

                //        }));
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Globalvariable.TimeUnixPrinter++;
                //    this.Invoke(new Action(() =>
                //    {
                //        opLaserPrinterTime.Text = $"{DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).ToString("dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US"))}";
                //        opLaserPrinterTime.ForeColor = Color.Red;
                //    }));
                //    logs++;
                //    if (logs > 10)
                //    {
                //        logs = 0;
                //        //ghi log lỗi
                //        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_ERROR, "Lỗi kết nối máy in laser", "System", ex.Message);
                //        SystemLogs.LogQueue.Enqueue(systemLogs);
                //    }
                    
                //}

                Thread.Sleep(1000);
            }
        }


        public enum e_App_State
        {
            LOGIN,
            ACTIVE,
            DEACTIVE
        }
        public enum e_Render_State
        {
            LOGIN,
            ACTIVE,
            DEACTIVE
        }
        long timelastclick = 0;
        long timecurrentclick = 15;

        private void btnDeActive_Click(object sender, EventArgs e)
        {
            timecurrentclick = DateTimeOffset.Now.ToUnixTimeSeconds();
            
            if (timecurrentclick - timelastclick > 5)
            {

                //ghi nhận log người dùng nhấn nút dừng kiểm
                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.USER_ACTION, "Người dùng nhấn nút Dừng Kiểm", Globalvariable.CurrentUser.Username, "Người dùng nhấn nút Dừng Kiểm");
                LogQueue.Enqueue(systemLogs);
                this.ShowInfoTip("Đã gửi sự kiện, vui lòng chờ. Nếu quá lâu có thể nhấn lại");
                //gửi xuống PLC

                //FDashboard_XK.SendUnActive();

                timecurrentclick = timelastclick = DateTimeOffset.Now.ToUnixTimeSeconds();
            }
            else
            {
                this.ShowErrorNotifier("Nhấn chậm chậm thôi, máy treo đó",false,2000);
            }

        }
    }


}
