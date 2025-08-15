using MASAN_SERIALIZATION.Helpers;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Database
{
    public partial class POrderNoViewer : UIPage
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        private List<DatabaseHelper.POInfo> _poList = new List<DatabaseHelper.POInfo>();
        private List<Row> _rows = new List<Row>();

        public class Row
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string CartonCode { get; set; }
            public int Status { get; set; }
            public string ActivateDate { get; set; }
            public string ProductionDate { get; set; }
            public string ActivateUser { get; set; }
        }

        public POrderNoViewer()
        {
            InitializeComponent();
            this.Text = "Xem dữ liệu orderNo.db";
            this.Symbol = 61639; // database icon
            InitGrid();
            LoadOrderNos();
        }

        private void InitGrid()
        {
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Row.Code),
                HeaderText = "Mã sản phẩm",
                Width = 220
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Row.CartonCode),
                HeaderText = "Mã thùng",
                Width = 160
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Row.Status),
                HeaderText = "Trạng thái",
                Width = 100
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Row.ActivateDate),
                HeaderText = "Ngày kích hoạt",
                Width = 160
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Row.ProductionDate),
                HeaderText = "Ngày SX",
                Width = 140
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Row.ActivateUser),
                HeaderText = "Người kích hoạt",
                Width = 160
            });
        }

        private void LoadOrderNos()
        {
            try
            {
                _poList = _db.GetAllPOInfo();
                var orders = _poList.Select(p => p.OrderNo).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
                cbOrderNo.DataSource = orders;
            }
            catch (Exception ex)
            {
                this.ShowErrorTip($"Không tải được danh sách OrderNo: {ex.Message}");
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var selected = cbOrderNo.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selected))
            {
                this.ShowWarningTip("Chọn OrderNo trước đã");
                return;
            }
            LoadDbByOrderNo(selected);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "SQLite DB (*.db)|*.db|All files (*.*)|*.*";
                ofd.InitialDirectory = @"C:\\MasanSerialization\\PODatabases";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtDbPath.Text = ofd.FileName;
                    LoadDbPath(ofd.FileName);
                }
            }
        }

        private void LoadDbByOrderNo(string orderNo)
        {
            var dbPath = Path.Combine(@"C:\\MasanSerialization\\PODatabases", $"{orderNo}.db");
            txtDbPath.Text = dbPath;
            LoadDbPath(dbPath);
        }

        private void LoadDbPath(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                this.ShowErrorTip($"Không tìm thấy file: {dbPath}");
                return;
            }

            try
            {
                _rows.Clear();
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT ID, Code, cartonCode, Status, ActivateDate, ProductionDate, ActivateUser FROM UniqueCodes LIMIT 1000", conn))
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            _rows.Add(new Row
                            {
                                ID = Convert.ToInt32(rd["ID"]),
                                Code = rd["Code"]?.ToString(),
                                CartonCode = rd["cartonCode"]?.ToString(),
                                Status = rd["Status"] != DBNull.Value ? Convert.ToInt32(rd["Status"]) : 0,
                                ActivateDate = rd["ActivateDate"]?.ToString(),
                                ProductionDate = rd["ProductionDate"]?.ToString(),
                                ActivateUser = rd["ActivateUser"]?.ToString()
                            });
                        }
                    }
                }

                this.ApplyFilter();
                lblStatus.Text = $"Đã tải: {_rows.Count:n0} dòng";
            }
            catch (Exception ex)
            {
                this.ShowErrorTip($"Lỗi đọc DB: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            var key = txtFilter.Text?.Trim();
            if (string.IsNullOrEmpty(key))
            {
                grid.DataSource = _rows.ToList();
            }
            else
            {
                grid.DataSource = _rows.Where(r =>
                    (r.Code ?? string.Empty).IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (r.CartonCode ?? string.Empty).IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            this.ApplyFilter();
        }
    }
}

