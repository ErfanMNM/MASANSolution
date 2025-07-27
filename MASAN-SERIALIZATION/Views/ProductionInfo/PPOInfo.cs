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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MASAN_SERIALIZATION.Views.ProductionInfo
{
    public partial class PPOInfo : UIPage
    {
        //khởi tạo biến toàn cục để ghi nhật ký cho giao diện này
        LogHelper<e_LogType> POPageLog;

        //biến toàn cục để lưu trữ trạng thái ứng dụng
        public CancellationTokenSource poPage_Main_Process = new CancellationTokenSource(); //token cho task chính
        private Task PPO_mainProcessTask;
        private Task _savingTask;
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
        int dem = 10; //biến đếm để kiểm tra xem task có chạy hay không
        public void TaskBody()
        {
            dem++;
            switch (Globals.Production_State) 
            {
                case e_Production_State.NoSelectedPO:
                    //bật các phím chức năng
                    if (!btnPO.Enabled)
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
                        //kiểm tra orderNo có trong danh sách của server không
                        var getPD = Globals.ProductionData.getfromMES.ProductionOrder_Detail(Globals.ProductionData.orderNo);

                        if (getPD.issucess)
                        {
                            //quăng qua loading nếu ipOrderNo có số lượng đơn hàng

                            if (ipOrderNO.Items.Count > 0)
                            {
                                Globals.Production_State = e_Production_State.Loading;
                            }

                        }
                        else
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier($"Lỗi PP404: {getPD.message}");
                                opTer.Text = "Không tìm thấy đơn hàng trong hệ thống MES, Vui lòng chọn đơn hàng khác.";
                                opTer.ForeColor = Color.Red;
                                opTer.Font = new Font("Tahoma", 14, FontStyle.Bold);
                            });
                            //quăng về trạng thái NoSelectedPO
                            Globals.Production_State = e_Production_State.NoSelectedPO;
                        }
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
                    //chọn po trong cbb

                    //kiểm tra xem orderNo có trong ipOrderNO không
                    bool exists = false;

                    foreach (var item in ipOrderNO.Items)
                    {
                        var drv = item as DataRowView;
                        if (drv != null && drv["orderNo"].ToString() == Globals.ProductionData.orderNo)
                        {
                            exists = true;
                            this.InvokeIfRequired(() =>
                            {
                                ipOrderNO.SelectedIndex = ipOrderNO.Items.IndexOf(item);
                            });

                            break;
                        }
                    }

                    if (exists)
                    {
                        // ipOrderNO.SelectedItem = Globals.ProductionData.orderNo;

                        //nhảy sang Saving
                        Task.Delay(2000).Wait(); //đợi 1 giây để tránh quá tải bên ccb
                        Globals.Production_State = e_Production_State.Saving;
                    }
                    else
                    {
                        //nếu không có thì quăng về trạng thái NoSelectedPO
                        this.ShowErrorNotifier("Lỗi PP041: Đơn hàng không tồn tại trong danh sách, Vui lòng chọn đơn hàng khác.");
                        //ghi log lỗi
                        POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, "Đơn hàng không tồn tại trong danh sách");
                        btnPO_Before_Edit();
                        Globals.Production_State = e_Production_State.NoSelectedPO;
                    }
                    break;
                case e_Production_State.Camera_Processing:
                    break;
                case e_Production_State.Pushing_new_PO_to_PLC:
                    break;
                case e_Production_State.Pushing_continue_PO_to_PLC:
                    break;
                case e_Production_State.Ready:
                    if (dem > 10)
                    {
                        Task.Run(() =>
                        {
                            Render_OP();
                        });
                        dem = 0; //reset biến đếm
                    }

                    //kiểm tra đơn hàng đã chạy hay chưa
                    if (Globals.ProductionData.counter.totalCount > 0)
                    {
                        //đã chạy rồi thì không cần làm gì cả
                        this.InvokeIfRequired(() =>
                        {
                            opTer.Text = "Đơn hàng đang chạy, chỉ có thể chỉnh ngày sản xuất - Trong trường hợp muốn hủy PO vui lòng nhấn phần cài đặt -> hủy PO và làm theo hướng dẫn";
                            opTer.ForeColor = Color.Red;
                            opTer.Font = new Font("Tahoma", 14, FontStyle.Bold);
                        });
                        if (!btnProductionDate.Enabled)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                //nếu nút chỉnh ngày sản xuất đang bật thì tắt nó đi
                                btnProductionDate.Enabled = true;
                                btnPO.Enabled = false; //bật nút PO
                                btnTestMode.Enabled = false; //bật nút Test Mode});

                            });
                        }
                    }

                    //kiểm tra đơn hàng đã đủ hay chưa 
                    if (Globals.ProductionData.counter.passCount == Globals.ProductionData.orderQty.ToInt32())
                    {
                        //đã đủ rồi thì không cần làm gì cả
                        this.InvokeIfRequired(() =>
                        {
                            opTer.Text = "Đơn hàng đã đủ số lượng, Vui lòng chọn đơn hàng khác.";
                            opTer.ForeColor = Color.Red;
                            opTer.Font = new Font("Tahoma", 14, FontStyle.Bold);
                        });
                        if (!btnProductionDate.Enabled)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                //nếu nút chỉnh ngày sản xuất đang bật thì tắt nó đi
                                btnProductionDate.Enabled = false;
                                btnPO.Enabled = true; //bật nút PO
                                btnTestMode.Enabled = false; //bật nút Test Mode});
                                //tắt nút RUN
                                btnRUN.Enabled = false; //tắt nút RUN
                            });
                        }

                        //quăng về trạng thái Completed
                        Globals.Production_State = e_Production_State.Completed;
                    }

                    break;
                case e_Production_State.Running:

                    if (!btnRUN.Enabled)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            //đổi tên nút RUN thành "Dừng sản xuất"
                            btnRUN.Text = "Dừng sản xuất";
                            //đổi màu nút RUN thành màu đỏ
                            btnRUN.FillColor = Color.Red; //màu đỏ
                            //đổi symbol thành hình dừng
                            btnRUN.Symbol = 61509; //hình dừng

                            btnRUN.Enabled = true; //bật nút RUN
                        });
                    }

                    if (Globals.ProductionData.counter.passCount == Globals.ProductionData.orderQty.ToInt32())
                    {
                        //Quăng về trạng thái kiểm Check_Queue
                        Globals.Production_State = e_Production_State.Checking_Queue;
                    }

                    break;
                case e_Production_State.Completed:
                    break;
                case e_Production_State.Editing:
                    break;
                case e_Production_State.Editting_ProductionDate:
                    break;
                case e_Production_State.Error:
                    break;
                case e_Production_State.Saving:
                    //Kiểm tra sơ bộ
                    if (Globals.ProductionData.getDataPO.Is_PO_Deleted(ipOrderNO.SelectedText))
                    {
                        this.ShowErrorNotifier("Lỗi PP02: Đơn hàng đã bị xóa, Vui lòng chọn đơn hàng khác.");
                        //quăng về trạng thái Edit 
                        btnPO_Before_Edit();
                        Globals.Production_State = e_Production_State.Editing;
                        break;
                    }

                    //kiểm tra xem có hoàn thành chưa
                    if (Globals.ProductionData.getDataPO.Is_PO_Completed(ipOrderNO.SelectedText))
                    {
                        this.ShowErrorNotifier("Lỗi PP03: Đơn hàng đã hoàn thành, Vui lòng chọn đơn hàng khác.");
                        //quăng về trạng thái Edit 
                        btnPO_Before_Edit();
                        Globals.Production_State = e_Production_State.Editing;
                        break;
                    }

                    //kiểm tra xem mã cz có > orderQty không
                    if (opCZCodeCount.Text.ToInt32() < opOrderQty.Text.ToInt32())
                    {
                        this.ShowErrorNotifier("Lỗi PP04: Số lượng mã MES gửi xuống chưa đủ");
                        //quăng về trạng thái Edit 

                        btnPO_Before_Edit();
                        Globals.Production_State = e_Production_State.Editing;
                        break;
                    }

                    if (opOrderQty.Text.ToInt32()%24 > 0)
                    {
                        this.ShowErrorDialog("Lỗi PP998: Đơn hàng có vấn đề về OrderQty, Sản lượng không đóng đủ thùng. Vui lòng kiểm tra lại phía tạo dữ liệu sản xuất MES của nhà máy.");
                        //quăng về trạng thái Edit 
                        btnPO_Before_Edit();
                        Globals.Production_State = e_Production_State.Editing;
                        break;
                    }

                    //saving dữ liệu
                    if (_savingTask == null || _savingTask.IsCompleted || _savingTask.IsCanceled || _savingTask.IsFaulted)
                    {
                        _savingTask = Task.Run(() => Saving());
                    }
                    else
                    {
                        //nếu đang chạy thì không làm gì cả
                    }

                    break;
                case e_Production_State.Pushing_to_Dic:
                    break;
                case e_Production_State.Checking_Queue:

                    if (Globals_Database.Insert_Product_To_Record_Queue.Count > 0 || Globals_Database.Update_Product_To_SQLite_Queue.Count > 0)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            opTer.Text = "Đang ghi dữ liệu vào cơ sở dữ liệu, Vui lòng đợi trong giây lát...";
                            opTer.ForeColor = Color.Teal;
                            opTer.Font = new Font("Tahoma", 14, FontStyle.Bold);
                            this.ShowErrorDialog("Lỗi PP231: Đang có dữ liệu đang được ghi vào cơ sở dữ liệu, Vui lòng đợi trong giây lát.");
                        });
                        
                        break;
                    }
                    else
                    {
                        //quăng về trạng thái Ready
                        Globals.Production_State = e_Production_State.Ready;
                    }

                    break;
            }
        }


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
                        this.InvokeIfRequired(() =>
                        {
                            opTer.Text = "Đã xảy ra lỗi trong quá trình xử lý, Vui lòng kiểm tra nhật ký để biết thêm chi tiết.";
                            opTer.ForeColor = Color.Red;
                            opTer.Font = new Font("Tahoma", 14, FontStyle.Bold);
                            this.ShowErrorNotifier($"Lỗi EM05 trong quá trình xử lý: {ex.Message}");
                        });
                        
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

                    btnPO_Before_Edit();
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
                    //kiểm tra xem đã chạy cái nào chưa
                    var CZRunCount = Globals.ProductionData.getDataPO.Get_Record_Count(ipOrderNO.Text);
                    if (!CZRunCount.issucess)
                    {
                        //ghi log thất bại
                        POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng mã code thất bại: {CZRunCount.message}");
                        //hiển thị thông báo lỗi
                        this.ShowErrorNotifier($"Lỗi PP07: {CZRunCount.message}");

                    }
                    opCZRunCount.Text = CZRunCount.RecordCount.ToString();

                    if (CZRunCount.RecordCount > 0 || Globals.ProductionData.counter.totalCount >0)
                    {
                        this.ShowErrorNotifier("Lỗi PP097: Đơn hàng đã sản xuất, không thể đổi đơn khác.");
                        //quăng về trạng thái Edit 
                        return;
                    }
                    btnPO_Before_Edit();
                    //quăng sang trạng thái Editing
                    Globals.Production_State = e_Production_State.Editing;
                    break;
                case e_Production_State.Running:
                    break;
                case e_Production_State.Completed:
                    break;
                case e_Production_State.Editing:

                    //tắt nút nhấn
                    btnPO.Enabled = false;
                    //đổi tên thành "Chọn PO"
                    btnPO.Text = "Đang lưu";
                    //đổi màu nút thành màu teal
                    btnPO.FillColor = Color.Teal; //màu teal
                    //đổi symbol thành hình chọn
                    btnPO.Symbol = 61508; //hình chọn

                    //tắt ipOrderNo và ipProductionDate
                    ipOrderNO.ReadOnly = true;
                    ipProductionDate.ReadOnly = true;
                    //đổi màu ipOrderNo và ipProductionDate thành màu CornflowerBlue
                    ipOrderNO.FillColor = Color.CornflowerBlue;
                    ipProductionDate.FillColor = Color.CornflowerBlue;
                    //đổi chữ ipOrderNo và ipProductionDate thành màu đen
                    ipOrderNO.ForeColor = Color.Black;
                    ipProductionDate.ForeColor = Color.Black;

                    opTer.Text = "Đang lưu thông tin đơn hàng, Vui lòng đợi trong giây lát...";

                    //chuyển trạng thái sang Loading
                    Globals.Production_State = e_Production_State.Saving;

                    break;
                case e_Production_State.Editting_ProductionDate:
                    break;
                case e_Production_State.Error:
                    break;
                case e_Production_State.Saving:

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
               
                Render_OP();
            }
        }

        private void btnRUN_Click(object sender, EventArgs e)
        {
            //ghi log người dùng nhấn nút RUN
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút RUN");
            switch (Globals.Production_State)
            {
                case e_Production_State.Ready:
                    btnRUN_Before_Running();
                    //kiểm tra xem các thiết bị đã sẵn sàng chưa
                    if(!Globals.APP_Ready)
                    {
                        this.ShowErrorNotifier("Lỗi PP590: Ứng dụng chưa sẵn sàng, Vui lòng kiểm tra lại.",false,5000);
                        btnPO_After_Running();
                        break;
                    }
                    //lấy lại counter cho chắc
                    Task.Run(() =>
                    {
                        var recordsDatabase = Globals.ProductionData.getDataPO.Get_Records(ipOrderNO.Text);

                        if (!recordsDatabase.issucess)
                        {
                            //ghi log thất bại
                            POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy dữ liệu đơn hàng thất bại: {recordsDatabase.message}");
                            //hiển thị thông báo lỗi
                            this.ShowErrorNotifier($"Lỗi PP05: {recordsDatabase.message}");
                            //quăng về trạng thái NoSelectedPO
                            return;
                        }
                        //reset counter
                        Globals.ProductionData.counter.Reset();
                        //lấy lại các counter
                        GetCounter_2(recordsDatabase.Records);
                        //chuyển sang trạng thái run

                        //kiểm tra số đếm xem đã chạy chưa
                        if(Globals.ProductionData.counter.totalCount == 0)
                        {
                            //chạy mới
                            Globals.Production_State = e_Production_State.Pushing_new_PO_to_PLC;
                        }
                        else
                        {
                            Globals.Production_State = e_Production_State.Pushing_continue_PO_to_PLC;
                        }

                    });
                    break;
                case e_Production_State.Running:
                    
                    //ghi log người dùng nhấn nút RUN
                    Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút dừng sản xuất");
                    btnPO_After_Running();
                    //quăng sang trạng thái kiểm tra Queue
                    Globals.Production_State = e_Production_State.Checking_Queue;

                    //kiểm
                    break;
            }
        }

        private void btnProductionDate_Click(object sender, EventArgs e)
        {
            btnProductionDate.Enabled = false;
            //ghi log người dùng nhấn nút chỉnh ngày sản xuất
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút chỉnh ngày sản xuất");
            switch (Globals.Production_State)
            {
                case e_Production_State.Ready:
                    //kiểm tra xem đơn hàng đã chạy chưa
                    if (Globals.ProductionData.counter.totalCount < 0)
                    {
                        //đã chạy rồi thì không thể chỉnh ngày sản xuất
                        this.ShowErrorNotifier("Lỗi PP096: Đơn hàng chưa chạy, không thể chỉnh ngày sản xuất.");
                        break;
                    }

                    
                    //bật ipProductionDate
                    ipProductionDate.ReadOnly = false;
                    //đổi màu ipProductionDate thành màu vàng chữ đen
                    ipProductionDate.FillColor = Color.Yellow; //màu vàng
                    ipProductionDate.ForeColor = Color.Black; //màu đen
                    //đổi tên nút thành "Đang chỉnh ngày sản xuất"
                    btnProductionDate.Text = "Lưu lại";
                    //đổi màu nút thành màu xanh lá
                    btnProductionDate.FillColor = Color.Green; //màu xanh lá
                    //đổi symbol thành hình lưu
                    btnProductionDate.Symbol = 61508; //hình lưu
                    //chuyển sang trạng thái Editting_ProductionDate
                    Globals.Production_State = e_Production_State.Editting_ProductionDate;

                    //bật nút chỉnh ngày sản xuất
                    btnProductionDate.Enabled = true; //bật nút chỉnh ngày sản xuất
                    break;
                case e_Production_State.Running:
                    this.ShowErrorNotifier("Lỗi PP098: Đang chạy sản xuất, không thể chỉnh ngày sản xuất.");
                    break;

                case e_Production_State.Editting_ProductionDate:
                    //ghi log người dùng nhấn nút lưu ngày sản xuất
                    Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút lưu ngày sản xuất");
                    //lưu lại ngày sản xuất
                    var saveProductionDate = Globals.ProductionData.Save_PO(Globals.ProductionData.orderNo, ipProductionDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), Globals.CurrentUser.Username);
                    if (saveProductionDate.issucces)
                    {
                        this.ShowSuccessTip("Lưu ngày sản xuất thành công");
                        
                        //đổi tên nút thành "Chỉnh ngày sản xuất"
                        btnProductionDate.Text = "Chỉnh ngày sản xuất";
                        //đổi màu nút thành màu CornflowerBlue
                        btnProductionDate.FillColor = Color.CornflowerBlue; //CornflowerBlue

                        //đổi ipProductionDate về trạng thái không chỉnh sửa
                        ipProductionDate.ReadOnly = true;
                        ipProductionDate.FillColor = Color.CornflowerBlue; //màu CornflowerBlue
                        ipProductionDate.ForeColor = Color.Black; //màu đen


                    }
                    else
                    {
                        this.ShowErrorNotifier($"Lỗi PP100: {saveProductionDate.message}");
                        //ghi log thất bại
                        POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lưu ngày sản xuất thất bại: {saveProductionDate.message}");
                    }
                    btnProductionDate.Enabled = true; //bật nút chỉnh ngày sản xuất
                    //quăng về trạng thái Ready
                    Globals.Production_State = e_Production_State.Ready;
                    break;
            }
        }
        #endregion

        #region Các hàm dữ liệu
        public void Render_OP()
        {
            var getPOInfo = Globals.ProductionData.getfromMES.ProductionOrder_Detail(ipOrderNO.Text);
            if (getPOInfo.issucess)
            {
                //kiểm tra file dư liệu có tồn tại không
                if (!Globals.ProductionData.Check_Database_File(ipOrderNO.SelectedText).issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Không tìm thấy dữ liệu đơn hàng: {ipOrderNO.SelectedText}");
                    //show dialog thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi EA001, Vui lòng liên hệ nhà cung cấp để kiểm tra lỗi này, Đây là lỗi VÔ CÙNG NGHIÊM TRỌNG");
                }
                
                

                var CZCodeCount = Globals.ProductionData.getfromMES.Get_Unique_Code_MES_Count(ipOrderNO.Text);
                if (!CZCodeCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng mã code thất bại: {CZCodeCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP08: {CZCodeCount.message}");
                }
                

                //render thông tin đơn hàng lên giao diện
                this.InvokeIfRequired(() =>
                {
                    opCZCodeCount.Text = CZCodeCount.CzCodeCount.ToString();

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
                    opUOM.Text = getPOInfo.PO.Rows[0]["uom"].ToString();
                });

                //Xử lý các Count nặng hơn
                GetCounter_1();
            }
            else
            {
                //ghi log thất bại
                POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin đơn hàng thất bại: {getPOInfo.message}");
                this.ShowErrorNotifier($"Lỗi PP06: {getPOInfo.message}");
            }
        }
        public void GetCounter_1()
        {
            this.InvokeIfRequired( () =>
            {
                //đẩy các dữ liệu counter sang loading
                opCZRunCount.Text = "Đang tải";
                opPassCount.Text = "Đang tải";
                opFailCount.Text = "Đang tải";
                opAWSFullOKCount.Text = "Đang tải";
                opAWSNotSent.Text = "Đang tải";
                opAWSSentWating.Text = "Đang tải";
            });
            

            //tạo 1 task để lấy dữ liệu counter 1
            Task.Run(() =>
            {
                //delay 1 giây để tránh quá tải
                Task.Delay(1000).Wait();
                var CZRunCount = Globals.ProductionData.getDataPO.Get_Record_Count(ipOrderNO.Text);
                if (!CZRunCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng mã code thất bại: {CZRunCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP07: {CZRunCount.message}");

                }
                opCZRunCount.Text = CZRunCount.RecordCount.ToString();
                //lấy các counter cơ bản
                //lấy số record passed

                var PassCount = Globals.ProductionData.getDataPO.Get_Record_Count_By_Status(ipOrderNO.Text, e_Production_Status.Pass);
                if (!PassCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record passed thất bại: {PassCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP09: {PassCount.message}");
                }
                opPassCount.Text = PassCount.Count.ToString();

                //lấy số record failed
                var FailCount = Globals.ProductionData.getDataPO.Get_Record_Count_By_Status(ipOrderNO.Text, e_Production_Status.Fail);
                if (!FailCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record failed thất bại: {FailCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP10: {FailCount.message}");
                }
                opFailCount.Text = FailCount.Count.ToString();


                //lấy số record waiting
                var AWSFullOKCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "!=");
                if (!AWSFullOKCount.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record waiting thất bại: {AWSFullOKCount.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP11: {AWSFullOKCount.message}");
                }
                opAWSFullOKCount.Text = AWSFullOKCount.Count.ToString();

                //lấy số record not sent
                var AWSNotSent = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Pending, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0");
                if (!AWSNotSent.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record not sent thất bại: {AWSNotSent.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP12: {AWSNotSent.message}");
                }

                //lấy số send failed
                var AWSSentFailed = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Failed, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0");
                if (!AWSSentFailed.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record sent failed thất bại: {AWSSentFailed.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP13: {AWSSentFailed.message}");
                }
                opAWSNotSent.Text = AWSNotSent.Count.ToString() + "/" + AWSSentFailed.Count.ToString();


                //lấy số record sent waiting
                var AWSSentWating = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "=");
                if (!AWSSentWating.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng record sent waiting thất bại: {AWSSentWating.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP14: {AWSSentWating.message}");
                }
                opAWSSentWating.Text = AWSSentWating.Count.ToString();
            });
        }

        public void GetAWSCounter()
        {
            Globals.ProductionData.awsSendCounter.pendingCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Pending, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0").Count;

            Globals.ProductionData.awsSendCounter.sentCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0").Count;

            Globals.ProductionData.awsSendCounter.failedCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Failed, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0").Count;

            Globals.ProductionData.awsRecivedCounter.waitingCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "=").Count;

            Globals.ProductionData.awsRecivedCounter.recivedCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "!=").Count;
        }

        public void GetCounter_2(DataTable dataTable)
        {
            

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                //nếu không có dữ liệu thì reset counter
                Globals.ProductionData.counter.Reset();
                Globals.ProductionData.awsSendCounter.Reset();
                Globals.ProductionData.awsRecivedCounter.Reset();

                var getmaxCartonID1 = Globals.ProductionData.getDataPO.Get_Max_Carton_ID(ipOrderNO.Text);

                if (getmaxCartonID1.issucess)
                {
                    Globals.ProductionData.counter.cartonID = getmaxCartonID1.MaxCartonID;
                }
                else
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng thùng lớn nhất thất bại: {getmaxCartonID1.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP15: {getmaxCartonID1.message}");
                }

                return;
            }

            foreach (DataRow row in dataTable.Rows)
            {
                //lấy dữ liệu từ SQLite
                string Code = row["Code"].ToString();
                string status = row["Status"].ToString();
                string activateDate = row["ActivateDate"].ToString();
                string productionDate = row["ProductionDate"].ToString();

                Globals.ProductionData.counter.totalCount++;

                if (status == e_Production_Status.Pass.ToString())
                {
                    Globals.ProductionData.counter.passCount++;
                }
                else if (status == e_Production_Status.ReadFail.ToString())
                {
                    Globals.ProductionData.counter.readfailCount++;
                }
                else if (status == e_Production_Status.NotFound.ToString())
                {
                    Globals.ProductionData.counter.notfoundCount++;
                }
                else if (status == e_Production_Status.Duplicate.ToString())
                {
                    Globals.ProductionData.counter.duplicateCount++;
                }
                else if (status == e_Production_Status.Error.ToString())
                {
                    Globals.ProductionData.counter.errorCount++;
                }

                //số fail là các số không phải pass
                if (status != e_Production_Status.Pass.ToString())
                {
                    Globals.ProductionData.counter.failCount++;
                }
            }

            //lấy id thùng lớn nhất
            var getmaxCartonID = Globals.ProductionData.getDataPO.Get_Max_Carton_ID(ipOrderNO.Text);

            if (getmaxCartonID.issucess)
            {
                Globals.ProductionData.counter.cartonID = getmaxCartonID.MaxCartonID;
            }
            else
            {
                //ghi log thất bại
                POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy thông tin số lượng thùng lớn nhất thất bại: {getmaxCartonID.message}");
                //hiển thị thông báo lỗi
                this.ShowErrorNotifier($"Lỗi PP15: {getmaxCartonID.message}");
            }

        }

        public void Saving()
        {
                //delay 1 giây để tránh quá tải
                Task.Delay(1000).Wait();

                Globals.ProductionData.orderNo = ipOrderNO.SelectedText;
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

                //tạm thời get dữ liệu counter từ MES
                var recordsDatabase = Globals.ProductionData.getDataPO.Get_Records(ipOrderNO.Text);

                if (!recordsDatabase.issucess)
                {
                    //ghi log thất bại
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lấy dữ liệu đơn hàng thất bại: {recordsDatabase.message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP05: {recordsDatabase.message}");
                    //quăng về trạng thái NoSelectedPO
                    Globals.Production_State = e_Production_State.NoSelectedPO;
                    return;
                }

                //reset counter

                Globals.ProductionData.counter.Reset();
                Globals.ProductionData.awsSendCounter.Reset();
                Globals.ProductionData.awsRecivedCounter.Reset();

                GetCounter_2(recordsDatabase.Records);

                GetAWSCounter();

                //lưu dữ liệu vào SQLite
                //chuyển ngày sản xuất sang định dạng yyyy-MM-dd HH:mm:ss.fff thành giờ utc
                try
                {
                    DateTime productionDateUtc = DateTime.ParseExact(ipProductionDate.Text, "yyyy-MM-dd HH:mm:ss", null).ToUniversalTime();
                    var savePO = Globals.ProductionData.Save_PO(ipOrderNO.SelectedText, productionDateUtc.ToString("yyyy-MM-dd HH:mm:ss.fff"), Globals.CurrentUser.Username);

                    if (!savePO.issucces)
                    {
                        //ghi log thất bại
                        POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lưu đơn hàng thất bại: {savePO.message}");
                        //hiển thị thông báo lỗi
                        this.ShowErrorNotifier($"Lỗi PP909: {savePO.message}");
                        //quăng về trạng thái NoSelectedPO
                        Globals.Production_State = e_Production_State.NoSelectedPO;
                        return;
                    }

                    //ghi log thành công hệ thống
                    Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, $"Người dùng đã lưu đơn hàng: {ipOrderNO.SelectedText}");

                    this.InvokeIfRequired(() =>
                    {
                        //hiển thị thông báo thành công
                        this.ShowSuccessTip("Đã lưu thông tin đơn hàng thành công.");
                        //hiển thị thông tin đơn hàng đã lưu
                        opTer.Text = $"Đơn hàng {ipOrderNO.SelectedText} đã được lưu thành công.";
                        opTer.ForeColor = Color.Green;
                        opTer.Font = new Font("Tahoma", 14, FontStyle.Bold);

                        //bật các nút chức năng
                        btnPO.Enabled = true;
                        btnTestMode.Enabled = true;
                        btnRUN.Enabled = true;
                        btnPO.Text = "Đổi PO"; //đổi chữ nút thành "Chọn PO"
                    });
                }
                catch (Exception ex)
                {
                    //ghi log lỗi
                    POPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lỗi khi lưu đơn hàng: {ex.Message}");
                    //hiển thị thông báo lỗi
                    this.ShowErrorNotifier($"Lỗi PP088: {ex.Message}");
                    return;
                }
                //Quăng về trạng thái ready
                Globals.Production_State = e_Production_State.Ready;
        }

        #endregion

        #region Hàm của btnPO
        private void btnPO_Before_Edit()
        {
            this.InvokeIfRequired(() =>
            {
                if (ipOrderNO.ReadOnly)
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
                    btnPO.Enabled = true; //bật nút nhấn
                    btnPO.FillColor = Color.FromArgb(0, 192, 0); //màu xanh lá cây
                    btnPO.Text = "Lưu PO"; //đổi chữ nút thành "Lưu đơn hàng và bắt đầu sản xuất"
                                           //đổi symbol thành hình lưu
                    btnPO.Symbol = 61639; //hình lưu

                    //tắt nút thử
                    btnTestMode.Enabled = false;
                    //tắt nút chạy
                    btnRUN.Enabled = false;
                }
            });
            
        }

        #endregion

        #region Hàm tính năng của btnRun

        private void btnRUN_Before_Running ()
        {
            this.InvokeIfRequired(() =>
            {
                //đổi nút thành màu đỏ
                btnRUN.Enabled = false; //tắt nút nhấn
                btnRUN.FillColor = Color.Red;
                btnRUN.Text = "Đang chuẩn bị"; //đổi chữ nút thành "Dừng sản xuất"
                //đổi symbol thành hình dừng
                btnRUN.Symbol = 61508; //hình dừng

                //tắt nút nhấn
                btnPO.Enabled = false; //tắt nút nhấn
                btnProductionDate.Enabled = false; //tắt nút nhấn
                btnTestMode.Enabled = false; //tắt nút nhấn
            });
        }

        private void btnPO_After_Running()
        {
            this.InvokeIfRequired(() =>
            {
                //khôi phục lại nút nhấn
                //nếu đơn hàng chưa chạy cái nào thì bật lại nút PO
                if (Globals.ProductionData.counter.totalCount <= 0)
                {
                    btnPO.Enabled = true; //bật nút nhấn
                    btnPO.FillColor = Color.FromArgb(0, 192, 0); //màu xanh lá cây
                    btnPO.Text = "Chọn PO"; //đổi chữ nút thành "Chọn PO"
                    //đổi symbol thành hình chọn
                    btnPO.Symbol = 61508; //hình chọn

                    //bật nút thử
                    btnTestMode.Enabled = true; //bật nút nhấn
                    //tắt nút ProductionDate
                    btnProductionDate.Enabled = false; //tắt nút nhấn
                    //bật nút RUN
                    btnRUN.Enabled = true; //bật nút nhấn
                    btnRUN.FillColor = Color.FromArgb(0, 192, 0); //màu xanh lá cây
                    btnRUN.Text = "Bắt đầu sản xuất"; //đổi chữ nút thành "Bắt đầu sản xuất"
                    //đổi symbol thành hình chạy
                    btnRUN.Symbol = 61508; //hình chạy
                }
                else
                {
                    btnPO.Enabled = false; //tắt nút nhấn
                    btnRUN.Enabled = true; //bật nút nhấn
                    btnRUN.FillColor = Color.FromArgb(0, 192, 0); //màu xanh lá cây
                    btnRUN.Text = "Bắt đầu sản xuất"; //đổi chữ nút thành "Dừng sản xuất"
                    //đổi symbol thành hình chạy
                    btnRUN.Symbol = 61508; //hình chạy

                    //tắt nút thử
                    btnTestMode.Enabled = false; //tắt nút nhấn
                    //bật nút ProductionDate
                    btnProductionDate.Enabled = true; //bật nút nhấn

                }
            });
        }
        #endregion

        #region Hàm tính năng của btnProductionDate

        #endregion


    }
}
