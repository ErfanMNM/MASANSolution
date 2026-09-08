using HslCommunication;
using HslCommunication.Profinet.Inovance;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Helpers;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using SpT.Communications.TCP;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MASAN_SERIALIZATION.Utils.ExtensionMethods;
using static SpT.OmronPLC_Hsl;

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public partial class FDashboard : UIPage
    {
        private bool IsTestModeEnabled => AppConfigs.Current.APP_TEST_MODE2;
        int lantest = 1;
        int maxCtest = 1;
        int k = 1;
        private ProductionCodeData GetOrCreateTestModeCodeData(string code)
        {
            if (Globals_Database.Dictionary_ProductionCode_Data.TryGetValue(code, out ProductionCodeData existing))
            {
                return existing;
            }

            ProductionCodeData virtualCodeData = new ProductionCodeData
            {
                orderNo = Globals.ProductionData.orderNo,
                Code = code,
                codeID = 0,
                cartonCode = "pending",
                Activate_User = Globals.CurrentUser.Username,
                Camera_Status = "0",
                Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"),
                Production_Datetime = Globals.ProductionData.productionDate
            };

            Globals_Database.Dictionary_ProductionCode_Data[code] = virtualCodeData;

            return virtualCodeData;
        }

        #region Private Fields
        private static LogHelper<e_Dash_LogType> DashboardPageLog;
        private static bool offThread = false;
        private Thread threadQueue;


        //các biến đo thời gian xử lý sản phẩm
        Stopwatch sw = new Stopwatch();
        double currentCameraSubProcessingTime = 0;
        double maxCameraSubProcessingTime = 0;

        #endregion

        #region Constructor
        public FDashboard()
        {
            InitializeComponent();
            DashboardPageLog = new LogHelper<e_Dash_LogType>(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MASAN-SERIALIZATION", "Logs", "Pages", "PDAlog.ptl"));
            // Expose PLC instances to Globals for use by Helpers
            try
            {
                Globals.PLC_Instance = OMRON_PLC;
                Globals.PLC_Instance_02 = OMRON_PLC_02;
            }
            catch (Exception ex)
            {
                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi khởi tạo PLC: {ex.Message}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });
            }
            
        }
        #endregion

        #region Startup Methods
        public void STARTUP()
        {
            InitializeDevices();
            InitializeTasks();
            Start_Queue_Process();
        }
        #endregion

        #region  Device Event Handlers
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


                    if (Globals.Production_State == e_Production_State.Running || Globals.Production_State == e_Production_State.Waiting_Stop || Globals.Production_State == e_Production_State.Check_After_Completed )
                    {

                        if(subpr.IsBusy)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Quá tải á");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            });

                            break;
                        }
                        else
                        {
                            subpr.RunWorkerAsync(data);
                        }
                    }
                    else
                    {
                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CM Máy chưa bắt đầu sản xuất");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        });
                        break;
                    }
                    
                    break;

                case enumClient.RECONNECT:
                    if (Globals.CameraMain_State != e_Camera_State.RECONNECTING)
                    {
                        Globals.CameraMain_State = e_Camera_State.RECONNECTING;
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

        private void OMRON_PLC_2_PLCStatus_OnChange(object sender, PLCStatusEventArgs e)
        {
            switch (e.Status)
            {
                case PLCStatus.Connecting:
                    if (!Globals.PLC_Connected_02)
                    {
                        Globals.PLC_Connected_02 = true;
                    }
                    break;
                case PLCStatus.Disconnect:
                    this.InvokeIfRequired(() =>
                    {
                        ipConsole.Items.Insert(0, "Chế độ 2 PLC");
                    });
                    if (Globals.PLC_Connected_02)
                    {
                        Globals.PLC_Connected_02 = false;
                    } 
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Camera Data Processing


        //Xử lý dữ liệu camera phụ, tương tự camera chính nhưng có thêm logic về thùng carton
        private void Camera_Process(string rawCode)
        {

            //A1. Camera Nhận dữ liệu, tăng tổng số đếm
            Globals.ProductionData.counter.totalCount++;
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: #{Globals.ProductionData.counter.totalCount} Camera chính nhận dữ liệu: {rawCode}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            //K1 Kiểm tra đảm bảo đang không ở trạng thái khác chen vào
            if (Globals.Production_State != e_Production_State.Waiting_Stop)
            {
                if (Globals.Production_State != e_Production_State.Running)
                {
                    this.InvokeIfRequired(() =>
                    {
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Camera: Sản phẩm loại do lỗi dồn");
                        ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                    });

                    bool stp = false;
                    sw.Stop();
                    currentCameraSubProcessingTime = sw.Elapsed.TotalMilliseconds;
                    if (AppConfigs.Current.PLC_Duo_Mode)
                    {
                        stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                    }
                    else
                    {
                        stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                    }

                    Send_Result_Content(e_Production_Status.Error, rawCode);
                    Enqueue_Product_To_Record(rawCode, e_Production_Status.Error, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                    return;
                }
            }

            //A2. Kiểm tra code không null hoặc rỗng
            if (rawCode.IsNullOrEmpty())
            {
                bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                Send_Result_Content(e_Production_Status.Error, rawCode);
                Enqueue_Product_To_Record(rawCode, e_Production_Status.Error, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                return;
            }


            //a3 kiểm tra đúng cấu trúc hay không

            //rawCode = Code|Result

            string[] arawCode = rawCode.Split('|');

            //Sai cấu trúc
            if (arawCode.Length != 2)
            {
                bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                Send_Result_Content(e_Production_Status.Error, rawCode);
                Enqueue_Product_To_Record(rawCode, e_Production_Status.Error, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                return;
            }

            //a4 Không đọc được
            if (arawCode[1].ToString() == "NOREAD")
            {

                if (AppConfigs.Current.TestLai)
                {
                   // var a = Globals.TestComand.Split(";");

                    //var b = a[lantest-1].ToInt32();
                    //if (maxCtest <= b)
                    //{
                        //if (lantest % 2 != 0)
                        //{
                            //Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "2");
                        //}
                        //else
                        //{
                            //Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                        //}
                   // }
                    //else
                   // {
                       // maxCtest = b;
                       // if (lantest % 2 != 0)
                       // {
                            //Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "2");
                       // }
                       // else
                       // {
                            //Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                       // }
                   // }
                   // lantest++;
                   // if (lantest>4)
                   // {
                   //     lantest = 1;
                  //  }
                   // maxCtest++;

                    if(k%2!=0)
                    {
                        Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "1");
                    }
                    else
                    {
                        Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "2");
                    }
                    k++;
                    return;
                }
                else
                {


                    bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                    Send_Result_Content(e_Production_Status.ReadFail, rawCode);
                    Enqueue_Product_To_Record(rawCode, e_Production_Status.ReadFail, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                    
                }
                return;
            }

            //a5 Lỗi
            if (arawCode[1].ToString() == "REJECT")
            {
                bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                Send_Result_Content(e_Production_Status.Error, rawCode);
                Enqueue_Product_To_Record(rawCode, e_Production_Status.Error, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                return;
            }

            //a6 Các lỗi khác
            if (arawCode[1].ToString() != "OK")
            {
                bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                Send_Result_Content(e_Production_Status.Error, rawCode);
                Enqueue_Product_To_Record(rawCode, e_Production_Status.Error, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                return;
            }


            //B1 chuyển 
            string code = arawCode[0];
            //a4 Chuyển sang chuẩn đúng, giữ nguyên ký tự
            code = code.Replace("<GS>", "\u001D").Replace("<RS>", "\u001E").Replace("<US>", "\u001F");
            code = code.Replace("(01)", "01").Replace("(21)", "21").Replace("(93)", "\u001D93");

            if (IsTestModeEnabled && !Globals_Database.Dictionary_ProductionCode_Data.ContainsKey(code))
            {
                GetOrCreateTestModeCodeData(code);
            }

            //kiểm tra mã có tồn tại hay không trong Dictionary chính (CameraMain)
            if (Globals_Database.Dictionary_ProductionCode_Data.TryGetValue(code, out ProductionCodeData _produtionCodeData))
            {
                    //D1. Trùng
                    if (_produtionCodeData.Camera_Status != "0")
                    {
                        // Đã được scan => Duplicate
                        bool stp = false;
                        sw.Stop();
                        currentCameraSubProcessingTime = sw.Elapsed.TotalMilliseconds;
                        if (AppConfigs.Current.PLC_Duo_Mode)
                        {
                            stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                        }
                        else
                        {
                            stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                        }
                        
                        Send_Result_Content(e_Production_Status.Duplicate,"D1:  " +code);
                        Enqueue_Product_To_Record(code, e_Production_Status.Duplicate, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                        return;
                    }
                
                //Xây dữ liệu thùng
                ProductionCartonData cartonData = new ProductionCartonData();
                int cache_CartonID = Globals.ProductionData.counter.cartonID;
                int cache_CartonCount = Globals.ProductionData.counter.carton_Packing_Count;

                //kiểm tra xem thùng đang đóng đã kích hoạt chưa, nếu chưa dừng băng tải
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData _produtionCartonData1))
                {
                    if (_produtionCartonData1.cartonCode == "0")
                    {
                        //nếu thùng hiện tại đã có mã thì không cần xử lý tiếp
                        bool stp = false; // Gửi dữ liệu loại sản phẩm đến PLC
                        sw.Stop();
                        currentCameraSubProcessingTime = sw.Elapsed.TotalMilliseconds;
                        if (AppConfigs.Current.PLC_Duo_Mode)
                        {
                            stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                        }
                        else
                        {
                            stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                        }

                        

                        //Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0"); // Gửi dữ liệu loại sản phẩm đến PLC
                        Enqueue_Product_To_Record(code, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                        Send_Result_Content(e_Production_Status.Error, code);

                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: AAA Mã thùng hiện tại chưa có, không xử lý tiếp.");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            //this.ShowErrorNotifier("PC 012 Chưa có mã thùng, không xử lý tiếp.", false, 10000);
                            Globals.Canhbao = "#Thùng đang chạy chưa có mã";
                        });
                        //Quăng về ready
                        Globals.Production_State = e_Production_State.Pause;
                        return;
                    }
                }

                //kiểm tra xem có quá số lượng trong thùng không
                if (Globals.ProductionData.counter.carton_Packing_Count > AppConfigs.Current.cartonPack)
                {
                    //kiểm tra thùng mới có mã chưa, nếu chưa có thì dừng line
                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out cartonData))
                    {
                        if (cartonData.cartonCode == "0")
                        {
                            //chưa được quét mã bắt đầu => dừng line, 
                            //Quăng về pause
                            Globals.Production_State = e_Production_State.Pause;
                            this.InvokeIfRequired(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Chưa quét mã thùng, không xử lý tiếp.");
                                Globals.Canhbao = "#Thùng đang xếp chưa có mã";
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                //this.ShowErrorNotifier("#02 Thùng chuẩn bị xếp chưa có mã!");
                            });
                            return; //nếu thùng chưa có mã thì không xử lý tiếp
                        }
                    }

                    //kích hoạt thùng mới
                    Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(cache_CartonID, out ProductionCartonData cartonDataz);
                    Globals_Database.Activate_Carton.Enqueue(cartonDataz.cartonCode);
                    //nâng ID thùng lên 1, và tạo thùng mới
                    Globals.ProductionData.counter.cartonID = cache_CartonID + 1; //cập nhật ID thùng
                    Globals.ProductionData.counter.carton_Packing_Count = 0; //đặt lại số lượng chai trong thùng
                    cache_CartonID = Globals.ProductionData.counter.cartonID;
                    cache_CartonCount = Globals.ProductionData.counter.carton_Packing_Count;
                }
                // Snapshot ID/Status hiện tại của CameraSub PLC trước khi gửi phân làn
                int cameraSubPreviousId = 0;
                int cameraSubPreviousStatus = 0;
                if (AppConfigs.Current.CameraSub_Timeout_Enabled && AppConfigs.Current.CameraSub_Timeout_Mode_2)
                {
                    Func<string, ushort, OperateResult<int[]>> readInt32BeforeSend = (address, length) =>
                    {
                        if (AppConfigs.Current.PLC_Duo_Mode)
                        {
                            return OMRON_PLC_02.plc.ReadInt32(address, length);
                        }

                        return OMRON_PLC.plc.ReadInt32(address, length);
                    };

                    string currentIdAddressBeforeSend = PLCAddress.Get("PLC_CurrentID_DM_C1");
                    string currentStatusAddressBeforeSend = PLCAddress.Get("PLC_CurrentStatus_DM_C1");
                    OperateResult<int[]> readCurrentIdBeforeSend = readInt32BeforeSend(currentIdAddressBeforeSend, 1);
                    OperateResult<int[]> readCurrentStatusBeforeSend = readInt32BeforeSend(currentStatusAddressBeforeSend, 1);

                    if (!readCurrentIdBeforeSend.IsSuccess || !readCurrentStatusBeforeSend.IsSuccess ||
                        readCurrentIdBeforeSend.Content == null || readCurrentStatusBeforeSend.Content == null ||
                        readCurrentIdBeforeSend.Content.Length == 0 || readCurrentStatusBeforeSend.Content.Length == 0)
                    {
                        Send_Result_Content(e_Production_Status.Error, code);
                        Enqueue_Product_To_Record(code, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);

                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS V2 READ BEFORE SEND ERROR - Mã {code}. IDMsg={readCurrentIdBeforeSend.Message}, StatusMsg={readCurrentStatusBeforeSend.Message}");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        });

                        return;
                    }

                    cameraSubPreviousId = readCurrentIdBeforeSend.Content[0];
                    cameraSubPreviousStatus = readCurrentStatusBeforeSend.Content[0];
                }

                //phân làn
                string sendCode = "0";
                if (cache_CartonID % 2 == 0)
                {
                    sendCode = "1"; // Gửi dữ liệu mã thùng đến PLC
                }
                else
                {
                    sendCode = "2"; // Gửi dữ liệu mã thùng đến PLC
                }

                //gửi lên PLC thành công
                bool successSend = false;
                sw.Stop();
                currentCameraSubProcessingTime = sw.Elapsed.TotalMilliseconds;

                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    successSend = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), sendCode);
                }
                else
                {
                    successSend = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), sendCode);
                }

                Globals.Printer_Job--;

                if (successSend)
                {
                    // Timeout check: cho phép tắt hẳn, hoặc chọn V1/V2 bằng config
                    if (AppConfigs.Current.CameraSub_Timeout_Enabled)
                    {
                        if (AppConfigs.Current.CameraSub_Timeout_Mode_2)
                        {
                            // V2: đồng bộ theo ID/Status PLC
                            CameraSubSyncV2Result resultV2 = CheckCameraSubTimeoutV2(code, cameraSubPreviousId, cameraSubPreviousStatus);

                            if (resultV2.Result == e_CameraSubSyncV2_Result.Timeout)
                            {
                                Send_Result_Content(e_Production_Status.Timeout, code);
                                Enqueue_Product_To_Record(code, e_Production_Status.Timeout, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);

                                this.InvokeIfRequired(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS TIMEOUT V2 - Mã {code}. {resultV2.Message}");
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                });

                                return;
                            }

                            if (resultV2.Result == e_CameraSubSyncV2_Result.NoResponse)
                            {
                                Send_Result_Content(e_Production_Status.Error, code);
                                Enqueue_Product_To_Record(code, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);

                                this.InvokeIfRequired(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS V2 POLLING EXIT -> APP ERROR - Mã {code}. {resultV2.Message}");
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                });

                                return;
                            }

                            if (resultV2.Result == e_CameraSubSyncV2_Result.SyncError || resultV2.Result == e_CameraSubSyncV2_Result.ReadError)
                            {
                                Send_Result_Content(e_Production_Status.Error, code);
                                Enqueue_Product_To_Record(code, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);

                                this.InvokeIfRequired(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS V2 SYNC ERROR - Mã {code}. {resultV2.Message}");
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                });

                                return;
                            }
                        }
                        else
                        {
                            // V1: giữ nguyên cơ chế timeout cũ
                            bool isTimeout = CheckCameraSubTimeout(code);
                            if (isTimeout)
                            {
                                Send_Result_Content(e_Production_Status.Timeout, code);
                                Enqueue_Product_To_Record(code, e_Production_Status.Timeout, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);

                                this.InvokeIfRequired(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS TIMEOUT V1 - Mã {code} bị PLC timeout, hủy thêm vào thùng");
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                });

                                return; // Dừng xử lý
                            }
                        }
                    }

                    // Không timeout - tiếp tục xử lý bình thường
                    cache_CartonCount++; //tăng số lượng chai trong thùng
                    Globals.ProductionData.counter.carton_Packing_Count = cache_CartonCount; //cập nhật số lượng chai trong thùng
                    _produtionCodeData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"); // Cập nhật thời gian kích hoạt từ camera phụ
                    _produtionCodeData.Camera_Status = "1";
                    // Cập nhật trạng thái CameraSub đã scan thành công
                    //  if (Globals_Database.Dictionary_ProductionCode_Data.TryGetValue(code, out ProductionCodeData _produtionCodeDataCS_Update))
                    //  {
                    //    _produtionCodeData_Update.Camera_Status = "1"; // Đánh dấu đã được scan bởi CameraSub
                    // _produtionCodeDataCS_Update.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                    // }

                    //active thùng

                    Enqueue_Product_To_SQLite(code, _produtionCodeData); //thêm vào hàng chờ lưu sqlite
                    Enqueue_Product_To_Record(code, e_Production_Status.Pass, true, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, Globals.ProductionData.counter.cartonID);
                    Send_Result_Content(e_Production_Status.Pass, code);

                    //kiểm tra thùng đã hết chưa, chuyển thùng mới từ đây
                    if (Globals.ProductionData.counter.carton_Packing_Count == AppConfigs.Current.cartonPack)
                    {
                        //kiểm tra thùng mới có mã chưa, nếu chưa có thì dừng line
                        if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out cartonData))
                        {
                            if (cartonData.cartonCode == "0")
                            {
                                //chưa được quét mã bắt đầu => dừng line, 
                                //Quăng về pause
                                Globals.Production_State = e_Production_State.Pause;
                                this.InvokeIfRequired(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Chưa quét mã thùng, không xử lý tiếp.");
                                    Globals.Canhbao = "#Thùng đang xếp chưa có mã";
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                    //this.ShowErrorNotifier("#02 Thùng chuẩn bị xếp chưa có mã!");
                                });
                                return; //nếu thùng chưa có mã thì không xử lý tiếp
                            }
                        }
                        //kích hoạt thùng mới
                        Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(cache_CartonID, out ProductionCartonData cartonDataz);
                        Globals_Database.Activate_Carton.Enqueue(cartonDataz.cartonCode);
                        //nâng ID thùng lên 1, và tạo thùng mới
                        Globals.ProductionData.counter.cartonID = cache_CartonID + 1; //cập nhật ID thùng
                        Globals.ProductionData.counter.carton_Packing_Count = 0; //đặt lại số lượng chai trong thùng
                    }

                    //sản phẩm ok nằm ở đây, thêm lại

                    return;
                }



                //nếu gửi PLC thất bại
                Enqueue_Product_To_Record(code, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                Send_Result_Content(e_Production_Status.Error, code);
                return;


            }

            //loại sản phẩm ngay lập tức vì không có trong đống code kkk
            bool stp2 = false;
            if (AppConfigs.Current.PLC_Duo_Mode)
            {
                stp2 = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
            }
            else
            {
                stp2 = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
            }

            //Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0"); // Gửi dữ liệu loại sản phẩm đến PLC
            Send_Result_Content(e_Production_Status.NotFound, code);
            //gửi vào hàng chờ thêm record
            Enqueue_Product_To_Record(code, e_Production_Status.NotFound, true, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
            return;

        }

        private void Send_Result_Content(e_Production_Status status, string data)
        {
            if (status != e_Production_Status.Pass)
            {
                Globals.ProductionData.counter.failCount++;
            }

            CameraMain_HMI.Camera_Status = status;
            CameraMain_HMI.ID = Globals.ProductionData.counter.totalCount;
            CameraMain_HMI.Camera_Content = data;
            
            switch (status)
            {
                case e_Production_Status.Pass:
                    Globals.ProductionData.counter.passCount++;

                    if (!IsTestModeEnabled && Globals.ProductionData.counter.passCount == Globals.ProductionData.orderQty.ToInt32())
                    {
                        Globals.Production_State = e_Production_State.Waiting_Stop;
                    }
                    break;
                case e_Production_Status.Duplicate:
                    Globals.ProductionData.counter.duplicateCount++;
                    break;
                case e_Production_Status.NotFound:
                    Globals.ProductionData.counter.notfoundCount++;
                    break;
                case e_Production_Status.Error:
                    Globals.ProductionData.counter.errorCount++;
                    break;
                case e_Production_Status.ReadFail:
                    Globals.ProductionData.counter.readfailCount++;
                    break;
                case e_Production_Status.Timeout:
                    Globals.ProductionData.counter.errorCount++;
                    break;
            }
        }
        
        private void Enqueue_Product_To_Record(string code, e_Production_Status status, bool plcstatus, string activate_datetime, string production_datetime, int cartonID = 0)
        {
            ProductionCodeData_Record _produtionCodeData = new ProductionCodeData_Record
            {
                code = code,
                status = status,
                PLCStatus = plcstatus,
                Activate_Datetime = activate_datetime,
                Production_Datetime = production_datetime,
                cartonCode = "pending",
                Activate_User = Globals.CurrentUser.Username,
                cartonID = cartonID
            };
                Globals_Database.Insert_Product_To_Record_Queue.Enqueue(_produtionCodeData);
        }

        private void Enqueue_Product_To_SQLite(string code, ProductionCodeData productionCodeData, bool duplicate = false)
        {
            Globals_Database.Update_Product_To_SQLite_Queue.Enqueue((code, productionCodeData, duplicate));
        }

        #endregion

        #region PLC Communication
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
                    DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError, "Lỗi D022 khi gửi dữ liệu đến PLC", write.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError, "Lỗi D023 khi gửi dữ liệu đến PLC", ExceptionToJson(ex));
                return false;
            }
        }

        Stopwatch sw2;

        double PLCcurrentCameraSubProcessingTime = 0;
        double PLCmaxCameraSubProcessingTime = 0;

        public bool Send_To_PLC_2(string DM, string _data)
        {
            sw2.Start();
            try
            {
                OperateResult write = OMRON_PLC_02.plc.Write(DM, int.Parse(_data));
                if (write.IsSuccess)
                {
                    sw2.Stop();
                    PLCcurrentCameraSubProcessingTime = sw2.Elapsed.TotalMilliseconds;
                    return true;
                }
                else
                {
                    sw2.Stop();
                    PLCcurrentCameraSubProcessingTime = sw2.Elapsed.TotalMilliseconds;
                    DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError, "Lỗi D024 khi gửi dữ liệu đến PLC 2", write.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                sw2.Stop();
                PLCcurrentCameraSubProcessingTime = sw2.Elapsed.TotalMilliseconds;
                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError, "Lỗi D025 khi gửi dữ liệu đến PLC 2", ExceptionToJson(ex));
                return false;
            }
        }
        #endregion

        #region Initialization
        public void InitializeDevices()
        {
            try
            {
               
                    Camera_HIK.IP = AppConfigs.Current.Camera_Main_IP;
                    Camera_HIK.Port = AppConfigs.Current.Camera_Main_Port;
                    Camera_HIK.Connect();
                try
                {
                    OMRON_PLC.PLC_IP = PLCAddress.Get("PLC_IP");
                    OMRON_PLC_02.PLC_IP = PLCAddress.Get("PLC2_IP");
                    OMRON_PLC_02.PLC_PORT = PLCAddress.Get("PLC2_PORT").ToInt32();
                    OMRON_PLC_02.PLC_Ready_DM = PLCAddress.Get("PLC2_Ready_DM");
                    OMRON_PLC.PLC_PORT = PLCAddress.Get("PLC_PORT").ToInt32();
                    OMRON_PLC.PLC_Ready_DM = PLCAddress.Get("PLC_Ready_DM");
                }
                catch
                {
                    this.InvokeIfRequired(() =>
                    {
                        ipConsole.Items.Insert(0, "Lỗi đọc cấu hình PLC");
                    });
                }
                if (AppConfigs.Current.PLC_Test_Mode)
                {
                    OMRON_PLC.PLC_IP = "127.0.0.1";
                    OMRON_PLC_02.PLC_IP = "127.0.0.1";
                    OMRON_PLC_02.PLC_PORT = 9001;
                    OMRON_PLC.PLC_PORT = 9600;
                }

                
                OMRON_PLC.Time_Update = 1000;
                OMRON_PLC_02.Time_Update = 1000;
                OMRON_PLC.InitPLC();

                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    try
                    {
                        OMRON_PLC_02.InitPLC();
                    }
                    catch (Exception e)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Insert(0, "Chế độ 2 PLC" + e.Message);
                        });
                    }
                    
                    
                }

            }
            catch (Exception ex)
            {
                this.ShowErrorDialog("Lỗi D001 khi khởi tạo thiết bị: " + ex.Message);
            }
        }

        public void InitializeTasks()
        {
            try
            {
                WK_Update_UI.RunWorkerAsync();
                WK_Update_UI.DoWork += WK_Update_UI_DoWork;
            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier("Lỗi D002 khi khởi tạo task: " + ex.Message);
            }
        }
        #endregion

        #region UI Update Methods
        public void UpdateDeviceState()
        {
            UpdateCameraMainState();
            UpdatePLCState();
        }

        public void UpdateCameraMainState()
        {
            this.InvokeIfRequired(() =>
            {
                switch (Globals.CameraMain_State)
                {
                    case e_Camera_State.CONNECTED:
                        if (opC1_State.Text != "Tốt")
                        {
                            opLedC1.Blink = false;
                            opC1_State.Text = "Tốt";
                            opC1_State.FillColor = Color.White;
                            opC1_State.RectColor = Color.Green;
                            opLedC1.Color = Color.Green;
                            opLedC1.On = true;
                        }
                        break;
                        
                    case e_Camera_State.DISCONNECTED:
                        if (opC1_State.Text != "Lỗi")
                        {
                            opLedC1.Blink = true;
                            opC1_State.Text = "Lỗi";
                            opC1_State.FillColor = Color.MistyRose;
                            opC1_State.RectColor = Color.Red;
                            opLedC1.Color = Color.Red;
                            opLedC1.On = true;
                        }
                        break;
                        
                    case e_Camera_State.RECONNECTING:
                        if (opC1_State.Text != "...")
                        {
                            opLedC1.Blink = true;
                            opC1_State.Text = "...";
                            opC1_State.FillColor = Color.Yellow;
                            opC1_State.RectColor = Color.Red;
                            opLedC1.Color = Color.Yellow;
                            opLedC1.On = true;
                        }
                        break;
                }
            });
        }

        public void UpdatePLCState()
        {
            if(AppConfigs.Current.PLC_Duo_Mode)
            {
                this.InvokeIfRequired(() =>
                {
                    if (Globals.PLC_Connected && Globals.PLC_Connected_02)
                    {
                        if (opPLC_State.Text != "Tốt2")
                        {
                            opLedPLC.Blink = false;
                            opPLC_State.Text = "Tốt2";
                            opPLC_State.FillColor = Color.White;
                            opPLC_State.RectColor = Color.Green;
                            opLedPLC.Color = Color.Green;
                            opLedPLC.On = true;
                        }
                    }
                    else
                    {
                        if (opPLC_State.Text != "Lỗi")
                        {
                            opLedPLC.Blink = true;
                            opPLC_State.Text = "Lỗi";
                            opPLC_State.FillColor = Color.MistyRose;
                            opPLC_State.RectColor = Color.Red;
                            opLedPLC.Color = Color.Red;
                            opLedPLC.On = true;
                        }
                    }
                });
            }
            else
            {
                this.InvokeIfRequired(() =>
                {
                    if (Globals.PLC_Connected)
                    {
                        if (opPLC_State.Text != "Tốt1")
                        {
                            opLedPLC.Blink = false;
                            opPLC_State.Text = "Tốt1";
                            opPLC_State.FillColor = Color.White;
                            opPLC_State.RectColor = Color.Green;
                            opLedPLC.Color = Color.Green;
                            opLedPLC.On = true;
                        }
                    }
                    else
                    {
                        if (opPLC_State.Text != "Lỗi")
                        {
                            opLedPLC.Blink = true;
                            opPLC_State.Text = "Lỗi";
                            opPLC_State.FillColor = Color.MistyRose;
                            opPLC_State.RectColor = Color.Red;
                            opLedPLC.Color = Color.Red;
                            opLedPLC.On = true;
                        }
                    }
                });
            }
            
            
            
        }
        public void UpdateCounterUI()
        {
            // Cập nhật các giá trị đếm từ PLC
            GetCounterFromPLC();

            this.InvokeIfRequired(() =>
            {
                opTotal.Value = Globals.CameraMain_PLC_Counter.total; // Tổng số sản phẩm đã sản xuất
                opPass.Value = Globals.CameraMain_PLC_Counter.total_pass; // Số sản phẩm đã kích hoạt
                opFail.Value = Globals.CameraMain_PLC_Counter.total_failed; // Số sản phẩm đã loại bỏ
                opReadFail.Value = Globals.CameraMain_PLC_Counter.timeout; // Số sản phẩm đã loại bỏ do không đọc được từ camera
                opTotalCase.Value = Globals.ProductionData.counter.cartonID;
            });
        }

        public void GetCounterFromPLC ()
        {
            // Đọc counter cho CameraMain (luôn từ PLC chính)
            OperateResult<int[]> readCount = OMRON_PLC.plc.ReadInt32(PLCAddress.Get("PLC_Total_Count_DM_C1"), 5);
            if (readCount.IsSuccess)
            {
                // Cập nhật các giá trị đếm từ PLC
                Globals.CameraMain_PLC_Counter.total = readCount.Content[0]; // Tổng số sản phẩm đã sản xuất
                Globals.CameraMain_PLC_Counter.camera_read_fail = readCount.Content[1]; // Số sản phẩm đã loại bỏ
                Globals.CameraMain_PLC_Counter.total_pass = readCount.Content[2]; // Số sản phẩm đã kích hoạt
                Globals.CameraMain_PLC_Counter.timeout = readCount.Content[3]; // Số sản phẩm bị timeout
                Globals.CameraMain_PLC_Counter.total_failed = readCount.Content[4] + readCount.Content[1]; // Số lượng sản phẩm không đọc được từ camera
            }
            else
            {
                //ghi log lỗi
                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError, "Lỗi DA01068 khi đọc dữ liệu đếm từ PLC", readCount.Message);
            }

            // Đọc counter cho CameraSub - Hỗ trợ PLC Duo Mode
            OperateResult<int[]> readCameraSub;
            string plcAddress;

            if (AppConfigs.Current.PLC_Duo_Mode)
            {
                // Đọc từ PLC2 khi ở chế độ Duo
                plcAddress = PLCAddress.Get("PLC2_Total_Count_DM_C1");
                readCameraSub = OMRON_PLC_02.plc.ReadInt32(plcAddress, 5);
            }
            else
            {
                // Đọc từ PLC chính khi không ở chế độ Duo
                plcAddress = PLCAddress.Get("PLC_Total_Count_DM_C1");
                readCameraSub = OMRON_PLC.plc.ReadInt32(plcAddress, 5);
            }

            if (readCameraSub.IsSuccess)
            {
                // Cập nhật các giá trị đếm từ PLC cho CameraSub
                Globals.CameraSub_PLC_Counter.total = readCameraSub.Content[0]; // Tổng số sản phẩm đã sản xuất
                Globals.CameraSub_PLC_Counter.camera_read_fail = readCameraSub.Content[1]; // Số sản phẩm đã loại bỏ
                Globals.CameraSub_PLC_Counter.total_pass = readCameraSub.Content[2]; // Số sản phẩm đã kích hoạt
                Globals.CameraSub_PLC_Counter.timeout = readCameraSub.Content[3]; // Số sản phẩm bị timeout
                Globals.CameraSub_PLC_Counter.total_failed = readCameraSub.Content[4] + readCameraSub.Content[1]; // Số lượng sản phẩm thất bại

                this.InvokeIfRequired( () =>
                {
                    // Hiển thị tổng số sản phẩm lỗi của Camera Sub (fail + read fail)
                    opFailC2.Value = readCameraSub.Content[4] + readCameraSub.Content[1];
                } );
            }
            else
            {
                // Ghi log lỗi khi đọc CameraSub counter
                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError,
                    $"Lỗi DA01069 khi đọc dữ liệu đếm CameraSub từ PLC{(AppConfigs.Current.PLC_Duo_Mode ? "2" : "")} [{plcAddress}]",
                    readCameraSub.Message);
            }
        }

        public CameraSubSyncV2Result CheckCameraSubTimeoutV2(string productCode, int? previousIdSnapshot = null, int? previousStatusSnapshot = null)
        {
            try
            {
                int pollingIntervalMs = AppConfigs.Current.CameraSub_V2_Polling_Delay_Ms > 0
                    ? AppConfigs.Current.CameraSub_V2_Polling_Delay_Ms
                    : AppConfigs.Current.CameraSub_Polling_Interval_Ms;
                int timeoutMs = AppConfigs.Current.CameraSub_V2_Polling_Exit_Ms > 0
                    ? AppConfigs.Current.CameraSub_V2_Polling_Exit_Ms
                    : AppConfigs.Current.CameraSub_Timeout_Ms;

                Func<string, ushort, OperateResult<int[]>> readInt32 = (address, length) =>
                {
                    if (AppConfigs.Current.PLC_Duo_Mode)
                    {
                        return OMRON_PLC_02.plc.ReadInt32(address, length);
                    }

                    return OMRON_PLC.plc.ReadInt32(address, length);
                };

                string currentIdAddress = PLCAddress.Get("PLC_CurrentID_DM_C1");
                string currentStatusAddress = PLCAddress.Get("PLC_CurrentStatus_DM_C1");
                string historyIdAddress = PLCAddress.Get("PLC_IDHistory_Start_DM_C1");
                string historyStatusAddress = PLCAddress.Get("PLC_StatusHistory_Start_DM_C1");

                CameraSubSyncV2Result result = CameraSubPlcSyncV2Helper.WaitAndResolve(
                    readInt32,
                    currentIdAddress,
                    currentStatusAddress,
                    historyIdAddress,
                    historyStatusAddress,
                    timeoutMs,
                    pollingIntervalMs,
                    status => status == 1 || status == 2, // PLC status PASS (lane 2 = 1, lane 1 = 2)
                    status => status == 3,  // PLC status TIMEOUT
                    previousIdSnapshot,
                    previousStatusSnapshot
                );

                if (AppConfigs.Current.CameraSub_Timeout_Log_Enabled)
                {
                    this.InvokeIfRequired(() =>
                    {
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS V2 - Code={productCode}, {result.Message}");
                        ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi CS V2 timeout sync", ExceptionToJson(ex));
                return new CameraSubSyncV2Result
                {
                    Result = e_CameraSubSyncV2_Result.ReadError,
                    Message = ex.Message
                };
            }
        }

        // Hàm kiểm tra timeout cho CameraSub với polling approach - Hỗ trợ PLC Duo Mode
        public bool CheckCameraSubTimeout(string productCode, int expectedPassCountIncrease = 1)
        {
            try
            {
                // Kiểm tra xem có bật tính năng timeout không
                if (!AppConfigs.Current.CameraSub_Timeout_Enabled)
                {
                    if (AppConfigs.Current.CameraSub_Timeout_Log_Enabled)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS TIMEOUT DISABLED - Bỏ qua kiểm tra timeout cho mã {productCode}");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        });
                    }
                    return false; // Không timeout - tính năng bị tắt
                }

                // Lưu snapshot counter trước khi gửi
                int passCountBefore = Globals.CameraSub_PLC_Counter.total_pass;

                // Thông số polling từ AppConfigs
                int pollingIntervalMs = AppConfigs.Current.CameraSub_Polling_Interval_Ms;
                int timeoutMs = AppConfigs.Current.CameraSub_Timeout_Ms;
                int maxPolls = timeoutMs / pollingIntervalMs;
                int pollCount = 0;

                DateTime startTime = DateTime.Now;

                if (AppConfigs.Current.CameraSub_Timeout_Log_Enabled)
                {
                    this.InvokeIfRequired(() =>
                    {
                       // ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS POLLING - Bắt đầu monitor PLC cho mã {productCode} (Duo: {AppConfigs.Current.PLC_Duo_Mode})");
                        ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                    });
                }

                // Xác định PLC address dựa trên PLC_Duo_Mode
                string plcAddress = AppConfigs.Current.PLC_Duo_Mode ?
                    PLCAddress.Get("PLC2_Total_Count_DM_C1") :
                    PLCAddress.Get("PLC_Total_Count_DM_C1");

                // Polling loop
                while (pollCount < maxPolls)
                {
                    pollCount++;

                    OperateResult<int[]> readCheck;

                    // Đọc từ PLC hoặc PLC2 tùy theo cấu hình
                    if (AppConfigs.Current.PLC_Duo_Mode)
                    {
                        readCheck = OMRON_PLC_02.plc.ReadInt32(plcAddress, 5);
                    }
                    else
                    {
                        readCheck = OMRON_PLC.plc.ReadInt32(plcAddress, 5);
                    }

                    if (readCheck.IsSuccess)
                    {
                        int passCountCurrent = readCheck.Content[2]; // Pass count
                        int timeoutCountCurrent = readCheck.Content[3]; // Timeout count

                        // Kiểm tra xem pass count đã tăng chưa
                        if (passCountCurrent >= (passCountBefore + expectedPassCountIncrease))
                        {
                            // Success - PLC đã xử lý thành công
                            TimeSpan processingTime = DateTime.Now - startTime;

                            if (AppConfigs.Current.CameraSub_Timeout_Log_Enabled)
                            {
                                this.InvokeIfRequired(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS SUCCESS - Mã {productCode} xử lý thành công sau {processingTime.TotalMilliseconds:F0}ms (poll #{pollCount}) [PLC{(AppConfigs.Current.PLC_Duo_Mode ? "2" : "")}]");
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                });
                            }

                            // Cập nhật counter local
                            Globals.CameraSub_PLC_Counter.total_pass = passCountCurrent;
                            Globals.CameraSub_PLC_Counter.timeout = timeoutCountCurrent;

                            return false; // Không timeout
                        }

                        // Kiểm tra xem timeout count có tăng không (PLC báo timeout)
                        if (timeoutCountCurrent > Globals.CameraSub_PLC_Counter.timeout)
                        {
                            // PLC báo timeout
                            TimeSpan processingTime = DateTime.Now - startTime;

                            this.InvokeIfRequired(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS TIMEOUT - PLC{(AppConfigs.Current.PLC_Duo_Mode ? "2" : "")} báo timeout cho mã {productCode} sau {processingTime.TotalMilliseconds:F0}ms (poll #{pollCount})");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            });

                            // Cập nhật counter local
                            Globals.CameraSub_PLC_Counter.timeout = timeoutCountCurrent;
                            Globals.CameraSub_PLC_Counter.total_pass = passCountCurrent;

                            return true; // Có timeout
                        }

                        // Cập nhật counter local cho lần đọc này
                        Globals.CameraSub_PLC_Counter.total_pass = passCountCurrent;
                        Globals.CameraSub_PLC_Counter.timeout = timeoutCountCurrent;
                    }
                    else
                    {
                        // Không đọc được PLC, ghi log nhưng tiếp tục polling
                        if (AppConfigs.Current.CameraSub_Timeout_Log_Enabled && (pollCount % 10 == 0)) // Log mỗi 10 lần polling để tránh spam
                        {
                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError,
                                $"Lỗi CS03 đọc PLC{(AppConfigs.Current.PLC_Duo_Mode ? "2" : "")} lần {pollCount} cho mã {productCode}", readCheck.Message);
                        }
                    }

                    // Delay theo config trước lần đọc tiếp theo
                    Thread.Sleep(pollingIntervalMs);
                }

                // Hết thời gian timeout - không có response từ PLC
                TimeSpan totalTime = DateTime.Now - startTime;

                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS TIMEOUT - Mã {productCode} không có response từ PLC{(AppConfigs.Current.PLC_Duo_Mode ? "2" : "")} sau {totalTime.TotalMilliseconds:F0}ms ({pollCount} polls)");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });

                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError,
                    $"CS TIMEOUT - Mã {productCode} timeout sau {pollCount} polls từ PLC{(AppConfigs.Current.PLC_Duo_Mode ? "2" : "")}",
                    $"Expected pass increase: {expectedPassCountIncrease}, Total time: {totalTime.TotalMilliseconds:F0}ms");

                return true; // Timeout
            }
            catch (Exception ex)
            {
                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error,
                    "Lỗi CS04 trong CheckCameraSubTimeout polling", ExceptionToJson(ex));
                return false; // Lỗi, giả định không timeout
            }
        }

        private void ProcessRunningState()
        {

            if (AppConfigs.Current.PLC_Duo_Mode)
            {

                    OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 0);

            }
            else
            {
                    OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 0);

            }

            //kiểm tra thùng hiện tại có mã chưa
            if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData))
            {

                if (cartonData.cartonCode == "0")
                {
                    Globals.Production_State = e_Production_State.Pause;
                    this.InvokeIfRequired(() =>
                    {
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Chưa có mã thùng mới, không xử lý tiếp.");
                        ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        //this.ShowErrorNotifier("Thùng đang xếp Chưa có mã thùng, không xử lý tiếp.", false, 10000);
                        Globals.Canhbao = "Thùng hiện tại chưa có mã";
                    });
                    return;
                }
            }
            else
            {
                //thùng không tồn tại
                Globals.Production_State = e_Production_State.Ready;
            }
            //kiểm tra thùng sắp tới có mã chưa
            if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData2))
            {
                if(Globals.ProductionData.counter.carton_Packing_Count >= (AppConfigs.Current.cartonPack - AppConfigs.Current.cartonOfset))
                {
                    if (cartonData2.cartonCode == "0")
                    {
                        Globals.Production_State = e_Production_State.Pause;
                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Chưa có mã thùng mới, không xử lý tiếp.");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            //this.ShowErrorNotifier("Thùng chuẩn bị xếp Chưa có mã thùng, không xử lý tiếp.", false, 10000);
                            Globals.Canhbao = "Thùng sắp tới chưa có mã";
                        });
                        return;
                    }
                }
                
            }

            bool canhbao = false;
            //kiểm tra bật cảnh báo sớm
            if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData21))
            {
                if (Globals.ProductionData.counter.carton_Packing_Count >= (AppConfigs.Current.cartonPack - AppConfigs.Current.cartonWarning))
                {
                    if (cartonData2.cartonCode == "0")
                    {
                        if(canhbao == false)
                        {
                            canhbao = true;
                            this.InvokeIfRequired(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Cảnh báo thùng sắp tới chưa có mã thùng.");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                //this.ShowErrorNotifier("Cảnh báo thùng sắp tới chưa có mã thùng.", false, 10000);
                                Globals.Canhbao = "Thùng sắp tới chưa có mã";
                            });

                            if (AppConfigs.Current.PLC_Duo_Mode)
                            {
                                if (Globals.ProductionData.counter.cartonID % 2 == 0)
                                {
                                    OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 3);
                                }
                                else
                                {
                                    OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 2);
                                }
                            }
                            else
                            {
                                if (Globals.ProductionData.counter.cartonID % 2 == 0)
                                {
                                    OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 3);
                                }
                                else
                                {
                                    OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 2);
                                }
                            }
                        }
                        return;
                    }
                }

            }
            else
            {
                if(canhbao)
                {
                    canhbao = false;
                    if (AppConfigs.Current.PLC_Duo_Mode)
                    {
                        OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 3);
                    }
                    else
                    {
                        OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 0);
                    }
                }
                
            }
        }

        public int state01 = 0;
        public int state02 = 0;
        private void ProcessPauseState()
        {

            state01 = state02 = 0;
            //kiểm tra thùng hiện tại có mã chưa
            if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData))
            {
                if (cartonData.cartonCode != "0")
                {
                    state01 = 2;
                   //Globals.Production_State = e_Production_State.Running; //nếu thùng hiện tại đã có mã thì chuyển sang running
                }
                else
                {
                    state01 = 1;
                }
            }

            //kiểm tra thùng sắp tới có mã chưa
            if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData2))
            {
                if (Globals.ProductionData.counter.carton_Packing_Count >= (AppConfigs.Current.cartonPack - AppConfigs.Current.cartonOfset))
                {
                    if (cartonData2.cartonCode != "0")
                    {
                        state02 = 2;
                        //Globals.Production_State = e_Production_State.Running; //nếu thùng sắp tới đã có mã thì chuyển sang running
                    }else
                    {
                        state02 = 1;
                    }
                }
                else
                                    {
                    state02 = 2; //nếu chưa đến lúc chuyển thùng thì cứ chạy bình thường
                }
            }
            else
            {
                //thùng cuối cùng không tồn tại
                state02 = 2;
            }

            if (state02 == 2 && state01 == 2)
            {
                Globals.Production_State = e_Production_State.Running;
            }

            if (state01 == 1)
            {
                if(AppConfigs.Current.PLC_Duo_Mode)
                {
                    if (Globals.ProductionData.counter.cartonID % 2 != 0)
                    {
                        OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 3);
                    }
                    else
                    {
                        OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 2);
                    }
                }
                else
                {
                    if (Globals.ProductionData.counter.cartonID % 2 != 0)
                    {
                        OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 2);
                    }
                    else
                    {
                        OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"),3);
                    }
                }
                
            }
            if (state02 == 1)
            {
                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    if (Globals.ProductionData.counter.cartonID % 2 != 0)
                    {
                        OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 2);
                    }
                    else
                    {
                        OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 3);
                    }
                }
                else
                {
                    if (Globals.ProductionData.counter.cartonID % 2 == 0)
                    {
                        OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 2);
                    }
                    else
                    {
                        OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 3);
                    }
                }
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

            if (Globals.Production_State == e_Production_State.Running || Globals.Production_State == e_Production_State.Waiting_Stop || Globals.Production_State == e_Production_State.Check_After_Completed)
            {
                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Conveyor_ENA_DM"), 1);
                }
                else
                {
                    OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Conveyor_ENA_DM"), 1);
                    //OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 1);
                }
                
            }
            else
            {
                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    OperateResult ws = OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Conveyor_ENA_DM"), 0);
                }
                else
                {
                    OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Conveyor_ENA_DM"), 0);
                    //OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 1);
                }
                // OperateResult ws = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Conveyor_ENA_DM"), 0);
            }
            this.InvokeIfRequired(() =>
            {
                if(oporderQty.Text != Globals.ProductionData.orderQty.ToString())
                {
                    oporderQty.Text = Globals.ProductionData.orderQty.ToString();
                }
                oporderNO.Text = Globals.ProductionData.orderNo;
                oporderQty.Text = Globals.ProductionData.orderQty.ToString();
                opproductionDate.Text = Globals.ProductionData.productionDate;
            });

            switch (Globals.Production_State)
            {
                case e_Production_State.NoSelectedPO:
                    break;
                case e_Production_State.Camera_Processing:

                    Task task = Task.Run(() =>
                    {
                        //chuyển trạng thái sang 
                        Globals.Production_State = e_Production_State.Pushing_to_Dic;
                        try
                        {
                            // Clear tất cả dictionaries trước khi load PO mới
                            Globals_Database.Dictionary_ProductionCode_Data.Clear();
                            Globals_Database.Dictionary_ProductionCarton_Data.Clear();

                            //lấy mã chai
                            var getCodes = Globals.ProductionData.getDataPO.Get_Codes(Globals.ProductionData.orderNo);

                            if (getCodes.issucess)
                            {
                                if (getCodes.Codes.Rows.Count == 0)
                                {
                                    //nếu không có mã nào thì chạy luôn

                                    Globals.Production_State = e_Production_State.Running;
                                    return;
                                }
                                //lấy dữ liệu thành công
                                bool hasDuplicateWithCarton = false;
                                bool hasDuplicateWithoutCarton = false;
                                List<string> duplicateCodesWithCarton = new List<string>();
                                List<string> duplicateCodesWithoutCarton = new List<string>();

                                foreach (DataRow codeRow in getCodes.Codes.Rows)
                                {
                                    string code = codeRow["Code"].ToString();
                                    string codeID = codeRow["ID"].ToString();
                                    string cartonCode = codeRow["cartonCode"].ToString();
                                    string status = codeRow["status"].ToString();
                                    string activateUser = codeRow["ActivateUser"].ToString();
                                    string activateDatetime = codeRow["ActivateDate"].ToString();
                                    string productionDate = codeRow["ProductionDate"].ToString();


                                    // Kiểm tra mã trùng với old_database (chỉ kiểm tra nếu không bypass)
                                    if (!AppConfigs.Current.Check_Db_Old_Bypass)
                                    {
                                        var checkResult = Globals.ProductionData.getDataPO.Check_Code_In_Old_Database(code);
                                        if (checkResult.exists)
                                        {
                                            // Kiểm tra cartonCode trong old_database
                                            if (checkResult.cartonCode != null && checkResult.cartonCode != "0" && checkResult.cartonCode != "pending")
                                            {
                                                // Mã trùng và có cartonCode != 0
                                                hasDuplicateWithCarton = true;
                                                duplicateCodesWithCarton.Add(code);
                                            }
                                            else
                                            {
                                                // Mã trùng nhưng cartonCode = 0 hoặc null
                                                hasDuplicateWithoutCarton = true;
                                                duplicateCodesWithoutCarton.Add(code);
                                            }
                                        }
                                    }

                                    if (!Globals_Database.Dictionary_ProductionCode_Data.ContainsKey(code))
                                    {
                                        //nếu chưa có thì thêm vào dictionary
                                        Globals_Database.Dictionary_ProductionCode_Data.Add(code, new ProductionCodeData
                                        {
                                            orderNo = Globals.ProductionData.orderNo,
                                            Code = code,
                                            codeID = codeID.ToInt32(),
                                            cartonCode = cartonCode, //sẽ cập nhật sau khi gửi xuống PLC
                                            Camera_Status = status, //chưa kích hoạt
                                            Activate_User = activateUser, //sẽ cập nhật sau khi kích hoạt
                                            Activate_Datetime = activateDatetime, //sẽ cập nhật sau khi kích hoạt
                                            Production_Datetime = productionDate, //ngày sản xuất sửa lại khi kích hoạt
                                        });
                                    }
                                }

                                // Xử lý kết quả kiểm tra mã trùng (chỉ xử lý nếu không bypass)
                                if (!AppConfigs.Current.Check_Db_Old_Bypass && (hasDuplicateWithCarton || hasDuplicateWithoutCarton))
                                {
                                    if (AppConfigs.Current.Check_Db_Old_Active)
                                    {
                                        // Nếu Check_Db_Old_Active = True
                                        if (hasDuplicateWithCarton)
                                        {
                                            // Có mã trùng với cartonCode != 0 -> cảnh báo lỗi và về Ready
                                            string errorMsg = $"Phát hiện mã trùng với dữ liệu cũ. Các mã: {string.Join(", ", duplicateCodesWithCarton.Take(10))}" +
                                                           (duplicateCodesWithCarton.Count > 10 ? $" và {duplicateCodesWithCarton.Count - 10} mã khác" : "");
                                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error,
                                                $"Phát hiện mã trùng với old_database (có cartonCode): {errorMsg}");
                                            this.InvokeIfRequired(() =>
                                            {
                                                this.ShowErrorDialog($"Phát hiện mã trùng với dữ liệu cũ: {errorMsg}");
                                            });
                                            Globals.Production_State = e_Production_State.Ready;
                                            return;
                                        }
                                        else if (hasDuplicateWithoutCarton)
                                        {
                                            // Chỉ có mã trùng với cartonCode = 0 -> chỉ cảnh báo, vẫn cho chạy
                                            string warningMsg = $"Cảnh báo: Phát hiện {duplicateCodesWithoutCarton.Count} mã trùng với dữ liệu cũ (cartonCode = 0). " +
                                                               $"Các mã: {string.Join(", ", duplicateCodesWithoutCarton.Take(5))}" +
                                                               (duplicateCodesWithoutCarton.Count > 5 ? $" và {duplicateCodesWithoutCarton.Count - 5} mã khác" : "");
                                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Warning, warningMsg);
                                            this.InvokeIfRequired(() =>
                                            {
                                                this.ShowWarningDialog(warningMsg);
                                            });
                                            // Tiếp tục chạy bình thường
                                        }
                                    }
                                    else
                                    {
                                        // Nếu Check_Db_Old_Active = False -> về Ready hết
                                        string errorMsg = $"Phát hiện mã trùng với dữ liệu cũ. " +
                                                         (hasDuplicateWithCarton ? $"Có {duplicateCodesWithCarton.Count} mã có cartonCode != 0. " : "") +
                                                         (hasDuplicateWithoutCarton ? $"Có {duplicateCodesWithoutCarton.Count} mã có cartonCode = 0." : "");
                                        DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error,
                                            $"Phát hiện mã trùng với old_database: {errorMsg}");
                                        this.InvokeIfRequired(() =>
                                        {
                                            this.ShowErrorDialog($"Phát hiện mã trùng với dữ liệu cũ: {errorMsg}");
                                        });
                                        Globals.Production_State = e_Production_State.Ready;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                //ghi log lỗi
                                DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DA01069 khi lấy dữ liệu mã sản phẩm", getCodes.message);
                                //this.ShowErrorNotifier($"Lỗi DA01069 khi lấy dữ liệu mã sản phẩm: {getCodes.message}");
                                //chuyển trạng thái về error
                                Globals.Production_State = e_Production_State.Error;
                            }

                            //lấy mã chai cho camera phụ


                        }
                        catch (Exception ex)
                        {
                            //ghi log lỗi
                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DA01071 khi xử lý dữ liệu mã sản phẩm", ExceptionToJson(ex));
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorDialog($"Lỗi DA01071 khi xử lý dữ liệu mã sản phẩm: {ex.Message}");
                            });
                            //chuyển trạng thái về error
                            Globals.Production_State = e_Production_State.Ready;
                        }

                        try
                        {
                            // Clear Dictionary_ProductionCarton_Data trước khi load PO mới
                            Globals_Database.Dictionary_ProductionCarton_Data.Clear();

                            TResult getCartons = new TResult(false, "Lỗi");
                            try
                            {
                                getCartons = Globals.ProductionData.getDataPO.Get_Cartons(Globals.ProductionData.orderNo);
                            }
                            catch (Exception ex)
                            {
                                this.ShowErrorDialog($"Lỗi DA1078 {ex.Message}");
                            }

                            if (getCartons.count == 0)
                            {

                                return;
                            }

                            try
                            {
                                if (getCartons.issuccess)
                                {
                                    //lấy dữ liệu thành công
                                    foreach (DataRow cartonRow in getCartons.data.Rows)
                                    {
                                        ProductionCartonData cartonData_a = new ProductionCartonData();
                                        cartonData_a.cartonCode = cartonRow["CartonCode"].ToString();
                                        cartonData_a.orderNo = Globals.ProductionData.orderNo;
                                        cartonData_a.Activate_User = cartonRow["ActivateUser"].ToString();
                                        cartonData_a.Activate_Datetime = cartonRow["Activate_Datetime"].ToString();
                                        cartonData_a.Start_Datetime = cartonRow["Start_Datetime"].ToString();
                                        cartonData_a.cartonID = cartonRow["ID"].ToString().ToInt32();
                                        cartonData_a.Production_Datetime = cartonRow["ProductionDate"].ToString();

                                        if (!Globals_Database.Dictionary_ProductionCarton_Data.ContainsKey(cartonData_a.cartonID))
                                        {
                                            Globals_Database.Dictionary_ProductionCarton_Data.Add(cartonData_a.cartonID, cartonData_a);
                                        }

                                        //kiểm tra xem có cái nào chưa hoàn tất save db không
                                        if (cartonData_a.Activate_Datetime != "0" && cartonData_a.Production_Datetime == "0")
                                        {
                                            Globals_Database.Activate_Carton.Enqueue(cartonData_a.cartonCode); //thêm vào hàng chờ cập nhật thùng đã kích hoạt
                                        }
                                    }
                                }
                                else
                                {
                                    //ghi log lỗi
                                    DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DA01070 khi lấy dữ liệu mã thùng", getCartons.message);
                                    this.InvokeIfRequired(() =>
                                    {
                                        //this.ShowErrorNotifier($"Lỗi DA01070 khi lấy dữ liệu mã thùng: {getCartons.message}");
                                    });

                                    //chuyển trạng thái về error
                                    Globals.Production_State = e_Production_State.Error;
                                }
                            }
                            catch (Exception e)
                            {
                                this.ShowErrorDialog($"Lỗi DA10707 {e.Message}");
                            }

                        }
                        catch (Exception ex)
                        {

                            //ghi log lỗi
                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DA01072 khi xử lý dữ liệu mã thùng", ExceptionToJson(ex));
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorDialog($"Lỗi DA01072 khi xử lý dữ liệu mã thùng: {ex.Message}");
                            });
                            //chuyển trạng thái về error
                            Globals.Production_State = e_Production_State.Ready;
                        }
                        //lấy mã thùng 


                        //kiểm tra lại các thùng đã active xem cập nhật chưa thì cập nhật lại
                        //chuyển sang runing
                        Globals.Production_State = e_Production_State.Running;

                    });
                    break;
                case e_Production_State.Pushing_new_PO_to_PLC:
                    //gửi dữ liệu mới xuống PLC
                    //xóa số đếm PLC
                    OperateResult writeClear = OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Reset_Counter_DM_C1"), 1);
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

                        // Clear tất cả dictionaries trước khi load PO mới (tiếp tục PO)
                        Globals_Database.Dictionary_ProductionCode_Data.Clear();
                        Globals_Database.Dictionary_ProductionCarton_Data.Clear();

                        var getCodes = Globals.ProductionData.getDataPO.Get_Codes(Globals.ProductionData.orderNo);
                        if (getCodes.issucess)
                        {
                            //lấy dữ liệu thành công
                            bool hasDuplicateWithCarton = false;
                            bool hasDuplicateWithoutCarton = false;
                            List<string> duplicateCodesWithCarton = new List<string>();
                            List<string> duplicateCodesWithoutCarton = new List<string>();

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

                                // Xác định trạng thái Camera Sub dựa trên SubCamera_ActivateDate
                                bool hasSubCameraScanned = !string.IsNullOrEmpty(subCameraDatetime) && subCameraDatetime != "0";
                                string subCameraStatus = hasSubCameraScanned ? "1" : "0";

                                // Kiểm tra mã trùng với old_database (chỉ kiểm tra nếu không bypass)
                                if (!AppConfigs.Current.Check_Db_Old_Bypass)
                                {
                                    var checkResult = Globals.ProductionData.getDataPO.Check_Code_In_Old_Database(code);
                                    if (checkResult.exists)
                                    {
                                        // Kiểm tra cartonCode trong old_database
                                        if (checkResult.cartonCode != null && checkResult.cartonCode != "0" && checkResult.cartonCode != "pending")
                                        {
                                            // Mã trùng và có cartonCode != 0
                                            hasDuplicateWithCarton = true;
                                            duplicateCodesWithCarton.Add(code);
                                        }
                                        else
                                        {
                                            // Mã trùng nhưng cartonCode = 0 hoặc null
                                            hasDuplicateWithoutCarton = true;
                                            duplicateCodesWithoutCarton.Add(code);
                                        }
                                    }
                                }

                                if (!Globals_Database.Dictionary_ProductionCode_Data.ContainsKey(code))
                                {
                                    //nếu chưa có thì thêm vào dictionary
                                    Globals_Database.Dictionary_ProductionCode_Data.Add(code, new ProductionCodeData
                                    {
                                        orderNo = Globals.ProductionData.orderNo,
                                        Code = code,
                                        codeID = codeID.ToInt32(),
                                        cartonCode = cartonCode, //sẽ cập nhật sau khi gửi xuống PLC
                                        Camera_Status = status, //chưa kích hoạt
                                        Activate_User = activateUser, //sẽ cập nhật sau khi kích hoạt
                                        Activate_Datetime = activateDatetime, //sẽ cập nhật sau khi kích hoạt
                                        Production_Datetime = productionDate, //ngày sản xuất sửa lại khi kích hoạt
                                    });
                                }
                            }

                            // Xử lý kết quả kiểm tra mã trùng (chỉ xử lý nếu không bypass)
                            if (!AppConfigs.Current.Check_Db_Old_Bypass && (hasDuplicateWithCarton || hasDuplicateWithoutCarton))
                            {
                                if (AppConfigs.Current.Check_Db_Old_Active)
                                {
                                    // Nếu Check_Db_Old_Active = True
                                    if (hasDuplicateWithCarton)
                                    {
                                        // Có mã trùng với cartonCode != 0 -> cảnh báo lỗi và về Ready
                                        string errorMsg = $"Phát hiện mã trùng với dữ liệu cũ. Các mã: {string.Join(", ", duplicateCodesWithCarton.Take(10))}" +
                                                       (duplicateCodesWithCarton.Count > 10 ? $" và {duplicateCodesWithCarton.Count - 10} mã khác" : "");
                                        DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error,
                                            $"Phát hiện mã trùng với old_database (có cartonCode): {errorMsg}");
                                        this.InvokeIfRequired(() =>
                                        {
                                            this.ShowErrorDialog($"Phát hiện mã trùng với dữ liệu cũ: {errorMsg}");
                                        });
                                        Globals.Production_State = e_Production_State.Ready;
                                        return;
                                    }
                                    else if (hasDuplicateWithoutCarton)
                                    {
                                        // Chỉ có mã trùng với cartonCode = 0 -> chỉ cảnh báo, vẫn cho chạy
                                        string warningMsg = $"Cảnh báo: Phát hiện {duplicateCodesWithoutCarton.Count} mã trùng với dữ liệu cũ (cartonCode = 0). " +
                                                           $"Các mã: {string.Join(", ", duplicateCodesWithoutCarton.Take(5))}" +
                                                           (duplicateCodesWithoutCarton.Count > 5 ? $" và {duplicateCodesWithoutCarton.Count - 5} mã khác" : "");
                                        DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Warning, warningMsg);
                                        Globals.Canhbao = warningMsg; // Lưu cảnh báo để hiển thị trên opTer
                                        this.InvokeIfRequired(() =>
                                        {
                                            this.ShowWarningDialog(warningMsg);
                                        });
                                        // Tiếp tục chạy bình thường
                                    }
                                }
                                else
                                {
                                    // Nếu Check_Db_Old_Active = False -> về Ready hết
                                    string errorMsg = $"Phát hiện mã trùng với dữ liệu cũ. " +
                                                     (hasDuplicateWithCarton ? $"Có {duplicateCodesWithCarton.Count} mã có cartonCode != 0. " : "") +
                                                     (hasDuplicateWithoutCarton ? $"Có {duplicateCodesWithoutCarton.Count} mã có cartonCode = 0." : "");
                                    DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error,
                                        $"Phát hiện mã trùng với old_database: {errorMsg}");
                                    this.InvokeIfRequired(() =>
                                    {
                                        this.ShowErrorDialog($"Phát hiện mã trùng với dữ liệu cũ: {errorMsg}");
                                    });
                                    Globals.Production_State = e_Production_State.Ready;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            //ghi log lỗi
                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DA01069 khi lấy dữ liệu mã sản phẩm", getCodes.message);
                            //this.ShowErrorNotifier($"Lỗi DA01069 khi lấy dữ liệu mã sản phẩm: {getCodes.message}");
                            //chuyển trạng thái về error
                            Globals.Production_State = e_Production_State.Error;
                        }

                        //chuyển sang camera
                        Globals.Production_State = e_Production_State.Camera_Processing;
                    });

                    //gửi dữ liệu tiếp tục xuống PLC
                    break;
                case e_Production_State.Ready:
                    break;
                case e_Production_State.Running:

                    //kiểm tra xem máy in ok chưa, nếu chưa chuyển sang chế độ check máy in 
                    //if(!Globals.Printer_Ready)
                    //{
                    //    Globals.Production_State = e_Production_State.Printer_Loading;
                    //}

                    ProcessRunningState();
                    break;
                case e_Production_State.Pause:
                    ProcessPauseState();
                    break;
                case e_Production_State.Start:
                    break;
                case e_Production_State.Checking_PO_Info:
                    break;
                case e_Production_State.Loading:
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
                case e_Production_State.Pushing_to_Dic:
                    break;
                case e_Production_State.Checking_Queue:
                    break;
                case e_Production_State.Waiting_Stop:

                    //bool isCartonReady4 = false;
                    //bool isCartonReady5 = false;

                    //kiểm tra thùng đang xếp xếp hết chưa
                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData5))
                    {
                        if (cartonData5.Activate_Datetime != "0")
                        {
                            //isCartonReady4 = true;
                        }
                    }

                    //KIỂM TRA  thùng cũ chốt mã chưa
                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonDatat))
                    {
                        if (cartonDatat.Activate_Datetime != "0")
                        {
                            //isCartonReady5 = true;
                        }
                    }

                    if (!IsTestModeEnabled && Globals.ProductionData.counter.cartonID > Globals.ProductionData.orderQty.ToInt32() / AppConfigs.Current.cartonPack)
                    {
                        //nếu thùng đang xếp đã chốt mã và thùng cũ đã chốt mã thì chuyển sang Completed
                        Globals.Production_State = e_Production_State.Check_After_Completed;
                    }

                    break;
                case e_Production_State.Check_After_Completed:

                    if (Globals_Database.Insert_Product_To_Record_Queue.Count > 0 || Globals_Database.Update_Product_To_SQLite_Queue.Count > 0 || Globals_Database.Update_Product_To_Record_Carton_Queue.Count > 0 || Globals_Database.aWS_Recive_Datas.Count > 0 || Globals_Database.Activate_Carton.Count > 0 || Globals_Database.aWS_Send_Datas.Count > 0)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            //UpdateStatusMessage("Đang ghi dữ liệu vào cơ sở dữ liệu, Vui lòng đợi trong giây lát...", Color.Teal);
                            this.ShowErrorDialog("Lỗi PP231: Đang có dữ liệu đang được ghi vào cơ sở dữ liệu, Vui lòng đợi trong giây lát.");
                        });
                    }
                    else
                    {
                        Thread.Sleep(AppConfigs.Current.Time_Delay_Complete);
                        Globals.Production_State = e_Production_State.Completed;
                    }

                    break;
                case e_Production_State.DuSanPham:
                    break;
                case e_Production_State.ThieuSanPham:
                    break;
                case e_Production_State.KiemTraThieu:
                    break;
                case e_Production_State.MaBiTrung:
                    break;
                case e_Production_State.Printer_Loading:
                    break;
                case e_Production_State.Printer_Ready:
                    break;
            }
        }

        int lastvisibleCount1 = 0; // Biến để lưu số lượng sản phẩm đã hiển thị
        int lastvisibleCount2 = 0; // Biến để lưu số lượng sản phẩm đã hiển thị cho camera phụ
        public void Update_Result_UI()
        {
            this.InvokeIfRequired(() =>
            {
                if(lastvisibleCount1 != CameraMain_HMI.ID)
                {
                    lastvisibleCount1 = CameraMain_HMI.ID; // Cập nhật số lượng sản phẩm đã hiển thị
                    opCameraMainConten.Text = CameraMain_HMI.Camera_Content; // Cập nhật nội dung camera chính
                    //opResultPassFailC2.Text = CameraMain_HMI.Camera_Status.ToString(); // Cập nhật trạng thái camera
                    opHis2.Items.Insert(0, $"#{CameraMain_HMI.ID} : {CameraMain_HMI.Camera_Status} - {CameraMain_HMI.Camera_Content}"); // Thêm mục mới vào danh sách lịch sử
                    if (opHis2.Items.Count > 20)
                    {
                        // Xóa các mục cũ nếu số lượng mục lớn hơn 20
                        opHis2.Items.Remove(opHis2.Items.Count -1);
                    }
                    
                    switch (CameraMain_HMI.Camera_Status)
                    {
                        case e_Production_Status.Pass:
                            opResultPassFailC2.FillColor = Color.Green; // Màu xanh cho sản phẩm hợp lệ
                            opResultPassFailC2.Text = "Tốt"; // Cập nhật trạng thái sản phẩm
                            break;
                        case e_Production_Status.Fail:
                            opResultPassFailC2.FillColor = Color.Red; // Màu đỏ cho sản phẩm lỗi
                            opResultPassFailC2.Text = "Lỗi"; // Cập nhật trạng thái sản phẩm
                            break;
                        case e_Production_Status.Duplicate:
                            opResultPassFailC2.FillColor = Color.Yellow; // Màu vàng cho sản phẩm trùng
                            opResultPassFailC2.Text = "Trùng"; // Cập nhật trạng thái sản phẩm

                            break;
                        case e_Production_Status.NotFound:
                            opResultPassFailC2.FillColor = Color.Orange; // Màu cam cho sản phẩm không tìm thấy
                            opResultPassFailC2.Text = "Không có"; // Cập nhật trạng thái sản phẩm

                            break;
                        case e_Production_Status.Error:
                            opResultPassFailC2.FillColor = Color.Red; // Màu cam cho sản phẩm lỗi hệ thống
                            opResultPassFailC2.Text = "Lỗi máy"; // Cập nhật trạng thái sản phẩm8935005801135
                            break;
                        case e_Production_Status.ReadFail:
                            opResultPassFailC2.FillColor = Color.Orange; // Màu cam cho sản phẩm không đọc được
                            opResultPassFailC2.Text = "Lỗi đọc"; // Cập nhật trạng thái sản phẩm
                            break;
                        case e_Production_Status.Timeout:
                            opResultPassFailC2.FillColor = Color.DarkOrange;
                            opResultPassFailC2.Text = "Timeout";
                            break;
                    }
                }

                if(lastvisibleCount2 != CameraSub_HMI.ID)
                {
                    lastvisibleCount2 = CameraSub_HMI.ID;
                    opHisCS.Items.Insert(0, $"#{CameraSub_HMI.ID} : {CameraSub_HMI.Camera_Status} - {CameraSub_HMI.Camera_Content}"); // Thêm mục mới vào danh sách lịch sử camera phụ
                    if (opHisCS.Items.Count > 20)
                    {
                        // Xóa các mục cũ nếu số lượng mục lớn hơn 20
                        opHisCS.Items.Remove(opHisCS.Items.Count - 1);
                    }
                }
            });

        }

        #endregion

        #region Background Workers & Threading
        private void WK_Update_UI_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Update_UI.CancellationPending)
            {
                try
                {
                    UpdateDeviceState();
                    UpdateCounterUI();
                    Process_Production_State();
                    Update_Result_UI();

                    //cập nhật time lên UI
                    this.InvokeIfRequired(() =>
                    {
                        opCurrentProcessingTime.Text = $"{ currentCameraSubProcessingTime: F4} ms";
                        if(currentCameraSubProcessingTime - maxCameraSubProcessingTime > 0)
                        {
                            maxCameraSubProcessingTime = currentCameraSubProcessingTime;
                        }
                        opMaxProcessingTime.Text = $"{maxCameraSubProcessingTime: F4} ms";

                        opPLCCurrentProcessingTime.Text = $"{PLCcurrentCameraSubProcessingTime: F4} ms";
                        if (PLCcurrentCameraSubProcessingTime - PLCmaxCameraSubProcessingTime > 0)
                        {
                            PLCmaxCameraSubProcessingTime = PLCcurrentCameraSubProcessingTime;
                        }
                        opPLCMaxProcessingTime.Text = $"{PLCmaxCameraSubProcessingTime: F4} ms";
                    });
                }
                catch (Exception ex)
                {
                    Globals.Log.WriteLogAsync("System", e_LogType.Error, $"Lỗi trong WK_Update_UI_DoWork: {ex.Message}");
                    //this.ShowErrorNotifier($"Lỗi DA0107 trong quá trình xử lý: {ex.Message}");
                }
                Thread.Sleep(100);
            }
        }

        public void Start_Queue_Process()
        {
            if (threadQueue == null || !threadQueue.IsAlive)
            {
                threadQueue = new Thread(Queue_Proccess);
                threadQueue.Start();
                threadQueue.IsBackground = true;
            }
        }
        #endregion

        #region Queue Processing
        public static void Queue_Proccess()
        {
            while(!offThread)
            {
                try
                {
                    // Cập nhật trạng thái Active của DB chính
                    if (Globals_Database.Update_Product_To_SQLite_Queue.Count > 0)
                    {
                        //// Lấy dữ liệu từ hàng đợi
                        var item = Globals_Database.Update_Product_To_SQLite_Queue.Dequeue();

                        string code = item.conten;
                        ProductionCodeData productionCodeData = item.data;
                        bool isDuplicate = item.duplicate;

                        Globals.ProductionData.setDB.Update_Active_Status(productionCodeData, Globals.ProductionData.orderNo);

                    }

                    //thêm sao lưu cho C chính
                    if (Globals_Database.Insert_Product_To_Record_Queue.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        ProductionCodeData_Record recordItem = Globals_Database.Insert_Product_To_Record_Queue.Dequeue();

                        Globals.ProductionData.setDB.Insert_Record(recordItem, Globals.ProductionData.orderNo);
                    }


                    //Xử lý camera phụ và phân thùng
                    //if (Globals_Database.Insert_Product_To_Record_CS_Queue.Count > 0)
                    //{
                    //    // Lấy dữ liệu từ hàng đợi
                    //    ProductionCodeData_Record recordItemCS = Globals_Database.Insert_Product_To_Record_CS_Queue.Dequeue();
                    //    Globals.ProductionData.setDB.Insert_Record_Camera_Sub(recordItemCS, Globals.ProductionData.orderNo);
                    //}

                    //thùng

                    if (Globals_Database.Update_Product_To_Record_Carton_Queue.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        ProductionCartonData cartonItem = Globals_Database.Update_Product_To_Record_Carton_Queue.Dequeue();
                        Globals.ProductionData.setDB.Update_Carton(cartonItem, Globals.ProductionData.orderNo);
                    }

                    //kiểm tra xem có thùng khởi động nào không
                    if (Globals_Database.Activate_Carton.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        string cartonCode = Globals_Database.Activate_Carton.Dequeue();

                        //lấy id thùng từ mã thùng 
                        DataRow cartonData = Globals.ProductionData.getDataPO.Get_Carton_By_Code(Globals.ProductionData.orderNo, cartonCode).Carton;

                        ProductionCartonData productionCartonData = new ProductionCartonData
                        {
                            cartonCode = cartonData["cartonCode"].ToString(),
                            orderNo = Globals.ProductionData.orderNo,
                            Activate_User = cartonData["ActivateUser"].ToString(), // Lưu thông tin người dùng kích hoạt
                            Activate_Datetime = cartonData["Activate_Datetime"].ToString(),
                            Start_Datetime = cartonData["Start_Datetime"].ToString(),
                            cartonID = cartonData["ID"].ToString().ToInt32(),
                            Production_Datetime = Globals.ProductionData.productionDate // Ngày sản xuất sẽ cập nhật sau khi kích hoạt
                        };

                        //lấy tất cả các sp trong record camer sub có status = pass và carton id = 
                        DataTable code_record = Globals.ProductionData.getDataPO.Get_Product_Carton_Records(Globals.ProductionData.orderNo, productionCartonData.cartonID).Records;

                        //đếm số lượng sản phẩm trong thùng
                        int product_in_carton = code_record.Rows.Count;

                        //nếu số lượng sản phẩm trong thùng khác với cấu hình thì dừng sản xuất và báo lỗi
                       if (product_in_carton != AppConfigs.Current.cartonPack)
                        {
                            //ghi log lỗi
                            DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Error, "Lỗi DA01073: Số lượng sản phẩm trong thùng không đúng", $"Mã thùng: {cartonCode}, Số lượng sản phẩm trong thùng: {product_in_carton}, Số lượng cấu hình: {AppConfigs.Current.cartonPack}");
                            //nếu lớn hơn thì báo lỗi

                            if (product_in_carton > AppConfigs.Current.cartonPack)
                            {
                                //nếu lớn hơn thì báo lỗi
                                Globals.Production_State = e_Production_State.DuSanPham;
                                //hiện dialog lỗi
                                
                                return;
                            }
                            //nếu nhỏ hơn thì báo lỗi
                            Globals.Production_State = e_Production_State.ThieuSanPham;
                            return;
                        }

                        //cập nhật mã thùng cho tất cả các sản phẩm trong db chính
                        foreach (DataRow codeRow in code_record.Rows)
                        {
                            string code = codeRow["Code"].ToString();
                            if (Globals_Database.Dictionary_ProductionCode_Data.TryGetValue(code, out ProductionCodeData productionCodeData))
                            {
                                //cập nhật mã thùng cho sản phẩm
                                productionCodeData.cartonCode = cartonCode;
                                Globals.ProductionData.setDB.Update_Active_Status(productionCodeData, Globals.ProductionData.orderNo);
                            }
                        }

                        //cập nhật lại thùng trong db
                        Globals.ProductionData.setDB.Update_Carton(productionCartonData, Globals.ProductionData.orderNo);
                    }

                    if (Globals_Database.aWS_Send_Datas.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        AWS_Send_Data itemAWS = Globals_Database.aWS_Send_Datas.Dequeue();
                        Globals.ProductionData.setDB.Update_Active_Status_With_KV_where_KV("Send_Status", itemAWS.send_Status, "ID", itemAWS.ID.ToString(), Globals.ProductionData.orderNo);
                    }

                    //cập nhật trạng thái AWS
                    if (Globals_Database.aWS_Recive_Datas.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        AWS_Recive_Data itemAWS = Globals_Database.aWS_Recive_Datas.Dequeue();
                        Globals.ProductionData.setDB.Update_Active_Status_With_KV_where_KV("Recive_Status", itemAWS.recive_Status, "ID", itemAWS.ID.ToString(), Globals.ProductionData.orderNo);
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

        #region Event Handlers
        private void oporderNO_Click(object sender, EventArgs e)
        {
            // Toggle thread state (existing functionality)
            offThread = !offThread;
        }

        private void btnClearPLC_Click(object sender, EventArgs e)
        {
            btnClearPLC.Enabled = false; // Vô hiệu hóa nút để tránh nhấn nhiều lần
            btnClearPLC.Text = "Đang xóa..."; // Cập nhật văn bản nút để hiển thị trạng thái đang reset
            Task task = Task.Run(() =>
            {
                Thread.Sleep(5000); // Đợi 1 giây để PLC xử lý lệnh
                OMRON_PLC.plc.Write(PLCAddress.Get("PLC_Clear_DM"), 1); // Gửi lệnh reset counter về 0
                if(AppConfigs.Current.PLC_Duo_Mode)
                {
                    OMRON_PLC_02.plc.Write(PLCAddress.Get("PLC2_Clear_DM"), 1); // Gửi lệnh reset counter về 0
                }
                this.InvokeIfRequired(() =>
                {
                    btnClearPLC.Enabled = true; // Kích hoạt lại nút sau khi hoàn thành
                    btnClearPLC.Text = "Xóa lỗi PLC"; // Cập nhật văn bản nút về trạng thái ban đầu
                });

            });
            
        }

        private void uiLedBulb1_Click(object sender, EventArgs e)
        {
            // TODO: Implement LED bulb click handler
        }
        #endregion

        #region Enums & Support Classes
        private enum e_Dash_LogType
        {
            Info,
            Warning,
            Error,
            PlcError,
            CameraError,
        }

        #endregion

        
        private void subpr_DoWork(object sender, DoWorkEventArgs e)
        {
            string code = e.Argument as string;
            Camera_Process(code);
        }

    }
}
