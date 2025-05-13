namespace MFI_Service
{
    partial class MFI_Service
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
            this.WK_Update = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel8 = new Sunny.UI.UIPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCloudMS = new Sunny.UI.UISymbolButton();
            this.btnUndo = new Sunny.UI.UISymbolButton();
            this.btnLoad_Code = new Sunny.UI.UISymbolButton();
            this.lblServerStatus = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel14 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel26 = new Sunny.UI.UIPanel();
            this.uiPanel25 = new Sunny.UI.UIPanel();
            this.uiPanel23 = new Sunny.UI.UIPanel();
            this.uiPanel24 = new Sunny.UI.UIPanel();
            this.uiPanel22 = new Sunny.UI.UIPanel();
            this.uiPanel13 = new Sunny.UI.UIPanel();
            this.opPallet_QR_Status = new Sunny.UI.UIPanel();
            this.uiPanel17 = new Sunny.UI.UIPanel();
            this.opCase_QR_Status = new Sunny.UI.UIPanel();
            this.opProduct_QR_Status = new Sunny.UI.UIPanel();
            this.uiPanel12 = new Sunny.UI.UIPanel();
            this.lblAllStatus = new Sunny.UI.UIPanel();
            this.uiPanel18 = new Sunny.UI.UIPanel();
            this.uiPanel16 = new Sunny.UI.UIPanel();
            this.uiPanel4 = new Sunny.UI.UIPanel();
            this.uiPanel11 = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel5 = new Sunny.UI.UITableLayoutPanel();
            this.ipPallerQRType = new Sunny.UI.UIComboBox();
            this.ipPallet_Size = new Sunny.UI.UINumPadTextBox();
            this.uiPanel2 = new Sunny.UI.UIPanel();
            this.uiPanel3 = new Sunny.UI.UIPanel();
            this.ipLOT = new Sunny.UI.UIDatePicker();
            this.uiPanel20 = new Sunny.UI.UIPanel();
            this.uiPanel10 = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            this.ipCase_Size = new Sunny.UI.UINumPadTextBox();
            this.ipBlock_Size = new Sunny.UI.UINumPadTextBox();
            this.uiPanel27 = new Sunny.UI.UIPanel();
            this.uiPanel9 = new Sunny.UI.UIPanel();
            this.ipSanLuong = new Sunny.UI.UINumPadTextBox();
            this.uiPanel6 = new Sunny.UI.UIPanel();
            this.uiPanel7 = new Sunny.UI.UIPanel();
            this.uiPanel5 = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel8 = new Sunny.UI.UITableLayoutPanel();
            this.ipProductBarcode = new Sunny.UI.UINumPadTextBox();
            this.btnScanBarcode = new Sunny.UI.UISymbolButton();
            this.uiTableLayoutPanel6 = new Sunny.UI.UITableLayoutPanel();
            this.btnEnterBatch = new Sunny.UI.UISymbolButton();
            this.ipBatchCode = new Sunny.UI.UIComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.ipConsole = new Sunny.UI.UIListBox();
            this.WK_MFI = new System.ComponentModel.BackgroundWorker();
            this.WK_Server_Status = new System.ComponentModel.BackgroundWorker();
            this.WK_LoadCloud = new System.ComponentModel.BackgroundWorker();
            this.googleService1 = new SPMS1.GoogleService(this.components);
            this.btnCaseBarcode = new Sunny.UI.UISymbolButton();
            this.uiTableLayoutPanel17 = new Sunny.UI.UITableLayoutPanel();
            this.ipCaseBarcode = new Sunny.UI.UINumPadTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel14.SuspendLayout();
            this.uiPanel23.SuspendLayout();
            this.uiPanel13.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.uiTableLayoutPanel5.SuspendLayout();
            this.uiTableLayoutPanel4.SuspendLayout();
            this.uiTableLayoutPanel8.SuspendLayout();
            this.uiTableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel17.SuspendLayout();
            this.SuspendLayout();
            // 
            // WK_Update
            // 
            this.WK_Update.WorkerReportsProgress = true;
            this.WK_Update.WorkerSupportsCancellation = true;
            this.WK_Update.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_Update_DoWork);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.77261F));
            this.tableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 57.66871F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.33129F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(836, 652);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.BackColor = System.Drawing.Color.Turquoise;
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Controls.Add(this.uiPanel8, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel2, 0, 1);
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 3;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.59627F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.40372F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(830, 369);
            this.uiTableLayoutPanel1.TabIndex = 4;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiPanel8
            // 
            this.uiPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel8.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiPanel8.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.uiPanel8.ForeColor = System.Drawing.Color.White;
            this.uiPanel8.Location = new System.Drawing.Point(2, 2);
            this.uiPanel8.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel8.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel8.Name = "uiPanel8";
            this.uiPanel8.Radius = 3;
            this.uiPanel8.RectColor = System.Drawing.Color.Teal;
            this.uiPanel8.Size = new System.Drawing.Size(826, 42);
            this.uiPanel8.TabIndex = 4;
            this.uiPanel8.Text = "Thông tin sản xuất tương ớt line 3";
            this.uiPanel8.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.16505F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.42719F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.17476F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.11165F));
            this.tableLayoutPanel3.Controls.Add(this.btnCloudMS, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnUndo, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnLoad_Code, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblServerStatus, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 320);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(824, 46);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // btnCloudMS
            // 
            this.btnCloudMS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCloudMS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCloudMS.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCloudMS.Location = new System.Drawing.Point(685, 3);
            this.btnCloudMS.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCloudMS.Name = "btnCloudMS";
            this.btnCloudMS.Size = new System.Drawing.Size(136, 40);
            this.btnCloudMS.Symbol = 558048;
            this.btnCloudMS.TabIndex = 34;
            this.btnCloudMS.Text = "Tải ERP";
            this.btnCloudMS.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCloudMS.Click += new System.EventHandler(this.btnCloudMS_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUndo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUndo.Enabled = false;
            this.btnUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnUndo.Location = new System.Drawing.Point(527, 3);
            this.btnUndo.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(152, 40);
            this.btnUndo.Symbol = 557410;
            this.btnUndo.TabIndex = 33;
            this.btnUndo.Text = "Khôi phục";
            this.btnUndo.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnLoad_Code
            // 
            this.btnLoad_Code.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad_Code.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoad_Code.Enabled = false;
            this.btnLoad_Code.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnLoad_Code.Location = new System.Drawing.Point(301, 3);
            this.btnLoad_Code.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLoad_Code.Name = "btnLoad_Code";
            this.btnLoad_Code.Size = new System.Drawing.Size(220, 40);
            this.btnLoad_Code.Symbol = 563628;
            this.btnLoad_Code.TabIndex = 28;
            this.btnLoad_Code.Text = "Lưu thông tin";
            this.btnLoad_Code.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLoad_Code.Click += new System.EventHandler(this.btnLoad_Code_Click_2);
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServerStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblServerStatus.Location = new System.Drawing.Point(4, 5);
            this.lblServerStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblServerStatus.MinimumSize = new System.Drawing.Size(1, 1);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(290, 36);
            this.lblServerStatus.TabIndex = 35;
            this.lblServerStatus.Text = "Đang kết nối máy chủ";
            this.lblServerStatus.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 2;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.56416F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.43584F));
            this.uiTableLayoutPanel2.Controls.Add(this.uiTableLayoutPanel14, 1, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.uiTableLayoutPanel3, 0, 0);
            this.uiTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(2, 48);
            this.uiTableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(826, 267);
            this.uiTableLayoutPanel2.TabIndex = 7;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // uiTableLayoutPanel14
            // 
            this.uiTableLayoutPanel14.BackColor = System.Drawing.Color.LightBlue;
            this.uiTableLayoutPanel14.ColumnCount = 2;
            this.uiTableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.04207F));
            this.uiTableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56.95793F));
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel26, 0, 5);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel25, 1, 4);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel23, 0, 4);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel22, 1, 3);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel13, 1, 2);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel17, 0, 1);
            this.uiTableLayoutPanel14.Controls.Add(this.opCase_QR_Status, 1, 1);
            this.uiTableLayoutPanel14.Controls.Add(this.opProduct_QR_Status, 1, 0);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel12, 0, 0);
            this.uiTableLayoutPanel14.Controls.Add(this.lblAllStatus, 1, 6);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel18, 0, 6);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel16, 1, 5);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel4, 0, 2);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel11, 0, 3);
            this.uiTableLayoutPanel14.Location = new System.Drawing.Point(494, 3);
            this.uiTableLayoutPanel14.Name = "uiTableLayoutPanel14";
            this.uiTableLayoutPanel14.RowCount = 7;
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28921F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28921F));
            this.uiTableLayoutPanel14.Size = new System.Drawing.Size(328, 261);
            this.uiTableLayoutPanel14.TabIndex = 13;
            this.uiTableLayoutPanel14.TagString = null;
            // 
            // uiPanel26
            // 
            this.uiPanel26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel26.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel26.Location = new System.Drawing.Point(2, 187);
            this.uiPanel26.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel26.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel26.Name = "uiPanel26";
            this.uiPanel26.Size = new System.Drawing.Size(137, 33);
            this.uiPanel26.TabIndex = 68;
            this.uiPanel26.Text = "Số mã pallet còn";
            this.uiPanel26.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel25
            // 
            this.uiPanel25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel25.FillColor = System.Drawing.Color.White;
            this.uiPanel25.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel25.Location = new System.Drawing.Point(143, 150);
            this.uiPanel25.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel25.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel25.Name = "uiPanel25";
            this.uiPanel25.Size = new System.Drawing.Size(183, 33);
            this.uiPanel25.TabIndex = 67;
            this.uiPanel25.Text = "-";
            this.uiPanel25.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel23
            // 
            this.uiPanel23.Controls.Add(this.uiPanel24);
            this.uiPanel23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel23.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel23.Location = new System.Drawing.Point(2, 150);
            this.uiPanel23.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel23.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel23.Name = "uiPanel23";
            this.uiPanel23.Size = new System.Drawing.Size(137, 33);
            this.uiPanel23.TabIndex = 66;
            this.uiPanel23.Text = "Loại QR Chai";
            this.uiPanel23.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel24
            // 
            this.uiPanel24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel24.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel24.Location = new System.Drawing.Point(0, 0);
            this.uiPanel24.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel24.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel24.Name = "uiPanel24";
            this.uiPanel24.Size = new System.Drawing.Size(137, 33);
            this.uiPanel24.TabIndex = 56;
            this.uiPanel24.Text = "Số mã thùng còn";
            this.uiPanel24.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel22
            // 
            this.uiPanel22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel22.FillColor = System.Drawing.Color.White;
            this.uiPanel22.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel22.Location = new System.Drawing.Point(143, 113);
            this.uiPanel22.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel22.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel22.Name = "uiPanel22";
            this.uiPanel22.Size = new System.Drawing.Size(183, 33);
            this.uiPanel22.TabIndex = 65;
            this.uiPanel22.Text = "-";
            this.uiPanel22.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel13
            // 
            this.uiPanel13.Controls.Add(this.opPallet_QR_Status);
            this.uiPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel13.Location = new System.Drawing.Point(143, 76);
            this.uiPanel13.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel13.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel13.Name = "uiPanel13";
            this.uiPanel13.Size = new System.Drawing.Size(183, 33);
            this.uiPanel13.TabIndex = 64;
            this.uiPanel13.Text = "Loại QR Chai";
            this.uiPanel13.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opPallet_QR_Status
            // 
            this.opPallet_QR_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opPallet_QR_Status.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opPallet_QR_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opPallet_QR_Status.Location = new System.Drawing.Point(0, 0);
            this.opPallet_QR_Status.Margin = new System.Windows.Forms.Padding(2);
            this.opPallet_QR_Status.MinimumSize = new System.Drawing.Size(1, 1);
            this.opPallet_QR_Status.Name = "opPallet_QR_Status";
            this.opPallet_QR_Status.Size = new System.Drawing.Size(183, 33);
            this.opPallet_QR_Status.TabIndex = 56;
            this.opPallet_QR_Status.Text = "Mất kết nối";
            this.opPallet_QR_Status.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel17
            // 
            this.uiPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel17.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel17.Location = new System.Drawing.Point(2, 39);
            this.uiPanel17.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel17.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel17.Name = "uiPanel17";
            this.uiPanel17.Size = new System.Drawing.Size(137, 33);
            this.uiPanel17.TabIndex = 63;
            this.uiPanel17.Text = "Máy in thùng";
            this.uiPanel17.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opCase_QR_Status
            // 
            this.opCase_QR_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opCase_QR_Status.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opCase_QR_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opCase_QR_Status.Location = new System.Drawing.Point(143, 39);
            this.opCase_QR_Status.Margin = new System.Windows.Forms.Padding(2);
            this.opCase_QR_Status.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCase_QR_Status.Name = "opCase_QR_Status";
            this.opCase_QR_Status.Size = new System.Drawing.Size(183, 33);
            this.opCase_QR_Status.TabIndex = 62;
            this.opCase_QR_Status.Text = "Mất kết nối";
            this.opCase_QR_Status.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opProduct_QR_Status
            // 
            this.opProduct_QR_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opProduct_QR_Status.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opProduct_QR_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opProduct_QR_Status.Location = new System.Drawing.Point(143, 2);
            this.opProduct_QR_Status.Margin = new System.Windows.Forms.Padding(2);
            this.opProduct_QR_Status.MinimumSize = new System.Drawing.Size(1, 1);
            this.opProduct_QR_Status.Name = "opProduct_QR_Status";
            this.opProduct_QR_Status.Size = new System.Drawing.Size(183, 33);
            this.opProduct_QR_Status.TabIndex = 61;
            this.opProduct_QR_Status.Text = "Mất kết nối";
            this.opProduct_QR_Status.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel12
            // 
            this.uiPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel12.Location = new System.Drawing.Point(2, 2);
            this.uiPanel12.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel12.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel12.Name = "uiPanel12";
            this.uiPanel12.Size = new System.Drawing.Size(137, 33);
            this.uiPanel12.TabIndex = 60;
            this.uiPanel12.Text = "Máy QR chai";
            this.uiPanel12.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAllStatus
            // 
            this.lblAllStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAllStatus.FillColor = System.Drawing.Color.White;
            this.lblAllStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblAllStatus.Location = new System.Drawing.Point(143, 224);
            this.lblAllStatus.Margin = new System.Windows.Forms.Padding(2);
            this.lblAllStatus.MinimumSize = new System.Drawing.Size(1, 1);
            this.lblAllStatus.Name = "lblAllStatus";
            this.lblAllStatus.Size = new System.Drawing.Size(183, 35);
            this.lblAllStatus.TabIndex = 59;
            this.lblAllStatus.Text = "-";
            this.lblAllStatus.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel18
            // 
            this.uiPanel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel18.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel18.Location = new System.Drawing.Point(2, 224);
            this.uiPanel18.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel18.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel18.Name = "uiPanel18";
            this.uiPanel18.Size = new System.Drawing.Size(137, 35);
            this.uiPanel18.TabIndex = 58;
            this.uiPanel18.Text = "Tất cả";
            this.uiPanel18.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel16
            // 
            this.uiPanel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel16.FillColor = System.Drawing.SystemColors.ButtonHighlight;
            this.uiPanel16.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel16.Location = new System.Drawing.Point(143, 187);
            this.uiPanel16.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel16.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel16.Name = "uiPanel16";
            this.uiPanel16.Size = new System.Drawing.Size(183, 33);
            this.uiPanel16.TabIndex = 57;
            this.uiPanel16.Text = "-";
            this.uiPanel16.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel4
            // 
            this.uiPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel4.Location = new System.Drawing.Point(2, 76);
            this.uiPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel4.Name = "uiPanel4";
            this.uiPanel4.Size = new System.Drawing.Size(137, 33);
            this.uiPanel4.TabIndex = 49;
            this.uiPanel4.Text = "Dữ liệu Pallet";
            this.uiPanel4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel11
            // 
            this.uiPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel11.Location = new System.Drawing.Point(2, 113);
            this.uiPanel11.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel11.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel11.Name = "uiPanel11";
            this.uiPanel11.Size = new System.Drawing.Size(137, 33);
            this.uiPanel11.TabIndex = 48;
            this.uiPanel11.Text = "Số mã chai còn";
            this.uiPanel11.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.BackColor = System.Drawing.Color.LightBlue;
            this.uiTableLayoutPanel3.ColumnCount = 2;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.1848F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.81519F));
            this.uiTableLayoutPanel3.Controls.Add(this.uiTableLayoutPanel5, 1, 6);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel3, 0, 6);
            this.uiTableLayoutPanel3.Controls.Add(this.ipLOT, 1, 3);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel20, 0, 5);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel10, 0, 4);
            this.uiTableLayoutPanel3.Controls.Add(this.uiTableLayoutPanel4, 1, 5);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel9, 0, 3);
            this.uiTableLayoutPanel3.Controls.Add(this.uiTableLayoutPanel17, 1, 1);
            this.uiTableLayoutPanel3.Controls.Add(this.ipSanLuong, 1, 4);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel6, 0, 1);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel7, 0, 2);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel5, 0, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.uiTableLayoutPanel8, 1, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.uiTableLayoutPanel6, 1, 2);
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 7;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28431F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28921F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28921F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(485, 261);
            this.uiTableLayoutPanel3.TabIndex = 12;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // uiTableLayoutPanel5
            // 
            this.uiTableLayoutPanel5.ColumnCount = 3;
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 111F));
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.uiTableLayoutPanel5.Controls.Add(this.ipPallerQRType, 2, 0);
            this.uiTableLayoutPanel5.Controls.Add(this.ipPallet_Size, 0, 0);
            this.uiTableLayoutPanel5.Controls.Add(this.uiPanel2, 1, 0);
            this.uiTableLayoutPanel5.Location = new System.Drawing.Point(148, 224);
            this.uiTableLayoutPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel5.Name = "uiTableLayoutPanel5";
            this.uiTableLayoutPanel5.RowCount = 1;
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel5.Size = new System.Drawing.Size(335, 35);
            this.uiTableLayoutPanel5.TabIndex = 60;
            this.uiTableLayoutPanel5.TagString = null;
            // 
            // ipPallerQRType
            // 
            this.ipPallerQRType.DataSource = null;
            this.ipPallerQRType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipPallerQRType.FillColor = System.Drawing.Color.White;
            this.ipPallerQRType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipPallerQRType.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipPallerQRType.Items.AddRange(new object[] {
            "Tăng dần"});
            this.ipPallerQRType.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipPallerQRType.Location = new System.Drawing.Point(206, 2);
            this.ipPallerQRType.Margin = new System.Windows.Forms.Padding(2);
            this.ipPallerQRType.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipPallerQRType.Name = "ipPallerQRType";
            this.ipPallerQRType.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipPallerQRType.Size = new System.Drawing.Size(127, 31);
            this.ipPallerQRType.SymbolSize = 24;
            this.ipPallerQRType.TabIndex = 62;
            this.ipPallerQRType.Text = "Ngẫu nhiên";
            this.ipPallerQRType.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ipPallerQRType.Watermark = "";
            // 
            // ipPallet_Size
            // 
            this.ipPallet_Size.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipPallet_Size.FillColor = System.Drawing.Color.White;
            this.ipPallet_Size.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipPallet_Size.Location = new System.Drawing.Point(2, 2);
            this.ipPallet_Size.Margin = new System.Windows.Forms.Padding(2);
            this.ipPallet_Size.Minimum = 0D;
            this.ipPallet_Size.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipPallet_Size.Name = "ipPallet_Size";
            this.ipPallet_Size.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipPallet_Size.Size = new System.Drawing.Size(89, 31);
            this.ipPallet_Size.SymbolDropDown = 557532;
            this.ipPallet_Size.SymbolNormal = 557532;
            this.ipPallet_Size.SymbolSize = 24;
            this.ipPallet_Size.TabIndex = 61;
            this.ipPallet_Size.Text = "-";
            this.ipPallet_Size.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipPallet_Size.Watermark = "";
            // 
            // uiPanel2
            // 
            this.uiPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel2.Location = new System.Drawing.Point(95, 2);
            this.uiPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel2.Name = "uiPanel2";
            this.uiPanel2.Size = new System.Drawing.Size(107, 31);
            this.uiPanel2.TabIndex = 47;
            this.uiPanel2.Text = "Loại mã Pallet";
            this.uiPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel3
            // 
            this.uiPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel3.Location = new System.Drawing.Point(2, 224);
            this.uiPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel3.Name = "uiPanel3";
            this.uiPanel3.Size = new System.Drawing.Size(142, 35);
            this.uiPanel3.TabIndex = 57;
            this.uiPanel3.Text = "Thùng trên pallet";
            this.uiPanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipLOT
            // 
            this.ipLOT.DateFormat = "dd-MM-yyyy";
            this.ipLOT.DateYearFormat = "yy";
            this.ipLOT.DateYearMonthFormat = "MM-yyyy";
            this.ipLOT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipLOT.FillColor = System.Drawing.Color.White;
            this.ipLOT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipLOT.Location = new System.Drawing.Point(148, 113);
            this.ipLOT.Margin = new System.Windows.Forms.Padding(2);
            this.ipLOT.MaxLength = 10;
            this.ipLOT.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipLOT.Name = "ipLOT";
            this.ipLOT.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipLOT.Size = new System.Drawing.Size(335, 33);
            this.ipLOT.SymbolDropDown = 61555;
            this.ipLOT.SymbolNormal = 61555;
            this.ipLOT.SymbolSize = 24;
            this.ipLOT.TabIndex = 59;
            this.ipLOT.Text = "20-01-2025";
            this.ipLOT.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipLOT.Value = new System.DateTime(2025, 1, 20, 0, 0, 0, 0);
            this.ipLOT.Watermark = "";
            // 
            // uiPanel20
            // 
            this.uiPanel20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel20.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel20.Location = new System.Drawing.Point(2, 187);
            this.uiPanel20.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel20.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel20.Name = "uiPanel20";
            this.uiPanel20.Size = new System.Drawing.Size(142, 33);
            this.uiPanel20.TabIndex = 55;
            this.uiPanel20.Text = "Chai trong block";
            this.uiPanel20.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel10
            // 
            this.uiPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel10.Location = new System.Drawing.Point(2, 150);
            this.uiPanel10.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel10.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel10.Name = "uiPanel10";
            this.uiPanel10.Size = new System.Drawing.Size(142, 33);
            this.uiPanel10.TabIndex = 53;
            this.uiPanel10.Text = "Sản lượng";
            this.uiPanel10.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel4
            // 
            this.uiTableLayoutPanel4.ColumnCount = 3;
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 111F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.uiTableLayoutPanel4.Controls.Add(this.ipCase_Size, 2, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.ipBlock_Size, 0, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.uiPanel27, 1, 0);
            this.uiTableLayoutPanel4.Location = new System.Drawing.Point(148, 187);
            this.uiTableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            this.uiTableLayoutPanel4.RowCount = 1;
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel4.Size = new System.Drawing.Size(335, 33);
            this.uiTableLayoutPanel4.TabIndex = 56;
            this.uiTableLayoutPanel4.TagString = null;
            // 
            // ipCase_Size
            // 
            this.ipCase_Size.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipCase_Size.FillColor = System.Drawing.Color.White;
            this.ipCase_Size.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipCase_Size.Location = new System.Drawing.Point(206, 2);
            this.ipCase_Size.Margin = new System.Windows.Forms.Padding(2);
            this.ipCase_Size.Minimum = 0D;
            this.ipCase_Size.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipCase_Size.Name = "ipCase_Size";
            this.ipCase_Size.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipCase_Size.Size = new System.Drawing.Size(127, 29);
            this.ipCase_Size.SymbolDropDown = 557532;
            this.ipCase_Size.SymbolNormal = 557532;
            this.ipCase_Size.SymbolSize = 24;
            this.ipCase_Size.TabIndex = 50;
            this.ipCase_Size.Text = "-";
            this.ipCase_Size.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipCase_Size.Watermark = "";
            // 
            // ipBlock_Size
            // 
            this.ipBlock_Size.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipBlock_Size.FillColor = System.Drawing.Color.White;
            this.ipBlock_Size.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipBlock_Size.Location = new System.Drawing.Point(2, 2);
            this.ipBlock_Size.Margin = new System.Windows.Forms.Padding(2);
            this.ipBlock_Size.Minimum = 0D;
            this.ipBlock_Size.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipBlock_Size.Name = "ipBlock_Size";
            this.ipBlock_Size.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipBlock_Size.Size = new System.Drawing.Size(89, 29);
            this.ipBlock_Size.SymbolDropDown = 557532;
            this.ipBlock_Size.SymbolNormal = 557532;
            this.ipBlock_Size.SymbolSize = 24;
            this.ipBlock_Size.TabIndex = 49;
            this.ipBlock_Size.Text = "-";
            this.ipBlock_Size.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipBlock_Size.Watermark = "";
            // 
            // uiPanel27
            // 
            this.uiPanel27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel27.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel27.Location = new System.Drawing.Point(95, 2);
            this.uiPanel27.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel27.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel27.Name = "uiPanel27";
            this.uiPanel27.Size = new System.Drawing.Size(107, 29);
            this.uiPanel27.TabIndex = 47;
            this.uiPanel27.Text = "Block/thùng";
            this.uiPanel27.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel9
            // 
            this.uiPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel9.Location = new System.Drawing.Point(2, 113);
            this.uiPanel9.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel9.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel9.Name = "uiPanel9";
            this.uiPanel9.Size = new System.Drawing.Size(142, 33);
            this.uiPanel9.TabIndex = 58;
            this.uiPanel9.Text = "Ngày sản xuất";
            this.uiPanel9.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipSanLuong
            // 
            this.ipSanLuong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipSanLuong.FillColor = System.Drawing.Color.White;
            this.ipSanLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipSanLuong.Location = new System.Drawing.Point(148, 150);
            this.ipSanLuong.Margin = new System.Windows.Forms.Padding(2);
            this.ipSanLuong.Minimum = 0D;
            this.ipSanLuong.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipSanLuong.Name = "ipSanLuong";
            this.ipSanLuong.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipSanLuong.Size = new System.Drawing.Size(335, 33);
            this.ipSanLuong.SymbolDropDown = 557532;
            this.ipSanLuong.SymbolNormal = 557532;
            this.ipSanLuong.SymbolSize = 24;
            this.ipSanLuong.TabIndex = 54;
            this.ipSanLuong.Text = "-";
            this.ipSanLuong.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipSanLuong.Watermark = "";
            // 
            // uiPanel6
            // 
            this.uiPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel6.Location = new System.Drawing.Point(2, 39);
            this.uiPanel6.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel6.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel6.Name = "uiPanel6";
            this.uiPanel6.Size = new System.Drawing.Size(142, 33);
            this.uiPanel6.TabIndex = 33;
            this.uiPanel6.Text = "Barcode thùng";
            this.uiPanel6.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel7
            // 
            this.uiPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel7.Location = new System.Drawing.Point(2, 76);
            this.uiPanel7.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel7.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel7.Name = "uiPanel7";
            this.uiPanel7.Size = new System.Drawing.Size(142, 33);
            this.uiPanel7.TabIndex = 35;
            this.uiPanel7.Text = "Số lô";
            this.uiPanel7.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel5
            // 
            this.uiPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel5.Location = new System.Drawing.Point(2, 2);
            this.uiPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel5.Name = "uiPanel5";
            this.uiPanel5.Size = new System.Drawing.Size(142, 33);
            this.uiPanel5.TabIndex = 31;
            this.uiPanel5.Text = "Barcode chai";
            this.uiPanel5.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel8
            // 
            this.uiTableLayoutPanel8.ColumnCount = 2;
            this.uiTableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.58347F));
            this.uiTableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.41653F));
            this.uiTableLayoutPanel8.Controls.Add(this.ipProductBarcode, 0, 0);
            this.uiTableLayoutPanel8.Controls.Add(this.btnScanBarcode, 1, 0);
            this.uiTableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel8.Location = new System.Drawing.Point(148, 0);
            this.uiTableLayoutPanel8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.uiTableLayoutPanel8.Name = "uiTableLayoutPanel8";
            this.uiTableLayoutPanel8.RowCount = 1;
            this.uiTableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel8.Size = new System.Drawing.Size(335, 37);
            this.uiTableLayoutPanel8.TabIndex = 32;
            this.uiTableLayoutPanel8.TagString = null;
            // 
            // ipProductBarcode
            // 
            this.ipProductBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipProductBarcode.FillColor = System.Drawing.Color.Yellow;
            this.ipProductBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipProductBarcode.Location = new System.Drawing.Point(2, 2);
            this.ipProductBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.ipProductBarcode.Minimum = 0D;
            this.ipProductBarcode.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipProductBarcode.Name = "ipProductBarcode";
            this.ipProductBarcode.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipProductBarcode.Size = new System.Drawing.Size(259, 33);
            this.ipProductBarcode.SymbolDropDown = 557532;
            this.ipProductBarcode.SymbolNormal = 557532;
            this.ipProductBarcode.SymbolSize = 24;
            this.ipProductBarcode.TabIndex = 18;
            this.ipProductBarcode.Text = "-";
            this.ipProductBarcode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipProductBarcode.Watermark = "";
            // 
            // btnScanBarcode
            // 
            this.btnScanBarcode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnScanBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnScanBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnScanBarcode.Location = new System.Drawing.Point(266, 3);
            this.btnScanBarcode.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnScanBarcode.Name = "btnScanBarcode";
            this.btnScanBarcode.Size = new System.Drawing.Size(66, 31);
            this.btnScanBarcode.Symbol = 61482;
            this.btnScanBarcode.TabIndex = 19;
            this.btnScanBarcode.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnScanBarcode.Click += new System.EventHandler(this.btnScanBarcode_Click);
            // 
            // uiTableLayoutPanel6
            // 
            this.uiTableLayoutPanel6.ColumnCount = 2;
            this.uiTableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.67868F));
            this.uiTableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.32132F));
            this.uiTableLayoutPanel6.Controls.Add(this.btnEnterBatch, 1, 0);
            this.uiTableLayoutPanel6.Controls.Add(this.ipBatchCode, 0, 0);
            this.uiTableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel6.Location = new System.Drawing.Point(148, 74);
            this.uiTableLayoutPanel6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.uiTableLayoutPanel6.Name = "uiTableLayoutPanel6";
            this.uiTableLayoutPanel6.RowCount = 1;
            this.uiTableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel6.Size = new System.Drawing.Size(335, 37);
            this.uiTableLayoutPanel6.TabIndex = 61;
            this.uiTableLayoutPanel6.TagString = null;
            // 
            // btnEnterBatch
            // 
            this.btnEnterBatch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnterBatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEnterBatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnEnterBatch.Location = new System.Drawing.Point(266, 3);
            this.btnEnterBatch.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnEnterBatch.Name = "btnEnterBatch";
            this.btnEnterBatch.Size = new System.Drawing.Size(66, 31);
            this.btnEnterBatch.Symbol = 362236;
            this.btnEnterBatch.TabIndex = 45;
            this.btnEnterBatch.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnEnterBatch.Click += new System.EventHandler(this.btnEnterBatch_Click);
            // 
            // ipBatchCode
            // 
            this.ipBatchCode.DataSource = null;
            this.ipBatchCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipBatchCode.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.ipBatchCode.FillColor = System.Drawing.Color.White;
            this.ipBatchCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipBatchCode.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipBatchCode.Items.AddRange(new object[] {
            "TO-123-123"});
            this.ipBatchCode.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipBatchCode.Location = new System.Drawing.Point(2, 2);
            this.ipBatchCode.Margin = new System.Windows.Forms.Padding(2);
            this.ipBatchCode.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipBatchCode.Name = "ipBatchCode";
            this.ipBatchCode.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipBatchCode.Size = new System.Drawing.Size(259, 33);
            this.ipBatchCode.SymbolSize = 24;
            this.ipBatchCode.TabIndex = 44;
            this.ipBatchCode.Text = "-";
            this.ipBatchCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipBatchCode.Watermark = "";
            this.ipBatchCode.SelectedIndexChanged += new System.EventHandler(this.ipBatchCode_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.uiPanel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ipConsole, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 379);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.7037F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86.2963F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(830, 270);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // uiPanel1
            // 
            this.uiPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel1.Location = new System.Drawing.Point(2, 2);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Size = new System.Drawing.Size(826, 32);
            this.uiPanel1.TabIndex = 36;
            this.uiPanel1.Text = "Bảng thông tin thông báo, lưu ý đây là bảng cho máy chủ không phải phần mềm";
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipConsole
            // 
            this.ipConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipConsole.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipConsole.ItemSelectForeColor = System.Drawing.Color.White;
            this.ipConsole.Location = new System.Drawing.Point(2, 38);
            this.ipConsole.Margin = new System.Windows.Forms.Padding(2);
            this.ipConsole.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipConsole.Name = "ipConsole";
            this.ipConsole.Padding = new System.Windows.Forms.Padding(2);
            this.ipConsole.ShowText = false;
            this.ipConsole.Size = new System.Drawing.Size(826, 229);
            this.ipConsole.TabIndex = 6;
            this.ipConsole.Text = null;
            // 
            // WK_MFI
            // 
            this.WK_MFI.WorkerReportsProgress = true;
            this.WK_MFI.WorkerSupportsCancellation = true;
            this.WK_MFI.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_MFI_DoWork);
            // 
            // WK_Server_Status
            // 
            this.WK_Server_Status.WorkerReportsProgress = true;
            this.WK_Server_Status.WorkerSupportsCancellation = true;
            this.WK_Server_Status.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_Server_Status_DoWork);
            // 
            // WK_LoadCloud
            // 
            this.WK_LoadCloud.WorkerReportsProgress = true;
            this.WK_LoadCloud.WorkerSupportsCancellation = true;
            this.WK_LoadCloud.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_LoadCloud_DoWork);
            this.WK_LoadCloud.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WK_LoadCloud_RunWorkerCompleted);
            // 
            // googleService1
            // 
            this.googleService1.contentType = null;
            this.googleService1.credentialFile = "C:\\A\\sales-268504-20a4b06ea0fb.json";
            this.googleService1.DataSheetID = "FactoryIntegration";
            this.googleService1.ObjectName = null;
            this.googleService1.projectName = "sales-268504";
            this.googleService1.TableID = "BatchProduction";
            this.googleService1.Upload_filePatch = null;
            // 
            // btnCaseBarcode
            // 
            this.btnCaseBarcode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCaseBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCaseBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCaseBarcode.Location = new System.Drawing.Point(266, 3);
            this.btnCaseBarcode.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCaseBarcode.Name = "btnCaseBarcode";
            this.btnCaseBarcode.Size = new System.Drawing.Size(66, 31);
            this.btnCaseBarcode.Symbol = 61482;
            this.btnCaseBarcode.TabIndex = 19;
            this.btnCaseBarcode.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCaseBarcode.Click += new System.EventHandler(this.btnCaseBarcode_Click);
            // 
            // uiTableLayoutPanel17
            // 
            this.uiTableLayoutPanel17.ColumnCount = 2;
            this.uiTableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.66666F));
            this.uiTableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.33333F));
            this.uiTableLayoutPanel17.Controls.Add(this.ipCaseBarcode, 0, 0);
            this.uiTableLayoutPanel17.Controls.Add(this.btnCaseBarcode, 1, 0);
            this.uiTableLayoutPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel17.Location = new System.Drawing.Point(148, 37);
            this.uiTableLayoutPanel17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.uiTableLayoutPanel17.Name = "uiTableLayoutPanel17";
            this.uiTableLayoutPanel17.RowCount = 1;
            this.uiTableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel17.Size = new System.Drawing.Size(335, 37);
            this.uiTableLayoutPanel17.TabIndex = 41;
            this.uiTableLayoutPanel17.TagString = null;
            // 
            // ipCaseBarcode
            // 
            this.ipCaseBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipCaseBarcode.FillColor = System.Drawing.Color.White;
            this.ipCaseBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipCaseBarcode.Location = new System.Drawing.Point(2, 2);
            this.ipCaseBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.ipCaseBarcode.Minimum = 0D;
            this.ipCaseBarcode.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipCaseBarcode.Name = "ipCaseBarcode";
            this.ipCaseBarcode.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipCaseBarcode.Size = new System.Drawing.Size(259, 33);
            this.ipCaseBarcode.SymbolDropDown = 557532;
            this.ipCaseBarcode.SymbolNormal = 557532;
            this.ipCaseBarcode.SymbolSize = 24;
            this.ipCaseBarcode.TabIndex = 18;
            this.ipCaseBarcode.Text = "-";
            this.ipCaseBarcode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipCaseBarcode.Watermark = "";
            // 
            // F2MFI
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(836, 652);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximumSize = new System.Drawing.Size(836, 652);
            this.MinimumSize = new System.Drawing.Size(836, 652);
            this.Name = "F2MFI";
            this.Symbol = 61672;
            this.Text = "Sản Xuất";
            this.Load += new System.EventHandler(this.FCasePrinter_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel14.ResumeLayout(false);
            this.uiPanel23.ResumeLayout(false);
            this.uiPanel13.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.uiTableLayoutPanel5.ResumeLayout(false);
            this.uiTableLayoutPanel4.ResumeLayout(false);
            this.uiTableLayoutPanel8.ResumeLayout(false);
            this.uiTableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel17.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker WK_Update;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UIPanel uiPanel8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Sunny.UI.UISymbolButton btnCloudMS;
        private Sunny.UI.UISymbolButton btnUndo;
        private Sunny.UI.UISymbolButton btnLoad_Code;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel14;
        private Sunny.UI.UIPanel uiPanel26;
        private Sunny.UI.UIPanel uiPanel25;
        private Sunny.UI.UIPanel uiPanel23;
        private Sunny.UI.UIPanel uiPanel24;
        private Sunny.UI.UIPanel uiPanel22;
        private Sunny.UI.UIPanel uiPanel13;
        private Sunny.UI.UIPanel opPallet_QR_Status;
        private Sunny.UI.UIPanel uiPanel17;
        private Sunny.UI.UIPanel opCase_QR_Status;
        private Sunny.UI.UIPanel opProduct_QR_Status;
        private Sunny.UI.UIPanel uiPanel12;
        private Sunny.UI.UIPanel lblAllStatus;
        private Sunny.UI.UIPanel uiPanel18;
        private Sunny.UI.UIPanel uiPanel16;
        private Sunny.UI.UIPanel uiPanel4;
        private Sunny.UI.UIPanel uiPanel11;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UIPanel uiPanel6;
        private Sunny.UI.UIPanel uiPanel7;
        private Sunny.UI.UIPanel uiPanel5;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel8;
        private Sunny.UI.UINumPadTextBox ipProductBarcode;
        private Sunny.UI.UISymbolButton btnScanBarcode;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel5;
        private Sunny.UI.UIComboBox ipPallerQRType;
        private Sunny.UI.UINumPadTextBox ipPallet_Size;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UIPanel uiPanel3;
        private Sunny.UI.UIDatePicker ipLOT;
        private Sunny.UI.UIPanel uiPanel20;
        private Sunny.UI.UIPanel uiPanel10;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UINumPadTextBox ipCase_Size;
        private Sunny.UI.UINumPadTextBox ipBlock_Size;
        private Sunny.UI.UIPanel uiPanel27;
        private Sunny.UI.UIPanel uiPanel9;
        private Sunny.UI.UINumPadTextBox ipSanLuong;
        private System.ComponentModel.BackgroundWorker WK_MFI;
        private Sunny.UI.UIPanel lblServerStatus;
        private System.ComponentModel.BackgroundWorker WK_Server_Status;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel6;
        private Sunny.UI.UISymbolButton btnEnterBatch;
        private Sunny.UI.UIComboBox ipBatchCode;
        private SPMS1.GoogleService googleService1;
        private System.ComponentModel.BackgroundWorker WK_LoadCloud;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIListBox ipConsole;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel17;
        private Sunny.UI.UINumPadTextBox ipCaseBarcode;
        private Sunny.UI.UISymbolButton btnCaseBarcode;
    }
}