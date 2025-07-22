namespace MASAN_SERIALIZATION.Views.Login
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
            this.ucUser_Login1 = new SpT.Auth.ucUser_Login();
            this.SuspendLayout();
            // 
            // ucUser_Login1
            // 
            this.ucUser_Login1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ucUser_Login1.IS2FAEnabled = true;
            this.ucUser_Login1.Location = new System.Drawing.Point(73, 155);
            this.ucUser_Login1.MinimumSize = new System.Drawing.Size(1, 1);
            this.ucUser_Login1.Name = "ucUser_Login1";
            this.ucUser_Login1.Size = new System.Drawing.Size(648, 324);
            this.ucUser_Login1.TabIndex = 0;
            this.ucUser_Login1.Text = "ucUser_Login1";
            this.ucUser_Login1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ucUser_Login1.OnLoginAction += new System.EventHandler<SpT.Auth.LoginActionEventArgs>(this.ucUser_Login1_OnLoginAction);
            // 
            // PLogin
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.ucUser_Login1);
            this.Name = "PLogin";
            this.Text = "PLogin";
            this.Initialize += new System.EventHandler(this.PLogin_Initialize);
            this.ResumeLayout(false);

        }

        #endregion

        private SpT.Auth.ucUser_Login ucUser_Login1;
    }
}