
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TeraCharts.Barcharts
{
    public partial class UcBarChartViewer : UserControl
    {
        private WebView2 webView;
        private string _chartTitle = "Bar Chart";
        private List<string> _categories = new List<string> { "Category 1", "Category 2", "Category 3", "Category 4", "Category 5" };
        private List<SeriesData> _seriesData = new List<SeriesData>();
        private string _templateFile = "bar-chart-offline.html";
        private string _tempHtmlFile;

        public UcBarChartViewer()
        {
            InitializeComponent();
            InitializeWebView();
            InitializeDemoData();
        }

        [Category("Chart Settings")]
        [Description("Title of the chart")]
        public string ChartTitle
        {
            get => _chartTitle;
            set
            {
                _chartTitle = value;
                if (webView != null && webView.CoreWebView2 != null)
                    UpdateChart();
            }
        }

        [Category("Chart Data")]
        [Description("Categories for X-axis")]
        [TypeConverter(typeof(CollectionConverter))]
        public List<string> Categories
        {
            get => _categories;
            set
            {
                _categories = value ?? new List<string>();
                if (webView != null && webView.CoreWebView2 != null)
                    UpdateChart();
            }
        }

        [Category("Chart Data")]
        [Description("Series data for the chart")]
        [TypeConverter(typeof(CollectionConverter))]
        public List<SeriesData> SeriesData
        {
            get => _seriesData;
            set
            {
                _seriesData = value ?? new List<SeriesData>();
                if (webView != null && webView.CoreWebView2 != null)
                    UpdateChart();
            }
        }

        [Category("Chart Settings")]
        [Description("HTML template file name in ChartCS folder")]
        public string TemplateFile
        {
            get => _templateFile;
            set
            {
                _templateFile = value ?? "bar-chart-offline.html";
                if (webView != null && webView.CoreWebView2 != null)
                    LoadChart();
            }
        }

        private void InitializeWebView()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(webView);

            webView.NavigationCompleted += WebView_NavigationCompleted;
        }

        private void InitializeDemoData()
        {
            _seriesData = new List<SeriesData>
            {
                new SeriesData
                {
                    Name = "Doanh Thu",
                    Data = new List<int> { 320, 332, 301, 334, 390 },
                    Color = "#5470c6"
                },
                new SeriesData
                {
                    Name = "Lợi Nhuận",
                    Data = new List<int> { 220, 182, 191, 234, 290 },
                    Color = "#91cc75"
                },
                new SeriesData
                {
                    Name = "Chi Phí",
                    Data = new List<int> { 150, 232, 201, 154, 190 },
                    Color = "#fac858"
                }
            };

            _categories = new List<string> { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5" };
            _templateFile = "bar-chart-offline.html"; // Default to new offline template
        }

        public void LoadChart()
        {
            try
            {
                string templatePath = GetTemplatePath();
                if (File.Exists(templatePath))
                {
                    string htmlContent = File.ReadAllText(templatePath);
                    string processedHtml = ProcessTemplate(htmlContent);
                    
                    _tempHtmlFile = Path.GetTempFileName() + ".html";
                    File.WriteAllText(_tempHtmlFile, processedHtml);
                    
                    webView.Source = new Uri(_tempHtmlFile);
                }
                else
                {
                    MessageBox.Show($"Template file not found: {templatePath}", "Template Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading template: {ex.Message}", "Template Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetTemplatePath()
        {
            string appPath = Application.StartupPath;
            return Path.Combine(appPath, "ChartCS", _templateFile);
        }

        private string ProcessTemplate(string htmlContent)
        {
            var option = CreateChartOption();
            string optionJson = JsonConvert.SerializeObject(option, Formatting.Indented);
            
            string processedHtml = Regex.Replace(htmlContent, 
                @"option\s*=\s*\{[\s\S]*?\};", 
                $"option = {optionJson};", 
                RegexOptions.Multiline);
            
            return processedHtml;
        }

        private async void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                await UpdateChart();
            }
        }

        private object CreateChartOption()
        {
            return new
            {
                title = new { text = _chartTitle, left = "center" },
                tooltip = new
                {
                    trigger = "axis",
                    axisPointer = new { type = "shadow" }
                },
                legend = new
                {
                    data = _seriesData.ConvertAll(s => s.Name)
                },
                toolbox = new
                {
                    show = true,
                    orient = "vertical",
                    left = "right",
                    top = "center",
                    feature = new
                    {
                        mark = new { show = true },
                        dataView = new { show = true, readOnly = false },
                        magicType = new { show = true, type = new[] { "line", "bar", "stack" } },
                        restore = new { show = true },
                        saveAsImage = new { show = true }
                    }
                },
                xAxis = new[]
                {
                    new
                    {
                        type = "category",
                        axisTick = new { show = false },
                        data = _categories
                    }
                },
                yAxis = new[]
                {
                    new { type = "value" }
                },
                series = _seriesData.ConvertAll(s => new
                {
                    name = s.Name,
                    type = "bar",
                    data = s.Data,
                    itemStyle = new { color = s.Color },
                    label = new
                    {
                        show = true,
                        position = "top",
                        formatter = "{c}"
                    },
                    emphasis = new { focus = "series" }
                })
            };
        }

        private async System.Threading.Tasks.Task UpdateChart()
        {
            if (webView?.CoreWebView2 == null) return;

            var option = CreateChartOption();
            string optionJson = JsonConvert.SerializeObject(option, Formatting.None);
            
            string script = $@"
                if (typeof myChart !== 'undefined' && myChart) {{
                    myChart.setOption({optionJson}, true);
                }}
            ";

            try
            {
                await webView.CoreWebView2.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating chart: {ex.Message}", "Chart Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void SetData(List<string> categories, List<SeriesData> seriesData)
        {
            _categories = categories ?? new List<string>();
            _seriesData = seriesData ?? new List<SeriesData>();
            
            if (webView != null && webView.CoreWebView2 != null)
                _ = UpdateChart();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(_tempHtmlFile) && File.Exists(_tempHtmlFile))
                {
                    try
                    {
                        File.Delete(_tempHtmlFile);
                    }
                    catch { }
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode)
            {
                LoadChart();
            }
        }
    }

    [Serializable]
    public class SeriesData
    {
        public string Name { get; set; }
        public List<int> Data { get; set; }
        public string Color { get; set; }

        public SeriesData()
        {
            Data = new List<int>();
        }
    }
}