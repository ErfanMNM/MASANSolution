namespace TestApp
{
    partial class DataWizardForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.uiGroupBox_Settings = new Sunny.UI.UIGroupBox();
            this.uiLabel_DataType = new Sunny.UI.UILabel();
            this.uiComboBox_DataType = new Sunny.UI.UIComboBox();
            this.uiLabel_Count = new Sunny.UI.UILabel();
            this.uiIntegerUpDown_Count = new Sunny.UI.UIIntegerUpDown();
            this.uiLabel_Prefix = new Sunny.UI.UILabel();
            this.uiTextBox_Prefix = new Sunny.UI.UITextBox();
            this.uiLabel_StartNumber = new Sunny.UI.UILabel();
            this.uiTextBox_StartNumber = new Sunny.UI.UITextBox();
            this.uiLabel_Length = new Sunny.UI.UILabel();
            this.uiIntegerUpDown_Length = new Sunny.UI.UIIntegerUpDown();
            this.uiButton_Generate = new Sunny.UI.UIButton();
            this.uiGroupBox_Preview = new Sunny.UI.UIGroupBox();
            this.uiListBox_Preview = new Sunny.UI.UIListBox();
            this.uiLabel_Status = new Sunny.UI.UILabel();
            this.uiButton_OK = new Sunny.UI.UIButton();
            this.uiButton_Cancel = new Sunny.UI.UIButton();
            this.uiGroupBox_Settings.SuspendLayout();
            this.uiGroupBox_Preview.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiGroupBox_Settings
            // 
            this.uiGroupBox_Settings.Controls.Add(this.uiLabel_DataType);
            this.uiGroupBox_Settings.Controls.Add(this.uiComboBox_DataType);
            this.uiGroupBox_Settings.Controls.Add(this.uiLabel_Count);
            this.uiGroupBox_Settings.Controls.Add(this.uiIntegerUpDown_Count);
            this.uiGroupBox_Settings.Controls.Add(this.uiLabel_Prefix);
            this.uiGroupBox_Settings.Controls.Add(this.uiTextBox_Prefix);
            this.uiGroupBox_Settings.Controls.Add(this.uiLabel_StartNumber);
            this.uiGroupBox_Settings.Controls.Add(this.uiTextBox_StartNumber);
            this.uiGroupBox_Settings.Controls.Add(this.uiLabel_Length);
            this.uiGroupBox_Settings.Controls.Add(this.uiIntegerUpDown_Length);
            this.uiGroupBox_Settings.Controls.Add(this.uiButton_Generate);
            this.uiGroupBox_Settings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiGroupBox_Settings.Location = new System.Drawing.Point(12, 12);
            this.uiGroupBox_Settings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox_Settings.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox_Settings.Name = "uiGroupBox_Settings";
            this.uiGroupBox_Settings.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox_Settings.Size = new System.Drawing.Size(460, 180);
            this.uiGroupBox_Settings.TabIndex = 0;
            this.uiGroupBox_Settings.Text = "Cài đặt tạo dữ liệu";
            this.uiGroupBox_Settings.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiLabel_DataType
            // 
            this.uiLabel_DataType.AutoSize = true;
            this.uiLabel_DataType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_DataType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_DataType.Location = new System.Drawing.Point(6, 40);
            this.uiLabel_DataType.Name = "uiLabel_DataType";
            this.uiLabel_DataType.Size = new System.Drawing.Size(95, 20);
            this.uiLabel_DataType.TabIndex = 0;
            this.uiLabel_DataType.Text = "Loại dữ liệu:";
            // 
            // uiComboBox_DataType
            // 
            this.uiComboBox_DataType.DataSource = null;
            this.uiComboBox_DataType.FillColor = System.Drawing.Color.White;
            this.uiComboBox_DataType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiComboBox_DataType.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiComboBox_DataType.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiComboBox_DataType.Location = new System.Drawing.Point(110, 37);
            this.uiComboBox_DataType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiComboBox_DataType.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiComboBox_DataType.Name = "uiComboBox_DataType";
            this.uiComboBox_DataType.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiComboBox_DataType.Size = new System.Drawing.Size(150, 29);
            this.uiComboBox_DataType.SymbolSize = 24;
            this.uiComboBox_DataType.TabIndex = 1;
            this.uiComboBox_DataType.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiComboBox_DataType.Watermark = "";
            this.uiComboBox_DataType.Items.AddRange(new string[] { "Số tuần tự", "Mã có tiền tố", "Mã ngẫu nhiên", "Thời gian" });
            this.uiComboBox_DataType.SelectedIndexChanged += new System.EventHandler(this.UiComboBox_DataType_SelectedIndexChanged);
            // 
            // uiLabel_Count
            // 
            this.uiLabel_Count.AutoSize = true;
            this.uiLabel_Count.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Count.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Count.Location = new System.Drawing.Point(270, 40);
            this.uiLabel_Count.Name = "uiLabel_Count";
            this.uiLabel_Count.Size = new System.Drawing.Size(79, 20);
            this.uiLabel_Count.TabIndex = 2;
            this.uiLabel_Count.Text = "Số lượng:";
            // 
            // uiIntegerUpDown_Count
            // 
            this.uiIntegerUpDown_Count.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiIntegerUpDown_Count.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiIntegerUpDown_Count.Location = new System.Drawing.Point(355, 37);
            this.uiIntegerUpDown_Count.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiIntegerUpDown_Count.Maximum = 10000D;
            this.uiIntegerUpDown_Count.Minimum = 1D;
            this.uiIntegerUpDown_Count.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiIntegerUpDown_Count.Name = "uiIntegerUpDown_Count";
            this.uiIntegerUpDown_Count.Padding = new System.Windows.Forms.Padding(5);
            this.uiIntegerUpDown_Count.ShowText = false;
            this.uiIntegerUpDown_Count.Size = new System.Drawing.Size(90, 29);
            this.uiIntegerUpDown_Count.TabIndex = 3;
            this.uiIntegerUpDown_Count.Text = "100";
            this.uiIntegerUpDown_Count.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiIntegerUpDown_Count.Value = 100;
            // 
            // uiLabel_Prefix
            // 
            this.uiLabel_Prefix.AutoSize = true;
            this.uiLabel_Prefix.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Prefix.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Prefix.Location = new System.Drawing.Point(6, 80);
            this.uiLabel_Prefix.Name = "uiLabel_Prefix";
            this.uiLabel_Prefix.Size = new System.Drawing.Size(71, 20);
            this.uiLabel_Prefix.TabIndex = 4;
            this.uiLabel_Prefix.Text = "Tiền tố:";
            // 
            // uiTextBox_Prefix
            // 
            this.uiTextBox_Prefix.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox_Prefix.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTextBox_Prefix.Location = new System.Drawing.Point(80, 77);
            this.uiTextBox_Prefix.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox_Prefix.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox_Prefix.Name = "uiTextBox_Prefix";
            this.uiTextBox_Prefix.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox_Prefix.ShowText = false;
            this.uiTextBox_Prefix.Size = new System.Drawing.Size(100, 29);
            this.uiTextBox_Prefix.TabIndex = 5;
            this.uiTextBox_Prefix.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox_Prefix.Watermark = "";
            // 
            // uiLabel_StartNumber
            // 
            this.uiLabel_StartNumber.AutoSize = true;
            this.uiLabel_StartNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_StartNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_StartNumber.Location = new System.Drawing.Point(190, 80);
            this.uiLabel_StartNumber.Name = "uiLabel_StartNumber";
            this.uiLabel_StartNumber.Size = new System.Drawing.Size(89, 20);
            this.uiLabel_StartNumber.TabIndex = 6;
            this.uiLabel_StartNumber.Text = "Bắt đầu từ:";
            // 
            // uiTextBox_StartNumber
            // 
            this.uiTextBox_StartNumber.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox_StartNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTextBox_StartNumber.Location = new System.Drawing.Point(285, 77);
            this.uiTextBox_StartNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox_StartNumber.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox_StartNumber.Name = "uiTextBox_StartNumber";
            this.uiTextBox_StartNumber.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox_StartNumber.ShowText = false;
            this.uiTextBox_StartNumber.Size = new System.Drawing.Size(80, 29);
            this.uiTextBox_StartNumber.TabIndex = 7;
            this.uiTextBox_StartNumber.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox_StartNumber.Watermark = "";
            // 
            // uiLabel_Length
            // 
            this.uiLabel_Length.AutoSize = true;
            this.uiLabel_Length.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Length.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Length.Location = new System.Drawing.Point(375, 80);
            this.uiLabel_Length.Name = "uiLabel_Length";
            this.uiLabel_Length.Size = new System.Drawing.Size(70, 20);
            this.uiLabel_Length.TabIndex = 8;
            this.uiLabel_Length.Text = "Độ dài:";
            // 
            // uiIntegerUpDown_Length
            // 
            this.uiIntegerUpDown_Length.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiIntegerUpDown_Length.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiIntegerUpDown_Length.Location = new System.Drawing.Point(375, 110);
            this.uiIntegerUpDown_Length.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiIntegerUpDown_Length.Maximum = 20D;
            this.uiIntegerUpDown_Length.Minimum = 1D;
            this.uiIntegerUpDown_Length.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiIntegerUpDown_Length.Name = "uiIntegerUpDown_Length";
            this.uiIntegerUpDown_Length.Padding = new System.Windows.Forms.Padding(5);
            this.uiIntegerUpDown_Length.ShowText = false;
            this.uiIntegerUpDown_Length.Size = new System.Drawing.Size(70, 29);
            this.uiIntegerUpDown_Length.TabIndex = 9;
            this.uiIntegerUpDown_Length.Text = "6";
            this.uiIntegerUpDown_Length.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiIntegerUpDown_Length.Value = 6;
            // 
            // uiButton_Generate
            // 
            this.uiButton_Generate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_Generate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_Generate.Location = new System.Drawing.Point(200, 140);
            this.uiButton_Generate.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_Generate.Name = "uiButton_Generate";
            this.uiButton_Generate.Size = new System.Drawing.Size(100, 30);
            this.uiButton_Generate.TabIndex = 10;
            this.uiButton_Generate.Text = "Tạo dữ liệu";
            this.uiButton_Generate.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiButton_Generate.Click += new System.EventHandler(this.BtnGenerate_Click);
            // 
            // uiGroupBox_Preview
            // 
            this.uiGroupBox_Preview.Controls.Add(this.uiListBox_Preview);
            this.uiGroupBox_Preview.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiGroupBox_Preview.Location = new System.Drawing.Point(12, 200);
            this.uiGroupBox_Preview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox_Preview.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox_Preview.Name = "uiGroupBox_Preview";
            this.uiGroupBox_Preview.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox_Preview.Size = new System.Drawing.Size(460, 120);
            this.uiGroupBox_Preview.TabIndex = 1;
            this.uiGroupBox_Preview.Text = "Xem trước";
            this.uiGroupBox_Preview.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiListBox_Preview
            // 
            this.uiListBox_Preview.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiListBox_Preview.Location = new System.Drawing.Point(6, 35);
            this.uiListBox_Preview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiListBox_Preview.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiListBox_Preview.Name = "uiListBox_Preview";
            this.uiListBox_Preview.Padding = new System.Windows.Forms.Padding(2);
            this.uiListBox_Preview.ShowText = false;
            this.uiListBox_Preview.Size = new System.Drawing.Size(448, 78);
            this.uiListBox_Preview.TabIndex = 0;
            this.uiListBox_Preview.Text = "uiListBox1";
            // 
            // uiLabel_Status
            // 
            this.uiLabel_Status.AutoSize = true;
            this.uiLabel_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Status.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Status.Location = new System.Drawing.Point(12, 330);
            this.uiLabel_Status.Name = "uiLabel_Status";
            this.uiLabel_Status.Size = new System.Drawing.Size(159, 20);
            this.uiLabel_Status.TabIndex = 2;
            this.uiLabel_Status.Text = "Sẵn sàng tạo dữ liệu";
            // 
            // uiButton_OK
            // 
            this.uiButton_OK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_OK.Enabled = false;
            this.uiButton_OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_OK.Location = new System.Drawing.Point(316, 325);
            this.uiButton_OK.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_OK.Name = "uiButton_OK";
            this.uiButton_OK.Size = new System.Drawing.Size(75, 30);
            this.uiButton_OK.TabIndex = 3;
            this.uiButton_OK.Text = "OK";
            this.uiButton_OK.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiButton_OK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // uiButton_Cancel
            // 
            this.uiButton_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_Cancel.Location = new System.Drawing.Point(397, 325);
            this.uiButton_Cancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_Cancel.Name = "uiButton_Cancel";
            this.uiButton_Cancel.Size = new System.Drawing.Size(75, 30);
            this.uiButton_Cancel.TabIndex = 4;
            this.uiButton_Cancel.Text = "Hủy";
            this.uiButton_Cancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiButton_Cancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // DataWizardForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(484, 367);
            this.Controls.Add(this.uiGroupBox_Settings);
            this.Controls.Add(this.uiGroupBox_Preview);
            this.Controls.Add(this.uiLabel_Status);
            this.Controls.Add(this.uiButton_OK);
            this.Controls.Add(this.uiButton_Cancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataWizardForm";
            this.Text = "Data Wizard";
            this.uiGroupBox_Settings.ResumeLayout(false);
            this.uiGroupBox_Settings.PerformLayout();
            this.uiGroupBox_Preview.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Sunny.UI.UIGroupBox uiGroupBox_Settings;
        private Sunny.UI.UILabel uiLabel_DataType;
        private Sunny.UI.UIComboBox uiComboBox_DataType;
        private Sunny.UI.UILabel uiLabel_Count;
        private Sunny.UI.UIIntegerUpDown uiIntegerUpDown_Count;
        private Sunny.UI.UILabel uiLabel_Prefix;
        private Sunny.UI.UITextBox uiTextBox_Prefix;
        private Sunny.UI.UILabel uiLabel_StartNumber;
        private Sunny.UI.UITextBox uiTextBox_StartNumber;
        private Sunny.UI.UILabel uiLabel_Length;
        private Sunny.UI.UIIntegerUpDown uiIntegerUpDown_Length;
        private Sunny.UI.UIButton uiButton_Generate;
        private Sunny.UI.UIGroupBox uiGroupBox_Preview;
        private Sunny.UI.UIListBox uiListBox_Preview;
        private Sunny.UI.UILabel uiLabel_Status;
        private Sunny.UI.UIButton uiButton_OK;
        private Sunny.UI.UIButton uiButton_Cancel;
    }
}