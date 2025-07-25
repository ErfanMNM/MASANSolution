using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Utils;
using MASAN_SERIALIZATION.Views.Dashboards;
using MASAN_SERIALIZATION.Views.Login;
using MASAN_SERIALIZATION.Views.ProductionInfo;
using MASAN_SERIALIZATION.Views.SCADA;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION
{
    public partial class FMain : UIForm
    {
        //khai báo biến toàn cục
        private PLogin _pLogin = new PLogin();// trang đăng nhập
        private FDashboard _pDashboard = new FDashboard();// trang dashboard
        private PPOInfo _pProduction = new PPOInfo(); // trang sản xuất
        private PStatictis _pStatictis = new PStatictis(); // trang thống kê

        private BackgroundWorker WK_Main_Proccess = new BackgroundWorker(); // BackgroundWorker để xử lý trạng thái ứng dụng


        public FMain()
        {
            InitializeComponent();
            //khởi tạo nhật ký hệ thống tại AppData/Local/MASAN-SERIALIZATION/Logs
            Globals.Log = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "applog.tl"));
            //ghi nhật ký hệ thống mở ứng dụng
            Globals.Log.WriteLogAsync("System", e_LogType.Info, "Ứng dụng MASAN-SERIALIZATION đã được khởi động");
            //khởi tạo giao diện
            InitializeUI();
            //khởi tạo cấu hình
            InitializeConfigs();

        }

        private void FMain_Load(object sender, EventArgs e)
        {
            RenderControlForm(); //khởi tạo giao diện điều khiển
            InitializePage(); //khởi tạo các trang chức năng
            ToggleFullScreen();//bật fullscreen
            Start_Main_Process_Task(); //bắt đầu task chính xử lý trạng thái ứng dụng
        }

        #region Các hàm INIT
        private void InitializeUI()
        {
            try
            {
                //khởi tạo giao diện cơ bản
                UIStyles.CultureInfo = CultureInfos.en_US;
                UIStyles.GlobalFont = true;
                UIStyles.GlobalFontName = "Tahoma";

                MainTabControl = TabBody;
                NavMenu.TabControl = TabBody;

            }
            catch (Exception ex)
            {
                //ghi nhật ký lỗi
                Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi khởi tạo giao diện: {ex.Message}");
                //hiển thị thông báo lỗi
                this.ShowErrorTip($"Lỗi EM01 khởi tạo giao diện: {ex.Message}");
            }

        }
        private void InitializeConfigs()
        {
            try
            {
                //nạp cấu hình từ file ini
                AppConfigs.Current.Load();
                //AppConfigs.Current.Save(); //lưu cấu hình nếu có thay đổi

                //nạp cấu hình PLC
                PLCAddress.Init(
                                "credentials.json",
                                "1V2xjY6AA4URrtcwUorQE54Ud5KyI7Ev2hpDPMMcXVTI",
                                "PLC!A1:C100"
                            );
            }
            catch (Exception ex)
            {
                //ghi nhật ký lỗi
                Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi khởi tạo cấu hình: {ex.Message}");
                //hiển thị thông báo lỗi
                this.ShowErrorDialog($"Lỗi EM02 khởi tạo cấu hình: {ex.Message}");
            }
            
        }
        private void RenderControlForm()
        {
            try
            {
                NavMenu.Nodes.Clear();
                // Các trang chức năng chính chạy từ 1001 - 1999
                NavMenu.CreateNode(AddPage(_pDashboard, 1001));
                NavMenu.CreateNode(AddPage(_pProduction, 1002)); // Thêm trang sản xuất
                NavMenu.CreateNode(AddPage(_pStatictis, 1003)); // Thêm trang thống kê

                //Các trang chức năng phụ chạy từ 2001 - 2999
                NavMenu.CreateNode(AddPage(_pLogin, 2001)); // Thêm trang đăng nhập
                //NavMenu.CreateNode(AddPage(deActive, 1998));

                NavMenu.SelectPage(2001); //chọn trang Dashboard mặc định
                NavMenu.Nodes[NavMenu.Nodes.Count - 1].Remove();

                NavMenu.Visible = false; //ẩn menu ban đầu
                NavMenu.Enabled = false; //vô hiệu hóa menu ban đầu

                Globals.AppRenderState = e_App_Render_State.LOGIN; //đặt trạng thái render ban đầu là LOGIN

                //INIT các trang chức năng
                _pProduction.START(); //khởi tạo trang sản xuất
            }
            catch (Exception ex)
            {
                //ghi nhật ký lỗi
                Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi khởi tạo giao diện: {ex.Message}");
                //hiển thị thông báo lỗi
                this.ShowErrorTip($"Lỗi EM03 khởi tạo giao diện: {ex.Message}");
            }
           
        }
        private void InitializePage()
        {
            try
            {
                _pLogin.INIT();
                _pDashboard.STARTUP();
                _pStatictis.INIT();
            }
            catch (Exception ex)
            {
                //ghi nhật ký lỗi
                Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi khởi tạo ứng dụng: {ex.Message}");
                //hiển thị thông báo lỗi
                this.ShowErrorTip($"Lỗi EM04 khởi tạo ứng dụng: {ex.Message}");
            }
        }

        #endregion

        #region Các hàm xử lý giao diện
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
        #endregion

        #region Các hàm xử lý sự kiện giao diện
        private void Login_Process()
        {
            switch (Globals.AppState)
            {
                case e_App_State.LOGIN:

                    //kiểm tra xem đã render trang đăng nhập chưa
                    if (Globals.AppRenderState != e_App_Render_State.LOGIN)
                    {
                        Globals.AppRenderState = e_App_Render_State.LOGIN; //đặt trạng thái render là LOGIN
                        this.Invoke(new Action(() =>
                        {

                            NavMenu.Nodes[NavMenu.Nodes.Count - 1].Remove(); // xóa trang đăng nhập nếu đã đăng nhập
                            NavMenu.CreateNode("DM", 2001); // thêm trang đăng nhập vào menu
                            NavMenu.SelectPage(2001); // chọn trang đăng nhập
                            NavMenu.Enabled = false; //vô hiệu hóa menu
                            NavMenu.Visible = false; //ẩn menu
                        }));
                    }
                    //kiểm tra xem đã đăng nhập chưa
                    if (Globals.CurrentUser.Username != string.Empty)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            switch (Globals.CurrentUser.Role)
                            {
                                case "Admin":
                                    opUser.Text = $"[ADMIN] {Globals.CurrentUser.Username}";
                                    opUser.ForeColor = Color.Red;
                                    break;
                                case "Operator":
                                    //màu vàng cho vận hành
                                    opUser.Text = $"[OPERATOR] {Globals.CurrentUser.Username}";
                                    opUser.ForeColor = Color.Yellow;
                                    break;
                                case "Worker":
                                    //màu xanh
                                    opUser.Text = $"[WORKER] {Globals.CurrentUser.Username}";
                                    opUser.ForeColor = Color.Green;
                                    break;
                                default:
                                    opUser.Text = "Không xác định";
                                    break;
                            }
                        });
                        // kích mặc định active
                            //chỗ này kiểm tra có bật kiểm hay không, để đây sửa sau
                            if (Globals.ACTIVE_State)
                            {
                                //nếu đã đăng nhập và ACTIVE thì chuyển sang trạng thái ACTIVE
                                Globals.AppState = e_App_State.ACTIVE;
                            }
                            else
                            {
                                //nếu đã đăng nhập nhưng không ACTIVE thì chuyển sang trạng thái UNACTIVE
                                Globals.AppState = e_App_State.DEACTIVE;
                            }
                    }
                    else
                    {
                        //nếu chưa đăng nhập thì vẫn ở trạng thái LOGIN
                        Globals.AppState = e_App_State.LOGIN;
                    }


                    break;
                case e_App_State.ACTIVE:
                    //kiểm tra xem đã render trang ACTIVE chưa
                    if (Globals.AppRenderState != e_App_Render_State.ACTIVE)
                    {
                        Globals.AppRenderState = e_App_Render_State.ACTIVE; //đặt trạng thái render là ACTIVE
                        this.Invoke(new Action(() =>
                        {
                           // btnDeActive.Enabled = true;
                            //NavMenu.Nodes[NavMenu.Nodes.Count - 1].Remove(); // xóa trang cuối nếu đã đăng nhập
                            NavMenu.SelectPage(1002); // chọn trang Dashboard
                            NavMenu.Enabled = true; //bật menu
                            NavMenu.Visible = true; //hiện menu
                        }));

                        //khởi động các trang chức năng

                    }

                    //kiểm tra xem người dùng đã đăng nhập chưa
                    if (Globals.CurrentUser.Username == string.Empty)
                    {
                        //nếu chưa đăng nhập thì chuyển sang trạng thái LOGIN
                        Globals.AppState = e_App_State.LOGIN;
                    }
                    else
                    {

                        if (Globals.ACTIVE_State == false)
                        {
                            //nếu ACTIVE = false thì chuyển sang trạng thái DEACTIVE
                            Globals.AppState = e_App_State.DEACTIVE;
                        }
                        else
                        {
                            //nếu ACTIVE = true thì vẫn ở trạng thái ACTIVE
                            Globals.AppState = e_App_State.ACTIVE;
                        }
                    }
                    break;
                case e_App_State.DEACTIVE:
                    //kiểm tra xem đã render trang DEACTIVE chưa
                    if (Globals.AppRenderState != e_App_Render_State.DEACTIVE)
                    {

                        this.Invoke(new Action(() =>
                        {

                            if (Globals.AppRenderState == e_App_Render_State.LOGIN)
                            {
                                NavMenu.Nodes[NavMenu.Nodes.Count - 1].Remove(); // xóa trang cuối nếu đã đăng nhập
                            }
                            //btnDeActive.Enabled = false;
                            NavMenu.CreateNode("DMA", 2001); // thêm trang DEACTIVE vào menu
                            NavMenu.SelectPage(2001); // chọn trang DEACTIVE
                            NavMenu.Enabled = false; //vô hiệu hóa menu
                            NavMenu.Visible = false; //ẩn menu
                        }));

                        Globals.AppRenderState = e_App_Render_State.DEACTIVE; //đặt trạng thái render là DEACTIVE
                    }
                    //kiểm tra xem người dùng đã đăng nhập chưa
                    if (Globals.CurrentUser.Username == string.Empty)
                    {
                        //nếu chưa đăng nhập thì chuyển sang trạng thái LOGIN
                        Globals.AppState = e_App_State.LOGIN;
                    }
                    else
                    {
                        //nếu đã đăng nhập thì vẫn ở trạng thái DEACTIVE
                        if (!Globals.ACTIVE_State)
                        {
                            Globals.AppState = e_App_State.DEACTIVE; //nếu ACTIVE = false thì vẫn ở trạng thái DEACTIVE
                        }
                        else
                        {
                            Globals.AppState = e_App_State.ACTIVE; //nếu ACTIVE = true thì chuyển sang trạng thái ACTIVE
                        }
                    }
                    break;
            }
        }

        private void App_State_Process()
        {
            //kiểm tra trạng thái ứng dụng và cập nhật giao diện
            switch (Globals.AppState)
            {
                case e_App_State.LOGIN:

                    Globals.APP_Ready = false; //đặt trạng thái ứng dụng chưa sẵn sàng
                    Globals.Device_Ready = false; //đặt trạng thái thiết bị chưa sẵn sàng
                    this.InvokeIfRequired(() =>
                    {
                        //nếu đang ở trạng thái LOGIN thì hiển thị trang đăng nhập
                        lblAllStatus.Text = "Chưa đăng nhập";
                        lblAllStatus.FillColor = Color.Red;
                        lblAllStatus.ForeColor = Color.Yellow;

                    });
                    break;
                case e_App_State.ACTIVE:

                    Globals.APP_Ready = true; //đặt trạng thái ứng dụng đã sẵn sàng
                    if (Globals.CameraMain_State != e_Camera_State.CONNECTED || Globals.CameraSub_State != e_Camera_State.CONNECTED ||Globals.PLC_Connected != true)
                    {
                        Globals.Device_Ready = false; 
                        this.InvokeIfRequired(() =>
                        {
                            //nếu camera chính hoặc phụ chưa kết nối hoặc PLC chưa kết nối thì hiển thị thông báo
                            lblAllStatus.Text = "Lỗi thiết bị";
                            lblAllStatus.FillColor = Color.Red;
                            lblAllStatus.ForeColor = Color.Yellow;
                        });
                    }
                    else
                    {
                        Globals.Device_Ready = true; 
                    }

                    if (Globals.APP_Ready && Globals.Device_Ready)
                    {
                        switch (Globals.Production_State)
                        {
                            case Production.e_Production_State.NoSelectedPO:
                                this.InvokeIfRequired(() =>
                                {
                                    lblAllStatus.Text = "Chưa chọn đơn hàng";
                                    lblAllStatus.FillColor = Color.Yellow;
                                    lblAllStatus.ForeColor = Color.Black;
                                });
                                break;
                            case Production.e_Production_State.Start:
                                break;
                            case Production.e_Production_State.Checking_PO_Info:
                                break;
                            case Production.e_Production_State.Loading:
                                break;
                            case Production.e_Production_State.Camera_Processing:
                                break;
                            case Production.e_Production_State.Pushing_new_PO_to_PLC:
                                break;
                            case Production.e_Production_State.Pushing_continue_PO_to_PLC:
                                break;
                            case Production.e_Production_State.Ready:
                                this.InvokeIfRequired(() =>
                                {
                                    lblAllStatus.Text = "Sẵn sàng sản xuất";
                                    lblAllStatus.FillColor = Color.Yellow;
                                    lblAllStatus.ForeColor = Color.Black;
                                });
                                break;
                            case Production.e_Production_State.Running:
                                this.InvokeIfRequired(() =>
                                {
                                    lblAllStatus.Text = "Đang sản xuất";
                                    lblAllStatus.FillColor = Color.Green;
                                    lblAllStatus.ForeColor = Color.White;
                                });
                                break;
                            case Production.e_Production_State.Completed:
                                this.InvokeIfRequired(() =>
                                {
                                    lblAllStatus.Text = "Đã kết thúc đơn";
                                    lblAllStatus.FillColor = Color.Blue;
                                    lblAllStatus.ForeColor = Color.White;
                                });
                                break;
                            case Production.e_Production_State.Editing:
                                this.InvokeIfRequired(() =>
                                {
                                    lblAllStatus.Text = "Đang chỉnh";
                                    lblAllStatus.FillColor = Color.Yellow;
                                    lblAllStatus.ForeColor = Color.Black;
                                });
                                break;
                            case Production.e_Production_State.Editting_ProductionDate:
                                this.InvokeIfRequired(() =>
                                {
                                    lblAllStatus.Text = "Đang chỉnh";
                                    lblAllStatus.FillColor = Color.Yellow;
                                    lblAllStatus.ForeColor = Color.Black;
                                });
                                break;
                            case Production.e_Production_State.Saving:
                                this.InvokeIfRequired(() =>
                                {
                                    lblAllStatus.Text = "Đang lưu";
                                    lblAllStatus.FillColor = Color.Yellow;
                                    lblAllStatus.ForeColor = Color.Black;
                                });
                                break;
                            case Production.e_Production_State.Error:
                                this.InvokeIfRequired(() =>
                                {
                                    lblAllStatus.Text = "Lỗi sản xuất";
                                    lblAllStatus.FillColor = Color.Red;
                                    lblAllStatus.ForeColor = Color.Yellow;
                                });
                                break;
                        }
                    }

                    break;
                case e_App_State.DEACTIVE:
                    //nếu đang ở trạng thái DEACTIVE thì hiển thị trang DEACTIVE
                    _pLogin.Show();
                    break;
            }
        }

        #endregion

        #region Luồng xử trạng thái ứng dụng

        //tạo một task với token
        private void Main_Process_Async()
        {
            // Bắt đầu nhiệm vụ
            try
            {
                while (!WK_Main_Proccess.CancellationPending)
                {
                    try
                    {
                        opClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");
                        lblStatus.Text = Globals.Production_State.ToString();
                        Login_Process();
                        // Kiểm tra trạng thái ứng dụng và cập nhật giao diện
                        App_State_Process();
                    }
                    catch (Exception ex)
                    {
                        // Ghi nhật ký lỗi
                        Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi trong Main_Process_Async: {ex.Message}");
                        // Hiển thị thông báo lỗi
                        this.ShowErrorTip($"Lỗi EM05 trong quá trình xử lý: {ex.Message}");
                    }
                    Thread.Sleep(100); // Đợi 0.1 giây trước khi lặp lại
                }
            }
            catch (TaskCanceledException) { }
        }
        private void Start_Main_Process_Task()
        {
            WK_Main_Proccess.WorkerSupportsCancellation = true; // Hỗ trợ hủy bỏ
            WK_Main_Proccess.DoWork += (s, e) =>
            {
                Main_Process_Async();
            };
            WK_Main_Proccess.RunWorkerAsync(); // Bắt đầu chạy nhiệm vụ
        }
        #endregion

        #region Các hàm sự kiện giao diện
        private void btnAppClose_Click(object sender, EventArgs e)
        {
            // Dừng task chính
            WK_Main_Proccess.CancelAsync();
            //dừng task phụ nếu có
            _pProduction.Stop_Process_Task();

            // Ghi nhật ký hệ thống
            Globals.Log.WriteLogAsync("System", e_LogType.Info, "Ứng dụng MASAN-SERIALIZATION đã được đóng");
            // Đóng ứng dụng
            Application.Exit();

        }

        private void btnMini_Click(object sender, EventArgs e)
        {
            //thu nhỏ ứng dụng
            WindowState = FormWindowState.Minimized;
        }

        #endregion
    }
}
