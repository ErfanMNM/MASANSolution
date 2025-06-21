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
using System.Windows.Forms;
using MFI_Service;
using QR_MASAN_01.Views;
using QR_MASAN_01.Mid;
using QR_MASAN_01.Views.Settings;
using QR_MASAN_01.Views.Printers;



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

        F1PLC _f1PLC = new F1PLC();
        public FMainQR01()
        {
            InitializeComponent();

            WKCheck.RunWorkerAsync();

            
            ClockWK.RunWorkerAsync();

            _setings.LoadSettings("C:/Phan_Mem/Configs.xlsx");

            RenderControlForm();
        }

        private void btnAppClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void RenderControlForm()
        {
            UIStyles.CultureInfo = CultureInfos.en_US;
            this.MainTabControl = uiTabControl1;

            uiNavMenu1.TabControl = uiTabControl1;
            uiNavMenu1.CreateNode(AddPage(_F1Dashboard, 1001));
           uiNavMenu1.CreateNode(AddPage(_FMFI, 1003));
           uiNavMenu1.CreateNode(AddPage(scanQR, 1004));
            uiNavMenu1.CreateNode(AddPage(FormTest, 1998));
            uiNavMenu1.CreateNode(AddPage(_f1PLC, 1009));
            uiNavMenu1.CreateNode(AddPage(_FStatistics, 1002));
            uiNavMenu1.SelectPage(1001);
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
           
        }
        //kiểm tra mấy thứ linh tinh
        bool InternetConnection = false;
        double InternetSpeed = 0;
        private void WKCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            int demso = 0;

            while (!WKCheck.CancellationPending)
            {
                demso++;
                Thread.Sleep(500);

                //internet
                if (demso == 10)
                {
                    InternetConnection = Internet.IsOK();
                    InternetSpeed = Internet.GetInternetSpeed();

                    if (InternetConnection)
                    {
                        lblInternet.Text = $"Internet:{InternetSpeed:F1} KBps";
                        lblInternet.FillColor = Color.FromArgb(243, 249, 255);
                    }
                    else
                    {
                        lblInternet.Text = "Internet: Lỗi";
                        lblInternet.FillColor = Color.Red;
                    }

                    demso = 0;
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
                lblClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                Thread.Sleep(100);
            }
            
        }

        private void uiSymbolLabel1_Click(object sender, EventArgs e)
        {
            string value = "";
            if (this.ShowInputPasswordDialog(ref value, false, "Enter Password", false, 30))
            {
                if (value != "tantien512" || string.IsNullOrEmpty(value))
                {
                    this.ShowWarningDialog("Incorrect password.", "Incorrect password or you do not have the authorization to perform this action. Please check again.", UIStyle.Red);
                    return;
                }
                else {
                    this.ShowWarningDialog("SU Enable.", "SU Enable.", UIStyle.Red);
                    //uiNavMenu1.CreateNode(AddPage(new F1Superuser(), 1999));
                }

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
    }
}
