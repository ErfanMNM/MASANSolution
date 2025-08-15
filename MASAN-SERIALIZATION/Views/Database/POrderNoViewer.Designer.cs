namespace MASAN_SERIALIZATION.Views.Database
{
    partial class POrderNoViewer
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
            this.header = new Sunny.UI.UIHeaderButton();
            this.top = new Sunny.UI.UIPanel();
            this.btnBrowse = new Sunny.UI.UIButton();
            this.btnLoad = new Sunny.UI.UIButton();
            this.cbOrderNo = new Sunny.UI.UIComboBox();
            this.lblOrderNo = new Sunny.UI.UILabel();
            this.txtDbPath = new Sunny.UI.UITextBox();
            this.lblDbPath = new Sunny.UI.UILabel();
            this.body = new Sunny.UI.UIPanel();
            this.grid = new Sunny.UI.UIDataGridView();
            this.bottom = new Sunny.UI.UIPanel();
            this.lblStatus = new Sunny.UI.UILabel();
            this.txtFilter = new Sunny.UI.UITextBox();
            this.lblFilter = new Sunny.UI.UILabel();
            this.top.SuspendLayout();
            this.body.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // header
            // 
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.header.Location = new System.Drawing.Point(0, 0);
            this.header.MinimumSize = new System.Drawing.Size(1, 1);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(900, 50);
            this.header.TabIndex = 0;
            this.header.Text = "Xem dữ liệu từ orderNo.db";
            // 
            // top
            // 
            this.top.Controls.Add(this.btnBrowse);
            this.top.Controls.Add(this.btnLoad);
            this.top.Controls.Add(this.cbOrderNo);
            this.top.Controls.Add(this.lblOrderNo);
            this.top.Controls.Add(this.txtDbPath);
            this.top.Controls.Add(this.lblDbPath);
            this.top.Dock = System.Windows.Forms.DockStyle.Top;
            this.top.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.top.Location = new System.Drawing.Point(0, 50);
            this.top.MinimumSize = new System.Drawing.Size(1, 1);
            this.top.Name = "top";
            this.top.Padding = new System.Windows.Forms.Padding(10);
            this.top.Size = new System.Drawing.Size(900, 110);
            this.top.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnBrowse.Location = new System.Drawing.Point(745, 65);
            this.btnBrowse.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(120, 30);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "Chọn file...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnLoad.Location = new System.Drawing.Point(620, 20);
            this.btnLoad.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(120, 30);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Tải dữ liệu";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // cbOrderNo
            // 
            this.cbOrderNo.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.cbOrderNo.FillColor = System.Drawing.Color.White;
            this.cbOrderNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.cbOrderNo.Location = new System.Drawing.Point(100, 20);
            this.cbOrderNo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbOrderNo.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbOrderNo.Name = "cbOrderNo";
            this.cbOrderNo.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbOrderNo.Size = new System.Drawing.Size(500, 30);
            this.cbOrderNo.TabIndex = 1;
            this.cbOrderNo.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOrderNo
            // 
            this.lblOrderNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblOrderNo.Location = new System.Drawing.Point(15, 20);
            this.lblOrderNo.Name = "lblOrderNo";
            this.lblOrderNo.Size = new System.Drawing.Size(85, 30);
            this.lblOrderNo.TabIndex = 0;
            this.lblOrderNo.Text = "OrderNo:";
            this.lblOrderNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDbPath
            // 
            this.txtDbPath.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtDbPath.Location = new System.Drawing.Point(100, 65);
            this.txtDbPath.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtDbPath.Name = "txtDbPath";
            this.txtDbPath.Padding = new System.Windows.Forms.Padding(5);
            this.txtDbPath.ReadOnly = true;
            this.txtDbPath.ShowText = false;
            this.txtDbPath.Size = new System.Drawing.Size(640, 30);
            this.txtDbPath.TabIndex = 4;
            this.txtDbPath.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDbPath
            // 
            this.lblDbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblDbPath.Location = new System.Drawing.Point(15, 65);
            this.lblDbPath.Name = "lblDbPath";
            this.lblDbPath.Size = new System.Drawing.Size(85, 30);
            this.lblDbPath.TabIndex = 3;
            this.lblDbPath.Text = "DB path:";
            this.lblDbPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // body
            // 
            this.body.Controls.Add(this.grid);
            this.body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.body.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.body.Location = new System.Drawing.Point(0, 160);
            this.body.MinimumSize = new System.Drawing.Size(1, 1);
            this.body.Name = "body";
            this.body.Padding = new System.Windows.Forms.Padding(10);
            this.body.Size = new System.Drawing.Size(900, 340);
            this.body.TabIndex = 2;
            // 
            // grid
            // 
            this.grid.BackgroundColor = System.Drawing.Color.White;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(10, 10);
            this.grid.Name = "grid";
            this.grid.RowHeadersVisible = false;
            this.grid.RowTemplate.Height = 28;
            this.grid.Size = new System.Drawing.Size(880, 320);
            this.grid.TabIndex = 0;
            // 
            // bottom
            // 
            this.bottom.Controls.Add(this.lblStatus);
            this.bottom.Controls.Add(this.txtFilter);
            this.bottom.Controls.Add(this.lblFilter);
            this.bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.bottom.Location = new System.Drawing.Point(0, 500);
            this.bottom.MinimumSize = new System.Drawing.Size(1, 1);
            this.bottom.Name = "bottom";
            this.bottom.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.bottom.Size = new System.Drawing.Size(900, 40);
            this.bottom.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblStatus.Location = new System.Drawing.Point(10, 5);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(350, 30);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Chưa tải";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFilter
            // 
            this.txtFilter.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFilter.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtFilter.Location = new System.Drawing.Point(600, 5);
            this.txtFilter.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Padding = new System.Windows.Forms.Padding(5);
            this.txtFilter.ShowText = false;
            this.txtFilter.Size = new System.Drawing.Size(290, 30);
            this.txtFilter.TabIndex = 1;
            this.txtFilter.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtFilter.Watermark = "Lọc theo Code/Mã thùng";
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // lblFilter
            // 
            this.lblFilter.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblFilter.Location = new System.Drawing.Point(530, 5);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(70, 30);
            this.lblFilter.TabIndex = 0;
            this.lblFilter.Text = "Lọc:";
            this.lblFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // POrderNoViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.body);
            this.Controls.Add(this.bottom);
            this.Controls.Add(this.top);
            this.Controls.Add(this.header);
            this.Name = "POrderNoViewer";
            this.Size = new System.Drawing.Size(900, 540);
            this.top.ResumeLayout(false);
            this.body.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.bottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIHeaderButton header;
        private Sunny.UI.UIPanel top;
        private Sunny.UI.UIButton btnLoad;
        private Sunny.UI.UIComboBox cbOrderNo;
        private Sunny.UI.UILabel lblOrderNo;
        private Sunny.UI.UIPanel body;
        private Sunny.UI.UIDataGridView grid;
        private Sunny.UI.UIPanel bottom;
        private Sunny.UI.UILabel lblFilter;
        private Sunny.UI.UITextBox txtFilter;
        private Sunny.UI.UILabel lblStatus;
        private Sunny.UI.UITextBox txtDbPath;
        private Sunny.UI.UILabel lblDbPath;
        private Sunny.UI.UIButton btnBrowse;
    }
}

