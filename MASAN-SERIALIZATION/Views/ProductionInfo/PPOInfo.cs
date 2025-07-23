using Google.Apis.Util;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.ProductionInfo
{
    public partial class PPOInfo : UIPage
    {
        //khởi tạo biến toàn cục để ghi nhật ký cho giao diện này
        LogHelper<e_LogType> POPageLog;

        //biến toàn cục để lưu trữ trạng thái ứng dụng
        public CancellationTokenSource poPage_Main_Process = new CancellationTokenSource(); //token cho task chính
        public PPOInfo()
        {
            InitializeComponent();
        }

        #region Sự kiện khởi động
        public void START()
        {
            //khởi tạo biến toàn cục để ghi nhật ký cho giao diện này
            POPageLog = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs","Pages", "PPOlog.ptl"));
            //render iporderNo
            iporderNo_Render();

            //khởi động nhiệm vụ chính
            Start_Process_Task();
        }
        private void PPOInfo_Initialize(object sender, EventArgs e)
        {
            //ghi log khởi tạo giao diện
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username,e_LogType.UserAction,"Người dùng mở giao diện chỉnh thông tin sản xuất");
        }

        private void PPOInfo_Finalize(object sender, EventArgs e)
        {

        }

        #endregion

        #region Các sự kiện render giao diện
        private void iporderNo_Render()
        {
            //load cbb
            var loadorderNO = Globals.ProductionData.getfromMES.MES_Load_OrderNo_ToComboBox(ipOrderNO);
            if (loadorderNO.issucess)
            {
                //ghi log tạo thành công danh sách đơn hàng
                POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Tạo thành công danh sách đơn hàng từ MES");
            }
            else
            {
                //ghi log tạo thất bại danh sách đơn hàng
                //tạo 1 string json phản hồi lỗi
                string errorResponse = $"{{\"error\":\"{loadorderNO.message}\"}}";
                POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Tạo thất bại danh sách đơn hàng từ MES", errorResponse);
            }
        }

        #endregion

        #region Luồng sự kiện sử lý trạng thái Production
        //lưu trữ nhiệm vụ chính của task
        public void TaskBody()
        {
            switch (Globals.Production_State)
            {
                case e_Production_State.NoSelectedPO:
                    //bật các phím chức năng
                    if(!btnPO.Enabled)
                    {
                        btnPO.Enabled = true;
                        btnTestMode.Enabled = true;
                    }

                    break;
                case e_Production_State.Start:
                    //lấy thông tin po dùng lần cuối
                    var getLastPO = Globals.ProductionData.getDataPO.GetLastPO();
                    if (getLastPO.issucess)
                    {
                        //cập nhật trạng thái sản xuất
                        Globals.Production_State = e_Production_State.Checking_PO_Info;
                        //gán vào selected po
                        Globals.ProductionData.orderNo = getLastPO.lastPO["orderNo"].ToString();
                    }
                    else
                    {
                        this.InvokeIfRequired(() =>
                        {
                            opTer.Text = "Không có đơn hàng nào được chọn. Vui lòng chọn đơn hàng để tiếp tục.";
                            opTer.ForeColor = Color.Teal;
                            opTer.Font = new Font("Tahoma", 14, FontStyle.Bold);
                        });
                        //quăng về trạng thái NoSelectedPO
                        Globals.Production_State = e_Production_State.NoSelectedPO;
                    }

                    break;
                case e_Production_State.Checking_PO_Info:
                    break;
                case e_Production_State.Loading:
                    break;
                case e_Production_State.Camera_Processing:
                    break;
                case e_Production_State.Pushing_new_PO_to_PLC:
                    break;
                case e_Production_State.Pushing_continue_PO_to_PLC:
                    break;
                case e_Production_State.Ready:
                    break;
                case e_Production_State.Running:
                    break;
                case e_Production_State.Completed:
                    break;
                case e_Production_State.Editing:
                    break;
                case e_Production_State.Editting_ProductionDate:
                    break;
                case e_Production_State.Error:
                    break;
            }
        }

        private Task PPO_mainProcessTask;
        private async Task Process_Async()
        {
            try
            {
                while (!poPage_Main_Process.Token.IsCancellationRequested)
                {
                    try
                    {
                        TaskBody();
                    }
                    catch (Exception ex)
                    {
                        await Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi trong Main_Process_Async: {ex.Message}");
                        this.ShowErrorTip($"Lỗi EM05 trong quá trình xử lý: {ex.Message}");
                    }
                    await Task.Delay(500, poPage_Main_Process.Token);
                }
            }
            catch (TaskCanceledException) { }
        }
        public void Start_Process_Task()
        {
            if (PPO_mainProcessTask == null ||
                PPO_mainProcessTask.IsCompleted ||
                PPO_mainProcessTask.IsCanceled ||
                PPO_mainProcessTask.IsFaulted)
            {
                PPO_mainProcessTask = Task.Run(Process_Async, poPage_Main_Process.Token);
            }
            else
            {
                //không thể khởi động lại nhiệm vụ nếu nó đã đang chạy
            }
        }
        public void Stop_Process_Task()
        {
            if (poPage_Main_Process != null && !poPage_Main_Process.IsCancellationRequested)
            {
                poPage_Main_Process.Cancel();
            }
        }

        #endregion

        #region Các sự kiện nút bấm
        private void btnPO_Click(object sender, EventArgs e)
        {
            //ghi log người dùng nhấn nút chọn đơn hàng
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút chọn đơn hàng");
            switch (Globals.Production_State)
            {
                case e_Production_State.NoSelectedPO:

                    if(ipOrderNO.ReadOnly)
                    {
                        //nếu chưa có đơn hàng nào được chọn thì mở giao diện chọn đơn hàng
                        ipOrderNO.ReadOnly = false;
                        ipProductionDate.ReadOnly = false;
                        //đổi màu vàng
                        ipOrderNO.FillColor = Color.Yellow;
                        ipProductionDate.FillColor = Color.Yellow;
                        //đổi chữ màu đen
                        ipOrderNO.ForeColor = Color.Black;
                        ipProductionDate.ForeColor = Color.Black;

                        //đổi nút thành màu xanh
                        btnPO.FillColor = Color.FromArgb(0, 192, 0); //màu xanh lá cây
                        btnPO.Text = "Lưu PO"; //đổi chữ nút thành "Lưu đơn hàng và bắt đầu sản xuất"
                        //đổi symbol thành hình lưu
                        btnPO.Symbol = 61639; //hình lưu

                        //tắt nút thử
                        btnTestMode.Enabled = false;
                    }

                    //quăng sang trạng thái Editing
                    Globals.Production_State = e_Production_State.Editing;

                    break;
                case e_Production_State.Start:
                    break;
                case e_Production_State.Checking_PO_Info:
                    break;
                case e_Production_State.Loading:
                    break;
                case e_Production_State.Camera_Processing:
                    break;
                case e_Production_State.Pushing_new_PO_to_PLC:
                    break;
                case e_Production_State.Pushing_continue_PO_to_PLC:
                    break;
                case e_Production_State.Ready:
                    break;
                case e_Production_State.Running:
                    break;
                case e_Production_State.Completed:
                    break;
                case e_Production_State.Editing:
                    break;
                case e_Production_State.Editting_ProductionDate:
                    break;
                case e_Production_State.Error:
                    break;
            }
        }

        private void ipOrderNO_SelectedIndexChanged(object sender, EventArgs e)
        {
            //chỉ hoạt động ở trạng thái Editing và loading 
            if (Globals.Production_State == e_Production_State.Editing || Globals.Production_State == e_Production_State.Loading)
            {
                //ghi log người dùng chọn đơn hàng
                Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, $"Người dùng chọn đơn hàng: {ipOrderNO.Text}");
                //lấy thông tin đơn hàng từ MES
                var getPOInfo = Globals.ProductionData.getfromMES.ProductionOrder_Detail(ipOrderNO.Text);
                if (getPOInfo.issucess)
                {
                    //kiểm tra file dư liệu có tồn tại không
                    if (!Globals.ProductionData.Check_Database_File(ipOrderNO.SelectedText).issucess) {
                        //ghi log thất bại
                        POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Không tìm thấy dữ liệu đơn hàng: {ipOrderNO.SelectedText}");
                        //show dialog thông báo lỗi
                        this.ShowErrorDialog($"Lỗi EA001, Vui lòng liên hệ nhà cung cấp để kiểm tra lỗi này, Đây là lỗi VÔ CÙNG NGHIÊM TRỌNG");
                    }
                    //render thông tin đơn hàng lên giao diện
                    opProductionLine.Text = getPOInfo.PO.Rows[0]["productionLine"].ToString();
                    opOrderQty.Text = getPOInfo.PO.Rows[0]["orderQty"].ToString();
                    opCustomerOrderNO.Text = getPOInfo.PO.Rows[0]["customerOrderNo"].ToString();
                    opProductName.Text = getPOInfo.PO.Rows[0]["productName"].ToString();
                    opProductCode.Text = getPOInfo.PO.Rows[0]["productCode"].ToString();
                    opLotNumber.Text = getPOInfo.PO.Rows[0]["lotNumber"].ToString();
                    opGTIN.Text = getPOInfo.PO.Rows[0]["gtin"].ToString();
                    opShift.Text = getPOInfo.PO.Rows[0]["shift"].ToString();
                    opFactory.Text = getPOInfo.PO.Rows[0]["factory"].ToString();
                    opSite.Text = getPOInfo.PO.Rows[0]["site"].ToString();
                    dfdsf.Text = getPOInfo.PO.Rows[0]["uom"].ToString();

                    var CZCodeCount = Globals.ProductionData.getfromMES.Get_Unique_Code_MES_Count(ipOrderNO.Text);
                    if (!CZCodeCount.issucess)
                    {
                        //ghi log thất bại
                        POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng mã code thất bại: {CZCodeCount.message}");
                        //hiển thị thông báo lỗi
                        this.ShowErrorDialog($"Lỗi PP08: {CZCodeCount.message}");
                    }
                    opCZCodeCount.Text = CZCodeCount.CzCodeCount.ToString();

                    //Quăng thông tin vào Globals.ProductionData
                    Globals.ProductionData.orderNo = ipOrderNO.Text;
                    Globals.ProductionData.productionLine = opProductionLine.Text;
                    Globals.ProductionData.uom = dfdsf.Text;
                    Globals.ProductionData.productionLine = opProductionLine.Text;
                    Globals.ProductionData.orderQty = opOrderQty.Text;
                    Globals.ProductionData.customerOrderNo = opCustomerOrderNO.Text;
                    Globals.ProductionData.productName = opProductName.Text;
                    Globals.ProductionData.productCode = opProductCode.Text;
                    Globals.ProductionData.lotNumber = opLotNumber.Text;
                    Globals.ProductionData.gtin = opGTIN.Text;
                    Globals.ProductionData.shift = opShift.Text;
                    Globals.ProductionData.factory = opFactory.Text;
                    Globals.ProductionData.site = opSite.Text;
                    Globals.ProductionData.productionDate = ipProductionDate.Text;

                    Globals.ProductionData.totalCZCode = opCZCodeCount.Text;

                    //Xử lý các Count nặng hơn
                    GetCounter_1();


                }
                else
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin đơn hàng thất bại: {getPOInfo.message}");
                    this.ShowErrorTip($"Lỗi PP06: {getPOInfo.message}");
                }
            }
        }
        #endregion

        #region Các hàm dữ liệu

        public void GetCounter_1()
        {
            //đẩy các dữ liệu counter sang loading
            opCZRunCount.Text = "Đang tải";
            opPassCount.Text = "Đang tải";
            opFailCount.Text = "Đang tải";
            opAWSFullOKCount.Text = "Đang tải";
            opAWSNotSent.Text = "Đang tải";
            opAWSSentWating.Text = "Đang tải";

            //tạo 1 task để lấy dữ liệu counter 1
            Task.Run(() =>
            {
                //delay 1 giây để tránh quá tải
                Task.Delay(1000).Wait();
                var CZRunCount = Globals.ProductionData.getfromMES.Get_Unique_Code_MES_Count(ipOrderNO.Text);
                if (!CZRunCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng mã code thất bại: {CZRunCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorDialog($"Lỗi PP07: {CZRunCount.message}");

                }
                opCZRunCount.Text = CZRunCount.CzCodeCount.ToString();
                //lấy các counter cơ bản
                //lấy số record passed

                var PassCount = Globals.ProductionData.getDataPO.Get_Record_Count_By_Status(ipOrderNO.Text, e_Production_Status.Pass);
                if (!PassCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record passed thất bại: {PassCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorDialog($"Lỗi PP09: {PassCount.message}");
                }
                opPassCount.Text = PassCount.Count.ToString();

                //lấy số record failed
                var FailCount = Globals.ProductionData.getDataPO.Get_Record_Count_By_Status(ipOrderNO.Text, e_Production_Status.Fail);
                if (!FailCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record failed thất bại: {FailCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorDialog($"Lỗi PP10: {FailCount.message}");
                }
                opFailCount.Text = FailCount.Count.ToString();


                //lấy số record waiting
                var AWSFullOKCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "!=");
                if (!AWSFullOKCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record waiting thất bại: {AWSFullOKCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorDialog($"Lỗi PP11: {AWSFullOKCount.message}");
                }
                opAWSFullOKCount.Text = AWSFullOKCount.Count.ToString();

                //lấy số record not sent
                var AWSNotSent = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Pending, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0");
                if (!AWSNotSent.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record not sent thất bại: {AWSNotSent.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorDialog($"Lỗi PP12: {AWSNotSent.message}");
                }

                //lấy số send failed
                var AWSSentFailed = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Failed, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0");
                if (!AWSSentFailed.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record sent failed thất bại: {AWSSentFailed.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorDialog($"Lỗi PP13: {AWSSentFailed.message}");
                }
                opAWSNotSent.Text = AWSNotSent.Count.ToString() + "/" + AWSSentFailed.Count.ToString();


                //lấy số record sent waiting
                var AWSSentWating = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "=");
                if (!AWSSentWating.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record sent waiting thất bại: {AWSSentWating.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorDialog($"Lỗi PP14: {AWSSentWating.message}");
                }
                opAWSSentWating.Text = AWSSentWating.Count.ToString();
            });
        }

        #endregion


    }
}
