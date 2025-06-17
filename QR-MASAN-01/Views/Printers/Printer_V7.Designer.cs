namespace QR_MASAN_01.Views.Printers
{
    partial class Printer_V7
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
            this.uc_NorwixV21 = new NorwixV2.uc_NorwixV2();
            this.SuspendLayout();
            // 
            // uc_NorwixV21
            // 
            this.uc_NorwixV21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uc_NorwixV21.Location = new System.Drawing.Point(0, 0);
            this.uc_NorwixV21.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.uc_NorwixV21.Name = "uc_NorwixV21";
            this.uc_NorwixV21.Size = new System.Drawing.Size(840, 652);
            this.uc_NorwixV21.TabIndex = 0;
            // 
            // Printer_V7
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 652);
            this.Controls.Add(this.uc_NorwixV21);
            this.Name = "Printer_V7";
            this.Symbol = 362838;
            this.Text = "Máy In";
            this.ResumeLayout(false);

        }

        #endregion

        private NorwixV2.uc_NorwixV2 uc_NorwixV21;
    }
}