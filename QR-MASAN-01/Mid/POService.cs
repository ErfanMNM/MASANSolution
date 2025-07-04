using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QR_MASAN_01
{
    public class POService
    {
        private string _connectionString;
        private string _codesPath;

        public POService(string dbPath = @"C:\Users\THUC\source\repos\ErfanMNM\MASANSolution\Server_Service\po.db", string CodesPath = @"C:\Users\THUC\source\repos\ErfanMNM\MASANSolution\Server_Service\codes")
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
            _codesPath = CodesPath;
        }

        public void LoadOrderNoToComboBox(UIComboBox comboBox)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                string query = "SELECT DISTINCT orderNo FROM POInfo ORDER BY orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();

                adapter.Fill(table);
                // Thêm một dòng rỗng vào đầu danh sách
                DataRow emptyRow = table.NewRow();
                emptyRow["orderNo"] = "Select Order No"; // Hoặc để trống
                table.Rows.InsertAt(emptyRow, 0);
                // Thiết lập DataSource cho ComboBox
                comboBox.DataSource = table;
                comboBox.DisplayMember = "orderNo";
                comboBox.ValueMember = "orderNo";
            }
        }

        //lấy danh sách PO trả về dataTable
        public DataTable GetPOList()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                string query = "SELECT * FROM po_records ORDER BY orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public DataTable GetPOByOrderNo(string orderNo)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                string query = "SELECT * FROM POInfo WHERE orderNo = @orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);

                adapter.SelectCommand.Parameters.AddWithValue("@orderNo", orderNo);

                var table = new DataTable();
                adapter.Fill(table);

                //thêm cột số mã CZ

                table.Columns.Add("UniqueCodeCount", typeof(int));
                //lấy số mã CZ trong thư mục _codesPath/<orderNo>.db
                int uniqueCodeCount = GetUniqueCodeCount(orderNo);
                //cập nhật số mã CZ vào cột UniqueCodeCount
                foreach (DataRow row in table.Rows)
                {
                    row["UniqueCodeCount"] = uniqueCodeCount;
                }

                return table;
            }
        }

        //lấy số count mã CZ nằm trong thư mục _codesPath/<orderNo>.db SELECT COUNT(*) FROM `UniqueCodes`;
        public int GetUniqueCodeCount(string orderNo)
        {
            try
            {
                string czpath = _codesPath + "/" + orderNo + ".db";
                using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM UniqueCodes";
                    var command = new SQLiteCommand(query, conn);
                    command.Parameters.AddWithValue("@orderNo", orderNo);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }
            catch 
            {
                return 0;
            }

        }

    }

}
