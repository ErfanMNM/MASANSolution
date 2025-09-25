namespace MASAN_SERIALIZATION.Views.Database
{
    partial class CheckVIP
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ipCode = new Sunny.UI.UITextBox();
            this.btnfind = new Sunny.UI.UISymbolButton();
            this.uiDataGridView1 = new Sunny.UI.UIDataGridView();
            this.uiRichTextBox1 = new Sunny.UI.UIRichTextBox();
            this.btnDeleteCarton = new Sunny.UI.UISymbolButton();
            this.opCaseCode = new Sunny.UI.UIRichTextBox();
            this.opCodeInfo = new Sunny.UI.UIRichTextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.lblStatus = new Sunny.UI.UILabel();
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ipCode
            // 
            this.ipCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ipCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipCode.Location = new System.Drawing.Point(13, 14);
            this.ipCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipCode.MinimumSize = new System.Drawing.Size(1, 16);
            this.ipCode.Name = "ipCode";
            this.ipCode.Padding = new System.Windows.Forms.Padding(5);
            this.ipCode.ShowText = false;
            this.ipCode.Size = new System.Drawing.Size(690, 44);
            this.ipCode.TabIndex = 0;
            this.ipCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipCode.Watermark = "";
            // 
            // btnfind
            // 
            this.btnfind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnfind.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnfind.Location = new System.Drawing.Point(710, 14);
            this.btnfind.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnfind.Name = "btnfind";
            this.btnfind.Size = new System.Drawing.Size(118, 44);
            this.btnfind.Symbol = 57591;
            this.btnfind.TabIndex = 1;
            this.btnfind.Text = "Tìm";
            this.btnfind.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnfind.Click += new System.EventHandler(this.btnfind_Click);
            // 
            // uiDataGridView1
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.uiDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.uiDataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.uiDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.uiDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.uiDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.uiDataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.uiDataGridView1.EnableHeadersVisualStyles = false;
            this.uiDataGridView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiDataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.Location = new System.Drawing.Point(12, 200);
            this.uiDataGridView1.Name = "uiDataGridView1";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.uiDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.uiDataGridView1.SelectedIndex = -1;
            this.uiDataGridView1.Size = new System.Drawing.Size(815, 345);
            this.uiDataGridView1.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.TabIndex = 2;
            // 
            // uiRichTextBox1
            // 
            this.uiRichTextBox1.FillColor = System.Drawing.Color.White;
            this.uiRichTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiRichTextBox1.Location = new System.Drawing.Point(13, 553);
            this.uiRichTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiRichTextBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiRichTextBox1.Name = "uiRichTextBox1";
            this.uiRichTextBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiRichTextBox1.ShowText = false;
            this.uiRichTextBox1.Size = new System.Drawing.Size(815, 58);
            this.uiRichTextBox1.TabIndex = 3;
            this.uiRichTextBox1.Text = "Quét mã để tìm ra thông tin mã và danh sách mã trong thùng đó.\nBạn chỉ có thể hủy" +
    " cả 1 thùng chứ không thể hủy đơn lẻ.";
            this.uiRichTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDeleteCarton
            // 
            this.btnDeleteCarton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteCarton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnDeleteCarton.Location = new System.Drawing.Point(661, 619);
            this.btnDeleteCarton.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDeleteCarton.Name = "btnDeleteCarton";
            this.btnDeleteCarton.Size = new System.Drawing.Size(167, 43);
            this.btnDeleteCarton.Symbol = 362189;
            this.btnDeleteCarton.TabIndex = 4;
            this.btnDeleteCarton.Text = "Hủy Thùng";
            this.btnDeleteCarton.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDeleteCarton.Click += new System.EventHandler(this.btnDeleteCarton_Click);
            // 
            // opCaseCode
            // 
            this.opCaseCode.FillColor = System.Drawing.Color.White;
            this.opCaseCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opCaseCode.Location = new System.Drawing.Point(13, 134);
            this.opCaseCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opCaseCode.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCaseCode.Name = "opCaseCode";
            this.opCaseCode.Padding = new System.Windows.Forms.Padding(2);
            this.opCaseCode.ShowText = false;
            this.opCaseCode.Size = new System.Drawing.Size(815, 58);
            this.opCaseCode.TabIndex = 4;
            this.opCaseCode.Text = "Quét mã để tìm ra thông tin mã và danh sách mã trong thùng đó";
            this.opCaseCode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opCodeInfo
            // 
            this.opCodeInfo.FillColor = System.Drawing.Color.White;
            this.opCodeInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opCodeInfo.Location = new System.Drawing.Point(13, 68);
            this.opCodeInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opCodeInfo.MinimumSize = new System.Drawing.Size(1, 1);
            this.opCodeInfo.Name = "opCodeInfo";
            this.opCodeInfo.Padding = new System.Windows.Forms.Padding(2);
            this.opCodeInfo.ShowText = false;
            this.opCodeInfo.Size = new System.Drawing.Size(815, 58);
            this.opCodeInfo.TabIndex = 4;
            this.opCodeInfo.Text = "Quét mã để tìm ra thông tin mã và danh sách mã trong thùng đó";
            this.opCodeInfo.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.lblStatus.Location = new System.Drawing.Point(12, 644);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(125, 18);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "⚡ Đang kết nối...";
            // 
            // CheckVIP
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.opCodeInfo);
            this.Controls.Add(this.opCaseCode);
            this.Controls.Add(this.btnDeleteCarton);
            this.Controls.Add(this.uiRichTextBox1);
            this.Controls.Add(this.uiDataGridView1);
            this.Controls.Add(this.btnfind);
            this.Controls.Add(this.ipCode);
            this.Name = "CheckVIP";
            this.Symbol = 561487;
            this.Text = "Kiểm Tra";
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UITextBox ipCode;
        private Sunny.UI.UISymbolButton btnfind;
        private Sunny.UI.UIDataGridView uiDataGridView1;
        private Sunny.UI.UIRichTextBox uiRichTextBox1;
        private Sunny.UI.UISymbolButton btnDeleteCarton;
        private Sunny.UI.UIRichTextBox opCaseCode;
        private Sunny.UI.UIRichTextBox opCodeInfo;
        private System.IO.Ports.SerialPort serialPort1;
        private Sunny.UI.UILabel lblStatus;
    }
}