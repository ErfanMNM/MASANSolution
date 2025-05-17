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
            this.hslGobang1 = new HslControls.HslGobang();
            this.SuspendLayout();
            // 
            // hslGobang1
            // 
            this.hslGobang1.BackColor = System.Drawing.Color.Transparent;
            this.hslGobang1.Location = new System.Drawing.Point(12, 12);
            this.hslGobang1.Name = "hslGobang1";
            this.hslGobang1.Size = new System.Drawing.Size(853, 625);
            this.hslGobang1.TabIndex = 0;
            // 
            // FormTest
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(868, 649);
            this.Controls.Add(this.hslGobang1);
            this.Name = "FormTest";
            this.Text = "FormTest";
            this.ResumeLayout(false);

        }

        #endregion

        private HslControls.HslGobang hslGobang1;
    }
}