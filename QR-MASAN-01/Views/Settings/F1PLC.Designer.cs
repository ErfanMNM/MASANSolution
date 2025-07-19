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
            this.WKUpdate = new System.ComponentModel.BackgroundWorker();
            this.WK_PLC = new System.ComponentModel.BackgroundWorker();
            this.WK_Write_To_PLC = new System.ComponentModel.BackgroundWorker();
            this.omronPLC_Hsl1 = new SPMS1.OmronPLC_Hsl(this.components);
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.MainPanel = new Sunny.UI.UITableLayoutPanel();
            this.panelRight = new Sunny.UI.UITableLayoutPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.uiTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.SuspendLayout();
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
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPage1);
            this.uiTabControl1.Controls.Add(this.tabPage2);
            this.uiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiTabControl1.ItemSize = new System.Drawing.Size(150, 40);
            this.uiTabControl1.Location = new System.Drawing.Point(0, 0);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(836, 652);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 0;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.MainPanel);
            this.tabPage1.Location = new System.Drawing.Point(0, 40);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(836, 612);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Camera Trước";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // MainPanel
            // 
            this.MainPanel.ColumnCount = 2;
            this.MainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.86603F));
            this.MainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.13397F));
            this.MainPanel.Controls.Add(this.panelRight, 1, 0);
            this.MainPanel.Location = new System.Drawing.Point(3, 3);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.RowCount = 1;
            this.MainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainPanel.Size = new System.Drawing.Size(836, 606);
            this.MainPanel.TabIndex = 1;
            this.MainPanel.TagString = null;
            // 
            // panelRight
            // 
            this.panelRight.ColumnCount = 1;
            this.panelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(353, 3);
            this.panelRight.Name = "panelRight";
            this.panelRight.RowCount = 2;
            this.panelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61.83333F));
            this.panelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.16667F));
            this.panelRight.Size = new System.Drawing.Size(480, 600);
            this.panelRight.TabIndex = 0;
            this.panelRight.TagString = null;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(0, 40);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(200, 60);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // F1PLC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(836, 652);
            this.Controls.Add(this.uiTabControl1);
            this.Name = "F1PLC";
            this.Symbol = 62171;
            this.Text = "Cài đặt PLC";
            this.Load += new System.EventHandler(this.FPLC_Load);
            this.uiTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker WKUpdate;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private SPMS1.OmronPLC_Hsl omronPLC_Hsl1;
        private System.ComponentModel.BackgroundWorker WK_PLC;
        private System.ComponentModel.BackgroundWorker WK_Write_To_PLC;
        private Sunny.UI.UITabControl uiTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Sunny.UI.UITableLayoutPanel MainPanel;
        private Sunny.UI.UITableLayoutPanel panelRight;
        private System.Windows.Forms.TabPage tabPage2;
    }
}