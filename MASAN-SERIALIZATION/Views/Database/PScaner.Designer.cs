namespace MASAN_SERIALIZATION.Views.Database
{
    partial class PScaner
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
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.txtScannedCode = new Sunny.UI.UITextBox();
            this.lblScannedCode = new Sunny.UI.UILabel();
            this.uiPanel2 = new Sunny.UI.UIPanel();
            this.btnSearch = new Sunny.UI.UIButton();
            this.txtSearchManual = new Sunny.UI.UITextBox();
            this.lblSearchManual = new Sunny.UI.UILabel();
            this.uiStatusPanel = new Sunny.UI.UIPanel();
            this.lblStatus = new Sunny.UI.UILabel();
            this.lblSearchResult = new Sunny.UI.UILabel();
            this.dgvSearchResult = new Sunny.UI.UIDataGridView();
            this.uiPanel3 = new Sunny.UI.UIPanel();
            this.uiHeaderPanel = new Sunny.UI.UIHeaderButton();
            this.uiPanel1.SuspendLayout();
            this.uiPanel2.SuspendLayout();
            this.uiStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchResult)).BeginInit();
            this.uiPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiPanel1
            // 
            this.uiPanel1.Controls.Add(this.txtScannedCode);
            this.uiPanel1.Controls.Add(this.lblScannedCode);
            this.uiPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.uiPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel1.Location = new System.Drawing.Point(0, 50);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.uiPanel1.Size = new System.Drawing.Size(901, 85);
            this.uiPanel1.TabIndex = 1;
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtScannedCode
            // 
            this.txtScannedCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtScannedCode.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtScannedCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.txtScannedCode.Location = new System.Drawing.Point(15, 45);
            this.txtScannedCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtScannedCode.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtScannedCode.Name = "txtScannedCode";
            this.txtScannedCode.Padding = new System.Windows.Forms.Padding(5);
            this.txtScannedCode.ReadOnly = true;
            this.txtScannedCode.ShowText = false;
            this.txtScannedCode.Size = new System.Drawing.Size(500, 30);
            this.txtScannedCode.TabIndex = 1;
            this.txtScannedCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtScannedCode.Watermark = "Mã sản phẩm sẽ hiển thị ở đây...";
            // 
            // lblScannedCode
            // 
            this.lblScannedCode.AutoSize = true;
            this.lblScannedCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblScannedCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.lblScannedCode.Location = new System.Drawing.Point(15, 15);
            this.lblScannedCode.Name = "lblScannedCode";
            this.lblScannedCode.Size = new System.Drawing.Size(134, 20);
            this.lblScannedCode.TabIndex = 0;
            this.lblScannedCode.Text = "📱 Mã vừa quét:";
            // 
            // uiPanel2
            // 
            this.uiPanel2.Controls.Add(this.btnSearch);
            this.uiPanel2.Controls.Add(this.txtSearchManual);
            this.uiPanel2.Controls.Add(this.lblSearchManual);
            this.uiPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.uiPanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel2.Location = new System.Drawing.Point(0, 135);
            this.uiPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel2.Name = "uiPanel2";
            this.uiPanel2.Padding = new System.Windows.Forms.Padding(10);
            this.uiPanel2.Size = new System.Drawing.Size(901, 85);
            this.uiPanel2.TabIndex = 2;
            this.uiPanel2.Text = null;
            this.uiPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSearch
            // 
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Location = new System.Drawing.Point(430, 45);
            this.btnSearch.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 30);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "🔍 Tìm kiếm";
            this.btnSearch.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearchManual
            // 
            this.txtSearchManual.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearchManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtSearchManual.Location = new System.Drawing.Point(15, 45);
            this.txtSearchManual.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearchManual.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtSearchManual.Name = "txtSearchManual";
            this.txtSearchManual.Padding = new System.Windows.Forms.Padding(5);
            this.txtSearchManual.ShowText = false;
            this.txtSearchManual.Size = new System.Drawing.Size(400, 30);
            this.txtSearchManual.TabIndex = 1;
            this.txtSearchManual.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSearchManual.Watermark = "Nhập mã để tìm kiếm...";
            this.txtSearchManual.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchManual_KeyDown);
            // 
            // lblSearchManual
            // 
            this.lblSearchManual.AutoSize = true;
            this.lblSearchManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblSearchManual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.lblSearchManual.Location = new System.Drawing.Point(15, 15);
            this.lblSearchManual.Name = "lblSearchManual";
            this.lblSearchManual.Size = new System.Drawing.Size(181, 20);
            this.lblSearchManual.TabIndex = 0;
            this.lblSearchManual.Text = "⌨️ Tìm kiếm thủ công:";
            // 
            // uiStatusPanel
            // 
            this.uiStatusPanel.Controls.Add(this.lblStatus);
            this.uiStatusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.uiStatusPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiStatusPanel.Location = new System.Drawing.Point(0, 518);
            this.uiStatusPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiStatusPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiStatusPanel.Name = "uiStatusPanel";
            this.uiStatusPanel.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.uiStatusPanel.Radius = 0;
            this.uiStatusPanel.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Top;
            this.uiStatusPanel.Size = new System.Drawing.Size(901, 30);
            this.uiStatusPanel.TabIndex = 4;
            this.uiStatusPanel.Text = null;
            this.uiStatusPanel.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.lblStatus.Location = new System.Drawing.Point(10, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(164, 18);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "⚡ Sẵn sàng tìm kiếm...";
            // 
            // lblSearchResult
            // 
            this.lblSearchResult.AutoSize = true;
            this.lblSearchResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblSearchResult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.lblSearchResult.Location = new System.Drawing.Point(15, 15);
            this.lblSearchResult.Name = "lblSearchResult";
            this.lblSearchResult.Size = new System.Drawing.Size(141, 20);
            this.lblSearchResult.TabIndex = 0;
            this.lblSearchResult.Text = "📋 Kết quả tìm kiếm:";
            // 
            // dgvSearchResult
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.dgvSearchResult.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearchResult.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvSearchResult.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.dgvSearchResult.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSearchResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchResult.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSearchResult.EnableHeadersVisualStyles = false;
            this.dgvSearchResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvSearchResult.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.dgvSearchResult.Location = new System.Drawing.Point(15, 45);
            this.dgvSearchResult.Name = "dgvSearchResult";
            this.dgvSearchResult.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchResult.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSearchResult.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvSearchResult.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSearchResult.RowTemplate.Height = 29;
            this.dgvSearchResult.SelectedIndex = -1;
            this.dgvSearchResult.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSearchResult.Size = new System.Drawing.Size(870, 220);
            this.dgvSearchResult.TabIndex = 1;
            // 
            // uiPanel3
            // 
            this.uiPanel3.Controls.Add(this.dgvSearchResult);
            this.uiPanel3.Controls.Add(this.lblSearchResult);
            this.uiPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPanel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPanel3.Location = new System.Drawing.Point(0, 220);
            this.uiPanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel3.Name = "uiPanel3";
            this.uiPanel3.Padding = new System.Windows.Forms.Padding(10);
            this.uiPanel3.Size = new System.Drawing.Size(901, 298);
            this.uiPanel3.TabIndex = 3;
            this.uiPanel3.Text = null;
            this.uiPanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiHeaderPanel
            // 
            this.uiHeaderPanel.CircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.uiHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.uiHeaderPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.uiHeaderPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.uiHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.uiHeaderPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiHeaderPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiHeaderPanel.Name = "uiHeaderPanel";
            this.uiHeaderPanel.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.uiHeaderPanel.Radius = 0;
            this.uiHeaderPanel.RadiusSides = Sunny.UI.UICornerRadiusSides.None;
            this.uiHeaderPanel.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.uiHeaderPanel.Size = new System.Drawing.Size(901, 50);
            this.uiHeaderPanel.Style = Sunny.UI.UIStyle.Custom;
            this.uiHeaderPanel.StyleCustomMode = true;
            this.uiHeaderPanel.TabIndex = 0;
            this.uiHeaderPanel.Text = "TÌM KIẾM MÃ SẢN PHẨM";
            this.uiHeaderPanel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // PScaner
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(901, 548);
            this.Controls.Add(this.uiPanel3);
            this.Controls.Add(this.uiStatusPanel);
            this.Controls.Add(this.uiPanel2);
            this.Controls.Add(this.uiPanel1);
            this.Controls.Add(this.uiHeaderPanel);
            this.Name = "PScaner";
            this.Style = Sunny.UI.UIStyle.Custom;
            this.Symbol = 559520;
            this.Text = "Tìm kiếm";
            this.uiPanel1.ResumeLayout(false);
            this.uiPanel1.PerformLayout();
            this.uiPanel2.ResumeLayout(false);
            this.uiPanel2.PerformLayout();
            this.uiStatusPanel.ResumeLayout(false);
            this.uiStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchResult)).EndInit();
            this.uiPanel3.ResumeLayout(false);
            this.uiPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UITextBox txtScannedCode;
        private Sunny.UI.UILabel lblScannedCode;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UIButton btnSearch;
        private Sunny.UI.UITextBox txtSearchManual;
        private Sunny.UI.UILabel lblSearchManual;
        private Sunny.UI.UIPanel uiStatusPanel;
        private Sunny.UI.UILabel lblStatus;
        private Sunny.UI.UILabel lblSearchResult;
        private Sunny.UI.UIDataGridView dgvSearchResult;
        private Sunny.UI.UIPanel uiPanel3;
        private Sunny.UI.UIHeaderButton uiHeaderPanel;
    }
}