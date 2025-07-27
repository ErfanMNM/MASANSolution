using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace SpT.DataSCADA
{
    public class ChartHelper
    {
        private readonly string _dbFolder = @"C:\MasanSerialization\PODatabases";

        public string GenerateHtmlFromSQLite(string fileName)
        {
            string dbPath = Path.Combine(_dbFolder, fileName + ".db");
            if (!File.Exists(dbPath))
                throw new FileNotFoundException("File not found", dbPath);

            var labels = new List<string>();
            var counts = new List<int>();

            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT ActivateDate FROM Records WHERE ActivateDate IS NOT NULL";

                var dict = new Dictionary<string, int>();
                using (var da = new SQLiteDataAdapter(cmd))
                {
                    var table = new DataTable();
                    da.Fill(table);

                    foreach (DataRow row in table.Rows)
                    {
                        var rawValue = row["ActivateDate"]?.ToString();
                        if (DateTime.TryParse(rawValue, out DateTime dt))
                        {
                            string hourKey = dt.ToString("yyyy-MM-dd HH:mm:ss");
                            if (!dict.ContainsKey(hourKey)) dict[hourKey] = 0;
                            dict[hourKey]++;
                        }
                    }
                }


                foreach (var kv in dict)
                {
                    labels.Add(kv.Key);
                    counts.Add(kv.Value);
                }
            }

            return GenerateEChartHtml(labels, counts);
        }

        private string GenerateEChartHtml(List<string> labels, List<int> counts)
        {
            var labelStr = string.Join(",", labels.ConvertAll(l => $"'{l}'"));
            var countStr = string.Join(",", counts);

            var html = new StringBuilder();
            html.AppendLine("<html><head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<script src='https://cdn.jsdelivr.net/npm/echarts@5/dist/echarts.min.js'></script>");
            html.AppendLine("</head><body>");
            html.AppendLine("<div id='main' style='width:100%;height:100vh;'></div>");
            html.AppendLine("<script>");
            html.AppendLine("var chart = echarts.init(document.getElementById('main'));");
            html.AppendLine("chart.setOption({");
            html.AppendLine("title: { text: 'Sản lượng theo giờ' },");
            html.AppendLine("tooltip: {},");
            html.AppendLine($"xAxis: {{ type: 'category', data: [{labelStr}] }},");
            html.AppendLine("yAxis: { type: 'value' },");
            html.AppendLine($"series: [{{ name: 'Sản lượng', type: 'line', data: [{countStr}] }}]");
            html.AppendLine("});");
            html.AppendLine("</script></body></html>");
            return html.ToString();
        }
    }

}
