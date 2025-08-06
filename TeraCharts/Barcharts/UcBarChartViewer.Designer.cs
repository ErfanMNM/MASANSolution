namespace TeraCharts.Barcharts
{
    partial class UcBarChartViewer
    {
        private System.ComponentModel.IContainer components = null;



        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(600, 488);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            // 
            // UcBarChartViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webView);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UcBarChartViewer";
            this.Size = new System.Drawing.Size(600, 488);
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
    }
}