namespace TeraCharts
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
            this.ucBCSimple1 = new TeraCharts.Barcharts.ucBCSimple();
            this.SuspendLayout();
            // 
            // ucBCSimple1
            // 
            this.ucBCSimple1.DataSource = "D:\\Work\\ChartCS";
            this.ucBCSimple1.Location = new System.Drawing.Point(12, 12);
            this.ucBCSimple1.Name = "ucBCSimple1";
            this.ucBCSimple1.Size = new System.Drawing.Size(719, 404);
            this.ucBCSimple1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1123, 520);
            this.Controls.Add(this.ucBCSimple1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Barcharts.ucBCSimple ucBCSimple1;
    }
}

