namespace QR_MASAN_01
{
    partial class ScanQR_V2
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.btnSearch = new Sunny.UI.UISymbolButton();
            this.ipQRContent = new Sunny.UI.UITextBox();
            this.btnKeyBoard = new Sunny.UI.UISymbolButton();
            this.opCMD = new Sunny.UI.UIListBox();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.opScanerCOM = new Sunny.UI.UIPanel();
            this.opScanerSTT = new Sunny.UI.UIPanel();
            this.ipSWMode = new Sunny.UI.UISwitch();
            this.opModeMess = new Sunny.UI.UIPanel();
            this.WK_Check = new System.ComponentModel.BackgroundWorker();
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiDataGridView1 = new Sunny.UI.UIDataGridView();
            this.oporderNo = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.uiTitlePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.Controls.Add(this.oporderNo, 0, 2);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel2, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.opCMD, 0, 3);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel3, 0, 4);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTitlePanel1, 0, 1);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 5;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.792285F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.4095F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.2504F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.885299F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(840, 674);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 3;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.40964F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.96386F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.50602F));
            this.uiTableLayoutPanel2.Controls.Add(this.btnSearch, 2, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.ipQRContent, 0, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.btnKeyBoard, 1, 0);
            this.uiTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(834, 54);
            this.uiTableLayoutPanel2.TabIndex = 0;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // btnSearch
            // 
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSearch.Location = new System.Drawing.Point(698, 3);
            this.btnSearch.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.RectColor = System.Drawing.Color.Blue;
            this.btnSearch.Size = new System.Drawing.Size(133, 48);
            this.btnSearch.Symbol = 57591;
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Xử lý";
            this.btnSearch.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // ipQRContent
            // 
            this.ipQRContent.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipQRContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipQRContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipQRContent.Location = new System.Drawing.Point(2, 2);
            this.ipQRContent.Margin = new System.Windows.Forms.Padding(2);
            this.ipQRContent.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipQRContent.Name = "ipQRContent";
            this.ipQRContent.Padding = new System.Windows.Forms.Padding(5);
            this.ipQRContent.ShowText = false;
            this.ipQRContent.Size = new System.Drawing.Size(600, 50);
            this.ipQRContent.TabIndex = 0;
            this.ipQRContent.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipQRContent.Watermark = "";
            // 
            // btnKeyBoard
            // 
            this.btnKeyBoard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnKeyBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnKeyBoard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnKeyBoard.Location = new System.Drawing.Point(607, 3);
            this.btnKeyBoard.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnKeyBoard.Name = "btnKeyBoard";
            this.btnKeyBoard.RectColor = System.Drawing.Color.Blue;
            this.btnKeyBoard.Size = new System.Drawing.Size(85, 48);
            this.btnKeyBoard.Symbol = 261724;
            this.btnKeyBoard.TabIndex = 1;
            this.btnKeyBoard.Text = "Nhập";
            this.btnKeyBoard.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnKeyBoard.Click += new System.EventHandler(this.btnKeyBoard_Click);
            // 
            // opCMD
            // 
            this.opCMD.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opCMD.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opCMD.ItemSelectForeColor = System.Drawing.Color.White;
            this.opCMD.Location = new System.Drawing.Point(2, 279);
            this.opCMD.Margin = new System.Windows.Forms.Padding(2);
            this.opCMD.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCMD.Name = "opCMD";
            this.opCMD.Padding = new System.Windows.Forms.Padding(2);
            this.opCMD.ShowText = false;
            this.opCMD.Size = new System.Drawing.Size(836, 336);
            this.opCMD.TabIndex = 1;
            this.opCMD.Text = "uiListBox1";
            this.opCMD.DoubleClick += new System.EventHandler(this.opCMD_DoubleClick);
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.ColumnCount = 4;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 148F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 185F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 405F));
            this.uiTableLayoutPanel3.Controls.Add(this.opScanerCOM, 1, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.opScanerSTT, 0, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.ipSWMode, 2, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.opModeMess, 3, 0);
            this.uiTableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(2, 619);
            this.uiTableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 1;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(836, 53);
            this.uiTableLayoutPanel3.TabIndex = 2;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // opScanerCOM
            // 
            this.opScanerCOM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opScanerCOM.FillColor = System.Drawing.Color.White;
            this.opScanerCOM.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opScanerCOM.Location = new System.Drawing.Point(150, 2);
            this.opScanerCOM.Margin = new System.Windows.Forms.Padding(2);
            this.opScanerCOM.MinimumSize = new System.Drawing.Size(1, 1);
            this.opScanerCOM.Name = "opScanerCOM";
            this.opScanerCOM.Size = new System.Drawing.Size(94, 49);
            this.opScanerCOM.TabIndex = 1;
            this.opScanerCOM.Text = "COM XX";
            this.opScanerCOM.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opScanerSTT
            // 
            this.opScanerSTT.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opScanerSTT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opScanerSTT.Location = new System.Drawing.Point(2, 2);
            this.opScanerSTT.Margin = new System.Windows.Forms.Padding(2);
            this.opScanerSTT.MinimumSize = new System.Drawing.Size(1, 1);
            this.opScanerSTT.Name = "opScanerSTT";
            this.opScanerSTT.Size = new System.Drawing.Size(144, 47);
            this.opScanerSTT.TabIndex = 0;
            this.opScanerSTT.Text = "Mất kết nối";
            this.opScanerSTT.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipSWMode
            // 
            this.ipSWMode.ActiveColor = System.Drawing.Color.Red;
            this.ipSWMode.ActiveText = "Tự kích hoạt";
            this.ipSWMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipSWMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipSWMode.InActiveColor = System.Drawing.Color.Green;
            this.ipSWMode.InActiveText = "Không kích hoạt";
            this.ipSWMode.Location = new System.Drawing.Point(249, 3);
            this.ipSWMode.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipSWMode.Name = "ipSWMode";
            this.ipSWMode.Radius = 0;
            this.ipSWMode.Size = new System.Drawing.Size(179, 47);
            this.ipSWMode.SwitchShape = Sunny.UI.UISwitch.UISwitchShape.Square;
            this.ipSWMode.TabIndex = 2;
            this.ipSWMode.Text = "Tự động thêm";
            this.ipSWMode.ValueChanged += new Sunny.UI.UISwitch.OnValueChanged(this.ipSWMode_ValueChanged);
            // 
            // opModeMess
            // 
            this.opModeMess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opModeMess.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opModeMess.Location = new System.Drawing.Point(433, 2);
            this.opModeMess.Margin = new System.Windows.Forms.Padding(2);
            this.opModeMess.MinimumSize = new System.Drawing.Size(1, 1);
            this.opModeMess.Name = "opModeMess";
            this.opModeMess.Size = new System.Drawing.Size(401, 49);
            this.opModeMess.TabIndex = 3;
            this.opModeMess.Text = "Phần mềm chỉ hiện thị trạng thái mã, gạt sw để thay đổi";
            this.opModeMess.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WK_Check
            // 
            this.WK_Check.WorkerReportsProgress = true;
            this.WK_Check.WorkerSupportsCancellation = true;
            this.WK_Check.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_Check_DoWork);
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.uiDataGridView1);
            this.uiTitlePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(2, 62);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(2);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(836, 158);
            this.uiTitlePanel1.TabIndex = 3;
            this.uiTitlePanel1.Text = "BẢNG THÔNG TIN MÃ";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiDataGridView1
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.uiDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.uiDataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.uiDataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.uiDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.uiDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.uiDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.uiDataGridView1.DefaultCellStyle = dataGridViewCellStyle8;
            this.uiDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiDataGridView1.EnableHeadersVisualStyles = false;
            this.uiDataGridView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiDataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.Location = new System.Drawing.Point(1, 35);
            this.uiDataGridView1.Name = "uiDataGridView1";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.uiDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.uiDataGridView1.SelectedIndex = -1;
            this.uiDataGridView1.Size = new System.Drawing.Size(834, 122);
            this.uiDataGridView1.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.TabIndex = 0;
            // 
            // oporderNo
            // 
            this.oporderNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.oporderNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.oporderNo.Location = new System.Drawing.Point(2, 224);
            this.oporderNo.Margin = new System.Windows.Forms.Padding(2);
            this.oporderNo.MinimumSize = new System.Drawing.Size(1, 1);
            this.oporderNo.Name = "oporderNo";
            this.oporderNo.Size = new System.Drawing.Size(836, 51);
            this.oporderNo.TabIndex = 4;
            this.oporderNo.Text = "-";
            this.oporderNo.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScanQR_V2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiTableLayoutPanel1);
            this.Name = "ScanQR_V2";
            this.Symbol = 561958;
            this.Text = "Quét mã QR";
            this.Initialize += new System.EventHandler(this.ScanQR_Initialize);
            this.Finalize += new System.EventHandler(this.ScanQR_Finalize);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.uiTitlePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UISymbolButton btnSearch;
        private Sunny.UI.UITextBox ipQRContent;
        private Sunny.UI.UISymbolButton btnKeyBoard;
        private Sunny.UI.UIListBox opCMD;
        private System.ComponentModel.BackgroundWorker WK_Check;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UIPanel opScanerSTT;
        private Sunny.UI.UIPanel opScanerCOM;
        private Sunny.UI.UISwitch ipSWMode;
        private Sunny.UI.UIPanel opModeMess;
        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UIDataGridView uiDataGridView1;
        private Sunny.UI.UIPanel oporderNo;
    }
}