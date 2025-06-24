using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpT
{
    public static class FileHelper
    {
        // Ví dụ trong 1 hàm bất kỳ (WinForms)
        public static string Get_Save_File_Path()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Tên mặc định: RP-ddMM
                string defaultFileName = $"RP-{DateTime.Now:ddMM}";

                saveFileDialog.FileName = defaultFileName; // tên file mặc định
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                saveFileDialog.Title = "Chọn nơi lưu báo cáo";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveFileDialog.FileName; // đường dẫn user chọn
                }
                else
                {
                    return null; // user bấm Cancel
                }
            }
        }

        public static string Get_Save_File_Path_CSV()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Tên mặc định: RP-ddMM
                string defaultFileName = $"RP-CSV-{DateTime.Now:ddMM}";

                saveFileDialog.FileName = defaultFileName; // tên file mặc định
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                saveFileDialog.Title = "Chọn nơi lưu báo cáo";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveFileDialog.FileName; // đường dẫn user chọn
                }
                else
                {
                    return null; // user bấm Cancel
                }
            }
        }

        //chuyển data table sang csv    

    }

    public static class CsvHelper
    {
        //nhập vào datatable, đường dẫn => xuất csv sau đó trả về IsSucces và Message
        //viết chú giải chi tiết về hàm này
        /// <summary>
        /// Hàm xuất DataTable sang định dạng CSV.
        /// dataTable: DataTable cần xuất.
        /// filePath: Đường dẫn file CSV sẽ được lưu.
        /// <summary>

        public static (bool IsSucces, string Message) ExportDataTableToCsv(DataTable dataTable, string filePath)
        {
            try
            {
                StringBuilder csvContent = new StringBuilder();
                // Thêm tiêu đề cột
                var columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                csvContent.AppendLine(string.Join(",", columnNames));
                // Thêm dữ liệu từng hàng
                foreach (var row in dataTable.AsEnumerable())
                {
                    var fields = row.ItemArray.Select(field => field.ToString().Replace(",", " ")); // Thay thế dấu phẩy để tránh lỗi CSV
                    csvContent.AppendLine(string.Join(",", fields));
                }
                File.WriteAllText(filePath, csvContent.ToString());
                return (true, "Xuất CSV thành công");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi xuất CSV: {ex.Message}");
            }
        }
    }
}
