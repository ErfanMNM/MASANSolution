namespace MASAN_SERIALIZATION.Views.SCADA
{
    partial class PStatictis
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
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.opMEMFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTitlePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Controls.Add(this.uiTitlePanel1, 0, 0);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 2;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(840, 674);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.opMEMFlow);
            this.uiTitlePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(2, 2);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(2);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(836, 333);
            this.uiTitlePanel1.TabIndex = 1;
            this.uiTitlePanel1.Text = "Thông tin dữ liệu MEM";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opMEMFlow
            // 
            this.opMEMFlow.AutoScroll = true;
            this.opMEMFlow.AutoSize = true;
            this.opMEMFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opMEMFlow.Location = new System.Drawing.Point(1, 35);
            this.opMEMFlow.Name = "opMEMFlow";
            this.opMEMFlow.Size = new System.Drawing.Size(834, 297);
            this.opMEMFlow.TabIndex = 0;
            // 
            // PStatictis
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiTableLayoutPanel1);
            this.Name = "PStatictis";
            this.Symbol = 61643;
            this.Text = "Bộ đếm";
            this.Initialize += new System.EventHandler(this.PStatictis_Initialize);
            this.Finalize += new System.EventHandler(this.PStatictis_Finalize);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTitlePanel1.ResumeLayout(false);
            this.uiTitlePanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private System.Windows.Forms.FlowLayoutPanel opMEMFlow;
    }
}