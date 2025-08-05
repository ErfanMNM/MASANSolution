namespace SpT.Static
{
    partial class panelS
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiTableLayoutPanel38 = new Sunny.UI.UITableLayoutPanel();
            this.opName = new Sunny.UI.UIPanel();
            this.opValue = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel38.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTableLayoutPanel38
            // 
            this.uiTableLayoutPanel38.BackColor = System.Drawing.Color.PaleTurquoise;
            this.uiTableLayoutPanel38.ColumnCount = 1;
            this.uiTableLayoutPanel38.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel38.Controls.Add(this.opName, 0, 0);
            this.uiTableLayoutPanel38.Controls.Add(this.opValue, 0, 1);
            this.uiTableLayoutPanel38.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel38.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel38.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel38.Name = "uiTableLayoutPanel38";
            this.uiTableLayoutPanel38.RowCount = 2;
            this.uiTableLayoutPanel38.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel38.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel38.Size = new System.Drawing.Size(255, 88);
            this.uiTableLayoutPanel38.TabIndex = 12;
            this.uiTableLayoutPanel38.TagString = null;
            // 
            // opName
            // 
            this.opName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opName.FillColor = System.Drawing.Color.DarkTurquoise;
            this.opName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opName.Location = new System.Drawing.Point(2, 2);
            this.opName.Margin = new System.Windows.Forms.Padding(2);
            this.opName.MinimumSize = new System.Drawing.Size(1, 1);
            this.opName.Name = "opName";
            this.opName.Radius = 1;
            this.opName.RectColor = System.Drawing.Color.Teal;
            this.opName.Size = new System.Drawing.Size(251, 31);
            this.opName.TabIndex = 1;
            this.opName.Text = "Camera trả về";
            this.opName.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opValue
            // 
            this.opValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opValue.Location = new System.Drawing.Point(2, 37);
            this.opValue.Margin = new System.Windows.Forms.Padding(2);
            this.opValue.MinimumSize = new System.Drawing.Size(1, 1);
            this.opValue.Name = "opValue";
            this.opValue.Radius = 1;
            this.opValue.Size = new System.Drawing.Size(251, 49);
            this.opValue.TabIndex = 2;
            this.opValue.Text = "-";
            this.opValue.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uiTableLayoutPanel38);
            this.Name = "panelS";
            this.Size = new System.Drawing.Size(255, 88);
            this.uiTableLayoutPanel38.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel38;
        private Sunny.UI.UIPanel opName;
        private Sunny.UI.UIPanel opValue;
    }
}
