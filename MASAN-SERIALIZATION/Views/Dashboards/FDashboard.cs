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

                    if(Globals.Production_State != e_Production_State.Running)
                    {
                        //nếu không phải trạng thái sản xuất thì không xử lý dữ liệu
                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Máy chưa bắt đầu sản xuất");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        });
                        break;
                    }
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
                OMRON_PLC.Time_Update = 1000; // Thời gian cập nhật PLC 1000ms
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

        #region Các hàm cập nhật giao diện và xử lý chương trinhf
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
            // Cập nhật các giá trị đếm từ PLC
            GetCounterFromPLC();

            this.InvokeIfRequired(() =>
            {
                opQueueRecord.Text = Globals_Database.Insert_Product_To_Record_Queue.Count.ToString();
                opQueueSqlite.Text = Globals_Database.Update_Product_To_SQLite_Queue.Count.ToString();

                opTotal.Value = Globals.CameraMain_PLC_Counter.total; // Tổng số sản phẩm đã sản xuất
                opPass.Value = Globals.CameraMain_PLC_Counter.total_pass; // Số sản phẩm đã kích hoạt
                opFail.Value = Globals.CameraMain_PLC_Counter.total_failed; // Số sản phẩm đã loại bỏ
                opReadFail.Value = Globals.CameraMain_PLC_Counter.camera_read_fail; // Số sản phẩm đã loại bỏ do không đọc được từ camera
            });
        }

        public void GetCounterFromPLC ()
        {
            OperateResult<int[]> readCount = OMRON_PLC.plc.ReadInt32(PLCAddress.Get("PLC_Total_Count_DM_C2"), 5);
            if (readCount.IsSuccess)
            {
                // Cập nhật các giá trị đếm từ PLC
                Globals.CameraMain_PLC_Counter.total = readCount.Content[0]; // Tổng số sản phẩm đã sản xuất
                Globals.CameraMain_PLC_Counter.total_pass = readCount.Content[2]; // Số sản phẩm đã kích hoạt
                Globals.CameraMain_PLC_Counter.camera_read_fail = readCount.Content[1]; // Số sản phẩm đã loại bỏ
                Globals.CameraMain_PLC_Counter.total_failed = readCount.Content[4] + readCount.Content[1]; // Số lượng sản phẩm không đọc được từ camera
            }
            else
            {
                //ghi log lỗi
                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError, "Lỗi DA01068 khi đọc dữ liệu đếm từ PLC", readCount.Message);
                this.ShowErrorNotifier($"Lỗi DA01068 khi đọc dữ liệu đếm từ PLC: {readCount.Message}");
            }
        }
        
        public void Process_Production_State()
        {
            if (Globals.APP_Ready && Globals.Device_Ready){
                OMRON_PLC.Ready = 1;
            }
            else
            {
                OMRON_PLC.Ready = 0;
            }

            if(Globals.Production_State != e_Production_State.Running)
            {
                OperateResult writeStart = OMRON_PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);//gửi lệnh bắt đầu = 0
            }
            else
            {
                OperateResult writeStart = OMRON_PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 1);//gửi lệnh bắt đầu = 1
            }

            switch (Globals.Production_State)
            {
                case e_Production_State.NoSelectedPO:
                    break;
                case e_Production_State.Start:
                    break;
                case e_Production_State.Checking_PO_Info:
                    break;
                case e_Production_State.Loading:
                    break;
                case e_Production_State.Camera_Processing:
                    //Đẩy dữ liệu vào dictionary
                    
                    Task task = Task.Run(() =>
                    {
                        //chuyển trạng thái sang 
                        Globals.Production_State = e_Production_State.Pushing_to_Dic;
                        var getCodes = Globals.ProductionData.getDataPO.Get_Codes(Globals.ProductionData.orderNo);
                        if(getCodes.issucess)
                        {
                            //lấy dữ liệu thành công
                            foreach (DataRow codeRow in getCodes.Codes.Rows)
                            {
                                string code = codeRow["Code"].ToString();
                                string codeID = codeRow["ID"].ToString();
                                string cartonCode = codeRow["cartonCode"].ToString();
                                string status = codeRow["status"].ToString();
                                string activateUser = codeRow["ActivateUser"].ToString();
                                string subCameraDatetime = codeRow["SubCamera_ActivateDate"].ToString();
                                string activateDatetime = codeRow["ActivateDate"].ToString();
                                string productionDate = codeRow["ProductionDate"].ToString();

                                if (!Globals_Database.Dictionary_ProductionCode_Data.ContainsKey(code))
                                {
                                    //nếu chưa có thì thêm vào dictionary
                                    Globals_Database.Dictionary_ProductionCode_Data.Add(code, new ProductionCodeData
                                    {
                                        orderNo = Globals.ProductionData.orderNo,
                                        Code = code,
                                        codeID = codeID.ToInt32(),
                                        cartonCode = cartonCode, //sẽ cập nhật sau khi gửi xuống PLC
                                        Main_Camera_Status = status, //chưa kích hoạt
                                        Activate_User = activateUser, //sẽ cập nhật sau khi kích hoạt
                                        Sub_Camera_Activate_Datetime = subCameraDatetime, //chưa kích hoạt
                                        Activate_Datetime = activateDatetime, //sẽ cập nhật sau khi kích hoạt
                                        Production_Datetime = productionDate, //ngày sản xuất sửa lại khi kích hoạt
                                    });
                                }
                            }
                        }
                        else
                        {
                            //ghi log lỗi
                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DA01069 khi lấy dữ liệu mã sản phẩm", getCodes.message);
                            this.ShowErrorNotifier($"Lỗi DA01069 khi lấy dữ liệu mã sản phẩm: {getCodes.message}");
                            //chuyển trạng thái về error
                            Globals.Production_State = e_Production_State.Error;
                        }

                        //chuyển sang runing
                        Globals.Production_State = e_Production_State.Running;
                    });

                    break;
                case e_Production_State.Pushing_new_PO_to_PLC:
                    //gửi dữ liệu mới xuống PLC
                    //xóa số đếm PLC
                    OperateResult writeClear = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Reset_Counter_DM_C2"), 1);
                    //xóa số đếm SS
                    OperateResult writeClear1 = OMRON_PLC.plc.Write(PLCAddress.Get("RESET_COUNT_DM_SS1"), 1);

                    OperateResult writeOrderQty = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_ORDERQTY_DM"), Globals.ProductionData.orderQty.ToInt32());
                    //chuyển sang Camera
                    Globals.Production_State = e_Production_State.Camera_Processing;
                    break;
                case e_Production_State.Pushing_continue_PO_to_PLC:

                    //gửi số lượng order xuống
                    OperateResult writeOrderQty1 = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_ORDERQTY_DM"), Globals.ProductionData.orderQty.ToInt32());
                    OperateResult writeStart3 = OMRON_PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 1);//gửi lệnh bắt đầu

                    Task task2 = Task.Run(() =>
                    {
                        //chuyển trạng thái sang 
                        Globals.Production_State = e_Production_State.Pushing_to_Dic;

                        var getCodes = Globals.ProductionData.getDataPO.Get_Codes(Globals.ProductionData.orderNo);
                        if (getCodes.issucess)
                        {
                            //lấy dữ liệu thành công
                            foreach (DataRow codeRow in getCodes.Codes.Rows)
                            {
                                string code = codeRow["Code"].ToString();
                                string codeID = codeRow["ID"].ToString();
                                string cartonCode = codeRow["cartonCode"].ToString();
                                string status = codeRow["status"].ToString();
                                string activateUser = codeRow["ActivateUser"].ToString();
                                string subCameraDatetime = codeRow["SubCamera_ActivateDate"].ToString();
                                string activateDatetime = codeRow["ActivateDate"].ToString();
                                string productionDate = codeRow["ProductionDate"].ToString();

                                if (!Globals_Database.Dictionary_ProductionCode_Data.ContainsKey(code))
                                {
                                    //nếu chưa có thì thêm vào dictionary
                                    Globals_Database.Dictionary_ProductionCode_Data.Add(code, new ProductionCodeData
                                    {
                                        orderNo = Globals.ProductionData.orderNo,
                                        Code = code,
                                        codeID = codeID.ToInt32(),
                                        cartonCode = cartonCode, //sẽ cập nhật sau khi gửi xuống PLC
                                        Main_Camera_Status = status, //chưa kích hoạt
                                        Activate_User = activateUser, //sẽ cập nhật sau khi kích hoạt
                                        Sub_Camera_Activate_Datetime = subCameraDatetime, //chưa kích hoạt
                                        Activate_Datetime = activateDatetime, //sẽ cập nhật sau khi kích hoạt
                                        Production_Datetime = productionDate, //ngày sản xuất sửa lại khi kích hoạt
                                    });
                                }
                            }
                        }
                        else
                        {
                            //ghi log lỗi
                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DA01069 khi lấy dữ liệu mã sản phẩm", getCodes.message);
                            this.ShowErrorNotifier($"Lỗi DA01069 khi lấy dữ liệu mã sản phẩm: {getCodes.message}");
                            //chuyển trạng thái về error
                            Globals.Production_State = e_Production_State.Error;
                        }

                        //chuyển sang runing
                        Globals.Production_State = e_Production_State.Running;
                    });

                    //gửi dữ liệu tiếp tục xuống PLC
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
                case e_Production_State.Saving:
                    break;
                case e_Production_State.Error:
                    break;
            }
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
                    Process_Production_State(); // Xử lý trạng thái sản xuất

                    //xử lý thông tin chính 
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
                        //// Lấy dữ liệu từ hàng đợi
                        //var item = Globals_Database.Update_Product_To_SQLite_Queue.Dequeue();

                        //string code = item.conten;
                        //ProductionCodeData productionCodeData = item.data;
                        //bool isDuplicate = item.duplicate;

                        ////Globals.ProductionData.setDB.Update_Active_Status(productionCodeData);

                    }
                    if (Globals_Database.Insert_Product_To_Record_Queue.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        //var recordItem = Globals_Database.Insert_Product_To_Record_Queue.Dequeue();
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
