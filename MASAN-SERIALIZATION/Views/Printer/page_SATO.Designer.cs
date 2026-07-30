namespace MASAN_SERIALIZATION.Views.Printer
{
    partial class page_SATO
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
            this.ipContent = new Sunny.UI.UIRichTextBox();
            this.btnSend = new Sunny.UI.UIButton();
            this.opConsole = new Sunny.UI.UIListBox();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.ipIP = new Sunny.UI.UIIPTextBox();
            this.ipPort = new Sunny.UI.UINumPadTextBox();
            this.btnConnect = new Sunny.UI.UIButton();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipContent
            // 
            this.ipContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipContent.FillColor = System.Drawing.Color.White;
            this.ipContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipContent.Location = new System.Drawing.Point(4, 5);
            this.ipContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipContent.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipContent.Name = "ipContent";
            this.ipContent.Padding = new System.Windows.Forms.Padding(2);
            this.ipContent.ShowText = false;
            this.ipContent.Size = new System.Drawing.Size(913, 70);
            this.ipContent.TabIndex = 0;
            this.ipContent.Text = "uiRichTextBox1";
            this.ipContent.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSend
            // 
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSend.Location = new System.Drawing.Point(924, 3);
            this.btnSend.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(188, 74);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "uiButton1";
            this.btnSend.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // opConsole
            // 
            this.opConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opConsole.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opConsole.ItemSelectForeColor = System.Drawing.Color.White;
            this.opConsole.Location = new System.Drawing.Point(4, 214);
            this.opConsole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opConsole.MinimumSize = new System.Drawing.Size(1, 1);
            this.opConsole.Name = "opConsole";
            this.opConsole.Padding = new System.Windows.Forms.Padding(2);
            this.opConsole.ShowText = false;
            this.opConsole.Size = new System.Drawing.Size(1113, 436);
            this.opConsole.TabIndex = 2;
            this.opConsole.Text = "uiListBox1";
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel1.Controls.Add(this.opConsole, 0, 2);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel2, 0, 1);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel3, 0, 0);
            this.uiTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 3;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 123F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.24266F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.75734F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(1121, 655);
            this.uiTableLayoutPanel1.TabIndex = 3;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 2;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.6009F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.3991F));
            this.uiTableLayoutPanel2.Controls.Add(this.ipContent, 0, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.btnSend, 1, 0);
            this.uiTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(3, 126);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(1115, 80);
            this.uiTableLayoutPanel2.TabIndex = 3;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.ColumnCount = 2;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.51121F));
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.48879F));
            this.uiTableLayoutPanel3.Controls.Add(this.btnConnect, 1, 1);
            this.uiTableLayoutPanel3.Controls.Add(this.ipIP, 0, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.ipPort, 1, 0);
            this.uiTableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 2;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(1115, 117);
            this.uiTableLayoutPanel3.TabIndex = 4;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // ipIP
            // 
            this.ipIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipIP.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipIP.Location = new System.Drawing.Point(4, 5);
            this.ipIP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipIP.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipIP.Name = "ipIP";
            this.ipIP.Padding = new System.Windows.Forms.Padding(1);
            this.ipIP.ShowText = false;
            this.ipIP.Size = new System.Drawing.Size(912, 48);
            this.ipIP.TabIndex = 0;
            this.ipIP.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipPort
            // 
            this.ipPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipPort.FillColor = System.Drawing.Color.White;
            this.ipPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipPort.Location = new System.Drawing.Point(924, 5);
            this.ipPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipPort.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipPort.Name = "ipPort";
            this.ipPort.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipPort.Size = new System.Drawing.Size(187, 48);
            this.ipPort.SymbolSize = 24;
            this.ipPort.TabIndex = 1;
            this.ipPort.Text = "uiNumPadTextBox1";
            this.ipPort.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipPort.Watermark = "";
            // 
            // btnConnect
            // 
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnConnect.Location = new System.Drawing.Point(923, 61);
            this.btnConnect.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(189, 53);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "uiButton3";
            this.btnConnect.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // page_SATO
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1121, 655);
            this.Controls.Add(this.uiTableLayoutPanel1);
            this.Name = "page_SATO";
            this.Text = "page_SATO";
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIRichTextBox ipContent;
        private Sunny.UI.UIButton btnSend;
        private Sunny.UI.UIListBox opConsole;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UIButton btnConnect;
        private Sunny.UI.UIIPTextBox ipIP;
        private Sunny.UI.UINumPadTextBox ipPort;
    }
}