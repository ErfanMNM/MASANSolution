namespace MFI_Service
{
    partial class MFI_Service_Form
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.ipConsole = new Sunny.UI.UIListBox();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel8 = new Sunny.UI.UIPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnLoadPO = new Sunny.UI.UISymbolButton();
            this.btnCloudMS = new Sunny.UI.UISymbolButton();
            this.btnUndo = new Sunny.UI.UISymbolButton();
            this.btnLoad_Code = new Sunny.UI.UISymbolButton();
            this.lblServerStatus = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel14 = new Sunny.UI.UITableLayoutPanel();
            this.ipOrderNO = new Sunny.UI.UIComboBox();
            this.ipShifss = new Sunny.UI.UIPanel();
            this.ipFactory = new Sunny.UI.UIPanel();
            this.ipShift = new Sunny.UI.UINumPadTextBox();
            this.ipUniqueCode = new Sunny.UI.UIPanel();
            this.uiPanel26 = new Sunny.UI.UIPanel();
            this.ipProductionLine = new Sunny.UI.UIPanel();
            this.uiPanel23 = new Sunny.UI.UIPanel();
            this.uiPanel24 = new Sunny.UI.UIPanel();
            this.ipSite = new Sunny.UI.UIPanel();
            this.uiPanel17 = new Sunny.UI.UIPanel();
            this.uiPanel12 = new Sunny.UI.UIPanel();
            this.ipProductionDate = new Sunny.UI.UIPanel();
            this.uiPanel4 = new Sunny.UI.UIPanel();
            this.uiPanel11 = new Sunny.UI.UIPanel();
            this.uiPanel18 = new Sunny.UI.UIPanel();
            this.ipTotalCode = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.ipLOT = new Sunny.UI.UIDatePicker();
            this.uiTableLayoutPanel5 = new Sunny.UI.UITableLayoutPanel();
            this.opMFIID = new Sunny.UI.UIPanel();
            this.ipPallet_Size = new Sunny.UI.UINumPadTextBox();
            this.uiPanel2 = new Sunny.UI.UIPanel();
            this.uiPanel3 = new Sunny.UI.UIPanel();
            this.uiPanel20 = new Sunny.UI.UIPanel();
            this.uiPanel10 = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            this.ipCase_Size = new Sunny.UI.UINumPadTextBox();
            this.ipBlock_Size = new Sunny.UI.UINumPadTextBox();
            this.uiPanel27 = new Sunny.UI.UIPanel();
            this.uiPanel9 = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel17 = new Sunny.UI.UITableLayoutPanel();
            this.ipCaseBarcode = new Sunny.UI.UINumPadTextBox();
            this.btnCaseBarcode = new Sunny.UI.UISymbolButton();
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
            this.WK_MFI = new System.ComponentModel.BackgroundWorker();
            this.WK_Server_Status = new System.ComponentModel.BackgroundWorker();
            this.WK_LoadCloud = new System.ComponentModel.BackgroundWorker();
            this.googleService1 = new SPMS1.GoogleService(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel14.SuspendLayout();
            this.uiPanel23.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.uiTableLayoutPanel5.SuspendLayout();
            this.uiTableLayoutPanel4.SuspendLayout();
            this.uiTableLayoutPanel17.SuspendLayout();
            this.uiTableLayoutPanel8.SuspendLayout();
            this.uiTableLayoutPanel6.SuspendLayout();
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
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 64.54006F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.45994F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(840, 674);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.uiPanel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ipConsole, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 437);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.87234F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.12766F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(836, 235);
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
            this.uiPanel1.Size = new System.Drawing.Size(832, 38);
            this.uiPanel1.TabIndex = 36;
            this.uiPanel1.Text = "Bảng thông tin thông báo, lưu ý đây là bảng cho máy chủ không phải phần mềm";
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipConsole
            // 
            this.ipConsole.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ipConsole.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipConsole.ItemSelectForeColor = System.Drawing.Color.White;
            this.ipConsole.Location = new System.Drawing.Point(2, 44);
            this.ipConsole.Margin = new System.Windows.Forms.Padding(2);
            this.ipConsole.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipConsole.Name = "ipConsole";
            this.ipConsole.Padding = new System.Windows.Forms.Padding(2);
            this.ipConsole.RectColor = System.Drawing.Color.Blue;
            this.ipConsole.ShowText = false;
            this.ipConsole.Size = new System.Drawing.Size(832, 186);
            this.ipConsole.TabIndex = 6;
            this.ipConsole.Text = null;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
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
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(834, 429);
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
            this.uiPanel8.Size = new System.Drawing.Size(830, 50);
            this.uiPanel8.TabIndex = 4;
            this.uiPanel8.Text = "BẢNG ĐIỀU CHỈNH THÔNG TIN SẢN XUẤT";
            this.uiPanel8.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.04043F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.26685F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.8814F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.53884F));
            this.tableLayoutPanel3.Controls.Add(this.btnLoadPO, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnCloudMS, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnUndo, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnLoad_Code, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblServerStatus, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 377);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(828, 49);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // btnLoadPO
            // 
            this.btnLoadPO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadPO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadPO.Enabled = false;
            this.btnLoadPO.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnLoadPO.Location = new System.Drawing.Point(592, 3);
            this.btnLoadPO.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLoadPO.Name = "btnLoadPO";
            this.btnLoadPO.Size = new System.Drawing.Size(88, 43);
            this.btnLoadPO.Symbol = 362831;
            this.btnLoadPO.TabIndex = 36;
            this.btnLoadPO.Text = "Tải PO";
            this.btnLoadPO.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnCloudMS
            // 
            this.btnCloudMS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCloudMS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCloudMS.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCloudMS.Location = new System.Drawing.Point(686, 3);
            this.btnCloudMS.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCloudMS.Name = "btnCloudMS";
            this.btnCloudMS.Size = new System.Drawing.Size(139, 43);
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
            this.btnUndo.Location = new System.Drawing.Point(490, 3);
            this.btnUndo.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(96, 43);
            this.btnUndo.Symbol = 557410;
            this.btnUndo.TabIndex = 33;
            this.btnUndo.Text = "Nhập lại";
            this.btnUndo.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnLoad_Code
            // 
            this.btnLoad_Code.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad_Code.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoad_Code.Enabled = false;
            this.btnLoad_Code.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnLoad_Code.Location = new System.Drawing.Point(260, 3);
            this.btnLoad_Code.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLoad_Code.Name = "btnLoad_Code";
            this.btnLoad_Code.Size = new System.Drawing.Size(224, 43);
            this.btnLoad_Code.Symbol = 560250;
            this.btnLoad_Code.TabIndex = 28;
            this.btnLoad_Code.Text = "Lưu thông tin sản xuất";
            this.btnLoad_Code.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLoad_Code.Click += new System.EventHandler(this.btnLoad_Code_Click_2);
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServerStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblServerStatus.Location = new System.Drawing.Point(4, 5);
            this.lblServerStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblServerStatus.MinimumSize = new System.Drawing.Size(1, 1);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Radius = 2;
            this.lblServerStatus.Size = new System.Drawing.Size(249, 39);
            this.lblServerStatus.TabIndex = 35;
            this.lblServerStatus.Text = "Chạy sản phẩm PO có mã cho trước";
            this.lblServerStatus.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 2;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.39759F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.60241F));
            this.uiTableLayoutPanel2.Controls.Add(this.uiTableLayoutPanel14, 1, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.uiTableLayoutPanel3, 0, 0);
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(2, 56);
            this.uiTableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(830, 316);
            this.uiTableLayoutPanel2.TabIndex = 7;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // uiTableLayoutPanel14
            // 
            this.uiTableLayoutPanel14.BackColor = System.Drawing.Color.Transparent;
            this.uiTableLayoutPanel14.ColumnCount = 2;
            this.uiTableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.34906F));
            this.uiTableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.65094F));
            this.uiTableLayoutPanel14.Controls.Add(this.ipOrderNO, 1, 0);
            this.uiTableLayoutPanel14.Controls.Add(this.ipShifss, 0, 6);
            this.uiTableLayoutPanel14.Controls.Add(this.ipFactory, 1, 2);
            this.uiTableLayoutPanel14.Controls.Add(this.ipShift, 1, 6);
            this.uiTableLayoutPanel14.Controls.Add(this.ipUniqueCode, 1, 1);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel26, 0, 5);
            this.uiTableLayoutPanel14.Controls.Add(this.ipProductionLine, 1, 4);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel23, 0, 4);
            this.uiTableLayoutPanel14.Controls.Add(this.ipSite, 1, 3);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel17, 0, 1);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel12, 0, 0);
            this.uiTableLayoutPanel14.Controls.Add(this.ipProductionDate, 1, 5);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel4, 0, 2);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel11, 0, 3);
            this.uiTableLayoutPanel14.Controls.Add(this.uiPanel18, 0, 7);
            this.uiTableLayoutPanel14.Controls.Add(this.ipTotalCode, 1, 7);
            this.uiTableLayoutPanel14.Location = new System.Drawing.Point(413, 3);
            this.uiTableLayoutPanel14.Name = "uiTableLayoutPanel14";
            this.uiTableLayoutPanel14.RowCount = 8;
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.uiTableLayoutPanel14.Size = new System.Drawing.Size(414, 310);
            this.uiTableLayoutPanel14.TabIndex = 13;
            this.uiTableLayoutPanel14.TagString = null;
            // 
            // ipOrderNO
            // 
            this.ipOrderNO.DataSource = null;
            this.ipOrderNO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipOrderNO.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.ipOrderNO.FillColor = System.Drawing.Color.White;
            this.ipOrderNO.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipOrderNO.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipOrderNO.Items.AddRange(new object[] {
            "TO-123-123"});
            this.ipOrderNO.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipOrderNO.Location = new System.Drawing.Point(202, 2);
            this.ipOrderNO.Margin = new System.Windows.Forms.Padding(2);
            this.ipOrderNO.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipOrderNO.Name = "ipOrderNO";
            this.ipOrderNO.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipOrderNO.Size = new System.Drawing.Size(210, 34);
            this.ipOrderNO.SymbolSize = 24;
            this.ipOrderNO.TabIndex = 74;
            this.ipOrderNO.Text = "-";
            this.ipOrderNO.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipOrderNO.Watermark = "";
            this.ipOrderNO.SelectedIndexChanged += new System.EventHandler(this.ipOrderNO_SelectedIndexChanged);
            // 
            // ipShifss
            // 
            this.ipShifss.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipShifss.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ipShifss.ForeColor = System.Drawing.Color.Black;
            this.ipShifss.Location = new System.Drawing.Point(2, 230);
            this.ipShifss.Margin = new System.Windows.Forms.Padding(2);
            this.ipShifss.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipShifss.Name = "ipShifss";
            this.ipShifss.RectColor = System.Drawing.Color.MediumBlue;
            this.ipShifss.Size = new System.Drawing.Size(196, 34);
            this.ipShifss.TabIndex = 73;
            this.ipShifss.Text = "Xưởng (Site)";
            this.ipShifss.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipFactory
            // 
            this.ipFactory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipFactory.FillColor = System.Drawing.Color.White;
            this.ipFactory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipFactory.Location = new System.Drawing.Point(202, 78);
            this.ipFactory.Margin = new System.Windows.Forms.Padding(2);
            this.ipFactory.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipFactory.Name = "ipFactory";
            this.ipFactory.Size = new System.Drawing.Size(210, 34);
            this.ipFactory.TabIndex = 71;
            this.ipFactory.Text = "-";
            this.ipFactory.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipShift
            // 
            this.ipShift.FillColor = System.Drawing.Color.White;
            this.ipShift.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipShift.Location = new System.Drawing.Point(202, 230);
            this.ipShift.Margin = new System.Windows.Forms.Padding(2);
            this.ipShift.Minimum = 0D;
            this.ipShift.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipShift.Name = "ipShift";
            this.ipShift.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipShift.Size = new System.Drawing.Size(210, 34);
            this.ipShift.SymbolDropDown = 557532;
            this.ipShift.SymbolNormal = 557532;
            this.ipShift.SymbolSize = 24;
            this.ipShift.TabIndex = 52;
            this.ipShift.Text = "-";
            this.ipShift.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipShift.Watermark = "";
            // 
            // ipUniqueCode
            // 
            this.ipUniqueCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipUniqueCode.FillColor = System.Drawing.Color.White;
            this.ipUniqueCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipUniqueCode.Location = new System.Drawing.Point(202, 40);
            this.ipUniqueCode.Margin = new System.Windows.Forms.Padding(2);
            this.ipUniqueCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipUniqueCode.Name = "ipUniqueCode";
            this.ipUniqueCode.Size = new System.Drawing.Size(210, 34);
            this.ipUniqueCode.TabIndex = 70;
            this.ipUniqueCode.Text = "-";
            this.ipUniqueCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel26
            // 
            this.uiPanel26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel26.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel26.ForeColor = System.Drawing.Color.Black;
            this.uiPanel26.Location = new System.Drawing.Point(2, 192);
            this.uiPanel26.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel26.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel26.Name = "uiPanel26";
            this.uiPanel26.RectColor = System.Drawing.Color.Blue;
            this.uiPanel26.Size = new System.Drawing.Size(196, 34);
            this.uiPanel26.TabIndex = 68;
            this.uiPanel26.Text = "Ngày sản xuất (ProductionDate)";
            this.uiPanel26.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipProductionLine
            // 
            this.ipProductionLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipProductionLine.FillColor = System.Drawing.Color.White;
            this.ipProductionLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipProductionLine.Location = new System.Drawing.Point(202, 154);
            this.ipProductionLine.Margin = new System.Windows.Forms.Padding(2);
            this.ipProductionLine.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipProductionLine.Name = "ipProductionLine";
            this.ipProductionLine.Size = new System.Drawing.Size(210, 34);
            this.ipProductionLine.TabIndex = 67;
            this.ipProductionLine.Text = "-";
            this.ipProductionLine.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel23
            // 
            this.uiPanel23.Controls.Add(this.uiPanel24);
            this.uiPanel23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel23.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel23.Location = new System.Drawing.Point(2, 154);
            this.uiPanel23.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel23.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel23.Name = "uiPanel23";
            this.uiPanel23.Size = new System.Drawing.Size(196, 34);
            this.uiPanel23.TabIndex = 66;
            this.uiPanel23.Text = "Loại QR Chai";
            this.uiPanel23.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel24
            // 
            this.uiPanel24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel24.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel24.ForeColor = System.Drawing.Color.Black;
            this.uiPanel24.Location = new System.Drawing.Point(0, 0);
            this.uiPanel24.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel24.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel24.Name = "uiPanel24";
            this.uiPanel24.RectColor = System.Drawing.Color.Blue;
            this.uiPanel24.Size = new System.Drawing.Size(196, 34);
            this.uiPanel24.TabIndex = 56;
            this.uiPanel24.Text = "Dây chuyền (Production Line)";
            this.uiPanel24.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiPanel24.Click += new System.EventHandler(this.uiPanel24_Click);
            // 
            // ipSite
            // 
            this.ipSite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipSite.FillColor = System.Drawing.Color.White;
            this.ipSite.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipSite.Location = new System.Drawing.Point(202, 116);
            this.ipSite.Margin = new System.Windows.Forms.Padding(2);
            this.ipSite.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipSite.Name = "ipSite";
            this.ipSite.Size = new System.Drawing.Size(210, 34);
            this.ipSite.TabIndex = 65;
            this.ipSite.Text = "-";
            this.ipSite.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel17
            // 
            this.uiPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel17.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel17.ForeColor = System.Drawing.Color.Black;
            this.uiPanel17.Location = new System.Drawing.Point(2, 40);
            this.uiPanel17.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel17.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel17.Name = "uiPanel17";
            this.uiPanel17.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel17.Size = new System.Drawing.Size(196, 34);
            this.uiPanel17.TabIndex = 63;
            this.uiPanel17.Text = "Mã duy nhất (uniqueCode)";
            this.uiPanel17.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel12
            // 
            this.uiPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel12.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel12.ForeColor = System.Drawing.Color.Black;
            this.uiPanel12.Location = new System.Drawing.Point(2, 2);
            this.uiPanel12.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel12.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel12.Name = "uiPanel12";
            this.uiPanel12.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel12.Size = new System.Drawing.Size(196, 34);
            this.uiPanel12.TabIndex = 60;
            this.uiPanel12.Text = "Số yêu cầu (orderNO)";
            this.uiPanel12.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipProductionDate
            // 
            this.ipProductionDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipProductionDate.FillColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ipProductionDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipProductionDate.Location = new System.Drawing.Point(202, 192);
            this.ipProductionDate.Margin = new System.Windows.Forms.Padding(2);
            this.ipProductionDate.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipProductionDate.Name = "ipProductionDate";
            this.ipProductionDate.Size = new System.Drawing.Size(210, 34);
            this.ipProductionDate.TabIndex = 57;
            this.ipProductionDate.Text = "-";
            this.ipProductionDate.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel4
            // 
            this.uiPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel4.ForeColor = System.Drawing.Color.Black;
            this.uiPanel4.Location = new System.Drawing.Point(2, 78);
            this.uiPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel4.Name = "uiPanel4";
            this.uiPanel4.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel4.Size = new System.Drawing.Size(196, 34);
            this.uiPanel4.TabIndex = 49;
            this.uiPanel4.Text = "Nhà máy (Factory)";
            this.uiPanel4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiPanel4.Click += new System.EventHandler(this.uiPanel4_Click);
            // 
            // uiPanel11
            // 
            this.uiPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel11.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel11.ForeColor = System.Drawing.Color.Black;
            this.uiPanel11.Location = new System.Drawing.Point(2, 116);
            this.uiPanel11.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel11.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel11.Name = "uiPanel11";
            this.uiPanel11.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel11.Size = new System.Drawing.Size(196, 34);
            this.uiPanel11.TabIndex = 48;
            this.uiPanel11.Text = "Xưởng (Site)";
            this.uiPanel11.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiPanel11.Click += new System.EventHandler(this.uiPanel11_Click);
            // 
            // uiPanel18
            // 
            this.uiPanel18.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel18.ForeColor = System.Drawing.Color.Black;
            this.uiPanel18.Location = new System.Drawing.Point(2, 268);
            this.uiPanel18.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel18.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel18.Name = "uiPanel18";
            this.uiPanel18.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel18.Size = new System.Drawing.Size(196, 40);
            this.uiPanel18.TabIndex = 72;
            this.uiPanel18.Text = "Tổng số mã sản phẩm";
            this.uiPanel18.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipTotalCode
            // 
            this.ipTotalCode.FillColor = System.Drawing.Color.White;
            this.ipTotalCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipTotalCode.Location = new System.Drawing.Point(202, 268);
            this.ipTotalCode.Margin = new System.Windows.Forms.Padding(2);
            this.ipTotalCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipTotalCode.Name = "ipTotalCode";
            this.ipTotalCode.Size = new System.Drawing.Size(210, 40);
            this.ipTotalCode.TabIndex = 59;
            this.ipTotalCode.Text = "-";
            this.ipTotalCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.BackColor = System.Drawing.Color.Transparent;
            this.uiTableLayoutPanel3.ColumnCount = 2;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.04061F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.95939F));
            this.uiTableLayoutPanel3.Controls.Add(this.ipLOT, 1, 3);
            this.uiTableLayoutPanel3.Controls.Add(this.uiTableLayoutPanel5, 1, 6);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel3, 0, 6);
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
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.44118F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.70588F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(404, 310);
            this.uiTableLayoutPanel3.TabIndex = 12;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // ipLOT
            // 
            this.ipLOT.DateFormat = "dd-MM-yyyy";
            this.ipLOT.DateYearFormat = "yy";
            this.ipLOT.DateYearMonthFormat = "MM-yyyy";
            this.ipLOT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipLOT.FillColor = System.Drawing.Color.White;
            this.ipLOT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipLOT.Location = new System.Drawing.Point(147, 131);
            this.ipLOT.Margin = new System.Windows.Forms.Padding(2);
            this.ipLOT.MaxLength = 10;
            this.ipLOT.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipLOT.Name = "ipLOT";
            this.ipLOT.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipLOT.Size = new System.Drawing.Size(255, 39);
            this.ipLOT.SymbolDropDown = 61555;
            this.ipLOT.SymbolNormal = 61555;
            this.ipLOT.SymbolSize = 24;
            this.ipLOT.TabIndex = 63;
            this.ipLOT.Text = "20-01-2025";
            this.ipLOT.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipLOT.Value = new System.DateTime(2025, 1, 20, 0, 0, 0, 0);
            this.ipLOT.Watermark = "";
            // 
            // uiTableLayoutPanel5
            // 
            this.uiTableLayoutPanel5.ColumnCount = 3;
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
            this.uiTableLayoutPanel5.Controls.Add(this.opMFIID, 2, 0);
            this.uiTableLayoutPanel5.Controls.Add(this.ipPallet_Size, 0, 0);
            this.uiTableLayoutPanel5.Controls.Add(this.uiPanel2, 1, 0);
            this.uiTableLayoutPanel5.Location = new System.Drawing.Point(147, 264);
            this.uiTableLayoutPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel5.Name = "uiTableLayoutPanel5";
            this.uiTableLayoutPanel5.RowCount = 1;
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel5.Size = new System.Drawing.Size(255, 44);
            this.uiTableLayoutPanel5.TabIndex = 60;
            this.uiTableLayoutPanel5.TagString = null;
            // 
            // opMFIID
            // 
            this.opMFIID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opMFIID.Location = new System.Drawing.Point(172, 2);
            this.opMFIID.Margin = new System.Windows.Forms.Padding(2);
            this.opMFIID.MinimumSize = new System.Drawing.Size(1, 1);
            this.opMFIID.Name = "opMFIID";
            this.opMFIID.Size = new System.Drawing.Size(83, 40);
            this.opMFIID.TabIndex = 62;
            this.opMFIID.Text = "kt01";
            this.opMFIID.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.ipPallet_Size.Size = new System.Drawing.Size(89, 40);
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
            this.uiPanel2.Size = new System.Drawing.Size(73, 40);
            this.uiPanel2.TabIndex = 47;
            this.uiPanel2.Text = "Mã phiên";
            this.uiPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel3
            // 
            this.uiPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel3.ForeColor = System.Drawing.Color.Black;
            this.uiPanel3.Location = new System.Drawing.Point(2, 264);
            this.uiPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel3.Name = "uiPanel3";
            this.uiPanel3.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel3.Size = new System.Drawing.Size(141, 44);
            this.uiPanel3.TabIndex = 57;
            this.uiPanel3.Text = "Thùng trên pallet";
            this.uiPanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel20
            // 
            this.uiPanel20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel20.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel20.ForeColor = System.Drawing.Color.Black;
            this.uiPanel20.Location = new System.Drawing.Point(2, 217);
            this.uiPanel20.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel20.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel20.Name = "uiPanel20";
            this.uiPanel20.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel20.Size = new System.Drawing.Size(141, 43);
            this.uiPanel20.TabIndex = 55;
            this.uiPanel20.Text = "Chai trong block";
            this.uiPanel20.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel10
            // 
            this.uiPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel10.ForeColor = System.Drawing.Color.Black;
            this.uiPanel10.Location = new System.Drawing.Point(2, 174);
            this.uiPanel10.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel10.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel10.Name = "uiPanel10";
            this.uiPanel10.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel10.Size = new System.Drawing.Size(141, 39);
            this.uiPanel10.TabIndex = 53;
            this.uiPanel10.Text = "Sản lượng dự toán";
            this.uiPanel10.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel4
            // 
            this.uiTableLayoutPanel4.ColumnCount = 3;
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.uiTableLayoutPanel4.Controls.Add(this.ipCase_Size, 2, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.ipBlock_Size, 0, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.uiPanel27, 1, 0);
            this.uiTableLayoutPanel4.Location = new System.Drawing.Point(147, 217);
            this.uiTableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            this.uiTableLayoutPanel4.RowCount = 1;
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel4.Size = new System.Drawing.Size(255, 43);
            this.uiTableLayoutPanel4.TabIndex = 56;
            this.uiTableLayoutPanel4.TagString = null;
            // 
            // ipCase_Size
            // 
            this.ipCase_Size.FillColor = System.Drawing.Color.White;
            this.ipCase_Size.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipCase_Size.Location = new System.Drawing.Point(158, 2);
            this.ipCase_Size.Margin = new System.Windows.Forms.Padding(2);
            this.ipCase_Size.Minimum = 0D;
            this.ipCase_Size.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipCase_Size.Name = "ipCase_Size";
            this.ipCase_Size.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipCase_Size.Size = new System.Drawing.Size(97, 39);
            this.ipCase_Size.SymbolDropDown = 557532;
            this.ipCase_Size.SymbolNormal = 557532;
            this.ipCase_Size.SymbolSize = 24;
            this.ipCase_Size.TabIndex = 52;
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
            this.ipBlock_Size.Size = new System.Drawing.Size(89, 39);
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
            this.uiPanel27.Size = new System.Drawing.Size(59, 39);
            this.uiPanel27.TabIndex = 47;
            this.uiPanel27.Text = "Block";
            this.uiPanel27.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel9
            // 
            this.uiPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel9.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel9.ForeColor = System.Drawing.Color.Black;
            this.uiPanel9.Location = new System.Drawing.Point(2, 131);
            this.uiPanel9.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel9.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel9.Name = "uiPanel9";
            this.uiPanel9.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel9.Size = new System.Drawing.Size(141, 39);
            this.uiPanel9.TabIndex = 58;
            this.uiPanel9.Text = "Ngày sản xuất";
            this.uiPanel9.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel17
            // 
            this.uiTableLayoutPanel17.ColumnCount = 2;
            this.uiTableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.66666F));
            this.uiTableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.33333F));
            this.uiTableLayoutPanel17.Controls.Add(this.ipCaseBarcode, 0, 0);
            this.uiTableLayoutPanel17.Controls.Add(this.btnCaseBarcode, 1, 0);
            this.uiTableLayoutPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel17.Location = new System.Drawing.Point(147, 43);
            this.uiTableLayoutPanel17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.uiTableLayoutPanel17.Name = "uiTableLayoutPanel17";
            this.uiTableLayoutPanel17.RowCount = 1;
            this.uiTableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel17.Size = new System.Drawing.Size(255, 43);
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
            this.ipCaseBarcode.Size = new System.Drawing.Size(196, 39);
            this.ipCaseBarcode.SymbolDropDown = 557532;
            this.ipCaseBarcode.SymbolNormal = 557532;
            this.ipCaseBarcode.SymbolSize = 24;
            this.ipCaseBarcode.TabIndex = 18;
            this.ipCaseBarcode.Text = "-";
            this.ipCaseBarcode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipCaseBarcode.Watermark = "";
            // 
            // btnCaseBarcode
            // 
            this.btnCaseBarcode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCaseBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCaseBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCaseBarcode.Location = new System.Drawing.Point(203, 3);
            this.btnCaseBarcode.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCaseBarcode.Name = "btnCaseBarcode";
            this.btnCaseBarcode.Size = new System.Drawing.Size(49, 37);
            this.btnCaseBarcode.Symbol = 61482;
            this.btnCaseBarcode.TabIndex = 19;
            this.btnCaseBarcode.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCaseBarcode.Click += new System.EventHandler(this.btnCaseBarcode_Click);
            // 
            // ipSanLuong
            // 
            this.ipSanLuong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipSanLuong.FillColor = System.Drawing.Color.White;
            this.ipSanLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipSanLuong.Location = new System.Drawing.Point(147, 174);
            this.ipSanLuong.Margin = new System.Windows.Forms.Padding(2);
            this.ipSanLuong.Minimum = 0D;
            this.ipSanLuong.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipSanLuong.Name = "ipSanLuong";
            this.ipSanLuong.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipSanLuong.Size = new System.Drawing.Size(255, 39);
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
            this.uiPanel6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel6.ForeColor = System.Drawing.Color.Black;
            this.uiPanel6.Location = new System.Drawing.Point(2, 45);
            this.uiPanel6.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel6.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel6.Name = "uiPanel6";
            this.uiPanel6.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel6.Size = new System.Drawing.Size(141, 39);
            this.uiPanel6.TabIndex = 33;
            this.uiPanel6.Text = "Barcode thùng";
            this.uiPanel6.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel7
            // 
            this.uiPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel7.ForeColor = System.Drawing.Color.Black;
            this.uiPanel7.Location = new System.Drawing.Point(2, 88);
            this.uiPanel7.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel7.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel7.Name = "uiPanel7";
            this.uiPanel7.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel7.Size = new System.Drawing.Size(141, 39);
            this.uiPanel7.TabIndex = 35;
            this.uiPanel7.Text = "Số lô sản xuất";
            this.uiPanel7.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel5
            // 
            this.uiPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPanel5.ForeColor = System.Drawing.Color.Black;
            this.uiPanel5.Location = new System.Drawing.Point(2, 2);
            this.uiPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel5.Name = "uiPanel5";
            this.uiPanel5.RectColor = System.Drawing.Color.MediumBlue;
            this.uiPanel5.Size = new System.Drawing.Size(141, 39);
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
            this.uiTableLayoutPanel8.Location = new System.Drawing.Point(147, 0);
            this.uiTableLayoutPanel8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.uiTableLayoutPanel8.Name = "uiTableLayoutPanel8";
            this.uiTableLayoutPanel8.RowCount = 1;
            this.uiTableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel8.Size = new System.Drawing.Size(255, 43);
            this.uiTableLayoutPanel8.TabIndex = 32;
            this.uiTableLayoutPanel8.TagString = null;
            // 
            // ipProductBarcode
            // 
            this.ipProductBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipProductBarcode.FillColor = System.Drawing.Color.White;
            this.ipProductBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipProductBarcode.Location = new System.Drawing.Point(2, 2);
            this.ipProductBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.ipProductBarcode.Minimum = 0D;
            this.ipProductBarcode.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipProductBarcode.Name = "ipProductBarcode";
            this.ipProductBarcode.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipProductBarcode.Size = new System.Drawing.Size(196, 39);
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
            this.btnScanBarcode.Location = new System.Drawing.Point(203, 3);
            this.btnScanBarcode.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnScanBarcode.Name = "btnScanBarcode";
            this.btnScanBarcode.Size = new System.Drawing.Size(49, 37);
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
            this.uiTableLayoutPanel6.Location = new System.Drawing.Point(147, 86);
            this.uiTableLayoutPanel6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.uiTableLayoutPanel6.Name = "uiTableLayoutPanel6";
            this.uiTableLayoutPanel6.RowCount = 1;
            this.uiTableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel6.Size = new System.Drawing.Size(255, 43);
            this.uiTableLayoutPanel6.TabIndex = 61;
            this.uiTableLayoutPanel6.TagString = null;
            // 
            // btnEnterBatch
            // 
            this.btnEnterBatch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnterBatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEnterBatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnEnterBatch.Location = new System.Drawing.Point(203, 3);
            this.btnEnterBatch.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnEnterBatch.Name = "btnEnterBatch";
            this.btnEnterBatch.Size = new System.Drawing.Size(49, 37);
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
            this.ipBatchCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.ipBatchCode.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipBatchCode.Items.AddRange(new object[] {
            "TO-123-123"});
            this.ipBatchCode.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipBatchCode.Location = new System.Drawing.Point(2, 2);
            this.ipBatchCode.Margin = new System.Windows.Forms.Padding(2);
            this.ipBatchCode.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipBatchCode.Name = "ipBatchCode";
            this.ipBatchCode.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipBatchCode.Size = new System.Drawing.Size(196, 39);
            this.ipBatchCode.SymbolSize = 24;
            this.ipBatchCode.TabIndex = 44;
            this.ipBatchCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipBatchCode.Watermark = "";
            this.ipBatchCode.SelectedIndexChanged += new System.EventHandler(this.ipBatchCode_SelectedIndexChanged);
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
            // MFI_Service_Form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MFI_Service_Form";
            this.Symbol = 61672;
            this.Text = "Sản Xuất";
            this.Load += new System.EventHandler(this.FCasePrinter_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel14.ResumeLayout(false);
            this.uiPanel23.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.uiTableLayoutPanel5.ResumeLayout(false);
            this.uiTableLayoutPanel4.ResumeLayout(false);
            this.uiTableLayoutPanel17.ResumeLayout(false);
            this.uiTableLayoutPanel8.ResumeLayout(false);
            this.uiTableLayoutPanel6.ResumeLayout(false);
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
        private System.ComponentModel.BackgroundWorker WK_MFI;
        private Sunny.UI.UIPanel lblServerStatus;
        private System.ComponentModel.BackgroundWorker WK_Server_Status;
        private SPMS1.GoogleService googleService1;
        private System.ComponentModel.BackgroundWorker WK_LoadCloud;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIListBox ipConsole;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel14;
        private Sunny.UI.UIPanel uiPanel26;
        private Sunny.UI.UIPanel ipProductionLine;
        private Sunny.UI.UIPanel uiPanel23;
        private Sunny.UI.UIPanel uiPanel24;
        private Sunny.UI.UIPanel ipSite;
        private Sunny.UI.UIPanel uiPanel17;
        private Sunny.UI.UIPanel uiPanel12;
        private Sunny.UI.UIPanel ipTotalCode;
        private Sunny.UI.UIPanel ipProductionDate;
        private Sunny.UI.UIPanel uiPanel4;
        private Sunny.UI.UIPanel uiPanel11;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel5;
        private Sunny.UI.UIPanel opMFIID;
        private Sunny.UI.UINumPadTextBox ipPallet_Size;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UIPanel uiPanel3;
        private Sunny.UI.UIPanel uiPanel20;
        private Sunny.UI.UIPanel uiPanel10;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UINumPadTextBox ipBlock_Size;
        private Sunny.UI.UIPanel uiPanel27;
        private Sunny.UI.UIPanel uiPanel9;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel17;
        private Sunny.UI.UINumPadTextBox ipCaseBarcode;
        private Sunny.UI.UISymbolButton btnCaseBarcode;
        private Sunny.UI.UINumPadTextBox ipSanLuong;
        private Sunny.UI.UIPanel uiPanel6;
        private Sunny.UI.UIPanel uiPanel7;
        private Sunny.UI.UIPanel uiPanel5;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel8;
        private Sunny.UI.UINumPadTextBox ipProductBarcode;
        private Sunny.UI.UISymbolButton btnScanBarcode;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel6;
        private Sunny.UI.UISymbolButton btnEnterBatch;
        private Sunny.UI.UIComboBox ipBatchCode;
        private Sunny.UI.UIPanel ipFactory;
        private Sunny.UI.UIPanel ipUniqueCode;
        private Sunny.UI.UINumPadTextBox ipCase_Size;
        private Sunny.UI.UINumPadTextBox ipShift;
        private Sunny.UI.UIPanel uiPanel18;
        private Sunny.UI.UIDatePicker ipLOT;
        private Sunny.UI.UIComboBox ipOrderNO;
        private Sunny.UI.UIPanel ipShifss;
        private Sunny.UI.UISymbolButton btnLoadPO;
    }
}