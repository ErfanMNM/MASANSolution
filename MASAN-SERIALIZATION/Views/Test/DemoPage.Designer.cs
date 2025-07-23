using System.Drawing;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Test
{
    partial class DemoPage
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
            this.toolbar = new System.Windows.Forms.Panel();
            this.btnLoadTemplate = new System.Windows.Forms.Button();
            this.btnAddLabel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolbar
            // 
            this.toolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.toolbar.Controls.Add(this.btnLoadTemplate);
            this.toolbar.Controls.Add(this.btnAddLabel);
            this.toolbar.Controls.Add(this.btnPrint);
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbar.Location = new System.Drawing.Point(0, 0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(800, 50);
            this.toolbar.TabIndex = 0;
            // 
            // btnLoadTemplate
            // 
            this.btnLoadTemplate.ForeColor = System.Drawing.Color.White;
            this.btnLoadTemplate.Location = new System.Drawing.Point(12, 10);
            this.btnLoadTemplate.Name = "btnLoadTemplate";
            this.btnLoadTemplate.Size = new System.Drawing.Size(120, 30);
            this.btnLoadTemplate.TabIndex = 0;
            this.btnLoadTemplate.Text = "Load Template";
            this.btnLoadTemplate.Click += new System.EventHandler(this.BtnLoadTemplate_Click);
            // 
            // btnAddLabel
            // 
            this.btnAddLabel.Enabled = false;
            this.btnAddLabel.ForeColor = System.Drawing.Color.White;
            this.btnAddLabel.Location = new System.Drawing.Point(140, 10);
            this.btnAddLabel.Name = "btnAddLabel";
            this.btnAddLabel.Size = new System.Drawing.Size(120, 30);
            this.btnAddLabel.TabIndex = 1;
            this.btnAddLabel.Text = "Add Label";
            this.btnAddLabel.Click += new System.EventHandler(this.BtnAddLabel_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(270, 10);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(120, 30);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print Preview";
            this.btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanel.Size = new System.Drawing.Size(800, 600);
            this.flowLayoutPanel.TabIndex = 1;
            // 
            // DemoPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.toolbar);
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "DemoPage";
            this.Text = "Label Renderer - .NET 4.8";
            this.toolbar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel toolbar;
    }
}