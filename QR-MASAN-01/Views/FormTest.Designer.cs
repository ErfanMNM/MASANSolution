namespace QR_MASAN_01.Views
{
    partial class FormTest
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
            this.uiListBox1 = new Sunny.UI.UIListBox();
            this.ipPub = new Sunny.UI.UITextBox();
            this.ipSub = new Sunny.UI.UITextBox();
            this.btnSub = new Sunny.UI.UIButton();
            this.btnPub = new Sunny.UI.UIButton();
            this.labelStatus = new Sunny.UI.UILabel();
            this.mqtT_Client1 = new SPMS1.MQTT.MQTT_Client(this.components);
            this.SuspendLayout();
            // 
            // uiListBox1
            // 
            this.uiListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiListBox1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiListBox1.ItemSelectForeColor = System.Drawing.Color.White;
            this.uiListBox1.Location = new System.Drawing.Point(13, 14);
            this.uiListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiListBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiListBox1.Name = "uiListBox1";
            this.uiListBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiListBox1.ShowText = false;
            this.uiListBox1.Size = new System.Drawing.Size(578, 201);
            this.uiListBox1.TabIndex = 0;
            this.uiListBox1.Text = "uiListBox1";
            // 
            // ipPub
            // 
            this.ipPub.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipPub.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipPub.Location = new System.Drawing.Point(13, 225);
            this.ipPub.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipPub.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipPub.Name = "ipPub";
            this.ipPub.Padding = new System.Windows.Forms.Padding(5);
            this.ipPub.ShowText = false;
            this.ipPub.Size = new System.Drawing.Size(477, 47);
            this.ipPub.TabIndex = 1;
            this.ipPub.Text = "uiTextBox1";
            this.ipPub.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipPub.Watermark = "";
            // 
            // ipSub
            // 
            this.ipSub.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipSub.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipSub.Location = new System.Drawing.Point(599, 14);
            this.ipSub.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipSub.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipSub.Name = "ipSub";
            this.ipSub.Padding = new System.Windows.Forms.Padding(5);
            this.ipSub.ShowText = false;
            this.ipSub.Size = new System.Drawing.Size(256, 52);
            this.ipSub.TabIndex = 2;
            this.ipSub.Text = "uiTextBox2";
            this.ipSub.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipSub.Watermark = "";
            // 
            // btnSub
            // 
            this.btnSub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSub.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSub.Location = new System.Drawing.Point(598, 74);
            this.btnSub.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(257, 44);
            this.btnSub.TabIndex = 3;
            this.btnSub.Text = "Đăng ký";
            this.btnSub.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // btnPub
            // 
            this.btnPub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPub.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnPub.Location = new System.Drawing.Point(497, 228);
            this.btnPub.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnPub.Name = "btnPub";
            this.btnPub.Size = new System.Drawing.Size(94, 44);
            this.btnPub.TabIndex = 4;
            this.btnPub.Text = "Gửi";
            this.btnPub.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPub.Click += new System.EventHandler(this.btnPub_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.labelStatus.Location = new System.Drawing.Point(12, 277);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(382, 60);
            this.labelStatus.TabIndex = 5;
            this.labelStatus.Text = "uiLabel1";
            // 
            // mqtT_Client1
            // 
            this.mqtT_Client1.brokerHost = "192.168.250.69";
            this.mqtT_Client1.brokerPort = 1883;
            this.mqtT_Client1.clientId = "379bca14-1238-49ea-aed5-6c1ca66682f9";
            this.mqtT_Client1.password = null;
            this.mqtT_Client1.username = null;
            // 
            // FormTest
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(868, 649);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnPub);
            this.Controls.Add(this.btnSub);
            this.Controls.Add(this.ipSub);
            this.Controls.Add(this.ipPub);
            this.Controls.Add(this.uiListBox1);
            this.Name = "FormTest";
            this.Text = "FormTest";
            this.Load += new System.EventHandler(this.FormTest_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private Sunny.UI.UIListBox uiListBox1;
        private Sunny.UI.UITextBox ipPub;
        private Sunny.UI.UITextBox ipSub;
        private Sunny.UI.UIButton btnSub;
        private Sunny.UI.UIButton btnPub;
        private SPMS1.MQTT.MQTT_Client mqtT_Client1;
        private Sunny.UI.UILabel labelStatus;
    }
}