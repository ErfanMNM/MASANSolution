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

using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Utils;
using MASAN_SERIALIZATION.Views.AWS;
using MASAN_SERIALIZATION.Views.Dashboards;
using MASAN_SERIALIZATION.Views.Database;
using MASAN_SERIALIZATION.Views.Login;
using MASAN_SERIALIZATION.Views.ProductionInfo;
using MASAN_SERIALIZATION.Views.SCADA;
using MASAN_SERIALIZATION.Views.Settings;

using SpT.Logs;
using Sunny.UI;

namespace MASAN_SERIALIZATION
{
    public partial class FMain : UIForm
    {
        #region Private Fields - Page Controls
        private PLogin _pLogin = new PLogin();
        private FDashboard _pDashboard = new FDashboard();
        private PPOInfo _pProduction = new PPOInfo();
        private PStatictis _pStatictis = new PStatictis();
        private PCartonDashboard _pCartonDashboard = new PCartonDashboard();
        private PAws _pAws1 = new PAws();
        private PAwsIot _pAws = new PAwsIot();
        private PSettings _pSettings = new PSettings();
        private PLCSetting _pPLCSetting = new PLCSetting();
        private PScaner _pScaner = new PScaner();
        #endregion

        #region Private Fields - Background Workers
        private BackgroundWorker WK_Main_Proccess = new BackgroundWorker();
        #endregion

        #region Constructor
        public FMain()
        {
            InitializeComponent();
            
            Globals.Log = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "applog.tl"));
            Globals.Log.WriteLogAsync("System", e_LogType.Info, "Ứng dụng MASAN-SERIALIZATION đã được khởi động");

            InitializeUI();
            InitializeConfigs();
            RenderControlForm();
            ToggleFullScreen();
            Start_Main_Process_Task();
            InitializePage();
        }
        #endregion

        #region Form Events
        private void FMain_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAppClose_Click(object sender, EventArgs e)
        {
            WK_Main_Proccess.CancelAsync();
            Globals.Log.WriteLogAsync("System", e_LogType.Info, "Ứng dụng MASAN-SERIALIZATION đã được đóng");
            Application.Exit();
        }

        private void btnMini_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region Private Methods - Initialization
        private void InitializeUI()
        {
            try
            {
                UIStyles.CultureInfo = CultureInfos.en_US;
                UIStyles.GlobalFont = true;
                UIStyles.GlobalFontName = "Tahoma";

                MainTabControl = TabBody;
                NavMenu.TabControl = TabBody;
            }
            catch (Exception ex)
            {
                Globals.Log.WriteLogAsync("System", e_LogType.Error, ErrorCodes.Main.INIT_UI_FAILED + ex);
                this.ShowErrorTip($"[{ErrorCodes.Main.INIT_UI_FAILED}] {ErrorCodes.GetErrorDescription(ErrorCodes.Main.INIT_UI_FAILED)}: {ex.Message}");
            }
        }

        private void InitializeConfigs()
        {
            try
            {
                AppConfigs.Current.Load();
                
                PLCAddress.Init(
                    "credentials.json",
                    "1V2xjY6AA4URrtcwUorQE54Ud5KyI7Ev2hpDPMMcXVTI",
                    "PLC!A1:C100"
                );
            }
            catch (Exception ex)
            {
                Globals.Log.WriteLogAsync("System", e_LogType.Error, ErrorCodes.Main.INIT_CONFIG_FAILED + ex);
                this.ShowErrorDialog($"[{ErrorCodes.Main.INIT_CONFIG_FAILED}] {ErrorCodes.GetErrorDescription(ErrorCodes.Main.INIT_CONFIG_FAILED)}: {ex.Message}");
            }
        }

        private void RenderControlForm()
        {
            try
            {
                NavMenu.Nodes.Clear();
                
                NavMenu.CreateNode(AddPage(_pDashboard, 1001));
                NavMenu.CreateNode(AddPage(_pProduction, 1002));
                NavMenu.CreateNode(AddPage(_pStatictis, 1003));
                NavMenu.CreateNode(AddPage(_pCartonDashboard, 1004));
                NavMenu.CreateNode(AddPage(_pScaner, 1005));
                NavMenu.CreateNode(AddPage(_pAws, 1006));
                NavMenu.CreateNode(AddPage(_pPLCSetting, 1007));
                NavMenu.CreateNode(AddPage(_pSettings, 1008));
                NavMenu.CreateNode(AddPage(_pAws1, 1009));
                NavMenu.CreateNode(AddPage(_pLogin, 2001));

                NavMenu.SelectPage(2001);
                NavMenu.Nodes[NavMenu.Nodes.Count - 1].Remove();

                NavMenu.Visible = false;
                NavMenu.Enabled = false;

                Globals.AppRenderState = e_App_Render_State.LOGIN;
            }
            catch (Exception ex)
            {
                Globals.Log.WriteLogAsync("System", e_LogType.Error, ErrorCodes.Main.INIT_CONTROLS_FAILED + ex);
                this.ShowErrorTip($"[{ErrorCodes.Main.INIT_CONTROLS_FAILED}] {ErrorCodes.GetErrorDescription(ErrorCodes.Main.INIT_CONTROLS_FAILED)}: {ex.Message}");
            }
        }

        private void InitializePage()
        {
            try
            {
                _pLogin.INIT();
                _pDashboard.STARTUP();
                _pStatictis.INIT();
                _pCartonDashboard.INIT();
                _pScaner.INIT();
                _pSettings.INIT();
                _pPLCSetting.INIT();
                _pProduction.START();
            }
            catch (Exception ex)
            {
                Globals.Log.WriteLogAsync("System", e_LogType.Error, ErrorCodes.Main.INIT_PAGES_FAILED + ex);
                this.ShowErrorTip($"[{ErrorCodes.Main.INIT_PAGES_FAILED}] {ErrorCodes.GetErrorDescription(ErrorCodes.Main.INIT_PAGES_FAILED)}: {ex.Message}");
            }
        }
        #endregion

        #region Private Methods - UI Operations
        private void ToggleFullScreen()
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.Tag = this.WindowState;
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = (FormWindowState)this.Tag;
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
        }
        #endregion

        #region Private Methods - State Processing
        private void Login_Process()
        {
            switch (Globals.AppState)
            {
                case e_App_State.LOGIN:
                    HandleLoginState();
                    break;
                case e_App_State.ACTIVE:
                    HandleActiveState();
                    break;
                case e_App_State.DEACTIVE:
                    HandleDeactiveState();
                    break;
            }
        }

        private void HandleLoginState()
        {
            if (Globals.AppRenderState != e_App_Render_State.LOGIN)
            {
                Globals.AppRenderState = e_App_Render_State.LOGIN;
                this.Invoke(new Action(() =>
                {
                    NavMenu.Nodes[NavMenu.Nodes.Count - 1].Remove();
                    NavMenu.CreateNode("DM", 2001);
                    NavMenu.SelectPage(2001);
                    NavMenu.Enabled = false;
                    NavMenu.Visible = false;
                }));
            }

            if (Globals.CurrentUser.Username != string.Empty)
            {
                UpdateUserDisplay();
                Globals.AppState = Globals.ACTIVE_State ? e_App_State.ACTIVE : e_App_State.DEACTIVE;
            }
            else
            {
                Globals.AppState = e_App_State.LOGIN;
            }
        }

        private void HandleActiveState()
        {
            if (Globals.AppRenderState != e_App_Render_State.ACTIVE)
            {
                Globals.AppRenderState = e_App_Render_State.ACTIVE;
                this.Invoke(new Action(() =>
                {
                    NavMenu.SelectPage(1002);
                    NavMenu.Enabled = true;
                    NavMenu.Visible = true;
                }));
            }

            if (Globals.CurrentUser.Username == string.Empty)
            {
                Globals.AppState = e_App_State.LOGIN;
            }
            else
            {
                Globals.AppState = Globals.ACTIVE_State ? e_App_State.ACTIVE : e_App_State.DEACTIVE;
            }
        }

        private void HandleDeactiveState()
        {
            if (Globals.AppRenderState != e_App_Render_State.DEACTIVE)
            {
                this.Invoke(new Action(() =>
                {
                    if (Globals.AppRenderState == e_App_Render_State.LOGIN)
                    {
                        NavMenu.Nodes[NavMenu.Nodes.Count - 1].Remove();
                    }
                    NavMenu.CreateNode("DMA", 2001);
                    NavMenu.SelectPage(2001);
                    NavMenu.Enabled = false;
                    NavMenu.Visible = false;
                }));
                Globals.AppRenderState = e_App_Render_State.DEACTIVE;
            }

            if (Globals.CurrentUser.Username == string.Empty)
            {
                Globals.AppState = e_App_State.LOGIN;
            }
            else
            {
                Globals.AppState = Globals.ACTIVE_State ? e_App_State.ACTIVE : e_App_State.DEACTIVE;
            }
        }

        private void UpdateUserDisplay()
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
                        opUser.Text = $"[OPERATOR] {Globals.CurrentUser.Username}";
                        opUser.ForeColor = Color.Yellow;
                        break;
                    case "Worker":
                        opUser.Text = $"[WORKER] {Globals.CurrentUser.Username}";
                        opUser.ForeColor = Color.Green;
                        break;
                    default:
                        opUser.Text = "Không xác định";
                        break;
                }
            });
        }

        private void App_State_Process()
        {
            switch (Globals.AppState)
            {
                case e_App_State.LOGIN:
                    HandleLoginStatusDisplay();
                    break;
                case e_App_State.ACTIVE:
                    HandleActiveStatusDisplay();
                    break;
                case e_App_State.DEACTIVE:
                    _pLogin.Show();
                    break;
            }
        }

        private void HandleLoginStatusDisplay()
        {
            Globals.APP_Ready = false;
            Globals.Device_Ready = false;
            this.InvokeIfRequired(() =>
            {
                lblAllStatus.Text = "Chưa đăng nhập";
                lblAllStatus.FillColor = Color.Red;
                lblAllStatus.ForeColor = Color.Yellow;
            });
        }

        private void HandleActiveStatusDisplay()
        {
            Globals.APP_Ready = true;
            
            if (Globals.CameraMain_State != e_Camera_State.CONNECTED || 
                Globals.CameraSub_State != e_Camera_State.CONNECTED || 
                Globals.PLC_Connected != true)
            {
                Globals.Device_Ready = false;
                this.InvokeIfRequired(() =>
                {
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
                UpdateProductionStatusDisplay();
            }
        }

        private void UpdateProductionStatusDisplay()
        {
            switch (Globals.Production_State)
            {
                case Production.e_Production_State.NoSelectedPO:
                    SetStatusDisplay("Chưa chọn đơn hàng", Color.Yellow, Color.Black);
                    break;
                case Production.e_Production_State.Ready:
                    SetStatusDisplay("Sẵn sàng sản xuất", Color.Yellow, Color.Black);
                    break;
                case Production.e_Production_State.Running:
                    SetStatusDisplay("Đang sản xuất", Color.Green, Color.White);
                    break;
                case Production.e_Production_State.Completed:
                    SetStatusDisplay("Đã kết thúc đơn", Color.Blue, Color.White);
                    break;
                case Production.e_Production_State.Editing:
                case Production.e_Production_State.Editting_ProductionDate:
                    SetStatusDisplay("Đang chỉnh", Color.Yellow, Color.Black);
                    break;
                case Production.e_Production_State.Saving:
                    SetStatusDisplay("Đang lưu", Color.Yellow, Color.Black);
                    break;
                case Production.e_Production_State.Error:
                    SetStatusDisplay("Lỗi sản xuất", Color.Red, Color.Yellow);
                    break;
            }
        }

        private void SetStatusDisplay(string text, Color fillColor, Color foreColor)
        {
            this.InvokeIfRequired(() =>
            {
                lblAllStatus.Text = text;
                lblAllStatus.FillColor = fillColor;
                lblAllStatus.ForeColor = foreColor;
            });
        }
        #endregion

        #region Private Methods - Background Processing
        private void Main_Process_Async()
        {
            try
            {
                while (!WK_Main_Proccess.CancellationPending)
                {
                    try
                    {
                        opClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");
                        lblStatus.Text = Globals.Production_State.ToString();
                        Login_Process();
                        App_State_Process();
                    }
                    catch (Exception ex)
                    {
                        Globals.Log.WriteLogAsync("System", e_LogType.Error, ErrorCodes.Main.MAIN_PROCESS_ERROR + ex);
                        this.ShowErrorTip($"[{ErrorCodes.Main.MAIN_PROCESS_ERROR}] {ErrorCodes.GetErrorDescription(ErrorCodes.Main.MAIN_PROCESS_ERROR)}: {ex.Message}");
                    }
                    Thread.Sleep(100);
                }
            }
            catch (TaskCanceledException) { }
        }

        private void Start_Main_Process_Task()
        {
            WK_Main_Proccess.WorkerSupportsCancellation = true;
            WK_Main_Proccess.DoWork += (s, e) => Main_Process_Async();
            WK_Main_Proccess.RunWorkerAsync();
        }

        #endregion
    }
}
