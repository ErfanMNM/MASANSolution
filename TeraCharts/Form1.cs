using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeraCharts.Barcharts;

namespace TeraCharts
{
    public partial class Form1 : Form
    {
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            cmbTemplate.Items.Clear();
            
            // Add new offline templates first
            cmbTemplate.Items.Add("bar-chart-offline.html");
            cmbTemplate.Items.Add("line-chart-offline.html");
            cmbTemplate.Items.Add("pie-chart-offline.html");
            cmbTemplate.Items.Add("bar-label-rotation.html");
            
            string chartPath = Path.Combine(Application.StartupPath, "ChartCS");
            if (Directory.Exists(chartPath))
            {
                var htmlFiles = Directory.GetFiles(chartPath, "*.html")
                    .Select(f => Path.GetFileName(f))
                    .Where(f => !cmbTemplate.Items.Contains(f));
                
                foreach (var file in htmlFiles)
                {
                    cmbTemplate.Items.Add(file);
                }
            }
            
            if (cmbTemplate.Items.Count > 0)
            {
                cmbTemplate.SelectedIndex = 0; // Default to bar-chart-offline.html
            }
        }

        private void btnUpdateData_Click(object sender, EventArgs e)
        {
            var categories = new List<string> { "Q1 2024", "Q2 2024", "Q3 2024", "Q4 2024", "Q1 2025" };
            
            var seriesData = new List<SeriesData>
            {
                new SeriesData
                {
                    Name = "Doanh Thu Thực Tế",
                    Data = GenerateRandomData(5, 100, 500),
                    Color = "#5470c6"
                },
                new SeriesData
                {
                    Name = "Kế Hoạch Doanh Thu",
                    Data = GenerateRandomData(5, 120, 480),
                    Color = "#91cc75"
                },
                new SeriesData
                {
                    Name = "Lợi Nhuận",
                    Data = GenerateRandomData(5, 50, 250),
                    Color = "#fac858"
                }
            };

            ucBarChartViewer1.SetData(categories, seriesData);
        }

        private void btnChangeTitle_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                ucBarChartViewer1.ChartTitle = txtTitle.Text;
            }
        }

        private List<int> GenerateRandomData(int count, int min, int max)
        {
            var data = new List<int>();
            for (int i = 0; i < count; i++)
            {
                data.Add(random.Next(min, max));
            }
            return data;
        }

        private void cmbTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTemplate.SelectedItem != null)
            {
                ucBarChartViewer1.TemplateFile = cmbTemplate.SelectedItem.ToString();
            }
        }
    }
}
