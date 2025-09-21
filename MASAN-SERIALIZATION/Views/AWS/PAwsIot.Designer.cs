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
            this.uiSymbolButton1 = new Sunny.UI.UISymbolButton();
            this.opErr = new Sunny.UI.UIListBox();
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
            this.opNotiboardAndSend.Size = new System.Drawing.Size(402, 340);
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
            this.opRecive.Size = new System.Drawing.Size(405, 340);
            this.opRecive.TabIndex = 10;
            this.opRecive.Text = "uiListBox1";
            this.opRecive.DoubleClick += new System.EventHandler(this.opRecive_DoubleClick);
            // 
            // btnSend
            // 
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSend.Location = new System.Drawing.Point(13, 626);
            this.btnSend.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(76, 41);
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
            // uiSymbolButton1
            // 
            this.uiSymbolButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiSymbolButton1.Location = new System.Drawing.Point(95, 626);
            this.uiSymbolButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton1.Name = "uiSymbolButton1";
            this.uiSymbolButton1.Size = new System.Drawing.Size(176, 41);
            this.uiSymbolButton1.TabIndex = 14;
            this.uiSymbolButton1.Text = "Lấy Mã R Pending";
            this.uiSymbolButton1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiSymbolButton1.Click += new System.EventHandler(this.uiSymbolButton1_Click);
            // 
            // opErr
            // 
            this.opErr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opErr.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opErr.ItemSelectForeColor = System.Drawing.Color.White;
            this.opErr.Location = new System.Drawing.Point(13, 406);
            this.opErr.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opErr.MinimumSize = new System.Drawing.Size(1, 1);
            this.opErr.Name = "opErr";
            this.opErr.Padding = new System.Windows.Forms.Padding(2);
            this.opErr.ShowText = false;
            this.opErr.Size = new System.Drawing.Size(815, 153);
            this.opErr.TabIndex = 11;
            this.opErr.Text = "uiListBox1";
            // 
            // PAwsIot
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.opErr);
            this.Controls.Add(this.uiSymbolButton1);
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
        private Sunny.UI.UISymbolButton uiSymbolButton1;
        private Sunny.UI.UIListBox opErr;
    }
}