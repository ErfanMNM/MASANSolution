using MASAN_SERIALIZATION.Helpers;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Database
{
    public partial class PCodeSearch : UIPage
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private List<DatabaseHelper.UniqueCode> _results = new List<DatabaseHelper.UniqueCode>();

        private int _pageSize = 50;
        private int _pageIndex = 0; // zero-based

        public PCodeSearch()
        {
            InitializeComponent();

            InitGrid();
            InitWorker();

            this.Text = "Tìm mã sản phẩm (SQLite)";
            this.Symbol = 61442; // search icon

            ipPageSize.SelectedIndex = 1; // default 50
            pagination.ActivePage = 1;
        }

        private void InitWorker()
        {
            _worker.DoWork += (s, e) =>
            {
                var code = e.Argument?.ToString()?.Trim();
                if (string.IsNullOrEmpty(code))
                {
                    e.Result = new List<DatabaseHelper.UniqueCode>();
                    return;
                }

                // Exact search first
                var list = _db.SearchCodeInAllPODatabases(code);
                if (list.Count == 0)
                {
                    // fallback to LIKE
                    list = _db.SearchCodeWithWildcard(code);
                }
                e.Result = list;
            };
            _worker.RunWorkerCompleted += (s, e) =>
            {
                btnSearch.Enabled = true;
                txtSearch.Enabled = true;

                if (e.Error != null)
                {
                    this.ShowErrorTip($"Lỗi tìm kiếm: {e.Error.Message}");
                    return;
                }

                _results = (e.Result as List<DatabaseHelper.UniqueCode>) ?? new List<DatabaseHelper.UniqueCode>();
                pagination.TotalCount = _results.Count;
                ApplyPage();
                lblStatus.Text = _results.Count == 0 ? "Không tìm thấy dữ liệu." : $"Tìm thấy: {_results.Count:n0} dòng";
            };
        }

        private void InitGrid()
        {
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DatabaseHelper.UniqueCode.Code),
                HeaderText = "Mã sản phẩm",
                Width = 220,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Consolas", 10, FontStyle.Bold) }
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DatabaseHelper.UniqueCode.OrderNo),
                HeaderText = "OrderNo",
                Width = 100
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DatabaseHelper.UniqueCode.CartonCode),
                HeaderText = "Mã thùng",
                Width = 140
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DatabaseHelper.UniqueCode.Status),
                HeaderText = "Trạng thái",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DatabaseHelper.UniqueCode.ActivateDate),
                HeaderText = "Ngày kích hoạt",
                Width = 140
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DatabaseHelper.UniqueCode.ProductionDate),
                HeaderText = "Ngày SX",
                Width = 120
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(DatabaseHelper.UniqueCode.ActivateUser),
                HeaderText = "Người kích hoạt",
                Width = 140
            });
        }

        private void ApplyPage()
        {
            // Clamp page size
            if (_pageSize <= 0) _pageSize = 50;
            if (_pageIndex < 0) _pageIndex = 0;

            var page = _results
                .Skip(_pageIndex * _pageSize)
                .Take(_pageSize)
                .ToList();

            grid.DataSource = page;
            pagination.PageSize = _pageSize;
            pagination.ActivePage = _pageIndex + 1;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            StartSearch();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartSearch();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void StartSearch()
        {
            var code = txtSearch.Text?.Trim();
            if (string.IsNullOrEmpty(code))
            {
                this.ShowWarningTip("Vui lòng nhập mã cần tìm");
                return;
            }

            btnSearch.Enabled = false;
            txtSearch.Enabled = false;
            lblStatus.Text = "Đang tìm kiếm...";
            _pageIndex = 0;
            pagination.ActivePage = 1;
            _results.Clear();
            grid.DataSource = null;

            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync(code);
            }
        }

        private void pagination_PageChanged(object sender, object pagingSource, int pageIndex, int count)
        {
            // Sunny.UI provides 1-based index in event
            _pageIndex = pageIndex;
            ApplyPage();
        }

        private void ipPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(ipPageSize.Text, out var sz))
            {
                _pageSize = sz;
                _pageIndex = 0;
                ApplyPage();
            }
        }
    }
}

