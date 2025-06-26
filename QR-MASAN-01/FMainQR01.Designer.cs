namespace QR_MASAN_01
{
    partial class FMainQR01
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMainQR01));
            this.mainPanelLayout = new Sunny.UI.UITableLayoutPanel();
            this.uiTopbar = new Sunny.UI.UIPanel();
            this.header = new Sunny.UI.UITableLayoutPanel();
            this.Logo = new Sunny.UI.UITableLayoutPanel();
            this.LogoImg = new Sunny.UI.UIPanel();
            this.Logotext = new Sunny.UI.UIPanel();
            this.opUser = new Sunny.UI.UISymbolLabel();
            this.BodyPanel = new Sunny.UI.UITableLayoutPanel();
            this.navPanel = new Sunny.UI.UITableLayoutPanel();
            this.btnDeActive = new Sunny.UI.UISymbolButton();
            this.uiNavMenu1 = new Sunny.UI.UINavMenu();
            this.btnAppClose = new Sunny.UI.UISymbolButton();
            this.btnMini = new Sunny.UI.UISymbolButton();
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.footer = new Sunny.UI.UITableLayoutPanel();
            this.opLaserPrinterTime = new Sunny.UI.UIPanel();
            this.uiScrollingText1 = new Sunny.UI.UIScrollingText();
            this.lblClock = new Sunny.UI.UIPanel();
            this.lblInternet = new Sunny.UI.UIPanel();
            this.lblAllStatus = new Sunny.UI.UIPanel();
            this.WKCheck = new System.ComponentModel.BackgroundWorker();
            this.ClockWK = new System.ComponentModel.BackgroundWorker();
            this.Internet = new SPMS1.InternetClass(this.components);
            this.WK_LaserPrinterTime = new System.ComponentModel.BackgroundWorker();
            this.mainPanelLayout.SuspendLayout();
            this.uiTopbar.SuspendLayout();
            this.header.SuspendLayout();
            this.Logo.SuspendLayout();
            this.BodyPanel.SuspendLayout();
            this.navPanel.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanelLayout
            // 
            this.mainPanelLayout.BackColor = System.Drawing.Color.AliceBlue;
            resources.ApplyResources(this.mainPanelLayout, "mainPanelLayout");
            this.mainPanelLayout.Controls.Add(this.uiTopbar, 0, 0);
            this.mainPanelLayout.Controls.Add(this.BodyPanel, 0, 1);
            this.mainPanelLayout.Controls.Add(this.footer, 0, 2);
            this.mainPanelLayout.Name = "mainPanelLayout";
            this.mainPanelLayout.TagString = null;
            // 
            // uiTopbar
            // 
            this.uiTopbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.uiTopbar.Controls.Add(this.header);
            this.uiTopbar.FillColor = System.Drawing.Color.Azure;
            this.uiTopbar.FillColor2 = System.Drawing.Color.Azure;
            resources.ApplyResources(this.uiTopbar, "uiTopbar");
            this.uiTopbar.Name = "uiTopbar";
            this.uiTopbar.RectColor = System.Drawing.SystemColors.Menu;
            this.uiTopbar.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // header
            // 
            this.header.BackColor = System.Drawing.Color.Lavender;
            resources.ApplyResources(this.header, "header");
            this.header.Controls.Add(this.Logo, 0, 0);
            this.header.Controls.Add(this.opUser, 1, 0);
            this.header.Name = "header";
            this.header.TagString = null;
            // 
            // Logo
            // 
            this.Logo.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.Logo, "Logo");
            this.Logo.Controls.Add(this.LogoImg, 0, 0);
            this.Logo.Controls.Add(this.Logotext, 1, 0);
            this.Logo.Name = "Logo";
            this.Logo.TagString = null;
            this.Logo.Paint += new System.Windows.Forms.PaintEventHandler(this.Logo_Paint);
            // 
            // LogoImg
            // 
            this.LogoImg.BackColor = System.Drawing.Color.Transparent;
            this.LogoImg.BackgroundImage = global::QR_MASAN_01.Properties.Resources.LogoTanTien;
            resources.ApplyResources(this.LogoImg, "LogoImg");
            this.LogoImg.Name = "LogoImg";
            this.LogoImg.RectColor = System.Drawing.Color.Transparent;
            this.LogoImg.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Logotext
            // 
            this.Logotext.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.Logotext, "Logotext");
            this.Logotext.FillColor = System.Drawing.Color.Transparent;
            this.Logotext.FillColor2 = System.Drawing.Color.Transparent;
            this.Logotext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Logotext.Name = "Logotext";
            this.Logotext.Radius = 0;
            this.Logotext.RectColor = System.Drawing.Color.Transparent;
            this.Logotext.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opUser
            // 
            resources.ApplyResources(this.opUser, "opUser");
            this.opUser.Name = "opUser";
            this.opUser.Symbol = 62142;
            this.opUser.SymbolSize = 30;
            // 
            // BodyPanel
            // 
            resources.ApplyResources(this.BodyPanel, "BodyPanel");
            this.BodyPanel.Controls.Add(this.navPanel, 0, 0);
            this.BodyPanel.Controls.Add(this.uiTabControl1, 1, 0);
            this.BodyPanel.Name = "BodyPanel";
            this.BodyPanel.TagString = null;
            // 
            // navPanel
            // 
            resources.ApplyResources(this.navPanel, "navPanel");
            this.navPanel.Controls.Add(this.btnDeActive, 0, 1);
            this.navPanel.Controls.Add(this.uiNavMenu1, 0, 0);
            this.navPanel.Controls.Add(this.btnAppClose, 0, 3);
            this.navPanel.Controls.Add(this.btnMini, 0, 2);
            this.navPanel.Name = "navPanel";
            this.navPanel.TagString = null;
            // 
            // btnDeActive
            // 
            this.btnDeActive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeActive.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            resources.ApplyResources(this.btnDeActive, "btnDeActive");
            this.btnDeActive.ForeColor = System.Drawing.Color.DarkRed;
            this.btnDeActive.Name = "btnDeActive";
            this.btnDeActive.Radius = 0;
            this.btnDeActive.RectColor = System.Drawing.Color.Blue;
            this.btnDeActive.RectSize = 2;
            this.btnDeActive.Symbol = 557571;
            this.btnDeActive.SymbolColor = System.Drawing.Color.Purple;
            this.btnDeActive.TipsColor = System.Drawing.Color.RoyalBlue;
            this.btnDeActive.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDeActive.Click += new System.EventHandler(this.btnDeActive_Click);
            // 
            // uiNavMenu1
            // 
            this.uiNavMenu1.BackColor = System.Drawing.Color.LightBlue;
            this.uiNavMenu1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.uiNavMenu1, "uiNavMenu1");
            this.uiNavMenu1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.uiNavMenu1.FillColor = System.Drawing.Color.LightBlue;
            this.uiNavMenu1.ForeColor = System.Drawing.Color.Black;
            this.uiNavMenu1.FullRowSelect = true;
            this.uiNavMenu1.HotTracking = true;
            this.uiNavMenu1.HoverColor = System.Drawing.Color.DeepSkyBlue;
            this.uiNavMenu1.ItemHeight = 40;
            this.uiNavMenu1.LineColor = System.Drawing.Color.White;
            this.uiNavMenu1.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.uiNavMenu1.Name = "uiNavMenu1";
            this.uiNavMenu1.SelectedColor = System.Drawing.Color.DodgerBlue;
            this.uiNavMenu1.SelectedColor2 = System.Drawing.Color.DodgerBlue;
            this.uiNavMenu1.SelectedForeColor = System.Drawing.SystemColors.HighlightText;
            this.uiNavMenu1.SelectedHighColor = System.Drawing.Color.Blue;
            this.uiNavMenu1.ShowLines = false;
            this.uiNavMenu1.ShowPlusMinus = false;
            this.uiNavMenu1.ShowRootLines = false;
            this.uiNavMenu1.ShowTips = true;
            this.uiNavMenu1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnAppClose
            // 
            this.btnAppClose.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnAppClose, "btnAppClose");
            this.btnAppClose.Name = "btnAppClose";
            this.btnAppClose.Radius = 0;
            this.btnAppClose.RectColor = System.Drawing.Color.Blue;
            this.btnAppClose.Symbol = 61453;
            this.btnAppClose.TipsColor = System.Drawing.Color.RoyalBlue;
            this.btnAppClose.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAppClose.Click += new System.EventHandler(this.btnAppClose_Click);
            // 
            // btnMini
            // 
            this.btnMini.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnMini, "btnMini");
            this.btnMini.Name = "btnMini";
            this.btnMini.Radius = 0;
            this.btnMini.RectColor = System.Drawing.Color.Blue;
            this.btnMini.Symbol = 61544;
            this.btnMini.TipsColor = System.Drawing.Color.RoyalBlue;
            this.btnMini.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnMini.Click += new System.EventHandler(this.btnMini_Click);
            // 
            // uiTabControl1
            // 
            resources.ApplyResources(this.uiTabControl1, "uiTabControl1");
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabBackColor = System.Drawing.Color.PaleTurquoise;
            this.uiTabControl1.TabVisible = false;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiTabControl1.SelectedIndexChanged += new System.EventHandler(this.uiTabControl1_SelectedIndexChanged);
            // 
            // footer
            // 
            this.footer.BackColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.footer, "footer");
            this.footer.Controls.Add(this.opLaserPrinterTime, 2, 0);
            this.footer.Controls.Add(this.uiScrollingText1, 0, 0);
            this.footer.Controls.Add(this.lblClock, 4, 0);
            this.footer.Controls.Add(this.lblInternet, 3, 0);
            this.footer.Controls.Add(this.lblAllStatus, 1, 0);
            this.footer.Name = "footer";
            this.footer.TagString = null;
            // 
            // opLaserPrinterTime
            // 
            this.opLaserPrinterTime.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            resources.ApplyResources(this.opLaserPrinterTime, "opLaserPrinterTime");
            this.opLaserPrinterTime.Name = "opLaserPrinterTime";
            this.opLaserPrinterTime.Radius = 0;
            this.opLaserPrinterTime.RectColor = System.Drawing.Color.Blue;
            this.opLaserPrinterTime.RectSize = 2;
            this.opLaserPrinterTime.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiScrollingText1
            // 
            this.uiScrollingText1.Active = true;
            resources.ApplyResources(this.uiScrollingText1, "uiScrollingText1");
            this.uiScrollingText1.Interval = 50;
            this.uiScrollingText1.Name = "uiScrollingText1";
            this.uiScrollingText1.Style = Sunny.UI.UIStyle.Custom;
            // 
            // lblClock
            // 
            resources.ApplyResources(this.lblClock, "lblClock");
            this.lblClock.Name = "lblClock";
            this.lblClock.Radius = 0;
            this.lblClock.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInternet
            // 
            this.lblInternet.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.lblInternet, "lblInternet");
            this.lblInternet.Name = "lblInternet";
            this.lblInternet.Radius = 0;
            this.lblInternet.RectColor = System.Drawing.Color.DodgerBlue;
            this.lblInternet.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAllStatus
            // 
            this.lblAllStatus.FillColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.lblAllStatus, "lblAllStatus");
            this.lblAllStatus.ForeColor = System.Drawing.Color.Yellow;
            this.lblAllStatus.Name = "lblAllStatus";
            this.lblAllStatus.Radius = 0;
            this.lblAllStatus.RectColor = System.Drawing.Color.DodgerBlue;
            this.lblAllStatus.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WKCheck
            // 
            this.WKCheck.WorkerSupportsCancellation = true;
            this.WKCheck.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WKCheck_DoWork);
            // 
            // ClockWK
            // 
            this.ClockWK.WorkerSupportsCancellation = true;
            this.ClockWK.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ClockWK_DoWork);
            // 
            // Internet
            // 
            this.Internet.Url = "8.8.8.8";
            // 
            // WK_LaserPrinterTime
            // 
            this.WK_LaserPrinterTime.WorkerSupportsCancellation = true;
            this.WK_LaserPrinterTime.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_LaserPrinterTime_DoWork);
            // 
            // FMainQR01
            // 
            this.AllowShowTitle = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.mainPanelLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IsForbidAltF4 = true;
            this.Name = "FMainQR01";
            this.ShowTitle = false;
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 1024, 768);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainPanelLayout.ResumeLayout(false);
            this.uiTopbar.ResumeLayout(false);
            this.header.ResumeLayout(false);
            this.Logo.ResumeLayout(false);
            this.BodyPanel.ResumeLayout(false);
            this.navPanel.ResumeLayout(false);
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITableLayoutPanel mainPanelLayout;
        private Sunny.UI.UIPanel uiTopbar;
        private Sunny.UI.UIPanel LogoImg;
        private Sunny.UI.UITableLayoutPanel Logo;
        private Sunny.UI.UIPanel Logotext;
        private Sunny.UI.UITableLayoutPanel header;
        private System.ComponentModel.BackgroundWorker WKCheck;
        private SPMS1.InternetClass Internet;
        private Sunny.UI.UITableLayoutPanel footer;
        private Sunny.UI.UIPanel lblClock;
        private Sunny.UI.UIPanel lblInternet;
        private System.ComponentModel.BackgroundWorker ClockWK;
        private Sunny.UI.UITableLayoutPanel BodyPanel;
        private Sunny.UI.UITableLayoutPanel navPanel;
        private Sunny.UI.UINavMenu uiNavMenu1;
        private Sunny.UI.UISymbolButton btnMini;
        private Sunny.UI.UISymbolButton btnAppClose;
        private Sunny.UI.UIPanel lblAllStatus;
        private Sunny.UI.UITabControl uiTabControl1;
        private Sunny.UI.UISymbolButton btnDeActive;
        private Sunny.UI.UISymbolLabel opUser;
        private Sunny.UI.UIPanel opLaserPrinterTime;
        private Sunny.UI.UIScrollingText uiScrollingText1;
        private System.ComponentModel.BackgroundWorker WK_LaserPrinterTime;
    }
}

