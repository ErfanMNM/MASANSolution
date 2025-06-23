namespace QR_MASAN_01.Views
{
    partial class LoginForm
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
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.ipPassword = new Sunny.UI.UITextBox();
            this.ipUserName = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.btnLogin = new Sunny.UI.UISymbolButton();
            this.uiTitlePanel1.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.uiTableLayoutPanel1);
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(169, 130);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 50, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(476, 246);
            this.uiTitlePanel1.TabIndex = 0;
            this.uiTitlePanel1.Text = "Đăng nhập";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.TitleHeight = 50;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Controls.Add(this.ipPassword, 0, 1);
            this.uiTableLayoutPanel1.Controls.Add(this.ipUserName, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel2, 0, 2);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(1, 50);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 3;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(474, 195);
            this.uiTableLayoutPanel1.TabIndex = 7;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // ipPassword
            // 
            this.ipPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipPassword.Location = new System.Drawing.Point(4, 70);
            this.ipPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipPassword.Name = "ipPassword";
            this.ipPassword.Padding = new System.Windows.Forms.Padding(5);
            this.ipPassword.PasswordChar = '*';
            this.ipPassword.ShowText = false;
            this.ipPassword.Size = new System.Drawing.Size(466, 54);
            this.ipPassword.Symbol = 361475;
            this.ipPassword.TabIndex = 3;
            this.ipPassword.Text = "Mật khẩu";
            this.ipPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipPassword.Watermark = "";
            this.ipPassword.DoubleClick += new System.EventHandler(this.ipPassword_DoubleClick);
            // 
            // ipUserName
            // 
            this.ipUserName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipUserName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipUserName.Location = new System.Drawing.Point(4, 5);
            this.ipUserName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipUserName.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipUserName.Name = "ipUserName";
            this.ipUserName.Padding = new System.Windows.Forms.Padding(5);
            this.ipUserName.ShowText = false;
            this.ipUserName.Size = new System.Drawing.Size(466, 55);
            this.ipUserName.Symbol = 62142;
            this.ipUserName.TabIndex = 0;
            this.ipUserName.Text = "Tên vận hành";
            this.ipUserName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipUserName.Watermark = "";
            this.ipUserName.DoubleClick += new System.EventHandler(this.ipUserName_DoubleClick);
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 2;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.60684F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.39316F));
            this.uiTableLayoutPanel2.Controls.Add(this.btnLogin, 1, 0);
            this.uiTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel2.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(3, 132);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(468, 60);
            this.uiTableLayoutPanel2.TabIndex = 6;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // btnLogin
            // 
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLogin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.Location = new System.Drawing.Point(296, 3);
            this.btnLogin.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(169, 54);
            this.btnLogin.Symbol = 560023;
            this.btnLogin.SymbolSize = 29;
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Đăng Nhập";
            this.btnLogin.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiTitlePanel1);
            this.Name = "LoginForm";
            this.Symbol = 61584;
            this.Text = "Đăng nhập";
            this.uiTitlePanel1.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UITextBox ipPassword;
        private Sunny.UI.UITextBox ipUserName;
        private Sunny.UI.UISymbolButton btnLogin;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
    }
}