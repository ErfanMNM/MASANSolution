namespace QR_MASAN_01.Views.Printers
{
    partial class MyLanPrinter
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
            this.ucPrinter1 = new Printer.ucPrinter();
            this.SuspendLayout();
            // 
            // ucPrinter1
            // 
            this.ucPrinter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucPrinter1.Location = new System.Drawing.Point(0, 0);
            this.ucPrinter1.Name = "ucPrinter1";
            this.ucPrinter1.Size = new System.Drawing.Size(840, 652);
            this.ucPrinter1.TabIndex = 0;
            // 
            // MyLanPrinter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 652);
            this.Controls.Add(this.ucPrinter1);
            this.Name = "MyLanPrinter";
            this.Symbol = 57594;
            this.Text = "Máy In";
            this.ResumeLayout(false);

        }

        #endregion

        private Printer.ucPrinter ucPrinter1;
    }
}