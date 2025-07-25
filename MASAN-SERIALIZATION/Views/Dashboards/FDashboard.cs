using HslCommunication;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using SpT.Auth;
using SpT.Communications.TCP;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MASAN_SERIALIZATION.Utils.ExtensionMethods;
using static SPMS1.OmronPLC_Hsl;

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public partial class FDashboard : UIPage
    {

        private static LogHelper<e_Dash_LogType> DashboardPageLog;

        public FDashboard()
        {
            InitializeComponent();
            //tạo log ở Appdata/Local/MasanSerialization/Logs/Dashboard
            //khởi tạo biến toàn cục để ghi nhật ký cho giao diện này
            DashboardPageLog = new LogHelper<e_Dash_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "Pages", "PDAlog.ptl"));
        }

        #region Sự kiện khởi tạo giao diện

        public void STARTUP()
        {
            InitializeDevices();
            InitializeTasks();
            Start_Queue_Process();
        }
        #endregion


        #region Sự kiện camera trả về của thiết bị
        //fix lỗi dội dữ liệu từ camera chính
        long lastReciveCM = 0;
        string lastDataCM = "";
        private void CameraMain_ClientCallBack(enumClient state, string data)
        {
            switch (state)
            {
                case enumClient.CONNECTED:

                    if (Globals.CameraMain_State != e_Camera_State.CONNECTED)
                    {
                        Globals.CameraMain_State = e_Camera_State.CONNECTED;
                    }
                    break;
                case enumClient.DISCONNECTED:

                    if (Globals.CameraMain_State != e_Camera_State.DISCONNECTED)
                    {
                        Globals.CameraMain_State = e_Camera_State.DISCONNECTED;
                    }
                    break;
                case enumClient.RECEIVED:
                    Task.Run(() =>
                    {
                        CameraMain_Process(data);
                    });
                    
                    break;
                case enumClient.RECONNECT:

                    if (Globals.CameraMain_State != e_Camera_State.RECONNECTING)
                    {
                        Globals.CameraMain_State = e_Camera_State.RECONNECTING;
                    }
                    break;
            }
        }

        //fix lỗi dội dữ liệu từ camera phụ
        long lastReciveCS = 0;
        string lastDataCS = "";

        private void CameraSub_ClientCallBack(enumClient state, string data)
        {
            switch (state)
            {
                case enumClient.CONNECTED:
                    if (Globals.CameraSub_State != e_Camera_State.CONNECTED)
                    {
                        Globals.CameraSub_State = e_Camera_State.CONNECTED;
                    }
                    break;
                case enumClient.DISCONNECTED:
                    if (Globals.CameraSub_State != e_Camera_State.DISCONNECTED)
                    {
                        Globals.CameraSub_State = e_Camera_State.DISCONNECTED;
                    }
                    break;
                case enumClient.RECEIVED:
                    //fix double call
                    var a = DateTime.Now.Ticks - lastReciveCS;
                    var b = TimeSpan.TicksPerMillisecond * 100;
                    //tránh gửi trùng liên tiếp 2 lần thời gian hiện tại - lastRecive < 100ms nếu code mới nhận về giống với lần trước
                    if (a < b)
                    {
                        //nếu dữ liệu nhận về giống với lần trước thì không xử lý
                        if (data == lastDataCS)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Chống dội Camerta sau");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            });
                            break;
                        }
                    }
                    CameraSub_Process(data);
                    break;
                case enumClient.RECONNECT:
                    if (Globals.CameraSub_State != e_Camera_State.RECONNECTING)
                    {
                        Globals.CameraSub_State = e_Camera_State.RECONNECTING;
                    }
                    break;
            }
        }

        private void OMRON_PLC_PLCStatus_OnChange(object sender, PLCStatusEventArgs e)
        {
            switch (e.Status)
            {
                case PLCStatus.Connecting:
                    if (!Globals.PLC_Connected)
                    {
                        Globals.PLC_Connected = true;
                    }
                    break;
                case PLCStatus.Disconnect:
                    if (Globals.PLC_Connected)
                    {
                        Globals.PLC_Connected = false;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Xử lý dữ liệu camera
        private void CameraMain_Process(string _data)
        {
            Globals.ProductionData.counter.totalCount++; //tăng tổng số sản phẩm đã nhận
            this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: #{Globals.ProductionData.counter.totalCount} Camera chính nhận dữ liệu: {_data}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });

            //xử lý dữ liệu từ camera chính
            if (_data.IsNullOrEmpty())
            {
                //loại sản phẩm ngay lập tức
                bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"),"0"); // Gửi dữ liệu loại sản phẩm đến PLC
                Send_Result_Content_CMain(e_Production_Status.Error, _data);
                //gửi vào hàng chờ thêm record
                Enqueue_Product_To_Record(_data, e_Production_Status.Error, stp, DateTime.UtcNow.ToString("o"), Globals.ProductionData.productionDate);
                return;
            }
            if (_data == "FAIL")
            {
                //loại sản phẩm ngay lập tức
                bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "0"); // Gửi dữ liệu loại sản phẩm đến PLC
                Send_Result_Content_CMain(e_Production_Status.ReadFail, _data);
                //gửi vào hàng chờ thêm record
                Enqueue_Product_To_Record(_data, e_Production_Status.ReadFail, stp, DateTime.UtcNow.ToString("o"), Globals.ProductionData.productionDate);
                return;
            }
            //đổi <GS> về đúng ký tự thật
            _data = _data.Replace("<GS>", "\u001D").Replace("<RS>", "\u001E").Replace("<US>", "\u001F");

            //kiểm tra mã có tồn tại hay không
            if (Globals_Database.Dictionary_ProductionCode_Data.TryGetValue(_data, out ProductionCodeData _produtionCodeData))
            {
                //nếu chưa kích hoạt thì kích hoạt
                if (_produtionCodeData.Main_Camera_Status == "0")
                {

                    // Gửi dữ liệu đến PLC
                    if (Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "1"))
                    {
                        // kích hoạt mã sản phẩm
                        _produtionCodeData.Main_Camera_Status = "1"; // Đánh dấu là đã kích hoạt
                        _produtionCodeData.Activate_Datetime = DateTime.UtcNow.ToString("o");
                        _produtionCodeData.Production_Datetime = Globals.ProductionData.productionDate;
                        _produtionCodeData.cartonCode = "pending"; // Lưu mã sản phẩm vào thùng
                        _produtionCodeData.SubCamera_Datetime = "pending"; // Đặt trạng thái chờ cho camera phụ
                        _produtionCodeData.Sub_Camera_Status = "0"; // Đặt trạng thái camera phụ là chưa kích hoạt
                                                                    // Cập nhật dữ liệu vào từ điển
                       //Globals_Database.Dictionary_ProductionCode_Data[_data] = _produtionCodeData;
                        // Gửi kết quả xử lý từ camera chính
                        Send_Result_Content_CMain(e_Production_Status.Pass, _data);

                        //gửi vào hàng chờ thêm record
                        Enqueue_Product_To_Record(_data, e_Production_Status.Pass, true, _produtionCodeData.Activate_Datetime, _produtionCodeData.Production_Datetime);
                        //thêm vào hàng chờ lưu sqlite
                        Enqueue_Product_To_SQLite(_data, _produtionCodeData);


                    }
                    else
                    {
                        // kích hoạt mã sản phẩm
                        _produtionCodeData.Main_Camera_Status = "-1"; // Đánh dấu là đã kích hoạt
                        _produtionCodeData.Activate_Datetime = DateTime.UtcNow.ToString("o");
                        _produtionCodeData.Production_Datetime = Globals.ProductionData.productionDate;
                        _produtionCodeData.cartonCode = "pending"; // Lưu mã sản phẩm vào thùng
                        _produtionCodeData.SubCamera_Datetime = "pending"; // Đặt trạng thái chờ cho camera phụ
                        _produtionCodeData.Sub_Camera_Status = "0"; // Đặt trạng thái camera phụ là chưa kích hoạt
                         // Cập nhật dữ liệu vào từ điển
                        Globals_Database.Dictionary_ProductionCode_Data[_data] = _produtionCodeData;
                        // Gửi kết quả lỗi nếu không gửi được đến PLC
                        Send_Result_Content_CMain(e_Production_Status.Error, _data);

                        //gửi vào hàng chờ thêm record
                        Enqueue_Product_To_Record(_data, e_Production_Status.Error, false, _produtionCodeData.Activate_Datetime, _produtionCodeData.Production_Datetime);
                        //thêm vào hàng chờ lưu sqlite
                        Enqueue_Product_To_SQLite(_data, _produtionCodeData);
                    }
                    return;
                }
                //nếu đã kích hoạt
                else
                {
                    //send dữ liệu đến PLC để loại sản phẩm
                    Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "0"); // Gửi dữ liệu loại sản phẩm đến PLC
                    //thêm vào thùng, cập nhật lại mã thùng
                    Send_Result_Content_CMain(e_Production_Status.Duplicate, _data);

                    //gửi vào hàng chờ thêm record
                    Enqueue_Product_To_Record(_data, e_Production_Status.Duplicate, true, _produtionCodeData.Activate_Datetime, _produtionCodeData.Production_Datetime);

                    //thêm vào hàng chờ lưu sqlite cập nhật trùng
                    Enqueue_Product_To_SQLite(_data, _produtionCodeData, true);

                    return;
                }
            }
            //nếu không tồn tại thì đá ra, không cần quan tâm thêm
            else
            {
                //loại sản phẩm ngay lập tức
                Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "0"); // Gửi dữ liệu loại sản phẩm đến PLC
                Send_Result_Content_CMain(e_Production_Status.NotFound, _data);

                //gửi vào hàng chờ thêm record
                Enqueue_Product_To_Record(_data, e_Production_Status.NotFound, true, DateTime.UtcNow.ToString("o"), Globals.ProductionData.productionDate);

                return;
            }
        }

        private void CameraSub_Process(string _data)
        {
            // Xử lý dữ liệu từ camera phụ
            // Ví dụ: Cập nhật giao diện, lưu trữ dữ liệu, v.v.
        }

        private void Send_Result_Content_CMain(e_Production_Status status, string data)
        {
            //tăng tổng số sản phẩm đã nhận
            //Globals.ProductionData.counter.totalCount++;
            if (status != e_Production_Status.Pass)
            {
               Globals.ProductionData.counter.failCount++; //tăng tổng số sản phẩm lỗi
            }
            switch (status)
            {
                case e_Production_Status.Pass:
                    //tăng tổng pass
                    Globals.ProductionData.counter.passCount++;
                    break;
                case e_Production_Status.Fail:
                    //tăng tổng fail
                    break;
                case e_Production_Status.Duplicate:
                    //tăng tổng duplicate
                    Globals.ProductionData.counter.duplicateCount++;
                    break;
                case e_Production_Status.NotFound:
                    //tăng tổng not found
                    Globals.ProductionData.counter.notfoundCount++;
                    break;
                case e_Production_Status.Error:
                    //tăng tổng error
                    Globals.ProductionData.counter.errorCount++;
                    break;
                case e_Production_Status.ReadFail:
                    //tăng tổng read fail
                    Globals.ProductionData.counter.readfailCount++;
                    break;
            }
        }

        private void Enqueue_Product_To_Record(string code, e_Production_Status status, bool plcstatus, string activate_datetime, string production_datetime)
        {
            //tạo dữ liệu record
            ProductionCodeData_Record _produtionCodeData = new ProductionCodeData_Record
            {
                code = code,
                status = status,
                PLCStatus = plcstatus,
                Activate_Datetime = activate_datetime,
                Production_Datetime = production_datetime
            };
            //thêm vào hàng chờ thêm record
            Globals_Database.Insert_Product_To_Record_Queue.Enqueue(_produtionCodeData);
        }

        private void Enqueue_Product_To_SQLite(string code, ProductionCodeData productionCodeData, bool duplicate = false)
        {
            //thêm vào hàng chờ lưu sqlite
            Globals_Database.Update_Product_To_SQLite_Queue.Enqueue((code, productionCodeData, duplicate));
        }
        #endregion

        #region Các hàm gửi PLC

        public bool Send_To_PLC(string DM, string _data)
        {
            try
            {
                OperateResult write = OMRON_PLC.plc.Write(DM, int.Parse(_data));
                if (write.IsSuccess)
                {
                    return true;
                }
                else
                {
                    //ghi log lỗi
                    DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError, "Lỗi D022 khi gửi dữ liệu đến PLC", write.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {   
                
                //ghi log lỗi
                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username,e_Dash_LogType.PlcError, "Lỗi D023 khi gửi dữ liệu đến PLC",ExceptionToJson(ex));
                return false;
                
            }
        }

        #endregion

        #region Các hàm khởi tạo

        public void InitializeDevices()
        {
            try
            {
                // Khởi tạo camera chính
                Camera_Main.IP = AppConfigs.Current.Camera_Main_IP;
                Camera_Main.Port = AppConfigs.Current.Camera_Main_Port;
                Camera_Main.Connect();
                // Khởi tạo camera phụ
                Camera_Sub.IP = AppConfigs.Current.Camera_Sub_IP;
                Camera_Sub.Port = AppConfigs.Current.Camera_Sub_Port;
                Camera_Sub.Connect();

                // PLC
                OMRON_PLC.PLC_IP = PLCAddress.Get("PLC_IP");
                OMRON_PLC.PLC_PORT = PLCAddress.Get("PLC_PORT").ToInt32();
                OMRON_PLC.PLC_Ready_DM = PLCAddress.Get("PLC_Ready_DM");
                OMRON_PLC.InitPLC();

            }
            catch (Exception ex)
            {
               this.ShowErrorNotifier("Lỗi D001 khi khởi tạo thiết bị: " + ex.Message);
            }
        }

        //khởi tạo task
        public void InitializeTasks()
        {
            try
            {
                WK_Update_UI.RunWorkerAsync(); // Bắt đầu task cập nhật giao diện
                WK_Update_UI.DoWork += WK_Update_UI_DoWork;

            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier("Lỗi D002 khi khởi tạo task: " + ex.Message);
            }
        }

        #endregion

        #region Các hàm cập nhật giao diện
        public void UpdateDeviceState()
        {
            UpdateCameraMainState();
            UpdateCameraSubState();
            UpdatePLCState();
        }

        public void UpdateCameraMainState()
        {
            this.InvokeIfRequired(() =>
            {
                if (Globals.CameraMain_State == e_Camera_State.CONNECTED)
                {
                    if (opCameraMain.Value != "Đã kết nối")
                    {
                        opCameraMain.IsBlinking = false;
                        opCameraMain.Value = "Đã kết nối";
                        opCameraMain.ONcolor = Color.Green;
                        opCameraMain.IsOn = true; // Đặt trạng thái ON cho opCameraMain
                    }

                }
                else if (Globals.CameraMain_State == e_Camera_State.DISCONNECTED)
                {
                    if (opCameraMain.Value != "Mất kết nối")
                    {
                        opCameraMain.IsBlinking = true;
                        opCameraMain.Value = "Mất kết nối";
                        opCameraMain.ONcolor = Color.Red;
                    }
                }
                else if (Globals.CameraMain_State == e_Camera_State.RECONNECTING)
                {
                    if (opCameraMain.Value != "Kết nối lại")
                    {
                        opCameraMain.IsBlinking = true;
                        opCameraMain.Value = "Kết nối lại";
                        opCameraMain.ONcolor = Color.Orange;
                    }
                }
            });
        }
        public void UpdateCameraSubState()
        {
            this.InvokeIfRequired(() =>
            {
                if (Globals.CameraSub_State == e_Camera_State.CONNECTED)
                {
                    if (opCameraSub.Value != "Đã kết nối")
                    {
                        opCameraSub.IsBlinking = false;
                        opCameraSub.Value = "Đã kết nối";
                        opCameraSub.ONcolor = Color.Green;
                        opCameraSub.IsOn = true; // Đặt trạng thái ON cho opCameraSub
                    }
                }
                else if (Globals.CameraSub_State == e_Camera_State.DISCONNECTED)
                {
                    if (opCameraSub.Value != "Mất kết nối")
                    {
                        opCameraSub.IsBlinking = true;
                        opCameraSub.Value = "Mất kết nối";
                        opCameraSub.ONcolor = Color.Red;
                    }
                }
                else if (Globals.CameraSub_State == e_Camera_State.RECONNECTING)
                {
                    if (opCameraSub.Value != "Kết nối lại")
                    {
                        opCameraSub.IsBlinking = true;
                        opCameraSub.Value = "Kết nối lại";
                        opCameraSub.ONcolor = Color.Orange;
                    }
                }
            });
        }
        public void UpdatePLCState() {
            this.InvokeIfRequired(() => {
                if (Globals.PLC_Connected)
                {
                    if (opPLCState.IsBlinking)
                    {
                        opPLCState.IsBlinking = false;
                        opPLCState.Value = "Đã kết nối";
                        opPLCState.ONcolor = Color.Green;
                        opPLCState.IsOn = true; // Đặt trạng thái ON cho opPLCState
                    }
                }
                else
                {
                    if (!opPLCState.IsBlinking)
                    {
                        opPLCState.IsBlinking = true;
                        opPLCState.Value = "Mất kết nối";
                        opPLCState.ONcolor = Color.Red;
                    }
                }
            });
        }

        public void UpdateCounterUI()
        {
            this.InvokeIfRequired(() =>
            {
                opQueueRecord.Text = Globals_Database.Insert_Product_To_Record_Queue.Count.ToString();
                opQueueSqlite.Text = Globals_Database.Update_Product_To_SQLite_Queue.Count.ToString();
            });
        }
        

        #endregion

        #region Các enum và class hỗ trợ

        private enum e_Dash_LogType
        {
            Info,
            Warning,
            Error,
            PlcError,
            CameraError,
        }

        #endregion

        #region Các luồng
        private void WK_Update_UI_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Update_UI.CancellationPending)
            {
                try
                {
                    UpdateDeviceState();
                    UpdateCounterUI(); // Cập nhật giao diện counter
                }
                catch (Exception ex)
                {
                    Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi trong WK_Update_UI_DoWork: {ex.Message}");
                    this.ShowErrorNotifier($"Lỗi DA0107 trong quá trình xử lý: {ex.Message}");
                }
                Thread.Sleep(100);
            }
           
        }

        public static bool offThread = false;
        Thread threadQueue;
        public void Start_Queue_Process()
        {
            if (threadQueue == null || !threadQueue.IsAlive)
            {

                threadQueue = new Thread(Queue_Proccess);
                threadQueue.Start();
                threadQueue.IsBackground = true; // Đặt luồng là nền để nó không chặn ứng dụng thoát
            }
            else
            {
                //không thể khởi động lại nhiệm vụ nếu nó đã đang chạy
            }
        }

        #endregion

        #region Các hàm xử lý Queue

        // Biến toàn cục để kiểm soát việc tắt luồng

        public static void Queue_Proccess()
        {
            while(!offThread)
            {
                try
                {
                    // Kiểm tra nếu hàng đợi có dữ liệu
                    if (Globals_Database.Update_Product_To_SQLite_Queue.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        var item = Globals_Database.Update_Product_To_SQLite_Queue.Dequeue();
                        
                    }
                    if (Globals_Database.Insert_Product_To_Record_Queue.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        var recordItem = Globals_Database.Insert_Product_To_Record_Queue.Dequeue();
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu có
                    DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DT004 trong quá trình xử lý hàng đợi", ExceptionToJson(ex));
                }
                Thread.Sleep(100); //đợi 0.1 giây để đảm bảo các thiết bị đã sẵn sàng
            }
            
        }
        
        #endregion

        #region Test
        private void oporderNO_Click(object sender, EventArgs e)
        {
            offThread = !offThread; // Chuyển đổi trạng thái offThread
        }

        #endregion

        #region Các hàm xử lý Sqlite
        
        #endregion
    }
}
