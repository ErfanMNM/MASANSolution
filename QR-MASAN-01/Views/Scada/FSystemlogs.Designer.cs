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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiPagination1 = new Sunny.UI.UIPagination();
            this.opDataG = new Sunny.UI.UIDataGridView();
            this.WK_AutoLog = new System.ComponentModel.BackgroundWorker();
            this.btnGetLogs = new Sunny.UI.UISymbolButton();
            this.ipLoginType = new Sunny.UI.UIComboBox();
            this.WK_Getlogs = new System.ComponentModel.BackgroundWorker();
            this.ipSize = new Sunny.UI.UIComboBox();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.uiLine1 = new Sunny.UI.UILine();
            this.uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            this.uiTitlePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opDataG)).BeginInit();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.uiTableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
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
            this.uiTitlePanel1.Size = new System.Drawing.Size(840, 674);
            this.uiTitlePanel1.TabIndex = 0;
            this.uiTitlePanel1.Text = "QUẢN LÝ LỊCH SỬ HỆ THỐNG";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.TitleHeight = 50;
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
            this.uiPagination1.Size = new System.Drawing.Size(628, 40);
            this.uiPagination1.TabIndex = 2;
            this.uiPagination1.Text = "uiPagination1";
            this.uiPagination1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiPagination1.TotalCount = 100;
            this.uiPagination1.PageChanged += new Sunny.UI.UIPagination.OnPageChangeEventHandler(this.uiPagination1_PageChanged);
            // 
            // opDataG
            // 
            this.opDataG.AllowUserToAddRows = false;
            this.opDataG.AllowUserToDeleteRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.opDataG.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.opDataG.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.opDataG.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.opDataG.BackgroundColor = System.Drawing.Color.White;
            this.opDataG.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.opDataG.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.opDataG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.opDataG.DefaultCellStyle = dataGridViewCellStyle8;
            this.opDataG.EnableHeadersVisualStyles = false;
            this.opDataG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opDataG.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.opDataG.Location = new System.Drawing.Point(3, 3);
            this.opDataG.Name = "opDataG";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.opDataG.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opDataG.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.opDataG.SelectedIndex = -1;
            this.opDataG.Size = new System.Drawing.Size(832, 305);
            this.opDataG.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.opDataG.TabIndex = 0;
            // 
            // WK_AutoLog
            // 
            this.WK_AutoLog.WorkerReportsProgress = true;
            this.WK_AutoLog.WorkerSupportsCancellation = true;
            this.WK_AutoLog.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_AutoLog_DoWork);
            // 
            // btnGetLogs
            // 
            this.btnGetLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGetLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnGetLogs.Location = new System.Drawing.Point(699, 3);
            this.btnGetLogs.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnGetLogs.Name = "btnGetLogs";
            this.btnGetLogs.Size = new System.Drawing.Size(124, 35);
            this.btnGetLogs.Symbol = 559524;
            this.btnGetLogs.TabIndex = 3;
            this.btnGetLogs.Text = "Lấy lịch sử";
            this.btnGetLogs.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnGetLogs.Click += new System.EventHandler(this.btnGetLogs_Click);
            // 
            // ipLoginType
            // 
            this.ipLoginType.DataSource = null;
            this.ipLoginType.FillColor = System.Drawing.Color.White;
            this.ipLoginType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipLoginType.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipLoginType.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipLoginType.Location = new System.Drawing.Point(529, 5);
            this.ipLoginType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipLoginType.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipLoginType.Name = "ipLoginType";
            this.ipLoginType.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipLoginType.Size = new System.Drawing.Size(163, 31);
            this.ipLoginType.SymbolSize = 24;
            this.ipLoginType.TabIndex = 4;
            this.ipLoginType.Text = "Loại logs";
            this.ipLoginType.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipLoginType.Watermark = "";
            // 
            // WK_Getlogs
            // 
            this.WK_Getlogs.WorkerReportsProgress = true;
            this.WK_Getlogs.WorkerSupportsCancellation = true;
            this.WK_Getlogs.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_Getlogs_DoWork);
            this.WK_Getlogs.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WK_Getlogs_RunWorkerCompleted);
            // 
            // ipSize
            // 
            this.ipSize.DataSource = null;
            this.ipSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipSize.FillColor = System.Drawing.Color.White;
            this.ipSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipSize.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipSize.Items.AddRange(new object[] {
            "2",
            "5",
            "10",
            "50",
            "100"});
            this.ipSize.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipSize.Location = new System.Drawing.Point(753, 5);
            this.ipSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipSize.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipSize.Name = "ipSize";
            this.ipSize.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipSize.Size = new System.Drawing.Size(69, 40);
            this.ipSize.SymbolSize = 24;
            this.ipSize.TabIndex = 5;
            this.ipSize.Text = "2";
            this.ipSize.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipSize.Watermark = "";
            this.ipSize.SelectedIndexChanged += new System.EventHandler(this.ipSize_SelectedIndexChanged);
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
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(838, 623);
            this.uiTableLayoutPanel1.TabIndex = 17;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 1;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Controls.Add(this.uiTableLayoutPanel4, 0, 1);
            this.uiTableLayoutPanel2.Controls.Add(this.uiTableLayoutPanel3, 0, 0);
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(3, 314);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 2;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37.01299F));
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 62.98701F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(832, 154);
            this.uiTableLayoutPanel2.TabIndex = 1;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.ColumnCount = 3;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.uiTableLayoutPanel3.Controls.Add(this.uiPagination1, 0, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.ipSize, 2, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.uiLine1, 1, 0);
            this.uiTableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 1;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(826, 50);
            this.uiTableLayoutPanel3.TabIndex = 2;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(639, 3);
            this.uiLine1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(107, 44);
            this.uiLine1.TabIndex = 6;
            this.uiLine1.Text = "Số lượng";
            // 
            // uiTableLayoutPanel4
            // 
            this.uiTableLayoutPanel4.ColumnCount = 3;
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 171F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.uiTableLayoutPanel4.Controls.Add(this.btnGetLogs, 2, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.ipLoginType, 1, 0);
            this.uiTableLayoutPanel4.Location = new System.Drawing.Point(3, 59);
            this.uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            this.uiTableLayoutPanel4.RowCount = 1;
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel4.Size = new System.Drawing.Size(826, 41);
            this.uiTableLayoutPanel4.TabIndex = 3;
            this.uiTableLayoutPanel4.TagString = null;
            // 
            // FSystemlogs
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiTitlePanel1);
            this.Name = "FSystemlogs";
            this.Symbol = 57591;
            this.Text = "Truy vết";
            this.Initialize += new System.EventHandler(this.FSystemlogs_Initialize);
            this.uiTitlePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.opDataG)).EndInit();
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.uiTableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UIDataGridView opDataG;
        private Sunny.UI.UIPagination uiPagination1;
        private System.ComponentModel.BackgroundWorker WK_AutoLog;
        private Sunny.UI.UISymbolButton btnGetLogs;
        private Sunny.UI.UIComboBox ipLoginType;
        private System.ComponentModel.BackgroundWorker WK_Getlogs;
        private Sunny.UI.UIComboBox ipSize;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UILine uiLine1;
    }
}