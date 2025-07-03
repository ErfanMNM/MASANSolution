namespace QR_MASAN_01.Views
{
    partial class FormTest
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
            this.uiSymbolButton1 = new Sunny.UI.UISymbolButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.uiTitlePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.pictureBox1);
            this.uiTitlePanel1.Controls.Add(this.uiSymbolButton1);
            this.uiTitlePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 45, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(868, 649);
            this.uiTitlePanel1.TabIndex = 0;
            this.uiTitlePanel1.Text = "Chương trình test giả lập";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.TitleHeight = 45;
            // 
            // uiSymbolButton1
            // 
            this.uiSymbolButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiSymbolButton1.Location = new System.Drawing.Point(370, 48);
            this.uiSymbolButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton1.Name = "uiSymbolButton1";
            this.uiSymbolButton1.Size = new System.Drawing.Size(139, 76);
            this.uiSymbolButton1.TabIndex = 1;
            this.uiSymbolButton1.Text = "Sinh mã 2FA";
            this.uiSymbolButton1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiSymbolButton1.Click += new System.EventHandler(this.uiSymbolButton1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(4, 48);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(360, 270);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // FormTest
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(868, 649);
            this.Controls.Add(this.uiTitlePanel1);
            this.Name = "FormTest";
            this.Text = "FormTest";
            this.Load += new System.EventHandler(this.FormTest_Load);
            this.uiTitlePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UISymbolButton uiSymbolButton1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}