namespace MASAN_SERIALIZATION.Views.AWS
{
    partial class PAwsIot
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
            this.opNotiboardAndSend = new Sunny.UI.UIListBox();
            this.btnGetSendPending = new Sunny.UI.UISymbolButton();
            this.btnConnect = new Sunny.UI.UISymbolButton();
            this.btnSendFailed = new Sunny.UI.UISymbolButton();
            this.opRecive = new Sunny.UI.UIListBox();
            this.btnSend = new Sunny.UI.UISymbolButton();
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.uiGroupBox1 = new Sunny.UI.UIGroupBox();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiTitlePanel7 = new Sunny.UI.UITitlePanel();
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiTitlePanel2 = new Sunny.UI.UITitlePanel();
            this.uiTitlePanel3 = new Sunny.UI.UITitlePanel();
            this.opSent = new Sunny.UI.UIDigitalLabel();
            this.opRecived = new Sunny.UI.UIDigitalLabel();
            this.opSentFailed = new Sunny.UI.UIDigitalLabel();
            this.opWaiting = new Sunny.UI.UIDigitalLabel();
            this.uiGroupBox1.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTitlePanel7.SuspendLayout();
            this.uiTitlePanel1.SuspendLayout();
            this.uiTitlePanel2.SuspendLayout();
            this.uiTitlePanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // opNotiboardAndSend
            // 
            this.opNotiboardAndSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opNotiboardAndSend.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opNotiboardAndSend.ItemSelectForeColor = System.Drawing.Color.White;
            this.opNotiboardAndSend.Location = new System.Drawing.Point(13, 56);
            this.opNotiboardAndSend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opNotiboardAndSend.MinimumSize = new System.Drawing.Size(1, 1);
            this.opNotiboardAndSend.Name = "opNotiboardAndSend";
            this.opNotiboardAndSend.Padding = new System.Windows.Forms.Padding(2);
            this.opNotiboardAndSend.ShowText = false;
            this.opNotiboardAndSend.Size = new System.Drawing.Size(402, 329);
            this.opNotiboardAndSend.TabIndex = 0;
            this.opNotiboardAndSend.Text = "uiListBox1";
            this.opNotiboardAndSend.DoubleClick += new System.EventHandler(this.opNotiboardAndSend_DoubleClick);
            // 
            // btnGetSendPending
            // 
            this.btnGetSendPending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGetSendPending.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnGetSendPending.Location = new System.Drawing.Point(440, 626);
            this.btnGetSendPending.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnGetSendPending.Name = "btnGetSendPending";
            this.btnGetSendPending.Size = new System.Drawing.Size(227, 41);
            this.btnGetSendPending.TabIndex = 1;
            this.btnGetSendPending.Text = "Lấy mã có thể gửi";
            this.btnGetSendPending.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnGetSendPending.Click += new System.EventHandler(this.btnGetSendPending_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Enabled = false;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnConnect.Location = new System.Drawing.Point(673, 626);
            this.btnConnect.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(155, 41);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnSendFailed
            // 
            this.btnSendFailed.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSendFailed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSendFailed.Location = new System.Drawing.Point(277, 626);
            this.btnSendFailed.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSendFailed.Name = "btnSendFailed";
            this.btnSendFailed.Size = new System.Drawing.Size(157, 41);
            this.btnSendFailed.TabIndex = 3;
            this.btnSendFailed.Text = "Lấy mã gửi lỗi";
            this.btnSendFailed.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSendFailed.Click += new System.EventHandler(this.btnSendFailed_Click);
            // 
            // opRecive
            // 
            this.opRecive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opRecive.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opRecive.ItemSelectForeColor = System.Drawing.Color.White;
            this.opRecive.Location = new System.Drawing.Point(423, 56);
            this.opRecive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opRecive.MinimumSize = new System.Drawing.Size(1, 1);
            this.opRecive.Name = "opRecive";
            this.opRecive.Padding = new System.Windows.Forms.Padding(2);
            this.opRecive.ShowText = false;
            this.opRecive.Size = new System.Drawing.Size(405, 329);
            this.opRecive.TabIndex = 10;
            this.opRecive.Text = "uiListBox1";
            this.opRecive.DoubleClick += new System.EventHandler(this.opRecive_DoubleClick);
            // 
            // btnSend
            // 
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSend.Location = new System.Drawing.Point(114, 626);
            this.btnSend.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(157, 41);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "Gửi";
            this.btnSend.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // uiPanel1
            // 
            this.uiPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel1.Location = new System.Drawing.Point(13, 3);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Size = new System.Drawing.Size(814, 43);
            this.uiPanel1.TabIndex = 12;
            this.uiPanel1.Text = "Lịch sử gửi nhận";
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiGroupBox1
            // 
            this.uiGroupBox1.Controls.Add(this.uiTableLayoutPanel1);
            this.uiGroupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiGroupBox1.Location = new System.Drawing.Point(13, 395);
            this.uiGroupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox1.Name = "uiGroupBox1";
            this.uiGroupBox1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox1.Size = new System.Drawing.Size(814, 220);
            this.uiGroupBox1.TabIndex = 13;
            this.uiGroupBox1.Text = "Thống kê chi tiết gửi nhận MES";
            this.uiGroupBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 2;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.87624F));
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.12376F));
            this.uiTableLayoutPanel1.Controls.Add(this.uiTitlePanel3, 1, 1);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTitlePanel2, 0, 1);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTitlePanel1, 1, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTitlePanel7, 0, 0);
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(3, 35);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 2;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(808, 182);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiTitlePanel7
            // 
            this.uiTitlePanel7.Controls.Add(this.opSent);
            this.uiTitlePanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel7.Location = new System.Drawing.Point(4, 5);
            this.uiTitlePanel7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel7.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel7.Name = "uiTitlePanel7";
            this.uiTitlePanel7.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel7.ShowText = false;
            this.uiTitlePanel7.Size = new System.Drawing.Size(395, 81);
            this.uiTitlePanel7.TabIndex = 4;
            this.uiTitlePanel7.Text = "Đã gửi";
            this.uiTitlePanel7.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel7.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.uiTitlePanel7.Click += new System.EventHandler(this.uiTitlePanel7_Click);
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.opRecived);
            this.uiTitlePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(407, 5);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(397, 81);
            this.uiTitlePanel1.TabIndex = 5;
            this.uiTitlePanel1.Text = "Đã nhận kết quả";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            // 
            // uiTitlePanel2
            // 
            this.uiTitlePanel2.Controls.Add(this.opSentFailed);
            this.uiTitlePanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel2.Location = new System.Drawing.Point(4, 96);
            this.uiTitlePanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel2.Name = "uiTitlePanel2";
            this.uiTitlePanel2.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel2.ShowText = false;
            this.uiTitlePanel2.Size = new System.Drawing.Size(395, 81);
            this.uiTitlePanel2.TabIndex = 6;
            this.uiTitlePanel2.Text = "Gửi lỗi";
            this.uiTitlePanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel2.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            // 
            // uiTitlePanel3
            // 
            this.uiTitlePanel3.Controls.Add(this.opWaiting);
            this.uiTitlePanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel3.Location = new System.Drawing.Point(407, 96);
            this.uiTitlePanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel3.Name = "uiTitlePanel3";
            this.uiTitlePanel3.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel3.ShowText = false;
            this.uiTitlePanel3.Size = new System.Drawing.Size(397, 81);
            this.uiTitlePanel3.TabIndex = 7;
            this.uiTitlePanel3.Text = "Đang chờ gửi - Gửi lại";
            this.uiTitlePanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel3.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            // 
            // opSent
            // 
            this.opSent.BackColor = System.Drawing.Color.Transparent;
            this.opSent.DecimalPlaces = 0;
            this.opSent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opSent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opSent.ForeColor = System.Drawing.Color.Black;
            this.opSent.Location = new System.Drawing.Point(1, 35);
            this.opSent.MinimumSize = new System.Drawing.Size(1, 1);
            this.opSent.Name = "opSent";
            this.opSent.Size = new System.Drawing.Size(393, 45);
            this.opSent.TabIndex = 0;
            this.opSent.Text = "uiDigitalLabel1";
            // 
            // opRecived
            // 
            this.opRecived.BackColor = System.Drawing.Color.Transparent;
            this.opRecived.DecimalPlaces = 0;
            this.opRecived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opRecived.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opRecived.ForeColor = System.Drawing.Color.Black;
            this.opRecived.Location = new System.Drawing.Point(1, 35);
            this.opRecived.MinimumSize = new System.Drawing.Size(1, 1);
            this.opRecived.Name = "opRecived";
            this.opRecived.Size = new System.Drawing.Size(395, 45);
            this.opRecived.TabIndex = 1;
            this.opRecived.Text = "uiDigitalLabel1";
            // 
            // opSentFailed
            // 
            this.opSentFailed.BackColor = System.Drawing.Color.Transparent;
            this.opSentFailed.DecimalPlaces = 0;
            this.opSentFailed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opSentFailed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opSentFailed.ForeColor = System.Drawing.Color.Black;
            this.opSentFailed.Location = new System.Drawing.Point(1, 35);
            this.opSentFailed.MinimumSize = new System.Drawing.Size(1, 1);
            this.opSentFailed.Name = "opSentFailed";
            this.opSentFailed.Size = new System.Drawing.Size(393, 45);
            this.opSentFailed.TabIndex = 2;
            this.opSentFailed.Text = "uiDigitalLabel1";
            // 
            // opWaiting
            // 
            this.opWaiting.BackColor = System.Drawing.Color.Transparent;
            this.opWaiting.DecimalPlaces = 0;
            this.opWaiting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opWaiting.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opWaiting.ForeColor = System.Drawing.Color.Black;
            this.opWaiting.Location = new System.Drawing.Point(1, 35);
            this.opWaiting.MinimumSize = new System.Drawing.Size(1, 1);
            this.opWaiting.Name = "opWaiting";
            this.opWaiting.Size = new System.Drawing.Size(395, 45);
            this.opWaiting.TabIndex = 2;
            this.opWaiting.Text = "uiDigitalLabel1";
            // 
            // PAwsIot
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.uiGroupBox1);
            this.Controls.Add(this.uiPanel1);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.opRecive);
            this.Controls.Add(this.btnSendFailed);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnGetSendPending);
            this.Controls.Add(this.opNotiboardAndSend);
            this.Name = "PAwsIot";
            this.Symbol = 162325;
            this.Text = "AWS IOT";
            this.Initialize += new System.EventHandler(this.PAwsIot_Initialize);
            this.Load += new System.EventHandler(this.PAwsIot_Load);
            this.uiGroupBox1.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTitlePanel7.ResumeLayout(false);
            this.uiTitlePanel1.ResumeLayout(false);
            this.uiTitlePanel2.ResumeLayout(false);
            this.uiTitlePanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIListBox opNotiboardAndSend;
        private Sunny.UI.UISymbolButton btnGetSendPending;
        private Sunny.UI.UISymbolButton btnConnect;
        private Sunny.UI.UISymbolButton btnSendFailed;
        private Sunny.UI.UIListBox opRecive;
        private Sunny.UI.UISymbolButton btnSend;
        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIGroupBox uiGroupBox1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITitlePanel uiTitlePanel7;
        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UITitlePanel uiTitlePanel3;
        private Sunny.UI.UIDigitalLabel opWaiting;
        private Sunny.UI.UITitlePanel uiTitlePanel2;
        private Sunny.UI.UIDigitalLabel opSentFailed;
        private Sunny.UI.UIDigitalLabel opRecived;
        private Sunny.UI.UIDigitalLabel opSent;
    }
}