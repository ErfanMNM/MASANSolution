namespace Dialogs
{
    partial class Scaner
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
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiRichTextBox1 = new Sunny.UI.UIRichTextBox();
            this.btnOK = new Sunny.UI.UISymbolButton();
            this.btnCancel = new Sunny.UI.UISymbolButton();
            this.pnConnect = new Sunny.UI.UIPanel();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.ipCOM = new Sunny.UI.UIComboBox();
            this.uiTitlePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.ipCOM);
            this.uiTitlePanel1.Controls.Add(this.uiRichTextBox1);
            this.uiTitlePanel1.Controls.Add(this.btnOK);
            this.uiTitlePanel1.Controls.Add(this.btnCancel);
            this.uiTitlePanel1.Controls.Add(this.pnConnect);
            this.uiTitlePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(583, 183);
            this.uiTitlePanel1.TabIndex = 0;
            this.uiTitlePanel1.Text = "Quét Mã Bằng Tay Cầm";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiRichTextBox1
            // 
            this.uiRichTextBox1.FillColor = System.Drawing.Color.White;
            this.uiRichTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiRichTextBox1.Location = new System.Drawing.Point(4, 40);
            this.uiRichTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiRichTextBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiRichTextBox1.Name = "uiRichTextBox1";
            this.uiRichTextBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiRichTextBox1.ShowText = false;
            this.uiRichTextBox1.Size = new System.Drawing.Size(576, 79);
            this.uiRichTextBox1.TabIndex = 3;
            this.uiRichTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnOK.Location = new System.Drawing.Point(436, 127);
            this.btnOK.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(144, 51);
            this.btnOK.Symbol = 61452;
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Lưu lại";
            this.btnOK.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCancel.Location = new System.Drawing.Point(285, 127);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(145, 51);
            this.btnCancel.Symbol = 61453;
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Thoát";
            this.btnCancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnConnect
            // 
            this.pnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnConnect.Location = new System.Drawing.Point(4, 127);
            this.pnConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnConnect.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnConnect.Name = "pnConnect";
            this.pnConnect.Size = new System.Drawing.Size(105, 51);
            this.pnConnect.TabIndex = 0;
            this.pnConnect.Text = "Mất kết nối";
            this.pnConnect.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipCOM
            // 
            this.ipCOM.DataSource = null;
            this.ipCOM.FillColor = System.Drawing.Color.White;
            this.ipCOM.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipCOM.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipCOM.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipCOM.Location = new System.Drawing.Point(117, 129);
            this.ipCOM.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipCOM.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipCOM.Name = "ipCOM";
            this.ipCOM.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipCOM.Size = new System.Drawing.Size(161, 48);
            this.ipCOM.SymbolSize = 24;
            this.ipCOM.TabIndex = 4;
            this.ipCOM.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipCOM.Watermark = "";
            this.ipCOM.SelectedIndexChanged += new System.EventHandler(this.ipCOM_SelectedIndexChanged);
            // 
            // Scaner
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(583, 183);
            this.Controls.Add(this.uiTitlePanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Scaner";
            this.Text = "Scaner";
            this.Load += new System.EventHandler(this.Scaner_Load);
            this.uiTitlePanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UIPanel pnConnect;
        private Sunny.UI.UISymbolButton btnOK;
        private Sunny.UI.UISymbolButton btnCancel;
        private Sunny.UI.UIRichTextBox uiRichTextBox1;
        private System.IO.Ports.SerialPort serialPort1;
        private Sunny.UI.UIComboBox ipCOM;
    }
}