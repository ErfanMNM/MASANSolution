namespace MASAN_SERIALIZATION.Views.AWS
{
    partial class PAws
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
            this.mainContainer = new Sunny.UI.UITableLayoutPanel();
            this.consoleGroup = new Sunny.UI.UIGroupBox();
            this.consoleContainer = new Sunny.UI.UITableLayoutPanel();
            this.lblSendConsole = new Sunny.UI.UILabel();
            this.lblReceiveConsole = new Sunny.UI.UILabel();
            this.opConsole = new Sunny.UI.UIListBox();
            this.opReciveConsole = new Sunny.UI.UIListBox();
            this.controlGroup = new Sunny.UI.UIGroupBox();
            this.controlContainer = new Sunny.UI.UITableLayoutPanel();
            this.dataInputGroup = new Sunny.UI.UIGroupBox();
            this.dataInputContainer = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.ipmesageID = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel2 = new Sunny.UI.UIPanel();
            this.iporderNo = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel3 = new Sunny.UI.UIPanel();
            this.ipuniqueCode = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel4 = new Sunny.UI.UIPanel();
            this.ipcartonCode = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel5 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel5 = new Sunny.UI.UIPanel();
            this.ipstatus = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel6 = new Sunny.UI.UITableLayoutPanel();
            this.uiPanel6 = new Sunny.UI.UIPanel();
            this.uiDatetimePicker1 = new Sunny.UI.UIDatetimePicker();
            this.buttonContainer = new Sunny.UI.UITableLayoutPanel();
            this.btnConnect = new Sunny.UI.UISymbolButton();
            this.btnGetData = new Sunny.UI.UISymbolButton();
            this.uiSymbolButton6 = new Sunny.UI.UISymbolButton();
            this.uiSymbolButton5 = new Sunny.UI.UISymbolButton();
            this.btnSendOne = new Sunny.UI.UISymbolButton();
            this.uiSymbolButton4 = new Sunny.UI.UISymbolButton();
            this.mainContainer.SuspendLayout();
            this.consoleGroup.SuspendLayout();
            this.consoleContainer.SuspendLayout();
            this.controlGroup.SuspendLayout();
            this.controlContainer.SuspendLayout();
            this.dataInputGroup.SuspendLayout();
            this.dataInputContainer.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.uiTableLayoutPanel4.SuspendLayout();
            this.uiTableLayoutPanel5.SuspendLayout();
            this.uiTableLayoutPanel6.SuspendLayout();
            this.buttonContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.ColumnCount = 1;
            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainContainer.Controls.Add(this.consoleGroup, 0, 0);
            this.mainContainer.Controls.Add(this.controlGroup, 0, 1);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.RowCount = 2;
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.mainContainer.Size = new System.Drawing.Size(824, 635);
            this.mainContainer.TabIndex = 0;
            this.mainContainer.TagString = null;
            // 
            // consoleGroup
            // 
            this.consoleGroup.Controls.Add(this.consoleContainer);
            this.consoleGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.consoleGroup.Location = new System.Drawing.Point(4, 5);
            this.consoleGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.consoleGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.consoleGroup.Name = "consoleGroup";
            this.consoleGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.consoleGroup.Size = new System.Drawing.Size(816, 402);
            this.consoleGroup.TabIndex = 0;
            this.consoleGroup.Text = "📊 Console Logs";
            this.consoleGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // consoleContainer
            // 
            this.consoleContainer.ColumnCount = 2;
            this.consoleContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.consoleContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.consoleContainer.Controls.Add(this.lblSendConsole, 0, 0);
            this.consoleContainer.Controls.Add(this.lblReceiveConsole, 1, 0);
            this.consoleContainer.Controls.Add(this.opConsole, 0, 1);
            this.consoleContainer.Controls.Add(this.opReciveConsole, 1, 1);
            this.consoleContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleContainer.Location = new System.Drawing.Point(3, 32);
            this.consoleContainer.Name = "consoleContainer";
            this.consoleContainer.RowCount = 2;
            this.consoleContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.consoleContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.consoleContainer.Size = new System.Drawing.Size(810, 367);
            this.consoleContainer.TabIndex = 0;
            this.consoleContainer.TagString = null;
            // 
            // lblSendConsole
            // 
            this.lblSendConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSendConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblSendConsole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblSendConsole.Location = new System.Drawing.Point(3, 0);
            this.lblSendConsole.Name = "lblSendConsole";
            this.lblSendConsole.Size = new System.Drawing.Size(399, 30);
            this.lblSendConsole.TabIndex = 0;
            this.lblSendConsole.Text = "📤 Send Console";
            this.lblSendConsole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblReceiveConsole
            // 
            this.lblReceiveConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReceiveConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblReceiveConsole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblReceiveConsole.Location = new System.Drawing.Point(408, 0);
            this.lblReceiveConsole.Name = "lblReceiveConsole";
            this.lblReceiveConsole.Size = new System.Drawing.Size(399, 30);
            this.lblReceiveConsole.TabIndex = 1;
            this.lblReceiveConsole.Text = "📥 Receive Console";
            this.lblReceiveConsole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // opConsole
            // 
            this.opConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.opConsole.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opConsole.ItemSelectForeColor = System.Drawing.Color.White;
            this.opConsole.Location = new System.Drawing.Point(4, 35);
            this.opConsole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opConsole.MinimumSize = new System.Drawing.Size(1, 1);
            this.opConsole.Name = "opConsole";
            this.opConsole.Padding = new System.Windows.Forms.Padding(2);
            this.opConsole.ShowText = false;
            this.opConsole.Size = new System.Drawing.Size(397, 327);
            this.opConsole.TabIndex = 2;
            this.opConsole.Text = "uiListBox1";
            // 
            // opReciveConsole
            // 
            this.opReciveConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opReciveConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.opReciveConsole.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opReciveConsole.ItemSelectForeColor = System.Drawing.Color.White;
            this.opReciveConsole.Location = new System.Drawing.Point(409, 35);
            this.opReciveConsole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opReciveConsole.MinimumSize = new System.Drawing.Size(1, 1);
            this.opReciveConsole.Name = "opReciveConsole";
            this.opReciveConsole.Padding = new System.Windows.Forms.Padding(2);
            this.opReciveConsole.ShowText = false;
            this.opReciveConsole.Size = new System.Drawing.Size(397, 327);
            this.opReciveConsole.TabIndex = 3;
            this.opReciveConsole.Text = "uiListBox1";
            this.opReciveConsole.DoubleClick += new System.EventHandler(this.opReciveConsole_DoubleClick);
            // 
            // controlGroup
            // 
            this.controlGroup.Controls.Add(this.controlContainer);
            this.controlGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.controlGroup.Location = new System.Drawing.Point(4, 417);
            this.controlGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.controlGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.controlGroup.Name = "controlGroup";
            this.controlGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.controlGroup.Size = new System.Drawing.Size(816, 213);
            this.controlGroup.TabIndex = 1;
            this.controlGroup.Text = "⚙️ Controls";
            this.controlGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // controlContainer
            // 
            this.controlContainer.ColumnCount = 2;
            this.controlContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.controlContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.controlContainer.Controls.Add(this.dataInputGroup, 0, 0);
            this.controlContainer.Controls.Add(this.buttonContainer, 1, 0);
            this.controlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlContainer.Location = new System.Drawing.Point(3, 32);
            this.controlContainer.Name = "controlContainer";
            this.controlContainer.RowCount = 1;
            this.controlContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.controlContainer.Size = new System.Drawing.Size(810, 178);
            this.controlContainer.TabIndex = 0;
            this.controlContainer.TagString = null;
            // 
            // dataInputGroup
            // 
            this.dataInputGroup.Controls.Add(this.dataInputContainer);
            this.dataInputGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataInputGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.dataInputGroup.Location = new System.Drawing.Point(4, 5);
            this.dataInputGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataInputGroup.MinimumSize = new System.Drawing.Size(1, 1);
            this.dataInputGroup.Name = "dataInputGroup";
            this.dataInputGroup.Padding = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.dataInputGroup.Size = new System.Drawing.Size(559, 168);
            this.dataInputGroup.TabIndex = 0;
            this.dataInputGroup.Text = "📝 Data Input";
            this.dataInputGroup.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataInputContainer
            // 
            this.dataInputContainer.ColumnCount = 3;
            this.dataInputContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.dataInputContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.dataInputContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.dataInputContainer.Controls.Add(this.uiTableLayoutPanel1, 0, 0);
            this.dataInputContainer.Controls.Add(this.uiTableLayoutPanel2, 1, 0);
            this.dataInputContainer.Controls.Add(this.uiTableLayoutPanel3, 2, 0);
            this.dataInputContainer.Controls.Add(this.uiTableLayoutPanel4, 0, 1);
            this.dataInputContainer.Controls.Add(this.uiTableLayoutPanel5, 1, 1);
            this.dataInputContainer.Controls.Add(this.uiTableLayoutPanel6, 2, 1);
            this.dataInputContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataInputContainer.Location = new System.Drawing.Point(3, 32);
            this.dataInputContainer.Name = "dataInputContainer";
            this.dataInputContainer.RowCount = 2;
            this.dataInputContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dataInputContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dataInputContainer.Size = new System.Drawing.Size(553, 133);
            this.dataInputContainer.TabIndex = 0;
            this.dataInputContainer.TagString = null;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.Controls.Add(this.uiPanel1, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.ipmesageID, 0, 1);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 2;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(178, 60);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiPanel1
            // 
            this.uiPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel1.Location = new System.Drawing.Point(2, 2);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Size = new System.Drawing.Size(174, 21);
            this.uiPanel1.TabIndex = 0;
            this.uiPanel1.Text = "🏷️ Message ID";
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ipmesageID
            // 
            this.ipmesageID.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipmesageID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipmesageID.DoubleValue = 6868D;
            this.ipmesageID.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ipmesageID.IntValue = 6868;
            this.ipmesageID.Location = new System.Drawing.Point(2, 27);
            this.ipmesageID.Margin = new System.Windows.Forms.Padding(2);
            this.ipmesageID.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipmesageID.Name = "ipmesageID";
            this.ipmesageID.Padding = new System.Windows.Forms.Padding(5);
            this.ipmesageID.ShowText = false;
            this.ipmesageID.Size = new System.Drawing.Size(174, 31);
            this.ipmesageID.TabIndex = 1;
            this.ipmesageID.Text = "6868";
            this.ipmesageID.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipmesageID.Watermark = "Nhập Message ID...";
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 1;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel2.Controls.Add(this.uiPanel2, 0, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.iporderNo, 0, 1);
            this.uiTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(187, 3);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 2;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(178, 60);
            this.uiTableLayoutPanel2.TabIndex = 1;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // uiPanel2
            // 
            this.uiPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel2.Location = new System.Drawing.Point(2, 2);
            this.uiPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel2.Name = "uiPanel2";
            this.uiPanel2.Size = new System.Drawing.Size(174, 21);
            this.uiPanel2.TabIndex = 0;
            this.uiPanel2.Text = "📋 Order No";
            this.uiPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // iporderNo
            // 
            this.iporderNo.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.iporderNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iporderNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.iporderNo.Location = new System.Drawing.Point(2, 27);
            this.iporderNo.Margin = new System.Windows.Forms.Padding(2);
            this.iporderNo.MinimumSize = new System.Drawing.Size(1, 16);
            this.iporderNo.Name = "iporderNo";
            this.iporderNo.Padding = new System.Windows.Forms.Padding(5);
            this.iporderNo.ShowText = false;
            this.iporderNo.Size = new System.Drawing.Size(174, 31);
            this.iporderNo.TabIndex = 1;
            this.iporderNo.Text = "PO_678";
            this.iporderNo.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.iporderNo.Watermark = "Nhập Order No...";
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.ColumnCount = 1;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel3.Controls.Add(this.uiPanel3, 0, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.ipuniqueCode, 0, 1);
            this.uiTableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(371, 3);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 2;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(179, 60);
            this.uiTableLayoutPanel3.TabIndex = 2;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // uiPanel3
            // 
            this.uiPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel3.Location = new System.Drawing.Point(2, 2);
            this.uiPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel3.Name = "uiPanel3";
            this.uiPanel3.Size = new System.Drawing.Size(175, 21);
            this.uiPanel3.TabIndex = 0;
            this.uiPanel3.Text = "🔑 Unique Code";
            this.uiPanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ipuniqueCode
            // 
            this.ipuniqueCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipuniqueCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipuniqueCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ipuniqueCode.Location = new System.Drawing.Point(2, 27);
            this.ipuniqueCode.Margin = new System.Windows.Forms.Padding(2);
            this.ipuniqueCode.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipuniqueCode.Name = "ipuniqueCode";
            this.ipuniqueCode.Padding = new System.Windows.Forms.Padding(5);
            this.ipuniqueCode.Radius = 1;
            this.ipuniqueCode.ShowText = false;
            this.ipuniqueCode.Size = new System.Drawing.Size(175, 31);
            this.ipuniqueCode.TabIndex = 1;
            this.ipuniqueCode.Text = "PO_678";
            this.ipuniqueCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipuniqueCode.Watermark = "Nhập Unique Code...";
            // 
            // uiTableLayoutPanel4
            // 
            this.uiTableLayoutPanel4.ColumnCount = 1;
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel4.Controls.Add(this.uiPanel4, 0, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.ipcartonCode, 0, 1);
            this.uiTableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel4.Location = new System.Drawing.Point(3, 69);
            this.uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            this.uiTableLayoutPanel4.RowCount = 2;
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel4.Size = new System.Drawing.Size(178, 61);
            this.uiTableLayoutPanel4.TabIndex = 3;
            this.uiTableLayoutPanel4.TagString = null;
            // 
            // uiPanel4
            // 
            this.uiPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel4.Location = new System.Drawing.Point(2, 2);
            this.uiPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel4.Name = "uiPanel4";
            this.uiPanel4.Size = new System.Drawing.Size(174, 21);
            this.uiPanel4.TabIndex = 0;
            this.uiPanel4.Text = "📦 Carton Code";
            this.uiPanel4.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ipcartonCode
            // 
            this.ipcartonCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipcartonCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipcartonCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ipcartonCode.Location = new System.Drawing.Point(2, 27);
            this.ipcartonCode.Margin = new System.Windows.Forms.Padding(2);
            this.ipcartonCode.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipcartonCode.Name = "ipcartonCode";
            this.ipcartonCode.Padding = new System.Windows.Forms.Padding(5);
            this.ipcartonCode.ShowText = false;
            this.ipcartonCode.Size = new System.Drawing.Size(174, 32);
            this.ipcartonCode.TabIndex = 1;
            this.ipcartonCode.Text = "PO_678";
            this.ipcartonCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipcartonCode.Watermark = "Nhập Carton Code...";
            // 
            // uiTableLayoutPanel5
            // 
            this.uiTableLayoutPanel5.ColumnCount = 1;
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel5.Controls.Add(this.uiPanel5, 0, 0);
            this.uiTableLayoutPanel5.Controls.Add(this.ipstatus, 0, 1);
            this.uiTableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel5.Location = new System.Drawing.Point(187, 69);
            this.uiTableLayoutPanel5.Name = "uiTableLayoutPanel5";
            this.uiTableLayoutPanel5.RowCount = 2;
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel5.Size = new System.Drawing.Size(178, 61);
            this.uiTableLayoutPanel5.TabIndex = 4;
            this.uiTableLayoutPanel5.TagString = null;
            // 
            // uiPanel5
            // 
            this.uiPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel5.Location = new System.Drawing.Point(2, 2);
            this.uiPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel5.Name = "uiPanel5";
            this.uiPanel5.Size = new System.Drawing.Size(174, 21);
            this.uiPanel5.TabIndex = 0;
            this.uiPanel5.Text = "ℹ️ Status";
            this.uiPanel5.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ipstatus
            // 
            this.ipstatus.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipstatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ipstatus.Location = new System.Drawing.Point(2, 27);
            this.ipstatus.Margin = new System.Windows.Forms.Padding(2);
            this.ipstatus.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipstatus.Name = "ipstatus";
            this.ipstatus.Padding = new System.Windows.Forms.Padding(5);
            this.ipstatus.ShowText = false;
            this.ipstatus.Size = new System.Drawing.Size(174, 32);
            this.ipstatus.TabIndex = 1;
            this.ipstatus.Text = "PO_678";
            this.ipstatus.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipstatus.Watermark = "Nhập Status...";
            // 
            // uiTableLayoutPanel6
            // 
            this.uiTableLayoutPanel6.ColumnCount = 1;
            this.uiTableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel6.Controls.Add(this.uiPanel6, 0, 0);
            this.uiTableLayoutPanel6.Controls.Add(this.uiDatetimePicker1, 0, 1);
            this.uiTableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel6.Location = new System.Drawing.Point(371, 69);
            this.uiTableLayoutPanel6.Name = "uiTableLayoutPanel6";
            this.uiTableLayoutPanel6.RowCount = 2;
            this.uiTableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.uiTableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel6.Size = new System.Drawing.Size(179, 61);
            this.uiTableLayoutPanel6.TabIndex = 5;
            this.uiTableLayoutPanel6.TagString = null;
            // 
            // uiPanel6
            // 
            this.uiPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.uiPanel6.Location = new System.Drawing.Point(2, 2);
            this.uiPanel6.Margin = new System.Windows.Forms.Padding(2);
            this.uiPanel6.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel6.Name = "uiPanel6";
            this.uiPanel6.Size = new System.Drawing.Size(175, 21);
            this.uiPanel6.TabIndex = 0;
            this.uiPanel6.Text = "📅 Production Date";
            this.uiPanel6.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiDatetimePicker1
            // 
            this.uiDatetimePicker1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiDatetimePicker1.FillColor = System.Drawing.Color.White;
            this.uiDatetimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiDatetimePicker1.Location = new System.Drawing.Point(2, 27);
            this.uiDatetimePicker1.Margin = new System.Windows.Forms.Padding(2);
            this.uiDatetimePicker1.MaxLength = 19;
            this.uiDatetimePicker1.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiDatetimePicker1.Name = "uiDatetimePicker1";
            this.uiDatetimePicker1.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiDatetimePicker1.Size = new System.Drawing.Size(175, 32);
            this.uiDatetimePicker1.SymbolDropDown = 61555;
            this.uiDatetimePicker1.SymbolNormal = 61555;
            this.uiDatetimePicker1.SymbolSize = 20;
            this.uiDatetimePicker1.TabIndex = 1;
            this.uiDatetimePicker1.Text = "2025-07-30 10:07:11";
            this.uiDatetimePicker1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiDatetimePicker1.Value = new System.DateTime(2025, 7, 30, 10, 7, 11, 0);
            this.uiDatetimePicker1.Watermark = "Chọn ngày sản xuất...";
            // 
            // buttonContainer
            // 
            this.buttonContainer.ColumnCount = 1;
            this.buttonContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonContainer.Controls.Add(this.btnConnect, 0, 0);
            this.buttonContainer.Controls.Add(this.btnGetData, 0, 1);
            this.buttonContainer.Controls.Add(this.uiSymbolButton6, 0, 2);
            this.buttonContainer.Controls.Add(this.uiSymbolButton5, 0, 3);
            this.buttonContainer.Controls.Add(this.btnSendOne, 0, 4);
            this.buttonContainer.Controls.Add(this.uiSymbolButton4, 0, 5);
            this.buttonContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonContainer.Location = new System.Drawing.Point(570, 3);
            this.buttonContainer.Name = "buttonContainer";
            this.buttonContainer.RowCount = 6;
            this.buttonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.buttonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.buttonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.buttonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.buttonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.buttonContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.68F));
            this.buttonContainer.Size = new System.Drawing.Size(237, 172);
            this.buttonContainer.TabIndex = 1;
            this.buttonContainer.TagString = null;
            // 
            // btnConnect
            // 
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnConnect.Location = new System.Drawing.Point(3, 3);
            this.btnConnect.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(231, 22);
            this.btnConnect.Symbol = 57387;
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnGetData
            // 
            this.btnGetData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGetData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGetData.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnGetData.Location = new System.Drawing.Point(3, 31);
            this.btnGetData.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(231, 22);
            this.btnGetData.Symbol = 57439;
            this.btnGetData.TabIndex = 1;
            this.btnGetData.Text = "Lấy dữ liệu";
            this.btnGetData.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // uiSymbolButton6
            // 
            this.uiSymbolButton6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSymbolButton6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.uiSymbolButton6.Location = new System.Drawing.Point(3, 59);
            this.uiSymbolButton6.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton6.Name = "uiSymbolButton6";
            this.uiSymbolButton6.Size = new System.Drawing.Size(231, 22);
            this.uiSymbolButton6.Symbol = 61530;
            this.uiSymbolButton6.TabIndex = 2;
            this.uiSymbolButton6.Text = "Lấy thông tin";
            this.uiSymbolButton6.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiSymbolButton5
            // 
            this.uiSymbolButton5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSymbolButton5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.uiSymbolButton5.Location = new System.Drawing.Point(3, 87);
            this.uiSymbolButton5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton5.Name = "uiSymbolButton5";
            this.uiSymbolButton5.Size = new System.Drawing.Size(231, 22);
            this.uiSymbolButton5.Symbol = 61956;
            this.uiSymbolButton5.TabIndex = 3;
            this.uiSymbolButton5.Text = "Gửi test";
            this.uiSymbolButton5.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnSendOne
            // 
            this.btnSendOne.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSendOne.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSendOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnSendOne.Location = new System.Drawing.Point(3, 115);
            this.btnSendOne.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSendOne.Name = "btnSendOne";
            this.btnSendOne.Size = new System.Drawing.Size(231, 22);
            this.btnSendOne.Symbol = 61544;
            this.btnSendOne.TabIndex = 4;
            this.btnSendOne.Text = "Gửi 1";
            this.btnSendOne.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSendOne.Click += new System.EventHandler(this.btnSendOne_Click);
            // 
            // uiSymbolButton4
            // 
            this.uiSymbolButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSymbolButton4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.uiSymbolButton4.Location = new System.Drawing.Point(3, 143);
            this.uiSymbolButton4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton4.Name = "uiSymbolButton4";
            this.uiSymbolButton4.Size = new System.Drawing.Size(231, 26);
            this.uiSymbolButton4.Symbol = 61547;
            this.uiSymbolButton4.TabIndex = 5;
            this.uiSymbolButton4.Text = "Gửi hết";
            this.uiSymbolButton4.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // PAws
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(824, 635);
            this.Controls.Add(this.mainContainer);
            this.Name = "PAws";
            this.Symbol = 162325;
            this.Text = "AWS IoT";
            this.mainContainer.ResumeLayout(false);
            this.consoleGroup.ResumeLayout(false);
            this.consoleContainer.ResumeLayout(false);
            this.controlGroup.ResumeLayout(false);
            this.controlContainer.ResumeLayout(false);
            this.dataInputGroup.ResumeLayout(false);
            this.dataInputContainer.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.uiTableLayoutPanel4.ResumeLayout(false);
            this.uiTableLayoutPanel5.ResumeLayout(false);
            this.uiTableLayoutPanel6.ResumeLayout(false);
            this.buttonContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITableLayoutPanel mainContainer;
        private Sunny.UI.UIGroupBox consoleGroup;
        private Sunny.UI.UITableLayoutPanel consoleContainer;
        private Sunny.UI.UILabel lblSendConsole;
        private Sunny.UI.UILabel lblReceiveConsole;
        private Sunny.UI.UIListBox opConsole;
        private Sunny.UI.UIListBox opReciveConsole;
        private Sunny.UI.UIGroupBox controlGroup;
        private Sunny.UI.UITableLayoutPanel controlContainer;
        private Sunny.UI.UIGroupBox dataInputGroup;
        private Sunny.UI.UITableLayoutPanel dataInputContainer;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UITextBox ipmesageID;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UITextBox iporderNo;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UITextBox ipuniqueCode;
        private Sunny.UI.UIPanel uiPanel3;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UITextBox ipcartonCode;
        private Sunny.UI.UIPanel uiPanel4;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel5;
        private Sunny.UI.UITextBox ipstatus;
        private Sunny.UI.UIPanel uiPanel5;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel6;
        private Sunny.UI.UIPanel uiPanel6;
        private Sunny.UI.UIDatetimePicker uiDatetimePicker1;
        private Sunny.UI.UITableLayoutPanel buttonContainer;
        private Sunny.UI.UISymbolButton btnConnect;
        private Sunny.UI.UISymbolButton btnGetData;
        private Sunny.UI.UISymbolButton uiSymbolButton6;
        private Sunny.UI.UISymbolButton uiSymbolButton5;
        private Sunny.UI.UISymbolButton btnSendOne;
        private Sunny.UI.UISymbolButton uiSymbolButton4;
    }
}