namespace TestApp
{
    partial class COMV
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbVirtualPort = new Sunny.UI.UIComboBox();
            this.cmbRealPort = new Sunny.UI.UIComboBox();
            this.btnConnect = new Sunny.UI.UIButton();
            this.btnDisconnect = new Sunny.UI.UIButton();
            this.lblVirtualPort = new Sunny.UI.UILabel();
            this.lblRealPort = new Sunny.UI.UILabel();
            this.lblStatus = new Sunny.UI.UILabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtDataReceived = new Sunny.UI.UIRichTextBox();
            this.txtDataToSend = new Sunny.UI.UITextBox();
            this.btnSendData = new Sunny.UI.UIButton();
            this.btnClearLog = new Sunny.UI.UIButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkAutoReconnect = new Sunny.UI.UICheckBox();
            this.numReconnectInterval = new Sunny.UI.UIIntegerUpDown();
            this.lblReconnectInterval = new Sunny.UI.UILabel();
            this.progressBar = new Sunny.UI.UIProcessBar();
            this.btnCreateVirtual = new Sunny.UI.UIButton();
            this.btnRefreshPorts = new Sunny.UI.UIButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.lblRealPort);
            this.groupBox1.Controls.Add(this.lblVirtualPort);
            this.groupBox1.Controls.Add(this.btnDisconnect);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.cmbRealPort);
            this.groupBox1.Controls.Add(this.cmbVirtualPort);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(894, 150);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "COM Bridge - Kết nối 2 ứng dụng";
            // 
            // cmbVirtualPort
            // 
            this.cmbVirtualPort.DataSource = null;
            this.cmbVirtualPort.FillColor = System.Drawing.Color.White;
            this.cmbVirtualPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cmbVirtualPort.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cmbVirtualPort.Location = new System.Drawing.Point(120, 30);
            this.cmbVirtualPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbVirtualPort.MinimumSize = new System.Drawing.Size(63, 0);
            this.cmbVirtualPort.Name = "cmbVirtualPort";
            this.cmbVirtualPort.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cmbVirtualPort.Size = new System.Drawing.Size(150, 29);
            this.cmbVirtualPort.SymbolSize = 24;
            this.cmbVirtualPort.TabIndex = 0;
            this.cmbVirtualPort.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmbVirtualPort.Watermark = "";
            // 
            // cmbRealPort
            // 
            this.cmbRealPort.DataSource = null;
            this.cmbRealPort.FillColor = System.Drawing.Color.White;
            this.cmbRealPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cmbRealPort.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cmbRealPort.Location = new System.Drawing.Point(400, 30);
            this.cmbRealPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbRealPort.MinimumSize = new System.Drawing.Size(63, 0);
            this.cmbRealPort.Name = "cmbRealPort";
            this.cmbRealPort.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cmbRealPort.Size = new System.Drawing.Size(150, 29);
            this.cmbRealPort.SymbolSize = 24;
            this.cmbRealPort.TabIndex = 1;
            this.cmbRealPort.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmbRealPort.Watermark = "";
            // 
            // btnConnect
            // 
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnConnect.Location = new System.Drawing.Point(600, 25);
            this.btnConnect.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 35);

            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDisconnect.Location = new System.Drawing.Point(720, 25);
            this.btnDisconnect.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(100, 35);

            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "Ngắt kết nối";
            this.btnDisconnect.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // lblVirtualPort
            // 
            this.lblVirtualPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblVirtualPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblVirtualPort.Location = new System.Drawing.Point(20, 30);
            this.lblVirtualPort.Name = "lblVirtualPort";
            this.lblVirtualPort.Size = new System.Drawing.Size(100, 23);
            this.lblVirtualPort.TabIndex = 4;
            this.lblVirtualPort.Text = "App 1 Port:";
            this.lblVirtualPort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRealPort
            // 
            this.lblRealPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblRealPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblRealPort.Location = new System.Drawing.Point(300, 30);
            this.lblRealPort.Name = "lblRealPort";
            this.lblRealPort.Size = new System.Drawing.Size(100, 23);
            this.lblRealPort.TabIndex = 5;
            this.lblRealPort.Text = "App 2 Port:";
            this.lblRealPort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(20, 70);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(400, 23);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Trạng thái: Chưa kết nối";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.progressBar.Location = new System.Drawing.Point(20, 100);
            this.progressBar.MinimumSize = new System.Drawing.Size(70, 1);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(800, 29);
            this.progressBar.TabIndex = 7;
            this.progressBar.Text = "0%";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnClearLog);
            this.groupBox2.Controls.Add(this.btnSendData);
            this.groupBox2.Controls.Add(this.txtDataToSend);
            this.groupBox2.Controls.Add(this.txtDataReceived);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.groupBox2.Location = new System.Drawing.Point(12, 180);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(594, 400);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dữ liệu truyền nhận";
            // 
            // txtDataReceived
            // 
            this.txtDataReceived.FillColor = System.Drawing.Color.White;
            this.txtDataReceived.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtDataReceived.Location = new System.Drawing.Point(20, 30);
            this.txtDataReceived.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDataReceived.MinimumSize = new System.Drawing.Size(1, 1);
            this.txtDataReceived.Name = "txtDataReceived";
            this.txtDataReceived.Padding = new System.Windows.Forms.Padding(2);
            this.txtDataReceived.ReadOnly = true;
            this.txtDataReceived.ShowText = false;
            this.txtDataReceived.Size = new System.Drawing.Size(554, 300);
            this.txtDataReceived.TabIndex = 0;
            this.txtDataReceived.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtDataToSend
            // 
            this.txtDataToSend.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDataToSend.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtDataToSend.Location = new System.Drawing.Point(20, 340);
            this.txtDataToSend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDataToSend.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtDataToSend.Name = "txtDataToSend";
            this.txtDataToSend.Padding = new System.Windows.Forms.Padding(5);
            this.txtDataToSend.ShowText = false;
            this.txtDataToSend.Size = new System.Drawing.Size(350, 29);
            this.txtDataToSend.TabIndex = 1;
            this.txtDataToSend.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtDataToSend.Watermark = "Nhập dữ liệu test để gửi đến App 1...";
            this.txtDataToSend.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDataToSend_KeyPress);
            // 
            // btnSendData
            // 
            this.btnSendData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSendData.Enabled = false;
            this.btnSendData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSendData.Location = new System.Drawing.Point(380, 340);
            this.btnSendData.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSendData.Name = "btnSendData";
            this.btnSendData.Size = new System.Drawing.Size(80, 29);

            this.btnSendData.TabIndex = 2;
            this.btnSendData.Text = "Gửi";
            this.btnSendData.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSendData.Click += new System.EventHandler(this.btnSendData_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClearLog.Location = new System.Drawing.Point(470, 340);
            this.btnClearLog.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(80, 29);

            this.btnClearLog.TabIndex = 3;
            this.btnClearLog.Text = "Xóa";
            this.btnClearLog.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnRefreshPorts);
            this.groupBox3.Controls.Add(this.btnCreateVirtual);
            this.groupBox3.Controls.Add(this.lblReconnectInterval);
            this.groupBox3.Controls.Add(this.numReconnectInterval);
            this.groupBox3.Controls.Add(this.chkAutoReconnect);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(620, 180);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(286, 400);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Cài đặt";
            // 
            // chkAutoReconnect
            // 
            this.chkAutoReconnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoReconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.chkAutoReconnect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkAutoReconnect.Location = new System.Drawing.Point(20, 30);
            this.chkAutoReconnect.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkAutoReconnect.Name = "chkAutoReconnect";
            this.chkAutoReconnect.Size = new System.Drawing.Size(200, 29);
            this.chkAutoReconnect.TabIndex = 0;
            this.chkAutoReconnect.Text = "Tự động kết nối lại";
            // 
            // numReconnectInterval
            // 
            this.numReconnectInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.numReconnectInterval.Location = new System.Drawing.Point(20, 90);
            this.numReconnectInterval.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numReconnectInterval.Maximum = 60;
            this.numReconnectInterval.Minimum = 1;
            this.numReconnectInterval.MinimumSize = new System.Drawing.Size(100, 0);
            this.numReconnectInterval.Name = "numReconnectInterval";
            this.numReconnectInterval.ShowText = false;
            this.numReconnectInterval.Size = new System.Drawing.Size(150, 29);
            this.numReconnectInterval.TabIndex = 1;
            this.numReconnectInterval.Text = "uiIntegerUpDown1";
            this.numReconnectInterval.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.numReconnectInterval.Value = 5;
            // 
            // lblReconnectInterval
            // 
            this.lblReconnectInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblReconnectInterval.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblReconnectInterval.Location = new System.Drawing.Point(20, 65);
            this.lblReconnectInterval.Name = "lblReconnectInterval";
            this.lblReconnectInterval.Size = new System.Drawing.Size(200, 23);
            this.lblReconnectInterval.TabIndex = 2;
            this.lblReconnectInterval.Text = "Thời gian kết nối lại (giây):";
            this.lblReconnectInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCreateVirtual
            // 
            this.btnCreateVirtual.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreateVirtual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCreateVirtual.Location = new System.Drawing.Point(20, 130);
            this.btnCreateVirtual.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCreateVirtual.Name = "btnCreateVirtual";
            this.btnCreateVirtual.Size = new System.Drawing.Size(240, 35);
            this.btnCreateVirtual.TabIndex = 3;
            this.btnCreateVirtual.Text = "🔧 Hướng dẫn tạo Virtual COM";
            this.btnCreateVirtual.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCreateVirtual.Click += new System.EventHandler(this.btnCreateVirtual_Click);
            // 
            // btnRefreshPorts
            // 
            this.btnRefreshPorts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshPorts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRefreshPorts.Location = new System.Drawing.Point(20, 175);
            this.btnRefreshPorts.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRefreshPorts.Name = "btnRefreshPorts";
            this.btnRefreshPorts.Size = new System.Drawing.Size(240, 35);
            this.btnRefreshPorts.TabIndex = 4;
            this.btnRefreshPorts.Text = "🔄 Refresh COM Ports";
            this.btnRefreshPorts.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRefreshPorts.Click += new System.EventHandler(this.btnRefreshPorts_Click);
            // 
            // COMV
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(918, 606);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "COMV";
            this.Text = "COM Virtual Bridge";
            this.Load += new System.EventHandler(this.COMV_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Sunny.UI.UIComboBox cmbVirtualPort;
        private Sunny.UI.UIComboBox cmbRealPort;
        private Sunny.UI.UIButton btnConnect;
        private Sunny.UI.UIButton btnDisconnect;
        private Sunny.UI.UILabel lblVirtualPort;
        private Sunny.UI.UILabel lblRealPort;
        private Sunny.UI.UILabel lblStatus;
        private System.Windows.Forms.GroupBox groupBox2;
        private Sunny.UI.UIRichTextBox txtDataReceived;
        private Sunny.UI.UITextBox txtDataToSend;
        private Sunny.UI.UIButton btnSendData;
        private Sunny.UI.UIButton btnClearLog;
        private System.Windows.Forms.GroupBox groupBox3;
        private Sunny.UI.UICheckBox chkAutoReconnect;
        private Sunny.UI.UIIntegerUpDown numReconnectInterval;
        private Sunny.UI.UILabel lblReconnectInterval;
        private Sunny.UI.UIProcessBar progressBar;
        private Sunny.UI.UIButton btnCreateVirtual;
        private Sunny.UI.UIButton btnRefreshPorts;
    }
}