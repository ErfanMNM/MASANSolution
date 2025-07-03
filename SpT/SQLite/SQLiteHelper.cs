using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpT.SQLite
{
    public class SQLiteToCSVExporter
    {
        public static void ExportToCSV(string dbFilePath, string sqlQuery, string outputCsvPath)
        {
            if (!File.Exists(dbFilePath))
            {
                Console.WriteLine("❌ File DB không tồn tại: " + dbFilePath);
                return;
            }

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sqlQuery, conn))
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        using (StreamWriter sw = new StreamWriter(outputCsvPath, false, Encoding.UTF8))
                        {
                            // Ghi header
                            string[] columnNames = new string[dt.Columns.Count];
                            for (int i = 0; i < dt.Columns.Count; i++)
                                columnNames[i] = dt.Columns[i].ColumnName;
                            sw.WriteLine(string.Join(",", columnNames));

                            // Ghi data
                            foreach (DataRow row in dt.Rows)
                            {
                                string[] fields = new string[dt.Columns.Count];
                                for (int i = 0; i < dt.Columns.Count; i++)
                                {
                                    var value = row[i]?.ToString() ?? "";
                                    if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                                    {
                                        value = "\"" + value.Replace("\"", "\"\"") + "\"";
                                    }
                                    fields[i] = value;
                                }
                                sw.WriteLine(string.Join(",", fields));
                            }
                        }

                        Console.WriteLine($"✅ Xuất CSV thành công: {outputCsvPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi: " + ex.Message);
            }
        }
    }

}
