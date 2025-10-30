using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Reports
{
    public partial class ProductionReportForm : UIForm
    {
        private string _orderNo;

        public ProductionReportForm(string orderNo)
        {
            _orderNo = orderNo;
            InitializeComponent();
            InitializeDataGridView();
            LoadReportData();
        }

        private void InitializeDataGridView()
        {
            // Xóa các cột mặc định
            dgvProductionDateStats.Columns.Clear();

            // Thêm các cột
            dgvProductionDateStats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductionDate",
                HeaderText = "Ngày sản xuất",
                DataPropertyName = "ProductionDate",
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(52, 73, 94),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvProductionDateStats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CartonCount",
                HeaderText = "Số thùng",
                DataPropertyName = "CartonCount",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(155, 89, 182),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "N0"
                }
            });

            dgvProductionDateStats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BottleCount",
                HeaderText = "Số chai",
                DataPropertyName = "BottleCount",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(46, 204, 113),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "N0"
                }
            });

            dgvProductionDateStats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Percentage",
                HeaderText = "Tỷ lệ (%)",
                DataPropertyName = "Percentage",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(52, 152, 219),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "0.00"
                }
            });

            // Cấu hình header style
            dgvProductionDateStats.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // Auto resize columns
            dgvProductionDateStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadReportData()
        {
            try
            {
                // Lấy thông tin đơn hàng từ MES
                var orderResult = Globals.ProductionData.getfromMES.ProductionOrder_Detail(_orderNo);
                if (orderResult.issuccess && orderResult.data.Rows.Count > 0)
                {
                    var orderRow = orderResult.data.Rows[0];

                    lblOrderNo.Text = _orderNo;
                    lblProductName.Text = orderRow["productName"].ToString();
                    lblOrderQty.Text = orderRow["orderQty"].ToString();
                    lblCustomerOrderNo.Text = orderRow["customerOrderNo"].ToString();
                    //lblProductCode.Text = orderRow["orderNo"].ToString();
                   // lblGTIN.Text = orderRow["GTIN"].ToString();
                   // lblLotNumber.Text = orderRow["lotNumber"].ToString();
                    //lblShift.Text = orderRow["shift"].ToString();
                    // lblFactory.Text = orderRow["factory"].ToString();
                    //lblSite.Text = orderRow["site"].ToString();
                    //lblProductionLine.Text = orderRow["productionLine"].ToString();
                }

                // Lấy thống kê sản xuất
                LoadProductionStatistics();

                // Lấy thông tin thùng đang đóng
                LoadCurrentCartonInfo();

                // Lấy thông tin MES
                LoadMESInfo();

                // Lấy thông tin PO
                LoadPOInfo();

                // Lấy thống kê theo ProductionDate
                LoadProductionDateStatistics();

            }
            catch (Exception ex)
            {
                this.ShowErrorDialog($"Lỗi khi tải dữ liệu báo cáo: {ex.Message}");
            }
        }

        private void LoadProductionStatistics()
        {
            // Tổng số sản phẩm đã chạy
            var totalResult = Globals.ProductionData.getDataPO.Get_Record_Count(_orderNo);
            lblTotalRun.Text = totalResult.issuccess ? totalResult.count.ToString() : "0";

            // Số tốt
            var passResult = Globals.ProductionData.getDataPO.Get_Record_Count_By_Status(_orderNo, e_Production_Status.Pass);
            lblPassCount.Text = passResult.issuccess ? passResult.count.ToString() : "0";

            // Số đã phân làn (CameraSub)
            var cameraSubResult = Globals.ProductionData.getDataPO.Get_Record_Count_CameraSub_By_Status(_orderNo, e_Production_Status.Pass);
            lblCameraSubCount.Text = cameraSubResult.issuccess ? cameraSubResult.count.ToString() : "0";

            // Số thùng đã hoàn tất
            int passCount = passResult.issuccess ? passResult.count : 0;
            int completedCartons = passCount / AppConfigs.Current.cartonPack;
            lblCompletedCartons.Text = completedCartons.ToString();

            // Số gửi MES thành công
            var mesSuccessResult = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(_orderNo,
                e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Pending, "!=");
            lblMESSuccess.Text = mesSuccessResult.issuccess ? mesSuccessResult.count.ToString() : "0";
        }

        private void LoadCurrentCartonInfo()
        {
            try
            {
                // Lấy thông tin thùng hiện tại
                int currentCartonID = Globals.ProductionData.counter.cartonID;
                int currentPackingCount = Globals.ProductionData.counter.carton_Packing_Count;
                int maxPack = AppConfigs.Current.cartonPack;

                lblCurrentCartonID.Text = currentCartonID.ToString();
                lblCurrentPackingCount.Text = $"{currentPackingCount}/{maxPack}";

                // Tính phần trăm đóng gói của thùng hiện tại
                float percentage = maxPack > 0 ? (float)currentPackingCount / maxPack * 100 : 0;
                lblPackingPercentage.Text = $"{percentage:F1}%";

                // Hiển thị trạng thái thùng
                if (currentPackingCount == maxPack)
                {
                    lblCartonStatus.Text = "Đã đầy - Sẵn sàng niêm phong";
                    lblCartonStatus.ForeColor = Color.Green;
                }
                else if (currentPackingCount > 0)
                {
                    lblCartonStatus.Text = "Đang đóng gói";
                    lblCartonStatus.ForeColor = Color.Orange;
                }
                else
                {
                    lblCartonStatus.Text = "Chưa bắt đầu";
                    lblCartonStatus.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                lblCurrentCartonID.Text = "Lỗi";
                lblCurrentPackingCount.Text = "Lỗi";
                lblCartonStatus.Text = $"Lỗi: {ex.Message}";
                lblCartonStatus.ForeColor = Color.Red;
            }
        }

        private void LoadMESInfo()
        {
            // Số đang chờ gửi MES
            var pendingResult = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(_orderNo,
                e_AWS_Send_Status.Pending, e_AWS_Recive_Status.Pending, "=", "AND Status != 0");
            lblMESPending.Text = pendingResult.issuccess ? pendingResult.count.ToString() : "0";

            // Số gửi thất bại
            var failedResult = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(_orderNo,
                e_AWS_Send_Status.Failed, e_AWS_Recive_Status.Pending, "=", "AND Status != 0");
            lblMESFailed.Text = failedResult.issuccess ? failedResult.count.ToString() : "0";

            // Số đang chờ trả về
            var waitingResult = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(_orderNo,
                e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "=");
            lblMESWaiting.Text = waitingResult.issuccess ? waitingResult.count.ToString() : "0";
        }

        private void LoadPOInfo()
        {
            try
            {
                // Lấy thông tin PO từ MES
                var orderResult = Globals.ProductionData.getfromMES.ProductionOrder_Detail(_orderNo);
                if (orderResult.issuccess && orderResult.data.Rows.Count > 0)
                {
                    var orderRow = orderResult.data.Rows[0];

                    // Hiển thị mã sản phẩm (ProductCode)
                    lblMESPending.Text = orderRow["productCode"] != DBNull.Value
                        ? orderRow["productCode"].ToString()
                        : "-";

                    // Hiển thị GTIN
                    lblMESFailed.Text = orderRow["GTIN"] != DBNull.Value
                        ? orderRow["GTIN"].ToString()
                        : "-";

                    // Hiển thị số lô (LotNumber)
                    lblMESWaiting.Text = orderRow["lotNumber"] != DBNull.Value
                        ? orderRow["lotNumber"].ToString()
                        : "-";

                    // Hiển thị ca sản xuất (Shift)
                    lblShift.Text = orderRow["shift"] != DBNull.Value
                        ? orderRow["shift"].ToString()
                        : "-";
                }
                else
                {
                    // Nếu không lấy được dữ liệu
                    lblMESPending.Text = "-";
                    lblMESFailed.Text = "-";
                    lblMESWaiting.Text = "-";
                    lblShift.Text = "-";
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                lblMESPending.Text = "Lỗi";
                lblMESFailed.Text = "Lỗi";
                lblMESWaiting.Text = "Lỗi";
                lblShift.Text = "Lỗi";
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load thông tin PO: {ex.Message}");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReportData();
            this.ShowSuccessTip("Đã cập nhật dữ liệu báo cáo!");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadProductionDateStatistics()
        {
            try
            {
                // Lấy tất cả records từ CameraSub
                var recordsResult = Globals.ProductionData.getDataPO.Get_Records_CameraSub(_orderNo);
                if (!recordsResult.issuccess || recordsResult.data == null)
                {
                    return;
                }

                // Tạo dictionary để nhóm theo ProductionDate
                var dateStats = new Dictionary<string, (int cartonCount, int bottleCount)>();

                foreach (DataRow row in recordsResult.data.Rows)
                {
                    // Chỉ tính các sản phẩm Pass
                    if (row["Status"].ToString() != e_Production_Status.Pass.ToString())
                        continue;

                    string productionDate = row["productionDate"].ToString();

                    // Chuyển đổi định dạng ngày nếu cần
                    if (DateTime.TryParse(productionDate, out DateTime date))
                    {
                        productionDate = date.ToString("yyyy-MM-dd");
                    }

                    if (!dateStats.ContainsKey(productionDate))
                    {
                        dateStats[productionDate] = (0, 0);
                    }

                    var current = dateStats[productionDate];

                    // Đếm chai (mỗi record là 1 chai)
                    int newBottleCount = current.bottleCount + 1;

                    // Tính số thùng (mỗi cartonPack chai = 1 thùng)
                    int newCartonCount = newBottleCount / AppConfigs.Current.cartonPack;

                    dateStats[productionDate] = (newCartonCount, newBottleCount);
                }

                // Hiển thị kết quả
                DisplayProductionDateStats(dateStats);
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không hiển thị dialog để không ảnh hưởng đến việc load dữ liệu khác
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load thống kê theo ngày sản xuất: {ex.Message}");
            }
        }

        private void DisplayProductionDateStats(Dictionary<string, (int cartonCount, int bottleCount)> dateStats)
        {
            // Sắp xếp theo ngày (giảm dần - ngày gần nhất ở trên)
            var sortedStats = dateStats.OrderByDescending(x => x.Key).ToList();

            // Tính tổng
            int totalBottles = sortedStats.Sum(x => x.Value.bottleCount);

            // Tạo DataTable để bind vào DataGridView
            var dataTable = new DataTable();
            dataTable.Columns.Add("ProductionDate", typeof(string));
            dataTable.Columns.Add("CartonCount", typeof(int));
            dataTable.Columns.Add("BottleCount", typeof(int));
            dataTable.Columns.Add("Percentage", typeof(double));

            foreach (var stat in sortedStats)
            {
                double percentage = totalBottles > 0 ? (double)stat.Value.bottleCount / totalBottles * 100 : 0;
                dataTable.Rows.Add(
                    stat.Key,
                    stat.Value.cartonCount,
                    stat.Value.bottleCount,
                    percentage
                );
            }

            // Thêm dòng tổng cộng
            if (sortedStats.Count > 0)
            {
                int totalCartons = sortedStats.Sum(x => x.Value.cartonCount);
                dataTable.Rows.Add(
                    "═══ TỔNG CỘNG ═══",
                    totalCartons,
                    totalBottles,
                    100.0
                );
            }

            // Bind vào DataGridView
            this.InvokeIfRequired(() =>
            {
                dgvProductionDateStats.DataSource = dataTable;

                // Làm nổi bật dòng tổng cộng
                if (dgvProductionDateStats.Rows.Count > 0)
                {
                    var lastRow = dgvProductionDateStats.Rows[dgvProductionDateStats.Rows.Count - 1];
                    lastRow.DefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                    lastRow.DefaultCellStyle.ForeColor = Color.White;
                    lastRow.DefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                }
            });
        }
    }
}