namespace CProject.Views
{
    partial class FDashboard
    {
        private System.ComponentModel.IContainer components = null;

        private Sunny.UI.UITitlePanel panelDataPool;
        private Sunny.UI.UITableLayoutPanel tableLayoutDataPool;
        private Sunny.UI.UITextBox txtPoolName;
        private Sunny.UI.UILabel lblPoolName;
        private Sunny.UI.UITextBox txtCode;
        private Sunny.UI.UILabel lblCode;
        private Sunny.UI.UISymbolButton btnCreatePool;
        private Sunny.UI.UISymbolButton btnAddCodeSingle;
        private Sunny.UI.UISymbolButton btnAddCodeFile;
        private Sunny.UI.UISymbolButton btnGetPoolInfo;
        private Sunny.UI.UIListBox lstResult;

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
            this.panelDataPool = new Sunny.UI.UITitlePanel();
            this.tableLayoutDataPool = new Sunny.UI.UITableLayoutPanel();
            this.lblPoolName = new Sunny.UI.UILabel();
            this.txtPoolName = new Sunny.UI.UITextBox();
            this.lblCode = new Sunny.UI.UILabel();
            this.txtCode = new Sunny.UI.UITextBox();
            this.btnCreatePool = new Sunny.UI.UISymbolButton();
            this.btnAddCodeSingle = new Sunny.UI.UISymbolButton();
            this.btnAddCodeFile = new Sunny.UI.UISymbolButton();
            this.btnGetPoolInfo = new Sunny.UI.UISymbolButton();
            this.lstResult = new Sunny.UI.UIListBox();

            this.panelDataPool.SuspendLayout();
            this.tableLayoutDataPool.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDataPool
            // 
            this.panelDataPool.Controls.Add(this.tableLayoutDataPool);
            this.panelDataPool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDataPool.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.panelDataPool.Location = new System.Drawing.Point(0, 0);
            this.panelDataPool.Name = "panelDataPool";
            this.panelDataPool.Padding = new System.Windows.Forms.Padding(1, 36, 1, 1);
            this.panelDataPool.Size = new System.Drawing.Size(961, 581);
            this.panelDataPool.TabIndex = 0;
            this.panelDataPool.Text = "QUẢN LÝ DATA POOL";
            this.panelDataPool.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelDataPool.TitleHeight = 36;
            // 
            // tableLayoutDataPool
            // 
            this.tableLayoutDataPool.BackColor = System.Drawing.Color.White;
            this.tableLayoutDataPool.ColumnCount = 4;
            this.tableLayoutDataPool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutDataPool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutDataPool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutDataPool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutDataPool.Controls.Add(this.lblPoolName, 0, 0);
            this.tableLayoutDataPool.Controls.Add(this.txtPoolName, 0, 1);
            this.tableLayoutDataPool.Controls.Add(this.lblCode, 1, 0);
            this.tableLayoutDataPool.Controls.Add(this.txtCode, 1, 1);
            this.tableLayoutDataPool.Controls.Add(this.btnCreatePool, 0, 2);
            this.tableLayoutDataPool.Controls.Add(this.btnAddCodeSingle, 1, 2);
            this.tableLayoutDataPool.Controls.Add(this.btnAddCodeFile, 2, 2);
            this.tableLayoutDataPool.Controls.Add(this.btnGetPoolInfo, 3, 2);
            this.tableLayoutDataPool.Controls.Add(this.lstResult, 0, 3);
            this.tableLayoutDataPool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutDataPool.Location = new System.Drawing.Point(1, 36);
            this.tableLayoutDataPool.Name = "tableLayoutDataPool";
            this.tableLayoutDataPool.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayoutDataPool.RowCount = 4;
            this.tableLayoutDataPool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutDataPool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutDataPool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutDataPool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutDataPool.Size = new System.Drawing.Size(959, 544);
            this.tableLayoutDataPool.TabIndex = 0;
            // 
            // lblPoolName
            // 
            this.lblPoolName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoolName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblPoolName.Location = new System.Drawing.Point(13, 10);
            this.lblPoolName.Name = "lblPoolName";
            this.lblPoolName.Size = new System.Drawing.Size(224, 30);
            this.lblPoolName.TabIndex = 0;
            this.lblPoolName.Text = "Tên Pool:";
            this.lblPoolName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPoolName
            // 
            this.txtPoolName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPoolName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtPoolName.Location = new System.Drawing.Point(13, 43);
            this.txtPoolName.Name = "txtPoolName";
            this.txtPoolName.Size = new System.Drawing.Size(224, 25);
            this.txtPoolName.TabIndex = 1;
            this.txtPoolName.Text = "TestPool";
            // 
            // lblCode
            // 
            this.lblCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblCode.Location = new System.Drawing.Point(251, 10);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(224, 30);
            this.lblCode.TabIndex = 2;
            this.lblCode.Text = "Code (cho mode 1):";
            this.lblCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCode
            // 
            this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtCode.Location = new System.Drawing.Point(251, 43);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(224, 25);
            this.txtCode.TabIndex = 3;
            this.txtCode.Text = "CODE001";
            // 
            // btnCreatePool
            // 
            this.btnCreatePool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreatePool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCreatePool.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCreatePool.Location = new System.Drawing.Point(13, 93);
            this.btnCreatePool.Name = "btnCreatePool";
            this.btnCreatePool.Size = new System.Drawing.Size(224, 44);
            this.btnCreatePool.Symbol = 57359;
            this.btnCreatePool.SymbolSize = 16;
            this.btnCreatePool.TabIndex = 4;
            this.btnCreatePool.Text = "Tạo Pool";
            this.btnCreatePool.Click += new System.EventHandler(this.btnCreatePool_Click);
            // 
            // btnAddCodeSingle
            // 
            this.btnAddCodeSingle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddCodeSingle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddCodeSingle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnAddCodeSingle.Location = new System.Drawing.Point(251, 93);
            this.btnAddCodeSingle.Name = "btnAddCodeSingle";
            this.btnAddCodeSingle.Size = new System.Drawing.Size(224, 44);
            this.btnAddCodeSingle.Symbol = 57345;
            this.btnAddCodeSingle.SymbolSize = 16;
            this.btnAddCodeSingle.TabIndex = 5;
            this.btnAddCodeSingle.Text = "Thêm 1 Code";
            this.btnAddCodeSingle.Click += new System.EventHandler(this.btnAddCodeSingle_Click);
            // 
            // btnAddCodeFile
            // 
            this.btnAddCodeFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddCodeFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddCodeFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnAddCodeFile.Location = new System.Drawing.Point(489, 93);
            this.btnAddCodeFile.Name = "btnAddCodeFile";
            this.btnAddCodeFile.Size = new System.Drawing.Size(224, 44);
            this.btnAddCodeFile.Symbol = 61443;
            this.btnAddCodeFile.SymbolSize = 16;
            this.btnAddCodeFile.TabIndex = 6;
            this.btnAddCodeFile.Text = "Thêm từ File";
            this.btnAddCodeFile.Click += new System.EventHandler(this.btnAddCodeFile_Click);
            // 
            // btnGetPoolInfo
            // 
            this.btnGetPoolInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGetPoolInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGetPoolInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnGetPoolInfo.Location = new System.Drawing.Point(727, 93);
            this.btnGetPoolInfo.Name = "btnGetPoolInfo";
            this.btnGetPoolInfo.Size = new System.Drawing.Size(219, 44);
            this.btnGetPoolInfo.Symbol = 62438;
            this.btnGetPoolInfo.SymbolSize = 16;
            this.btnGetPoolInfo.TabIndex = 7;
            this.btnGetPoolInfo.Text = "Lấy thông tin Pool";
            this.btnGetPoolInfo.Click += new System.EventHandler(this.btnGetPoolInfo_Click);
            // 
            // lstResult
            // 
            this.tableLayoutDataPool.SetColumnSpan(this.lstResult, 4);
            this.lstResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lstResult.FormattingEnabled = true;
            this.lstResult.ItemHeight = 17;
            this.lstResult.Location = new System.Drawing.Point(13, 150);
            this.lstResult.Name = "lstResult";
            this.lstResult.Size = new System.Drawing.Size(933, 381);
            this.lstResult.TabIndex = 8;
            // 
            // FDashboard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(961, 581);
            this.Controls.Add(this.panelDataPool);
            this.Name = "FDashboard";
            this.Text = "Quản Lý Data Pool";
            this.panelDataPool.ResumeLayout(false);
            this.tableLayoutDataPool.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}