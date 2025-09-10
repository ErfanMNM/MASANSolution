using Google.Apis.Util;
using MASAN_SERIALIZATION.Configs;
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
using static MASAN_SERIALIZATION.Production.ProductionOrder;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MASAN_SERIALIZATION.Views.ProductionInfo
{
    public partial class PPOInfo : UIPage
    {
        #region Private Fields

        private LogHelper<e_LogType> _pageLogger;
        private int _processCounter = 10;

        private readonly BackgroundWorker _mainProcessWorker = new BackgroundWorker()
        {
            WorkerSupportsCancellation = true
        };

        private readonly BackgroundWorker _savingWorker = new BackgroundWorker()
        {
            WorkerSupportsCancellation = true
        };

        #endregion

        #region Constructor and Initialization

        public PPOInfo()
        {
            InitializeComponent();
        }

        public void START()
        {
            InitializeLogger();
            InitializeOrderNoComboBox();
            InitializeBackgroundWorkers();
            StartMainProcess();
        }

        private void InitializeLogger()
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MASAN-SERIALIZATION", "Logs", "Pages", "PPOlog.ptl"
            );
            _pageLogger = new LogHelper<e_LogType>(logPath);
        }

        private void InitializeOrderNoComboBox()
        {
            var loadResult = Globals.ProductionData.getfromMES.MES_Load_OrderNo_ToComboBox(ipOrderNO);
            if (!loadResult.issucess)
            {
                string errorResponse = $"{{\"error\":\"{loadResult.message}\"}}";
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error,
                    "Tạo thất bại danh sách đơn hàng từ MES", errorResponse);
            }
        }

        private void InitializeBackgroundWorkers()
        {
            _mainProcessWorker.DoWork += (s, e) => ProcessMainLoop();
            _savingWorker.DoWork += (s, e) => ExecuteSavingProcess();
        }

        private void StartMainProcess()
        {
            if (!_mainProcessWorker.IsBusy)
            {
                _mainProcessWorker.RunWorkerAsync();
            }
            else
            {
                this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
            }
        }

        private void PPOInfo_Initialize(object sender, EventArgs e)
        {
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction,
                "Người dùng mở giao diện chỉnh thông tin sản xuất");
        }

        private void PPOInfo_Finalize(object sender, EventArgs e)
        {
            // Clean up resources if needed
        }

        #endregion

        #region Main Process Loop

        private void ProcessMainLoop()
        {
            try
            {
                while (!_mainProcessWorker.CancellationPending)
                {
                    try
                    {
                        ProcessProductionState();
                    }
                    catch (Exception ex)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            UpdateStatusMessage("Đã xảy ra lỗi trong quá trình xử lý, Vui lòng kiểm tra nhật ký để biết thêm chi tiết.", Color.Red);
                            this.ShowErrorDialog($"Lỗi EM0523 trong quá trình xử lý: {ex.Message}");
                        });
                        HandleProcessError(ex);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (TaskCanceledException) { }
        }

        private void ProcessProductionState()
        {
            _processCounter++;

            switch (Globals.Production_State)
            {
                case e_Production_State.NoSelectedPO:
                    HandleNoSelectedPOState();
                    break;
                case e_Production_State.Start:

                    if (Globals.CurrentUser.Username.Length > 1)
                    {
                        HandleStartState();
                    }
                    break;
                case e_Production_State.Loading:
                    HandleLoadingState();
                    break;
                case e_Production_State.Ready:
                    HandleReadyState();
                    break;
                case e_Production_State.Running:
                    HandleRunningState();
                    break;
                case e_Production_State.Saving:
                    HandleSavingState();
                    break;
                case e_Production_State.Checking_Queue:
                    HandleCheckingQueueState();
                    break;
                case e_Production_State.Checking_PO_Info:
                    break;
                case e_Production_State.Camera_Processing:
                    break;
                case e_Production_State.Pushing_new_PO_to_PLC:
                    break;
                case e_Production_State.Pushing_continue_PO_to_PLC:
                    break;
                case e_Production_State.Completed:

                    if (_processCounter > 50)
                    {
                        Task.Run(() => RenderOrderInfo());
                        _processCounter = 0;
                    }

                    this.InvokeIfRequired(() =>
                    {
                        if(btnRUN.Enabled)
                        {
                            UpdateStatusMessage("Đơn hàng đã hoàn thành, Vui lòng chọn đơn hàng khác.", Color.Red);
                            btnPO.Enabled = true;
                            btnClosePO.Enabled = false;
                            btnRUN.Enabled = false;
                            btnProductionDate.Enabled = false;
                        }
                        
                    });
                    break;
                case e_Production_State.Editing:
                    break;
                case e_Production_State.Editting_ProductionDate:
                    break;
                case e_Production_State.Error:
                    break;
                case e_Production_State.Pushing_to_Dic:
                    break;
                case e_Production_State.Pause:
                    break;
                case e_Production_State.Waiting_Stop:
                    if (_processCounter > 50)
                    {
                        Task.Run(() => RenderOrderInfo());
                        _processCounter = 0;
                    }
                    break;
                case e_Production_State.Check_After_Completed:
                    break;
                // Other states can be handled as needed
                default:
                    break;
            }
        }

        private void HandleProcessError(Exception ex)
        {
            Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi trong Main_Process_Async: {ex.Message}");
            this.InvokeIfRequired(() =>
            {
                UpdateStatusMessage("Đã xảy ra lỗi trong quá trình xử lý, Vui lòng kiểm tra nhật ký để biết thêm chi tiết.", Color.Red);
                this.ShowErrorDialog($"Lỗi EM05 trong quá trình xử lý: {ex.Message}");
            });
        }

        #endregion

        #region Production State Handlers

        private void HandleNoSelectedPOState()
        {
            if (!btnPO.Enabled)
            {
                this.InvokeIfRequired(() => {
                    btnPO.Enabled = true;
                    btnClosePO.Enabled = true;
                });
            }
        }

        private void HandleStartState()
        {
            TResult lastPOResult = Globals.ProductionData.getDataPO.GetLastPO();

            if (lastPOResult.issuccess)
            {
                ProcessLastPOSuccess(lastPOResult);
            }
            else
            {
                HandleNoLastPO();
            }
        }

        private void ProcessLastPOSuccess(TResult lastPO)
        {
            Globals.Production_State = e_Production_State.Checking_PO_Info;
            if(lastPO.data != null)
            {
                Globals.ProductionData.orderNo = lastPO.data.Rows[0]["orderNo"].ToString();
            }

            TResult orderDetailResult = Globals.ProductionData.getfromMES.ProductionOrder_Detail(Globals.ProductionData.orderNo);
            if (orderDetailResult.issuccess && ipOrderNO.Items.Count > 0)
            {
                Globals.Production_State = e_Production_State.Loading;
            }
            else
            {
                HandleOrderNotFound(orderDetailResult.message);
            }
        }

        private void HandleNoLastPO()
        {
            this.InvokeIfRequired(() =>
            {
                UpdateStatusMessage("Không có đơn hàng nào được chọn. Vui lòng chọn đơn hàng để tiếp tục.", Color.Teal);
            });
            Globals.Production_State = e_Production_State.NoSelectedPO;
        }

        private void HandleOrderNotFound(string errorMessage)
        {
            this.InvokeIfRequired(() =>
            {
                this.ShowErrorDialog($"Lỗi PP404: {errorMessage}");
                UpdateStatusMessage("Không tìm thấy đơn hàng trong hệ thống MES, Vui lòng chọn đơn hàng khác.", Color.Red);
            });
            Globals.Production_State = e_Production_State.NoSelectedPO;
        }

        private void HandleLoadingState()
        {
            if (TrySelectOrderInComboBox())
            {
                Task.Delay(2000).Wait();
                Globals.Production_State = e_Production_State.Saving;
            }
            else
            {
                HandleOrderNotInList();
            }
        }

        private bool TrySelectOrderInComboBox()
        {
            foreach (var item in ipOrderNO.Items)
            {
                if (item is DataRowView drv && drv["orderNo"].ToString() == Globals.ProductionData.orderNo)
                {
                    this.InvokeIfRequired(() => ipOrderNO.SelectedIndex = ipOrderNO.Items.IndexOf(item));
                    return true;
                }
            }
            return false;
        }

        private void HandleOrderNotInList()
        {
            this.ShowErrorDialog("Lỗi PP041: Đơn hàng không tồn tại trong danh sách, Vui lòng chọn đơn hàng khác.");
            _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, "Đơn hàng không tồn tại trong danh sách");
            SetEditMode();
            Globals.Production_State = e_Production_State.NoSelectedPO;
        }

        private void HandleReadyState()
        {
            if (_processCounter > 50)
            {
                Task.Run(() => RenderOrderInfo());
                _processCounter = 0;
            }

            CheckOrderStatus();
        }

        private void CheckOrderStatus()
        {
            if (Globals.ProductionData.counter.totalCount > 0)
            {
                HandleRunningOrder();
            }

            if (IsOrderCompleted())
            {
                HandleCompletedOrder();
            }
        }

        private void HandleRunningOrder()
        {
            this.InvokeIfRequired(() =>
            {
                UpdateStatusMessage("Đơn hàng đang chạy, chỉ có thể chỉnh ngày sản xuất - Trong trường hợp muốn hủy PO vui lòng nhấn phần cài đặt -> hủy PO và làm theo hướng dẫn", Color.Red);
                if (!btnProductionDate.Enabled)
                {
                    btnProductionDate.Enabled = true;
                    btnPO.Enabled = false;
                    btnClosePO.Enabled = false;
                }
            });
        }

        private bool IsOrderCompleted()
        {
            return Globals.ProductionData.counter.passCount == Globals.ProductionData.orderQty.ToInt32();
        }

        private void HandleCompletedOrder()
        {
            this.InvokeIfRequired(() =>
            {
                UpdateStatusMessage("Đơn hàng đã đủ số lượng, Vui lòng chọn đơn hàng khác.", Color.Red);
                if (!btnProductionDate.Enabled)
                {
                    btnProductionDate.Enabled = false;
                    btnPO.Enabled = true;
                    btnClosePO.Enabled = false;
                    btnRUN.Enabled = false;
                }
            });
            Globals.Production_State = e_Production_State.Completed;
        }

        private void HandleRunningState()
        {
            if (_processCounter > 50)
            {
                Task.Run(() => RenderOrderInfo());
                _processCounter = 0;
            }

            if (!btnRUN.Enabled)
            {
                this.InvokeIfRequired(() =>
                {
                    ConfigureStopButton();
                    btnProductionDate.Enabled = false;
                });
            }

            if (IsOrderCompleted())
            {
                Globals.Production_State = e_Production_State.Waiting_Stop;
                //Globals.Production_State = e_Production_State.Checking_Queue;
            }
        }

        private void ConfigureStopButton()
        {
            btnRUN.Text = "Dừng sản xuất";
            btnRUN.FillColor = Color.Red;
            btnRUN.Symbol = 61517;
            btnRUN.Enabled = true;
        }

        private void HandleSavingState()
        {
            if (ValidateOrderForSaving() && !_savingWorker.IsBusy)
            {
                this.InvokeIfRequired(() =>
                {
                    UpdateStatusMessage("Đang lưu dữ liệu, Vui lòng đợi trong giây lát...", Color.Teal);
                });
                _savingWorker.RunWorkerAsync();
            }
        }

        private bool ValidateOrderForSaving()
        {
            if (Globals.ProductionData.getDataPO.Is_PO_Deleted(ipOrderNO.SelectedText))
            {
                this.ShowErrorDialog("Lỗi PP02: Đơn hàng đã bị xóa, Vui lòng chọn đơn hàng khác.");
                SetEditMode();
                Globals.Production_State = e_Production_State.Editing;
                return false;
            }

            if (Globals.ProductionData.getDataPO.Is_PO_Completed(ipOrderNO.SelectedText))
            {
                this.InvokeIfRequired(() =>
                {
                    UpdateStatusMessage("Đơn hàng đã hoàn thành, Vui lòng chọn đơn hàng khác.", Color.Red);
                    this.ShowErrorDialog("Lỗi PP03: Đơn hàng đã hoàn thành, Vui lòng chọn đơn hàng khác.");
                });
                SetEditMode();
                Globals.Production_State = e_Production_State.Editing;
                return false;
            }

            if (opCZCodeCount.Text.ToInt32() < opOrderQty.Text.ToInt32())
            {
                this.InvokeIfRequired(() =>
                {
                    UpdateStatusMessage("Số lượng mã CZ gửi xuống chưa đủ, Vui lòng kiểm tra lại.", Color.Red);
                    this.ShowErrorDialog("Lỗi PP04: Số lượng mã MES gửi xuống chưa đủ");
                });
                SetEditMode();
                Globals.Production_State = e_Production_State.Editing;
                return false;
            }

            return true;
        }

        private void HandleCheckingQueueState()
        {
            if (Globals_Database.Insert_Product_To_Record_Queue.Count > 0 || Globals_Database.Update_Product_To_SQLite_Queue.Count > 0 || Globals_Database.Insert_Product_To_Record_CS_Queue.Count > 0 || Globals_Database.Update_Product_To_Record_Carton_Queue.Count > 0 ||Globals_Database.aWS_Recive_Datas.Count > 0 || Globals_Database.Activate_Carton.Count > 0 || Globals_Database.aWS_Send_Datas.Count > 0)
            {
                this.InvokeIfRequired(() =>
                {
                    UpdateStatusMessage("Đang ghi dữ liệu vào cơ sở dữ liệu, Vui lòng đợi trong giây lát...", Color.Teal);
                    this.ShowErrorDialog("Lỗi PP231: Đang có dữ liệu đang được ghi vào cơ sở dữ liệu, Vui lòng đợi trong giây lát.");
                });
            }
            else
            {
                Globals.Production_State = e_Production_State.Ready;
            }
        }

        #endregion

        #region Button Event Handlers

        private void btnPO_Click(object sender, EventArgs e)
        {
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút chọn đơn hàng");
            btnPO.Enabled = false;

            switch (Globals.Production_State)
            {
                case e_Production_State.NoSelectedPO:
                    SetEditMode();
                    Globals.Production_State = e_Production_State.Editing;
                    break;

                case e_Production_State.Ready:
                    HandlePOClickInReadyState();
                    break;

                case e_Production_State.Editing:
                    HandlePOClickInEditingState();
                    break;
                case e_Production_State.Completed:
                    SetEditMode();
                    Globals.Production_State = e_Production_State.Editing;
                    break;
            }
        }

        private void HandlePOClickInReadyState()
        {
            TResult recordCountResult = Globals.ProductionData.getDataPO.Get_Record_Count(ipOrderNO.Text);
            if (!recordCountResult.issuccess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lấy thông tin số lượng mã code thất bại: {recordCountResult.message}");
                this.ShowErrorDialog($"Lỗi PP07: {recordCountResult.message}");
            }

            opCZRunCount.Text = recordCountResult.count.ToString();

            if (recordCountResult.count > 0 || Globals.ProductionData.counter.totalCount > 0)
            {
                this.ShowErrorDialog("Lỗi PP097: Đơn hàng đã sản xuất, không thể đổi đơn khác.");
                return;
            }

            Globals.ProductionData.getfromMES.MES_Load_OrderNo_ToComboBox(ipOrderNO);

            if (ipOrderNO.Items.Count == 0)
            {
                this.ShowErrorDialog("Lỗi PP042: Không có đơn hàng nào trong hệ thống MES, Vui lòng liên hệ quản trị viên.");
                SetEditMode();
                Globals.Production_State = e_Production_State.Editing;
                return;
            }

            SetEditMode();
            Globals.Production_State = e_Production_State.Editing;
        }

        private void HandlePOClickInEditingState()
        {
            ConfigureSavingMode();
            Globals.Production_State = e_Production_State.Saving;
        }

        private void ConfigureSavingMode()
        {
            btnPO.Enabled = false;
            btnPO.Text = "Đang lưu";
            btnPO.FillColor = Color.Teal;
            btnPO.Symbol = 61508;

            ipOrderNO.ReadOnly = true;
            ipProductionDate.ReadOnly = true;
            ipOrderNO.FillColor = Color.CornflowerBlue;
            ipProductionDate.FillColor = Color.CornflowerBlue;
            ipOrderNO.ForeColor = Color.Black;
            ipProductionDate.ForeColor = Color.Black;

            opTer.Text = "Đang lưu thông tin đơn hàng, Vui lòng đợi trong giây lát...";
        }

        private void ipOrderNO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Globals.Production_State == e_Production_State.Editing || Globals.Production_State == e_Production_State.Loading)
            {
                Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, $"Người dùng chọn đơn hàng: {ipOrderNO.Text}");
                RenderOrderInfo();
            }
        }

        private void btnRUN_Click(object sender, EventArgs e)
        {
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút RUN");
            
            switch (Globals.Production_State)
            {
                case e_Production_State.Ready:
                    HandleRunButtonInReadyState();
                    break;

                case e_Production_State.Running:
                    HandleRunButtonInRunningState();
                    break;
                case e_Production_State.Pause:
                    this.ShowErrorDialog("Lỗi PP443: Vui lòng hoàn tất các sự kiện trước khi dừng sản xuất");
                    break;
            }
        }

        private void HandleRunButtonInReadyState()
        {
            ConfigurePreparingMode();
            if (!Globals.APP_Ready)
            {
                this.ShowErrorDialog("Lỗi PP590: Ứng dụng chưa sẵn sàng, Vui lòng kiểm tra lại.", false, 5000);
                RestoreAfterRunning();
                return;
            }

            if (!Globals.Device_Ready)
            {
                this.ShowErrorDialog("Lỗi PP591: Thiết bị chưa sẵn sàng, Vui lòng kiểm tra lại.");
                RestoreAfterRunning();
                return;
            }
            Task.Run(() => PrepareProductionData());
        }

        private void PrepareProductionData()
        {
            Thread.Sleep(1000);
            TResult recordsResult = Globals.ProductionData.getDataPO.Get_Records(ipOrderNO.Text);
            
            if (!recordsResult.issuccess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lấy dữ liệu đơn hàng thất bại: {recordsResult.message}");
                this.ShowErrorDialog($"Lỗi PP05: {recordsResult.message}");
                return;
            }

            ResetCounters();
            CalculateCounters(recordsResult.data);

            if (Globals.ProductionData.counter.totalCount == 0)
            {
                Globals.Production_State = e_Production_State.Pushing_new_PO_to_PLC;
            }
            else
            {
                Globals.Production_State = e_Production_State.Pushing_continue_PO_to_PLC;
            }
        }

        private void HandleRunButtonInRunningState()
        {
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút dừng sản xuất");
            RestoreAfterRunning();
            Globals.Production_State = e_Production_State.Checking_Queue;
        }

        private void btnProductionDate_Click(object sender, EventArgs e)
        {
            btnProductionDate.Enabled = false;
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút chỉnh ngày sản xuất");

            switch (Globals.Production_State)
            {
                case e_Production_State.Ready:
                    HandleProductionDateInReadyState();
                    break;

                case e_Production_State.Running:
                    this.ShowErrorDialog("Lỗi PP098: Đang chạy sản xuất, không thể chỉnh ngày sản xuất.");
                    break;

                case e_Production_State.Editting_ProductionDate:
                    HandleSaveProductionDate();
                    break;
            }
        }

        private void HandleProductionDateInReadyState()
        {
            if (Globals.ProductionData.counter.totalCount < 0)
            {
                this.ShowErrorDialog("Lỗi PP096: Đơn hàng chưa chạy, không thể chỉnh ngày sản xuất.");
                return;
            }

            ConfigureProductionDateEditMode();
        }

        private void ConfigureProductionDateEditMode()
        {
            ipProductionDate.ReadOnly = false;
            ipProductionDate.FillColor = Color.Yellow;
            ipProductionDate.ForeColor = Color.Black;
            btnProductionDate.Text = "Lưu lại";
            btnProductionDate.FillColor = Color.Green;
            btnProductionDate.Symbol = 61508;
            btnProductionDate.Enabled = true;
            Globals.Production_State = e_Production_State.Editting_ProductionDate;
        }

        private void HandleSaveProductionDate()
        {
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, "Người dùng nhấn nút lưu ngày sản xuất");
            
            var saveResult = Globals.ProductionData.Save_PO(Globals.ProductionData.orderNo, 
                ipProductionDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), Globals.CurrentUser.Username);

            if (saveResult.issucces)
            {
                this.ShowSuccessTip("Lưu ngày sản xuất thành công");
                RestoreProductionDateMode();
            }
            else
            {
                this.ShowErrorDialog($"Lỗi PP100: {saveResult.message}");
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lưu ngày sản xuất thất bại: {saveResult.message}");
            }

            btnProductionDate.Enabled = true;
            Globals.Production_State = e_Production_State.Ready;
        }

        private void RestoreProductionDateMode()
        {
            btnProductionDate.Text = "Chỉnh ngày sản xuất";
            btnProductionDate.FillColor = Color.CornflowerBlue;
            ipProductionDate.ReadOnly = true;
            ipProductionDate.FillColor = Color.CornflowerBlue;
            ipProductionDate.ForeColor = Color.Black;
        }

        #endregion

        #region Data Operations

        public void RenderOrderInfo()
        {
            TResult orderInfoResult = Globals.ProductionData.getfromMES.ProductionOrder_Detail(ipOrderNO.Text);
            //lấy thông tin productionDate đã chỉnh
            var POlog = Globals.ProductionData.getDataPO.Get_PO_Run_History_By_OrderNo(ipOrderNO.Text);

            if (POlog.issucess && POlog.logPO.Rows.Count > 0)
            {
                this.InvokeIfRequired(() =>
                {
                    ipProductionDate.Value = DateTime.Parse(POlog.logPO.Rows[0]["productionDate"].ToString());
                });
                
            }

            if (!orderInfoResult.issuccess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lấy thông tin đơn hàng thất bại: {orderInfoResult.message}");
                this.ShowErrorDialog($"Lỗi PP06: {orderInfoResult.message}");
                return;
            }

            ValidateDatabaseFile(orderInfoResult.data);
            TResult codeCountResult = GetCodeCount();
            UpdateOrderInfoUI(orderInfoResult, codeCountResult);
            LoadCountersAsync();
        }

        private void ValidateDatabaseFile(DataTable PO)
        {
            string orderQty = PO.Rows[0]["orderQty"].ToString();

            var checkResult = Globals.ProductionData.Check_Database_File(ipOrderNO.SelectedText, 
                orderQty);
            if (!checkResult.issucess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Không tìm thấy dữ liệu đơn hàng: {ipOrderNO.SelectedText}");
                this.ShowErrorDialog($"Lỗi EA001, Vui lòng liên hệ nhà cung cấp để kiểm tra lỗi này, Đây là lỗi VÔ CÙNG NGHIÊM TRỌNG");
            }
        }

        private TResult GetCodeCount()
        {
            TResult codeCountResult = Globals.ProductionData.getfromMES.Get_Unique_Code_MES_Count(ipOrderNO.Text);
            if (!codeCountResult.issuccess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lấy thông tin số lượng mã code thất bại: {codeCountResult.message}");
                this.ShowErrorDialog($"Lỗi PP08: {codeCountResult.message}");
            }
            return codeCountResult;
        }

        private void UpdateOrderInfoUI(TResult orderInfoResult, TResult codeCountResult)
        {
            this.InvokeIfRequired(() =>
            {
                var row = orderInfoResult.data.Rows[0];
                opCZCodeCount.Text = codeCountResult.count.ToString();
                opProductionLine.Text = row["productionLine"].ToString();
                opOrderQty.Text = row["orderQty"].ToString();
                opCustomerOrderNO.Text = row["customerOrderNo"].ToString();
                opProductName.Text = row["productName"].ToString();
                opProductCode.Text = row["productCode"].ToString();
                opLotNumber.Text = row["lotNumber"].ToString();
                opGTIN.Text = row["gtin"].ToString();
                opShift.Text = row["shift"].ToString();
                opFactory.Text = row["factory"].ToString();
                opSite.Text = row["site"].ToString();
                opUOM.Text = row["uom"].ToString();
                //ipProductionDate.Value = DateTime.Parse(row["productionDate"].ToString());
            });
        }

        private void LoadCountersAsync()
        {
            Task.Run(() =>
            {
                this.InvokeIfRequired(() =>
                {
                    //SetCountersToLoading();
                });

                Task.Delay(1000).Wait();
                    LoadAndDisplayCounters();
            });
        }

        private void SetCountersToLoading()
        {
            opCZRunCount.Text = "Đang tải";
            opPassCount.Text = "Đang tải";
            opFailCount.Text = "Đang tải";
            opAWSFullOKCount.Text = "Đang tải";
            opAWSNotSent.Text = "Đang tải";
            opAWSSentWating.Text = "Đang tải";
        }

        private void LoadAndDisplayCounters()
        {
            var counters = LoadCountersFromDatabase();
            this.InvokeIfRequired(() =>
            {
                opCZRunCount.Text = counters.runCount.ToString();
                opPassCount.Text = counters.passCount.ToString();
                opFailCount.Text = counters.failCount.ToString();
                opAWSFullOKCount.Text = counters.awsFullOKCount.ToString();
                opAWSNotSent.Text = $"{counters.awsNotSent}/{counters.awsSentFailed}";
                opAWSSentWating.Text = counters.awsSentWaiting.ToString();
            });
        }

        private (int runCount, int passCount, int failCount, int awsFullOKCount, int awsNotSent, int awsSentFailed, int awsSentWaiting) LoadCountersFromDatabase()
        {
            TResult runCountResult = GetRecordCount();
            TResult passCountResult = GetRecordCountByStatus(e_Production_Status.Pass);
            TResult failCountResult = GetRecordCountByStatus(e_Production_Status.Fail);

            TResult notfountCount = GetRecordCountByStatus(e_Production_Status.NotFound);
            TResult readfailCount = GetRecordCountByStatus(e_Production_Status.ReadFail);
            TResult duplicateCount = GetRecordCountByStatus(e_Production_Status.Duplicate);

            int totalfailCount = failCountResult.count +
                notfountCount.count + readfailCount.count + duplicateCount.count;

            TResult awsFullOKResult = GetAWSRecordCount(e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "!=");
            TResult awsNotSentResult = GetAWSRecordCount(e_AWS_Send_Status.Pending, e_AWS_Recive_Status.Pending, "=", "AND Status != 0 AND cartonCode != 'pending' AND cartonCode != '0' ");
            TResult awsSentFailedResult = GetAWSRecordCount(e_AWS_Send_Status.Failed, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0");
            TResult awsSentWaitingResult = GetAWSRecordCount(e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "=");

            return (runCountResult.count, passCountResult.count, totalfailCount,
                    awsFullOKResult.count, awsNotSentResult.count, awsSentFailedResult.count, awsSentWaitingResult.count);
        }

        private TResult GetRecordCount()
        {
            TResult result = Globals.ProductionData.getDataPO.Get_Record_Count(ipOrderNO.Text);
            if (!result.issuccess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lấy thông tin số lượng mã code thất bại: {result.message}");
                //this.ShowErrorDialog($"Lỗi PP07: {result.message}");
            }
            return result;
        }

        private TResult GetRecordCountByStatus(e_Production_Status status)
        {
            TResult result = Globals.ProductionData.getDataPO.Get_Record_Count_By_Status(ipOrderNO.Text, status);
            if (!result.issuccess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lấy thông tin số lượng record {status} thất bại: {result.message}");
                this.ShowErrorDialog($"Lỗi PP09-14: {result.message}");
            }
            return result;
        }

        private TResult GetAWSRecordCount(e_AWS_Send_Status sendStatus, e_AWS_Recive_Status receiveStatus, string condition, string additionalCondition = "")
        {
            TResult result = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(ipOrderNO.Text, sendStatus, receiveStatus,condition,additionalCondition);
            if (!result.issuccess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lấy thông tin AWS record thất bại: {result.message}");
                this.ShowErrorDialog($"Lỗi PP11-14: {result.message}");
            }
            return result;
        }

        public void UpdateAWSCounters()
        {
            var orderText = ipOrderNO.Text;
            Globals.ProductionData.awsSendCounter.pendingCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(orderText, e_AWS_Send_Status.Pending, e_AWS_Recive_Status.Pending, "=", "AND Status != 0").count;
            Globals.ProductionData.awsSendCounter.sentCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(orderText, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0").count;
            Globals.ProductionData.awsSendCounter.failedCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(orderText, e_AWS_Send_Status.Failed, e_AWS_Recive_Status.Waiting, "=", "AND Status != 0").count;
            Globals.ProductionData.awsRecivedCounter.waitingCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(orderText, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "=").count;
            Globals.ProductionData.awsRecivedCounter.recivedCount = Globals.ProductionData.getDataPO.Get_Record_Sent_Recive_Count(orderText, e_AWS_Send_Status.Sent, e_AWS_Recive_Status.Waiting, "!=").count;
        }

        public void CalculateCounters(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                ResetCounters();
                Globals.ProductionData.counter.cartonID = 1;
                return;
            }

            ProcessDataRows(dataTable);
            CalculateCartonID();
        }

        private void ProcessDataRows(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                string status = row["Status"].ToString();
                Globals.ProductionData.counter.totalCount++;

                UpdateCountersByStatus(status);
            }
        }

        private void UpdateCountersByStatus(string status)
        {
            switch (status)
            {
                case var s when s == e_Production_Status.Pass.ToString():
                    Globals.ProductionData.counter.passCount++;
                    break;
                case var s when s == e_Production_Status.ReadFail.ToString():
                    Globals.ProductionData.counter.readfailCount++;
                    break;
                case var s when s == e_Production_Status.NotFound.ToString():
                    Globals.ProductionData.counter.notfoundCount++;
                    break;
                case var s when s == e_Production_Status.Duplicate.ToString():
                    Globals.ProductionData.counter.duplicateCount++;
                    break;
                case var s when s == e_Production_Status.Error.ToString():
                    Globals.ProductionData.counter.errorCount++;
                    break;
            }

            if (status != e_Production_Status.Pass.ToString())
            {
                Globals.ProductionData.counter.failCount++;
            }
        }

        private void CalculateCartonID()
        {
            if (Globals.ProductionData.counter.passCount == 0)
            {
                Globals.ProductionData.counter.cartonID = 1;
            }
            else
            {
                int packingCount = Globals.ProductionData.counter.passCount % AppConfigs.Current.cartonPack;
                Globals.ProductionData.counter.carton_Packing_Count = packingCount;
                
                Globals.ProductionData.counter.cartonID = packingCount == 0
                    ? Globals.ProductionData.counter.passCount / AppConfigs.Current.cartonPack
                    : (Globals.ProductionData.counter.passCount / AppConfigs.Current.cartonPack) + 1;
            }
        }

        private void ExecuteSavingProcess()
        {
            Task.Delay(1000).Wait();
            try
            {
                SaveProductionData();
                TResult recordsResult = LoadProductionRecords();

                if (!recordsResult.issuccess)
                {
                    Globals.Production_State = e_Production_State.NoSelectedPO;
                    return;
                }

                ResetAndCalculateCounters(recordsResult);

                SaveToDatabase();
            }
            catch (Exception ex)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error,
                    $"Lỗi khi lưu thông tin đơn hàng: {ex.Message}");
                this.ShowErrorDialog($"Lỗi PP088: {ex.Message}");
            }

            
        }

        private void SaveProductionData()
        {
            Globals.ProductionData.orderNo = ipOrderNO.SelectedText;
            Globals.ProductionData.productionLine = opProductionLine.Text;
            Globals.ProductionData.uom = opUOM.Text;
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
        }

        private TResult LoadProductionRecords()
        {
            TResult result = Globals.ProductionData.getDataPO.Get_Records(ipOrderNO.Text);
            if (!result.issuccess)
            {
                _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                    $"Lấy dữ liệu đơn hàng thất bại: {result.message}");
                this.ShowErrorDialog($"Lỗi PP05121: {result.message}");
                Globals.Production_State = e_Production_State.NoSelectedPO;
            }
            return result;
        }

        private void ResetAndCalculateCounters(TResult recordsResult)
        {
            ResetCounters();
            CalculateCounters(recordsResult.data);
            UpdateAWSCounters();
        }

        private void SaveToDatabase()
        {
            try
            {
                var productionDateUtc = DateTime.ParseExact(ipProductionDate.Text, "dd-MM-yyyy HH:mm:ss", null);

                var saveResult = Globals.ProductionData.Save_PO(ipOrderNO.SelectedText, 
                    productionDateUtc.ToString("yyyy-MM-dd HH:mm:ss.fff"), Globals.CurrentUser.Username);

                if (saveResult.issucces)
                {
                    HandleSaveSuccess();
                }
                else
                {
                    HandleSaveError(saveResult.message);
                }
            }
            catch (Exception ex)
            {
                HandleSaveException(ex);
            }

            Globals.Production_State = e_Production_State.Ready;
        }

        private void HandleSaveSuccess()
        {
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, 
                $"Người dùng đã lưu đơn hàng: {ipOrderNO.SelectedText}");

            this.InvokeIfRequired(() =>
            {
                this.ShowSuccessTip("Đã lưu thông tin đơn hàng thành công.");
                UpdateStatusMessage($"Đơn hàng {ipOrderNO.SelectedText} đã được lưu thành công.", Color.Green);
                
                btnPO.Enabled = true;
                btnClosePO.Enabled = true;
                btnRUN.Enabled = true;
                btnPO.Text = "Đổi PO";
            });
        }

        private void HandleSaveError(string message)
        {
            _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                $"Lưu đơn hàng thất bại: {message}");
            this.ShowErrorDialog($"Lỗi PP909: {message}");
            Globals.Production_State = e_Production_State.NoSelectedPO;
        }

        private void HandleSaveException(Exception ex)
        {
            _pageLogger.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, 
                $"Lỗi khi lưu đơn hàng: {ex.Message}");
            this.ShowErrorDialog($"Lỗi PP088: {ex.Message}");
        }

        #endregion

        #region UI Helper Methods

        private void UpdateStatusMessage(string message, Color color)
        {
            opTer.Text = message;
            opTer.ForeColor = color;
            opTer.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        }

        private void SetEditMode()
        {
            this.InvokeIfRequired(() =>
            {
                if (ipOrderNO.ReadOnly)
                {
                    ipOrderNO.ReadOnly = false;
                    ipProductionDate.ReadOnly = false;
                    ipOrderNO.FillColor = Color.Yellow;
                    ipProductionDate.FillColor = Color.Yellow;
                    ipOrderNO.ForeColor = Color.Black;
                    ipProductionDate.ForeColor = Color.Black;

                    btnPO.Enabled = true;
                    btnPO.FillColor = Color.FromArgb(0, 192, 0);
                    btnPO.Text = "Lưu PO";
                    btnPO.Symbol = 61639;

                    btnClosePO.Enabled = false;
                    btnRUN.Enabled = false;
                }
            });
        }

        private void ConfigurePreparingMode()
        {
            this.InvokeIfRequired(() =>
            {
                btnRUN.Enabled = false;
                btnRUN.FillColor = Color.Red;
                btnRUN.Text = "Đang chuẩn bị";
                btnRUN.Symbol = 61508;

                btnPO.Enabled = false;
                btnProductionDate.Enabled = false;
                btnClosePO.Enabled = false;
            });
        }

        private void RestoreAfterRunning()
        {
            this.InvokeIfRequired(() =>
            {
                if (Globals.ProductionData.counter.totalCount <= 0)
                {
                    btnPO.Enabled = true;
                    btnPO.FillColor = Color.FromArgb(52, 152, 219);
                    btnPO.Text = "Chọn PO";
                    btnPO.Symbol = 61508;
                    btnClosePO.Enabled = true;
                    btnProductionDate.Enabled = false;
                }
                else
                {
                    btnPO.Enabled = false;
                    btnClosePO.Enabled = false;
                    btnProductionDate.Enabled = true;
                }

                btnRUN.Enabled = true;
                btnRUN.FillColor = Color.FromArgb(0, 192, 0);
                btnRUN.Text = "Bắt đầu sản xuất";
                btnRUN.Symbol = 61515;
            });
        }

        private void ResetCounters()
        {
            Globals.ProductionData.counter.Reset();
            Globals.ProductionData.awsSendCounter.Reset();
            Globals.ProductionData.awsRecivedCounter.Reset();
        }

        #endregion

        private void uiGroupBox1_Click(object sender, EventArgs e)
        {

        }
    }
}