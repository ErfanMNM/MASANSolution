namespace MASAN_SERIALIZATION.Views.AWS
{
    partial class PAws
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
            this.opConsole = new Sunny.UI.UIListBox();
            this.SuspendLayout();
            // 
            // opConsole
            // 
            this.opConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opConsole.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.opConsole.ItemSelectForeColor = System.Drawing.Color.White;
            this.opConsole.Location = new System.Drawing.Point(13, 2);
            this.opConsole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.opConsole.MinimumSize = new System.Drawing.Size(1, 1);
            this.opConsole.Name = "opConsole";
            this.opConsole.Padding = new System.Windows.Forms.Padding(2);
            this.opConsole.ShowText = false;
            this.opConsole.Size = new System.Drawing.Size(798, 501);
            this.opConsole.TabIndex = 0;
            this.opConsole.Text = "uiListBox1";
            // 
            // PAws
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(824, 635);
            this.Controls.Add(this.opConsole);
            this.Name = "PAws";
            this.Symbol = 162325;
            this.Text = "AWS";
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIListBox opConsole;
    }
}