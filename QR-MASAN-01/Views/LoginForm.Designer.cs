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
            this.uiTextBox1 = new Sunny.UI.UITextBox();
            this.uiTextBox2 = new Sunny.UI.UITextBox();
            this.uiLine1 = new Sunny.UI.UILine();
            this.btnLogin = new Sunny.UI.UISymbolButton();
            this.uiVerificationCode1 = new Sunny.UI.UIVerificationCode();
            this.uiTitlePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.uiVerificationCode1);
            this.uiTitlePanel1.Controls.Add(this.btnLogin);
            this.uiTitlePanel1.Controls.Add(this.uiLine1);
            this.uiTitlePanel1.Controls.Add(this.uiTextBox2);
            this.uiTitlePanel1.Controls.Add(this.uiTextBox1);
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(57, 96);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 50, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(691, 398);
            this.uiTitlePanel1.TabIndex = 0;
            this.uiTitlePanel1.Text = "Đăng nhập";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.TitleHeight = 50;
            // 
            // uiTextBox1
            // 
            this.uiTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTextBox1.Location = new System.Drawing.Point(15, 102);
            this.uiTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox1.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox1.Name = "uiTextBox1";
            this.uiTextBox1.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox1.ShowText = false;
            this.uiTextBox1.Size = new System.Drawing.Size(568, 78);
            this.uiTextBox1.Symbol = 62142;
            this.uiTextBox1.TabIndex = 0;
            this.uiTextBox1.Text = "uiTextBox1";
            this.uiTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox1.Watermark = "";
            // 
            // uiTextBox2
            // 
            this.uiTextBox2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTextBox2.Location = new System.Drawing.Point(15, 229);
            this.uiTextBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox2.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox2.Name = "uiTextBox2";
            this.uiTextBox2.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox2.PasswordChar = '*';
            this.uiTextBox2.ShowText = false;
            this.uiTextBox2.Size = new System.Drawing.Size(568, 68);
            this.uiTextBox2.Symbol = 561506;
            this.uiTextBox2.TabIndex = 3;
            this.uiTextBox2.Text = "uiTextBox2";
            this.uiTextBox2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox2.Watermark = "";
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(15, 65);
            this.uiLine1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(369, 29);
            this.uiLine1.TabIndex = 4;
            this.uiLine1.Text = "uiLine1";
            // 
            // btnLogin
            // 
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnLogin.Location = new System.Drawing.Point(449, 314);
            this.btnLogin.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(205, 63);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "uiSymbolButton1";
            this.btnLogin.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // uiVerificationCode1
            // 
            this.uiVerificationCode1.CodeLength = 6;
            this.uiVerificationCode1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiVerificationCode1.Location = new System.Drawing.Point(15, 305);
            this.uiVerificationCode1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiVerificationCode1.Name = "uiVerificationCode1";
            this.uiVerificationCode1.Size = new System.Drawing.Size(300, 72);
            this.uiVerificationCode1.TabIndex = 6;
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiTitlePanel1);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.uiTitlePanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UILine uiLine1;
        private Sunny.UI.UITextBox uiTextBox2;
        private Sunny.UI.UITextBox uiTextBox1;
        private Sunny.UI.UIVerificationCode uiVerificationCode1;
        private Sunny.UI.UISymbolButton btnLogin;
    }
}