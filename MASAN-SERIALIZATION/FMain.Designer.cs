namespace MASAN_SERIALIZATION
{
    partial class FMain
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
            this.lblStatus = new Sunny.UI.UIPanel();
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.opClock = new Sunny.UI.UIPanel();
            this.lblAllStatus = new Sunny.UI.UIPanel();
            this.footer = new Sunny.UI.UITableLayoutPanel();
            this.NavMenu = new Sunny.UI.UINavMenu();
            this.BodyPanel = new Sunny.UI.UITableLayoutPanel();
            this.navPanel = new Sunny.UI.UITableLayoutPanel();
            this.btnAppClose = new Sunny.UI.UISymbolButton();
            this.btnMini = new Sunny.UI.UISymbolButton();
            this.TabBody = new Sunny.UI.UITabControl();
            this.opUser = new Sunny.UI.UISymbolLabel();
            this.Logotext = new Sunny.UI.UIPanel();
            this.uiPanel2 = new Sunny.UI.UIPanel();
            this.Logo = new Sunny.UI.UITableLayoutPanel();
            this.LogoImg = new Sunny.UI.UIPanel();
            this.header = new Sunny.UI.UITableLayoutPanel();
            this.uiTopbar = new Sunny.UI.UIPanel();
            this.mainPanelLayout = new Sunny.UI.UITableLayoutPanel();
            this.footer.SuspendLayout();
            this.BodyPanel.SuspendLayout();
            this.navPanel.SuspendLayout();
            this.Logo.SuspendLayout();
            this.header.SuspendLayout();
            this.uiTopbar.SuspendLayout();
            this.mainPanelLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblStatus.Location = new System.Drawing.Point(653, 0);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0);
            this.lblStatus.MinimumSize = new System.Drawing.Size(1, 1);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Radius = 0;
            this.lblStatus.RectColor = System.Drawing.Color.DodgerBlue;
            this.lblStatus.Size = new System.Drawing.Size(205, 39);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "...";
            this.lblStatus.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel1
            // 
            this.uiPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.uiPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.uiPanel1.ForeColor = System.Drawing.Color.White;
            this.uiPanel1.Location = new System.Drawing.Point(0, 0);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Radius = 0;
            this.uiPanel1.RectColor = System.Drawing.Color.DodgerBlue;
            this.uiPanel1.Size = new System.Drawing.Size(183, 39);
            this.uiPanel1.TabIndex = 7;
            this.uiPanel1.Text = "MASAN 12";
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opClock
            // 
            this.opClock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opClock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.opClock.Location = new System.Drawing.Point(858, 0);
            this.opClock.Margin = new System.Windows.Forms.Padding(0);
            this.opClock.MinimumSize = new System.Drawing.Size(1, 1);
            this.opClock.Name = "opClock";
            this.opClock.Radius = 0;
            this.opClock.Size = new System.Drawing.Size(166, 39);
            this.opClock.TabIndex = 1;
            this.opClock.Text = "20/11/2024 17:27:00.000";
            this.opClock.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAllStatus
            // 
            this.lblAllStatus.FillColor = System.Drawing.Color.Red;
            this.lblAllStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblAllStatus.ForeColor = System.Drawing.Color.Yellow;
            this.lblAllStatus.Location = new System.Drawing.Point(357, 0);
            this.lblAllStatus.Margin = new System.Windows.Forms.Padding(0);
            this.lblAllStatus.MinimumSize = new System.Drawing.Size(1, 1);
            this.lblAllStatus.Name = "lblAllStatus";
            this.lblAllStatus.Radius = 0;
            this.lblAllStatus.RectColor = System.Drawing.Color.DodgerBlue;
            this.lblAllStatus.RectSize = 2;
            this.lblAllStatus.Size = new System.Drawing.Size(296, 39);
            this.lblAllStatus.TabIndex = 3;
            this.lblAllStatus.Text = "Chưa sẵn sàng";
            this.lblAllStatus.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // footer
            // 
            this.footer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.footer.ColumnCount = 5;
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 183F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 296F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 205F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 166F));
            this.footer.Controls.Add(this.uiPanel1, 0, 0);
            this.footer.Controls.Add(this.opClock, 4, 0);
            this.footer.Controls.Add(this.lblStatus, 3, 0);
            this.footer.Controls.Add(this.lblAllStatus, 2, 0);
            this.footer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footer.Location = new System.Drawing.Point(0, 729);
            this.footer.Margin = new System.Windows.Forms.Padding(0);
            this.footer.Name = "footer";
            this.footer.RowCount = 1;
            this.footer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footer.Size = new System.Drawing.Size(1024, 39);
            this.footer.TabIndex = 3;
            this.footer.TagString = null;
            // 
            // NavMenu
            // 
            this.NavMenu.BackColor = System.Drawing.Color.LightBlue;
            this.NavMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NavMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NavMenu.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.NavMenu.FillColor = System.Drawing.Color.LightBlue;
            this.NavMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.NavMenu.ForeColor = System.Drawing.Color.Black;
            this.NavMenu.FullRowSelect = true;
            this.NavMenu.HotTracking = true;
            this.NavMenu.HoverColor = System.Drawing.Color.DeepSkyBlue;
            this.NavMenu.ItemHeight = 40;
            this.NavMenu.LineColor = System.Drawing.Color.White;
            this.NavMenu.Location = new System.Drawing.Point(0, 0);
            this.NavMenu.Margin = new System.Windows.Forms.Padding(0);
            this.NavMenu.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.NavMenu.Name = "NavMenu";
            this.NavMenu.SelectedColor = System.Drawing.Color.DodgerBlue;
            this.NavMenu.SelectedColor2 = System.Drawing.Color.DodgerBlue;
            this.NavMenu.SelectedForeColor = System.Drawing.SystemColors.HighlightText;
            this.NavMenu.SelectedHighColor = System.Drawing.Color.Blue;
            this.NavMenu.ShowLines = false;
            this.NavMenu.ShowPlusMinus = false;
            this.NavMenu.ShowRootLines = false;
            this.NavMenu.ShowTips = true;
            this.NavMenu.Size = new System.Drawing.Size(184, 585);
            this.NavMenu.TabIndex = 0;
            this.NavMenu.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // BodyPanel
            // 
            this.BodyPanel.ColumnCount = 2;
            this.BodyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.BodyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82F));
            this.BodyPanel.Controls.Add(this.navPanel, 0, 0);
            this.BodyPanel.Controls.Add(this.TabBody, 1, 0);
            this.BodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BodyPanel.Location = new System.Drawing.Point(0, 55);
            this.BodyPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BodyPanel.Name = "BodyPanel";
            this.BodyPanel.RowCount = 1;
            this.BodyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.BodyPanel.Size = new System.Drawing.Size(1024, 674);
            this.BodyPanel.TabIndex = 2;
            this.BodyPanel.TagString = null;
            // 
            // navPanel
            // 
            this.navPanel.BackColor = System.Drawing.Color.LightBlue;
            this.navPanel.ColumnCount = 1;
            this.navPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.navPanel.Controls.Add(this.NavMenu, 0, 0);
            this.navPanel.Controls.Add(this.btnAppClose, 0, 2);
            this.navPanel.Controls.Add(this.btnMini, 0, 1);
            this.navPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navPanel.Location = new System.Drawing.Point(0, 0);
            this.navPanel.Margin = new System.Windows.Forms.Padding(0);
            this.navPanel.Name = "navPanel";
            this.navPanel.RowCount = 3;
            this.navPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.navPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.navPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.navPanel.Size = new System.Drawing.Size(184, 674);
            this.navPanel.TabIndex = 2;
            this.navPanel.TagString = null;
            // 
            // btnAppClose
            // 
            this.btnAppClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAppClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAppClose.ForeColor = System.Drawing.Color.MistyRose;
            this.btnAppClose.Location = new System.Drawing.Point(2, 635);
            this.btnAppClose.Margin = new System.Windows.Forms.Padding(2);
            this.btnAppClose.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAppClose.Name = "btnAppClose";
            this.btnAppClose.Radius = 0;
            this.btnAppClose.RectColor = System.Drawing.Color.Blue;
            this.btnAppClose.Size = new System.Drawing.Size(180, 37);
            this.btnAppClose.Symbol = 61457;
            this.btnAppClose.SymbolColor = System.Drawing.Color.MistyRose;
            this.btnAppClose.TabIndex = 0;
            this.btnAppClose.Text = "Tắt máy";
            this.btnAppClose.TipsColor = System.Drawing.Color.RoyalBlue;
            this.btnAppClose.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAppClose.Click += new System.EventHandler(this.btnAppClose_Click);
            // 
            // btnMini
            // 
            this.btnMini.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMini.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnMini.Location = new System.Drawing.Point(2, 587);
            this.btnMini.Margin = new System.Windows.Forms.Padding(2);
            this.btnMini.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnMini.Name = "btnMini";
            this.btnMini.Radius = 0;
            this.btnMini.RectColor = System.Drawing.Color.Blue;
            this.btnMini.Size = new System.Drawing.Size(180, 44);
            this.btnMini.Symbol = 61544;
            this.btnMini.TabIndex = 3;
            this.btnMini.Text = "Thu nhỏ";
            this.btnMini.TipsColor = System.Drawing.Color.RoyalBlue;
            this.btnMini.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnMini.Click += new System.EventHandler(this.btnMini_Click);
            // 
            // TabBody
            // 
            this.TabBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabBody.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.TabBody.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.TabBody.ItemSize = new System.Drawing.Size(0, 1);
            this.TabBody.Location = new System.Drawing.Point(184, 0);
            this.TabBody.MainPage = "";
            this.TabBody.Margin = new System.Windows.Forms.Padding(0);
            this.TabBody.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.TabBody.Name = "TabBody";
            this.TabBody.SelectedIndex = 0;
            this.TabBody.Size = new System.Drawing.Size(840, 674);
            this.TabBody.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabBody.TabBackColor = System.Drawing.Color.PaleTurquoise;
            this.TabBody.TabIndex = 1;
            this.TabBody.TabVisible = false;
            this.TabBody.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // opUser
            // 
            this.opUser.Dock = System.Windows.Forms.DockStyle.Right;
            this.opUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.opUser.Location = new System.Drawing.Point(835, 3);
            this.opUser.MinimumSize = new System.Drawing.Size(1, 1);
            this.opUser.Name = "opUser";
            this.opUser.Size = new System.Drawing.Size(186, 49);
            this.opUser.Symbol = 61447;
            this.opUser.SymbolSize = 30;
            this.opUser.TabIndex = 3;
            this.opUser.Text = "-";
            // 
            // Logotext
            // 
            this.Logotext.BackColor = System.Drawing.Color.Transparent;
            this.Logotext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Logotext.FillColor = System.Drawing.Color.Transparent;
            this.Logotext.FillColor2 = System.Drawing.Color.Transparent;
            this.Logotext.Font = new System.Drawing.Font("Tahoma", 15.25F, System.Drawing.FontStyle.Bold);
            this.Logotext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Logotext.Location = new System.Drawing.Point(74, 0);
            this.Logotext.Margin = new System.Windows.Forms.Padding(0);
            this.Logotext.MinimumSize = new System.Drawing.Size(1, 1);
            this.Logotext.Name = "Logotext";
            this.Logotext.Radius = 0;
            this.Logotext.RectColor = System.Drawing.Color.Transparent;
            this.Logotext.Size = new System.Drawing.Size(197, 49);
            this.Logotext.TabIndex = 2;
            this.Logotext.Text = "Tân Tiến HighTech";
            this.Logotext.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel2
            // 
            this.uiPanel2.BackColor = System.Drawing.Color.Transparent;
            this.uiPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel2.FillColor = System.Drawing.Color.Transparent;
            this.uiPanel2.FillColor2 = System.Drawing.Color.Transparent;
            this.uiPanel2.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold);
            this.uiPanel2.ForeColor = System.Drawing.Color.Blue;
            this.uiPanel2.Location = new System.Drawing.Point(277, 0);
            this.uiPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.uiPanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel2.Name = "uiPanel2";
            this.uiPanel2.Radius = 0;
            this.uiPanel2.RectColor = System.Drawing.Color.Transparent;
            this.uiPanel2.Size = new System.Drawing.Size(555, 55);
            this.uiPanel2.TabIndex = 4;
            this.uiPanel2.Text = "Serialization System";
            this.uiPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Logo
            // 
            this.Logo.BackColor = System.Drawing.Color.Transparent;
            this.Logo.ColumnCount = 2;
            this.Logo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.Logo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Logo.Controls.Add(this.LogoImg, 0, 0);
            this.Logo.Controls.Add(this.Logotext, 1, 0);
            this.Logo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Logo.Location = new System.Drawing.Point(3, 3);
            this.Logo.Name = "Logo";
            this.Logo.RowCount = 1;
            this.Logo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.3871F));
            this.Logo.Size = new System.Drawing.Size(271, 49);
            this.Logo.TabIndex = 2;
            this.Logo.TagString = null;
            // 
            // LogoImg
            // 
            this.LogoImg.BackColor = System.Drawing.Color.Transparent;
            //this.LogoImg.BackgroundImage = global::MASAN_SERIALIZATION.Properties.Resources.LogoTanTien;
            this.LogoImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.LogoImg.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.LogoImg.Location = new System.Drawing.Point(4, 5);
            this.LogoImg.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LogoImg.MinimumSize = new System.Drawing.Size(1, 1);
            this.LogoImg.Name = "LogoImg";
            this.LogoImg.RectColor = System.Drawing.Color.Transparent;
            this.LogoImg.Size = new System.Drawing.Size(66, 39);
            this.LogoImg.TabIndex = 0;
            this.LogoImg.Text = null;
            this.LogoImg.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // header
            // 
            this.header.BackColor = System.Drawing.Color.White;
            this.header.ColumnCount = 3;
            this.header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 277F));
            this.header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 555F));
            this.header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.header.Controls.Add(this.uiPanel2, 1, 0);
            this.header.Controls.Add(this.Logo, 0, 0);
            this.header.Controls.Add(this.opUser, 2, 0);
            this.header.Location = new System.Drawing.Point(0, 0);
            this.header.Margin = new System.Windows.Forms.Padding(0);
            this.header.Name = "header";
            this.header.RowCount = 1;
            this.header.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.header.Size = new System.Drawing.Size(1024, 55);
            this.header.TabIndex = 3;
            this.header.TagString = null;
            // 
            // uiTopbar
            // 
            this.uiTopbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.uiTopbar.Controls.Add(this.header);
            this.uiTopbar.FillColor = System.Drawing.Color.Azure;
            this.uiTopbar.FillColor2 = System.Drawing.Color.Azure;
            this.uiTopbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTopbar.Location = new System.Drawing.Point(0, 0);
            this.uiTopbar.Margin = new System.Windows.Forms.Padding(0);
            this.uiTopbar.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTopbar.Name = "uiTopbar";
            this.uiTopbar.RectColor = System.Drawing.SystemColors.Menu;
            this.uiTopbar.Size = new System.Drawing.Size(1024, 55);
            this.uiTopbar.TabIndex = 1;
            this.uiTopbar.Text = null;
            this.uiTopbar.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mainPanelLayout
            // 
            this.mainPanelLayout.BackColor = System.Drawing.Color.AliceBlue;
            this.mainPanelLayout.ColumnCount = 1;
            this.mainPanelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanelLayout.Controls.Add(this.uiTopbar, 0, 0);
            this.mainPanelLayout.Controls.Add(this.BodyPanel, 0, 1);
            this.mainPanelLayout.Controls.Add(this.footer, 0, 2);
            this.mainPanelLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanelLayout.Location = new System.Drawing.Point(0, 0);
            this.mainPanelLayout.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanelLayout.Name = "mainPanelLayout";
            this.mainPanelLayout.RowCount = 3;
            this.mainPanelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.161458F));
            this.mainPanelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.76041F));
            this.mainPanelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.mainPanelLayout.Size = new System.Drawing.Size(1024, 768);
            this.mainPanelLayout.TabIndex = 1;
            this.mainPanelLayout.TagString = null;
            // 
            // FMain
            // 
            this.AllowShowTitle = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.mainPanelLayout);
            this.MaximumSize = new System.Drawing.Size(1024, 768);
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "FMain";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.ShowTitle = false;
            this.Text = "Main Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 677, 462);
            this.Load += new System.EventHandler(this.FMain_Load);
            this.footer.ResumeLayout(false);
            this.BodyPanel.ResumeLayout(false);
            this.navPanel.ResumeLayout(false);
            this.Logo.ResumeLayout(false);
            this.header.ResumeLayout(false);
            this.uiTopbar.ResumeLayout(false);
            this.mainPanelLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Sunny.UI.UIPanel lblStatus;
        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIPanel opClock;
        private Sunny.UI.UIPanel lblAllStatus;
        private Sunny.UI.UITableLayoutPanel footer;
        private Sunny.UI.UINavMenu NavMenu;
        private Sunny.UI.UITableLayoutPanel BodyPanel;
        private Sunny.UI.UITableLayoutPanel navPanel;
        private Sunny.UI.UISymbolButton btnAppClose;
        private Sunny.UI.UISymbolButton btnMini;
        private Sunny.UI.UITabControl TabBody;
        private Sunny.UI.UISymbolLabel opUser;
        private Sunny.UI.UIPanel LogoImg;
        private Sunny.UI.UIPanel Logotext;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UITableLayoutPanel Logo;
        private Sunny.UI.UITableLayoutPanel header;
        private Sunny.UI.UIPanel uiTopbar;
        private Sunny.UI.UITableLayoutPanel mainPanelLayout;
    }
}

