using HslCommunication;
using HslCommunication.Profinet.Inovance;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using SpT.Auth;
using SpT.Communications.TCP;
using SpT.Logs;
using Sunny.UI;
using Sunny.UI.Win32;
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
using System.Xml.Linq;
using static MASAN_SERIALIZATION.Utils.ExtensionMethods;
using static SPMS1.OmronPLC_Hsl;

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public partial class FDashboard : UIPage
    {
        #region Private Fields
        private static LogHelper<e_Dash_LogType> DashboardPageLog;
        private static bool offThread = false;
        private Thread threadQueue;
        #endregion

        #region Constructor
        public FDashboard()
        {
            InitializeComponent();
            DashboardPageLog = new LogHelper<e_Dash_LogType>(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "MASAN-SERIALIZATION", "Logs", "Pages", "PDAlog.ptl"));
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
                        Task.Run(() => CameraMain_Process(data));
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
                    //Task.Run(() => CameraSub_Process(data));

                    if (subpr.IsBusy)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS Bận, không xử lý mã: {data}");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        });
                        //gửi xuống PLC loại sản phẩm
                        if (AppConfigs.Current.PLC_Duo_Mode)
                        {
                            Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                        }
                        else
                        {
                            Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                        }

                        //gửi loại sản phẩm
                        // Timeout detected - hủy thêm vào thùng
                        Send_Result_Content_CSub(e_Production_Status.Error, data);
                        Enqueue_Product_To_Record(data, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);

                    }
                    else
                    {
                        if (Globals.Production_State == e_Production_State.Running || Globals.Production_State == e_Production_State.Waiting_Stop || Globals.Production_State == e_Production_State.Check_After_Completed)
                        {
                            subpr.RunWorkerAsync(data);
                        }
                         else
                        {
                            if (AppConfigs.Current.PLC_Duo_Mode)
                            {
                                Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                            }
                            else
                            {
                                Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                            }

                        }
                    }
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
        private void CameraMain_Process(string _data)
        {
            Globals.ProductionData.counter.totalCount++;
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: #{Globals.ProductionData.counter.totalCount} Camera chính nhận dữ liệu: {_data}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            if (_data.IsNullOrEmpty())
            {
                bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "0");
                Send_Result_Content_CMain(e_Production_Status.Error, _data);
                Enqueue_Product_To_Record(_data, e_Production_Status.Error, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                return;
            }

            if (_data == "FAIL")
            {
                bool stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "0");
                Send_Result_Content_CMain(e_Production_Status.ReadFail, _data);
                Enqueue_Product_To_Record(_data, e_Production_Status.ReadFail, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                return;
            }

            _data = _data.Replace("<GS>", "\u001D").Replace("<RS>", "\u001E").Replace("<US>", "\u001F");

            if (Globals_Database.Dictionary_ProductionCode_Data.TryGetValue(_data, out ProductionCodeData _produtionCodeData))
            {
                if (_produtionCodeData.Main_Camera_Status == "0")
                {
                    // ✅ KIỂM TRA CROSS-PO: Kiểm tra xem mã đã được activate ở PO khác cùng GTIN chưa
                    var crossPOCheck = Globals.ProductionData.getDataPO.CheckCodeActivatedInOtherPO(_data, Globals.ProductionData.orderNo);

                    if (!crossPOCheck.issuccess)
                    {
                        // Mã đã được dùng ở PO khác cùng GTIN → Reject
                        Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "0");
                        Send_Result_Content_CMain(e_Production_Status.Duplicate, _data);
                        Enqueue_Product_To_Record(_data, e_Production_Status.Duplicate, true, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);

                        // Log chi tiết
                        DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.Warning,
                            $"[CROSS-PO DUPLICATE] {crossPOCheck.message}", _data);

                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: ❌ TRÙNG CROSS-PO: {crossPOCheck.message}");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        });

                        return;
                    }

                    // Mã OK - chưa dùng ở đâu cả → Tiếp tục activate
                    if (Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "1"))
                    {
                        _produtionCodeData.Main_Camera_Status = "1";
                        _produtionCodeData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                        _produtionCodeData.Production_Datetime = Globals.ProductionData.productionDate;
                        _produtionCodeData.cartonCode = "pending";
                        _produtionCodeData.Activate_User = Globals.CurrentUser.Username;

                        Send_Result_Content_CMain(e_Production_Status.Pass, _data);
                        Enqueue_Product_To_Record(_data, e_Production_Status.Pass, true, _produtionCodeData.Activate_Datetime, _produtionCodeData.Production_Datetime);
                        Enqueue_Product_To_SQLite(_data, _produtionCodeData);
                    }
                    else
                    {
                        _produtionCodeData.Main_Camera_Status = "-1";
                        _produtionCodeData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                        _produtionCodeData.Production_Datetime = Globals.ProductionData.productionDate;
                        _produtionCodeData.cartonCode = "pending";
                        _produtionCodeData.Activate_User = Globals.CurrentUser.Username;

                        Send_Result_Content_CMain(e_Production_Status.Error, _data);
                        Enqueue_Product_To_Record(_data, e_Production_Status.Error, false, _produtionCodeData.Activate_Datetime, _produtionCodeData.Production_Datetime);
                        Enqueue_Product_To_SQLite(_data, _produtionCodeData);
                    }
                    return;
                }
                else
                {
                    // Duplicate - đã dùng trong cùng PO này rồi
                    Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "0");
                    Send_Result_Content_CMain(e_Production_Status.Duplicate, _data);
                    Enqueue_Product_To_Record(_data, e_Production_Status.Duplicate, true, _produtionCodeData.Activate_Datetime, _produtionCodeData.Production_Datetime);
                    Enqueue_Product_To_SQLite(_data, _produtionCodeData, true);
                    return;
                }
            }
            else
            {
                Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C2"), "0");
                Send_Result_Content_CMain(e_Production_Status.NotFound, _data);
                Enqueue_Product_To_Record(_data, e_Production_Status.NotFound, true, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate);
                return;
            }
        }
        private void CameraSub_Process(string _data)
        {
            Globals.productionData_Cs.counter.totalCount++;

            if(Globals.Production_State != e_Production_State.Waiting_Stop)
            {
                if (Globals.Production_State != e_Production_State.Running)
                {
                    this.InvokeIfRequired(() =>
                    {
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Camera sau: Sản phẩm loại do lỗi dồn");
                        ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                    });
                    bool stp = false;
                    if (AppConfigs.Current.PLC_Duo_Mode)
                    {
                        stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                    }
                    else
                    {
                        stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                    }
                    Send_Result_Content_CSub(e_Production_Status.Error, _data);
                    Enqueue_Product_To_Record(_data, e_Production_Status.Error, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);
                    return;
                }
            }

            if (_data.IsNullOrEmpty())
            {
                bool stp = false;
                if(AppConfigs.Current.PLC_Duo_Mode)
                {
                    stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                }
                else
                {
                    stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                }
                Send_Result_Content_CSub(e_Production_Status.Error, _data);
                Enqueue_Product_To_Record(_data, e_Production_Status.Error, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);
                return;
            }

            if (_data == "FAIL")
            {
                bool stp = false;
                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                }
                else
                {
                    stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                }

                Send_Result_Content_CSub(e_Production_Status.ReadFail, _data);
                Enqueue_Product_To_Record(_data, e_Production_Status.ReadFail, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);
                return;
            }

            _data = _data.Replace("<GS>", "\u001D").Replace("<RS>", "\u001E").Replace("<US>", "\u001F");

            //kiểm tra mã có tồn tại hay không trong Dictionary chính (CameraMain)
            if (Globals_Database.Dictionary_ProductionCode_Data.TryGetValue(_data, out ProductionCodeData _produtionCodeData))
            {
                //chưa kích hoạt từ camera chính => Loại sản phẩm
                if (_produtionCodeData.Main_Camera_Status == "0")
                {
                    bool stp = false; // Gửi dữ liệu loại sản phẩm đến PLC
                    if (AppConfigs.Current.PLC_Duo_Mode)
                    {
                        stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                    }
                    else
                    {
                        stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                    }

                    //gửi vào hàng chờ thêm record
                    Enqueue_Product_To_Record(_data, e_Production_Status.ReadFail, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);
                    return;
                }

                // Kiểm tra trùng lặp trong Dictionary CameraSub
                if (Globals_Database.Dictionary_ProductionCode_CameraSub_Data.TryGetValue(_data, out ProductionCodeData _produtionCodeDataCS))
                {
                    // Kiểm tra xem đã được CameraSub scan chưa
                    if (_produtionCodeDataCS.Sub_Camera_Status != "0")
                    {
                        // Đã được scan => Duplicate
                        bool stp = false;
                        if (AppConfigs.Current.PLC_Duo_Mode)
                        {
                            stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                        }
                        else
                        {
                            stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                        }
                        Send_Result_Content_CSub(e_Production_Status.Duplicate, _data);
                        Enqueue_Product_To_Record(_data, e_Production_Status.Duplicate, stp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);
                        return;
                    }
                }

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
                        if (AppConfigs.Current.PLC_Duo_Mode)
                        {
                            stp = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), "0");
                        }
                        else
                        {
                            stp = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0");
                        }
                        //Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), "0"); // Gửi dữ liệu loại sản phẩm đến PLC
                        Enqueue_Product_To_Record(_data, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);
                        Send_Result_Content_CSub(e_Production_Status.Error, _data);

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
                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    successSend = Send_To_PLC_2(PLCAddress.Get("PLC2_Reject_DM_C1"), sendCode);
                }
                else
                {
                    successSend = Send_To_PLC(PLCAddress.Get("PLC_Reject_DM_C1"), sendCode);
                }

                if (successSend)
                {
                    // Kiểm tra timeout sau khi gửi PLC thành công
                    bool isTimeout = CheckCameraSubTimeout(_data);

                    if (isTimeout)
                    {
                        // Timeout detected - hủy thêm vào thùng
                        Send_Result_Content_CSub(e_Production_Status.Error, _data);
                        Enqueue_Product_To_Record(_data, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);

                        this.InvokeIfRequired(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS TIMEOUT - Mã {_data} bị PLC timeout, hủy thêm vào thùng");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        });

                        return; // Dừng xử lý
                    }

                    // Không timeout - tiếp tục xử lý bình thường
                    cache_CartonCount++; //tăng số lượng chai trong thùng
                    Globals.ProductionData.counter.carton_Packing_Count = cache_CartonCount; //cập nhật số lượng chai trong thùng
                    _produtionCodeData.Sub_Camera_Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"); // Cập nhật thời gian kích hoạt từ camera phụ

                    // Cập nhật trạng thái CameraSub đã scan thành công
                    if (Globals_Database.Dictionary_ProductionCode_CameraSub_Data.TryGetValue(_data, out ProductionCodeData _produtionCodeDataCS_Update))
                    {
                        _produtionCodeDataCS_Update.Sub_Camera_Status = "1"; // Đánh dấu đã được scan bởi CameraSub
                        _produtionCodeDataCS_Update.Sub_Camera_Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                    }

                    //active thùng

                    Enqueue_Product_To_SQLite(_data, _produtionCodeData); //thêm vào hàng chờ lưu sqlite
                    Enqueue_Product_To_Record(_data, e_Production_Status.Pass, true, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false, Globals.ProductionData.counter.cartonID);
                    Send_Result_Content_CSub(e_Production_Status.Pass, _data);

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
                Enqueue_Product_To_Record(_data, e_Production_Status.Error, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);
                Send_Result_Content_CSub(e_Production_Status.Error, _data);
                return;


            }

            //loại sản phẩm ngay lập tức
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
            Send_Result_Content_CSub(e_Production_Status.NotFound, _data);
            //gửi vào hàng chờ thêm record
            Enqueue_Product_To_Record(_data, e_Production_Status.NotFound, true, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"), Globals.ProductionData.productionDate, false);
            return;

        }
        private void Send_Result_Content_CMain(e_Production_Status status, string data)
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

                    if (Globals.ProductionData.counter.passCount == Globals.ProductionData.orderQty.ToInt32())
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
            }
        }
        private void Send_Result_Content_CSub(e_Production_Status status, string data)
        {
            if (status != e_Production_Status.Pass)
            {
                Globals.productionData_Cs.counter.failCount++;
            }

            CameraSub_HMI.Camera_Status = status;
            CameraSub_HMI.ID = Globals.productionData_Cs.counter.totalCount;
            CameraSub_HMI.Camera_Content = data;

            switch (status)
            {
                case e_Production_Status.Pass:
                    Globals.productionData_Cs.counter.passCount++;
                    break;
                case e_Production_Status.Duplicate:
                    Globals.productionData_Cs.counter.duplicateCount++;
                    break;
                case e_Production_Status.NotFound:
                    Globals.productionData_Cs.counter.notfoundCount++;
                    break;
                case e_Production_Status.Error:
                    Globals.productionData_Cs.counter.errorCount++;
                    break;
                case e_Production_Status.ReadFail:
                    Globals.productionData_Cs.counter.readfailCount++;
                    break;
            }
        }

        private void Enqueue_Product_To_Record(string code, e_Production_Status status, bool plcstatus, string activate_datetime, string production_datetime, bool CameraMain = true, int cartonID = 0)
        {
            ProductionCodeData_Record _produtionCodeData = new ProductionCodeData_Record
            {
                code = code,
                status = status,
                PLCStatus = plcstatus,
                Activate_Datetime = activate_datetime,
                Production_Datetime = production_datetime,
                cartonCode = "pending",
                Activate_User = Globals.CurrentUser.Username
            };
            
            if (!CameraMain)
            {
                _produtionCodeData.cartonID = cartonID;
                Globals_Database.Insert_Product_To_Record_CS_Queue.Enqueue(_produtionCodeData);
            }
            else
            {
                Globals_Database.Insert_Product_To_Record_Queue.Enqueue(_produtionCodeData);
            }
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

        public bool Send_To_PLC_2(string DM, string _data)
        {
            try
            {
                OperateResult write = OMRON_PLC_02.plc.Write(DM, int.Parse(_data));
                if (write.IsSuccess)
                {
                    return true;
                }
                else
                {
                    DashboardPageLog.WriteLogAsync(Globals.CurrentUser.Username, e_Dash_LogType.PlcError, "Lỗi D024 khi gửi dữ liệu đến PLC 2", write.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
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
                Camera_Main.IP = AppConfigs.Current.Camera_Main_IP;
                Camera_Main.Port = AppConfigs.Current.Camera_Main_Port;
                Camera_Main.Connect();
                
                Camera_Sub.IP = AppConfigs.Current.Camera_Sub_IP;
                Camera_Sub.Port = AppConfigs.Current.Camera_Sub_Port;
                Camera_Sub.Connect();

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
                //this.ShowErrorNotifier("Lỗi D002 khi khởi tạo task: " + ex.Message);
            }
        }
        #endregion

        #region UI Update Methods
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
        public void UpdateCameraSubState()
        {
            this.InvokeIfRequired(() =>
            {
                switch (Globals.CameraSub_State)
                {
                    case e_Camera_State.CONNECTED:
                        if (opC2_State.Text != "Tốt")
                        {
                            opLedC2.Blink = false;
                            opC2_State.Text = "Tốt";
                            opC2_State.FillColor = Color.White;
                            opC2_State.RectColor = Color.Green;
                            opLedC2.Color = Color.Green;
                            opLedC2.On = true;
                        }
                        break;
                        
                    case e_Camera_State.DISCONNECTED:
                        if (opC2_State.Text != "Lỗi")
                        {
                            opLedC2.Blink = true;
                            opC2_State.Text = "Lỗi";
                            opC2_State.FillColor = Color.MistyRose;
                            opC2_State.RectColor = Color.Red;
                            opLedC2.Color = Color.Red;
                            opLedC2.On = true;
                        }
                        break;
                        
                    case e_Camera_State.RECONNECTING:
                        if (opC2_State.Text != "...")
                        {
                            opLedC2.Blink = true;
                            opC2_State.Text = "...";
                            opC2_State.FillColor = Color.Yellow;
                            opC2_State.RectColor = Color.Red;
                            opLedC2.Color = Color.Yellow;
                            opLedC2.On = true;
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
                opReadFail.Value = Globals.CameraMain_PLC_Counter.camera_read_fail; // Số sản phẩm đã loại bỏ do không đọc được từ camera

                //opCaseCount.Text = Globals.ProductionData.counter.totalCartonCount.ToString(); // Số lượng thùng đã đóng gói
                opCaseCount.Text = Globals.ProductionData.counter.cartonID.ToString();
                opTotalCase.Value = Globals.ProductionData.counter.cartonID;
            });
        }

        public void GetCounterFromPLC ()
        {
            // Đọc counter cho CameraMain (luôn từ PLC chính)
            OperateResult<int[]> readCount = OMRON_PLC.plc.ReadInt32(PLCAddress.Get("PLC_Total_Count_DM_C2"), 5);
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
                    opFailC2.Value = readCount.Content[4] + readCount.Content[1];
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
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: CS POLLING - Bắt đầu monitor PLC cho mã {productCode} (Duo: {AppConfigs.Current.PLC_Duo_Mode})");
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

        // TEST METHOD - Thử nghiệm timeout mechanism
        public void TestCameraSubTimeoutMechanism()
        {
            try
            {
                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: ========== BẮT ĐẦU TEST TIMEOUT MECHANISM ==========");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });

                // Test Case 1: Simulate normal successful case
                TestCase_NormalSuccess();
                Thread.Sleep(1000);

                // Test Case 2: Simulate timeout case
                TestCase_Timeout();
                Thread.Sleep(1000);

                // Test Case 3: Test với mã giả
                TestCase_WithFakeCode();

                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: ========== KẾT THÚC TEST ==========");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });
            }
            catch (Exception ex)
            {
                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: TEST ERROR - {ex.Message}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });
            }
        }

        private void TestCase_NormalSuccess()
        {
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: --- TEST CASE 1: Normal Success ---");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Đọc counter hiện tại
            GetCounterFromPLC();
            int currentPass = Globals.CameraSub_PLC_Counter.total_pass;

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Current Pass Count: {currentPass}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Giả lập gửi PLC (không thực sự gửi, chỉ test polling)
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Giả lập gửi Pass command đến PLC...");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Test timeout check
            bool isTimeout = CheckCameraSubTimeout("TEST_SUCCESS_001");

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết quả: {(isTimeout ? "TIMEOUT" : "SUCCESS")}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });
        }

        private void TestCase_Timeout()
        {
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: --- TEST CASE 2: Timeout Simulation ---");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Lưu counter hiện tại
            int originalPass = Globals.CameraSub_PLC_Counter.total_pass;

            // Giả lập không có response từ PLC (không thay đổi counter)
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Giả lập PLC không response...");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Test với timeout ngắn hơn
            bool isTimeout = CheckCameraSubTimeoutShort("TEST_TIMEOUT_002", 100); // 100ms timeout

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết quả: {(isTimeout ? "TIMEOUT (Expected)" : "SUCCESS (Unexpected)")}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });
        }

        private void TestCase_WithFakeCode()
        {
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: --- TEST CASE 3: With Fake Product Code ---");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Simulate CameraSub_Process với mã fake
            string fakeCode = $"FAKE_{DateTime.Now:HHmmss}";

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Testing với mã giả: {fakeCode}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Test full flow
            TestFullCameraSubFlow(fakeCode);
        }

        // Helper method: Test timeout với thời gian ngắn hơn
        private bool CheckCameraSubTimeoutShort(string productCode, int timeoutMs)
        {
            try
            {
                int passCountBefore = Globals.CameraSub_PLC_Counter.total_pass;
                int pollingIntervalMs = 10;
                int maxPolls = timeoutMs / pollingIntervalMs;
                int pollCount = 0;
                DateTime startTime = DateTime.Now;

                while (pollCount < maxPolls)
                {
                    pollCount++;

                    OperateResult<int[]> readCheck = OMRON_PLC.plc.ReadInt32(PLCAddress.Get("PLC_Total_Count_DM_C1"), 5);
                    if (readCheck.IsSuccess)
                    {
                        int passCountCurrent = readCheck.Content[2];
                        if (passCountCurrent > passCountBefore)
                        {
                            return false; // Success
                        }
                    }

                    Thread.Sleep(pollingIntervalMs);
                }

                // Timeout
                TimeSpan totalTime = DateTime.Now - startTime;
                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: SHORT TIMEOUT - {productCode} sau {totalTime.TotalMilliseconds:F0}ms");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });

                return true;
            }
            catch (Exception ex)
            {
                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: TEST ERROR - {ex.Message}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });
                return false;
            }
        }

        // Helper method: Test full CameraSub flow
        private void TestFullCameraSubFlow(string testCode)
        {
            try
            {
                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Bắt đầu full flow test cho {testCode}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });

                // Giả lập gửi PLC
                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Giả lập Send_To_PLC cho {testCode}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });

                // Test timeout check
                bool isTimeout = CheckCameraSubTimeout(testCode);

                // Log kết quả
                this.InvokeIfRequired(() =>
                {
                    if (isTimeout)
                    {
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: FULL FLOW - {testCode} bị TIMEOUT, sẽ hủy thêm vào thùng");
                    }
                    else
                    {
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: FULL FLOW - {testCode} SUCCESS, sẽ thêm vào thùng");
                    }
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });

            }
            catch (Exception ex)
            {
                this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: FULL FLOW ERROR - {ex.Message}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });
            }
        }

        // SIMPLE TEST METHOD - Gọi trực tiếp để test nhanh - Hỗ trợ cấu hình
        public void SimpleTestTimeout()
        {
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: ========== SIMPLE TEST TIMEOUT ==========");
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Config - Timeout Enabled: {AppConfigs.Current.CameraSub_Timeout_Enabled}");
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Config - Timeout Ms: {AppConfigs.Current.CameraSub_Timeout_Ms}");
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Config - Polling Interval: {AppConfigs.Current.CameraSub_Polling_Interval_Ms}ms");
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Config - PLC Duo Mode: {AppConfigs.Current.PLC_Duo_Mode}");
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Config - Log Enabled: {AppConfigs.Current.CameraSub_Timeout_Log_Enabled}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Test với mã đơn giản
            string testCode = "SIMPLE_TEST_" + DateTime.Now.ToString("HHmmss");

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Testing mã: {testCode}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Đọc counter trước test
            GetCounterFromPLC();
            int beforePass = Globals.CameraSub_PLC_Counter.total_pass;
            int beforeTimeout = Globals.CameraSub_PLC_Counter.timeout;

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Counter trước test - Pass: {beforePass}, Timeout: {beforeTimeout}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Chạy test
            bool result = CheckCameraSubTimeout(testCode);

            // Đọc counter sau test
            GetCounterFromPLC();
            int afterPass = Globals.CameraSub_PLC_Counter.total_pass;
            int afterTimeout = Globals.CameraSub_PLC_Counter.timeout;

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Counter sau test - Pass: {afterPass}, Timeout: {afterTimeout}");
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết quả test: {(result ? "TIMEOUT" : "SUCCESS")}");
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: ========== KẾT THÚC SIMPLE TEST ==========");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });
        }

        // Test với cấu hình khác nhau
        public void TestWithDifferentConfigs()
        {
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: ========== TEST VỚI CÁC CẤU HÌNH KHÁC NHAU ==========");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Test 1: Timeout disabled
            TestTimeoutDisabled();
            Thread.Sleep(1000);

            // Test 2: Fast polling
            TestFastPolling();
            Thread.Sleep(1000);

            // Test 3: PLC Duo Mode toggle
            TestPLCDuoModeToggle();

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: ========== KẾT THÚC TEST CẤU HÌNH ==========");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });
        }

        private void TestTimeoutDisabled()
        {
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: --- TEST: Timeout Disabled ---");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Backup original setting
            bool originalSetting = AppConfigs.Current.CameraSub_Timeout_Enabled;

            // Disable timeout
            AppConfigs.Current.CameraSub_Timeout_Enabled = false;

            bool result = CheckCameraSubTimeout("TEST_DISABLED");

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết quả (nên là SUCCESS vì disabled): {(result ? "TIMEOUT" : "SUCCESS")}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Restore original setting
            AppConfigs.Current.CameraSub_Timeout_Enabled = originalSetting;
        }

        private void TestFastPolling()
        {
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: --- TEST: Fast Polling (5ms) ---");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Backup original settings
            int originalInterval = AppConfigs.Current.CameraSub_Polling_Interval_Ms;
            int originalTimeout = AppConfigs.Current.CameraSub_Timeout_Ms;

            // Set fast polling
            AppConfigs.Current.CameraSub_Polling_Interval_Ms = 5; // 5ms
            AppConfigs.Current.CameraSub_Timeout_Ms = 100; // 100ms total

            bool result = CheckCameraSubTimeout("TEST_FAST_POLL");

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết quả Fast Polling: {(result ? "TIMEOUT" : "SUCCESS")}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Restore original settings
            AppConfigs.Current.CameraSub_Polling_Interval_Ms = originalInterval;
            AppConfigs.Current.CameraSub_Timeout_Ms = originalTimeout;
        }

        private void TestPLCDuoModeToggle()
        {
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: --- TEST: PLC Duo Mode Toggle ---");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Backup original setting
            bool originalDuoMode = AppConfigs.Current.PLC_Duo_Mode;

            // Test với PLC_Duo_Mode = false
            AppConfigs.Current.PLC_Duo_Mode = false;
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Testing với PLC Duo Mode = false");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });
            bool result1 = CheckCameraSubTimeout("TEST_PLC_SINGLE");

            // Test với PLC_Duo_Mode = true
            AppConfigs.Current.PLC_Duo_Mode = true;
            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Testing với PLC Duo Mode = true");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });
            bool result2 = CheckCameraSubTimeout("TEST_PLC_DUO");

            this.InvokeIfRequired(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết quả Single PLC: {(result1 ? "TIMEOUT" : "SUCCESS")}");
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết quả Duo PLC: {(result2 ? "TIMEOUT" : "SUCCESS")}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });

            // Restore original setting
            AppConfigs.Current.PLC_Duo_Mode = originalDuoMode;
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
            if (state02 == 1)
            {
                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    if (Globals.ProductionData.counter.cartonID % 2 == 0)
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

            //Đẩy dữ liệu vào dictionary
            this.InvokeIfRequired(() =>
            {
                if(oporderQty.Text != Globals.ProductionData.orderQty.ToString())
                {
                    oporderQty.Text = Globals.ProductionData.orderQty.ToString();
                }
                oporderNO.Text = Globals.ProductionData.orderNo;
                oporderQty.Text = Globals.ProductionData.orderQty.ToString();
                opproductionDate.Text = Globals.ProductionData.productionDate;
                opGTIN.Text = Globals.ProductionData.gtin;
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
                                            Sub_Camera_Status = "0", //chưa kích hoạt từ camera phụ
                                            Activate_User = activateUser, //sẽ cập nhật sau khi kích hoạt
                                            Sub_Camera_Activate_Datetime = subCameraDatetime, //chưa kích hoạt
                                            Activate_Datetime = activateDatetime, //sẽ cập nhật sau khi kích hoạt
                                            Production_Datetime = productionDate, //ngày sản xuất sửa lại khi kích hoạt
                                        });
                                    }

                                    // Thêm vào Dictionary cho CameraSub để kiểm tra trùng lặp
                                    if (!Globals_Database.Dictionary_ProductionCode_CameraSub_Data.ContainsKey(code))
                                    {
                                        Globals_Database.Dictionary_ProductionCode_CameraSub_Data.Add(code, new ProductionCodeData
                                        {
                                            orderNo = Globals.ProductionData.orderNo,
                                            Code = code,
                                            codeID = codeID.ToInt32(),
                                            cartonCode = cartonCode,
                                            Main_Camera_Status = status,
                                            Sub_Camera_Status = "0", //khởi tạo chưa được scan bởi camera phụ
                                            Activate_User = activateUser,
                                            Sub_Camera_Activate_Datetime = subCameraDatetime,
                                            Activate_Datetime = activateDatetime,
                                            Production_Datetime = productionDate,
                                        });
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
                                        Sub_Camera_Status = "0", //chưa kích hoạt từ camera phụ
                                        Activate_User = activateUser, //sẽ cập nhật sau khi kích hoạt
                                        Sub_Camera_Activate_Datetime = subCameraDatetime, //chưa kích hoạt
                                        Activate_Datetime = activateDatetime, //sẽ cập nhật sau khi kích hoạt
                                        Production_Datetime = productionDate, //ngày sản xuất sửa lại khi kích hoạt
                                    });
                                }

                                // Thêm vào Dictionary cho CameraSub để kiểm tra trùng lặp
                                if (!Globals_Database.Dictionary_ProductionCode_CameraSub_Data.ContainsKey(code))
                                {
                                    Globals_Database.Dictionary_ProductionCode_CameraSub_Data.Add(code, new ProductionCodeData
                                    {
                                        orderNo = Globals.ProductionData.orderNo,
                                        Code = code,
                                        codeID = codeID.ToInt32(),
                                        cartonCode = cartonCode,
                                        Main_Camera_Status = status,
                                        Sub_Camera_Status = "0", //khởi tạo chưa được scan bởi camera phụ
                                        Activate_User = activateUser,
                                        Sub_Camera_Activate_Datetime = subCameraDatetime,
                                        Activate_Datetime = activateDatetime,
                                        Production_Datetime = productionDate,
                                    });
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

                    bool isCartonReady4 = false;
                    bool isCartonReady5 = false;

                    //kiểm tra thùng đang xếp xếp hết chưa
                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData5))
                    {
                       if(cartonData5.Activate_Datetime != "0")
                        {
                            isCartonReady4 = true;
                        }
                    }

                    //KIỂM TRA  thùng cũ chốt mã chưa
                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID-1, out ProductionCartonData cartonDatat))
                    {
                        if (cartonDatat.Activate_Datetime != "0")
                        {
                            isCartonReady5 = true;
                        }
                    }

                    if (Globals.ProductionData.counter.cartonID > Globals.ProductionData.orderQty.ToInt32() / AppConfigs.Current.cartonPack)
                    {
                        //nếu thùng đang xếp đã chốt mã và thùng cũ đã chốt mã thì chuyển sang Completed
                        Globals.Production_State = e_Production_State.Check_After_Completed;
                    }

                    break;
                case e_Production_State.Check_After_Completed:

                    if (Globals_Database.Insert_Product_To_Record_Queue.Count > 0 || Globals_Database.Update_Product_To_SQLite_Queue.Count > 0 || Globals_Database.Insert_Product_To_Record_CS_Queue.Count > 0 || Globals_Database.Update_Product_To_Record_Carton_Queue.Count > 0 || Globals_Database.aWS_Recive_Datas.Count > 0 || Globals_Database.Activate_Carton.Count > 0 || Globals_Database.aWS_Send_Datas.Count > 0)
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
                    if (Globals_Database.Insert_Product_To_Record_CS_Queue.Count > 0)
                    {
                        // Lấy dữ liệu từ hàng đợi
                        ProductionCodeData_Record recordItemCS = Globals_Database.Insert_Product_To_Record_CS_Queue.Dequeue();
                        Globals.ProductionData.setDB.Insert_Record_Camera_Sub(recordItemCS, Globals.ProductionData.orderNo);
                    }

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

            // Run timeout test when clicking orderNO (for testing purposes)
            Task.Run(() => TestCameraSubTimeoutMechanism());
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

            CameraSub_Process(code);
        }
    }
}
