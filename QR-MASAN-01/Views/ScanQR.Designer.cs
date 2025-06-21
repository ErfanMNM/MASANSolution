namespace QR_MASAN_01
{
    partial class ScanQR
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
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel2 = new Sunny.UI.UIPanel();
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.ipDateSearch = new Sunny.UI.UIDatePicker();
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
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel4.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel4, 0, 1);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel2, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.opCMD, 0, 2);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel3, 0, 3);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 4;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.2649F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.7351F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(840, 674);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiTableLayoutPanel4
            // 
            this.uiTableLayoutPanel4.ColumnCount = 3;
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 121F));
            this.uiTableLayoutPanel4.Controls.Add(this.uiPanel2, 1, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.uiPanel1, 0, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.ipDateSearch, 2, 0);
            this.uiTableLayoutPanel4.Location = new System.Drawing.Point(3, 60);
            this.uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            this.uiTableLayoutPanel4.RowCount = 1;
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel4.Size = new System.Drawing.Size(834, 52);
            this.uiTableLayoutPanel4.TabIndex = 3;
            this.uiTableLayoutPanel4.TagString = null;
            // 
            // uiPanel2
            // 
            this.uiPanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel2.Location = new System.Drawing.Point(593, 2);
            this.uiPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel2.Name = "uiPanel2";
            this.uiPanel2.Size = new System.Drawing.Size(118, 48);
            this.uiPanel2.TabIndex = 5;
            this.uiPanel2.Text = "Chọn tháng";
            this.uiPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel1
            // 
            this.uiPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel1.Location = new System.Drawing.Point(2, 2);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Size = new System.Drawing.Size(583, 48);
            this.uiPanel1.TabIndex = 4;
            this.uiPanel1.Text = "STT-Mã QR-Trạng thái (1 là đã active, 0 là chưa) - Thời gian Active";
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipDateSearch
            // 
            this.ipDateSearch.DateYearMonthFormat = "MM-yyyy";
            this.ipDateSearch.FillColor = System.Drawing.Color.White;
            this.ipDateSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipDateSearch.Location = new System.Drawing.Point(715, 2);
            this.ipDateSearch.Margin = new System.Windows.Forms.Padding(2);
            this.ipDateSearch.MaxLength = 7;
            this.ipDateSearch.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipDateSearch.Name = "ipDateSearch";
            this.ipDateSearch.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipDateSearch.ShowType = Sunny.UI.UIDateType.YearMonth;
            this.ipDateSearch.Size = new System.Drawing.Size(117, 48);
            this.ipDateSearch.SymbolDropDown = 61555;
            this.ipDateSearch.SymbolNormal = 61555;
            this.ipDateSearch.SymbolSize = 24;
            this.ipDateSearch.TabIndex = 6;
            this.ipDateSearch.Text = "06-2025";
            this.ipDateSearch.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipDateSearch.Value = new System.DateTime(2025, 6, 1, 0, 0, 0, 0);
            this.ipDateSearch.Watermark = "";
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
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(834, 51);
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
            this.btnSearch.Size = new System.Drawing.Size(133, 45);
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
            this.ipQRContent.Size = new System.Drawing.Size(600, 47);
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
            this.btnKeyBoard.Size = new System.Drawing.Size(85, 45);
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
            this.opCMD.Location = new System.Drawing.Point(2, 117);
            this.opCMD.Margin = new System.Windows.Forms.Padding(2);
            this.opCMD.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCMD.Name = "opCMD";
            this.opCMD.Padding = new System.Windows.Forms.Padding(2);
            this.opCMD.ShowText = false;
            this.opCMD.Size = new System.Drawing.Size(836, 500);
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
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(2, 621);
            this.uiTableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 1;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(836, 51);
            this.uiTableLayoutPanel3.TabIndex = 2;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // opScanerCOM
            // 
            this.opScanerCOM.FillColor = System.Drawing.Color.White;
            this.opScanerCOM.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opScanerCOM.Location = new System.Drawing.Point(150, 2);
            this.opScanerCOM.Margin = new System.Windows.Forms.Padding(2);
            this.opScanerCOM.MinimumSize = new System.Drawing.Size(1, 1);
            this.opScanerCOM.Name = "opScanerCOM";
            this.opScanerCOM.Size = new System.Drawing.Size(90, 47);
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
            this.ipSWMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipSWMode.InActiveColor = System.Drawing.Color.Green;
            this.ipSWMode.InActiveText = "Không kích hoạt";
            this.ipSWMode.Location = new System.Drawing.Point(249, 3);
            this.ipSWMode.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipSWMode.Name = "ipSWMode";
            this.ipSWMode.Radius = 0;
            this.ipSWMode.Size = new System.Drawing.Size(179, 45);
            this.ipSWMode.SwitchShape = Sunny.UI.UISwitch.UISwitchShape.Square;
            this.ipSWMode.TabIndex = 2;
            this.ipSWMode.Text = "Tự động thêm";
            this.ipSWMode.ValueChanged += new Sunny.UI.UISwitch.OnValueChanged(this.ipSWMode_ValueChanged);
            // 
            // opModeMess
            // 
            this.opModeMess.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opModeMess.Location = new System.Drawing.Point(435, 5);
            this.opModeMess.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opModeMess.MinimumSize = new System.Drawing.Size(1, 1);
            this.opModeMess.Name = "opModeMess";
            this.opModeMess.Size = new System.Drawing.Size(397, 41);
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
            // ScanQR
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiTableLayoutPanel1);
            this.Name = "ScanQR";
            this.Symbol = 561958;
            this.Text = "Quét mã QR";
            this.Initialize += new System.EventHandler(this.ScanQR_Initialize);
            this.Finalize += new System.EventHandler(this.ScanQR_Finalize);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel4.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
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
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIDatePicker ipDateSearch;
    }
}