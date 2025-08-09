namespace TestApp
{
    partial class CameraSimulatorForm
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
            this.uiGroupBox_ServerControl = new Sunny.UI.UIGroupBox();
            this.uiLabel_CameraSelect = new Sunny.UI.UILabel();
            this.uiComboBox_CameraSelect = new Sunny.UI.UIComboBox();
            this.uiLabel_Port = new Sunny.UI.UILabel();
            this.uiTextBox_Port = new Sunny.UI.UITextBox();
            this.uiButton_StartServer = new Sunny.UI.UIButton();
            this.uiButton_StopServer = new Sunny.UI.UIButton();
            this.uiButton_StartAllCameras = new Sunny.UI.UIButton();
            this.uiButton_StopAllCameras = new Sunny.UI.UIButton();
            this.uiLabel_ClientCount = new Sunny.UI.UILabel();
            this.uiGroupBox_SingleSend = new Sunny.UI.UIGroupBox();
            this.uiLabel_SingleCode = new Sunny.UI.UILabel();
            this.uiTextBox_SingleCode = new Sunny.UI.UITextBox();
            this.uiButton_SendSingle = new Sunny.UI.UIButton();
            this.uiGroupBox_ContinuousSend = new Sunny.UI.UIGroupBox();
            this.uiButton_LoadFile = new Sunny.UI.UIButton();
            this.uiButton_LoadCSV = new Sunny.UI.UIButton();
            this.uiButton_DataWizard = new Sunny.UI.UIButton();
            this.uiLabel_FileStatus = new Sunny.UI.UILabel();
            this.uiLabel_Count = new Sunny.UI.UILabel();
            this.uiIntegerUpDown_Count = new Sunny.UI.UIIntegerUpDown();
            this.uiLabel_Delay = new Sunny.UI.UILabel();
            this.uiIntegerUpDown_Delay = new Sunny.UI.UIIntegerUpDown();
            this.uiLabel_FromCode = new Sunny.UI.UILabel();
            this.uiTextBox_FromCode = new Sunny.UI.UITextBox();
            this.uiLabel_ToCode = new Sunny.UI.UILabel();
            this.uiTextBox_ToCode = new Sunny.UI.UITextBox();
            this.uiButton_SendRange = new Sunny.UI.UIButton();
            this.uiButton_StartContinuous = new Sunny.UI.UIButton();
            this.uiButton_StopContinuous = new Sunny.UI.UIButton();
            this.uiLabel_SendStatus = new Sunny.UI.UILabel();
            this.uiGroupBox_SQLite = new Sunny.UI.UIGroupBox();
            this.uiButton_LoadSQLite = new Sunny.UI.UIButton();
            this.uiLabel_DBStatus = new Sunny.UI.UILabel();
            this.uiLabel_Tables = new Sunny.UI.UILabel();
            this.uiComboBox_Tables = new Sunny.UI.UIComboBox();
            this.uiLabel_Columns = new Sunny.UI.UILabel();
            this.uiComboBox_Columns = new Sunny.UI.UIComboBox();
            this.uiButton_LoadColumnData = new Sunny.UI.UIButton();
            this.uiLabel_RecordCount = new Sunny.UI.UILabel();
            this.uiGroupBox_Log = new Sunny.UI.UIGroupBox();
            this.uiRichTextBox_Log = new Sunny.UI.UIRichTextBox();
            this.uiGroupBox_ServerControl.SuspendLayout();
            this.uiGroupBox_SingleSend.SuspendLayout();
            this.uiGroupBox_ContinuousSend.SuspendLayout();
            this.uiGroupBox_SQLite.SuspendLayout();
            this.uiGroupBox_Log.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiGroupBox_ServerControl
            // 
            this.uiGroupBox_ServerControl.Controls.Add(this.uiLabel_CameraSelect);
            this.uiGroupBox_ServerControl.Controls.Add(this.uiComboBox_CameraSelect);
            this.uiGroupBox_ServerControl.Controls.Add(this.uiLabel_Port);
            this.uiGroupBox_ServerControl.Controls.Add(this.uiTextBox_Port);
            this.uiGroupBox_ServerControl.Controls.Add(this.uiButton_StartServer);
            this.uiGroupBox_ServerControl.Controls.Add(this.uiButton_StopServer);
            this.uiGroupBox_ServerControl.Controls.Add(this.uiButton_StartAllCameras);
            this.uiGroupBox_ServerControl.Controls.Add(this.uiButton_StopAllCameras);
            this.uiGroupBox_ServerControl.Controls.Add(this.uiLabel_ClientCount);
            this.uiGroupBox_ServerControl.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.uiGroupBox_ServerControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.uiGroupBox_ServerControl.Location = new System.Drawing.Point(15, 15);
            this.uiGroupBox_ServerControl.Margin = new System.Windows.Forms.Padding(8);
            this.uiGroupBox_ServerControl.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox_ServerControl.Name = "uiGroupBox_ServerControl";
            this.uiGroupBox_ServerControl.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiGroupBox_ServerControl.Size = new System.Drawing.Size(320, 200);
            this.uiGroupBox_ServerControl.TabIndex = 0;
            this.uiGroupBox_ServerControl.Text = "🖥️ Điều khiển Camera Server";
            this.uiGroupBox_ServerControl.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiLabel_CameraSelect
            // 
            this.uiLabel_CameraSelect.AutoSize = true;
            this.uiLabel_CameraSelect.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.uiLabel_CameraSelect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_CameraSelect.Location = new System.Drawing.Point(10, 45);
            this.uiLabel_CameraSelect.Name = "uiLabel_CameraSelect";
            this.uiLabel_CameraSelect.Size = new System.Drawing.Size(60, 19);
            this.uiLabel_CameraSelect.TabIndex = 0;
            this.uiLabel_CameraSelect.Text = "Camera:";
            // 
            // uiComboBox_CameraSelect
            // 
            this.uiComboBox_CameraSelect.DataSource = null;
            this.uiComboBox_CameraSelect.FillColor = System.Drawing.Color.White;
            this.uiComboBox_CameraSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiComboBox_CameraSelect.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiComboBox_CameraSelect.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiComboBox_CameraSelect.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiComboBox_CameraSelect.Location = new System.Drawing.Point(80, 42);
            this.uiComboBox_CameraSelect.Margin = new System.Windows.Forms.Padding(5);
            this.uiComboBox_CameraSelect.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiComboBox_CameraSelect.Name = "uiComboBox_CameraSelect";
            this.uiComboBox_CameraSelect.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiComboBox_CameraSelect.Size = new System.Drawing.Size(220, 26);
            this.uiComboBox_CameraSelect.SymbolSize = 20;
            this.uiComboBox_CameraSelect.TabIndex = 1;
            this.uiComboBox_CameraSelect.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiComboBox_CameraSelect.Watermark = "";
            this.uiComboBox_CameraSelect.SelectedIndexChanged += new System.EventHandler(this.UiComboBox_CameraSelect_SelectedIndexChanged);
            // 
            // uiLabel_Port
            // 
            this.uiLabel_Port.AutoSize = true;
            this.uiLabel_Port.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Port.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Port.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uiLabel_Port.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_Port.Location = new System.Drawing.Point(10, 80);
            this.uiLabel_Port.Name = "uiLabel_Port";
            this.uiLabel_Port.Size = new System.Drawing.Size(40, 19);
            this.uiLabel_Port.TabIndex = 2;
            this.uiLabel_Port.Text = "Cổng:";
            // 
            // uiTextBox_Port
            // 
            this.uiTextBox_Port.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox_Port.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTextBox_Port.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiTextBox_Port.Location = new System.Drawing.Point(80, 77);
            this.uiTextBox_Port.Margin = new System.Windows.Forms.Padding(5);
            this.uiTextBox_Port.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox_Port.Name = "uiTextBox_Port";
            this.uiTextBox_Port.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.uiTextBox_Port.ShowText = false;
            this.uiTextBox_Port.Size = new System.Drawing.Size(80, 26);
            this.uiTextBox_Port.TabIndex = 3;
            this.uiTextBox_Port.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox_Port.Watermark = "";
            // 
            // uiButton_StartServer
            // 
            this.uiButton_StartServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_StartServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_StartServer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_StartServer.Location = new System.Drawing.Point(10, 115);
            this.uiButton_StartServer.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_StartServer.Name = "uiButton_StartServer";
            this.uiButton_StartServer.Radius = 8;
            this.uiButton_StartServer.Size = new System.Drawing.Size(90, 30);
            this.uiButton_StartServer.TabIndex = 4;
            this.uiButton_StartServer.Text = "Khởi động";
            this.uiButton_StartServer.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiButton_StopServer
            // 
            this.uiButton_StopServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_StopServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_StopServer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_StopServer.Location = new System.Drawing.Point(110, 115);
            this.uiButton_StopServer.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_StopServer.Name = "uiButton_StopServer";
            this.uiButton_StopServer.Radius = 8;
            this.uiButton_StopServer.Size = new System.Drawing.Size(90, 30);
            this.uiButton_StopServer.TabIndex = 5;
            this.uiButton_StopServer.Text = "Dừng";
            this.uiButton_StopServer.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiLabel_ClientCount
            // 
            this.uiLabel_ClientCount.AutoSize = true;
            this.uiLabel_ClientCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_ClientCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            // 
            // uiButton_StartAllCameras
            // 
            this.uiButton_StartAllCameras.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_StartAllCameras.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.uiButton_StartAllCameras.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.uiButton_StartAllCameras.Location = new System.Drawing.Point(10, 155);
            this.uiButton_StartAllCameras.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_StartAllCameras.Name = "uiButton_StartAllCameras";
            this.uiButton_StartAllCameras.Radius = 6;
            this.uiButton_StartAllCameras.Size = new System.Drawing.Size(90, 25);
            this.uiButton_StartAllCameras.TabIndex = 6;
            this.uiButton_StartAllCameras.Text = "Khởi động tất cả";
            this.uiButton_StartAllCameras.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiButton_StopAllCameras
            // 
            this.uiButton_StopAllCameras.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_StopAllCameras.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.uiButton_StopAllCameras.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.uiButton_StopAllCameras.Location = new System.Drawing.Point(110, 155);
            this.uiButton_StopAllCameras.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_StopAllCameras.Name = "uiButton_StopAllCameras";
            this.uiButton_StopAllCameras.Radius = 6;
            this.uiButton_StopAllCameras.Size = new System.Drawing.Size(90, 25);
            this.uiButton_StopAllCameras.TabIndex = 7;
            this.uiButton_StopAllCameras.Text = "Dừng tất cả";
            this.uiButton_StopAllCameras.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiLabel_ClientCount
            // 
            this.uiLabel_ClientCount.AutoSize = true;
            this.uiLabel_ClientCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_ClientCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_ClientCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.uiLabel_ClientCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.uiLabel_ClientCount.Location = new System.Drawing.Point(210, 125);
            this.uiLabel_ClientCount.Name = "uiLabel_ClientCount";
            this.uiLabel_ClientCount.Size = new System.Drawing.Size(100, 50);
            this.uiLabel_ClientCount.TabIndex = 8;
            this.uiLabel_ClientCount.Text = "Clients kết nối: 0";
            // 
            // uiGroupBox_SingleSend
            // 
            this.uiGroupBox_SingleSend.Controls.Add(this.uiLabel_SingleCode);
            this.uiGroupBox_SingleSend.Controls.Add(this.uiTextBox_SingleCode);
            this.uiGroupBox_SingleSend.Controls.Add(this.uiButton_SendSingle);
            this.uiGroupBox_SingleSend.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.uiGroupBox_SingleSend.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.uiGroupBox_SingleSend.Location = new System.Drawing.Point(350, 15);
            this.uiGroupBox_SingleSend.Margin = new System.Windows.Forms.Padding(8);
            this.uiGroupBox_SingleSend.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox_SingleSend.Name = "uiGroupBox_SingleSend";
            this.uiGroupBox_SingleSend.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiGroupBox_SingleSend.Size = new System.Drawing.Size(300, 90);
            this.uiGroupBox_SingleSend.TabIndex = 1;
            this.uiGroupBox_SingleSend.Text = "📤 Gửi mã đơn lẻ";
            this.uiGroupBox_SingleSend.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiLabel_SingleCode
            // 
            this.uiLabel_SingleCode.AutoSize = true;
            this.uiLabel_SingleCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_SingleCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_SingleCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uiLabel_SingleCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_SingleCode.Location = new System.Drawing.Point(10, 45);
            this.uiLabel_SingleCode.Name = "uiLabel_SingleCode";
            this.uiLabel_SingleCode.Size = new System.Drawing.Size(30, 19);
            this.uiLabel_SingleCode.TabIndex = 0;
            this.uiLabel_SingleCode.Text = "Mã:";
            // 
            // uiTextBox_SingleCode
            // 
            this.uiTextBox_SingleCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox_SingleCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTextBox_SingleCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiTextBox_SingleCode.Location = new System.Drawing.Point(50, 42);
            this.uiTextBox_SingleCode.Margin = new System.Windows.Forms.Padding(5);
            this.uiTextBox_SingleCode.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox_SingleCode.Name = "uiTextBox_SingleCode";
            this.uiTextBox_SingleCode.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.uiTextBox_SingleCode.ShowText = false;
            this.uiTextBox_SingleCode.Size = new System.Drawing.Size(150, 26);
            this.uiTextBox_SingleCode.TabIndex = 1;
            this.uiTextBox_SingleCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox_SingleCode.Watermark = "";
            // 
            // uiButton_SendSingle
            // 
            this.uiButton_SendSingle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_SendSingle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_SendSingle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_SendSingle.Location = new System.Drawing.Point(210, 40);
            this.uiButton_SendSingle.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_SendSingle.Name = "uiButton_SendSingle";
            this.uiButton_SendSingle.Radius = 8;
            this.uiButton_SendSingle.Size = new System.Drawing.Size(70, 30);
            this.uiButton_SendSingle.TabIndex = 2;
            this.uiButton_SendSingle.Text = "Gửi";
            this.uiButton_SendSingle.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiGroupBox_ContinuousSend
            // 
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiButton_LoadFile);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiButton_LoadCSV);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiButton_DataWizard);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiLabel_FileStatus);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiLabel_Count);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiIntegerUpDown_Count);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiLabel_Delay);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiIntegerUpDown_Delay);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiLabel_FromCode);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiTextBox_FromCode);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiLabel_ToCode);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiTextBox_ToCode);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiButton_SendRange);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiButton_StartContinuous);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiButton_StopContinuous);
            this.uiGroupBox_ContinuousSend.Controls.Add(this.uiLabel_SendStatus);
            this.uiGroupBox_ContinuousSend.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.uiGroupBox_ContinuousSend.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.uiGroupBox_ContinuousSend.Location = new System.Drawing.Point(15, 230);
            this.uiGroupBox_ContinuousSend.Margin = new System.Windows.Forms.Padding(8);
            this.uiGroupBox_ContinuousSend.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox_ContinuousSend.Name = "uiGroupBox_ContinuousSend";
            this.uiGroupBox_ContinuousSend.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiGroupBox_ContinuousSend.Size = new System.Drawing.Size(970, 180);
            this.uiGroupBox_ContinuousSend.TabIndex = 2;
            this.uiGroupBox_ContinuousSend.Text = "📦 Quản lý Dữ liệu & Gửi Tin";
            this.uiGroupBox_ContinuousSend.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiButton_LoadFile
            // 
            this.uiButton_LoadFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_LoadFile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_LoadFile.Location = new System.Drawing.Point(15, 45);
            this.uiButton_LoadFile.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_LoadFile.Name = "uiButton_LoadFile";
            this.uiButton_LoadFile.Radius = 8;
            this.uiButton_LoadFile.Size = new System.Drawing.Size(90, 32);
            this.uiButton_LoadFile.TabIndex = 0;
            this.uiButton_LoadFile.Text = "File TXT";
            this.uiButton_LoadFile.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiButton_LoadCSV
            // 
            this.uiButton_LoadCSV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_LoadCSV.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_LoadCSV.Location = new System.Drawing.Point(115, 45);
            this.uiButton_LoadCSV.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_LoadCSV.Name = "uiButton_LoadCSV";
            this.uiButton_LoadCSV.Radius = 8;
            this.uiButton_LoadCSV.Size = new System.Drawing.Size(90, 32);
            this.uiButton_LoadCSV.TabIndex = 1;
            this.uiButton_LoadCSV.Text = "File CSV";
            this.uiButton_LoadCSV.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiButton_DataWizard
            // 
            this.uiButton_DataWizard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_DataWizard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_DataWizard.Location = new System.Drawing.Point(215, 45);
            this.uiButton_DataWizard.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_DataWizard.Name = "uiButton_DataWizard";
            this.uiButton_DataWizard.Radius = 8;
            this.uiButton_DataWizard.Size = new System.Drawing.Size(110, 32);
            this.uiButton_DataWizard.TabIndex = 2;
            this.uiButton_DataWizard.Text = "Data Wizard";
            this.uiButton_DataWizard.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiLabel_FileStatus
            // 
            this.uiLabel_FileStatus.AutoSize = true;
            this.uiLabel_FileStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_FileStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_FileStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.uiLabel_FileStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.uiLabel_FileStatus.Location = new System.Drawing.Point(340, 52);
            this.uiLabel_FileStatus.Name = "uiLabel_FileStatus";
            this.uiLabel_FileStatus.Size = new System.Drawing.Size(300, 20);
            this.uiLabel_FileStatus.TabIndex = 3;
            this.uiLabel_FileStatus.Text = "Chưa chọn file";
            // 
            // uiLabel_Count
            // 
            this.uiLabel_Count.AutoSize = true;
            this.uiLabel_Count.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Count.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Count.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uiLabel_Count.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_Count.Location = new System.Drawing.Point(15, 90);
            this.uiLabel_Count.Name = "uiLabel_Count";
            this.uiLabel_Count.Size = new System.Drawing.Size(70, 19);
            this.uiLabel_Count.TabIndex = 4;
            this.uiLabel_Count.Text = "Số lượng:";
            // 
            // uiIntegerUpDown_Count
            // 
            this.uiIntegerUpDown_Count.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiIntegerUpDown_Count.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiIntegerUpDown_Count.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiIntegerUpDown_Count.Location = new System.Drawing.Point(90, 87);
            this.uiIntegerUpDown_Count.Margin = new System.Windows.Forms.Padding(5);
            this.uiIntegerUpDown_Count.Maximum = 10000D;
            this.uiIntegerUpDown_Count.Minimum = 1D;
            this.uiIntegerUpDown_Count.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiIntegerUpDown_Count.Name = "uiIntegerUpDown_Count";
            this.uiIntegerUpDown_Count.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.uiIntegerUpDown_Count.ShowText = false;
            this.uiIntegerUpDown_Count.Size = new System.Drawing.Size(70, 26);
            this.uiIntegerUpDown_Count.TabIndex = 5;
            this.uiIntegerUpDown_Count.Text = "10";
            this.uiIntegerUpDown_Count.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiIntegerUpDown_Count.Value = 10;
            // 
            // uiLabel_Delay
            // 
            this.uiLabel_Delay.AutoSize = true;
            this.uiLabel_Delay.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Delay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Delay.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uiLabel_Delay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_Delay.Location = new System.Drawing.Point(175, 90);
            this.uiLabel_Delay.Name = "uiLabel_Delay";
            this.uiLabel_Delay.Size = new System.Drawing.Size(75, 19);
            this.uiLabel_Delay.TabIndex = 6;
            this.uiLabel_Delay.Text = "Delay (ms):";
            // 
            // uiIntegerUpDown_Delay
            // 
            this.uiIntegerUpDown_Delay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiIntegerUpDown_Delay.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiIntegerUpDown_Delay.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiIntegerUpDown_Delay.Location = new System.Drawing.Point(255, 87);
            this.uiIntegerUpDown_Delay.Margin = new System.Windows.Forms.Padding(5);
            this.uiIntegerUpDown_Delay.Maximum = 60000D;
            this.uiIntegerUpDown_Delay.Minimum = 100D;
            this.uiIntegerUpDown_Delay.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiIntegerUpDown_Delay.Name = "uiIntegerUpDown_Delay";
            this.uiIntegerUpDown_Delay.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.uiIntegerUpDown_Delay.ShowText = false;
            this.uiIntegerUpDown_Delay.Size = new System.Drawing.Size(80, 26);
            this.uiIntegerUpDown_Delay.TabIndex = 7;
            this.uiIntegerUpDown_Delay.Text = "1000";
            this.uiIntegerUpDown_Delay.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiIntegerUpDown_Delay.Value = 1000;
            // 
            // uiLabel_FromCode
            // 
            this.uiLabel_FromCode.AutoSize = true;
            this.uiLabel_FromCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_FromCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_FromCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uiLabel_FromCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_FromCode.Location = new System.Drawing.Point(15, 125);
            this.uiLabel_FromCode.Name = "uiLabel_FromCode";
            this.uiLabel_FromCode.Size = new System.Drawing.Size(30, 19);
            this.uiLabel_FromCode.TabIndex = 8;
            this.uiLabel_FromCode.Text = "Từ:";
            // 
            // uiTextBox_FromCode
            // 
            this.uiTextBox_FromCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox_FromCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTextBox_FromCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiTextBox_FromCode.Location = new System.Drawing.Point(50, 122);
            this.uiTextBox_FromCode.Margin = new System.Windows.Forms.Padding(5);
            this.uiTextBox_FromCode.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox_FromCode.Name = "uiTextBox_FromCode";
            this.uiTextBox_FromCode.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.uiTextBox_FromCode.ShowText = false;
            this.uiTextBox_FromCode.Size = new System.Drawing.Size(90, 26);
            this.uiTextBox_FromCode.TabIndex = 9;
            this.uiTextBox_FromCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox_FromCode.Watermark = "";
            // 
            // uiLabel_ToCode
            // 
            this.uiLabel_ToCode.AutoSize = true;
            this.uiLabel_ToCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_ToCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_ToCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uiLabel_ToCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_ToCode.Location = new System.Drawing.Point(155, 125);
            this.uiLabel_ToCode.Name = "uiLabel_ToCode";
            this.uiLabel_ToCode.Size = new System.Drawing.Size(35, 19);
            this.uiLabel_ToCode.TabIndex = 10;
            this.uiLabel_ToCode.Text = "Đến:";
            // 
            // uiTextBox_ToCode
            // 
            this.uiTextBox_ToCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox_ToCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTextBox_ToCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiTextBox_ToCode.Location = new System.Drawing.Point(195, 122);
            this.uiTextBox_ToCode.Margin = new System.Windows.Forms.Padding(5);
            this.uiTextBox_ToCode.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox_ToCode.Name = "uiTextBox_ToCode";
            this.uiTextBox_ToCode.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.uiTextBox_ToCode.ShowText = false;
            this.uiTextBox_ToCode.Size = new System.Drawing.Size(90, 26);
            this.uiTextBox_ToCode.TabIndex = 11;
            this.uiTextBox_ToCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox_ToCode.Watermark = "";
            // 
            // uiButton_SendRange
            // 
            this.uiButton_SendRange.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_SendRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiButton_SendRange.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_SendRange.Location = new System.Drawing.Point(300, 120);
            this.uiButton_SendRange.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_SendRange.Name = "uiButton_SendRange";
            this.uiButton_SendRange.Radius = 8;
            this.uiButton_SendRange.Size = new System.Drawing.Size(100, 30);
            this.uiButton_SendRange.TabIndex = 12;
            this.uiButton_SendRange.Text = "Gửi dải mã";
            this.uiButton_SendRange.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiButton_StartContinuous
            // 
            this.uiButton_StartContinuous.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_StartContinuous.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_StartContinuous.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_StartContinuous.Location = new System.Drawing.Point(420, 87);
            this.uiButton_StartContinuous.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_StartContinuous.Name = "uiButton_StartContinuous";
            this.uiButton_StartContinuous.Radius = 8;
            this.uiButton_StartContinuous.Size = new System.Drawing.Size(80, 30);
            this.uiButton_StartContinuous.TabIndex = 13;
            this.uiButton_StartContinuous.Text = "Bắt đầu";
            this.uiButton_StartContinuous.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiButton_StopContinuous
            // 
            this.uiButton_StopContinuous.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_StopContinuous.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_StopContinuous.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_StopContinuous.Location = new System.Drawing.Point(510, 87);
            this.uiButton_StopContinuous.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_StopContinuous.Name = "uiButton_StopContinuous";
            this.uiButton_StopContinuous.Radius = 8;
            this.uiButton_StopContinuous.Size = new System.Drawing.Size(80, 30);
            this.uiButton_StopContinuous.TabIndex = 14;
            this.uiButton_StopContinuous.Text = "Dừng";
            this.uiButton_StopContinuous.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiLabel_SendStatus
            // 
            this.uiLabel_SendStatus.AutoSize = true;
            this.uiLabel_SendStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_SendStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_SendStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.uiLabel_SendStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.uiLabel_SendStatus.Location = new System.Drawing.Point(420, 125);
            this.uiLabel_SendStatus.Name = "uiLabel_SendStatus";
            this.uiLabel_SendStatus.Size = new System.Drawing.Size(300, 20);
            this.uiLabel_SendStatus.TabIndex = 15;
            this.uiLabel_SendStatus.Text = "Trạng thái: Sẵn sàng";
            // 
            // uiGroupBox_SQLite
            // 
            this.uiGroupBox_SQLite.Controls.Add(this.uiButton_LoadSQLite);
            this.uiGroupBox_SQLite.Controls.Add(this.uiLabel_DBStatus);
            this.uiGroupBox_SQLite.Controls.Add(this.uiLabel_Tables);
            this.uiGroupBox_SQLite.Controls.Add(this.uiComboBox_Tables);
            this.uiGroupBox_SQLite.Controls.Add(this.uiLabel_Columns);
            this.uiGroupBox_SQLite.Controls.Add(this.uiComboBox_Columns);
            this.uiGroupBox_SQLite.Controls.Add(this.uiButton_LoadColumnData);
            this.uiGroupBox_SQLite.Controls.Add(this.uiLabel_RecordCount);
            this.uiGroupBox_SQLite.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.uiGroupBox_SQLite.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.uiGroupBox_SQLite.Location = new System.Drawing.Point(665, 15);
            this.uiGroupBox_SQLite.Margin = new System.Windows.Forms.Padding(8);
            this.uiGroupBox_SQLite.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox_SQLite.Name = "uiGroupBox_SQLite";
            this.uiGroupBox_SQLite.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiGroupBox_SQLite.Size = new System.Drawing.Size(320, 200);
            this.uiGroupBox_SQLite.TabIndex = 3;
            this.uiGroupBox_SQLite.Text = "🗄️ SQLite Database";
            this.uiGroupBox_SQLite.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiButton_LoadSQLite
            // 
            this.uiButton_LoadSQLite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_LoadSQLite.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton_LoadSQLite.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.uiButton_LoadSQLite.Location = new System.Drawing.Point(10, 45);
            this.uiButton_LoadSQLite.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_LoadSQLite.Name = "uiButton_LoadSQLite";
            this.uiButton_LoadSQLite.Radius = 8;
            this.uiButton_LoadSQLite.Size = new System.Drawing.Size(90, 30);
            this.uiButton_LoadSQLite.TabIndex = 0;
            this.uiButton_LoadSQLite.Text = "Chọn DB";
            this.uiButton_LoadSQLite.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiLabel_DBStatus
            // 
            this.uiLabel_DBStatus.AutoSize = true;
            this.uiLabel_DBStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_DBStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_DBStatus.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.uiLabel_DBStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.uiLabel_DBStatus.Location = new System.Drawing.Point(110, 52);
            this.uiLabel_DBStatus.Name = "uiLabel_DBStatus";
            this.uiLabel_DBStatus.Size = new System.Drawing.Size(113, 20);
            this.uiLabel_DBStatus.TabIndex = 1;
            this.uiLabel_DBStatus.Text = "Chưa chọn DB";
            // 
            // uiLabel_Tables
            // 
            this.uiLabel_Tables.AutoSize = true;
            this.uiLabel_Tables.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Tables.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Tables.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uiLabel_Tables.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_Tables.Location = new System.Drawing.Point(10, 85);
            this.uiLabel_Tables.Name = "uiLabel_Tables";
            this.uiLabel_Tables.Size = new System.Drawing.Size(51, 20);
            this.uiLabel_Tables.TabIndex = 2;
            this.uiLabel_Tables.Text = "Bảng:";
            // 
            // uiComboBox_Tables
            // 
            this.uiComboBox_Tables.DataSource = null;
            this.uiComboBox_Tables.FillColor = System.Drawing.Color.White;
            this.uiComboBox_Tables.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiComboBox_Tables.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiComboBox_Tables.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiComboBox_Tables.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiComboBox_Tables.Location = new System.Drawing.Point(60, 82);
            this.uiComboBox_Tables.Margin = new System.Windows.Forms.Padding(5);
            this.uiComboBox_Tables.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiComboBox_Tables.Name = "uiComboBox_Tables";
            this.uiComboBox_Tables.Padding = new System.Windows.Forms.Padding(0, 0, 25, 2);
            this.uiComboBox_Tables.Size = new System.Drawing.Size(180, 26);
            this.uiComboBox_Tables.SymbolSize = 20;
            this.uiComboBox_Tables.TabIndex = 3;
            this.uiComboBox_Tables.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiComboBox_Tables.Watermark = "";
            this.uiComboBox_Tables.SelectedIndexChanged += new System.EventHandler(this.UiComboBox_Tables_SelectedIndexChanged);
            // 
            // uiLabel_Columns
            // 
            this.uiLabel_Columns.AutoSize = true;
            this.uiLabel_Columns.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_Columns.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_Columns.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uiLabel_Columns.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.uiLabel_Columns.Location = new System.Drawing.Point(10, 120);
            this.uiLabel_Columns.Name = "uiLabel_Columns";
            this.uiLabel_Columns.Size = new System.Drawing.Size(38, 20);
            this.uiLabel_Columns.TabIndex = 4;
            this.uiLabel_Columns.Text = "Cột:";
            // 
            // uiComboBox_Columns
            // 
            this.uiComboBox_Columns.DataSource = null;
            this.uiComboBox_Columns.FillColor = System.Drawing.Color.White;
            this.uiComboBox_Columns.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiComboBox_Columns.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiComboBox_Columns.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiComboBox_Columns.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.uiComboBox_Columns.Location = new System.Drawing.Point(50, 117);
            this.uiComboBox_Columns.Margin = new System.Windows.Forms.Padding(5);
            this.uiComboBox_Columns.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiComboBox_Columns.Name = "uiComboBox_Columns";
            this.uiComboBox_Columns.Padding = new System.Windows.Forms.Padding(0, 0, 25, 2);
            this.uiComboBox_Columns.Size = new System.Drawing.Size(180, 26);
            this.uiComboBox_Columns.SymbolSize = 20;
            this.uiComboBox_Columns.TabIndex = 5;
            this.uiComboBox_Columns.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiComboBox_Columns.Watermark = "";
            // 
            // uiButton_LoadColumnData
            // 
            this.uiButton_LoadColumnData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_LoadColumnData.Enabled = false;
            this.uiButton_LoadColumnData.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiButton_LoadColumnData.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.uiButton_LoadColumnData.Location = new System.Drawing.Point(245, 115);
            this.uiButton_LoadColumnData.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_LoadColumnData.Name = "uiButton_LoadColumnData";
            this.uiButton_LoadColumnData.Radius = 8;
            this.uiButton_LoadColumnData.Size = new System.Drawing.Size(65, 28);
            this.uiButton_LoadColumnData.TabIndex = 6;
            this.uiButton_LoadColumnData.Text = "Tải dữ liệu cột";
            this.uiButton_LoadColumnData.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // uiLabel_RecordCount
            // 
            this.uiLabel_RecordCount.AutoSize = true;
            this.uiLabel_RecordCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel_RecordCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel_RecordCount.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.uiLabel_RecordCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.uiLabel_RecordCount.Location = new System.Drawing.Point(10, 155);
            this.uiLabel_RecordCount.Name = "uiLabel_RecordCount";
            this.uiLabel_RecordCount.Size = new System.Drawing.Size(200, 20);
            this.uiLabel_RecordCount.TabIndex = 7;
            this.uiLabel_RecordCount.Text = "Chưa tải dữ liệu";
            // 
            // uiGroupBox_Log
            // 
            this.uiGroupBox_Log.Controls.Add(this.uiRichTextBox_Log);
            this.uiGroupBox_Log.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.uiGroupBox_Log.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.uiGroupBox_Log.Location = new System.Drawing.Point(15, 430);
            this.uiGroupBox_Log.Margin = new System.Windows.Forms.Padding(8);
            this.uiGroupBox_Log.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox_Log.Name = "uiGroupBox_Log";
            this.uiGroupBox_Log.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiGroupBox_Log.Size = new System.Drawing.Size(970, 300);
            this.uiGroupBox_Log.TabIndex = 4;
            this.uiGroupBox_Log.Text = "📄 Nhật ký Hệ thống";
            this.uiGroupBox_Log.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiRichTextBox_Log
            // 
            this.uiRichTextBox_Log.FillColor = System.Drawing.Color.White;
            this.uiRichTextBox_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiRichTextBox_Log.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.uiRichTextBox_Log.Font = new System.Drawing.Font("Consolas", 8F);
            this.uiRichTextBox_Log.Location = new System.Drawing.Point(10, 45);
            this.uiRichTextBox_Log.Margin = new System.Windows.Forms.Padding(8);
            this.uiRichTextBox_Log.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiRichTextBox_Log.Name = "uiRichTextBox_Log";
            this.uiRichTextBox_Log.Padding = new System.Windows.Forms.Padding(8);
            this.uiRichTextBox_Log.ReadOnly = true;
            this.uiRichTextBox_Log.ShowText = false;
            this.uiRichTextBox_Log.Size = new System.Drawing.Size(950, 240);
            this.uiRichTextBox_Log.TabIndex = 0;
            this.uiRichTextBox_Log.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CameraSimulatorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1000, 750);
            this.Controls.Add(this.uiGroupBox_ServerControl);
            this.Controls.Add(this.uiGroupBox_SingleSend);
            this.Controls.Add(this.uiGroupBox_ContinuousSend);
            this.Controls.Add(this.uiGroupBox_SQLite);
            this.Controls.Add(this.uiGroupBox_Log);
            this.Name = "CameraSimulatorForm";
            this.Text = "Camera TCP Server Simulator";
            this.uiGroupBox_ServerControl.ResumeLayout(false);
            this.uiGroupBox_ServerControl.PerformLayout();
            this.uiGroupBox_SingleSend.ResumeLayout(false);
            this.uiGroupBox_SingleSend.PerformLayout();
            this.uiGroupBox_ContinuousSend.ResumeLayout(false);
            this.uiGroupBox_ContinuousSend.PerformLayout();
            this.uiGroupBox_SQLite.ResumeLayout(false);
            this.uiGroupBox_SQLite.PerformLayout();
            this.uiGroupBox_Log.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIGroupBox uiGroupBox_ServerControl;
        private Sunny.UI.UILabel uiLabel_CameraSelect;
        private Sunny.UI.UIComboBox uiComboBox_CameraSelect;
        private Sunny.UI.UILabel uiLabel_Port;
        private Sunny.UI.UITextBox uiTextBox_Port;
        private Sunny.UI.UIButton uiButton_StartServer;
        private Sunny.UI.UIButton uiButton_StopServer;
        private Sunny.UI.UIButton uiButton_StartAllCameras;
        private Sunny.UI.UIButton uiButton_StopAllCameras;
        private Sunny.UI.UILabel uiLabel_ClientCount;
        
        private Sunny.UI.UIGroupBox uiGroupBox_SingleSend;
        private Sunny.UI.UILabel uiLabel_SingleCode;
        private Sunny.UI.UITextBox uiTextBox_SingleCode;
        private Sunny.UI.UIButton uiButton_SendSingle;
        
        private Sunny.UI.UIGroupBox uiGroupBox_ContinuousSend;
        private Sunny.UI.UIButton uiButton_LoadFile;
        private Sunny.UI.UIButton uiButton_LoadCSV;
        private Sunny.UI.UIButton uiButton_DataWizard;
        private Sunny.UI.UILabel uiLabel_FileStatus;
        private Sunny.UI.UILabel uiLabel_Count;
        private Sunny.UI.UIIntegerUpDown uiIntegerUpDown_Count;
        private Sunny.UI.UILabel uiLabel_Delay;
        private Sunny.UI.UIIntegerUpDown uiIntegerUpDown_Delay;
        private Sunny.UI.UILabel uiLabel_FromCode;
        private Sunny.UI.UITextBox uiTextBox_FromCode;
        private Sunny.UI.UILabel uiLabel_ToCode;
        private Sunny.UI.UITextBox uiTextBox_ToCode;
        private Sunny.UI.UIButton uiButton_SendRange;
        private Sunny.UI.UIButton uiButton_StartContinuous;
        private Sunny.UI.UIButton uiButton_StopContinuous;
        private Sunny.UI.UILabel uiLabel_SendStatus;
        
        private Sunny.UI.UIGroupBox uiGroupBox_SQLite;
        private Sunny.UI.UIButton uiButton_LoadSQLite;
        private Sunny.UI.UILabel uiLabel_DBStatus;
        private Sunny.UI.UILabel uiLabel_Tables;
        private Sunny.UI.UIComboBox uiComboBox_Tables;
        private Sunny.UI.UILabel uiLabel_Columns;
        private Sunny.UI.UIComboBox uiComboBox_Columns;
        private Sunny.UI.UIButton uiButton_LoadColumnData;
        private Sunny.UI.UILabel uiLabel_RecordCount;
        
        private Sunny.UI.UIGroupBox uiGroupBox_Log;
        private Sunny.UI.UIRichTextBox uiRichTextBox_Log;
    }
}