namespace CProject
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MainTabBody = new Sunny.UI.UITabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            MainNavMenu = new Sunny.UI.UINavMenu();
            uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            MainTabBody.SuspendLayout();
            uiTableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // MainTabBody
            // 
            MainTabBody.Controls.Add(tabPage1);
            MainTabBody.Controls.Add(tabPage2);
            MainTabBody.Dock = DockStyle.Fill;
            MainTabBody.DrawMode = TabDrawMode.OwnerDrawFixed;
            MainTabBody.Font = new Font("Microsoft Sans Serif", 12F);
            MainTabBody.ItemSize = new Size(0, 1);
            MainTabBody.Location = new Point(160, 3);
            MainTabBody.MainPage = "";
            MainTabBody.Name = "MainTabBody";
            MainTabBody.SelectedIndex = 0;
            MainTabBody.Size = new Size(893, 573);
            MainTabBody.SizeMode = TabSizeMode.Fixed;
            MainTabBody.TabIndex = 0;
            MainTabBody.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            MainTabBody.TabVisible = false;
            MainTabBody.TipsFont = new Font("Microsoft Sans Serif", 9F);
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(0, 0);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(893, 573);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(0, 40);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(200, 60);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // MainNavMenu
            // 
            MainNavMenu.BorderStyle = BorderStyle.None;
            MainNavMenu.Dock = DockStyle.Fill;
            MainNavMenu.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            MainNavMenu.Font = new Font("Microsoft Sans Serif", 12F);
            MainNavMenu.FullRowSelect = true;
            MainNavMenu.HotTracking = true;
            MainNavMenu.ItemHeight = 50;
            MainNavMenu.Location = new Point(3, 3);
            MainNavMenu.Name = "MainNavMenu";
            MainNavMenu.ShowLines = false;
            MainNavMenu.ShowPlusMinus = false;
            MainNavMenu.ShowRootLines = false;
            MainNavMenu.Size = new Size(151, 573);
            MainNavMenu.TabIndex = 1;
            MainNavMenu.TipsFont = new Font("Microsoft Sans Serif", 9F);
            // 
            // uiTableLayoutPanel1
            // 
            uiTableLayoutPanel1.ColumnCount = 2;
            uiTableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.867424F));
            uiTableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85.132576F));
            uiTableLayoutPanel1.Controls.Add(MainNavMenu, 0, 0);
            uiTableLayoutPanel1.Controls.Add(MainTabBody, 1, 0);
            uiTableLayoutPanel1.Dock = DockStyle.Fill;
            uiTableLayoutPanel1.Location = new Point(0, 35);
            uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            uiTableLayoutPanel1.RowCount = 1;
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel1.Size = new Size(1056, 579);
            uiTableLayoutPanel1.TabIndex = 2;
            uiTableLayoutPanel1.TagString = null;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1056, 614);
            Controls.Add(uiTableLayoutPanel1);
            Name = "MainForm";
            Text = "Form1";
            ZoomScaleRect = new Rectangle(15, 15, 800, 450);
            MainTabBody.ResumeLayout(false);
            uiTableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UITabControl MainTabBody;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Sunny.UI.UINavMenu MainNavMenu;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
    }
}
