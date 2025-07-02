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

        public POService(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
        }

        public void LoadOrderNoToComboBox(UIComboBox comboBox)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                string query = "SELECT DISTINCT orderNo FROM POInfo ORDER BY orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();

                adapter.Fill(table);

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
                string query = "SELECT * FROM po_records WHERE orderNo = @orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);

                adapter.SelectCommand.Parameters.AddWithValue("@orderNo", orderNo);

                var table = new DataTable();
                adapter.Fill(table);

                return table;
            }
        }

    }

}
