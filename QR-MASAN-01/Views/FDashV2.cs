using DocumentFormat.OpenXml.ExtendedProperties;
using HslCommunication;
using MainClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QR_MASAN_01.Auth;
using SpT;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using static MFI_Service.MFI_Service_Form;
using static QR_MASAN_01.ActiveLogs;
using static QR_MASAN_01.AWSLogs;
using static QR_MASAN_01.SystemLogs;

namespace QR_MASAN_01
{
    public partial class FDashboard : UIPage
    {
        public AwsIotClientHelper awsClient;
        public FDashboard()
        {
            InitializeComponent();
            WK_Process_AUData.RunWorkerAsync();
        }

        public void INIT()
        {
            try
            {
                // Khởi tạo các thành phần cần thiết
                WK_Update.RunWorkerAsync();
                WK_UI_CAM_Update.RunWorkerAsync();
                Camera.Connect();
                if (Setting.Current.Camera_Slot > 1)
                {
                    Camera_c.Connect();
                }
                PLC.PLC_IP = PLCAddress.Get("PLC_IP");
                PLC.PLC_PORT = Convert.ToInt32(PLCAddress.Get("PLC_PORT"));
                PLC.PLC_Ready_DM = PLCAddress.Get("PLC_Ready_DM");
                PLC.InitPLC();

                //kết nối MQTT

                string host = "a22qv9bgjnbsae-ats.iot.ap-southeast-1.amazonaws.com";
                string clientId = "MIPWP501";

                string rootCAPath = @"C:\Users\THUC\Downloads\Compressed\Archive\MIPWP501\AmazonRootCA1.pem";
                string pfxPath = @"C:\Users\THUC\Downloads\Compressed\Archive\MIPWP501\client-certificate.pfx";
                string pfxPassword = "thuc";

                awsClient = new AwsIotClientHelper(
                    host,
                    clientId,
                    rootCAPath,
                    "",
                    pfxPath,
                    pfxPassword

                );
                awsClient.AWSStatus_OnChange += AWS_Status_Onchange;
                awsClient.AWSStatus_OnReceive += AWS_Status_OnReceive;

                awsClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_ERROR, "Lỗi khởi tạo Dashboard", "System", ex.Message);
                SystemLogs.InsertToSQLite(systemLogs);

                // Hiển thị thông báo lỗi trên giao diện
                this.ShowErrorDialog("Lỗi khởi tạo Dashboard, Vui lòng tắt máy mở lại", ex.Message);
                //Hiện lên box
                LogUpdate($"Lỗi khởi tạo Dashboard: {ex.Message}");
                LogUpdate($"VUI LÒNG TẮT MÁY MỞ LẠI");
            }

        }

        void SafeInvoke(Action action)
        {
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }

        private void AWS_Status_OnReceive(object sender, AwsIotClientHelper.AWSStatusReceiveEventArgs e)
        {
            JObject jsonObject = JObject.Parse(e.Payload);
            // Lấy giá trị của trường "status" từ JSON
            string status = jsonObject["status"]?.ToString();
            //lấy giá trị của trường "message_id" từ JSON
            string messageId = jsonObject["message_id"]?.ToString();
            // Lấy giá trị của trường "error_message" từ JSON
            string errorMessage = jsonObject["error_message"]?.ToString();
            //thêm vào queue
            GV.AWS_Response_Queue.Enqueue(new AWS_Response
            {
                status = status,
                message_id = messageId,
                error_message = errorMessage
            });

        }

        private void AWS_Status_Onchange(object sender, AwsIotClientHelper.AWSStatusEventArgs e)
        {
            switch (e.Status)
            {
                case AwsIotClientHelper.e_awsIot_status.Connected:
                    // ghi log
                    SafeInvoke(() =>
                    {
                        AWSLogs aWSLogs = new AWSLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.CONNECT, "Connected", Globalvariable.CurrentUser.Username, "Kết nối thành công với AWS IoT Core");
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs);

                        //cập nhật trạng thái kết nối
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết nối thành công với AWS IoT Core.");
                    });

                    string[] topicsToSub = new[]
                                      {
                                            "CZ/MIPWP501/response"
                                        };

                    awsClient.SubscribeMultiple(topicsToSub);

                    break;
                case AwsIotClientHelper.e_awsIot_status.Disconnected:
                    SafeInvoke(() =>
                    {
                        //ghi log
                        AWSLogs aWSLogs1 = new AWSLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.DISCONNECT, "Disconnect", Globalvariable.CurrentUser.Username, "Mất kết nối với AWS IoT Core");
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs1);
                        //cập nhật trạng thái kết nối
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Mất kết nối với AWS IoT Core.");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Connecting:
                    SafeInvoke(() =>
                    {
                        //ghi log
                        AWSLogs aWSLogs2 = new AWSLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.CONNECT, "Connecting", Globalvariable.CurrentUser.Username, "Đang kết nối với AWS IoT Core");
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs2);
                        //cập nhật trạng thái kết nối
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đang kết nối với AWS IoT Core...");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Error:
                    SafeInvoke(() =>
                    {
                        //ghi log
                        AWSLogs aWSLogs3 = new AWSLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.ERROR, "Error", Globalvariable.CurrentUser.Username, $"Lỗi: {e.Message}");
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs3);
                        //cập nhật trạng thái kết nối
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi: {e.Message}");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Subscribed:
                    SafeInvoke(() =>
                    {
                        //ghi log
                        AWSLogs aWSLogs4 = new AWSLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.SUBSCRIBE, "Subscribed", Globalvariable.CurrentUser.Username, $"Đã đăng ký topic: {e.Message}");
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs4);
                        //cập nhật trạng thái kết nối
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã đăng ký topic: {e.Message}");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Unsubscribed:
                    SafeInvoke(() =>
                    {
                        //ghi log
                        AWSLogs aWSLogs5 = new AWSLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.SUBSCRIBE, "Unsubscribed", Globalvariable.CurrentUser.Username, "Đã hủy đăng ký các topic.");
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs5);
                        //cập nhật trạng thái kết nối
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã hủy đăng ký các topic.");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Published:
                    
                    break;
                case AwsIotClientHelper.e_awsIot_status.Unpublished:
                    SafeInvoke(() =>
                    {
                        //ghi log
                        AWSLogs aWSLogs7 = new AWSLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.PUBLISH, "Unpublished", Globalvariable.CurrentUser.Username, $"Không thể publish: {e.Message}");
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs7);
                    });
                    break;
            }

            bool ClearPLC = false;
        }

        public string dataBase_FileName = "";

        public void Update_HMI()
        {
            if(GV.Production_Status == e_Production_Status.RUNNING)
            {
                // Cập nhật trạng thái của các thành phần
                this.Invoke(new Action(() =>
                {
                    oporderNO.Text = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString();
                    opproductionDate.Text = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString();
                    opGTIN.Text = Globalvariable.Seleted_PO_Data.Rows[0]["GTIN"].ToString();
                    oporderQty.Text = Globalvariable.Seleted_PO_Data.Rows[0]["orderQty"].ToString();
                    opCodeCount.Text = Globalvariable.Seleted_PO_Data.Rows[0]["UniqueCodeCount"].ToString();
                }));
            }
            // Cập nhật các trường thông tin MFI trên giao diện
           
        }

        #region Các cập nhật lên màn hình
        //Gửi lên màn hình và lưu log
        public void LogUpdate(string message)
        {
            this.Invoke(new Action(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: {message}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            }));
        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Update.CancellationPending)
            {
                if (Globalvariable.WB_Color == Globalvariable.OK_Color)
                {
                    Globalvariable.WB_Color = Globalvariable.NG_Color;
                }
                else
                {
                    Globalvariable.WB_Color = Globalvariable.OK_Color;
                }

                if (GCamera.Camera_Status == e_Camera_Status.DISCONNECTED)
                {
                    opCamera.FillColor = Globalvariable.WB_Color;
                }



                if (!Globalvariable.PLCConnect)
                {
                    opPLCStatus.FillColor = Globalvariable.WB_Color;
                }

               

                //chế độ dữ liệu mới cũ

                bool printers = false;

                if (GPrinter.Printer_Status == e_PRINTER_Status.PRINTING)
                {
                    printers = true;
                }
                else
                {
                    printers = false;
                    if (Setting.Current.App_Mode == "ADD_Data")
                    {
                        printers = true;
                    }
                }

                if (Setting.Current.Camera_Slot == 1)
                {
                    GCamera.Camera_Status_02 = e_Camera_Status.CONNECTED;

                    this.Invoke(new Action(() =>
                    {
                        opCMR02Stt.Text = "Không dùng";
                        opCMR02Stt.FillColor = Color.Yellow;
                    }));
                }
                else
                {
                    if (GCamera.Camera_Status_02 == e_Camera_Status.DISCONNECTED)
                    {
                        opCMR02Stt.FillColor = Globalvariable.WB_Color;
                    }
                }
                //Ready
                if(Globalvariable.All_Ready)
                {
                    if (PLC.Ready != 1)
                    {
                        PLC.Ready = 1;
                    }
                }
                else
                {
                    if (PLC.Ready != 0)
                    {
                        PLC.Ready = 0;
                    }
                }


                if (GCamera.Camera_Status == e_Camera_Status.CONNECTED && GCamera.Camera_Status_02 == e_Camera_Status.CONNECTED && Globalvariable.PLCConnect)
                {
                    if (!Globalvariable.FDashBoard_Ready)
                    {
                        Globalvariable.FDashBoard_Ready = true;
                    }
                }
                else
                {
                    if (Globalvariable.FDashBoard_Ready)
                    {
                        Globalvariable.FDashBoard_Ready = false;
                    }
                }

                if (APP.ByPass_Ready)
                {
                    PLC.Ready = 1;
                }
                
                //if (ClearPLC)
                //{
                //    this.Invoke(new Action(() =>
                //    {
                //        btnClearPLC.Enabled = true;
                //        btnClearPLC.Text = "Xóa lỗi PLC";
                //        ClearPLC = false;
                //    }));
                //}

                //Kiểm tra PLC_ACTIVE_DM nếu = 1 set Globale ACTIVE = true dùng hsl
                OperateResult<int> read = PLC.plc.ReadInt32(PLCAddress.Get("PLC_Bypass_DM_C1"));
                if (read.IsSuccess)
                {
                    if (read.Content != 1)
                    {
                        if (Globalvariable.ACTIVE_C1 == false)
                        {
                            ////ghi log 
                            //ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Bật Camera 01", "PLC", "Nhận kích hoạt camera 01 từ PLC, nhận giá trị khác 1");
                            ////Ghi vào hàng chờ
                            //ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C1 = true;
                        }
                    }
                    else
                    {
                        if (Globalvariable.ACTIVE_C1 == true)
                        {
                            ////ghi log
                            //ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Tắt Camera 01", "PLC", "Nhận ngừng kích hoạt camera 01 từ PLC, nhận giá trị bằng 1");
                            ////Ghi vào hàng chờ
                            //ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C1 = false;
                        }
                    }
                }

                OperateResult<int> read1 = PLC.plc.ReadInt32(PLCAddress.Get("PLC_Bypass_DM_C2"));
                if (read1.IsSuccess)
                {
                    if (read1.Content != 1)
                    {
                        if (Globalvariable.ACTIVE_C2 == false)
                        {
                            //ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.UNACTIVE, "Bật Camera 02", "PLC", "Nhận kích hoạt camera 02 từ PLC, nhận giá trị khác 1");
                            ////Ghi vào hàng chờ
                            //ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C2 = true;
                        }
                    }
                    else
                    {
                        if(Globalvariable.ACTIVE_C2 == true)
                        {
                            //ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.UNACTIVE, "Bật Camera 02", "PLC", "Nhận kích hoạt camera 02 từ PLC, nhận giá trị bằng 1");
                            ////Ghi vào hàng chờ
                            //ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C2 = false;
                        }
                        
                    }
                }

                if(Globalvariable.ACTIVE_C1 && Globalvariable.ACTIVE_C2)
                {
                    //ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Kích hoạt kiểm", "PLC", "Nhận kích hoạt kiểm từ PLC");
                    ////Ghi vào hàng chờ
                    //ActiveLogQueue.Enqueue(activeLogs);
                    Globalvariable.ACTIVE = true;
                }
                else
                {
                    Globalvariable.ACTIVE = false;
                }

                Update_HMI();

                Thread.Sleep(1000);
            }
        }

        //Cập nhật mã vừa đọc lên màn hình
        private void WK_Update_Result_To_UI_DoWork(object sender, DoWorkEventArgs e)
        {
            int lastShowID = 0;
            while (!WK_UI_CAM_Update.CancellationPending)
            {
                this.Invoke(new Action(() => {
                    opContentC1.Text = Globalvariable.C1_UI.Curent_Content;
                    if(GV.ID != lastShowID)
                    {
                        opHis2.Items.Add($"#{GV.ID}:{DateTime.Now.ToString("HH:mm:ss.fff")} : {Globalvariable.C2_UI.Curent_Content}");

                        if (Globalvariable.C1_UI.IsPass)
                        {
                            opResultPassFailC1.Text = "TỐT";
                            opResultPassFailC1.FillColor = Color.Green;
                        }
                        else
                        {
                            opResultPassFailC1.Text = "LỖI";
                            opResultPassFailC1.FillColor = Color.Red;
                        }

                        opContentC2.Text = Globalvariable.C2_UI.Curent_Content;

                        if (Globalvariable.C2_UI.IsPass)
                        {
                            opResultPassFailC2.Text = "TỐT";
                            opResultPassFailC2.FillColor = Color.Green;
                        }
                        else
                        {
                            opResultPassFailC2.Text = "LỖI";
                            opResultPassFailC2.FillColor = Color.Red;
                        }


                        if (Alarm.Alarm1)
                        {
                            lblAlarm.Text = "CẢNH BÁO SAI BARCODE (" + Alarm.Alarm1_Count.ToString() + ")";
                            lblAlarm.FillColor = Globalvariable.NG_Color;
                        }
                        lastShowID = GV.ID;
                    }
                    
                    
                }));
                Thread.Sleep(50);
            }
        }

        #endregion

        //Load lần đầu

        public string QR_Content = "Ver 18972";
        public string QR_Content_His = "";
        public string timeProcess = "0";
        public  int QRContentCount = 0;
        public bool ISPass = true;

        #region Xử lý tín hiệu từ camera

        private void Camera_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Sẵn sàng";
                            opCamera.FillColor = Globalvariable.OK_Color;
                        }));
                    }
                    break;
                case SPMS1.enumClient.DISCONNECTED:
                    if (GCamera.Camera_Status != e_Camera_Status.DISCONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.DISCONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Mất kết nối";
                        }));
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:

                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Sẵn sàng";
                            opCamera.FillColor = Globalvariable.OK_Color;
                        }));
                    }
                    //tạm tắt CMR phụ
                    //if (Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.RUNNING)
                    //{
                    //    Globalvariable.GCounter.Total_C1++;
                    //    try
                    //    {
                    //        if (!WK_CMR1.IsBusy)
                    //        {
                    //            WK_CMR1.RunWorkerAsync(_strData);
                    //        }
                    //        else if (!WK_CMR2.IsBusy)
                    //        {
                    //            WK_CMR2.RunWorkerAsync(_strData);
                    //        }
                    //        else if (!WK_CMR3.IsBusy)
                    //        {
                    //            WK_CMR3.RunWorkerAsync(_strData);
                    //        }
                    //        else
                    //        {
                    //            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : Không đủ luồng xử lí");
                    //            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;

                    //            //ghi log lỗi
                    //            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C1", Globalvariable.CurrentUser.Username, "Không đủ luồng xử lí");
                    //            Send_Result_Content_C1(e_Content_Result.ERROR, "Lỗi khi camera 02 trả về: Không đủ luồng xử lí");
                    //            //thêm vào Queue để ghi log
                    //            SystemLogs.LogQueue.Enqueue(systemLogs);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        this.Invoke(new Action(() =>
                    //        {
                    //            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : {ex.Message}");
                    //            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                    //        }));

                    //        //ghi log lỗi
                    //        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C1", Globalvariable.CurrentUser.Username, ex.Message);
                    //        //thêm vào Queue để ghi log
                    //        SystemLogs.LogQueue.Enqueue(systemLogs);
                    //    }
                    //}
                    //else
                    //{
                    //    LogUpdate("CHƯA KHỞI ĐỘNG SẢN XUẤT");
                    //}
                    
                    break;
                case SPMS1.enumClient.RECONNECT:
                    if (GCamera.Camera_Status != e_Camera_Status.RECONNECT)
                    {
                        GCamera.Camera_Status = e_Camera_Status.RECONNECT;

                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Kết nối lại";
                        }));
                    }
                    
                    
                    break;
            }
        }

        //camera 02
        private void Camera_c_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status_02 != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.CONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCMR02Stt.Text = "Sẵn sàng";
                            opCMR02Stt.FillColor = Globalvariable.OK_Color;
                        }));
                    }
                    break;
                case SPMS1.enumClient.DISCONNECTED:
                    if (GCamera.Camera_Status_02 != e_Camera_Status.DISCONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.DISCONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCMR02Stt.Text = "Mất kết nối";
                        }));
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:
                    if(Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.RUNNING)
                    {
                        if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                        {
                            GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                            Invoke(new Action(() =>
                            {
                                opCamera.Text = "Sẵn sàng";
                                opCamera.FillColor = Globalvariable.OK_Color;
                            }));
                        }
                        //xử lý dữ liệu nhận về
                        if (!WK_CMR4.IsBusy)
                        {
                            WK_CMR4.RunWorkerAsync(_strData);
                        }
                        else if (!WK_CMR5.IsBusy)
                        {
                            WK_CMR5.RunWorkerAsync(_strData);
                        }
                        else if (!WK_CMR6.IsBusy)
                        {
                            WK_CMR6.RunWorkerAsync(_strData);
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera 02 trả về : Không đủ luồng xử lí");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));

                            Send_Result_Content_C2(e_Content_Result.ERROR, "Lỗi khi camera 02 trả về: Không đủ luồng xử lí");

                            //ghi log lỗi
                            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C2", Globalvariable.CurrentUser.Username, "Không đủ luồng xử lí");
                            //thêm vào Queue để ghi log
                            SystemLogs.LogQueue.Enqueue(systemLogs);
                        }
                    }
                    else
                    {
                        LogUpdate("CHƯA KHỞI ĐỘNG SẢN XUẤT");
                    }
                    break;
                case SPMS1.enumClient.RECONNECT:

                    Invoke(new Action(() =>
                    {
                        opCMR02Stt.Text = "Kết nối lại";
                        opCMR02Stt.FillColor = Color.Red;
                    }));

                    break;
            }
        }

        //chương trình xử lý dữ liệu camera 01 khi có dữ liệu cho trước
        //Kiểm tra Pass/Fail.
        //Không tồn tại => loại
        //Đã kích hoạt => loại trừ nếu ở chế độ thả lại

        public void C1_Data_Process(string _strData)
        {
            //Xử lý dữ liệu nhanh nhất có thể
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //kiểm tra chuỗi có hợp lệ hay không
            //Kiểm tra tính hợp lệ của dữ liệu
            if (_strData.IsNullOrEmpty())
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C1(e_Content_Result.EMPTY, "MÃ RỖNG");
                return;
            }
            if (_strData == "FAIL")
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C1(e_Content_Result.FAIL, "Không đọc được");
                return;
            }
            //đổi <GS> về đúng ký tự thật
            _strData = _strData.Replace("<GS>", "\u001D").Replace("<RS>", "\u001E").Replace("<US>", "\u001F");
            //kiểm tra chuỗi có tồn tại trong bể dữ liệu chính hay không
            if (GV.C1_CodeData_Dictionary.TryGetValue(_strData, out CodeData C1CodeData))
            {
                //nếu chưa kích hoạt thì kích hoạt
                if (C1CodeData.Status == "0")
                {
                    C1CodeData.Status = "1";
                    //giờ kích hoạt theo ISO
                    C1CodeData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    C1CodeData.Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString();
                    //Gửi vào hàng chờ để cập nhật SQLite
                    GV.C1_Update_Content_To_SQLite_Queue.Enqueue(C1CodeData);
                }
                //nếu đã kích hoạt thì đá ra
                else
                {
                    //đá ra
                    Send_Result_Content_C1(e_Content_Result.DUPLICATE, _strData);
                    return;
                }
            }
            //nếu không tồn tại thì đá ra, không cần quan tâm thêm
            else {
                Send_Result_Content_C1(e_Content_Result.NOT_FOUND, _strData);
                return;
            }
        }

        //Camera 2 tương tự camera 01
        public void C2_Data_Process(string _strData)
        {
            //Xử lý dữ liệu nhanh nhất có thể
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            GV.ID++;
            //kiểm tra chuỗi có hợp lệ hay không
            //Kiểm tra tính hợp lệ của dữ liệu
            if (_strData.IsNullOrEmpty())
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C2(e_Content_Result.EMPTY, "MÃ RỖNG");
                return;
            }
            if (_strData == "FAIL")
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C2(e_Content_Result.FAIL, "Không đọc được");
                return;
            }
            //đổi <GS> về đúng ký tự thật
            _strData = _strData.Replace("<GS>", "\u001D").Replace("<RS>", "\u001E").Replace("<US>", "\u001F");
            //kiểm tra chuỗi có tồn tại trong bể dữ liệu chính hay không
            if (GV.C2_CodeData_Dictionary.TryGetValue(_strData, out CodeData C2CodeData))
            {
                //nếu chưa kích hoạt thì kích hoạt
                if (C2CodeData.Status == "0")
                {
                    //xác nhận PLC là pas
                    //gửi xuống PLC pass và xử lý tại đây
                    OperateResult write = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("1"));
                    //Ghi log
                    if (write.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C2++;
                        C2CodeData.Status = "1";
                        //giờ kích hoạt theo ISO
                        C2CodeData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        C2CodeData.Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString();
                        //Gửi vào hàng chờ để cập nhật SQLite
                        GV.C2_Update_Content_To_SQLite_Queue.Enqueue(C2CodeData);
                        //Ghi thành công
                        Send_Result_Content_C2(e_Content_Result.PASS, _strData);
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C2++;
                        //Gửi xuống PLC fail được tính là fail trong csdl luôn
                        C2CodeData.Status = "-1";
                        //giờ kích hoạt theo ISO
                        C2CodeData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        C2CodeData.Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString();
                        //Gửi vào hàng chờ để cập nhật SQLite
                        GV.C2_Update_Content_To_SQLite_Queue.Enqueue(C2CodeData);
                        //Ghi thất bại
                        Send_Result_Content_C2(e_Content_Result.ERROR,_strData);
                    }

                    
                }
                //nếu đã kích hoạt thì đá ra
                else
                {
                    //đá ra
                    Send_Result_Content_C2(e_Content_Result.DUPLICATE, _strData);
                    return;
                }
            }
            //nếu không tồn tại thì đá ra, không cần quan tâm thêm
            else
            {
                Send_Result_Content_C2(e_Content_Result.NOT_FOUND, _strData);
                return;
            }
        }

        #region Quản lý PLC và gửi tín hiệu PLC

        public enum e_Content_Result
        {
            PASS,//tốt
            FAIL, //lỗi
            REWORK, //thả lại
            DUPLICATE, //trùng
            EMPTY,//không có
            ERR_FORMAT, //lỗi định dạng
            NOT_FOUND, //không tìm thấy mã
            ERROR //lỗi không xác định
        }

        public void Send_Result_Content_C1(e_Content_Result content_Result, string _content)
        {
            switch (content_Result)
            {
                case e_Content_Result.PASS:

                    Globalvariable.GCounter.Total_Pass_C1++;
                    Globalvariable.C1_UI.Curent_Content = _content;
                    Globalvariable.C1_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("1"));
                    if (write.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C1++;
                    }
                    break;

                case e_Content_Result.FAIL:

                    Globalvariable.GCounter.Camera_Read_Fail_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;

                    Globalvariable.C1_UI.Curent_Content = "Không đọc được";
                    Globalvariable.C1_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write1 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));

                    if (write1.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }

                    break;

                case e_Content_Result.REWORK:

                    Globalvariable.GCounter.Rework_C1++; //Cái này không cộng vào số pass nếu phát hiện trùng

                    Globalvariable.C1_UI.Curent_Content = "Thả lại:" + _content;
                    Globalvariable.C1_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write5 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("1"));
                    if (write5.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C1++;
                    }
                    break;

                case e_Content_Result.DUPLICATE:

                    Globalvariable.GCounter.Duplicate_C1++;
                    Globalvariable.GCounter.Total_Failed_C1 += 1;

                    Globalvariable.C1_UI.Curent_Content = "Mã đã kích hoạt (trùng)";
                    Globalvariable.C1_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write4 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write4.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.EMPTY:

                    Globalvariable.GCounter.Empty_C1++;
                    Globalvariable.GCounter.Total_Failed_C1 += 1;

                    Globalvariable.C1_UI.Curent_Content = _content;
                    Globalvariable.C1_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write3 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write3.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.ERR_FORMAT:

                    Globalvariable.GCounter.Format_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;

                    Globalvariable.C1_UI.Curent_Content = "Sai cấu trúc!!!";
                    Globalvariable.C1_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write2 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write2.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.NOT_FOUND:

                    Globalvariable.GCounter.Empty_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;
                    Globalvariable.C1_UI.Curent_Content = "Mã không tồn tại";
                    Globalvariable.C1_UI.IsPass = false;
                    OperateResult write8 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write8.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.ERROR:
                    Globalvariable.C1_UI.Curent_Content = "Lỗi không xác định";
                    Globalvariable.C1_UI.IsPass = false;
                    Globalvariable.GCounter.Error_C1++;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write6 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write6.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;
            }
        }

        public void Send_Result_Content_C2(e_Content_Result content_Result, string _content)
        {
            DataResultSave dataResultSave = new DataResultSave();
            switch (content_Result)
            {
                
                case e_Content_Result.PASS:

                    Globalvariable.GCounter.Total_Pass_C2++;
                    Globalvariable.C2_UI.Curent_Content = _content;
                    Globalvariable.C2_UI.IsPass = true;

                        //gửi vào chỗ lưu
                        dataResultSave = new DataResultSave
                        {
                            ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                            Code = _content,
                            orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                            Status = content_Result.ToString(),
                            PLC_Send_Status = "true",
                            Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString()
                        };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);

                    break;

                case e_Content_Result.FAIL:

                    Globalvariable.C2_UI.Curent_Content = "Không đọc được";
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Camera_Read_Fail_C2++;
                    Globalvariable.GCounter.Total_Failed_C2++;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write1 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));

                    if (write1.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }

                    dataResultSave = new DataResultSave
                    {
                        ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                        Code = _content,
                        orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write1.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString()
                    };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;

                case e_Content_Result.REWORK:

                    Globalvariable.C2_UI.Curent_Content = "Thả lại:" + _content;
                    Globalvariable.C2_UI.IsPass = true;
                    Globalvariable.GCounter.Rework_C2++; //Cái này không cộng vào số pass nếu phát hiện trùng
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write5 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("1"));
                    if (write5.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C2++;
                    }
                    //gửi vào chỗ lưu
                    dataResultSave = new DataResultSave
                    {
                        ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                        Code = _content,
                        orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write5.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString()
                    };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;

                case e_Content_Result.DUPLICATE:
                    Globalvariable.C2_UI.Curent_Content = "Mã đã kích hoạt (trùng)";
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Duplicate_C2++;
                    Globalvariable.GCounter.Total_Failed_C2 += 1;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write4 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));
                    if (write4.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }

                    dataResultSave = new DataResultSave
                    {
                        ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                        Code = _content,
                        orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write4.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString()
                    };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;

                case e_Content_Result.EMPTY:
                    Globalvariable.C2_UI.Curent_Content = _content;
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Empty_C2++;
                    Globalvariable.GCounter.Total_Failed_C2 += 1;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write3 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));
                    if (write3.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }

                    dataResultSave = new DataResultSave
                    {
                        ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                        Code = _content,
                        orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write3.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString()
                    };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;

                case e_Content_Result.ERR_FORMAT:

                    Globalvariable.C2_UI.Curent_Content = "Sai cấu trúc!!!";
                    Globalvariable.C2_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write2 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));
                    if (write2.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }

                    dataResultSave = new DataResultSave
                    {
                        ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                        Code = _content,
                        orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write2.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString()
                    };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;

                case e_Content_Result.NOT_FOUND:

                    Globalvariable.C2_UI.Curent_Content = "Mã không tồn tại";
                    Globalvariable.C2_UI.IsPass = false;
                    OperateResult write8 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));
                    if (write8.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    dataResultSave = new DataResultSave
                    {
                        ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                        Code = _content,
                        orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write8.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString()
                    };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;

                case e_Content_Result.ERROR:
                    Globalvariable.C2_UI.Curent_Content = "Lỗi không xác định";
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Error_C2++;

                    dataResultSave = new DataResultSave
                    {
                        ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                        Code = _content,
                        orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = "FAIL-1",
                        Activate_Datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Production_Datetime = Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"].ToString()
                    };

                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;
            }
        }

        private void PLC_PLCStatus_OnChange(object sender, SPMS1.OmronPLC_Hsl.PLCStatusEventArgs e)
        {
            switch (e.Status)
            {
                case SPMS1.OmronPLC_Hsl.PLCStatus.Connecting:
                    Globalvariable.PLCConnect = true;
                    this.Invoke(new Action(() =>
                    {
                        opPLCStatus.Text = "Kết nối";
                        opPLCStatus.FillColor = Globalvariable.OK_Color;
                    }));
                    break;
                case SPMS1.OmronPLC_Hsl.PLCStatus.Disconnect:
                    Globalvariable.PLCConnect = false;
                    this.Invoke(new Action(() =>
                    {
                        opPLCStatus.Text = "Mất kết nối";
                        opPLCStatus.FillColor = Globalvariable.NG_Color;
                    }));
                    break;
            }
        }
        #endregion

        public void Update_Active_Status(CodeData _CodeItem)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/.ABC/{Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString()}.db;Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `UniqueCodes` SET " +
                               "`Status` = '1', " +
                               "`ActivateDate` = @activateDate, " +
                               "`ProductionDate` = @productionDate,  " +
                               "`ActivateUser` = @UserName  " +
                               "WHERE `_rowid_` = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", _CodeItem.ID);
                    command.Parameters.AddWithValue("@activateDate", _CodeItem.Activate_Datetime);
                    command.Parameters.AddWithValue("@productionDate", _CodeItem.Production_Datetime);
                    command.Parameters.AddWithValue("@UserName", Globalvariable.CurrentUser.Username);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        public void Update_Send_Status(int ID, string Status)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/.ABC/{Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString()}.db;Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `UniqueCodes` SET " +
                               "`Send_Status` = @send_status " +
                               "WHERE `_rowid_` = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", ID);
                    command.Parameters.AddWithValue("@send_status", Status);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        public void Update_Recive_Status(int ID, string Recive_Status)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/.ABC/{Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString()}.db;Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `UniqueCodes` SET " +
                               "`Recive_Status` = @recive_status " +
                               "WHERE `_rowid_` = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", ID);
                    command.Parameters.AddWithValue("@recive_status", Recive_Status);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Add_Content_To_SQLite( DataResultSave _Queue)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/.ABC/Record_{Globalvariable.Seleted_PO_Data.Rows[0]["orderNo"].ToString()}.db;Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO `Records` " +
                            "(ID, Code, Status, PLC_Status, ActivateDate, ActivateUser, ProductionDate) " +
                    "VALUES (@ID,@code,@status,@plc_status,@activatedate, @activateuser,@productiondate);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", _Queue.ID);
                    command.Parameters.AddWithValue("@code", _Queue.Code);
                    command.Parameters.AddWithValue("@status", _Queue.Status);
                    command.Parameters.AddWithValue("@plc_status", _Queue.PLC_Send_Status);
                    command.Parameters.AddWithValue("@activatedate", _Queue.Activate_Datetime);
                    command.Parameters.AddWithValue("@activateuser", Globalvariable.CurrentUser.Username);
                    command.Parameters.AddWithValue("@productiondate", _Queue.Production_Datetime);

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        private void btnResetCounter_Click(object sender, EventArgs e)
        {

            //OperateResult write = PLC.plc.Write("D22", short.Parse("1"));
            //if (write.IsSuccess)
            //{

            //    lblFail.Value = 0;
            //    lblPass.Value = 0;
            //    lblTotal.Value = 0;
            //}
            //else
            //{

            //}
        }

        private void btnClearPLC_Click(object sender, EventArgs e)
        {
            //btnClearPLC.Enabled = false;
            //btnClearPLC.Text = "Đang xóa lỗi";
            //OperateResult write = PLC.plc.Write("D18", short.Parse("1"));
            //if (write.IsSuccess)
            //{
            //    btnClearPLC.Enabled = true;

            //    btnClearPLC.Text = "Xóa lỗi PLC";

            //    Alarm.Alarm1 = false;
            //    Alarm.Alarm1_Count = 0;

            //    lblAlarm.Text = "-";
            //    lblAlarm.FillColor = Globalvariable.OK_Color;

            //}
            //else
            //{
            //    ClearPLC = true;
            //    //btnClearPLC.Enabled = true;
            //}
        }

        //xử kiện xử lý Queue

        private void WK_Process_SQLite_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_UI_CAM_Update.CancellationPending)
            {
                if (GV.Production_Status == e_Production_Status.RUNNING)
                {
                    //kiểm tra các queue cập nhật dữ liệu SQLite trước
                    if (GV.C2_Update_Content_To_SQLite_Queue.Count > 0)
                    {
                        //lấy ra dữ liệu
                        CodeData queueItem = GV.C2_Update_Content_To_SQLite_Queue.Dequeue();
                        //cập nhật vào SQLite
                        Update_Active_Status(queueItem);
                    }
                    //lưu lịch sử kiểm
                    if (GV.C2_Save_Result_To_SQLite_Queue.Count > 0)
                    {
                        //lấy ra dữ liệu
                        DataResultSave queueItem = GV.C2_Save_Result_To_SQLite_Queue.Dequeue();
                        //cập nhật vào SQLite
                        Add_Content_To_SQLite(queueItem);
                    }
                    //kiểm tra các queue cập nhật trạng thái gửi đi
                    //lấy tất cả các mã đã acvtivate và gửi đi
                    DataTable dataTable = new DataTable();
                    dataTable = Get_Unique_Codes_Run_Send_Pending(Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString());

                    //gửi mã
                    //if (dataTable.Rows.Count > 0)
                    //{
                    //    //lấy ra mã và gửi đi
                    //    for (int i = 0; i < dataTable.Rows.Count; i++)
                    //    {
                    //        string orderNO = Globalvariable.Seleted_PO_Data.Rows[0]["orderNo"].ToString();
                    //        string ID = dataTable.Rows[i]["ID"].ToString();
                    //        string code = dataTable.Rows[i]["Code"].ToString();
                    //        string Status = dataTable.Rows[i]["Status"].ToString();
                    //        string activateDate = dataTable.Rows[i]["ActivateDate"].ToString();
                    //        string productionDate = dataTable.Rows[i]["ProductionDate"].ToString();
                    //        var payload = new
                    //        {
                    //            message_id = $"{ID}-{orderNO}",
                    //            orderNo = orderNO,
                    //            uniqueCode = code,
                    //            status = Status,
                    //            activate_datetime = activateDate,
                    //            production_date = productionDate,
                    //            thing_name = "MIPWP501"
                    //        };
                    //        string json = JsonConvert.SerializeObject(payload);
                    //        var rs = awsClient.Publish_V2("CZ/data", json);

                    //        if (rs.Issuccess)
                    //        {
                    //            //cập nhật trạng thái đã gửi
                    //            Update_Send_Status(ID.ToInt32(), "Sent");
                    //        }
                    //        else
                    //        {
                    //            //ghi log lỗi không gửi được
                    //            Update_Send_Status(ID.ToInt32(), "Failed");
                    //        }


                    //    }
                    //}

                    ////kiểm tra danh sách các mã đã gửi đi
                    //if (GV.AWS_Response_Queue.Count > 0)
                    //{
                    //    //lấy ra dữ liệu
                    //    var responseItem = GV.AWS_Response_Queue.Dequeue();
                    //    string ID = responseItem.message_id.Split('-')[0];
                    //    //cập nhật trạng thái đã gửi
                    //    try {
                    //        Update_Recive_Status(ID.ToInt32(), responseItem.status);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //ghi log
                    //        AWSLogs aWSLogs = new AWSLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.ERROR, "Lỗi khi nhận tin nhắn trả về", Globalvariable.CurrentUser.Username, ex.Message);
                    //        //thêm vào Queue để ghi log
                    //        AWSLogsQueue.Enqueue(aWSLogs);
                    //    }
                        
                    //}

                    //cập nhật số lượng đã gửi
                    GV.Sent_Count = Get_Unique_Codes_Run_Send_Count(Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString());
                    //cập nhật số lượng đã nhận
                    GV.Received_Count = Get_Unique_Codes_Run_Recive_Count(Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString());

                    if ((GV.Sent_Count - GV.Received_Count) > 20)
                    {
                        //sub lại
                        string[] topicsToSub = new[]
                                          {
                                            "CZ/MIPWP501/response"
                                        };

                        awsClient.SubscribeMultiple(topicsToSub);

                    }

                    //gửi lại các mã Fail
                    DataTable dataTableFailed = new DataTable();
                    dataTableFailed = Get_Unique_Codes_Run_Send_Failed(Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString());
                    if (dataTableFailed.Rows.Count > 0)
                    {
                        //lấy ra mã và gửi đi
                        for (int i = 0; i < dataTableFailed.Rows.Count; i++)
                        {
                            string orderNO = Globalvariable.Seleted_PO_Data.Rows[0]["orderNo"].ToString();
                            string ID = dataTableFailed.Rows[i]["ID"].ToString();
                            string code = dataTableFailed.Rows[i]["Code"].ToString();
                            string Status = dataTableFailed.Rows[i]["Status"].ToString();
                            string activateDate = dataTableFailed.Rows[i]["ActivateDate"].ToString();
                            string productionDate = dataTableFailed.Rows[i]["ProductionDate"].ToString();
                            var payload = new
                            {
                                message_id = $"{ID}-{orderNO}",
                                orderNo = orderNO,
                                uniqueCode = code,
                                status = Status,
                                activate_datetime = activateDate,
                                production_date = productionDate,
                                thing_name = "MIPWP501"
                            };
                            string json = JsonConvert.SerializeObject(payload);
                            var rs = awsClient.Publish_V2("CZ/data", json);
                            if (rs.Issuccess)
                            {
                                //cập nhật trạng thái đã gửi
                                Update_Send_Status(ID.ToInt32(), "Sent");
                            }
                            else
                            {
                                //ghi log lỗi không gửi được
                                Update_Send_Status(ID.ToInt32(), "Failed");
                            }
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }


        public DataTable Get_Unique_Codes_Run_Send_Pending(string orderNo)
        {
            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";

            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                string query = "SELECT \"_rowid_\",* FROM \"main\".\"UniqueCodes\" WHERE \"Send_Status\" = 'pending'  AND \"Status\" != '0' ";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public DataTable Get_Unique_Codes_Run_Send_Failed(string orderNo)
        {
            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";

            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                string query = "SELECT \"_rowid_\",* FROM \"main\".\"UniqueCodes\" WHERE \"Send_Status\" = 'Failed'  AND \"Status\" != '0' ";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        //lấy số lượng Count có Send_Status = 'Sent'
        public int Get_Unique_Codes_Run_Send_Count(string orderNo)
        {
            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";
            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                string query = "SELECT COUNT(*) FROM \"main\".\"UniqueCodes\" WHERE \"Send_Status\" = 'Sent'  AND \"Status\" != '0' ";
                var command = new SQLiteCommand(query, conn);
                conn.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }
        }

        //lấy số lượng Count có Recive_Status != 'waiting'
        public int Get_Unique_Codes_Run_Recive_Count(string orderNo)
        {
            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";
            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                string query = "SELECT COUNT(*) FROM \"main\".\"UniqueCodes\" WHERE \"Recive_Status\" != 'waiting'  AND \"Status\" != '0' ";
                var command = new SQLiteCommand(query, conn);
                conn.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }
        }


        #region Các luồng xử lý dữ liệu khi có tín hiệu
        private double maxTimeT1 = 0;
        private double maxTimeT2 = 0;
        private double maxTimeT3 = 0;
        private double maxTimeT4 = 0;
        private double maxTimeT5 = 0;
        private double maxTimeT6 = 0;
        private void WK_CMR1_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            C1_Data_Process(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT1)
                {
                    maxTimeT1 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK1_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT1}" ;

        }
        private void WK_CMR2_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            //WhenDataRecive(inputString);
            //Camera_01_Data_Recive(inputString);
            C1_Data_Process(inputString);

            stopwatch.Stop();

            if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT2)
            {
                maxTimeT2 = stopwatch.Elapsed.TotalMilliseconds;
            }
            Globalvariable.GCounter.WK2_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT2}";

        }
        private void WK_CMR3_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            //WhenDataRecive(inputString);
            C1_Data_Process(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT3)
                {
                    maxTimeT3 = stopwatch.Elapsed.TotalMilliseconds;
                }
            Globalvariable.GCounter.WK3_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT3}";
        }
        private void WK_CMR4_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

           // Camera_02_Data_Recive(inputString);
           C2_Data_Process(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT4)
                {
                    maxTimeT4 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK4_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT4}";
            }
        private void WK_CMR5_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            
            C2_Data_Process(inputString);
            
            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT5)
                {
                    maxTimeT5 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK5_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT5}";
            }
        private void WK_CMR6_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            C2_Data_Process(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT6)
                {
                    maxTimeT6 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK5_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT5}";
            }

        #endregion

        private void uiTitlePanel5_Click(object sender, EventArgs e)
        {

        }

        private void ipConsole_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog(ipConsole.SelectedItem as string);
        }

        public void SendUnActive()
        {
            OperateResult operateResult = PLC.plc.Write(PLCAddress.Get("PLC_Bypass_DM_C1"),int.Parse("1"));
            OperateResult operateResult2 = PLC.plc.Write(PLCAddress.Get("PLC_Bypass_DM_C2"), int.Parse("1"));
            //không cần đợi trả về làm gì
        }
    }
}