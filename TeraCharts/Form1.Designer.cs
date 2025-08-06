namespace TeraCharts
{
    partial class Form1
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
            this.ucBarChartViewer1 = new TeraCharts.Barcharts.UcBarChartViewer();
            this.btnUpdateData = new System.Windows.Forms.Button();
            this.btnChangeTitle = new System.Windows.Forms.Button();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbTemplate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ucBarChartViewer1
            // 
            this.ucBarChartViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ucBarChartViewer1.ChartTitle = "Biểu Đồ Demo";
            this.ucBarChartViewer1.Location = new System.Drawing.Point(12, 60);
            this.ucBarChartViewer1.Name = "ucBarChartViewer1";
            this.ucBarChartViewer1.Size = new System.Drawing.Size(884, 426);
            this.ucBarChartViewer1.TabIndex = 0;
            // 
            // btnUpdateData
            // 
            this.btnUpdateData.Location = new System.Drawing.Point(216, 8);
            this.btnUpdateData.Name = "btnUpdateData";
            this.btnUpdateData.Size = new System.Drawing.Size(100, 23);
            this.btnUpdateData.TabIndex = 1;
            this.btnUpdateData.Text = "Cập nhật dữ liệu";
            this.btnUpdateData.UseVisualStyleBackColor = true;
            this.btnUpdateData.Click += new System.EventHandler(this.btnUpdateData_Click);
            // 
            // btnChangeTitle
            // 
            this.btnChangeTitle.Location = new System.Drawing.Point(216, 34);
            this.btnChangeTitle.Name = "btnChangeTitle";
            this.btnChangeTitle.Size = new System.Drawing.Size(100, 23);
            this.btnChangeTitle.TabIndex = 2;
            this.btnChangeTitle.Text = "Đổi tiêu đề";
            this.btnChangeTitle.UseVisualStyleBackColor = true;
            this.btnChangeTitle.Click += new System.EventHandler(this.btnChangeTitle_Click);
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(60, 36);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(150, 20);
            this.txtTitle.TabIndex = 3;
            this.txtTitle.Text = "Biểu Đồ Tùy Chỉnh";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tiêu đề:";
            // 
            // cmbTemplate
            // 
            this.cmbTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTemplate.FormattingEnabled = true;
            this.cmbTemplate.Location = new System.Drawing.Point(400, 10);
            this.cmbTemplate.Name = "cmbTemplate";
            this.cmbTemplate.Size = new System.Drawing.Size(150, 21);
            this.cmbTemplate.TabIndex = 6;
            this.cmbTemplate.SelectedIndexChanged += new System.EventHandler(this.cmbTemplate_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Template:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbTemplate);
            this.panel1.Controls.Add(this.btnUpdateData);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnChangeTitle);
            this.panel1.Controls.Add(this.txtTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(908, 65);
            this.panel1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 498);
            this.Controls.Add(this.ucBarChartViewer1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "TeraCharts - ECharts Demo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TeraCharts.Barcharts.UcBarChartViewer ucBarChartViewer1;
        private System.Windows.Forms.Button btnUpdateData;
        private System.Windows.Forms.Button btnChangeTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cmbTemplate;
        private System.Windows.Forms.Label label2;
    }
}