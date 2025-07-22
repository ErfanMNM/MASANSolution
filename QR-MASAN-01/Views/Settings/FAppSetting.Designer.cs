namespace QR_MASAN_01.Views.Settings
{
    partial class FAppSetting
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
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.treeView = new Sunny.UI.UITreeView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.uc_UserSetting1 = new SpT.Auth.uc_UserSetting();
            this.uc_UserManager2 = new SpT.Auth.uc_UserManager();
            this.uiTabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPage3);
            this.uiTabControl1.Controls.Add(this.tabPage4);
            this.uiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTabControl1.ItemSize = new System.Drawing.Size(150, 40);
            this.uiTabControl1.Location = new System.Drawing.Point(0, 0);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(840, 674);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 1;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.treeView);
            this.tabPage3.Location = new System.Drawing.Point(0, 40);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(840, 634);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Cài Đặt Ứng Dụng";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.FillColor = System.Drawing.Color.White;
            this.treeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeView.MinimumSize = new System.Drawing.Size(1, 1);
            this.treeView.Name = "treeView";
            this.treeView.ScrollBarStyleInherited = false;
            this.treeView.ShowText = false;
            this.treeView.Size = new System.Drawing.Size(840, 634);
            this.treeView.TabIndex = 1;
            this.treeView.Text = "uiTreeView1";
            this.treeView.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.uiTableLayoutPanel1);
            this.tabPage4.Location = new System.Drawing.Point(0, 40);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(840, 634);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Cài Đặt Tài Khoản";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel2, 0, 0);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 2;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.7918F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.2082F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(840, 634);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 2;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Controls.Add(this.uc_UserSetting1, 0, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.uc_UserManager2, 1, 0);
            this.uiTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.uiTableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(836, 299);
            this.uiTableLayoutPanel2.TabIndex = 0;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // uc_UserSetting1
            // 
            this.uc_UserSetting1.CurrentUserName = null;
            this.uc_UserSetting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uc_UserSetting1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uc_UserSetting1.IS2FAEnabled = false;
            this.uc_UserSetting1.Location = new System.Drawing.Point(3, 3);
            this.uc_UserSetting1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uc_UserSetting1.Name = "uc_UserSetting1";
            this.uc_UserSetting1.Size = new System.Drawing.Size(412, 293);
            this.uc_UserSetting1.TabIndex = 1;
            this.uc_UserSetting1.Text = "uc_UserSetting1";
            this.uc_UserSetting1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            userData1.Key2FA = null;
            userData1.Password = null;
            userData1.Role = null;
            userData1.Salt = null;
            userData1.Username = "";
            this.uc_UserSetting1.userData = userData1;
            // 
            // uc_UserManager2
            // 
            this.uc_UserManager2.CurrentUserName = "";
            this.uc_UserManager2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uc_UserManager2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uc_UserManager2.IS2FAEnabled = false;
            this.uc_UserManager2.Location = new System.Drawing.Point(421, 3);
            this.uc_UserManager2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uc_UserManager2.Name = "uc_UserManager2";
            this.uc_UserManager2.Size = new System.Drawing.Size(412, 293);
            this.uc_UserManager2.TabIndex = 2;
            this.uc_UserManager2.Text = null;
            this.uc_UserManager2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uc_UserManager2.OnAction += new System.EventHandler<SpT.Auth.LoginActionEventArgs>(this.uc_UserManager2_OnAction);
            // 
            // FAppSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiTabControl1);
            this.Name = "FAppSetting";
            this.Text = "FAppSetting";
            this.Initialize += new System.EventHandler(this.FAppSetting_Initialize);
            this.uiTabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITabControl uiTabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private Sunny.UI.UITreeView treeView;
        private System.Windows.Forms.TabPage tabPage4;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private SpT.Auth.uc_UserSetting uc_UserSetting1;
        private SpT.Auth.uc_UserManager uc_UserManager2;
    }
}