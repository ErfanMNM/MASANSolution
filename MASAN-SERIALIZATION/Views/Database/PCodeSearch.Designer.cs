namespace MASAN_SERIALIZATION.Views.Database
{
    partial class PCodeSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.uiHeader = new Sunny.UI.UIHeaderButton();
            this.topPanel = new Sunny.UI.UIPanel();
            this.btnSearch = new Sunny.UI.UIButton();
            this.txtSearch = new Sunny.UI.UITextBox();
            this.lblHint = new Sunny.UI.UILabel();
            this.middlePanel = new Sunny.UI.UIPanel();
            this.grid = new Sunny.UI.UIDataGridView();
            this.bottomPanel = new Sunny.UI.UIPanel();
            this.lblStatus = new Sunny.UI.UILabel();
            this.pagination = new Sunny.UI.UIPagination();
            this.ipPageSize = new Sunny.UI.UIComboBox();
            this.topPanel.SuspendLayout();
            this.middlePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiHeader
            // 
            this.uiHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.uiHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.uiHeader.Location = new System.Drawing.Point(0, 0);
            this.uiHeader.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiHeader.Name = "uiHeader";
            this.uiHeader.Padding = new System.Windows.Forms.Padding(0, 8, 0, 3);
            this.uiHeader.Radius = 0;
            this.uiHeader.RadiusSides = Sunny.UI.UICornerRadiusSides.None;
            this.uiHeader.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.uiHeader.Size = new System.Drawing.Size(900, 50);
            this.uiHeader.TabIndex = 0;
            this.uiHeader.Text = "Tra cứu mã sản phẩm (SQLite)";
            this.uiHeader.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.btnSearch);
            this.topPanel.Controls.Add(this.txtSearch);
            this.topPanel.Controls.Add(this.lblHint);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.topPanel.Location = new System.Drawing.Point(0, 50);
            this.topPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.topPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(900, 80);
            this.topPanel.TabIndex = 1;
            this.topPanel.Text = null;
            this.topPanel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSearch
            // 
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Location = new System.Drawing.Point(780, 40);
            this.btnSearch.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(110, 30);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtSearch.Location = new System.Drawing.Point(15, 40);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Padding = new System.Windows.Forms.Padding(5);
            this.txtSearch.ShowText = false;
            this.txtSearch.Size = new System.Drawing.Size(758, 30);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSearch.Watermark = "Nhập mã, Enter để tìm...";
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblHint.Location = new System.Drawing.Point(12, 12);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(305, 18);
            this.lblHint.TabIndex = 0;
            this.lblHint.Text = "Tìm theo chính xác, không thấy thì LIKE";
            // 
            // middlePanel
            // 
            this.middlePanel.Controls.Add(this.grid);
            this.middlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.middlePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.middlePanel.Location = new System.Drawing.Point(0, 130);
            this.middlePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.middlePanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.middlePanel.Name = "middlePanel";
            this.middlePanel.Padding = new System.Windows.Forms.Padding(10);
            this.middlePanel.Size = new System.Drawing.Size(900, 390);
            this.middlePanel.TabIndex = 2;
            this.middlePanel.Text = null;
            this.middlePanel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grid
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grid.BackgroundColor = System.Drawing.Color.White;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = dataGridViewCellStyle3;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.EnableHeadersVisualStyles = false;
            this.grid.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.grid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.grid.Location = new System.Drawing.Point(10, 10);
            this.grid.Name = "grid";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.grid.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.grid.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.grid.RowTemplate.Height = 28;
            this.grid.SelectedIndex = -1;
            this.grid.Size = new System.Drawing.Size(880, 370);
            this.grid.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.grid.TabIndex = 0;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.lblStatus);
            this.bottomPanel.Controls.Add(this.pagination);
            this.bottomPanel.Controls.Add(this.ipPageSize);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.bottomPanel.Location = new System.Drawing.Point(0, 520);
            this.bottomPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bottomPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.bottomPanel.Size = new System.Drawing.Size(900, 40);
            this.bottomPanel.TabIndex = 3;
            this.bottomPanel.Text = null;
            this.bottomPanel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblStatus.Location = new System.Drawing.Point(10, 5);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(350, 30);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Sẵn sàng";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pagination
            // 
            this.pagination.ButtonFillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(204)))));
            this.pagination.ButtonStyleInherited = false;
            this.pagination.Dock = System.Windows.Forms.DockStyle.Right;
            this.pagination.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.pagination.Location = new System.Drawing.Point(179, 5);
            this.pagination.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pagination.MinimumSize = new System.Drawing.Size(1, 1);
            this.pagination.Name = "pagination";
            this.pagination.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pagination.ShowText = false;
            this.pagination.Size = new System.Drawing.Size(611, 30);
            this.pagination.TabIndex = 1;
            this.pagination.Text = "pagination";
            this.pagination.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipPageSize
            // 
            this.ipPageSize.DataSource = null;
            this.ipPageSize.Dock = System.Windows.Forms.DockStyle.Right;
            this.ipPageSize.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.ipPageSize.FillColor = System.Drawing.Color.White;
            this.ipPageSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ipPageSize.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.ipPageSize.Items.AddRange(new object[] {
            "20",
            "50",
            "100",
            "200"});
            this.ipPageSize.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ipPageSize.Location = new System.Drawing.Point(790, 5);
            this.ipPageSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipPageSize.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipPageSize.Name = "ipPageSize";
            this.ipPageSize.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipPageSize.Size = new System.Drawing.Size(100, 30);
            this.ipPageSize.SymbolSize = 24;
            this.ipPageSize.TabIndex = 0;
            this.ipPageSize.Text = "50";
            this.ipPageSize.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipPageSize.Watermark = "";
            this.ipPageSize.SelectedIndexChanged += new System.EventHandler(this.ipPageSize_SelectedIndexChanged);
            // 
            // PCodeSearch
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.middlePanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.uiHeader);
            this.Name = "PCodeSearch";
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.middlePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIHeaderButton uiHeader;
        private Sunny.UI.UIPanel topPanel;
        private Sunny.UI.UIButton btnSearch;
        private Sunny.UI.UITextBox txtSearch;
        private Sunny.UI.UILabel lblHint;
        private Sunny.UI.UIPanel middlePanel;
        private Sunny.UI.UIDataGridView grid;
        private Sunny.UI.UIPanel bottomPanel;
        private Sunny.UI.UILabel lblStatus;
        private Sunny.UI.UIPagination pagination;
        private Sunny.UI.UIComboBox ipPageSize;
    }
}

