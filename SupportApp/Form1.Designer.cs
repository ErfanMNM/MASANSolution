namespace SupportApp
{
    partial class Form1
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
            this.opConsole = new System.Windows.Forms.ListBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnDung = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // opConsole
            // 
            this.opConsole.FormattingEnabled = true;
            this.opConsole.Location = new System.Drawing.Point(2, 1);
            this.opConsole.Name = "opConsole";
            this.opConsole.Size = new System.Drawing.Size(407, 524);
            this.opConsole.TabIndex = 0;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(2, 526);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(105, 43);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Mở File";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(113, 526);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(105, 43);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Gửi dữ liệu";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(426, 9);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(35, 13);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "label1";
            // 
            // btnDung
            // 
            this.btnDung.Location = new System.Drawing.Point(224, 526);
            this.btnDung.Name = "btnDung";
            this.btnDung.Size = new System.Drawing.Size(105, 43);
            this.btnDung.TabIndex = 4;
            this.btnDung.Text = "Dừng gửi";
            this.btnDung.UseVisualStyleBackColor = true;
            this.btnDung.Click += new System.EventHandler(this.btnDung_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 570);
            this.Controls.Add(this.btnDung);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.opConsole);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox opConsole;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnDung;
    }
}

