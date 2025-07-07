namespace QR_MASAN_01.Views.Scada
{
    partial class FSystemlogs
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.WK_AutoLog = new System.ComponentModel.BackgroundWorker();
            this.WK_Getlogs = new System.ComponentModel.BackgroundWorker();
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.opDataG = new Sunny.UI.UIDataGridView();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            this.btnGetAll = new Sunny.UI.UISymbolButton();
            this.ipDateFrom = new Sunny.UI.UIDatePicker();
            this.btnExportCsv = new Sunny.UI.UISymbolButton();
            this.btnExportPDF = new Sunny.UI.UISymbolButton();
            this.btnGetLogs = new Sunny.UI.UISymbolButton();
            this.ipLoginType = new Sunny.UI.UIComboBox();
            this.ipDateTo = new Sunny.UI.UIDatePicker();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.ipSize = new Sunny.UI.UIComboBox();
            this.uiPagination1 = new Sunny.UI.UIPagination();
            this.opTotalCount = new Sunny.UI.UIPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.uiTitlePanel2 = new Sunny.UI.UITitlePanel();
            this.uiTableLayoutPanel5 = new Sunny.UI.UITableLayoutPanel();
            this.uiDataGridView1 = new Sunny.UI.UIDataGridView();
            this.uiTableLayoutPanel6 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel7 = new Sunny.UI.UITableLayoutPanel();
            this.uiSymbolButton1 = new Sunny.UI.UISymbolButton();
            this.uiDatePicker1 = new Sunny.UI.UIDatePicker();
            this.uiSymbolButton2 = new Sunny.UI.UISymbolButton();
            this.uiSymbolButton3 = new Sunny.UI.UISymbolButton();
            this.uiSymbolButton4 = new Sunny.UI.UISymbolButton();
            this.uiComboBox1 = new Sunny.UI.UIComboBox();
            this.uiDatePicker2 = new Sunny.UI.UIDatePicker();
            this.uiTableLayoutPanel8 = new Sunny.UI.UITableLayoutPanel();
            this.uiComboBox2 = new Sunny.UI.UIComboBox();
            this.uiPagination2 = new Sunny.UI.UIPagination();
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.uiTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.uiTitlePanel1.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opDataG)).BeginInit();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel4.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.uiTitlePanel2.SuspendLayout();
            this.uiTableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).BeginInit();
            this.uiTableLayoutPanel6.SuspendLayout();
            this.uiTableLayoutPanel7.SuspendLayout();
            this.uiTableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // WK_AutoLog
            // 
            this.WK_AutoLog.WorkerReportsProgress = true;
            this.WK_AutoLog.WorkerSupportsCancellation = true;
            this.WK_AutoLog.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_AutoLog_DoWork);
            // 
            // WK_Getlogs
            // 
            this.WK_Getlogs.WorkerReportsProgress = true;
            this.WK_Getlogs.WorkerSupportsCancellation = true;
            this.WK_Getlogs.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_Getlogs_DoWork);
            this.WK_Getlogs.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WK_Getlogs_RunWorkerCompleted);
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPage1);
            this.uiTabControl1.Controls.Add(this.tabPage2);
            this.uiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTabControl1.ItemSize = new System.Drawing.Size(150, 40);
            this.uiTabControl1.Location = new System.Drawing.Point(0, 0);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(840, 674);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 0;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.uiTitlePanel1);
            this.tabPage1.Location = new System.Drawing.Point(0, 40);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(840, 634);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Hệ thống";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.uiTableLayoutPanel1);
            this.uiTitlePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 50, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(840, 634);
            this.uiTitlePanel1.TabIndex = 1;
            this.uiTitlePanel1.Text = "QUẢN LÝ LỊCH SỬ HỆ THỐNG";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.TitleHeight = 50;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Controls.Add(this.opDataG, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel2, 0, 1);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(1, 50);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 2;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80.41734F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.58266F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(838, 583);
            this.uiTableLayoutPanel1.TabIndex = 17;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // opDataG
            // 
            this.opDataG.AllowUserToAddRows = false;
            this.opDataG.AllowUserToDeleteRows = false;
            this.opDataG.AllowUserToOrderColumns = true;
            this.opDataG.AllowUserToResizeColumns = false;
            this.opDataG.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.opDataG.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.opDataG.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.opDataG.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.opDataG.BackgroundColor = System.Drawing.Color.White;
            this.opDataG.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.opDataG.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.opDataG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.opDataG.DefaultCellStyle = dataGridViewCellStyle3;
            this.opDataG.EnableHeadersVisualStyles = false;
            this.opDataG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opDataG.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.opDataG.Location = new System.Drawing.Point(3, 3);
            this.opDataG.Name = "opDataG";
            this.opDataG.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.opDataG.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opDataG.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.opDataG.ScrollMode = Sunny.UI.UIDataGridView.UIDataGridViewScrollMode.Page;
            this.opDataG.SelectedIndex = -1;
            this.opDataG.Size = new System.Drawing.Size(832, 462);
            this.opDataG.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.opDataG.TabIndex = 0;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 1;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Controls.Add(this.uiTableLayoutPanel4, 0, 1);
            this.uiTableLayoutPanel2.Controls.Add(this.uiTableLayoutPanel3, 0, 0);
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(3, 471);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 2;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44.82759F));
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.17241F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(832, 109);
            this.uiTableLayoutPanel2.TabIndex = 1;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // uiTableLayoutPanel4
            // 
            this.uiTableLayoutPanel4.ColumnCount = 7;
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 131F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 248F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.uiTableLayoutPanel4.Controls.Add(this.btnGetAll, 6, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.ipDateFrom, 2, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.btnExportCsv, 1, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.btnExportPDF, 0, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.btnGetLogs, 5, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.ipLoginType, 4, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.ipDateTo, 3, 0);
            this.uiTableLayoutPanel4.Location = new System.Drawing.Point(3, 51);
            this.uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            this.uiTableLayoutPanel4.RowCount = 1;
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel4.Size = new System.Drawing.Size(826, 55);
            this.uiTableLayoutPanel4.TabIndex = 3;
            this.uiTableLayoutPanel4.TagString = null;
            // 
            // btnGetAll
            // 
            this.btnGetAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGetAll.FillColor = System.Drawing.Color.Aquamarine;
            this.btnGetAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnGetAll.Location = new System.Drawing.Point(728, 3);
            this.btnGetAll.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnGetAll.Name = "btnGetAll";
            this.btnGetAll.RectColor = System.Drawing.Color.Blue;
            this.btnGetAll.RectSize = 2;
            this.btnGetAll.Size = new System.Drawing.Size(95, 49);
            this.btnGetAll.Symbol = 559775;
            this.btnGetAll.SymbolColor = System.Drawing.Color.MediumBlue;
            this.btnGetAll.SymbolSize = 30;
            this.btnGetAll.TabIndex = 12;
            this.btnGetAll.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnGetAll.Click += new System.EventHandler(this.btnGetAll_Click);
            // 
            // ipDateFrom
            // 
            this.ipDateFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipDateFrom.FillColor = System.Drawing.Color.White;
            this.ipDateFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipDateFrom.Location = new System.Drawing.Point(140, 2);
            this.ipDateFrom.Margin = new System.Windows.Forms.Padding(2);
            this.ipDateFrom.MaxLength = 10;
            this.ipDateFrom.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipDateFrom.Name = "ipDateFrom";
            this.ipDateFrom.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipDateFrom.RectColor = System.Drawing.Color.Blue;
            this.ipDateFrom.RectSize = 2;
            this.ipDateFrom.Size = new System.Drawing.Size(127, 51);
            this.ipDateFrom.SymbolDropDown = 61555;
            this.ipDateFrom.SymbolNormal = 61555;
            this.ipDateFrom.SymbolSize = 24;
            this.ipDateFrom.TabIndex = 11;
            this.ipDateFrom.Text = "2025-06-24";
            this.ipDateFrom.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipDateFrom.Value = new System.DateTime(2025, 6, 24, 0, 0, 0, 0);
            this.ipDateFrom.Watermark = "";
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportCsv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportCsv.FillColor = System.Drawing.Color.WhiteSmoke;
            this.btnExportCsv.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnExportCsv.Location = new System.Drawing.Point(71, 2);
            this.btnExportCsv.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportCsv.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.RectColor = System.Drawing.Color.Blue;
            this.btnExportCsv.RectSize = 2;
            this.btnExportCsv.Size = new System.Drawing.Size(65, 51);
            this.btnExportCsv.Symbol = 363197;
            this.btnExportCsv.SymbolColor = System.Drawing.Color.Green;
            this.btnExportCsv.SymbolSize = 50;
            this.btnExportCsv.TabIndex = 10;
            this.btnExportCsv.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // btnExportPDF
            // 
            this.btnExportPDF.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportPDF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportPDF.FillColor = System.Drawing.Color.WhiteSmoke;
            this.btnExportPDF.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnExportPDF.Location = new System.Drawing.Point(2, 2);
            this.btnExportPDF.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportPDF.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.RectColor = System.Drawing.Color.Blue;
            this.btnExportPDF.RectSize = 2;
            this.btnExportPDF.Size = new System.Drawing.Size(65, 51);
            this.btnExportPDF.Symbol = 261889;
            this.btnExportPDF.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnExportPDF.SymbolSize = 50;
            this.btnExportPDF.TabIndex = 9;
            this.btnExportPDF.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnExportPDF.Click += new System.EventHandler(this.btnExportPDF_Click);
            // 
            // btnGetLogs
            // 
            this.btnGetLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGetLogs.FillColor = System.Drawing.Color.Aquamarine;
            this.btnGetLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnGetLogs.Location = new System.Drawing.Point(645, 3);
            this.btnGetLogs.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnGetLogs.Name = "btnGetLogs";
            this.btnGetLogs.RectColor = System.Drawing.Color.Blue;
            this.btnGetLogs.RectSize = 2;
            this.btnGetLogs.Size = new System.Drawing.Size(77, 49);
            this.btnGetLogs.Symbol = 61473;
            this.btnGetLogs.SymbolColor = System.Drawing.Color.MediumBlue;
            this.btnGetLogs.SymbolSize = 30;
            this.btnGetLogs.TabIndex = 3;
            this.btnGetLogs.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnGetLogs.Click += new System.EventHandler(this.btnGetLogs_Click);
            // 
            // ipLoginType
            // 
            this.ipLoginType.DataSource = null;
            this.ipLoginType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipLoginType.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.ipLoginType.FillColor = System.Drawing.Color.White;
            this.ipLoginType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipLoginType.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipLoginType.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipLoginType.Location = new System.Drawing.Point(396, 2);
            this.ipLoginType.Margin = new System.Windows.Forms.Padding(2);
            this.ipLoginType.MaxDropDownItems = 16;
            this.ipLoginType.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipLoginType.Name = "ipLoginType";
            this.ipLoginType.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipLoginType.RectColor = System.Drawing.Color.Blue;
            this.ipLoginType.RectSize = 2;
            this.ipLoginType.Size = new System.Drawing.Size(244, 51);
            this.ipLoginType.SymbolSize = 24;
            this.ipLoginType.TabIndex = 6;
            this.ipLoginType.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipLoginType.Watermark = "";
            this.ipLoginType.SelectedIndexChanged += new System.EventHandler(this.ipLoginType_SelectedIndexChanged);
            // 
            // ipDateTo
            // 
            this.ipDateTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipDateTo.FillColor = System.Drawing.Color.White;
            this.ipDateTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipDateTo.Location = new System.Drawing.Point(271, 2);
            this.ipDateTo.Margin = new System.Windows.Forms.Padding(2);
            this.ipDateTo.MaxLength = 10;
            this.ipDateTo.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipDateTo.Name = "ipDateTo";
            this.ipDateTo.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipDateTo.RectColor = System.Drawing.Color.Blue;
            this.ipDateTo.RectSize = 2;
            this.ipDateTo.Size = new System.Drawing.Size(121, 51);
            this.ipDateTo.SymbolDropDown = 61555;
            this.ipDateTo.SymbolNormal = 61555;
            this.ipDateTo.SymbolSize = 24;
            this.ipDateTo.TabIndex = 8;
            this.ipDateTo.Text = "2025-06-24";
            this.ipDateTo.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipDateTo.Value = new System.DateTime(2025, 6, 24, 0, 0, 0, 0);
            this.ipDateTo.Watermark = "";
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.ColumnCount = 3;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.uiTableLayoutPanel3.Controls.Add(this.ipSize, 1, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.uiPagination1, 0, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.opTotalCount, 2, 0);
            this.uiTableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 1;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(826, 42);
            this.uiTableLayoutPanel3.TabIndex = 2;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // ipSize
            // 
            this.ipSize.DataSource = null;
            this.ipSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipSize.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.ipSize.FillColor = System.Drawing.Color.White;
            this.ipSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipSize.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipSize.Items.AddRange(new object[] {
            "",
            "5",
            "10",
            "50",
            "100",
            "500",
            "1000"});
            this.ipSize.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipSize.Location = new System.Drawing.Point(605, 5);
            this.ipSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipSize.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipSize.Name = "ipSize";
            this.ipSize.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipSize.Size = new System.Drawing.Size(85, 32);
            this.ipSize.SymbolSize = 24;
            this.ipSize.TabIndex = 7;
            this.ipSize.Text = "50";
            this.ipSize.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipSize.Watermark = "";
            this.ipSize.SelectedIndexChanged += new System.EventHandler(this.ipSize_SelectedIndexChanged_1);
            // 
            // uiPagination1
            // 
            this.uiPagination1.ButtonFillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(204)))));
            this.uiPagination1.ButtonStyleInherited = false;
            this.uiPagination1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPagination1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPagination1.Location = new System.Drawing.Point(4, 5);
            this.uiPagination1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPagination1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPagination1.Name = "uiPagination1";
            this.uiPagination1.PageSize = 2;
            this.uiPagination1.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.uiPagination1.ShowText = false;
            this.uiPagination1.Size = new System.Drawing.Size(593, 32);
            this.uiPagination1.TabIndex = 2;
            this.uiPagination1.Text = "uiPagination1";
            this.uiPagination1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiPagination1.TotalCount = 100;
            this.uiPagination1.PageChanged += new Sunny.UI.UIPagination.OnPageChangeEventHandler(this.uiPagination1_PageChanged);
            // 
            // opTotalCount
            // 
            this.opTotalCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opTotalCount.Location = new System.Drawing.Point(698, 5);
            this.opTotalCount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opTotalCount.MinimumSize = new System.Drawing.Size(1, 1);
            this.opTotalCount.Name = "opTotalCount";
            this.opTotalCount.Size = new System.Drawing.Size(124, 32);
            this.opTotalCount.TabIndex = 8;
            this.opTotalCount.Text = "0";
            this.opTotalCount.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.uiTitlePanel2);
            this.tabPage2.Location = new System.Drawing.Point(0, 40);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(840, 634);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sản xuất";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // uiTitlePanel2
            // 
            this.uiTitlePanel2.Controls.Add(this.uiTableLayoutPanel5);
            this.uiTitlePanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.uiTitlePanel2.Location = new System.Drawing.Point(0, 0);
            this.uiTitlePanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel2.Name = "uiTitlePanel2";
            this.uiTitlePanel2.Padding = new System.Windows.Forms.Padding(1, 50, 1, 1);
            this.uiTitlePanel2.ShowText = false;
            this.uiTitlePanel2.Size = new System.Drawing.Size(840, 634);
            this.uiTitlePanel2.TabIndex = 2;
            this.uiTitlePanel2.Text = "QUẢN LÝ LỊCH SỬ HỆ THỐNG";
            this.uiTitlePanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel2.TitleHeight = 50;
            // 
            // uiTableLayoutPanel5
            // 
            this.uiTableLayoutPanel5.ColumnCount = 1;
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel5.Controls.Add(this.uiDataGridView1, 0, 0);
            this.uiTableLayoutPanel5.Controls.Add(this.uiTableLayoutPanel6, 0, 1);
            this.uiTableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel5.Location = new System.Drawing.Point(1, 50);
            this.uiTableLayoutPanel5.Name = "uiTableLayoutPanel5";
            this.uiTableLayoutPanel5.RowCount = 2;
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80.41734F));
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.58266F));
            this.uiTableLayoutPanel5.Size = new System.Drawing.Size(838, 583);
            this.uiTableLayoutPanel5.TabIndex = 17;
            this.uiTableLayoutPanel5.TagString = null;
            // 
            // uiDataGridView1
            // 
            this.uiDataGridView1.AllowUserToAddRows = false;
            this.uiDataGridView1.AllowUserToDeleteRows = false;
            this.uiDataGridView1.AllowUserToOrderColumns = true;
            this.uiDataGridView1.AllowUserToResizeColumns = false;
            this.uiDataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.uiDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.uiDataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.uiDataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.uiDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
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
            this.uiDataGridView1.EnableHeadersVisualStyles = false;
            this.uiDataGridView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiDataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.Location = new System.Drawing.Point(3, 3);
            this.uiDataGridView1.Name = "uiDataGridView1";
            this.uiDataGridView1.ReadOnly = true;
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
            this.uiDataGridView1.ScrollMode = Sunny.UI.UIDataGridView.UIDataGridViewScrollMode.Page;
            this.uiDataGridView1.SelectedIndex = -1;
            this.uiDataGridView1.Size = new System.Drawing.Size(832, 462);
            this.uiDataGridView1.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.TabIndex = 0;
            // 
            // uiTableLayoutPanel6
            // 
            this.uiTableLayoutPanel6.ColumnCount = 1;
            this.uiTableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel6.Controls.Add(this.uiTableLayoutPanel7, 0, 1);
            this.uiTableLayoutPanel6.Controls.Add(this.uiTableLayoutPanel8, 0, 0);
            this.uiTableLayoutPanel6.Location = new System.Drawing.Point(3, 471);
            this.uiTableLayoutPanel6.Name = "uiTableLayoutPanel6";
            this.uiTableLayoutPanel6.RowCount = 2;
            this.uiTableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44.82759F));
            this.uiTableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.17241F));
            this.uiTableLayoutPanel6.Size = new System.Drawing.Size(832, 109);
            this.uiTableLayoutPanel6.TabIndex = 1;
            this.uiTableLayoutPanel6.TagString = null;
            // 
            // uiTableLayoutPanel7
            // 
            this.uiTableLayoutPanel7.ColumnCount = 7;
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 131F));
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 248F));
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.uiTableLayoutPanel7.Controls.Add(this.uiSymbolButton1, 6, 0);
            this.uiTableLayoutPanel7.Controls.Add(this.uiDatePicker1, 2, 0);
            this.uiTableLayoutPanel7.Controls.Add(this.uiSymbolButton2, 1, 0);
            this.uiTableLayoutPanel7.Controls.Add(this.uiSymbolButton3, 0, 0);
            this.uiTableLayoutPanel7.Controls.Add(this.uiSymbolButton4, 5, 0);
            this.uiTableLayoutPanel7.Controls.Add(this.uiComboBox1, 4, 0);
            this.uiTableLayoutPanel7.Controls.Add(this.uiDatePicker2, 3, 0);
            this.uiTableLayoutPanel7.Location = new System.Drawing.Point(3, 51);
            this.uiTableLayoutPanel7.Name = "uiTableLayoutPanel7";
            this.uiTableLayoutPanel7.RowCount = 1;
            this.uiTableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel7.Size = new System.Drawing.Size(826, 55);
            this.uiTableLayoutPanel7.TabIndex = 3;
            this.uiTableLayoutPanel7.TagString = null;
            // 
            // uiSymbolButton1
            // 
            this.uiSymbolButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton1.FillColor = System.Drawing.Color.Aquamarine;
            this.uiSymbolButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiSymbolButton1.Location = new System.Drawing.Point(728, 3);
            this.uiSymbolButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton1.Name = "uiSymbolButton1";
            this.uiSymbolButton1.RectColor = System.Drawing.Color.Blue;
            this.uiSymbolButton1.RectSize = 2;
            this.uiSymbolButton1.Size = new System.Drawing.Size(95, 49);
            this.uiSymbolButton1.Symbol = 559775;
            this.uiSymbolButton1.SymbolColor = System.Drawing.Color.MediumBlue;
            this.uiSymbolButton1.SymbolSize = 30;
            this.uiSymbolButton1.TabIndex = 12;
            this.uiSymbolButton1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiDatePicker1
            // 
            this.uiDatePicker1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiDatePicker1.FillColor = System.Drawing.Color.White;
            this.uiDatePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiDatePicker1.Location = new System.Drawing.Point(140, 2);
            this.uiDatePicker1.Margin = new System.Windows.Forms.Padding(2);
            this.uiDatePicker1.MaxLength = 10;
            this.uiDatePicker1.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiDatePicker1.Name = "uiDatePicker1";
            this.uiDatePicker1.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiDatePicker1.RectColor = System.Drawing.Color.Blue;
            this.uiDatePicker1.RectSize = 2;
            this.uiDatePicker1.Size = new System.Drawing.Size(127, 51);
            this.uiDatePicker1.SymbolDropDown = 61555;
            this.uiDatePicker1.SymbolNormal = 61555;
            this.uiDatePicker1.SymbolSize = 24;
            this.uiDatePicker1.TabIndex = 11;
            this.uiDatePicker1.Text = "2025-06-24";
            this.uiDatePicker1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiDatePicker1.Value = new System.DateTime(2025, 6, 24, 0, 0, 0, 0);
            this.uiDatePicker1.Watermark = "";
            // 
            // uiSymbolButton2
            // 
            this.uiSymbolButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSymbolButton2.FillColor = System.Drawing.Color.WhiteSmoke;
            this.uiSymbolButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiSymbolButton2.Location = new System.Drawing.Point(71, 2);
            this.uiSymbolButton2.Margin = new System.Windows.Forms.Padding(2);
            this.uiSymbolButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton2.Name = "uiSymbolButton2";
            this.uiSymbolButton2.RectColor = System.Drawing.Color.Blue;
            this.uiSymbolButton2.RectSize = 2;
            this.uiSymbolButton2.Size = new System.Drawing.Size(65, 51);
            this.uiSymbolButton2.Symbol = 363197;
            this.uiSymbolButton2.SymbolColor = System.Drawing.Color.Green;
            this.uiSymbolButton2.SymbolSize = 50;
            this.uiSymbolButton2.TabIndex = 10;
            this.uiSymbolButton2.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiSymbolButton3
            // 
            this.uiSymbolButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSymbolButton3.FillColor = System.Drawing.Color.WhiteSmoke;
            this.uiSymbolButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiSymbolButton3.Location = new System.Drawing.Point(2, 2);
            this.uiSymbolButton3.Margin = new System.Windows.Forms.Padding(2);
            this.uiSymbolButton3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton3.Name = "uiSymbolButton3";
            this.uiSymbolButton3.RectColor = System.Drawing.Color.Blue;
            this.uiSymbolButton3.RectSize = 2;
            this.uiSymbolButton3.Size = new System.Drawing.Size(65, 51);
            this.uiSymbolButton3.Symbol = 261889;
            this.uiSymbolButton3.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.uiSymbolButton3.SymbolSize = 50;
            this.uiSymbolButton3.TabIndex = 9;
            this.uiSymbolButton3.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiSymbolButton4
            // 
            this.uiSymbolButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton4.FillColor = System.Drawing.Color.Aquamarine;
            this.uiSymbolButton4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiSymbolButton4.Location = new System.Drawing.Point(645, 3);
            this.uiSymbolButton4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton4.Name = "uiSymbolButton4";
            this.uiSymbolButton4.RectColor = System.Drawing.Color.Blue;
            this.uiSymbolButton4.RectSize = 2;
            this.uiSymbolButton4.Size = new System.Drawing.Size(77, 49);
            this.uiSymbolButton4.Symbol = 61473;
            this.uiSymbolButton4.SymbolColor = System.Drawing.Color.MediumBlue;
            this.uiSymbolButton4.SymbolSize = 30;
            this.uiSymbolButton4.TabIndex = 3;
            this.uiSymbolButton4.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiComboBox1
            // 
            this.uiComboBox1.DataSource = null;
            this.uiComboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiComboBox1.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.uiComboBox1.FillColor = System.Drawing.Color.White;
            this.uiComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiComboBox1.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiComboBox1.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiComboBox1.Location = new System.Drawing.Point(396, 2);
            this.uiComboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.uiComboBox1.MaxDropDownItems = 16;
            this.uiComboBox1.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiComboBox1.Name = "uiComboBox1";
            this.uiComboBox1.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiComboBox1.RectColor = System.Drawing.Color.Blue;
            this.uiComboBox1.RectSize = 2;
            this.uiComboBox1.Size = new System.Drawing.Size(244, 51);
            this.uiComboBox1.SymbolSize = 24;
            this.uiComboBox1.TabIndex = 6;
            this.uiComboBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiComboBox1.Watermark = "";
            // 
            // uiDatePicker2
            // 
            this.uiDatePicker2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiDatePicker2.FillColor = System.Drawing.Color.White;
            this.uiDatePicker2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiDatePicker2.Location = new System.Drawing.Point(271, 2);
            this.uiDatePicker2.Margin = new System.Windows.Forms.Padding(2);
            this.uiDatePicker2.MaxLength = 10;
            this.uiDatePicker2.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiDatePicker2.Name = "uiDatePicker2";
            this.uiDatePicker2.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiDatePicker2.RectColor = System.Drawing.Color.Blue;
            this.uiDatePicker2.RectSize = 2;
            this.uiDatePicker2.Size = new System.Drawing.Size(121, 51);
            this.uiDatePicker2.SymbolDropDown = 61555;
            this.uiDatePicker2.SymbolNormal = 61555;
            this.uiDatePicker2.SymbolSize = 24;
            this.uiDatePicker2.TabIndex = 8;
            this.uiDatePicker2.Text = "2025-06-24";
            this.uiDatePicker2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiDatePicker2.Value = new System.DateTime(2025, 6, 24, 0, 0, 0, 0);
            this.uiDatePicker2.Watermark = "";
            // 
            // uiTableLayoutPanel8
            // 
            this.uiTableLayoutPanel8.ColumnCount = 3;
            this.uiTableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.uiTableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.uiTableLayoutPanel8.Controls.Add(this.uiComboBox2, 1, 0);
            this.uiTableLayoutPanel8.Controls.Add(this.uiPagination2, 0, 0);
            this.uiTableLayoutPanel8.Controls.Add(this.uiPanel1, 2, 0);
            this.uiTableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel8.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel8.Name = "uiTableLayoutPanel8";
            this.uiTableLayoutPanel8.RowCount = 1;
            this.uiTableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel8.Size = new System.Drawing.Size(826, 42);
            this.uiTableLayoutPanel8.TabIndex = 2;
            this.uiTableLayoutPanel8.TagString = null;
            // 
            // uiComboBox2
            // 
            this.uiComboBox2.DataSource = null;
            this.uiComboBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiComboBox2.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.uiComboBox2.FillColor = System.Drawing.Color.White;
            this.uiComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiComboBox2.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiComboBox2.Items.AddRange(new object[] {
            "",
            "5",
            "10",
            "50",
            "100",
            "500",
            "1000"});
            this.uiComboBox2.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiComboBox2.Location = new System.Drawing.Point(605, 5);
            this.uiComboBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiComboBox2.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiComboBox2.Name = "uiComboBox2";
            this.uiComboBox2.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiComboBox2.Size = new System.Drawing.Size(85, 32);
            this.uiComboBox2.SymbolSize = 24;
            this.uiComboBox2.TabIndex = 7;
            this.uiComboBox2.Text = "50";
            this.uiComboBox2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiComboBox2.Watermark = "";
            // 
            // uiPagination2
            // 
            this.uiPagination2.ButtonFillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(204)))));
            this.uiPagination2.ButtonStyleInherited = false;
            this.uiPagination2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPagination2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPagination2.Location = new System.Drawing.Point(4, 5);
            this.uiPagination2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPagination2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPagination2.Name = "uiPagination2";
            this.uiPagination2.PageSize = 2;
            this.uiPagination2.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.uiPagination2.ShowText = false;
            this.uiPagination2.Size = new System.Drawing.Size(593, 32);
            this.uiPagination2.TabIndex = 2;
            this.uiPagination2.Text = "uiPagination2";
            this.uiPagination2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiPagination2.TotalCount = 100;
            // 
            // uiPanel1
            // 
            this.uiPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel1.Location = new System.Drawing.Point(698, 5);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Size = new System.Drawing.Size(124, 32);
            this.uiPanel1.TabIndex = 8;
            this.uiPanel1.Text = "0";
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FSystemlogs
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiTabControl1);
            this.Name = "FSystemlogs";
            this.Symbol = 57591;
            this.Text = "Truy vết";
            this.Initialize += new System.EventHandler(this.FSystemlogs_Initialize);
            this.uiTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.uiTitlePanel1.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.opDataG)).EndInit();
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel4.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.uiTitlePanel2.ResumeLayout(false);
            this.uiTableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).EndInit();
            this.uiTableLayoutPanel6.ResumeLayout(false);
            this.uiTableLayoutPanel7.ResumeLayout(false);
            this.uiTableLayoutPanel8.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker WK_AutoLog;
        private System.ComponentModel.BackgroundWorker WK_Getlogs;
        private Sunny.UI.UITabControl uiTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UIDataGridView opDataG;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UISymbolButton btnGetAll;
        private Sunny.UI.UIDatePicker ipDateFrom;
        private Sunny.UI.UISymbolButton btnExportCsv;
        private Sunny.UI.UISymbolButton btnExportPDF;
        private Sunny.UI.UISymbolButton btnGetLogs;
        private Sunny.UI.UIComboBox ipLoginType;
        private Sunny.UI.UIDatePicker ipDateTo;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UIComboBox ipSize;
        private Sunny.UI.UIPagination uiPagination1;
        private Sunny.UI.UIPanel opTotalCount;
        private System.Windows.Forms.TabPage tabPage2;
        private Sunny.UI.UITitlePanel uiTitlePanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel5;
        private Sunny.UI.UIDataGridView uiDataGridView1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel6;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel7;
        private Sunny.UI.UISymbolButton uiSymbolButton1;
        private Sunny.UI.UIDatePicker uiDatePicker1;
        private Sunny.UI.UISymbolButton uiSymbolButton2;
        private Sunny.UI.UISymbolButton uiSymbolButton3;
        private Sunny.UI.UISymbolButton uiSymbolButton4;
        private Sunny.UI.UIComboBox uiComboBox1;
        private Sunny.UI.UIDatePicker uiDatePicker2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel8;
        private Sunny.UI.UIComboBox uiComboBox2;
        private Sunny.UI.UIPagination uiPagination2;
        private Sunny.UI.UIPanel uiPanel1;
    }
}