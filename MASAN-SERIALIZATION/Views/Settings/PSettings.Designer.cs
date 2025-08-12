namespace MASAN_SERIALIZATION.Views.Settings
{
    partial class PSettings
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
            SpT.Auth.UserData userData1 = new SpT.Auth.UserData();
            this.btnSave = new Sunny.UI.UIButton();
            this.btnReset = new Sunny.UI.UIButton();
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.uc_UserSetting1 = new SpT.Auth.uc_UserSetting();
            this.uc_UserManager1 = new SpT.Auth.uc_UserManager();
            this.tabPageDynamic = new System.Windows.Forms.TabPage();
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.uiPanel1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.uiTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(125)))), ((int)(((byte)(50)))));
            this.btnSave.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(160)))), ((int)(((byte)(71)))));
            this.btnSave.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(94)))), ((int)(((byte)(32)))));
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnSave.Location = new System.Drawing.Point(724, 15);
            this.btnSave.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSave.Name = "btnSave";
            this.btnSave.Radius = 10;
            this.btnSave.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(125)))), ((int)(((byte)(50)))));
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "💾 Lưu";
            this.btnSave.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(124)))), ((int)(((byte)(0)))));
            this.btnReset.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(149)))), ((int)(((byte)(0)))));
            this.btnReset.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(116)))), ((int)(((byte)(0)))));
            this.btnReset.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnReset.Location = new System.Drawing.Point(617, 15);
            this.btnReset.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnReset.Name = "btnReset";
            this.btnReset.Radius = 10;
            this.btnReset.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(124)))), ((int)(((byte)(0)))));
            this.btnReset.Size = new System.Drawing.Size(100, 35);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "🔄 Khôi phục";
            this.btnReset.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // uiPanel1
            // 
            this.uiPanel1.Controls.Add(this.btnReset);
            this.uiPanel1.Controls.Add(this.btnSave);
            this.uiPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.uiPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.uiPanel1.Font = new System.Drawing.Font("Tahoma", 12F);
            this.uiPanel1.Location = new System.Drawing.Point(0, 592);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(220)))), ((int)(((byte)(232)))));
            this.uiPanel1.RectSize = 2;
            this.uiPanel1.Size = new System.Drawing.Size(840, 60);
            this.uiPanel1.TabIndex = 0;
            this.uiPanel1.Text = null;
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tabPage1.Controls.Add(this.uc_UserSetting1);
            this.tabPage1.Controls.Add(this.uc_UserManager1);
            this.tabPage1.Location = new System.Drawing.Point(0, 40);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(200, 60);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "👤 Người dùng";
            // 
            // uc_UserSetting1
            // 
            this.uc_UserSetting1.CurrentUserName = null;
            this.uc_UserSetting1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uc_UserSetting1.IS2FAEnabled = false;
            this.uc_UserSetting1.Location = new System.Drawing.Point(430, 3);
            this.uc_UserSetting1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uc_UserSetting1.Name = "uc_UserSetting1";
            this.uc_UserSetting1.Size = new System.Drawing.Size(407, 348);
            this.uc_UserSetting1.TabIndex = 1;
            this.uc_UserSetting1.Text = "uc_UserSetting1";
            this.uc_UserSetting1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            userData1.Key2FA = null;
            userData1.Password = null;
            userData1.Role = null;
            userData1.Salt = null;
            userData1.Username = "";
            this.uc_UserSetting1.userData = userData1;
            this.uc_UserSetting1.OnUserAction += new System.EventHandler<SpT.Auth.LoginActionEventArgs>(this.uc_UserSetting1_OnUserAction);
            // 
            // uc_UserManager1
            // 
            this.uc_UserManager1.CurrentUserName = "";
            this.uc_UserManager1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uc_UserManager1.IS2FAEnabled = false;
            this.uc_UserManager1.Location = new System.Drawing.Point(3, 0);
            this.uc_UserManager1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uc_UserManager1.Name = "uc_UserManager1";
            this.uc_UserManager1.Size = new System.Drawing.Size(428, 351);
            this.uc_UserManager1.TabIndex = 0;
            this.uc_UserManager1.Text = "uc_UserManager1";
            this.uc_UserManager1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uc_UserManager1.OnAction += new System.EventHandler<SpT.Auth.LoginActionEventArgs>(this.uc_UserManager1_OnAction);
            // 
            // tabPageDynamic
            // 
            this.tabPageDynamic.AutoScroll = true;
            this.tabPageDynamic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tabPageDynamic.Location = new System.Drawing.Point(0, 40);
            this.tabPageDynamic.Name = "tabPageDynamic";
            this.tabPageDynamic.Size = new System.Drawing.Size(840, 552);
            this.tabPageDynamic.TabIndex = 0;
            this.tabPageDynamic.Text = "⚙️ Cài đặt";
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPageDynamic);
            this.uiTabControl1.Controls.Add(this.tabPage1);
            this.uiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.uiTabControl1.Font = new System.Drawing.Font("Tahoma", 12F);
            this.uiTabControl1.ItemSize = new System.Drawing.Size(150, 40);
            this.uiTabControl1.Location = new System.Drawing.Point(0, 0);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(840, 592);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 1;
            this.uiTabControl1.TabSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.uiTabControl1.TabSelectedForeColor = System.Drawing.Color.White;
            this.uiTabControl1.TabUnSelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.uiTabControl1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // PSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(840, 652);
            this.Controls.Add(this.uiTabControl1);
            this.Controls.Add(this.uiPanel1);
            this.Name = "PSettings";
            this.Symbol = 61573;
            this.Text = "Cấu Hình";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.Initialize += new System.EventHandler(this.PSettings_Initialize);
            this.uiPanel1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.uiTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIButton btnSave;
        private Sunny.UI.UIButton btnReset;
        private Sunny.UI.UIPanel uiPanel1;
        private System.Windows.Forms.TabPage tabPage1;
        private SpT.Auth.uc_UserSetting uc_UserSetting1;
        private SpT.Auth.uc_UserManager uc_UserManager1;
        private System.Windows.Forms.TabPage tabPageDynamic;
        private Sunny.UI.UITabControl uiTabControl1;
    }
}