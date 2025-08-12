using DATALOGIC_SCAN;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Helpers;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace MASAN_SERIALIZATION.Views.Database
{
    public partial class PScaner : UIPage
    {
        Connection _ScanConection01 = new Connection();
        DatabaseHelper dbHelper = new DatabaseHelper();
        private List<DatabaseHelper.UniqueCode> currentSearchResults = new List<DatabaseHelper.UniqueCode>();
        private BackgroundWorker searchWorker = new BackgroundWorker();

        public PScaner()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeBackgroundWorker();
            this.Symbol = 61442; // Search icon
        }

        private void InitializeBackgroundWorker()
        {
            searchWorker.DoWork += SearchWorker_DoWork;
            searchWorker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;
            searchWorker.WorkerSupportsCancellation = true;
        }

        private void InitializeDataGridView()
        {
            dgvSearchResult.AutoGenerateColumns = false;
            dgvSearchResult.Columns.Clear();
            dgvSearchResult.AllowUserToAddRows = false;
            dgvSearchResult.AllowUserToDeleteRows = false;
            dgvSearchResult.AllowUserToResizeColumns = true;
            dgvSearchResult.MultiSelect = false;

            dgvSearchResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Code",
                HeaderText = "MÃ SẢN PHẨM",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Consolas", 10, FontStyle.Bold) }
            });

            dgvSearchResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OrderNo",
                HeaderText = "SỐ PO",
                Width = 100
            });

            dgvSearchResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CartonCode",
                HeaderText = "MÃ THÙNG",
                Width = 120
            });

            dgvSearchResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "TRẠNG THÁI",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvSearchResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ActivateDate",
                HeaderText = "NGÀY KÍCH HOẠT",
                Width = 130
            });

            dgvSearchResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductionDate",
                HeaderText = "NGÀY SẢN XUẤT",
                Width = 130
            });

            dgvSearchResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ActivateUser",
                HeaderText = "NGƯỜI KÍCH HOẠT",
                Width = 130
            });
        }
        //Ghi chú hướng dẫn
        //Người dùng có thể vừa quét mã vừa nhập dữ liệu vào các trường khác mã quét sẽ là Recive string s trong _ScanConection01_EVENT.
        //Thực hiện kiểm tra xem mã có tồn tại hay không bằng cách lấy danh sách PO ở bảng POInfo nằm tại C:\MasanSerialization\Server_Service\po.db
        //Thông tin bảng như sau
        //CREATE TABLE "POInfo" (
        //	"id"	INTEGER,
        //	"orderNo"	TEXT UNIQUE,
        //	"site"	TEXT,
        //	"factory"	TEXT,
        //	"productionLine"	TEXT,
        //	"productionDate"	TEXT,
        //	"shift"	TEXT,
        //	"orderQty"	INTEGER,
        //	"lotNumber"	TEXT,
        //	"productCode"	TEXT,
        //	"productName"	TEXT,
        //	"GTIN"	TEXT,
        //	"customerOrderNo"	TEXT,
        //	"uom"	TEXT,
        //	"lastUpdated"	TEXT,
        //	PRIMARY KEY("id" AUTOINCREMENT)
        //);

        //Mã sẽ nằm ở các file trong thư mục C:\MasanSerialization\PODatabases với orderNo.db cấu trúc bảng như sau:
        //        CREATE TABLE "UniqueCodes" (
        //	"ID"	INTEGER NOT NULL UNIQUE,
        //	"Code"	TEXT NOT NULL UNIQUE,
        //	"cartonCode"	TEXT NOT NULL DEFAULT 0,
        //	"Status"	INTEGER NOT NULL DEFAULT 0,
        //	"ActivateDate"	TEXT NOT NULL DEFAULT 0,
        //	"ProductionDate"	TEXT NOT NULL DEFAULT 0,
        //	"ActivateUser"	TEXT NOT NULL DEFAULT 0,
        //	"SubCamera_ActivateDate"	TEXT NOT NULL DEFAULT 0,
        //	"Send_Status"	TEXT NOT NULL DEFAULT 0,
        //	"Recive_Status"	TEXT NOT NULL DEFAULT 0,
        //	"Send_Recive_Logs"	JSON,
        //	"Duplicate"	JSON,
        //	PRIMARY KEY("ID" AUTOINCREMENT)
        //);

        //Tìm kiếm mã sản phẩm trong bảng UniqueCodes của file {orderNo}.db cho tất cả PO sau đó hiện lên màn hình

        public void INIT()
        {
            // Khởi tạo kết nối với thiết bị quét mã vạch
            _ScanConection01.SERIALPORT = serialPort1;
            _ScanConection01.EVENT += _ScanConection01_EVENT;
            _ScanConection01.LOAD();
            _ScanConection01.CONNECT(AppConfigs.Current.HandScanCOMMain);
        }

        private void _ScanConection01_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    UpdateStatus("Đã kết nối scanner");
                    break;
                case e_Serial.Disconnected:
                    UpdateStatus("Mất kết nối scanner");
                    break;
                case e_Serial.Recive:
                    if (!string.IsNullOrEmpty(s))
                    {
                        string scannedCode = s.Trim();
                        ProcessScannedCode(scannedCode);
                    }
                    break;
            }
        }

        private void ProcessScannedCode(string code)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ProcessScannedCode), code);
                return;
            }

            txtScannedCode.Text = code;
            txtScannedCode.FillColor = Color.FromArgb(144, 238, 144); // Light green
            
            SearchCode(code);

            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => txtScannedCode.FillColor = Color.FromArgb(255, 255, 192)));
                }
                else
                {
                    txtScannedCode.FillColor = Color.FromArgb(255, 255, 192); // Light yellow
                }
                timer?.Dispose();
            }, null, 2000, System.Threading.Timeout.Infinite);
        }

        private void SearchCode(string searchCode)
        {
            if (string.IsNullOrWhiteSpace(searchCode))
            {
                UpdateStatus("Vui lòng nhập mã cần tìm");
                return;
            }

            // Hủy tìm kiếm trước đó nếu đang chạy
            if (searchWorker.IsBusy)
            {
                searchWorker.CancelAsync();
                return;
            }

            UpdateStatus("🔍 Đang tìm kiếm...");
            dgvSearchResult.DataSource = null;
            
            // Disable các control trong khi tìm kiếm
            btnSearch.Enabled = false;
            txtSearchManual.Enabled = false;
            
            // Bắt đầu tìm kiếm trong background
            searchWorker.RunWorkerAsync(searchCode);
        }

        private void SearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string searchCode = e.Argument.ToString();
            var worker = sender as BackgroundWorker;
            
            try
            {
                // Kiểm tra hủy
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                // Tìm kiếm chính xác trước
                var results = dbHelper.SearchCodeInAllPODatabases(searchCode);
                
                // Kiểm tra hủy
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                
                // Nếu không tìm thấy, tìm kiếm wildcard
                if (results.Count == 0)
                {
                    results = dbHelper.SearchCodeWithWildcard(searchCode);
                }
                
                e.Result = new { SearchCode = searchCode, Results = results };
            }
            catch (Exception ex)
            {
                e.Result = new { SearchCode = searchCode, Error = ex.Message };
            }
        }

        private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Enable lại các control
            btnSearch.Enabled = true;
            txtSearchManual.Enabled = true;
            
            if (e.Cancelled)
            {
                UpdateStatus("⚠️ Tìm kiếm đã bị hủy");
                return;
            }
            
            if (e.Error != null)
            {
                UpdateStatus($"❌ Lỗi tìm kiếm: {e.Error.Message}");
                return;
            }
            
            var result = (dynamic)e.Result;
            
            //if (result.Error != null)
            //{
            //    UpdateStatus($"❌ Lỗi tìm kiếm: {result.Error}");
            //    return;
            //}
            
            currentSearchResults = result.Results;
            string searchCode = result.SearchCode;
            
            if (currentSearchResults.Count > 0)
            {
                dgvSearchResult.DataSource = currentSearchResults;
                UpdateStatus($"✅ Tìm thấy {currentSearchResults.Count} kết quả cho '{searchCode}'");
                
                // Hiển thị thông tin chi tiết nếu chỉ có 1 kết quả
                if (currentSearchResults.Count == 1)
                {
                    ShowDetailInfo(currentSearchResults[0]);
                }
            }
            else
            {
                UpdateStatus($"❌ Không tìm thấy mã: '{searchCode}'");
            }
        }
        
        private void ShowDetailInfo(DatabaseHelper.UniqueCode uniqueCode)
        {
            try
            {
                var poInfo = dbHelper.GetPOInfoByOrderNo(uniqueCode.OrderNo);
                if (poInfo != null)
                {
                    string detailInfo = $"📦 {poInfo.ProductName} | 🏷️ {poInfo.ProductCode} | 📋 {poInfo.OrderNo} | 🔢 {poInfo.GTIN}";
                    UpdateStatus(detailInfo);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"⚠️ Không thể lấy thông tin chi tiết: {ex.Message}");
            }
        }

        private void UpdateStatus(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateStatus), message);
                return;
            }
            
            lblStatus.Text = message;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearchManual.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                txtScannedCode.Text = searchText;
                SearchCode(searchText);
            }
            else
            {
                UpdateStatus("⚠️ Vui lòng nhập mã cần tìm kiếm");
            }
        }

        private void txtSearchManual_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}