using System;
using System.Data;
using System.IO;
using System.Text;
using System.ComponentModel;

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

                // Thêm tiêu đề cột vào CSV
                foreach (DataColumn column in dataTable.Columns)
                {
                    csvContent.Append(column.ColumnName + ",");
                }
                csvContent.Remove(csvContent.Length - 1, 1); // Xóa dấu phẩy cuối cùng
                csvContent.AppendLine();

                // Thêm dữ liệu từ DataTable vào CSV
                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        csvContent.Append(row[column].ToString().Replace(",", ";") + ","); // Thay dấu phẩy bằng chấm phẩy
                    }
                    csvContent.Remove(csvContent.Length - 1, 1); // Xóa dấu phẩy cuối dòng
                    csvContent.AppendLine();
                }

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
