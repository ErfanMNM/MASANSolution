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
            this.lblCurId = new Sunny.UI.UILabel();
            this.lblCurCode = new Sunny.UI.UILabel();
            this.lblCurBottle = new Sunny.UI.UILabel();
            this.opCartonMaxID = new Sunny.UI.UIPanel();
            this.opCartonCode = new Sunny.UI.UIPanel();
            this.opcartonPackCount = new Sunny.UI.UIPanel();
            this.previousCartonGroup = new Sunny.UI.UIGroupBox();
            this.previousCartonContainer = new Sunny.UI.UITableLayoutPanel();
            this.lblPrevId = new Sunny.UI.UILabel();
            this.lblPrevCode = new Sunny.UI.UILabel();
            this.lblPrevStart = new Sunny.UI.UILabel();
            this.opLastID = new Sunny.UI.UIPanel();
            this.opLastCode = new Sunny.UI.UIPanel();
            this.opLastActive = new Sunny.UI.UIPanel();
            this.nextCartonGroup = new Sunny.UI.UIGroupBox();
            this.nextCartonContainer = new Sunny.UI.UITableLayoutPanel();
            this.lblNextId = new Sunny.UI.UILabel();
            this.lblNextCode = new Sunny.UI.UILabel();
            this.lblNextStart = new Sunny.UI.UILabel();
            this.opnextID = new Sunny.UI.UIPanel();
            this.opnextCode = new Sunny.UI.UIPanel();
            this.opnextStart = new Sunny.UI.UIPanel();
            this.warningGroup = new Sunny.UI.UIGroupBox();
            this.opWarning = new Sunny.UI.UIListBox();
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.debugGroup = new Sunny.UI.UIGroupBox();
            this.debugContainer = new Sunny.UI.UITableLayoutPanel();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.btnNextCarton = new Sunny.UI.UISymbolButton();
            this.uiSymbolButton1 = new Sunny.UI.UISymbolButton();
            this.uiLabel4 = new Sunny.UI.UILabel();
            this.uiLabel5 = new Sunny.UI.UILabel();
            this.ipTest1 = new Sunny.UI.UITextBox();
            this.ipTest2 = new Sunny.UI.UITextBox();
            this.btnSend1 = new Sunny.UI.UISymbolButton();
            this.btnSend2 = new Sunny.UI.UISymbolButton();
            this.uiSymbolButton2 = new Sunny.UI.UISymbolButton();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.tcpClient1 = new SpT.Communications.TCP.TCPClient(this.components);
            this.mainContainer.SuspendLayout();
            this.scannerGroup.SuspendLayout();
            this.scannerContainer.SuspendLayout();
            this.lane01Group.SuspendLayout();
            this.lane02Group.SuspendLayout();
            this.statusGroup.SuspendLayout();
            this.statusContainer.SuspendLayout();
            this.currentCartonGroup.SuspendLayout();
            this.currentCartonContainer.SuspendLayout();
            this.previousCartonGroup.SuspendLayout();
            this.previousCartonContainer.SuspendLayout();
            this.nextCartonGroup.SuspendLayout();
            this.nextCartonContainer.SuspendLayout();
            this.warningGroup.SuspendLayout();
            this.uiTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.debugGroup.SuspendLayout();
            this.debugContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.ColumnCount = 2;
            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53F));
            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.mainContainer.Controls.Add(this.scannerGroup, 0, 0);
            this.mainContainer.Controls.Add(this.statusGroup, 1, 0);
            this.mainContainer.Controls.Add(this.warningGroup, 0, 1);
            this.mainContainer.Controls.Add(this.uiTabControl1, 1, 1);
            this.mainContainer.BackColor = System.Drawing.Color.FromArgb(242, 244, 247);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.TagString = null;
            this.mainContainer.Padding = new System.Windows.Forms.Padding(10);
            this.mainContainer.Size = new System.Drawing.Size(1560, 904);
            this.mainContainer.TabIndex = 0;
            this.mainContainer.RowCount = 2;
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            // 
            // scannerGroup
            // 
            this.scannerGroup.Controls.Add(this.scannerContainer);
            this.scannerGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerGroup.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.scannerGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.scannerGroup.ForeColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.scannerGroup.Margin = new System.Windows.Forms.Padding(6);
            this.scannerGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.scannerGroup.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.scannerGroup.Radius = 18;
            this.scannerGroup.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.scannerGroup.StyleCustomMode = true;
            this.scannerGroup.Text = "LỊCH SỬ QUÉT";
            this.scannerGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scannerContainer
            // 
            this.scannerContainer.ColumnCount = 1;
            this.scannerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scannerContainer.Controls.Add(this.lane01Group, 0, 0);
            this.scannerContainer.Controls.Add(this.lane02Group, 0, 1);
            this.scannerContainer.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.scannerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerContainer.TagString = null;
            this.scannerContainer.RowCount = 2;
            this.scannerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.scannerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            // 
            // lane01Group
            // 
            this.lane01Group.Controls.Add(this.opLane01);
            this.lane01Group.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lane01Group.FillColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.lane01Group.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lane01Group.ForeColor = System.Drawing.Color.FromArgb(15, 157, 117);
            this.lane01Group.Margin = new System.Windows.Forms.Padding(5);
            this.lane01Group.MinimumSize = new System.Drawing.Size(1, 1);
            this.lane01Group.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.lane01Group.Radius = 14;
            this.lane01Group.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.lane01Group.StyleCustomMode = true;
            this.lane01Group.Text = "LÀN 01";
            this.lane01Group.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opLane01
            // 
            this.opLane01.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLane01.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opLane01.Font = new System.Drawing.Font("Consolas", 11F);
            this.opLane01.ForeColor = System.Drawing.Color.FromArgb(52, 64, 84);
            this.opLane01.HoverColor = System.Drawing.Color.FromArgb(231, 246, 239);
            this.opLane01.ItemSelectForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opLane01.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opLane01.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLane01.Padding = new System.Windows.Forms.Padding(2);
            this.opLane01.Radius = 10;
            this.opLane01.RectColor = System.Drawing.Color.FromArgb(228, 231, 236);
            this.opLane01.ShowText = false;
            this.opLane01.StyleCustomMode = true;
            this.opLane01.Text = "opLane01";
            this.opLane01.DoubleClick += new System.EventHandler(this.opLane01_DoubleClick);
            // 
            // lane02Group
            // 
            this.lane02Group.Controls.Add(this.opLane02);
            this.lane02Group.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lane02Group.FillColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.lane02Group.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lane02Group.ForeColor = System.Drawing.Color.FromArgb(15, 157, 117);
            this.lane02Group.Margin = new System.Windows.Forms.Padding(5);
            this.lane02Group.MinimumSize = new System.Drawing.Size(1, 1);
            this.lane02Group.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.lane02Group.Radius = 14;
            this.lane02Group.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.lane02Group.StyleCustomMode = true;
            this.lane02Group.Text = "LÀN 02";
            this.lane02Group.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opLane02
            // 
            this.opLane02.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLane02.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opLane02.Font = new System.Drawing.Font("Consolas", 11F);
            this.opLane02.ForeColor = System.Drawing.Color.FromArgb(52, 64, 84);
            this.opLane02.HoverColor = System.Drawing.Color.FromArgb(231, 246, 239);
            this.opLane02.ItemSelectForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opLane02.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opLane02.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLane02.Padding = new System.Windows.Forms.Padding(2);
            this.opLane02.Radius = 10;
            this.opLane02.RectColor = System.Drawing.Color.FromArgb(228, 231, 236);
            this.opLane02.ShowText = false;
            this.opLane02.StyleCustomMode = true;
            this.opLane02.Text = "opLane02";
            this.opLane02.DoubleClick += new System.EventHandler(this.opLane02_DoubleClick);
            // 
            // statusGroup
            // 
            this.statusGroup.Controls.Add(this.statusContainer);
            this.statusGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusGroup.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.statusGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.statusGroup.ForeColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.statusGroup.Margin = new System.Windows.Forms.Padding(6);
            this.statusGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.statusGroup.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.statusGroup.Radius = 18;
            this.statusGroup.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.statusGroup.StyleCustomMode = true;
            this.statusGroup.Text = "TRẠNG THÁI THÙNG";
            this.statusGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusContainer
            // 
            this.statusContainer.ColumnCount = 1;
            this.statusContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusContainer.Controls.Add(this.currentCartonGroup, 0, 0);
            this.statusContainer.Controls.Add(this.previousCartonGroup, 0, 1);
            this.statusContainer.Controls.Add(this.nextCartonGroup, 0, 2);
            this.statusContainer.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.statusContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusContainer.TagString = null;
            this.statusContainer.RowCount = 3;
            this.statusContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this.statusContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.statusContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32F));
            // 
            // currentCartonGroup
            // 
            this.currentCartonGroup.Controls.Add(this.currentCartonContainer);
            this.currentCartonGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentCartonGroup.FillColor = System.Drawing.Color.FromArgb(236, 253, 243);
            this.currentCartonGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.currentCartonGroup.ForeColor = System.Drawing.Color.FromArgb(6, 118, 71);
            this.currentCartonGroup.Margin = new System.Windows.Forms.Padding(5);
            this.currentCartonGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.currentCartonGroup.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.currentCartonGroup.Radius = 14;
            this.currentCartonGroup.RectColor = System.Drawing.Color.FromArgb(166, 244, 197);
            this.currentCartonGroup.StyleCustomMode = true;
            this.currentCartonGroup.Text = "THÙNG ĐANG XẾP";
            this.currentCartonGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // currentCartonContainer
            // 
            this.currentCartonContainer.ColumnCount = 3;
            this.currentCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.currentCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.currentCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.currentCartonContainer.Controls.Add(this.lblCurId, 0, 0);
            this.currentCartonContainer.Controls.Add(this.lblCurCode, 1, 0);
            this.currentCartonContainer.Controls.Add(this.lblCurBottle, 2, 0);
            this.currentCartonContainer.Controls.Add(this.opCartonMaxID, 0, 1);
            this.currentCartonContainer.Controls.Add(this.opCartonCode, 1, 1);
            this.currentCartonContainer.Controls.Add(this.opcartonPackCount, 2, 1);
            this.currentCartonContainer.BackColor = System.Drawing.Color.FromArgb(236, 253, 243);
            this.currentCartonContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentCartonContainer.TagString = null;
            this.currentCartonContainer.RowCount = 2;
            this.currentCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.currentCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // lblCurId
            // 
            this.lblCurId.AutoSize = false;
            this.lblCurId.BackColor = System.Drawing.Color.FromArgb(236, 253, 243);
            this.lblCurId.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblCurId.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblCurId.Text = "STT đang xếp";
            this.lblCurId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCurId.StyleCustomMode = true;
            this.lblCurId.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // lblCurCode
            // 
            this.lblCurCode.AutoSize = false;
            this.lblCurCode.BackColor = System.Drawing.Color.FromArgb(236, 253, 243);
            this.lblCurCode.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblCurCode.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblCurCode.Text = "Mã đang xếp";
            this.lblCurCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCurCode.StyleCustomMode = true;
            this.lblCurCode.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // lblCurBottle
            // 
            this.lblCurBottle.AutoSize = false;
            this.lblCurBottle.BackColor = System.Drawing.Color.FromArgb(236, 253, 243);
            this.lblCurBottle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblCurBottle.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblCurBottle.Text = "Số chai";
            this.lblCurBottle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCurBottle.StyleCustomMode = true;
            this.lblCurBottle.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // opCartonMaxID
            // 
            this.opCartonMaxID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opCartonMaxID.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opCartonMaxID.Font = new System.Drawing.Font("Consolas", 26F, System.Drawing.FontStyle.Bold);
            this.opCartonMaxID.ForeColor = System.Drawing.Color.FromArgb(6, 118, 71);
            this.opCartonMaxID.Margin = new System.Windows.Forms.Padding(3);
            this.opCartonMaxID.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCartonMaxID.Radius = 10;
            this.opCartonMaxID.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opCartonMaxID.StyleCustomMode = true;
            this.opCartonMaxID.Text = "-";
            this.opCartonMaxID.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opCartonCode
            // 
            this.opCartonCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opCartonCode.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opCartonCode.Font = new System.Drawing.Font("Consolas", 13F, System.Drawing.FontStyle.Bold);
            this.opCartonCode.ForeColor = System.Drawing.Color.FromArgb(6, 118, 71);
            this.opCartonCode.Margin = new System.Windows.Forms.Padding(3);
            this.opCartonCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCartonCode.Radius = 10;
            this.opCartonCode.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opCartonCode.StyleCustomMode = true;
            this.opCartonCode.Text = "-";
            this.opCartonCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opcartonPackCount
            // 
            this.opcartonPackCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opcartonPackCount.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opcartonPackCount.Font = new System.Drawing.Font("Consolas", 26F, System.Drawing.FontStyle.Bold);
            this.opcartonPackCount.ForeColor = System.Drawing.Color.FromArgb(6, 118, 71);
            this.opcartonPackCount.Margin = new System.Windows.Forms.Padding(3);
            this.opcartonPackCount.MinimumSize = new System.Drawing.Size(1, 1);
            this.opcartonPackCount.Radius = 10;
            this.opcartonPackCount.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opcartonPackCount.StyleCustomMode = true;
            this.opcartonPackCount.Text = "-";
            this.opcartonPackCount.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // previousCartonGroup
            // 
            this.previousCartonGroup.Controls.Add(this.previousCartonContainer);
            this.previousCartonGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previousCartonGroup.FillColor = System.Drawing.Color.FromArgb(255, 250, 235);
            this.previousCartonGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.previousCartonGroup.ForeColor = System.Drawing.Color.FromArgb(181, 71, 8);
            this.previousCartonGroup.Margin = new System.Windows.Forms.Padding(5);
            this.previousCartonGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.previousCartonGroup.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.previousCartonGroup.Radius = 14;
            this.previousCartonGroup.RectColor = System.Drawing.Color.FromArgb(254, 223, 137);
            this.previousCartonGroup.StyleCustomMode = true;
            this.previousCartonGroup.Text = "THÙNG TRƯỚC";
            this.previousCartonGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // previousCartonContainer
            // 
            this.previousCartonContainer.ColumnCount = 3;
            this.previousCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.previousCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38F));
            this.previousCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.previousCartonContainer.Controls.Add(this.lblPrevId, 0, 0);
            this.previousCartonContainer.Controls.Add(this.lblPrevCode, 1, 0);
            this.previousCartonContainer.Controls.Add(this.lblPrevStart, 2, 0);
            this.previousCartonContainer.Controls.Add(this.opLastID, 0, 1);
            this.previousCartonContainer.Controls.Add(this.opLastCode, 1, 1);
            this.previousCartonContainer.Controls.Add(this.opLastActive, 2, 1);
            this.previousCartonContainer.BackColor = System.Drawing.Color.FromArgb(255, 250, 235);
            this.previousCartonContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previousCartonContainer.TagString = null;
            this.previousCartonContainer.RowCount = 2;
            this.previousCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.previousCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // lblPrevId
            // 
            this.lblPrevId.AutoSize = false;
            this.lblPrevId.BackColor = System.Drawing.Color.FromArgb(255, 250, 235);
            this.lblPrevId.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPrevId.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblPrevId.Text = "STT trước đó";
            this.lblPrevId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPrevId.StyleCustomMode = true;
            this.lblPrevId.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // lblPrevCode
            // 
            this.lblPrevCode.AutoSize = false;
            this.lblPrevCode.BackColor = System.Drawing.Color.FromArgb(255, 250, 235);
            this.lblPrevCode.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPrevCode.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblPrevCode.Text = "Mã thùng";
            this.lblPrevCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPrevCode.StyleCustomMode = true;
            this.lblPrevCode.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // lblPrevStart
            // 
            this.lblPrevStart.AutoSize = false;
            this.lblPrevStart.BackColor = System.Drawing.Color.FromArgb(255, 250, 235);
            this.lblPrevStart.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPrevStart.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblPrevStart.Text = "Bắt đầu";
            this.lblPrevStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPrevStart.StyleCustomMode = true;
            this.lblPrevStart.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // opLastID
            // 
            this.opLastID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLastID.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opLastID.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold);
            this.opLastID.ForeColor = System.Drawing.Color.FromArgb(181, 71, 8);
            this.opLastID.Margin = new System.Windows.Forms.Padding(3);
            this.opLastID.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLastID.Radius = 10;
            this.opLastID.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opLastID.StyleCustomMode = true;
            this.opLastID.Text = "-";
            this.opLastID.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opLastCode
            // 
            this.opLastCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLastCode.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opLastCode.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.opLastCode.ForeColor = System.Drawing.Color.FromArgb(181, 71, 8);
            this.opLastCode.Margin = new System.Windows.Forms.Padding(3);
            this.opLastCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLastCode.Radius = 10;
            this.opLastCode.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opLastCode.StyleCustomMode = true;
            this.opLastCode.Text = "-";
            this.opLastCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opLastActive
            // 
            this.opLastActive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opLastActive.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opLastActive.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this.opLastActive.ForeColor = System.Drawing.Color.FromArgb(181, 71, 8);
            this.opLastActive.Margin = new System.Windows.Forms.Padding(3);
            this.opLastActive.MinimumSize = new System.Drawing.Size(1, 1);
            this.opLastActive.Radius = 10;
            this.opLastActive.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opLastActive.StyleCustomMode = true;
            this.opLastActive.Text = "-";
            this.opLastActive.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nextCartonGroup
            // 
            this.nextCartonGroup.Controls.Add(this.nextCartonContainer);
            this.nextCartonGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextCartonGroup.FillColor = System.Drawing.Color.FromArgb(239, 248, 255);
            this.nextCartonGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.nextCartonGroup.ForeColor = System.Drawing.Color.FromArgb(23, 92, 211);
            this.nextCartonGroup.Margin = new System.Windows.Forms.Padding(5);
            this.nextCartonGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.nextCartonGroup.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.nextCartonGroup.Radius = 14;
            this.nextCartonGroup.RectColor = System.Drawing.Color.FromArgb(178, 221, 255);
            this.nextCartonGroup.StyleCustomMode = true;
            this.nextCartonGroup.Text = "THÙNG SẮP TỚI";
            this.nextCartonGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nextCartonContainer
            // 
            this.nextCartonContainer.ColumnCount = 3;
            this.nextCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.nextCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38F));
            this.nextCartonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.nextCartonContainer.Controls.Add(this.lblNextId, 0, 0);
            this.nextCartonContainer.Controls.Add(this.lblNextCode, 1, 0);
            this.nextCartonContainer.Controls.Add(this.lblNextStart, 2, 0);
            this.nextCartonContainer.Controls.Add(this.opnextID, 0, 1);
            this.nextCartonContainer.Controls.Add(this.opnextCode, 1, 1);
            this.nextCartonContainer.Controls.Add(this.opnextStart, 2, 1);
            this.nextCartonContainer.BackColor = System.Drawing.Color.FromArgb(239, 248, 255);
            this.nextCartonContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextCartonContainer.TagString = null;
            this.nextCartonContainer.RowCount = 2;
            this.nextCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.nextCartonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // lblNextId
            // 
            this.lblNextId.AutoSize = false;
            this.lblNextId.BackColor = System.Drawing.Color.FromArgb(239, 248, 255);
            this.lblNextId.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblNextId.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblNextId.Text = "STT tiếp theo";
            this.lblNextId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNextId.StyleCustomMode = true;
            this.lblNextId.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // lblNextCode
            // 
            this.lblNextCode.AutoSize = false;
            this.lblNextCode.BackColor = System.Drawing.Color.FromArgb(239, 248, 255);
            this.lblNextCode.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblNextCode.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblNextCode.Text = "Mã thùng";
            this.lblNextCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNextCode.StyleCustomMode = true;
            this.lblNextCode.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // lblNextStart
            // 
            this.lblNextStart.AutoSize = false;
            this.lblNextStart.BackColor = System.Drawing.Color.FromArgb(239, 248, 255);
            this.lblNextStart.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblNextStart.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.lblNextStart.Text = "Bắt đầu";
            this.lblNextStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNextStart.StyleCustomMode = true;
            this.lblNextStart.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // opnextID
            // 
            this.opnextID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opnextID.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opnextID.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold);
            this.opnextID.ForeColor = System.Drawing.Color.FromArgb(23, 92, 211);
            this.opnextID.Margin = new System.Windows.Forms.Padding(3);
            this.opnextID.MinimumSize = new System.Drawing.Size(1, 1);
            this.opnextID.Radius = 10;
            this.opnextID.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opnextID.StyleCustomMode = true;
            this.opnextID.Text = "-";
            this.opnextID.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opnextCode
            // 
            this.opnextCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opnextCode.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opnextCode.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.opnextCode.ForeColor = System.Drawing.Color.FromArgb(23, 92, 211);
            this.opnextCode.Margin = new System.Windows.Forms.Padding(3);
            this.opnextCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.opnextCode.Radius = 10;
            this.opnextCode.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opnextCode.StyleCustomMode = true;
            this.opnextCode.Text = "-";
            this.opnextCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opnextStart
            // 
            this.opnextStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opnextStart.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opnextStart.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this.opnextStart.ForeColor = System.Drawing.Color.FromArgb(23, 92, 211);
            this.opnextStart.Margin = new System.Windows.Forms.Padding(3);
            this.opnextStart.MinimumSize = new System.Drawing.Size(1, 1);
            this.opnextStart.Radius = 10;
            this.opnextStart.RectColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.opnextStart.StyleCustomMode = true;
            this.opnextStart.Text = "-";
            this.opnextStart.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // warningGroup
            // 
            this.warningGroup.Controls.Add(this.opWarning);
            this.warningGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.warningGroup.FillColor = System.Drawing.Color.FromArgb(254, 243, 242);
            this.warningGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.warningGroup.ForeColor = System.Drawing.Color.FromArgb(217, 45, 32);
            this.warningGroup.Margin = new System.Windows.Forms.Padding(6);
            this.warningGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.warningGroup.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.warningGroup.Radius = 18;
            this.warningGroup.RectColor = System.Drawing.Color.FromArgb(254, 205, 202);
            this.warningGroup.StyleCustomMode = true;
            this.warningGroup.Text = "CÁC CẢNH BÁO";
            this.warningGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opWarning
            // 
            this.opWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opWarning.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opWarning.Font = new System.Drawing.Font("Consolas", 11F);
            this.opWarning.ForeColor = System.Drawing.Color.FromArgb(180, 35, 24);
            this.opWarning.HoverColor = System.Drawing.Color.FromArgb(254, 228, 226);
            this.opWarning.ItemSelectForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.opWarning.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opWarning.MinimumSize = new System.Drawing.Size(1, 1);
            this.opWarning.Padding = new System.Windows.Forms.Padding(2);
            this.opWarning.Radius = 10;
            this.opWarning.RectColor = System.Drawing.Color.FromArgb(254, 205, 202);
            this.opWarning.ShowText = false;
            this.opWarning.StyleCustomMode = true;
            this.opWarning.Text = "opWarning";
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPage1);
            this.uiTabControl1.Controls.Add(this.tabPage3);
            this.uiTabControl1.Controls.Add(this.tabPage2);
            this.uiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiTabControl1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.uiTabControl1.ItemSize = new System.Drawing.Size(150, 40);
            this.uiTabControl1.Margin = new System.Windows.Forms.Padding(6);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.StyleCustomMode = true;
            this.uiTabControl1.TabBackColor = System.Drawing.Color.FromArgb(242, 244, 247);
            this.uiTabControl1.TabSelectedColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiTabControl1.TabSelectedForeColor = System.Drawing.Color.FromArgb(15, 157, 117);
            this.uiTabControl1.TabSelectedHighColor = System.Drawing.Color.FromArgb(15, 157, 117);
            this.uiTabControl1.TabUnSelectedColor = System.Drawing.Color.FromArgb(228, 231, 236);
            this.uiTabControl1.TabUnSelectedForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.uiTabControl1.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.debugGroup);
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.tabPage1.Location = new System.Drawing.Point(0, 40);
            this.tabPage1.Size = new System.Drawing.Size(732, 180);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Debug";
            this.tabPage1.UseVisualStyleBackColor = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnNextCarton);
            this.tabPage3.Controls.Add(this.uiSymbolButton1);
            this.tabPage3.Controls.Add(this.uiLabel4);
            this.tabPage3.Controls.Add(this.uiLabel5);
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.tabPage3.Location = new System.Drawing.Point(0, 40);
            this.tabPage3.Size = new System.Drawing.Size(732, 180);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Tùy Chỉnh";
            this.tabPage3.UseVisualStyleBackColor = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.uiSymbolButton2);
            this.tabPage2.Controls.Add(this.ipTest2);
            this.tabPage2.Controls.Add(this.ipTest1);
            this.tabPage2.Controls.Add(this.btnSend2);
            this.tabPage2.Controls.Add(this.btnSend1);
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.tabPage2.Location = new System.Drawing.Point(0, 40);
            this.tabPage2.Size = new System.Drawing.Size(732, 180);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Test";
            this.tabPage2.UseVisualStyleBackColor = false;
            // 
            // debugGroup
            // 
            this.debugGroup.Controls.Add(this.debugContainer);
            this.debugGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugGroup.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.debugGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.debugGroup.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.debugGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.debugGroup.Padding = new System.Windows.Forms.Padding(6, 36, 6, 6);
            this.debugGroup.Radius = 8;
            this.debugGroup.RectColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.debugGroup.StyleCustomMode = true;
            this.debugGroup.Text = "THÔNG TIN DEBUG";
            this.debugGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.debugGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            // 
            // debugContainer
            // 
            this.debugContainer.ColumnCount = 1;
            this.debugContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.debugContainer.Controls.Add(this.uiLabel1, 0, 0);
            this.debugContainer.Controls.Add(this.uiLabel2, 0, 1);
            this.debugContainer.Controls.Add(this.uiLabel3, 0, 2);
            this.debugContainer.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.debugContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugContainer.TagString = null;
            this.debugContainer.RowCount = 3;
            this.debugContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.debugContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.debugContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            // 
            // uiLabel1
            // 
            this.uiLabel1.AutoSize = false;
            this.uiLabel1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.uiLabel1.Text = "Debug 1: -";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.StyleCustomMode = true;
            this.uiLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLabel1.Font = new System.Drawing.Font("Consolas", 11F);
            // 
            // uiLabel2
            // 
            this.uiLabel2.AutoSize = false;
            this.uiLabel2.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiLabel2.ForeColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.uiLabel2.Text = "Debug 2: -";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel2.StyleCustomMode = true;
            this.uiLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLabel2.Font = new System.Drawing.Font("Consolas", 11F);
            // 
            // uiLabel3
            // 
            this.uiLabel3.AutoSize = false;
            this.uiLabel3.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiLabel3.ForeColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.uiLabel3.Text = "Debug 3: -";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel3.StyleCustomMode = true;
            this.uiLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLabel3.Font = new System.Drawing.Font("Consolas", 11F);
            // 
            // btnNextCarton
            // 
            this.btnNextCarton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNextCarton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnNextCarton.Location = new System.Drawing.Point(16, 14);
            this.btnNextCarton.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNextCarton.Radius = 10;
            this.btnNextCarton.Size = new System.Drawing.Size(250, 52);
            this.btnNextCarton.StyleCustomMode = true;
            this.btnNextCarton.Text = "Bắt đầu thùng mới";
            this.btnNextCarton.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            this.btnNextCarton.Click += new System.EventHandler(this.btnNextCarton_Click);
            this.btnNextCarton.Symbol = 61473;
            this.btnNextCarton.SymbolSize = 24;
            this.btnNextCarton.FillColor = System.Drawing.Color.FromArgb(247, 144, 9);
            this.btnNextCarton.FillHoverColor = System.Drawing.Color.FromArgb(253, 176, 34);
            this.btnNextCarton.FillPressColor = System.Drawing.Color.FromArgb(220, 104, 3);
            this.btnNextCarton.RectColor = System.Drawing.Color.FromArgb(247, 144, 9);
            this.btnNextCarton.RectHoverColor = System.Drawing.Color.FromArgb(253, 176, 34);
            this.btnNextCarton.RectPressColor = System.Drawing.Color.FromArgb(220, 104, 3);
            this.btnNextCarton.ForeColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.btnNextCarton.ForeHoverColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.btnNextCarton.ForePressColor = System.Drawing.Color.FromArgb(16, 24, 40);
            // 
            // uiSymbolButton1
            // 
            this.uiSymbolButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.uiSymbolButton1.Location = new System.Drawing.Point(278, 14);
            this.uiSymbolButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton1.Radius = 10;
            this.uiSymbolButton1.Size = new System.Drawing.Size(170, 52);
            this.uiSymbolButton1.StyleCustomMode = true;
            this.uiSymbolButton1.Text = "Kiểm tra";
            this.uiSymbolButton1.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            this.uiSymbolButton1.Click += new System.EventHandler(this.uiSymbolButton1_Click);
            this.uiSymbolButton1.Symbol = 61473;
            this.uiSymbolButton1.SymbolSize = 24;
            this.uiSymbolButton1.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiSymbolButton1.FillHoverColor = System.Drawing.Color.FromArgb(242, 244, 247);
            this.uiSymbolButton1.FillPressColor = System.Drawing.Color.FromArgb(234, 236, 240);
            this.uiSymbolButton1.RectColor = System.Drawing.Color.FromArgb(208, 213, 221);
            this.uiSymbolButton1.RectHoverColor = System.Drawing.Color.FromArgb(152, 162, 179);
            this.uiSymbolButton1.RectPressColor = System.Drawing.Color.FromArgb(152, 162, 179);
            this.uiSymbolButton1.ForeColor = System.Drawing.Color.FromArgb(52, 64, 84);
            this.uiSymbolButton1.ForeHoverColor = System.Drawing.Color.FromArgb(52, 64, 84);
            this.uiSymbolButton1.ForePressColor = System.Drawing.Color.FromArgb(52, 64, 84);
            // 
            // uiLabel4
            // 
            this.uiLabel4.AutoSize = false;
            this.uiLabel4.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiLabel4.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.uiLabel4.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.uiLabel4.Text = "Tính năng này sẽ ngưng việc đóng thùng hiện tại";
            this.uiLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel4.StyleCustomMode = true;
            this.uiLabel4.Location = new System.Drawing.Point(16, 80);
            this.uiLabel4.Size = new System.Drawing.Size(690, 28);
            // 
            // uiLabel5
            // 
            this.uiLabel5.AutoSize = false;
            this.uiLabel5.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiLabel5.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.uiLabel5.ForeColor = System.Drawing.Color.FromArgb(102, 112, 133);
            this.uiLabel5.Text = "Lập tức qua thùng mới để đóng";
            this.uiLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel5.StyleCustomMode = true;
            this.uiLabel5.Location = new System.Drawing.Point(16, 108);
            this.uiLabel5.Size = new System.Drawing.Size(690, 28);
            // 
            // ipTest1
            // 
            this.ipTest1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipTest1.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.ipTest1.Font = new System.Drawing.Font("Consolas", 12F);
            this.ipTest1.ForeColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.ipTest1.Location = new System.Drawing.Point(16, 12);
            this.ipTest1.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipTest1.Padding = new System.Windows.Forms.Padding(5);
            this.ipTest1.Radius = 8;
            this.ipTest1.RectColor = System.Drawing.Color.FromArgb(208, 213, 221);
            this.ipTest1.ShowText = false;
            this.ipTest1.Size = new System.Drawing.Size(440, 48);
            this.ipTest1.StyleCustomMode = true;
            this.ipTest1.Text = "THUNG -1-1";
            this.ipTest1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipTest1.Watermark = "";
            // 
            // ipTest2
            // 
            this.ipTest2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipTest2.FillColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.ipTest2.Font = new System.Drawing.Font("Consolas", 12F);
            this.ipTest2.ForeColor = System.Drawing.Color.FromArgb(16, 24, 40);
            this.ipTest2.Location = new System.Drawing.Point(16, 72);
            this.ipTest2.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipTest2.Padding = new System.Windows.Forms.Padding(5);
            this.ipTest2.Radius = 8;
            this.ipTest2.RectColor = System.Drawing.Color.FromArgb(208, 213, 221);
            this.ipTest2.ShowText = false;
            this.ipTest2.Size = new System.Drawing.Size(440, 48);
            this.ipTest2.StyleCustomMode = true;
            this.ipTest2.Text = "THUNG -2-2";
            this.ipTest2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipTest2.Watermark = "";
            // 
            // btnSend1
            // 
            this.btnSend1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSend1.Location = new System.Drawing.Point(468, 12);
            this.btnSend1.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSend1.Radius = 10;
            this.btnSend1.Size = new System.Drawing.Size(110, 48);
            this.btnSend1.StyleCustomMode = true;
            this.btnSend1.Text = "Thùng 1";
            this.btnSend1.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSend1.Click += new System.EventHandler(this.btnSend1_Click);
            this.btnSend1.FillColor = System.Drawing.Color.FromArgb(21, 112, 239);
            this.btnSend1.FillHoverColor = System.Drawing.Color.FromArgb(46, 144, 250);
            this.btnSend1.FillPressColor = System.Drawing.Color.FromArgb(23, 92, 211);
            this.btnSend1.RectColor = System.Drawing.Color.FromArgb(21, 112, 239);
            this.btnSend1.RectHoverColor = System.Drawing.Color.FromArgb(46, 144, 250);
            this.btnSend1.RectPressColor = System.Drawing.Color.FromArgb(23, 92, 211);
            this.btnSend1.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnSend1.ForeHoverColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnSend1.ForePressColor = System.Drawing.Color.FromArgb(255, 255, 255);
            // 
            // btnSend2
            // 
            this.btnSend2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSend2.Location = new System.Drawing.Point(468, 72);
            this.btnSend2.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSend2.Radius = 10;
            this.btnSend2.Size = new System.Drawing.Size(110, 48);
            this.btnSend2.StyleCustomMode = true;
            this.btnSend2.Text = "Thùng 2";
            this.btnSend2.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSend2.Click += new System.EventHandler(this.btnSend2_Click);
            this.btnSend2.FillColor = System.Drawing.Color.FromArgb(21, 112, 239);
            this.btnSend2.FillHoverColor = System.Drawing.Color.FromArgb(46, 144, 250);
            this.btnSend2.FillPressColor = System.Drawing.Color.FromArgb(23, 92, 211);
            this.btnSend2.RectColor = System.Drawing.Color.FromArgb(21, 112, 239);
            this.btnSend2.RectHoverColor = System.Drawing.Color.FromArgb(46, 144, 250);
            this.btnSend2.RectPressColor = System.Drawing.Color.FromArgb(23, 92, 211);
            this.btnSend2.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnSend2.ForeHoverColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnSend2.ForePressColor = System.Drawing.Color.FromArgb(255, 255, 255);
            // 
            // uiSymbolButton2
            // 
            this.uiSymbolButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.uiSymbolButton2.Location = new System.Drawing.Point(592, 12);
            this.uiSymbolButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton2.Radius = 10;
            this.uiSymbolButton2.Size = new System.Drawing.Size(120, 108);
            this.uiSymbolButton2.StyleCustomMode = true;
            this.uiSymbolButton2.Text = "testMode";
            this.uiSymbolButton2.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            this.uiSymbolButton2.Click += new System.EventHandler(this.uiSymbolButton2_Click);
            this.uiSymbolButton2.FillColor = System.Drawing.Color.FromArgb(3, 152, 85);
            this.uiSymbolButton2.FillHoverColor = System.Drawing.Color.FromArgb(18, 183, 106);
            this.uiSymbolButton2.FillPressColor = System.Drawing.Color.FromArgb(2, 122, 72);
            this.uiSymbolButton2.RectColor = System.Drawing.Color.FromArgb(3, 152, 85);
            this.uiSymbolButton2.RectHoverColor = System.Drawing.Color.FromArgb(18, 183, 106);
            this.uiSymbolButton2.RectPressColor = System.Drawing.Color.FromArgb(2, 122, 72);
            this.uiSymbolButton2.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiSymbolButton2.ForeHoverColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.uiSymbolButton2.ForePressColor = System.Drawing.Color.FromArgb(255, 255, 255);
            // 
            // serialPort1
            // 
            // 
            // serialPort2
            // 
            // 
            // tcpClient1
            // 
            this.tcpClient1.IP = "127.0.0.1";
            this.tcpClient1.Port = 5566;
            this.tcpClient1.ClientCallBack += new SpT.Communications.TCP.TCPClient.EventForClient(this.tcpClient1_ClientCallBack);
            // 
            // PCartonDashboard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1560, 904);
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
            this.previousCartonGroup.ResumeLayout(false);
            this.previousCartonContainer.ResumeLayout(false);
            this.nextCartonGroup.ResumeLayout(false);
            this.nextCartonContainer.ResumeLayout(false);
            this.warningGroup.ResumeLayout(false);
            this.uiTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.debugGroup.ResumeLayout(false);
            this.debugContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

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
        private Sunny.UI.UILabel lblCurId;
        private Sunny.UI.UILabel lblCurCode;
        private Sunny.UI.UILabel lblCurBottle;
        private Sunny.UI.UIPanel opCartonMaxID;
        private Sunny.UI.UIPanel opCartonCode;
        private Sunny.UI.UIPanel opcartonPackCount;
        private Sunny.UI.UIGroupBox previousCartonGroup;
        private Sunny.UI.UITableLayoutPanel previousCartonContainer;
        private Sunny.UI.UILabel lblPrevId;
        private Sunny.UI.UILabel lblPrevCode;
        private Sunny.UI.UILabel lblPrevStart;
        private Sunny.UI.UIPanel opLastID;
        private Sunny.UI.UIPanel opLastCode;
        private Sunny.UI.UIPanel opLastActive;
        private Sunny.UI.UIGroupBox nextCartonGroup;
        private Sunny.UI.UITableLayoutPanel nextCartonContainer;
        private Sunny.UI.UILabel lblNextId;
        private Sunny.UI.UILabel lblNextCode;
        private Sunny.UI.UILabel lblNextStart;
        private Sunny.UI.UIPanel opnextID;
        private Sunny.UI.UIPanel opnextCode;
        private Sunny.UI.UIPanel opnextStart;
        private Sunny.UI.UIGroupBox warningGroup;
        private Sunny.UI.UIListBox opWarning;
        private Sunny.UI.UITabControl uiTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage2;
        private Sunny.UI.UIGroupBox debugGroup;
        private Sunny.UI.UITableLayoutPanel debugContainer;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UISymbolButton btnNextCarton;
        private Sunny.UI.UISymbolButton uiSymbolButton1;
        private Sunny.UI.UILabel uiLabel4;
        private Sunny.UI.UILabel uiLabel5;
        private Sunny.UI.UITextBox ipTest1;
        private Sunny.UI.UITextBox ipTest2;
        private Sunny.UI.UISymbolButton btnSend1;
        private Sunny.UI.UISymbolButton btnSend2;
        private Sunny.UI.UISymbolButton uiSymbolButton2;
        private System.IO.Ports.SerialPort serialPort1;
        private System.IO.Ports.SerialPort serialPort2;
        private SpT.Communications.TCP.TCPClient tcpClient1;
    }
}