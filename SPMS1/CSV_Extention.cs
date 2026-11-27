using System;
using System.Data;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Linq;

namespace SPMS1
{
    public partial class CSV_Extention : Component
    {
        private BackgroundWorker backgroundWorker;

        // Sự kiện khi hoàn thành xuất CSV
        public event EventHandler<string> Completed;

        // Sự kiện khi có lỗi trong quá trình xuất
        public event EventHandler<Exception> ErrorOccurred;

        public CSV_Extention()
        {
            InitializeComponent();
        }

        public CSV_Extention(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private static string EscapeCsvField(string value)
        {
            value = value ?? string.Empty;
            return value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0
                ? value : "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        // Phương thức để xuất DataTable thành file CSV
        public void ExportDataTableToCsv(DataTable dataTable, string DirectoryPath, string FileName)
        {
            try
            {
                // Kiểm tra xem thư mục có tồn tại không
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                // Tạo đường dẫn đầy đủ cho file CSV
                string filePath = Path.Combine(DirectoryPath, FileName);

                StringBuilder csvContent = new StringBuilder();

                csvContent.AppendLine(string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(c => EscapeCsvField(c.ColumnName))));
                foreach (DataRow row in dataTable.Rows)
                    csvContent.AppendLine(string.Join(",", row.ItemArray.Select(value => EscapeCsvField(Convert.ToString(value)))));

                // Lưu nội dung CSV vào file
                File.WriteAllText(filePath, csvContent.ToString());

                // Thông báo thành công
                Completed?.Invoke(this, filePath); // Gửi đường dẫn file xuất ra sự kiện Completed
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, gửi sự kiện lỗi
                ErrorOccurred?.Invoke(this, ex);
            }
        } 
    }
}
