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
            this.uiListBox1 = new Sunny.UI.UIListBox();
            this.uiTitlePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.uiListBox1);
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
            // uiListBox1
            // 
            this.uiListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiListBox1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiListBox1.ItemSelectForeColor = System.Drawing.Color.White;
            this.uiListBox1.Location = new System.Drawing.Point(5, 50);
            this.uiListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiListBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiListBox1.Name = "uiListBox1";
            this.uiListBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiListBox1.ShowText = false;
            this.uiListBox1.Size = new System.Drawing.Size(858, 274);
            this.uiListBox1.TabIndex = 0;
            this.uiListBox1.Text = "uiListBox1";
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
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UIListBox uiListBox1;
    }
}