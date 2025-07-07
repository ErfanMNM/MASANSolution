namespace Dialogs
{
    partial class Pom_dialog
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
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.uiRichTextBox1 = new Sunny.UI.UIRichTextBox();
            this.uiNumPadTextBox1 = new Sunny.UI.UINumPadTextBox();
            this.btnOK = new Sunny.UI.UISymbolButton();
            this.btnCancel = new Sunny.UI.UISymbolButton();
            this.pnConnect = new Sunny.UI.UIPanel();
            this.uiTitlePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.uiRichTextBox1);
            this.uiTitlePanel1.Controls.Add(this.uiNumPadTextBox1);
            this.uiTitlePanel1.Controls.Add(this.btnOK);
            this.uiTitlePanel1.Controls.Add(this.btnCancel);
            this.uiTitlePanel1.Controls.Add(this.pnConnect);
            this.uiTitlePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitlePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F);
            this.uiTitlePanel1.Location = new System.Drawing.Point(0, 0);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(1, 50, 1, 1);
            this.uiTitlePanel1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.uiTitlePanel1.RectSize = 2;
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(606, 320);
            this.uiTitlePanel1.TabIndex = 0;
            this.uiTitlePanel1.Text = "THÔNG BÁO";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.TitleHeight = 50;
            // 
            // uiRichTextBox1
            // 
            this.uiRichTextBox1.FillColor = System.Drawing.Color.White;
            this.uiRichTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiRichTextBox1.Location = new System.Drawing.Point(5, 55);
            this.uiRichTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiRichTextBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiRichTextBox1.Name = "uiRichTextBox1";
            this.uiRichTextBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiRichTextBox1.ReadOnly = true;
            this.uiRichTextBox1.ShowText = false;
            this.uiRichTextBox1.Size = new System.Drawing.Size(596, 202);
            this.uiRichTextBox1.TabIndex = 4;
            this.uiRichTextBox1.Text = "Bạn có chắc chắn thay đổi thông tin PO?  \nHệ thống sẽ dừng khi đang chỉnh thông t" +
    "in!\nVui lòng nhập mã xác thực và nhấn Đồng Ý.";
            this.uiRichTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiNumPadTextBox1
            // 
            this.uiNumPadTextBox1.DecimalPlaces = 0;
            this.uiNumPadTextBox1.FillColor = System.Drawing.Color.White;
            this.uiNumPadTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiNumPadTextBox1.Location = new System.Drawing.Point(118, 265);
            this.uiNumPadTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiNumPadTextBox1.Maximum = 999999D;
            this.uiNumPadTextBox1.Minimum = 0D;
            this.uiNumPadTextBox1.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiNumPadTextBox1.Name = "uiNumPadTextBox1";
            this.uiNumPadTextBox1.NumPadType = Sunny.UI.NumPadType.Integer;
            this.uiNumPadTextBox1.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiNumPadTextBox1.Size = new System.Drawing.Size(206, 49);
            this.uiNumPadTextBox1.SymbolDropDown = 557532;
            this.uiNumPadTextBox1.SymbolNormal = 557532;
            this.uiNumPadTextBox1.SymbolSize = 24;
            this.uiNumPadTextBox1.TabIndex = 3;
            this.uiNumPadTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiNumPadTextBox1.Watermark = "";
            // 
            // btnOK
            // 
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnOK.Location = new System.Drawing.Point(431, 265);
            this.btnOK.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(170, 51);
            this.btnOK.Symbol = 61452;
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Đồng Ý";
            this.btnOK.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCancel.Location = new System.Drawing.Point(331, 265);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 51);
            this.btnCancel.Symbol = 61453;
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Thoát";
            this.btnCancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnConnect
            // 
            this.pnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnConnect.Location = new System.Drawing.Point(5, 265);
            this.pnConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnConnect.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnConnect.Name = "pnConnect";
            this.pnConnect.Size = new System.Drawing.Size(105, 49);
            this.pnConnect.TabIndex = 0;
            this.pnConnect.Text = "Mã xác thực";
            this.pnConnect.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Pom_dialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(606, 320);
            this.Controls.Add(this.uiTitlePanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Pom_dialog";
            this.Text = "THÔNG BÁO";
            this.uiTitlePanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UIPanel pnConnect;
        private Sunny.UI.UISymbolButton btnOK;
        private Sunny.UI.UISymbolButton btnCancel;
        private Sunny.UI.UIRichTextBox uiRichTextBox1;
        private Sunny.UI.UINumPadTextBox uiNumPadTextBox1;
    }
}