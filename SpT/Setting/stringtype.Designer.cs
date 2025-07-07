namespace SpT.Setting
{
    partial class stringtype
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
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.labelName = new Sunny.UI.UIPanel();
            this.textBoxValue = new Sunny.UI.UITextBox();
            this.checkBoxValue = new Sunny.UI.UISwitch();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.textBoxValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 2;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.11547F));
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.88453F));
            this.uiTableLayoutPanel1.Controls.Add(this.labelName, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.textBoxValue, 1, 0);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 1;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(301, 54);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // labelName
            // 
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelName.Location = new System.Drawing.Point(4, 5);
            this.labelName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.labelName.MinimumSize = new System.Drawing.Size(1, 1);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(142, 44);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "uiPanel1";
            this.labelName.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxValue
            // 
            this.textBoxValue.Controls.Add(this.checkBoxValue);
            this.textBoxValue.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.textBoxValue.Location = new System.Drawing.Point(154, 5);
            this.textBoxValue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxValue.MinimumSize = new System.Drawing.Size(1, 16);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Padding = new System.Windows.Forms.Padding(5);
            this.textBoxValue.ShowText = false;
            this.textBoxValue.Size = new System.Drawing.Size(143, 44);
            this.textBoxValue.TabIndex = 1;
            this.textBoxValue.Text = "uiTextBox1";
            this.textBoxValue.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.textBoxValue.Watermark = "";
            // 
            // checkBoxValue
            // 
            this.checkBoxValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.checkBoxValue.Location = new System.Drawing.Point(-1, -1);
            this.checkBoxValue.MinimumSize = new System.Drawing.Size(1, 1);
            this.checkBoxValue.Name = "checkBoxValue";
            this.checkBoxValue.Size = new System.Drawing.Size(144, 47);
            this.checkBoxValue.TabIndex = 3;
            this.checkBoxValue.Text = "uiSwitch1";
            // 
            // stringtype
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uiTableLayoutPanel1);
            this.Name = "stringtype";
            this.Size = new System.Drawing.Size(301, 54);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.textBoxValue.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UIPanel labelName;
        private Sunny.UI.UITextBox textBoxValue;
        private Sunny.UI.UISwitch checkBoxValue;
    }
}
