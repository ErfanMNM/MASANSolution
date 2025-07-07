namespace QR_MASAN_01.Views.Settings
{
    partial class FAppSetting
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
            this.treeView = new Sunny.UI.UITreeView();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.FillColor = System.Drawing.Color.White;
            this.treeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.treeView.Location = new System.Drawing.Point(3, 4);
            this.treeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeView.MinimumSize = new System.Drawing.Size(1, 1);
            this.treeView.Name = "treeView";
            this.treeView.ScrollBarStyleInherited = false;
            this.treeView.ShowText = false;
            this.treeView.Size = new System.Drawing.Size(834, 665);
            this.treeView.TabIndex = 0;
            this.treeView.Text = "uiTreeView1";
            this.treeView.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            // 
            // FAppSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.treeView);
            this.Name = "FAppSetting";
            this.Text = "FAppSetting";
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITreeView treeView;
    }
}