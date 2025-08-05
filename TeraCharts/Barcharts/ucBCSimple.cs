using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeraCharts.Barcharts
{
    public partial class ucBCSimple : UserControl
    {
        public string DataSource { get; set; } = @"D:\Work\ChartCS";
        public ucBCSimple()
        {
            InitializeComponent();
        }

        private void ucBCSimple_Load(object sender, EventArgs e)
        {
            string tempPath = System.IO.Path.Combine(DataSource, "bar-label-rotation.html");
            webView21.Source = new Uri(tempPath);
        }
    }
}
