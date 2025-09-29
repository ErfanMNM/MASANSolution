using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using Sunny.UI;
using System;
using System.Data;
using System.Drawing;
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
            LoadReportData();
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReportData();
            this.ShowSuccessTip("Đã cập nhật dữ liệu báo cáo!");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}