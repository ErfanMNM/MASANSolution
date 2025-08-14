namespace MASAN_SERIALIZATION.Views.Dashboards
{
    partial class PCartonDashboard
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
            this.mainContainer = new Sunny.UI.UITableLayoutPanel();
            this.scannerGroup = new Sunny.UI.UIGroupBox();
            this.scannerContainer = new Sunny.UI.UITableLayoutPanel();
            this.lane01Group = new Sunny.UI.UIGroupBox();
            this.opLane01 = new Sunny.UI.UIListBox();
            this.lane02Group = new Sunny.UI.UIGroupBox();
            this.opLane02 = new Sunny.UI.UIListBox();
            this.statusGroup = new Sunny.UI.UIGroupBox();
            this.statusContainer = new Sunny.UI.UITableLayoutPanel();
            this.currentCartonGroup = new Sunny.UI.UIGroupBox();
            this.currentCartonContainer = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel9 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel18 = new Sunny.UI.UIPanel();
            this.opCartonMaxID = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel10 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel19 = new Sunny.UI.UIPanel();
            this.opCartonCode = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.opcartonPackCount = new Sunny.UI.UIPanel();
            this.uiPanel3 = new Sunny.UI.UIPanel();
            this.previousCartonGroup = new Sunny.UI.UIGroupBox();
            this.previousCartonContainer = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel12 = new Sunny.UI.UITableLayoutPanel();
            this.opLastID = new Sunny.UI.UIPanel();
            this.uiPanel11 = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.opLastCode = new Sunny.UI.UIPanel();
            this.uiPanel5 = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel11 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel2 = new Sunny.UI.UIPanel();
            this.opLastActive = new Sunny.UI.UIPanel();
            this.nextCartonGroup = new Sunny.UI.UIGroupBox();
            this.nextCartonContainer = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel7 = new Sunny.UI.UIPanel();
            this.opnextID = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel5 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel12 = new Sunny.UI.UIPanel();
            this.opnextCode = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel9 = new Sunny.UI.UIPanel();
            this.opnextStart = new Sunny.UI.UIPanel();
            this.warningGroup = new Sunny.UI.UIGroupBox();
            this.opWarning = new Sunny.UI.UIListBox();
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.debugGroup = new Sunny.UI.UIGroupBox();
            this.debugContainer = new Sunny.UI.UITableLayoutPanel();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ipTest2 = new Sunny.UI.UITextBox();
            this.ipTest1 = new Sunny.UI.UITextBox();
            this.btnSend2 = new Sunny.UI.UISymbolButton();
            this.btnSend1 = new Sunny.UI.UISymbolButton();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.mainContainer.SuspendLayout();
            this.scannerGroup.SuspendLayout();
            this.scannerContainer.SuspendLayout();
            this.lane01Group.SuspendLayout();
            this.lane02Group.SuspendLayout();
            this.statusGroup.SuspendLayout();
            this.statusContainer.SuspendLayout();
            this.currentCartonGroup.SuspendLayout();
            this.currentCartonContainer.SuspendLayout();
            this.uiTableLayoutPanel9.SuspendLayout();
            this.uiTableLayoutPanel10.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.previousCartonGroup.SuspendLayout();
            this.previousCartonContainer.SuspendLayout();
            this.uiTableLayoutPanel12.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel11.SuspendLayout();
            this.nextCartonGroup.SuspendLayout();
            this.nextCartonContainer.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.uiTableLayoutPanel5.SuspendLayout();
            this.uiTableLayoutPanel4.SuspendLayout();
            this.warningGroup.SuspendLayout();
            this.uiTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.debugGroup.SuspendLayout();
            this.debugContainer.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.ColumnCount = 2;
            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.72953F));
            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.27047F));
            this.mainContainer.Controls.Add(this.scannerGroup, 0, 0);
            this.mainContainer.Controls.Add(this.statusGroup, 1, 0);
            this.mainContainer.Controls.Add(this.warningGroup, 0, 1);
            this.mainContainer.Controls.Add(this.uiTabControl1, 1, 1);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.RowCount = 2;
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainContainer.Size = new System.Drawing.Size(806, 628);
            this.mainContainer.TabIndex = 0;
            this.mainContainer.TagString = null;
            // 
            // scannerGroup
            // 
            this.scannerGroup.Controls.Add(this.scannerContainer);
            this.scannerGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.scannerGroup.Location = new System.Drawing.Point(4, 5);
            this.scannerGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.scannerGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.scannerGroup.Name = "scannerGroup";
            this.scannerGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.scannerGroup.Size = new System.Drawing.Size(417, 461);
            this.scannerGroup.TabIndex = 0;
            this.scannerGroup.Text = "📱 Lịch sử quét";
            this.scannerGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scannerContainer
            // 
            this.scannerContainer.ColumnCount = 1;
            this.scannerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scannerContainer.Controls.Add(this.lane01Group, 0, 0);
            this.scannerContainer.Controls.Add(this.lane02Group, 0, 1);
            this.scannerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerContainer.Location = new System.Drawing.Point(3, 32);
            this.scannerContainer.Name = "scannerContainer";
            this.scannerContainer.RowCount = 2;
            this.scannerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.scannerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.scannerContainer.Size = new System.Drawing.Size(411, 426);
            this.scannerContainer.TabIndex = 0;
            this.scannerContainer.TagString = null;
            // 
            // lane01Group
            // 
            this.lane01Group.Controls.Add(this.opLane01);
            this.lane01Group.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lane01Group.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lane01Group.Location = new System.Drawing.Point(4, 5);
            this.lane01Group.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lane01Group.MinimumSize = new System.Drawing.Size(1, 1);
            this.lane01Group.Name = "lane01Group";
            this.lane01Group.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.lane01Group.Size = new System.Drawing.Size(403, 203);
            this.lane01Group.TabIndex = 0;
            this.lane01Group.Text = "🗜Lịch sử làn 01";
            this.lane01Group.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opLane01
            // 
            this.opLane01.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLane01.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.opLane01.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opLane01.ItemSelectForeColor = System.Drawing.Color.White;
            this.opLane01.Location = new System.Drawing.Point(3, 32);
            this.opLane01.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opLane01.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLane01.Name = "opLane01";
            this.opLane01.Padding = new System.Windows.Forms.Padding(2);
            this.opLane01.ShowText = false;
            this.opLane01.Size = new System.Drawing.Size(397, 168);
            this.opLane01.TabIndex = 0;
            this.opLane01.Text = "uiListBox1";
            // 
            // lane02Group
            // 
            this.lane02Group.Controls.Add(this.opLane02);
            this.lane02Group.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lane02Group.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lane02Group.Location = new System.Drawing.Point(4, 218);
            this.lane02Group.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lane02Group.MinimumSize = new System.Drawing.Size(1, 1);
            this.lane02Group.Name = "lane02Group";
            this.lane02Group.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.lane02Group.Size = new System.Drawing.Size(403, 203);
            this.lane02Group.TabIndex = 1;
            this.lane02Group.Text = "🗜Lịch sử làn 02";
            this.lane02Group.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opLane02
            // 
            this.opLane02.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLane02.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.opLane02.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opLane02.ItemSelectForeColor = System.Drawing.Color.White;
            this.opLane02.Location = new System.Drawing.Point(3, 32);
            this.opLane02.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opLane02.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLane02.Name = "opLane02";
            this.opLane02.Padding = new System.Windows.Forms.Padding(2);
            this.opLane02.ShowText = false;
            this.opLane02.Size = new System.Drawing.Size(397, 168);
            this.opLane02.TabIndex = 1;
            this.opLane02.Text = "uiListBox2";
            // 
            // statusGroup
            // 
            this.statusGroup.Controls.Add(this.statusContainer);
            this.statusGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.statusGroup.Location = new System.Drawing.Point(429, 5);
            this.statusGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.statusGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.statusGroup.Name = "statusGroup";
            this.statusGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.statusGroup.Size = new System.Drawing.Size(373, 461);
            this.statusGroup.TabIndex = 1;
            this.statusGroup.Text = "📊 Trạng thái thùng";
            this.statusGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusContainer
            // 
            this.statusContainer.ColumnCount = 1;
            this.statusContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusContainer.Controls.Add(this.currentCartonGroup, 0, 0);
            this.statusContainer.Controls.Add(this.previousCartonGroup, 0, 1);
            this.statusContainer.Controls.Add(this.nextCartonGroup, 0, 2);
            this.statusContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusContainer.Location = new System.Drawing.Point(3, 32);
            this.statusContainer.Name = "statusContainer";
            this.statusContainer.RowCount = 3;
            this.statusContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.statusContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.statusContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.statusContainer.Size = new System.Drawing.Size(367, 426);
            this.statusContainer.TabIndex = 0;
            this.statusContainer.TagString = null;
            // 
            // currentCartonGroup
            // 
            this.currentCartonGroup.Controls.Add(this.currentCartonContainer);
            this.currentCartonGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentCartonGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.currentCartonGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.currentCartonGroup.Location = new System.Drawing.Point(4, 5);
            this.currentCartonGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.currentCartonGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.currentCartonGroup.Name = "currentCartonGroup";
            this.currentCartonGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.currentCartonGroup.Size = new System.Drawing.Size(359, 131);
            this.currentCartonGroup.TabIndex = 0;
            this.currentCartonGroup.Text = "📦 Thùng đang xếp";
            this.currentCartonGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // currentCartonContainer
            // 
            this.currentCartonContainer.ColumnCount = 1;
            this.currentCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.currentCartonContainer.Controls.Add(this.uiTableLayoutPanel9, 0, 0);
            this.currentCartonContainer.Controls.Add(this.uiTableLayoutPanel10, 0, 1);
            this.currentCartonContainer.Controls.Add(this.uiTableLayoutPanel1, 0, 2);
            this.currentCartonContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentCartonContainer.Location = new System.Drawing.Point(3, 32);
            this.currentCartonContainer.Name = "currentCartonContainer";
            this.currentCartonContainer.RowCount = 3;
            this.currentCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.currentCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.currentCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.currentCartonContainer.Size = new System.Drawing.Size(353, 96);
            this.currentCartonContainer.TabIndex = 0;
            this.currentCartonContainer.TagString = null;
            // 
            // uiTableLayoutPanel9
            // 
            this.uiTableLayoutPanel9.ColumnCount = 2;
            this.uiTableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel9.Controls.Add(this.uiPanel18, 0, 0);
            this.uiTableLayoutPanel9.Controls.Add(this.opCartonMaxID, 1, 0);
            this.uiTableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel9.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel9.Name = "uiTableLayoutPanel9";
            this.uiTableLayoutPanel9.RowCount = 1;
            this.uiTableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel9.Size = new System.Drawing.Size(347, 25);
            this.uiTableLayoutPanel9.TabIndex = 0;
            this.uiTableLayoutPanel9.TagString = null;
            // 
            // uiPanel18
            // 
            this.uiPanel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel18.Location = new System.Drawing.Point(2, 2);
            this.uiPanel18.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel18.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel18.Name = "uiPanel18";
            this.uiPanel18.Size = new System.Drawing.Size(134, 21);
            this.uiPanel18.TabIndex = 0;
            this.uiPanel18.Text = "🏷️ STT đang xếp:";
            this.uiPanel18.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opCartonMaxID
            // 
            this.opCartonMaxID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opCartonMaxID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.opCartonMaxID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opCartonMaxID.Location = new System.Drawing.Point(140, 2);
            this.opCartonMaxID.Margin = new System.Windows.Forms.Padding(2);
            this.opCartonMaxID.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCartonMaxID.Name = "opCartonMaxID";
            this.opCartonMaxID.Size = new System.Drawing.Size(205, 21);
            this.opCartonMaxID.TabIndex = 1;
            this.opCartonMaxID.Text = "-";
            this.opCartonMaxID.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel10
            // 
            this.uiTableLayoutPanel10.ColumnCount = 2;
            this.uiTableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel10.Controls.Add(this.uiPanel19, 0, 0);
            this.uiTableLayoutPanel10.Controls.Add(this.opCartonCode, 1, 0);
            this.uiTableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel10.Location = new System.Drawing.Point(3, 34);
            this.uiTableLayoutPanel10.Name = "uiTableLayoutPanel10";
            this.uiTableLayoutPanel10.RowCount = 1;
            this.uiTableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel10.Size = new System.Drawing.Size(347, 25);
            this.uiTableLayoutPanel10.TabIndex = 1;
            this.uiTableLayoutPanel10.TagString = null;
            // 
            // uiPanel19
            // 
            this.uiPanel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel19.Location = new System.Drawing.Point(2, 2);
            this.uiPanel19.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel19.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel19.Name = "uiPanel19";
            this.uiPanel19.Size = new System.Drawing.Size(134, 21);
            this.uiPanel19.TabIndex = 0;
            this.uiPanel19.Text = "📎 Mã đang xếp:";
            this.uiPanel19.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opCartonCode
            // 
            this.opCartonCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opCartonCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.opCartonCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opCartonCode.Location = new System.Drawing.Point(140, 2);
            this.opCartonCode.Margin = new System.Windows.Forms.Padding(2);
            this.opCartonCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCartonCode.Name = "opCartonCode";
            this.opCartonCode.Size = new System.Drawing.Size(205, 21);
            this.opCartonCode.TabIndex = 1;
            this.opCartonCode.Text = "-";
            this.opCartonCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 2;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.21595F));
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.78405F));
            this.uiTableLayoutPanel1.Controls.Add(this.opcartonPackCount, 1, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.uiPanel3, 0, 0);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(3, 65);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 1;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(347, 28);
            this.uiTableLayoutPanel1.TabIndex = 26;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // opcartonPackCount
            // 
            this.opcartonPackCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opcartonPackCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.opcartonPackCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opcartonPackCount.Location = new System.Drawing.Point(140, 2);
            this.opcartonPackCount.Margin = new System.Windows.Forms.Padding(2);
            this.opcartonPackCount.MinimumSize = new System.Drawing.Size(1, 1);
            this.opcartonPackCount.Name = "opcartonPackCount";
            this.opcartonPackCount.Size = new System.Drawing.Size(205, 24);
            this.opcartonPackCount.TabIndex = 1;
            this.opcartonPackCount.Text = "-";
            this.opcartonPackCount.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel3
            // 
            this.uiPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel3.Location = new System.Drawing.Point(2, 2);
            this.uiPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel3.Name = "uiPanel3";
            this.uiPanel3.Size = new System.Drawing.Size(134, 24);
            this.uiPanel3.TabIndex = 0;
            this.uiPanel3.Text = "🍾 Số chai:";
            this.uiPanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // previousCartonGroup
            // 
            this.previousCartonGroup.Controls.Add(this.previousCartonContainer);
            this.previousCartonGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previousCartonGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.previousCartonGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.previousCartonGroup.Location = new System.Drawing.Point(4, 146);
            this.previousCartonGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.previousCartonGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.previousCartonGroup.Name = "previousCartonGroup";
            this.previousCartonGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.previousCartonGroup.Size = new System.Drawing.Size(359, 131);
            this.previousCartonGroup.TabIndex = 1;
            this.previousCartonGroup.Text = "📋 Thùng trước";
            this.previousCartonGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // previousCartonContainer
            // 
            this.previousCartonContainer.ColumnCount = 1;
            this.previousCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.previousCartonContainer.Controls.Add(this.uiTableLayoutPanel12, 0, 0);
            this.previousCartonContainer.Controls.Add(this.uiTableLayoutPanel2, 0, 1);
            this.previousCartonContainer.Controls.Add(this.uiTableLayoutPanel11, 0, 2);
            this.previousCartonContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previousCartonContainer.Location = new System.Drawing.Point(3, 32);
            this.previousCartonContainer.Name = "previousCartonContainer";
            this.previousCartonContainer.RowCount = 3;
            this.previousCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.previousCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.previousCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.previousCartonContainer.Size = new System.Drawing.Size(353, 96);
            this.previousCartonContainer.TabIndex = 0;
            this.previousCartonContainer.TagString = null;
            // 
            // uiTableLayoutPanel12
            // 
            this.uiTableLayoutPanel12.ColumnCount = 2;
            this.uiTableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.21262F));
            this.uiTableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.78738F));
            this.uiTableLayoutPanel12.Controls.Add(this.opLastID, 1, 0);
            this.uiTableLayoutPanel12.Controls.Add(this.uiPanel11, 0, 0);
            this.uiTableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel12.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel12.Name = "uiTableLayoutPanel12";
            this.uiTableLayoutPanel12.RowCount = 1;
            this.uiTableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel12.Size = new System.Drawing.Size(347, 25);
            this.uiTableLayoutPanel12.TabIndex = 24;
            this.uiTableLayoutPanel12.TagString = null;
            // 
            // opLastID
            // 
            this.opLastID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLastID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opLastID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opLastID.Location = new System.Drawing.Point(140, 2);
            this.opLastID.Margin = new System.Windows.Forms.Padding(2);
            this.opLastID.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLastID.Name = "opLastID";
            this.opLastID.Size = new System.Drawing.Size(205, 21);
            this.opLastID.TabIndex = 6;
            this.opLastID.Text = "-";
            this.opLastID.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel11
            // 
            this.uiPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel11.Location = new System.Drawing.Point(2, 2);
            this.uiPanel11.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel11.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel11.Name = "uiPanel11";
            this.uiPanel11.Size = new System.Drawing.Size(134, 21);
            this.uiPanel11.TabIndex = 5;
            this.uiPanel11.Text = "STT trước đó";
            this.uiPanel11.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 2;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.21595F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.78405F));
            this.uiTableLayoutPanel2.Controls.Add(this.opLastCode, 1, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.uiPanel5, 0, 0);
            this.uiTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(3, 34);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(347, 25);
            this.uiTableLayoutPanel2.TabIndex = 27;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // opLastCode
            // 
            this.opLastCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLastCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opLastCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opLastCode.Location = new System.Drawing.Point(140, 2);
            this.opLastCode.Margin = new System.Windows.Forms.Padding(2);
            this.opLastCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLastCode.Name = "opLastCode";
            this.opLastCode.Size = new System.Drawing.Size(205, 21);
            this.opLastCode.TabIndex = 6;
            this.opLastCode.Text = "-";
            this.opLastCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiPanel5
            // 
            this.uiPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel5.Location = new System.Drawing.Point(2, 2);
            this.uiPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel5.Name = "uiPanel5";
            this.uiPanel5.Size = new System.Drawing.Size(134, 21);
            this.uiPanel5.TabIndex = 5;
            this.uiPanel5.Text = "Mã thùng";
            this.uiPanel5.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel11
            // 
            this.uiTableLayoutPanel11.ColumnCount = 2;
            this.uiTableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel11.Controls.Add(this.uiPanel2, 0, 0);
            this.uiTableLayoutPanel11.Controls.Add(this.opLastActive, 1, 0);
            this.uiTableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel11.Location = new System.Drawing.Point(3, 65);
            this.uiTableLayoutPanel11.Name = "uiTableLayoutPanel11";
            this.uiTableLayoutPanel11.RowCount = 1;
            this.uiTableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel11.Size = new System.Drawing.Size(347, 28);
            this.uiTableLayoutPanel11.TabIndex = 2;
            this.uiTableLayoutPanel11.TagString = null;
            // 
            // uiPanel2
            // 
            this.uiPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel2.Location = new System.Drawing.Point(2, 2);
            this.uiPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel2.Name = "uiPanel2";
            this.uiPanel2.Size = new System.Drawing.Size(134, 24);
            this.uiPanel2.TabIndex = 5;
            this.uiPanel2.Text = "⏰ Bắt đầu";
            this.uiPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opLastActive
            // 
            this.opLastActive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLastActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.opLastActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.opLastActive.Location = new System.Drawing.Point(140, 2);
            this.opLastActive.Margin = new System.Windows.Forms.Padding(2);
            this.opLastActive.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLastActive.Name = "opLastActive";
            this.opLastActive.Size = new System.Drawing.Size(205, 24);
            this.opLastActive.TabIndex = 1;
            this.opLastActive.Text = "-";
            this.opLastActive.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nextCartonGroup
            // 
            this.nextCartonGroup.Controls.Add(this.nextCartonContainer);
            this.nextCartonGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextCartonGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.nextCartonGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.nextCartonGroup.Location = new System.Drawing.Point(4, 287);
            this.nextCartonGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nextCartonGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.nextCartonGroup.Name = "nextCartonGroup";
            this.nextCartonGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.nextCartonGroup.Size = new System.Drawing.Size(359, 134);
            this.nextCartonGroup.TabIndex = 2;
            this.nextCartonGroup.Text = "🚀 Thùng sắp tới";
            this.nextCartonGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nextCartonContainer
            // 
            this.nextCartonContainer.ColumnCount = 1;
            this.nextCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.nextCartonContainer.Controls.Add(this.uiTableLayoutPanel3, 0, 0);
            this.nextCartonContainer.Controls.Add(this.uiTableLayoutPanel5, 0, 1);
            this.nextCartonContainer.Controls.Add(this.uiTableLayoutPanel4, 0, 2);
            this.nextCartonContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextCartonContainer.Location = new System.Drawing.Point(3, 32);
            this.nextCartonContainer.Name = "nextCartonContainer";
            this.nextCartonContainer.RowCount = 3;
            this.nextCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.nextCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.nextCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.nextCartonContainer.Size = new System.Drawing.Size(353, 99);
            this.nextCartonContainer.TabIndex = 0;
            this.nextCartonContainer.TagString = null;
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.ColumnCount = 2;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.21262F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.78738F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel7, 0, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.opnextID, 1, 0);
            this.uiTableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 1;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(347, 26);
            this.uiTableLayoutPanel3.TabIndex = 0;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // uiPanel7
            // 
            this.uiPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel7.Location = new System.Drawing.Point(2, 2);
            this.uiPanel7.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel7.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel7.Name = "uiPanel7";
            this.uiPanel7.Size = new System.Drawing.Size(121, 22);
            this.uiPanel7.TabIndex = 0;
            this.uiPanel7.Text = "🔜 STT tiếp theo:";
            this.uiPanel7.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opnextID
            // 
            this.opnextID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opnextID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.opnextID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.opnextID.Location = new System.Drawing.Point(127, 2);
            this.opnextID.Margin = new System.Windows.Forms.Padding(2);
            this.opnextID.MinimumSize = new System.Drawing.Size(1, 1);
            this.opnextID.Name = "opnextID";
            this.opnextID.Size = new System.Drawing.Size(218, 22);
            this.opnextID.TabIndex = 1;
            this.opnextID.Text = "-";
            this.opnextID.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel5
            // 
            this.uiTableLayoutPanel5.ColumnCount = 2;
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.21262F));
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.78738F));
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel5.Controls.Add(this.uiPanel12, 0, 0);
            this.uiTableLayoutPanel5.Controls.Add(this.opnextCode, 1, 0);
            this.uiTableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel5.Location = new System.Drawing.Point(3, 35);
            this.uiTableLayoutPanel5.Name = "uiTableLayoutPanel5";
            this.uiTableLayoutPanel5.RowCount = 1;
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel5.Size = new System.Drawing.Size(347, 26);
            this.uiTableLayoutPanel5.TabIndex = 1;
            this.uiTableLayoutPanel5.TagString = null;
            // 
            // uiPanel12
            // 
            this.uiPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel12.Location = new System.Drawing.Point(2, 2);
            this.uiPanel12.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel12.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel12.Name = "uiPanel12";
            this.uiPanel12.Size = new System.Drawing.Size(121, 22);
            this.uiPanel12.TabIndex = 0;
            this.uiPanel12.Text = "📎 Mã thùng:";
            this.uiPanel12.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opnextCode
            // 
            this.opnextCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opnextCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.opnextCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.opnextCode.Location = new System.Drawing.Point(127, 2);
            this.opnextCode.Margin = new System.Windows.Forms.Padding(2);
            this.opnextCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.opnextCode.Name = "opnextCode";
            this.opnextCode.Size = new System.Drawing.Size(218, 22);
            this.opnextCode.TabIndex = 1;
            this.opnextCode.Text = "-";
            this.opnextCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel4
            // 
            this.uiTableLayoutPanel4.ColumnCount = 2;
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.21262F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.78738F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.uiTableLayoutPanel4.Controls.Add(this.uiPanel9, 0, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.opnextStart, 1, 0);
            this.uiTableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel4.Location = new System.Drawing.Point(3, 67);
            this.uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            this.uiTableLayoutPanel4.RowCount = 1;
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel4.Size = new System.Drawing.Size(347, 29);
            this.uiTableLayoutPanel4.TabIndex = 2;
            this.uiTableLayoutPanel4.TagString = null;
            // 
            // uiPanel9
            // 
            this.uiPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel9.Location = new System.Drawing.Point(2, 2);
            this.uiPanel9.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel9.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel9.Name = "uiPanel9";
            this.uiPanel9.Size = new System.Drawing.Size(121, 25);
            this.uiPanel9.TabIndex = 0;
            this.uiPanel9.Text = "⏰ Bắt đầu:";
            this.uiPanel9.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opnextStart
            // 
            this.opnextStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opnextStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.opnextStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.opnextStart.Location = new System.Drawing.Point(127, 2);
            this.opnextStart.Margin = new System.Windows.Forms.Padding(2);
            this.opnextStart.MinimumSize = new System.Drawing.Size(1, 1);
            this.opnextStart.Name = "opnextStart";
            this.opnextStart.Size = new System.Drawing.Size(218, 25);
            this.opnextStart.TabIndex = 1;
            this.opnextStart.Text = "-";
            this.opnextStart.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // warningGroup
            // 
            this.warningGroup.Controls.Add(this.opWarning);
            this.warningGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.warningGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.warningGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.warningGroup.Location = new System.Drawing.Point(4, 476);
            this.warningGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.warningGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.warningGroup.Name = "warningGroup";
            this.warningGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.warningGroup.Size = new System.Drawing.Size(417, 147);
            this.warningGroup.TabIndex = 2;
            this.warningGroup.Text = "⚠️ Các CẢNH BÁO";
            this.warningGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opWarning
            // 
            this.opWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.opWarning.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.opWarning.ItemSelectForeColor = System.Drawing.Color.White;
            this.opWarning.Location = new System.Drawing.Point(3, 32);
            this.opWarning.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opWarning.MinimumSize = new System.Drawing.Size(1, 1);
            this.opWarning.Name = "opWarning";
            this.opWarning.Padding = new System.Windows.Forms.Padding(2);
            this.opWarning.ShowText = false;
            this.opWarning.Size = new System.Drawing.Size(411, 112);
            this.opWarning.TabIndex = 0;
            this.opWarning.Text = "uiListBox2";
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPage1);
            this.uiTabControl1.Controls.Add(this.tabPage2);
            this.uiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTabControl1.ItemSize = new System.Drawing.Size(150, 40);
            this.uiTabControl1.Location = new System.Drawing.Point(428, 474);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(375, 151);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 3;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.debugGroup);
            this.tabPage1.Location = new System.Drawing.Point(0, 40);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(375, 111);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Debug";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // debugGroup
            // 
            this.debugGroup.Controls.Add(this.debugContainer);
            this.debugGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.debugGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.debugGroup.Location = new System.Drawing.Point(0, 0);
            this.debugGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.debugGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.debugGroup.Name = "debugGroup";
            this.debugGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.debugGroup.Size = new System.Drawing.Size(375, 111);
            this.debugGroup.TabIndex = 4;
            this.debugGroup.Text = "🔧 Thông tin debug";
            this.debugGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // debugContainer
            // 
            this.debugContainer.ColumnCount = 1;
            this.debugContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.debugContainer.Controls.Add(this.uiLabel1, 0, 0);
            this.debugContainer.Controls.Add(this.uiLabel2, 0, 1);
            this.debugContainer.Controls.Add(this.uiLabel3, 0, 2);
            this.debugContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugContainer.Location = new System.Drawing.Point(3, 32);
            this.debugContainer.Name = "debugContainer";
            this.debugContainer.RowCount = 3;
            this.debugContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.debugContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.debugContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.debugContainer.Size = new System.Drawing.Size(369, 76);
            this.debugContainer.TabIndex = 0;
            this.debugContainer.TagString = null;
            // 
            // uiLabel1
            // 
            this.uiLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.uiLabel1.Location = new System.Drawing.Point(3, 0);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(363, 25);
            this.uiLabel1.TabIndex = 0;
            this.uiLabel1.Text = "Debug 1: -";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiLabel2
            // 
            this.uiLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.uiLabel2.Location = new System.Drawing.Point(3, 25);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(363, 25);
            this.uiLabel2.TabIndex = 1;
            this.uiLabel2.Text = "Debug 2: -";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiLabel3
            // 
            this.uiLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.uiLabel3.Location = new System.Drawing.Point(3, 50);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(363, 26);
            this.uiLabel3.TabIndex = 2;
            this.uiLabel3.Text = "Debug 3: -";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ipTest2);
            this.tabPage2.Controls.Add(this.ipTest1);
            this.tabPage2.Controls.Add(this.btnSend2);
            this.tabPage2.Controls.Add(this.btnSend1);
            this.tabPage2.Location = new System.Drawing.Point(0, 40);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(200, 60);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Test";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ipTest2
            // 
            this.ipTest2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipTest2.DoubleValue = 12345D;
            this.ipTest2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipTest2.IntValue = 12345;
            this.ipTest2.Location = new System.Drawing.Point(4, 57);
            this.ipTest2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipTest2.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipTest2.Name = "ipTest2";
            this.ipTest2.Padding = new System.Windows.Forms.Padding(5);
            this.ipTest2.ShowText = false;
            this.ipTest2.Size = new System.Drawing.Size(260, 51);
            this.ipTest2.TabIndex = 3;
            this.ipTest2.Text = "12345";
            this.ipTest2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipTest2.Watermark = "";
            // 
            // ipTest1
            // 
            this.ipTest1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipTest1.DoubleValue = 12345D;
            this.ipTest1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipTest1.IntValue = 12345;
            this.ipTest1.Location = new System.Drawing.Point(4, 5);
            this.ipTest1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipTest1.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipTest1.Name = "ipTest1";
            this.ipTest1.Padding = new System.Windows.Forms.Padding(5);
            this.ipTest1.ShowText = false;
            this.ipTest1.Size = new System.Drawing.Size(260, 49);
            this.ipTest1.TabIndex = 2;
            this.ipTest1.Text = "12345";
            this.ipTest1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipTest1.Watermark = "";
            // 
            // btnSend2
            // 
            this.btnSend2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSend2.Location = new System.Drawing.Point(271, 57);
            this.btnSend2.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSend2.Name = "btnSend2";
            this.btnSend2.Size = new System.Drawing.Size(100, 51);
            this.btnSend2.TabIndex = 1;
            this.btnSend2.Text = "Gửi 2";
            this.btnSend2.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSend2.Click += new System.EventHandler(this.btnSend2_Click);
            // 
            // btnSend1
            // 
            this.btnSend1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSend1.Location = new System.Drawing.Point(271, 3);
            this.btnSend1.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSend1.Name = "btnSend1";
            this.btnSend1.Size = new System.Drawing.Size(100, 51);
            this.btnSend1.TabIndex = 0;
            this.btnSend1.Text = "Gửi 1";
            this.btnSend1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSend1.Click += new System.EventHandler(this.btnSend1_Click);
            // 
            // PCartonDashboard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(806, 628);
            this.Controls.Add(this.mainContainer);
            this.Name = "PCartonDashboard";
            this.Symbol = 362622;
            this.Text = "Đóng thùng";
            this.mainContainer.ResumeLayout(false);
            this.scannerGroup.ResumeLayout(false);
            this.scannerContainer.ResumeLayout(false);
            this.lane01Group.ResumeLayout(false);
            this.lane02Group.ResumeLayout(false);
            this.statusGroup.ResumeLayout(false);
            this.statusContainer.ResumeLayout(false);
            this.currentCartonGroup.ResumeLayout(false);
            this.currentCartonContainer.ResumeLayout(false);
            this.uiTableLayoutPanel9.ResumeLayout(false);
            this.uiTableLayoutPanel10.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.previousCartonGroup.ResumeLayout(false);
            this.previousCartonContainer.ResumeLayout(false);
            this.uiTableLayoutPanel12.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel11.ResumeLayout(false);
            this.nextCartonGroup.ResumeLayout(false);
            this.nextCartonContainer.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.uiTableLayoutPanel5.ResumeLayout(false);
            this.uiTableLayoutPanel4.ResumeLayout(false);
            this.warningGroup.ResumeLayout(false);
            this.uiTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.debugGroup.ResumeLayout(false);
            this.debugContainer.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.IO.Ports.SerialPort serialPort2;
        private Sunny.UI.UITableLayoutPanel mainContainer;
        private Sunny.UI.UIGroupBox scannerGroup;
        private Sunny.UI.UITableLayoutPanel scannerContainer;
        private Sunny.UI.UIGroupBox lane01Group;
        private Sunny.UI.UIListBox opLane01;
        private Sunny.UI.UIGroupBox lane02Group;
        private Sunny.UI.UIListBox opLane02;
        private Sunny.UI.UIGroupBox statusGroup;
        private Sunny.UI.UITableLayoutPanel statusContainer;
        private Sunny.UI.UIGroupBox currentCartonGroup;
        private Sunny.UI.UITableLayoutPanel currentCartonContainer;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel9;
        private Sunny.UI.UIPanel uiPanel18;
        private Sunny.UI.UIPanel opCartonMaxID;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel10;
        private Sunny.UI.UIPanel uiPanel19;
        private Sunny.UI.UIPanel opCartonCode;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UIPanel uiPanel3;
        private Sunny.UI.UIPanel opcartonPackCount;
        private Sunny.UI.UIGroupBox previousCartonGroup;
        private Sunny.UI.UITableLayoutPanel previousCartonContainer;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel12;
        private Sunny.UI.UIPanel uiPanel11;
        private Sunny.UI.UIPanel opLastID;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UIPanel uiPanel5;
        private Sunny.UI.UIPanel opLastCode;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel11;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UIPanel opLastActive;
        private Sunny.UI.UIGroupBox nextCartonGroup;
        private Sunny.UI.UITableLayoutPanel nextCartonContainer;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UIPanel uiPanel7;
        private Sunny.UI.UIPanel opnextID;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel5;
        private Sunny.UI.UIPanel uiPanel12;
        private Sunny.UI.UIPanel opnextCode;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UIPanel uiPanel9;
        private Sunny.UI.UIPanel opnextStart;
        private Sunny.UI.UIGroupBox warningGroup;
        private Sunny.UI.UIListBox opWarning;
        private Sunny.UI.UITabControl uiTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Sunny.UI.UIGroupBox debugGroup;
        private Sunny.UI.UITableLayoutPanel debugContainer;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UILabel uiLabel3;
        private System.Windows.Forms.TabPage tabPage2;
        private Sunny.UI.UITextBox ipTest2;
        private Sunny.UI.UITextBox ipTest1;
        private Sunny.UI.UISymbolButton btnSend2;
        private Sunny.UI.UISymbolButton btnSend1;
    }
}