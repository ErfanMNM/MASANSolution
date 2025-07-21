namespace QR_MASAN_01.Views.User_Pages
{
    partial class PLogin
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
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.ucUser_Login1 = new SpT.Auth.ucUser_Login();
            this.uiPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiPanel1
            // 
            this.uiPanel1.Controls.Add(this.ucUser_Login1);
            this.uiPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel1.Location = new System.Drawing.Point(49, 164);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Size = new System.Drawing.Size(602, 328);
            this.uiPanel1.TabIndex = 0;
            this.uiPanel1.Text = "uiPanel1";
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ucUser_Login1
            // 
            this.ucUser_Login1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucUser_Login1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ucUser_Login1.IS2FAEnabled = true;
            this.ucUser_Login1.Location = new System.Drawing.Point(0, 0);
            this.ucUser_Login1.MinimumSize = new System.Drawing.Size(1, 1);
            this.ucUser_Login1.Name = "ucUser_Login1";
            this.ucUser_Login1.Size = new System.Drawing.Size(602, 328);
            this.ucUser_Login1.TabIndex = 0;
            this.ucUser_Login1.Text = "ucUser_Login1";
            this.ucUser_Login1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ucUser_Login1.OnLoginAction += new System.EventHandler<SpT.Auth.LoginActionEventArgs>(this.ucUser_Login1_OnLoginAction);
            // 
            // PLogin
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiPanel1);
            this.Name = "PLogin";
            this.Text = "PLogin";
            this.Initialize += new System.EventHandler(this.PLogin_Initialize);
            this.uiPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIPanel uiPanel1;
        private SpT.Auth.ucUser_Login ucUser_Login1;
    }
}