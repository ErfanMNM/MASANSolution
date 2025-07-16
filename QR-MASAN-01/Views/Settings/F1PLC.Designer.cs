namespace QR_MASAN_01.Views.Settings
{
    partial class F1PLC
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
            this.MainPanel = new Sunny.UI.UITableLayoutPanel();
            this.panelRight = new Sunny.UI.UITableLayoutPanel();
            this.uiTitlePanel2 = new Sunny.UI.UITitlePanel();
            this.uiTableLayoutPanel22 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel23 = new Sunny.UI.UITableLayoutPanel();
            this.btnSave = new Sunny.UI.UISymbolButton();
            this.btnUndo = new Sunny.UI.UISymbolButton();
            this.btnDelete = new Sunny.UI.UISymbolButton();
            this.uiTableLayoutPanel16 = new Sunny.UI.UITableLayoutPanel();
            this.btnNewRecipe = new Sunny.UI.UISymbolButton();
            this.cbbRecipe = new Sunny.UI.UIComboBox();

            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            this.uiTableLayoutPanel10 = new Sunny.UI.UITableLayoutPanel();
            this.uiLine5 = new Sunny.UI.UILine();
            this.uiTableLayoutPanel11 = new Sunny.UI.UITableLayoutPanel();
            this.btnCustom = new Sunny.UI.UISymbolButton();
            this.ipDCustom = new Sunny.UI.UINumPadTextBox();
            this.ipChange_Read_Write_Mode = new Sunny.UI.UIPanel();
            this.uiTableLayoutPanel8 = new Sunny.UI.UITableLayoutPanel();
            this.uiLine4 = new Sunny.UI.UILine();
            this.uiTableLayoutPanel9 = new Sunny.UI.UITableLayoutPanel();
            this.ipProduct_Length = new Sunny.UI.UIIntegerUpDown();
            this.opProductLenght = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel6 = new Sunny.UI.UITableLayoutPanel();
            this.uiLine3 = new Sunny.UI.UILine();
            this.uiTableLayoutPanel7 = new Sunny.UI.UITableLayoutPanel();
            this.ipProcess_TimeOut = new Sunny.UI.UIIntegerUpDown();
            this.opProcessing = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel3 = new Sunny.UI.UITableLayoutPanel();
            this.uiLine1 = new Sunny.UI.UILine();
            this.uiTableLayoutPanel4 = new Sunny.UI.UITableLayoutPanel();
            this.ipDelay_Reject = new Sunny.UI.UIIntegerUpDown();
            this.opDelayReject = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel5 = new Sunny.UI.UITableLayoutPanel();
            this.uiLine2 = new Sunny.UI.UILine();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.ipDelay_Camera = new Sunny.UI.UIIntegerUpDown();
            this.opDelayCmr = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel12 = new Sunny.UI.UITableLayoutPanel();
            this.uiLine6 = new Sunny.UI.UILine();
            this.uiTableLayoutPanel13 = new Sunny.UI.UITableLayoutPanel();
            this.ipDebounce_Sensor = new Sunny.UI.UIIntegerUpDown();
            this.opDeboundSensor = new Sunny.UI.UITextBox();
            this.uiTableLayoutPanel14 = new Sunny.UI.UITableLayoutPanel();
            this.uiLine7 = new Sunny.UI.UILine();
            this.uiTableLayoutPanel15 = new Sunny.UI.UITableLayoutPanel();
            this.ipReject_Streng = new Sunny.UI.UIIntegerUpDown();
            this.opRejectStreng = new Sunny.UI.UITextBox();
            this.WKUpdate = new System.ComponentModel.BackgroundWorker();
            this.WK_PLC = new System.ComponentModel.BackgroundWorker();
            this.WK_Write_To_PLC = new System.ComponentModel.BackgroundWorker();
            this.omronPLC_Hsl1 = new SPMS1.OmronPLC_Hsl(this.components);
            this.ipopValueCustom = new Sunny.UI.UINumPadTextBox();
            this.MainPanel.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.uiTitlePanel2.SuspendLayout();
            this.uiTableLayoutPanel22.SuspendLayout();
            this.uiTableLayoutPanel23.SuspendLayout();
            this.uiTableLayoutPanel16.SuspendLayout();
            this.uiTitlePanel1.SuspendLayout();
            this.uiTableLayoutPanel1.SuspendLayout();
            this.uiTableLayoutPanel10.SuspendLayout();
            this.uiTableLayoutPanel11.SuspendLayout();
            this.uiTableLayoutPanel8.SuspendLayout();
            this.uiTableLayoutPanel9.SuspendLayout();
            this.uiTableLayoutPanel6.SuspendLayout();
            this.uiTableLayoutPanel7.SuspendLayout();
            this.uiTableLayoutPanel3.SuspendLayout();
            this.uiTableLayoutPanel4.SuspendLayout();
            this.uiTableLayoutPanel5.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.uiTableLayoutPanel12.SuspendLayout();
            this.uiTableLayoutPanel13.SuspendLayout();
            this.uiTableLayoutPanel14.SuspendLayout();
            this.uiTableLayoutPanel15.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.ColumnCount = 2;
            this.MainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.80929F));
            this.MainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.19071F));
            this.MainPanel.Controls.Add(this.panelRight, 1, 0);
            this.MainPanel.Controls.Add(this.uiTitlePanel1, 0, 0);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.RowCount = 1;
            this.MainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainPanel.Size = new System.Drawing.Size(836, 652);
            this.MainPanel.TabIndex = 0;
            this.MainPanel.TagString = null;
            // 
            // panelRight
            // 
            this.panelRight.ColumnCount = 1;
            this.panelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelRight.Controls.Add(this.uiTitlePanel2, 0, 1);

            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(352, 3);
            this.panelRight.Name = "panelRight";
            this.panelRight.RowCount = 2;
            this.panelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.05264F));
            this.panelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.94737F));
            this.panelRight.Size = new System.Drawing.Size(481, 646);
            this.panelRight.TabIndex = 0;
            this.panelRight.TagString = null;
            // 
            // uiTitlePanel2
            // 
            this.uiTitlePanel2.Controls.Add(this.uiTableLayoutPanel22);
            this.uiTitlePanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel2.Location = new System.Drawing.Point(4, 464);
            this.uiTitlePanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel2.Name = "uiTitlePanel2";
            this.uiTitlePanel2.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel2.ShowText = false;
            this.uiTitlePanel2.Size = new System.Drawing.Size(473, 177);
            this.uiTitlePanel2.TabIndex = 2;
            this.uiTitlePanel2.Text = "SẢN PHẨM";
            this.uiTitlePanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel22
            // 
            this.uiTableLayoutPanel22.ColumnCount = 1;
            this.uiTableLayoutPanel22.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.02632F));
            this.uiTableLayoutPanel22.Controls.Add(this.uiTableLayoutPanel23, 0, 1);
            this.uiTableLayoutPanel22.Controls.Add(this.uiTableLayoutPanel16, 0, 0);
            this.uiTableLayoutPanel22.Location = new System.Drawing.Point(3, 39);
            this.uiTableLayoutPanel22.Name = "uiTableLayoutPanel22";
            this.uiTableLayoutPanel22.RowCount = 2;
            this.uiTableLayoutPanel22.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.73529F));
            this.uiTableLayoutPanel22.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.26471F));
            this.uiTableLayoutPanel22.Size = new System.Drawing.Size(466, 136);
            this.uiTableLayoutPanel22.TabIndex = 0;
            this.uiTableLayoutPanel22.TagString = null;
            // 
            // uiTableLayoutPanel23
            // 
            this.uiTableLayoutPanel23.ColumnCount = 3;
            this.uiTableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.55556F));
            this.uiTableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.11111F));
            this.uiTableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.uiTableLayoutPanel23.Controls.Add(this.btnSave, 0, 0);
            this.uiTableLayoutPanel23.Controls.Add(this.btnUndo, 1, 0);
            this.uiTableLayoutPanel23.Controls.Add(this.btnDelete, 2, 0);
            this.uiTableLayoutPanel23.Location = new System.Drawing.Point(3, 71);
            this.uiTableLayoutPanel23.Name = "uiTableLayoutPanel23";
            this.uiTableLayoutPanel23.RowCount = 1;
            this.uiTableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel23.Size = new System.Drawing.Size(460, 61);
            this.uiTableLayoutPanel23.TabIndex = 1;
            this.uiTableLayoutPanel23.TagString = null;
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(166, 55);
            this.btnSave.Symbol = 557704;
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Áp dụng";
            this.btnSave.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUndo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnUndo.Location = new System.Drawing.Point(175, 3);
            this.btnUndo.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(127, 55);
            this.btnUndo.Symbol = 74;
            this.btnUndo.TabIndex = 1;
            this.btnUndo.Text = "Nhập lại";
            this.btnUndo.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnDelete.Location = new System.Drawing.Point(308, 3);
            this.btnDelete.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(149, 55);
            this.btnDelete.Symbol = 61460;
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // uiTableLayoutPanel16
            // 
            this.uiTableLayoutPanel16.ColumnCount = 2;
            this.uiTableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.22222F));
            this.uiTableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.77778F));
            this.uiTableLayoutPanel16.Controls.Add(this.btnNewRecipe, 1, 0);
            this.uiTableLayoutPanel16.Controls.Add(this.cbbRecipe, 0, 0);
            this.uiTableLayoutPanel16.Location = new System.Drawing.Point(3, 3);
            this.uiTableLayoutPanel16.Name = "uiTableLayoutPanel16";
            this.uiTableLayoutPanel16.RowCount = 1;
            this.uiTableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel16.Size = new System.Drawing.Size(460, 62);
            this.uiTableLayoutPanel16.TabIndex = 2;
            this.uiTableLayoutPanel16.TagString = null;
            // 
            // btnNewRecipe
            // 
            this.btnNewRecipe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewRecipe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNewRecipe.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNewRecipe.Location = new System.Drawing.Point(381, 3);
            this.btnNewRecipe.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNewRecipe.Name = "btnNewRecipe";
            this.btnNewRecipe.Size = new System.Drawing.Size(76, 56);
            this.btnNewRecipe.Symbol = 61543;
            this.btnNewRecipe.TabIndex = 2;
            this.btnNewRecipe.Text = "Mới";
            this.btnNewRecipe.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnNewRecipe.Click += new System.EventHandler(this.btnNewRecipe_Click);
            // 
            // cbbRecipe
            // 
            this.cbbRecipe.DataSource = null;
            this.cbbRecipe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbbRecipe.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.cbbRecipe.FillColor = System.Drawing.Color.White;
            this.cbbRecipe.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbRecipe.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cbbRecipe.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.cbbRecipe.Location = new System.Drawing.Point(2, 2);
            this.cbbRecipe.Margin = new System.Windows.Forms.Padding(2);
            this.cbbRecipe.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbbRecipe.Name = "cbbRecipe";
            this.cbbRecipe.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbbRecipe.Size = new System.Drawing.Size(374, 58);
            this.cbbRecipe.SymbolSize = 24;
            this.cbbRecipe.TabIndex = 1;
            this.cbbRecipe.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbbRecipe.Watermark = "";
            this.cbbRecipe.SelectedIndexChanged += new System.EventHandler(this.cbbRecipe_SelectedIndexChanged);
            // 
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.uiTableLayoutPanel1);
            this.uiTitlePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(4, 5);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 35, 1, 1);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(341, 642);
            this.uiTitlePanel1.TabIndex = 1;
            this.uiTitlePanel1.Text = "PLC Parameters";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel1
            // 
            this.uiTableLayoutPanel1.ColumnCount = 1;
            this.uiTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel10, 0, 6);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel8, 0, 5);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel6, 0, 2);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel3, 0, 1);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel5, 0, 0);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel12, 0, 3);
            this.uiTableLayoutPanel1.Controls.Add(this.uiTableLayoutPanel14, 0, 4);
            this.uiTableLayoutPanel1.Location = new System.Drawing.Point(3, 37);
            this.uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            this.uiTableLayoutPanel1.RowCount = 7;
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.uiTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.uiTableLayoutPanel1.Size = new System.Drawing.Size(334, 602);
            this.uiTableLayoutPanel1.TabIndex = 0;
            this.uiTableLayoutPanel1.TagString = null;
            // 
            // uiTableLayoutPanel10
            // 
            this.uiTableLayoutPanel10.ColumnCount = 1;
            this.uiTableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel10.Controls.Add(this.uiLine5, 0, 0);
            this.uiTableLayoutPanel10.Controls.Add(this.uiTableLayoutPanel11, 0, 1);
            this.uiTableLayoutPanel10.Location = new System.Drawing.Point(0, 515);
            this.uiTableLayoutPanel10.Margin = new System.Windows.Forms.Padding(0);
            this.uiTableLayoutPanel10.Name = "uiTableLayoutPanel10";
            this.uiTableLayoutPanel10.RowCount = 2;
            this.uiTableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.uiTableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.uiTableLayoutPanel10.Size = new System.Drawing.Size(334, 87);
            this.uiTableLayoutPanel10.TabIndex = 74;
            this.uiTableLayoutPanel10.TagString = null;
            // 
            // uiLine5
            // 
            this.uiLine5.BackColor = System.Drawing.Color.Transparent;
            this.uiLine5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLine5.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLine5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine5.Location = new System.Drawing.Point(3, 3);
            this.uiLine5.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine5.Name = "uiLine5";
            this.uiLine5.Size = new System.Drawing.Size(328, 24);
            this.uiLine5.TabIndex = 55;
            this.uiLine5.Text = "Đọc ghi tùy chỉnh";
            this.uiLine5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel11
            // 
            this.uiTableLayoutPanel11.ColumnCount = 4;
            this.uiTableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.uiTableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
            this.uiTableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.uiTableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.uiTableLayoutPanel11.Controls.Add(this.ipopValueCustom, 2, 0);
            this.uiTableLayoutPanel11.Controls.Add(this.btnCustom, 3, 0);
            this.uiTableLayoutPanel11.Controls.Add(this.ipDCustom, 1, 0);
            this.uiTableLayoutPanel11.Controls.Add(this.ipChange_Read_Write_Mode, 0, 0);
            this.uiTableLayoutPanel11.Location = new System.Drawing.Point(3, 33);
            this.uiTableLayoutPanel11.Name = "uiTableLayoutPanel11";
            this.uiTableLayoutPanel11.RowCount = 1;
            this.uiTableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel11.Size = new System.Drawing.Size(328, 51);
            this.uiTableLayoutPanel11.TabIndex = 0;
            this.uiTableLayoutPanel11.TagString = null;
            // 
            // btnCustom
            // 
            this.btnCustom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCustom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCustom.Location = new System.Drawing.Point(269, 3);
            this.btnCustom.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCustom.Name = "btnCustom";
            this.btnCustom.Size = new System.Drawing.Size(56, 45);
            this.btnCustom.Symbol = 161804;
            this.btnCustom.SymbolSize = 26;
            this.btnCustom.TabIndex = 4;
            this.btnCustom.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCustom.Click += new System.EventHandler(this.btnCustom_Click);
            // 
            // ipDCustom
            // 
            this.ipDCustom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipDCustom.FillColor = System.Drawing.Color.White;
            this.ipDCustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipDCustom.Location = new System.Drawing.Point(55, 2);
            this.ipDCustom.Margin = new System.Windows.Forms.Padding(2);
            this.ipDCustom.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipDCustom.Name = "ipDCustom";
            this.ipDCustom.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipDCustom.Size = new System.Drawing.Size(87, 47);
            this.ipDCustom.SymbolDropDown = 557532;
            this.ipDCustom.SymbolNormal = 557532;
            this.ipDCustom.SymbolSize = 24;
            this.ipDCustom.TabIndex = 0;
            this.ipDCustom.Text = "D2025";
            this.ipDCustom.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipDCustom.Watermark = "";
            // 
            // ipChange_Read_Write_Mode
            // 
            this.ipChange_Read_Write_Mode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipChange_Read_Write_Mode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipChange_Read_Write_Mode.Location = new System.Drawing.Point(2, 2);
            this.ipChange_Read_Write_Mode.Margin = new System.Windows.Forms.Padding(2);
            this.ipChange_Read_Write_Mode.MinimumSize = new System.Drawing.Size(1, 1);
            this.ipChange_Read_Write_Mode.Name = "ipChange_Read_Write_Mode";
            this.ipChange_Read_Write_Mode.Size = new System.Drawing.Size(49, 47);
            this.ipChange_Read_Write_Mode.TabIndex = 5;
            this.ipChange_Read_Write_Mode.Text = "Đọc";
            this.ipChange_Read_Write_Mode.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ipChange_Read_Write_Mode.Click += new System.EventHandler(this.ipChange_Read_Write_Mode_Click);
            // 
            // uiTableLayoutPanel8
            // 
            this.uiTableLayoutPanel8.ColumnCount = 1;
            this.uiTableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel8.Controls.Add(this.uiLine4, 0, 0);
            this.uiTableLayoutPanel8.Controls.Add(this.uiTableLayoutPanel9, 0, 1);
            this.uiTableLayoutPanel8.Location = new System.Drawing.Point(0, 429);
            this.uiTableLayoutPanel8.Margin = new System.Windows.Forms.Padding(0);
            this.uiTableLayoutPanel8.Name = "uiTableLayoutPanel8";
            this.uiTableLayoutPanel8.RowCount = 2;
            this.uiTableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.uiTableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.uiTableLayoutPanel8.Size = new System.Drawing.Size(334, 86);
            this.uiTableLayoutPanel8.TabIndex = 73;
            this.uiTableLayoutPanel8.TagString = null;
            // 
            // uiLine4
            // 
            this.uiLine4.BackColor = System.Drawing.Color.Transparent;
            this.uiLine4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLine4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLine4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine4.Location = new System.Drawing.Point(3, 3);
            this.uiLine4.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine4.Name = "uiLine4";
            this.uiLine4.Size = new System.Drawing.Size(328, 24);
            this.uiLine4.TabIndex = 55;
            this.uiLine4.Text = "Độ dài sản phẩm (xung) chỉ đọc";
            this.uiLine4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel9
            // 
            this.uiTableLayoutPanel9.ColumnCount = 2;
            this.uiTableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.uiTableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.uiTableLayoutPanel9.Controls.Add(this.ipProduct_Length, 0, 0);
            this.uiTableLayoutPanel9.Controls.Add(this.opProductLenght, 1, 0);
            this.uiTableLayoutPanel9.Location = new System.Drawing.Point(3, 33);
            this.uiTableLayoutPanel9.Name = "uiTableLayoutPanel9";
            this.uiTableLayoutPanel9.RowCount = 1;
            this.uiTableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel9.Size = new System.Drawing.Size(328, 50);
            this.uiTableLayoutPanel9.TabIndex = 0;
            this.uiTableLayoutPanel9.TagString = null;
            // 
            // ipProduct_Length
            // 
            this.ipProduct_Length.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipProduct_Length.Enabled = false;
            this.ipProduct_Length.Font = new System.Drawing.Font("SimSun", 12F);
            this.ipProduct_Length.Location = new System.Drawing.Point(0, 3);
            this.ipProduct_Length.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.ipProduct_Length.Maximum = 65535;
            this.ipProduct_Length.Minimum = 0;
            this.ipProduct_Length.MinimumSize = new System.Drawing.Size(100, 0);
            this.ipProduct_Length.Name = "ipProduct_Length";
            this.ipProduct_Length.ShowText = false;
            this.ipProduct_Length.Size = new System.Drawing.Size(183, 47);
            this.ipProduct_Length.Step = 10;
            this.ipProduct_Length.TabIndex = 8;
            this.ipProduct_Length.Text = "_uiIntegerUpDown1";
            this.ipProduct_Length.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opProductLenght
            // 
            this.opProductLenght.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.opProductLenght.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opProductLenght.DoubleValue = 1000D;
            this.opProductLenght.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opProductLenght.IntValue = 1000;
            this.opProductLenght.Location = new System.Drawing.Point(186, 3);
            this.opProductLenght.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.opProductLenght.MinimumSize = new System.Drawing.Size(1, 16);
            this.opProductLenght.Name = "opProductLenght";
            this.opProductLenght.Padding = new System.Windows.Forms.Padding(5);
            this.opProductLenght.ReadOnly = true;
            this.opProductLenght.ShowText = false;
            this.opProductLenght.Size = new System.Drawing.Size(139, 47);
            this.opProductLenght.TabIndex = 2;
            this.opProductLenght.Text = "1000";
            this.opProductLenght.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.opProductLenght.Watermark = "";
            // 
            // uiTableLayoutPanel6
            // 
            this.uiTableLayoutPanel6.ColumnCount = 1;
            this.uiTableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel6.Controls.Add(this.uiLine3, 0, 0);
            this.uiTableLayoutPanel6.Controls.Add(this.uiTableLayoutPanel7, 0, 1);
            this.uiTableLayoutPanel6.Location = new System.Drawing.Point(0, 171);
            this.uiTableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.uiTableLayoutPanel6.Name = "uiTableLayoutPanel6";
            this.uiTableLayoutPanel6.RowCount = 2;
            this.uiTableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.uiTableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.uiTableLayoutPanel6.Size = new System.Drawing.Size(334, 86);
            this.uiTableLayoutPanel6.TabIndex = 68;
            this.uiTableLayoutPanel6.TagString = null;
            // 
            // uiLine3
            // 
            this.uiLine3.BackColor = System.Drawing.Color.Transparent;
            this.uiLine3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLine3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLine3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine3.Location = new System.Drawing.Point(3, 3);
            this.uiLine3.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine3.Name = "uiLine3";
            this.uiLine3.Size = new System.Drawing.Size(328, 24);
            this.uiLine3.TabIndex = 55;
            this.uiLine3.Text = "Thời gian xử lí tối đa (ms)";
            this.uiLine3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel7
            // 
            this.uiTableLayoutPanel7.ColumnCount = 2;
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.uiTableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.uiTableLayoutPanel7.Controls.Add(this.ipProcess_TimeOut, 0, 0);
            this.uiTableLayoutPanel7.Controls.Add(this.opProcessing, 1, 0);
            this.uiTableLayoutPanel7.Location = new System.Drawing.Point(3, 33);
            this.uiTableLayoutPanel7.Name = "uiTableLayoutPanel7";
            this.uiTableLayoutPanel7.RowCount = 1;
            this.uiTableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel7.Size = new System.Drawing.Size(328, 50);
            this.uiTableLayoutPanel7.TabIndex = 0;
            this.uiTableLayoutPanel7.TagString = null;
            // 
            // ipProcess_TimeOut
            // 
            this.ipProcess_TimeOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipProcess_TimeOut.Font = new System.Drawing.Font("SimSun", 12F);
            this.ipProcess_TimeOut.Location = new System.Drawing.Point(0, 3);
            this.ipProcess_TimeOut.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.ipProcess_TimeOut.Maximum = 65535;
            this.ipProcess_TimeOut.Minimum = 0;
            this.ipProcess_TimeOut.MinimumSize = new System.Drawing.Size(100, 0);
            this.ipProcess_TimeOut.Name = "ipProcess_TimeOut";
            this.ipProcess_TimeOut.ShowText = false;
            this.ipProcess_TimeOut.Size = new System.Drawing.Size(183, 47);
            this.ipProcess_TimeOut.Step = 10;
            this.ipProcess_TimeOut.TabIndex = 8;
            this.ipProcess_TimeOut.Text = "_uiIntegerUpDown1";
            this.ipProcess_TimeOut.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ipProcess_TimeOut.Value = 10;
            // 
            // opProcessing
            // 
            this.opProcessing.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.opProcessing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opProcessing.DoubleValue = 1000D;
            this.opProcessing.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opProcessing.IntValue = 1000;
            this.opProcessing.Location = new System.Drawing.Point(186, 3);
            this.opProcessing.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.opProcessing.MinimumSize = new System.Drawing.Size(1, 16);
            this.opProcessing.Name = "opProcessing";
            this.opProcessing.Padding = new System.Windows.Forms.Padding(5);
            this.opProcessing.ReadOnly = true;
            this.opProcessing.ShowText = false;
            this.opProcessing.Size = new System.Drawing.Size(139, 47);
            this.opProcessing.TabIndex = 2;
            this.opProcessing.Text = "1000";
            this.opProcessing.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.opProcessing.Watermark = "";
            // 
            // uiTableLayoutPanel3
            // 
            this.uiTableLayoutPanel3.ColumnCount = 1;
            this.uiTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel3.Controls.Add(this.uiLine1, 0, 0);
            this.uiTableLayoutPanel3.Controls.Add(this.uiTableLayoutPanel4, 0, 1);
            this.uiTableLayoutPanel3.Location = new System.Drawing.Point(0, 85);
            this.uiTableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.uiTableLayoutPanel3.Name = "uiTableLayoutPanel3";
            this.uiTableLayoutPanel3.RowCount = 2;
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.uiTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.uiTableLayoutPanel3.Size = new System.Drawing.Size(334, 86);
            this.uiTableLayoutPanel3.TabIndex = 67;
            this.uiTableLayoutPanel3.TagString = null;
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLine1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(3, 3);
            this.uiLine1.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(328, 24);
            this.uiLine1.TabIndex = 55;
            this.uiLine1.Text = "Độ trễ bộ loại (xung)";
            this.uiLine1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel4
            // 
            this.uiTableLayoutPanel4.ColumnCount = 2;
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.uiTableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.uiTableLayoutPanel4.Controls.Add(this.ipDelay_Reject, 0, 0);
            this.uiTableLayoutPanel4.Controls.Add(this.opDelayReject, 1, 0);
            this.uiTableLayoutPanel4.Location = new System.Drawing.Point(3, 33);
            this.uiTableLayoutPanel4.Name = "uiTableLayoutPanel4";
            this.uiTableLayoutPanel4.RowCount = 1;
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel4.Size = new System.Drawing.Size(328, 50);
            this.uiTableLayoutPanel4.TabIndex = 0;
            this.uiTableLayoutPanel4.TagString = null;
            // 
            // ipDelay_Reject
            // 
            this.ipDelay_Reject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipDelay_Reject.Font = new System.Drawing.Font("SimSun", 12F);
            this.ipDelay_Reject.Location = new System.Drawing.Point(0, 3);
            this.ipDelay_Reject.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.ipDelay_Reject.Maximum = 65535;
            this.ipDelay_Reject.Minimum = 0;
            this.ipDelay_Reject.MinimumSize = new System.Drawing.Size(100, 0);
            this.ipDelay_Reject.Name = "ipDelay_Reject";
            this.ipDelay_Reject.ShowText = false;
            this.ipDelay_Reject.Size = new System.Drawing.Size(183, 47);
            this.ipDelay_Reject.Step = 10;
            this.ipDelay_Reject.TabIndex = 8;
            this.ipDelay_Reject.Text = "_uiIntegerUpDown1";
            this.ipDelay_Reject.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ipDelay_Reject.Value = 10;
            // 
            // opDelayReject
            // 
            this.opDelayReject.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.opDelayReject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opDelayReject.DoubleValue = 1000D;
            this.opDelayReject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opDelayReject.IntValue = 1000;
            this.opDelayReject.Location = new System.Drawing.Point(186, 3);
            this.opDelayReject.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.opDelayReject.MinimumSize = new System.Drawing.Size(1, 16);
            this.opDelayReject.Name = "opDelayReject";
            this.opDelayReject.Padding = new System.Windows.Forms.Padding(5);
            this.opDelayReject.ReadOnly = true;
            this.opDelayReject.ShowText = false;
            this.opDelayReject.Size = new System.Drawing.Size(139, 47);
            this.opDelayReject.TabIndex = 2;
            this.opDelayReject.Text = "1000";
            this.opDelayReject.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.opDelayReject.Watermark = "";
            // 
            // uiTableLayoutPanel5
            // 
            this.uiTableLayoutPanel5.ColumnCount = 1;
            this.uiTableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel5.Controls.Add(this.uiLine2, 0, 0);
            this.uiTableLayoutPanel5.Controls.Add(this.uiTableLayoutPanel2, 0, 1);
            this.uiTableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.uiTableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.uiTableLayoutPanel5.Name = "uiTableLayoutPanel5";
            this.uiTableLayoutPanel5.RowCount = 2;
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.uiTableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.uiTableLayoutPanel5.Size = new System.Drawing.Size(334, 85);
            this.uiTableLayoutPanel5.TabIndex = 66;
            this.uiTableLayoutPanel5.TagString = null;
            // 
            // uiLine2
            // 
            this.uiLine2.BackColor = System.Drawing.Color.Transparent;
            this.uiLine2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLine2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLine2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine2.Location = new System.Drawing.Point(3, 3);
            this.uiLine2.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine2.Name = "uiLine2";
            this.uiLine2.Size = new System.Drawing.Size(328, 23);
            this.uiLine2.TabIndex = 55;
            this.uiLine2.Text = "Độ trễ chụp (xung)";
            this.uiLine2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 2;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.uiTableLayoutPanel2.Controls.Add(this.ipDelay_Camera, 0, 0);
            this.uiTableLayoutPanel2.Controls.Add(this.opDelayCmr, 1, 0);
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(3, 32);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(328, 50);
            this.uiTableLayoutPanel2.TabIndex = 0;
            this.uiTableLayoutPanel2.TagString = null;
            // 
            // ipDelay_Camera
            // 
            this.ipDelay_Camera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipDelay_Camera.Font = new System.Drawing.Font("SimSun", 12F);
            this.ipDelay_Camera.Location = new System.Drawing.Point(0, 3);
            this.ipDelay_Camera.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.ipDelay_Camera.Maximum = 65535;
            this.ipDelay_Camera.Minimum = 0;
            this.ipDelay_Camera.MinimumSize = new System.Drawing.Size(100, 0);
            this.ipDelay_Camera.Name = "ipDelay_Camera";
            this.ipDelay_Camera.ShowText = false;
            this.ipDelay_Camera.Size = new System.Drawing.Size(183, 47);
            this.ipDelay_Camera.Step = 10;
            this.ipDelay_Camera.TabIndex = 8;
            this.ipDelay_Camera.Text = "_uiIntegerUpDown1";
            this.ipDelay_Camera.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ipDelay_Camera.Value = 10;
            // 
            // opDelayCmr
            // 
            this.opDelayCmr.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.opDelayCmr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opDelayCmr.DoubleValue = 1000D;
            this.opDelayCmr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opDelayCmr.IntValue = 1000;
            this.opDelayCmr.Location = new System.Drawing.Point(186, 3);
            this.opDelayCmr.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.opDelayCmr.MinimumSize = new System.Drawing.Size(1, 16);
            this.opDelayCmr.Name = "opDelayCmr";
            this.opDelayCmr.Padding = new System.Windows.Forms.Padding(5);
            this.opDelayCmr.ReadOnly = true;
            this.opDelayCmr.ShowText = false;
            this.opDelayCmr.Size = new System.Drawing.Size(139, 47);
            this.opDelayCmr.TabIndex = 2;
            this.opDelayCmr.Text = "1000";
            this.opDelayCmr.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.opDelayCmr.Watermark = "";
            // 
            // uiTableLayoutPanel12
            // 
            this.uiTableLayoutPanel12.ColumnCount = 1;
            this.uiTableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel12.Controls.Add(this.uiLine6, 0, 0);
            this.uiTableLayoutPanel12.Controls.Add(this.uiTableLayoutPanel13, 0, 1);
            this.uiTableLayoutPanel12.Location = new System.Drawing.Point(0, 257);
            this.uiTableLayoutPanel12.Margin = new System.Windows.Forms.Padding(0);
            this.uiTableLayoutPanel12.Name = "uiTableLayoutPanel12";
            this.uiTableLayoutPanel12.RowCount = 2;
            this.uiTableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.uiTableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.uiTableLayoutPanel12.Size = new System.Drawing.Size(334, 86);
            this.uiTableLayoutPanel12.TabIndex = 71;
            this.uiTableLayoutPanel12.TagString = null;
            // 
            // uiLine6
            // 
            this.uiLine6.BackColor = System.Drawing.Color.Transparent;
            this.uiLine6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLine6.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLine6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine6.Location = new System.Drawing.Point(3, 3);
            this.uiLine6.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine6.Name = "uiLine6";
            this.uiLine6.Size = new System.Drawing.Size(328, 24);
            this.uiLine6.TabIndex = 55;
            this.uiLine6.Text = "Chống nhiễu cảm biến chụp (xung)";
            this.uiLine6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel13
            // 
            this.uiTableLayoutPanel13.ColumnCount = 2;
            this.uiTableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.uiTableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.uiTableLayoutPanel13.Controls.Add(this.ipDebounce_Sensor, 0, 0);
            this.uiTableLayoutPanel13.Controls.Add(this.opDeboundSensor, 1, 0);
            this.uiTableLayoutPanel13.Location = new System.Drawing.Point(3, 33);
            this.uiTableLayoutPanel13.Name = "uiTableLayoutPanel13";
            this.uiTableLayoutPanel13.RowCount = 1;
            this.uiTableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel13.Size = new System.Drawing.Size(328, 50);
            this.uiTableLayoutPanel13.TabIndex = 0;
            this.uiTableLayoutPanel13.TagString = null;
            // 
            // ipDebounce_Sensor
            // 
            this.ipDebounce_Sensor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipDebounce_Sensor.Font = new System.Drawing.Font("SimSun", 12F);
            this.ipDebounce_Sensor.Location = new System.Drawing.Point(0, 3);
            this.ipDebounce_Sensor.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.ipDebounce_Sensor.Maximum = 65535;
            this.ipDebounce_Sensor.Minimum = 0;
            this.ipDebounce_Sensor.MinimumSize = new System.Drawing.Size(100, 0);
            this.ipDebounce_Sensor.Name = "ipDebounce_Sensor";
            this.ipDebounce_Sensor.ShowText = false;
            this.ipDebounce_Sensor.Size = new System.Drawing.Size(183, 47);
            this.ipDebounce_Sensor.Step = 10;
            this.ipDebounce_Sensor.TabIndex = 8;
            this.ipDebounce_Sensor.Text = "_uiIntegerUpDown1";
            this.ipDebounce_Sensor.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ipDebounce_Sensor.Value = 10;
            // 
            // opDeboundSensor
            // 
            this.opDeboundSensor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.opDeboundSensor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opDeboundSensor.DoubleValue = 1000D;
            this.opDeboundSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opDeboundSensor.IntValue = 1000;
            this.opDeboundSensor.Location = new System.Drawing.Point(186, 3);
            this.opDeboundSensor.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.opDeboundSensor.MinimumSize = new System.Drawing.Size(1, 16);
            this.opDeboundSensor.Name = "opDeboundSensor";
            this.opDeboundSensor.Padding = new System.Windows.Forms.Padding(5);
            this.opDeboundSensor.ReadOnly = true;
            this.opDeboundSensor.ShowText = false;
            this.opDeboundSensor.Size = new System.Drawing.Size(139, 47);
            this.opDeboundSensor.TabIndex = 2;
            this.opDeboundSensor.Text = "1000";
            this.opDeboundSensor.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.opDeboundSensor.Watermark = "";
            // 
            // uiTableLayoutPanel14
            // 
            this.uiTableLayoutPanel14.ColumnCount = 1;
            this.uiTableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel14.Controls.Add(this.uiLine7, 0, 0);
            this.uiTableLayoutPanel14.Controls.Add(this.uiTableLayoutPanel15, 0, 1);
            this.uiTableLayoutPanel14.Location = new System.Drawing.Point(0, 343);
            this.uiTableLayoutPanel14.Margin = new System.Windows.Forms.Padding(0);
            this.uiTableLayoutPanel14.Name = "uiTableLayoutPanel14";
            this.uiTableLayoutPanel14.RowCount = 2;
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.uiTableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.uiTableLayoutPanel14.Size = new System.Drawing.Size(334, 86);
            this.uiTableLayoutPanel14.TabIndex = 72;
            this.uiTableLayoutPanel14.TagString = null;
            // 
            // uiLine7
            // 
            this.uiLine7.BackColor = System.Drawing.Color.Transparent;
            this.uiLine7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLine7.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLine7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine7.Location = new System.Drawing.Point(3, 3);
            this.uiLine7.MinimumSize = new System.Drawing.Size(16, 16);
            this.uiLine7.Name = "uiLine7";
            this.uiLine7.Size = new System.Drawing.Size(328, 24);
            this.uiLine7.TabIndex = 55;
            this.uiLine7.Text = "Độ mạnh bộ đá (ms)";
            this.uiLine7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTableLayoutPanel15
            // 
            this.uiTableLayoutPanel15.ColumnCount = 2;
            this.uiTableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.uiTableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.uiTableLayoutPanel15.Controls.Add(this.ipReject_Streng, 0, 0);
            this.uiTableLayoutPanel15.Controls.Add(this.opRejectStreng, 1, 0);
            this.uiTableLayoutPanel15.Location = new System.Drawing.Point(3, 33);
            this.uiTableLayoutPanel15.Name = "uiTableLayoutPanel15";
            this.uiTableLayoutPanel15.RowCount = 1;
            this.uiTableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTableLayoutPanel15.Size = new System.Drawing.Size(328, 50);
            this.uiTableLayoutPanel15.TabIndex = 0;
            this.uiTableLayoutPanel15.TagString = null;
            // 
            // ipReject_Streng
            // 
            this.ipReject_Streng.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipReject_Streng.Font = new System.Drawing.Font("SimSun", 12F);
            this.ipReject_Streng.Location = new System.Drawing.Point(0, 3);
            this.ipReject_Streng.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.ipReject_Streng.Maximum = 65535;
            this.ipReject_Streng.Minimum = 0;
            this.ipReject_Streng.MinimumSize = new System.Drawing.Size(100, 0);
            this.ipReject_Streng.Name = "ipReject_Streng";
            this.ipReject_Streng.ShowText = false;
            this.ipReject_Streng.Size = new System.Drawing.Size(183, 47);
            this.ipReject_Streng.Step = 10;
            this.ipReject_Streng.TabIndex = 8;
            this.ipReject_Streng.Text = "_uiIntegerUpDown1";
            this.ipReject_Streng.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ipReject_Streng.Value = 10;
            // 
            // opRejectStreng
            // 
            this.opRejectStreng.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.opRejectStreng.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opRejectStreng.DoubleValue = 1000D;
            this.opRejectStreng.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opRejectStreng.IntValue = 1000;
            this.opRejectStreng.Location = new System.Drawing.Point(186, 3);
            this.opRejectStreng.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.opRejectStreng.MinimumSize = new System.Drawing.Size(1, 16);
            this.opRejectStreng.Name = "opRejectStreng";
            this.opRejectStreng.Padding = new System.Windows.Forms.Padding(5);
            this.opRejectStreng.ReadOnly = true;
            this.opRejectStreng.ShowText = false;
            this.opRejectStreng.Size = new System.Drawing.Size(139, 47);
            this.opRejectStreng.TabIndex = 2;
            this.opRejectStreng.Text = "1000";
            this.opRejectStreng.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.opRejectStreng.Watermark = "";
            // 
            // WKUpdate
            // 
            this.WKUpdate.WorkerReportsProgress = true;
            this.WKUpdate.WorkerSupportsCancellation = true;
            this.WKUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WKUpdate_DoWork);
            // 
            // WK_PLC
            // 
            this.WK_PLC.WorkerReportsProgress = true;
            this.WK_PLC.WorkerSupportsCancellation = true;
            this.WK_PLC.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_PLC_DoWork);
            this.WK_PLC.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WK_PLC_RunWorkerCompleted);
            // 
            // WK_Write_To_PLC
            // 
            this.WK_Write_To_PLC.WorkerReportsProgress = true;
            this.WK_Write_To_PLC.WorkerSupportsCancellation = true;
            this.WK_Write_To_PLC.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_Write_To_PLC_DoWork);
            this.WK_Write_To_PLC.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WK_Write_To_PLC_RunWorkerCompleted);
            // 
            // omronPLC_Hsl1
            // 
            this.omronPLC_Hsl1.PLC_IP = "192.168.250.1";
            this.omronPLC_Hsl1.PLC_PORT = 9600;
            this.omronPLC_Hsl1.PLC_Ready_DM = "D2025";
            this.omronPLC_Hsl1.PLC_STATUS = SPMS1.OmronPLC_Hsl.PLCStatus.Disconnect;
            this.omronPLC_Hsl1.Ready = 0;
            this.omronPLC_Hsl1.Time_Update = 10000;
            // 
            // ipopValueCustom
            // 
            this.ipopValueCustom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipopValueCustom.FillColor = System.Drawing.Color.White;
            this.ipopValueCustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ipopValueCustom.Location = new System.Drawing.Point(146, 2);
            this.ipopValueCustom.Margin = new System.Windows.Forms.Padding(2);
            this.ipopValueCustom.MinimumSize = new System.Drawing.Size(63, 0);
            this.ipopValueCustom.Name = "ipopValueCustom";
            this.ipopValueCustom.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ipopValueCustom.Size = new System.Drawing.Size(118, 47);
            this.ipopValueCustom.SymbolDropDown = 557532;
            this.ipopValueCustom.SymbolNormal = 557532;
            this.ipopValueCustom.SymbolSize = 24;
            this.ipopValueCustom.TabIndex = 6;
            this.ipopValueCustom.Text = "0";
            this.ipopValueCustom.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ipopValueCustom.Watermark = "";
            // 
            // F1PLC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(836, 652);
            this.Controls.Add(this.MainPanel);
            this.Name = "F1PLC";
            this.Symbol = 62171;
            this.Text = "Cài đặt PLC";
            this.Load += new System.EventHandler(this.FPLC_Load);
            this.MainPanel.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.uiTitlePanel2.ResumeLayout(false);
            this.uiTableLayoutPanel22.ResumeLayout(false);
            this.uiTableLayoutPanel23.ResumeLayout(false);
            this.uiTableLayoutPanel16.ResumeLayout(false);

            this.uiTitlePanel1.ResumeLayout(false);
            this.uiTableLayoutPanel1.ResumeLayout(false);
            this.uiTableLayoutPanel10.ResumeLayout(false);
            this.uiTableLayoutPanel11.ResumeLayout(false);
            this.uiTableLayoutPanel8.ResumeLayout(false);
            this.uiTableLayoutPanel9.ResumeLayout(false);
            this.uiTableLayoutPanel6.ResumeLayout(false);
            this.uiTableLayoutPanel7.ResumeLayout(false);
            this.uiTableLayoutPanel3.ResumeLayout(false);
            this.uiTableLayoutPanel4.ResumeLayout(false);
            this.uiTableLayoutPanel5.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.uiTableLayoutPanel12.ResumeLayout(false);
            this.uiTableLayoutPanel13.ResumeLayout(false);
            this.uiTableLayoutPanel14.ResumeLayout(false);
            this.uiTableLayoutPanel15.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITableLayoutPanel MainPanel;
        private Sunny.UI.UITableLayoutPanel panelRight;
        private Sunny.UI.UITitlePanel uiTitlePanel2;
        private Sunny.UI.UITitlePanel uiTitlePanel1;
        //private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel2;
        private Sunny.UI.UITextBox opDelayCmr;
        private Sunny.UI.UIIntegerUpDown ipDelay_Camera;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel5;
        private Sunny.UI.UILine uiLine2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel14;
        private Sunny.UI.UILine uiLine7;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel15;
        private Sunny.UI.UIIntegerUpDown ipReject_Streng;
        private Sunny.UI.UITextBox opRejectStreng;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel12;
        private Sunny.UI.UILine uiLine6;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel13;
        private Sunny.UI.UIIntegerUpDown ipDebounce_Sensor;
        private Sunny.UI.UITextBox opDeboundSensor;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel6;
        private Sunny.UI.UILine uiLine3;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel7;
        private Sunny.UI.UIIntegerUpDown ipProcess_TimeOut;
        private Sunny.UI.UITextBox opProcessing;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel3;
        private Sunny.UI.UILine uiLine1;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel4;
        private Sunny.UI.UIIntegerUpDown ipDelay_Reject;
        private Sunny.UI.UITextBox opDelayReject;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel22;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel23;
        private Sunny.UI.UISymbolButton btnSave;
        private Sunny.UI.UISymbolButton btnUndo;
        private Sunny.UI.UISymbolButton btnDelete;

        private System.ComponentModel.BackgroundWorker WKUpdate;

        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel10;
        private Sunny.UI.UILine uiLine5;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel11;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel8;
        private Sunny.UI.UILine uiLine4;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel9;
        private Sunny.UI.UIIntegerUpDown ipProduct_Length;
        private Sunny.UI.UITextBox opProductLenght;
        private Sunny.UI.UINumPadTextBox ipDCustom;
        private Sunny.UI.UISymbolButton btnCustom;
        private Sunny.UI.UIPanel ipChange_Read_Write_Mode;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel16;
        private Sunny.UI.UISymbolButton btnNewRecipe;
        private Sunny.UI.UIComboBox cbbRecipe;
        private SPMS1.OmronPLC_Hsl omronPLC_Hsl1;
        private System.ComponentModel.BackgroundWorker WK_PLC;
        private System.ComponentModel.BackgroundWorker WK_Write_To_PLC;
        private Sunny.UI.UINumPadTextBox ipopValueCustom;
    }
}