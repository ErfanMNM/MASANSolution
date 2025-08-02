namespace TestApp
{
    partial class FMain
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
            this.opClock = new Sunny.UI.UIPanel();
            this.footer = new Sunny.UI.UITableLayoutPanel();
            this.NavMenu = new Sunny.UI.UINavMenu();
            this.BodyPanel = new Sunny.UI.UITableLayoutPanel();
            this.navPanel = new Sunny.UI.UITableLayoutPanel();
            this.btnAppClose = new Sunny.UI.UISymbolButton();
            this.btnMini = new Sunny.UI.UISymbolButton();
            this.TabBody = new Sunny.UI.UITabControl();
            this.mainPanelLayout = new Sunny.UI.UITableLayoutPanel();
            this.footer.SuspendLayout();
            this.BodyPanel.SuspendLayout();
            this.navPanel.SuspendLayout();
            this.mainPanelLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // opClock
            // 
            this.opClock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opClock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.opClock.Location = new System.Drawing.Point(858, 0);
            this.opClock.Margin = new System.Windows.Forms.Padding(0);
            this.opClock.MinimumSize = new System.Drawing.Size(1, 1);
            this.opClock.Name = "opClock";
            this.opClock.Radius = 0;
            this.opClock.Size = new System.Drawing.Size(166, 42);
            this.opClock.TabIndex = 1;
            this.opClock.Text = "20/11/2024 17:27:00.000";
            this.opClock.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // footer
            // 
            this.footer.BackColor = System.Drawing.Color.Green;
            this.footer.ColumnCount = 5;
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 183F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 296F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 205F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 166F));
            this.footer.Controls.Add(this.opClock, 4, 0);
            this.footer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footer.Location = new System.Drawing.Point(0, 726);
            this.footer.Margin = new System.Windows.Forms.Padding(0);
            this.footer.Name = "footer";
            this.footer.RowCount = 1;
            this.footer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footer.Size = new System.Drawing.Size(1024, 42);
            this.footer.TabIndex = 3;
            this.footer.TagString = null;
            // 
            // NavMenu
            // 
            this.NavMenu.BackColor = System.Drawing.Color.LightBlue;
            this.NavMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NavMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NavMenu.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.NavMenu.FillColor = System.Drawing.Color.LightBlue;
            this.NavMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.NavMenu.ForeColor = System.Drawing.Color.Black;
            this.NavMenu.FullRowSelect = true;
            this.NavMenu.HotTracking = true;
            this.NavMenu.HoverColor = System.Drawing.Color.DeepSkyBlue;
            this.NavMenu.ItemHeight = 40;
            this.NavMenu.LineColor = System.Drawing.Color.White;
            this.NavMenu.Location = new System.Drawing.Point(0, 0);
            this.NavMenu.Margin = new System.Windows.Forms.Padding(0);
            this.NavMenu.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.NavMenu.Name = "NavMenu";
            this.NavMenu.SelectedColor = System.Drawing.Color.DodgerBlue;
            this.NavMenu.SelectedColor2 = System.Drawing.Color.DodgerBlue;
            this.NavMenu.SelectedForeColor = System.Drawing.SystemColors.HighlightText;
            this.NavMenu.SelectedHighColor = System.Drawing.Color.Blue;
            this.NavMenu.ShowLines = false;
            this.NavMenu.ShowPlusMinus = false;
            this.NavMenu.ShowRootLines = false;
            this.NavMenu.ShowTips = true;
            this.NavMenu.Size = new System.Drawing.Size(184, 637);
            this.NavMenu.TabIndex = 0;
            this.NavMenu.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // BodyPanel
            // 
            this.BodyPanel.ColumnCount = 2;
            this.BodyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.BodyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82F));
            this.BodyPanel.Controls.Add(this.navPanel, 0, 0);
            this.BodyPanel.Controls.Add(this.TabBody, 1, 0);
            this.BodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BodyPanel.Location = new System.Drawing.Point(0, 0);
            this.BodyPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BodyPanel.Name = "BodyPanel";
            this.BodyPanel.RowCount = 1;
            this.BodyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.BodyPanel.Size = new System.Drawing.Size(1024, 726);
            this.BodyPanel.TabIndex = 2;
            this.BodyPanel.TagString = null;
            // 
            // navPanel
            // 
            this.navPanel.BackColor = System.Drawing.Color.LightBlue;
            this.navPanel.ColumnCount = 1;
            this.navPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.navPanel.Controls.Add(this.NavMenu, 0, 0);
            this.navPanel.Controls.Add(this.btnAppClose, 0, 2);
            this.navPanel.Controls.Add(this.btnMini, 0, 1);
            this.navPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navPanel.Location = new System.Drawing.Point(0, 0);
            this.navPanel.Margin = new System.Windows.Forms.Padding(0);
            this.navPanel.Name = "navPanel";
            this.navPanel.RowCount = 3;
            this.navPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.navPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.navPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.navPanel.Size = new System.Drawing.Size(184, 726);
            this.navPanel.TabIndex = 2;
            this.navPanel.TagString = null;
            // 
            // btnAppClose
            // 
            this.btnAppClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAppClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAppClose.ForeColor = System.Drawing.Color.MistyRose;
            this.btnAppClose.Location = new System.Drawing.Point(2, 687);
            this.btnAppClose.Margin = new System.Windows.Forms.Padding(2);
            this.btnAppClose.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAppClose.Name = "btnAppClose";
            this.btnAppClose.Radius = 0;
            this.btnAppClose.RectColor = System.Drawing.Color.Blue;
            this.btnAppClose.Size = new System.Drawing.Size(180, 37);
            this.btnAppClose.Symbol = 61457;
            this.btnAppClose.SymbolColor = System.Drawing.Color.MistyRose;
            this.btnAppClose.TabIndex = 0;
            this.btnAppClose.Text = "Đóng";
            this.btnAppClose.TipsColor = System.Drawing.Color.RoyalBlue;
            this.btnAppClose.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAppClose.Click += new System.EventHandler(this.btnAppClose_Click);
            // 
            // btnMini
            // 
            this.btnMini.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMini.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnMini.Location = new System.Drawing.Point(2, 639);
            this.btnMini.Margin = new System.Windows.Forms.Padding(2);
            this.btnMini.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnMini.Name = "btnMini";
            this.btnMini.Radius = 0;
            this.btnMini.RectColor = System.Drawing.Color.Blue;
            this.btnMini.Size = new System.Drawing.Size(180, 44);
            this.btnMini.Symbol = 61544;
            this.btnMini.TabIndex = 3;
            this.btnMini.Text = "Thu nhỏ";
            this.btnMini.TipsColor = System.Drawing.Color.RoyalBlue;
            this.btnMini.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnMini.Click += new System.EventHandler(this.btnMini_Click);
            // 
            // TabBody
            // 
            this.TabBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabBody.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.TabBody.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.TabBody.ItemSize = new System.Drawing.Size(0, 1);
            this.TabBody.Location = new System.Drawing.Point(184, 0);
            this.TabBody.MainPage = "";
            this.TabBody.Margin = new System.Windows.Forms.Padding(0);
            this.TabBody.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.TabBody.Name = "TabBody";
            this.TabBody.SelectedIndex = 0;
            this.TabBody.Size = new System.Drawing.Size(840, 726);
            this.TabBody.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabBody.TabBackColor = System.Drawing.Color.PaleTurquoise;
            this.TabBody.TabIndex = 1;
            this.TabBody.TabVisible = false;
            this.TabBody.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // mainPanelLayout
            // 
            this.mainPanelLayout.BackColor = System.Drawing.Color.AliceBlue;
            this.mainPanelLayout.ColumnCount = 1;
            this.mainPanelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanelLayout.Controls.Add(this.BodyPanel, 0, 0);
            this.mainPanelLayout.Controls.Add(this.footer, 0, 1);
            this.mainPanelLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanelLayout.Location = new System.Drawing.Point(0, 0);
            this.mainPanelLayout.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanelLayout.Name = "mainPanelLayout";
            this.mainPanelLayout.RowCount = 2;
            this.mainPanelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.76041F));
            this.mainPanelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.mainPanelLayout.Size = new System.Drawing.Size(1024, 768);
            this.mainPanelLayout.TabIndex = 1;
            this.mainPanelLayout.TagString = null;
            // 
            // FMain
            // 
            this.AllowShowTitle = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.mainPanelLayout);
            this.MaximumSize = new System.Drawing.Size(1024, 768);
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "FMain";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.ShowTitle = false;
            this.Text = "Main Form";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 677, 462);
            this.Load += new System.EventHandler(this.FMain_Load);
            this.footer.ResumeLayout(false);
            this.BodyPanel.ResumeLayout(false);
            this.navPanel.ResumeLayout(false);
            this.mainPanelLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Sunny.UI.UIPanel opClock;
        private Sunny.UI.UITableLayoutPanel footer;
        private Sunny.UI.UINavMenu NavMenu;
        private Sunny.UI.UITableLayoutPanel BodyPanel;
        private Sunny.UI.UITableLayoutPanel navPanel;
        private Sunny.UI.UISymbolButton btnAppClose;
        private Sunny.UI.UISymbolButton btnMini;
        private Sunny.UI.UITabControl TabBody;
        private Sunny.UI.UITableLayoutPanel mainPanelLayout;
    }
}

