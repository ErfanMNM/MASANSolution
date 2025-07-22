using DocumentFormat.OpenXml.ExtendedProperties;
using HslCommunication;
using MainClass;
using MainClass.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QR_MASAN_01.Utils;
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
using System.Threading.Tasks;
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
                WK_PO.RunWorkerAsync();
                WK_UI_CAM_Update.RunWorkerAsync();

                StartTask();
                Init_Camera();
                Connect_AWS();

                PLC.PLC_IP = PLCAddress.Get("PLC_IP");
                PLC.PLC_PORT = Convert.ToInt32(PLCAddress.Get("PLC_PORT"));
                PLC.PLC_Ready_DM = PLCAddress.Get("PLC_Ready_DM");
                
                PLC.InitPLC();

                Camera.Connect();
                Camera_c.Connect();

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

        private void Connect_AWS()
        {
            if(Setting.Current.AWS_ENA)
            {
                //kết nối MQTT
                string host = Setting.Current.host;
                string clientId = Setting.Current.clientId;
                string rootCAPath = Setting.Current.rootCAPath;
                string pfxPath = Setting.Current.pfxPath;
                string pfxPassword = Setting.Current.pfxPassword;

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
                Task.Run(() =>
                {
                    try
                    {
                        awsClient.ConnectAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        //ghi log lỗi
                        AWSLogs aWSLogs = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.ERROR, "Lỗi kết nối AWS IoT Core", Globalvariable.CurrentUser.Username, ex.Message);
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs);
                        LogUpdate($"Lỗi kết nối AWS IoT Core: {ex.Message}");
                    }
                });
            }
           
        }

        private void Init_Camera()
        {
            Camera_c.IP = Setting.Current.IP_Camera_02;
            Camera.IP = Setting.Current.IP_Camera_01;
            Camera_c.Port = Setting.Current.Port_Camera_02;
            Camera.Port = Setting.Current.Port_Camera_01;
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
                        AWSLogs aWSLogs = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.CONNECT, "Connected", Globalvariable.CurrentUser.Username, "Kết nối thành công với AWS IoT Core");
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
                        AWSLogs aWSLogs1 = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.DISCONNECT, "Disconnect", Globalvariable.CurrentUser.Username, "Mất kết nối với AWS IoT Core");
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
                        AWSLogs aWSLogs2 = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.CONNECT, "Connecting", Globalvariable.CurrentUser.Username, "Đang kết nối với AWS IoT Core");
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
                        AWSLogs aWSLogs3 = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.ERROR, "Error", Globalvariable.CurrentUser.Username, $"Lỗi: {e.Message}");
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
                        AWSLogs aWSLogs4 = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.SUBSCRIBE, "Subscribed", Globalvariable.CurrentUser.Username, $"Đã đăng ký topic: {e.Message}");
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
                        AWSLogs aWSLogs5 = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.SUBSCRIBE, "Unsubscribed", Globalvariable.CurrentUser.Username, "Đã hủy đăng ký các topic.");
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
                        AWSLogs aWSLogs7 = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.PUBLISH, "Unpublished", Globalvariable.CurrentUser.Username, $"Không thể publish: {e.Message}");
                        //thêm vào Queue để ghi log
                        AWSLogsQueue.Enqueue(aWSLogs7);
                    });
                    break;
            }
        }

        public string dataBase_FileName = "";

        public void Update_HMI()
        {
            if (GV.Production_Status == e_Production_Status.RUNNING)
            {
                SafeInvoke(() =>
                {
                    if (oporderNO.Text != GV.Selected_PO.orderNo.ToString())
                    {
                        oporderNO.Text = GV.Selected_PO.orderNo.ToString();
                        opproductionDate.Text = GV.Selected_PO.productionDate.ToString();
                        opGTIN.Text = GV.Selected_PO.GTIN.ToString();
                        oporderQty.Text = GV.Selected_PO.orderQty.ToString();
                        opReQty.Text = GV.Selected_PO.CodeCount.ToString();
                    }

                    opCamera_M_Pass.Value = Globalvariable.GCounter.Total_Pass_C2;
                    opCamera_M_Fail.Value = Globalvariable.GCounter.Total_Failed_C2;
                    opCamera_M_Total.Value = Globalvariable.GCounter.Total_C2;

                    lblPass.Value = PLC_Comfirm.Curent_Pass;
                    lblTotal.Value = PLC_Comfirm.Curent_Total;
                    lblFail.Value = PLC_Comfirm.Curent_Fail;
                });
            }
            // Cập nhật các trường thông tin MFI trên giao diện
           
        }

        #region Các cập nhật lên màn hình
        //Gửi lên màn hình và lưu log
        public void LogUpdate(string message)
        {
             this.InvokeIfRequired (() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: {message}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            });
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

                if (GCamera.Camera_Status_02 == e_Camera_Status.DISCONNECTED)
                {
                    opCMR02Stt.FillColor = Globalvariable.WB_Color;
                }
                //Ready
                if (Globalvariable.All_Ready)
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


                Active_Pr();

                SafeInvoke(() =>
                {
                    lblPass.Text = "-1";
                    lblTotal.Text = "-1";
                    lblFail.Text = "-1";
                    opCamera_M_Pass.Text = "-1";
                });


                Update_HMI();
                Thread.Sleep(200);
            }
        }

        public void Active_Pr()
        {
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
                        Globalvariable.ACTIVE_C2 = true;
                    }
                }
                else
                {
                    if (Globalvariable.ACTIVE_C2 == true)
                    {
                        Globalvariable.ACTIVE_C2 = false;
                    }

                }
            }

            if (Globalvariable.ACTIVE_C1 && Globalvariable.ACTIVE_C2)
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
        }

        public void AddHISTORY()
        {
             this.InvokeIfRequired (() =>
            {
                opHis2.Items.Add($"#{GV.ID}:{DateTime.Now.ToString("HH:mm:ss.fff")} : {Globalvariable.C2_UI.Curent_Content}");
                opHis2.SelectedIndex = opHis2.Items.Count - 1;
                if (opHis2.Items.Count > 100)
                {
                    opHis2.Items.RemoveAt(0); // Giữ số lượng mục trong danh sách không vượt quá 1000
                }
            });
        }

        //Cập nhật mã vừa đọc lên màn hình
        private void WK_Update_Result_To_UI_DoWork(object sender, DoWorkEventArgs e)
        {
            int lastShowID = 0;
            while (!WK_UI_CAM_Update.CancellationPending)
            {
                this.InvokeIfRequired (() =>
                {
                    //opContentC1.Text = Globalvariable.C1_UI.Curent_Content;
                    if (GV.ID != lastShowID)
                    {
                        AddHISTORY();

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
                });
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

        //camera 01
        long lastRecive1 = 0;
        string lastData1 = "";
        private void Camera_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                         this.InvokeIfRequired (() =>
                        {
                            opCamera.Text = "Sẵn sàng";
                            opCamera.FillColor = Globalvariable.OK_Color;
                        });
                    }
                    break;
                case SPMS1.enumClient.DISCONNECTED:
                    if (GCamera.Camera_Status != e_Camera_Status.DISCONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.DISCONNECTED;
                         this.InvokeIfRequired (() =>
                        {
                            opCamera.Text = "Mất kết nối";
                        });
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:

                    var a = DateTime.Now.Ticks - lastRecive1;
                    var b = TimeSpan.TicksPerMillisecond * 100;
                    //tránh gửi trùng liên tiếp 2 lần thời gian hiện tại - lastRecive < 100ms nếu code mới nhận về giống với lần trước
                    if (a < b)
                    {
                        //nếu dữ liệu nhận về giống với lần trước thì không xử lý
                        if (_strData == lastData1)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Chống dội C1");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            });
                            break;
                        }
                    }

                    lastRecive1 = DateTime.Now.Ticks;

                    lastData1 = _strData;

                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                         this.InvokeIfRequired (() =>
                        {
                            opCamera.Text = "Sẵn sàng";
                            opCamera.FillColor = Globalvariable.OK_Color;
                        });
                    }

                    //đếm đủ số chai, gửi PLC
                    if (Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.RUNNING)
                    {
                        Globalvariable.GCounter.Total_C1++;
                        try
                        {
                            if (!WK_CMR1.IsBusy)
                            {
                                WK_CMR1.RunWorkerAsync(_strData);
                            }
                            else if (!WK_CMR2.IsBusy)
                            {
                                WK_CMR2.RunWorkerAsync(_strData);
                            }
                            else if (!WK_CMR3.IsBusy)
                            {
                                WK_CMR3.RunWorkerAsync(_strData);
                            }
                            else
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : Không đủ luồng xử lí");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;

                                //ghi log lỗi
                                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C1", Globalvariable.CurrentUser.Username, "Không đủ luồng xử lí");
                                Send_Result_Content_C1(e_Content_Result.ERROR, "Lỗi khi camera 02 trả về: Không đủ luồng xử lí");
                                //thêm vào Queue để ghi log
                                SystemLogs.LogQueue.Enqueue(systemLogs);
                            }
                        }
                        catch (Exception ex)
                        {
                             this.InvokeIfRequired (() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : {ex.Message}");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            });

                            //ghi log lỗi
                            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C1", Globalvariable.CurrentUser.Username, ex.Message);
                            //thêm vào Queue để ghi log
                            SystemLogs.LogQueue.Enqueue(systemLogs);
                        }
                    }
                    else if (Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.TESTING)
                    {
                        LogUpdate("Đang ở chế độ TESTING, không xử lý dữ liệu camera.");
                    }
                    else
                    {
                        LogUpdate("CHƯA KHỞI ĐỘNG SẢN XUẤT");
                    }

                    break;
                case SPMS1.enumClient.RECONNECT:
                    if (GCamera.Camera_Status != e_Camera_Status.RECONNECT)
                    {
                        GCamera.Camera_Status = e_Camera_Status.RECONNECT;

                         this.InvokeIfRequired (() =>
                        {
                            opCamera.Text = "Kết nối lại";
                        });
                    }
                    
                    
                    break;
            }
        }

        long lastRecive = 0;
        string lastData = "";
        //camera 02
        private void Camera_c_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status_02 != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.CONNECTED;
                         this.InvokeIfRequired (() =>
                        {
                            opCMR02Stt.Text = "Sẵn sàng";
                            opCMR02Stt.FillColor = Globalvariable.OK_Color;
                        });
                    }
                    break;
                case SPMS1.enumClient.DISCONNECTED:
                    if (GCamera.Camera_Status_02 != e_Camera_Status.DISCONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.DISCONNECTED;
                         this.InvokeIfRequired (() =>
                        {
                            opCMR02Stt.Text = "Mất kết nối";
                        });
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:

                    var a = DateTime.Now.Ticks - lastRecive;
                    var b = TimeSpan.TicksPerMillisecond * 100;
                    //tránh gửi trùng liên tiếp 2 lần thời gian hiện tại - lastRecive < 100ms nếu code mới nhận về giống với lần trước
                    if (a < b)
                    {
                        //nếu dữ liệu nhận về giống với lần trước thì không xử lý
                        if (_strData == lastData)
                        {
                             this.InvokeIfRequired (() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Chống dội");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            });
                            break;
                        }
                    }

                    lastRecive = DateTime.Now.Ticks;

                    lastData = _strData;

                     this.InvokeIfRequired (() =>
                    {
                        ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: {_strData}");
                        ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                    });

                    

                    if (Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.RUNNING)
                    {
                        if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                        {
                            GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                             this.InvokeIfRequired (() =>
                            {
                                opCamera.Text = "Sẵn sàng";
                                opCamera.FillColor = Globalvariable.OK_Color;
                            });
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
                             this.InvokeIfRequired (() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera 02 trả về : Không đủ luồng xử lí");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            });

                            Send_Result_Content_C2(e_Content_Result.ERROR, "Lỗi khi camera 02 trả về: Không đủ luồng xử lí");

                            //ghi log lỗi
                            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C2", Globalvariable.CurrentUser.Username, "Không đủ luồng xử lí");
                            //thêm vào Queue để ghi log
                            SystemLogs.LogQueue.Enqueue(systemLogs);
                        }
                    }
                    else if (Globalvariable.All_Ready && GV.Production_Status == e_Production_Status.TESTING)
                    {
                        if (_strData == "FAIL")
                        {
                            //truyền Fail xuống PLC
                            OperateResult write = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));
                            return;
                        }
                        else
                        {

                            //truyền pass xuống PLC
                            OperateResult write = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("1"));
                        }

                    }
                    else
                    {
                        LogUpdate("CHƯA KHỞI ĐỘNG SẢN XUẤT");
                    }
                    break;
                case SPMS1.enumClient.RECONNECT:

                     this.InvokeIfRequired (() =>
                    {
                        opCMR02Stt.Text = "Kết nối lại";
                        opCMR02Stt.FillColor = Color.Red;
                    });

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
                //Đi thẳng sản phẩm
               Send_Result_Content_C1(e_Content_Result.EMPTY, "MÃ RỖNG");
                return;
            }
            if (_strData == "FAIL")
            {
                //Đi thẳng luôn
                Send_Result_Content_C1(e_Content_Result.FAIL, "KHÔNG ĐỌC ĐƯỢC");
                return;
            }
            //đổi <GS> về đúng ký tự thật
            _strData = _strData.Replace("<GS>", "\u001D").Replace("<RS>", "\u001E").Replace("<US>", "\u001F");
            //kiểm tra chuỗi có tồn tại trong bể dữ liệu chính hay không
            if (GV.C2_CodeData_Dictionary.TryGetValue(_strData, out CodeData C1CodeData))
            {
                //nếu chưa kích hoạt thì kích hoạt
                if (C1CodeData.Status == "0")
                {
                    //C1CodeData.ID = GV.ID;
                    //Chưa kích hoạt báo lỗi
                    Send_Result_Content_C1(e_Content_Result.FAIL, _strData);
                    return;
                }
                //nếu đã kích hoạt
                else
                {
                    //thêm vào thùng, cập nhật lại mã thùng
                    Send_Result_Content_C1(e_Content_Result.PASS, _strData);
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
            try
            {
                //Xử lý dữ liệu nhanh nhất có thể
                //Kích hoạt hệ thống đo đạc
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //tăng tổng số Camera 02 đã nhận
                Globalvariable.GCounter.Total_C2++;
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
                    Send_Result_Content_C2(e_Content_Result.FAIL, "KHÔNG ĐỌC ĐƯỢC");
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

                            while (true)
                            {
                                //đợi cho đến khi có giá trị
                                //nếu có giá trị thì lấy Status_s
                                PLC_Comfirm.PLC_Total_Status_Dictionary.TryGetValue(Globalvariable.GCounter.Total_C2, out string Status_s);

                                if (Status_s == "PASS")
                                {
                                    //pass
                                    C2CodeData.Status = "1";
                                    //giờ kích hoạt theo ISO
                                    C2CodeData.Activate_Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    //chuyển productionDate dạng string về UTC
                                    C2CodeData.Production_Datetime = GV.Selected_PO.productionDate;
                                    //Gửi vào hàng chờ để cập nhật SQLite
                                    GV.C2_Update_Content_To_SQLite_Queue.Enqueue(C2CodeData);
                                    //Ghi thành công
                                    Send_Result_Content_C2(e_Content_Result.PASS, _strData);
                                    return;
                                }
                                else if (Status_s == "FAIL")
                                {
                                    //fail
                                    //Gửi xuống PLC fail được tính là fail trong csdl luôn
                                    C2CodeData.Status = "-1";
                                    //giờ kích hoạt theo ISO
                                    C2CodeData.Activate_Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    C2CodeData.Production_Datetime = GV.Selected_PO.productionDate;
                                    //Gửi vào hàng chờ để cập nhật SQLite
                                    GV.C2_Update_Content_To_SQLite_Queue.Enqueue(C2CodeData);
                                    //Ghi thất bại
                                    Send_Result_Content_C2(e_Content_Result.ERROR, _strData);

                                    break;
                                }
                                else if (Status_s == "TIMEOUT")
                                {
                                    //Gửi xuống PLC fail được tính là fail trong csdl luôn
                                    C2CodeData.Status = "-1";
                                    //giờ kích hoạt theo ISO
                                    C2CodeData.Activate_Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    C2CodeData.Production_Datetime = GV.Selected_PO.productionDate;
                                    //Gửi vào hàng chờ để cập nhật SQLite
                                    GV.C2_Update_Content_To_SQLite_Queue.Enqueue(C2CodeData);
                                    //Ghi thất bại
                                    Send_Result_Content_C2(e_Content_Result.ERROR, _strData);
                                    break;
                                }
                                else
                                {
                                    //nếu không có giá trị thì đợi 100ms
                                    //để tránh treo ứng dụng
                                    Thread.Sleep(5);
                                }
                            }
                        }
                        else
                        {
                            Globalvariable.GCounter.PLC_1_Fail_C2++;
                            //Gửi xuống PLC fail được tính là fail trong csdl luôn
                            C2CodeData.Status = "-1";
                            //giờ kích hoạt theo ISO
                            C2CodeData.Activate_Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            C2CodeData.Production_Datetime = GV.Selected_PO.productionDate;
                            //Gửi vào hàng chờ để cập nhật SQLite
                            GV.C2_Update_Content_To_SQLite_Queue.Enqueue(C2CodeData);
                            //Ghi thất bại
                            Send_Result_Content_C2(e_Content_Result.ERROR, _strData);
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
            catch (Exception ex)
            {
                 this.InvokeIfRequired (() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera 02 trả về : {ex.Message}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });
                //ghi log lỗi
                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C2", Globalvariable.CurrentUser.Username, ex.Message);
                //thêm vào Queue để ghi log
                SystemLogs.LogQueue.Enqueue(systemLogs);
            }
            
        }

        #region Quản lý PLC và gửi tín hiệu PLC

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
                            orderNo = GV.Selected_PO.orderNo.ToString(),
                            Status = content_Result.ToString(),
                            PLC_Send_Status = "true",
                            Activate_Datetime = DateTime.Now.ToString("o"),
                            Production_Datetime = GV.Selected_PO.productionDate
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
                        orderNo = GV.Selected_PO.orderNo.ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write1.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("o"),
                        Production_Datetime = GV.Selected_PO.productionDate
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
                        orderNo = GV.Selected_PO.orderNo.ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write5.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("o"),
                        Production_Datetime = GV.Selected_PO.productionDate
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
                        orderNo = GV.Selected_PO.orderNo.ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write4.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("o"),
                        Production_Datetime = GV.Selected_PO.productionDate
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
                        orderNo = GV.Selected_PO.orderNo.ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write3.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("o"),
                        Production_Datetime = GV.Selected_PO.productionDate
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
                        orderNo = GV.Selected_PO.orderNo.ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write2.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("o"),
                        Production_Datetime = GV.Selected_PO.productionDate
                    };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;

                case e_Content_Result.NOT_FOUND:
                    
                    Globalvariable.C2_UI.Curent_Content = $"Mã không tồn tại : {_content}";
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Empty_C2++;
                    Globalvariable.GCounter.Total_Failed_C2 += 1;
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
                        orderNo = GV.Selected_PO.orderNo.ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = write8.IsSuccess.ToString(),
                        Activate_Datetime = DateTime.Now.ToString("o"),
                        Production_Datetime = GV.Selected_PO.productionDate.ToString()
                    };
                    GV.C2_Save_Result_To_SQLite_Queue.Enqueue(dataResultSave);
                    break;

                case e_Content_Result.ERROR:
                    Globalvariable.C2_UI.Curent_Content = "Lỗi không xác định";
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Error_C2++;
                    Globalvariable.GCounter.Total_Failed_C2++;

                    dataResultSave = new DataResultSave
                    {
                        ID = GV.ID, // Fix: Change the property type in DataResultSave class to string instead of int.  
                        Code = _content,
                        orderNo = GV.Selected_PO.orderNo.ToString(),
                        Status = content_Result.ToString(),
                        PLC_Send_Status = "FAIL-1",
                        Activate_Datetime = DateTime.Now.ToString("o"),
                        Production_Datetime = GV.Selected_PO.productionDate
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
                     this.InvokeIfRequired (() =>
                    {
                        opPLCStatus.Text = "Kết nối";
                        opPLCStatus.FillColor = Globalvariable.OK_Color;
                    });
                    break;
                case SPMS1.OmronPLC_Hsl.PLCStatus.Disconnect:
                    Globalvariable.PLCConnect = false;
                     this.InvokeIfRequired (() =>
                    {
                        opPLCStatus.Text = "Mất kết nối";
                        opPLCStatus.FillColor = Globalvariable.NG_Color;
                    });
                    break;
            }
        }
        #endregion

        public void Update_Active_Status(CodeData _CodeItem)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/.ABC/{GV.Selected_PO.orderNo.ToString()}.db;Version=3;"))
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
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/.ABC/{GV.Selected_PO.orderNo.ToString()}.db;Version=3;"))
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
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/.ABC/{GV.Selected_PO.orderNo.ToString()}.db;Version=3;"))
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
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/.ABC/Record_{GV.Selected_PO.orderNo.ToString()}.db;Version=3;"))
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

                    #region AWS
                    if(Setting.Current.AWS_ENA)
                    {
                        DataTable dataTable = new DataTable();
                        dataTable = Get_Unique_Codes_Run_Send_Pending(GV.Selected_PO.orderNo.ToString());

                        //gửi mã
                        if (dataTable.Rows.Count > 0)
                        {
                            //lấy ra mã và gửi đi
                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                string orderNO = GV.Selected_PO.orderNo.ToString();
                                string ID = dataTable.Rows[i]["ID"].ToString();
                                string code = dataTable.Rows[i]["Code"].ToString();
                                string Status = dataTable.Rows[i]["Status"].ToString();
                                string activateDate = dataTable.Rows[i]["ActivateDate"].ToString();
                                string productionDate = dataTable.Rows[i]["ProductionDate"].ToString();
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

                        //kiểm tra danh sách các mã đã gửi đi
                        if (GV.AWS_Response_Queue.Count > 0)
                        {
                            //lấy ra dữ liệu
                            var responseItem = GV.AWS_Response_Queue.Dequeue();
                            string ID = responseItem.message_id.Split('-')[0];
                            //cập nhật trạng thái đã gửi
                            try
                            {
                                Update_Recive_Status(ID.ToInt32(), responseItem.status);
                            }
                            catch (Exception ex)
                            {
                                //ghi log
                                AWSLogs aWSLogs = new AWSLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_AWSLogType.ERROR, "Lỗi khi nhận tin nhắn trả về", Globalvariable.CurrentUser.Username, ex.Message);
                                //thêm vào Queue để ghi log
                                AWSLogsQueue.Enqueue(aWSLogs);
                            }

                        }

                        //cập nhật số lượng đã gửi
                        GV.Sent_Count = Get_Unique_Codes_Run_Send_Count(GV.Selected_PO.orderNo.ToString());
                        //cập nhật số lượng đã nhận
                        GV.Received_Count = Get_Unique_Codes_Run_Recive_Count(GV.Selected_PO.orderNo.ToString());

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
                        dataTableFailed = Get_Unique_Codes_Run_Send_Failed(GV.Selected_PO.orderNo.ToString());
                        if (dataTableFailed.Rows.Count > 0)
                        {
                            //lấy ra mã và gửi đi
                            for (int i = 0; i < dataTableFailed.Rows.Count; i++)
                            {
                                string orderNO = GV.Selected_PO.orderNo.ToString();
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
                    #endregion

                }

                Thread.Sleep(100);
            }
        }
        public DataTable Get_Unique_Codes_Run_Send_Pending(string orderNo)
        {
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

        //xử lý trạng thái phần mềm theo PO
        private void WK_PO_DoWork(object sender, DoWorkEventArgs e)
        {
            int dem = 50;
            while(!WK_PO.CancellationPending)
            {
                dem++;
                switch (GV.Production_Status)
                {
                    case e_Production_Status.EDITING:
                        OperateResult writeStartdd = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.DATE_EDITING:
                        OperateResult writeStartvc = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.PLC_NEW_PO:
                        //xóa số đếm PLC
                        OperateResult writeClear = PLC.plc.Write(PLCAddress.Get("PLC_Reset_Counter_DM_C2"), 1);
                        //xóa số đếm PLC 
                        OperateResult writeClear1 = PLC.plc.Write(PLCAddress.Get("RESET_COUNT_DM_SS1"), 1);

                        OperateResult writeOrderQty = PLC.plc.Write(PLCAddress.Get("PLC_ORDERQTY_DM"), GV.Selected_PO.orderQty.ToInt32());
                        //chuyển sang Ready
                        GV.Production_Status = e_Production_Status.RUNNING;

                        break;
                    case e_Production_Status.STOPPED:
                        OperateResult writeStartsssssss = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.RUNNING:
                        OperateResult writeStart = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 1);//gửi lệnh bắt đầu
                        break;
                    case e_Production_Status.PAUSED:
                        OperateResult writeStartsssss = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.READY:
                        OperateResult writeStartssssss = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.NOPO:
                        OperateResult writeStartssss = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.STARTUP:
                        OperateResult writeStartsss= PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.LOAD:
                        OperateResult writeStartss = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.CHECKING:
                        OperateResult writeStartd = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.COMPLETE:
                       // OperateResult writeStartsad = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                    case e_Production_Status.TESTING:
                        //gửi lệnh bắt đầu và số lượng sản xuất = 10 để test
                        
                        OperateResult writeStart1 = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 1);
                        OperateResult writeT = PLC.plc.Write(PLCAddress.Get("PLC_ORDERQTY_DM"), 10);
                        break;
                    case e_Production_Status.FINALTESTING:
                        //dừng chạy kích đủ PO
                        OperateResult writeStart2 = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        //xóa số đếm PLC 
                        OperateResult writeClear11 = PLC.plc.Write(PLCAddress.Get("RESET_COUNT_DM_SS1"), 1);
                        //chuyển sang Ready
                        GV.Production_Status = e_Production_Status.READY;
                        break;
                    case e_Production_Status.PLC_CON_PO:
                        //gửi số lượng order xuống
                        OperateResult writeOrderQtyu = PLC.plc.Write(PLCAddress.Get("PLC_ORDERQTY_DM"), GV.Selected_PO.orderQty.ToInt32());
                        OperateResult writeStart3 = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 1);//gửi lệnh bắt đầu
                        
                        GV.Production_Status = e_Production_Status.RUNNING;
                        break;
                    case e_Production_Status.UNKNOWN:
                        OperateResult writeStart11 = PLC.plc.Write(PLCAddress.Get("ENA_START_PO_DM"), 0);
                        break;
                }

                if (dem > 50)
                {
                    dem = 0;
                }
                Thread.Sleep(100);
            }
        }

        #region Xử lý PLC Comfirm
        private async Task PLC_Comfirm_Async()
        {
            
            // Bắt đầu nhiệm vụ
            try
            {
                while (!GTask.Task_PLC_Comfirm.Token.IsCancellationRequested)
                {
                    if (GV.Production_Status == e_Production_Status.RUNNING)
                    {
                        // đọc PLC
                        //đọc từ PLC
                        OperateResult<int[]> readCount = PLC.plc.ReadInt32("D130", 5);
                        if (readCount.IsSuccess)
                        {
                            PLC_Comfirm.Curent_Pass = readCount.Content[2].ToString().ToInt32();
                            PLC_Comfirm.Curent_Total = readCount.Content[0].ToString().ToInt32();
                            PLC_Comfirm.Curent_Fail = readCount.Content[1].ToString().ToInt32();
                            PLC_Comfirm.Curent_Timeout = readCount.Content[4].ToString().ToInt32();
                        }

                        //kiểm tra xem biến nào lệch thì nhảy sự kiện biến đó
                        if (PLC_Comfirm.Curent_Total != PLC_Comfirm.Last_Total)
                        {
                            //kiểm tra từng cái xem cái nào khác
                            if (PLC_Comfirm.Curent_Pass != PLC_Comfirm.Last_Pass)
                            {
                                PLC_Comfirm.PLC_Total_Status_Dictionary[PLC_Comfirm.Curent_Total] = "PASS";
                            }
                            if (PLC_Comfirm.Curent_Fail != PLC_Comfirm.Last_Fail)
                            {
                                PLC_Comfirm.PLC_Total_Status_Dictionary[PLC_Comfirm.Curent_Total] = "FAIL";
                            }
                            if (PLC_Comfirm.Curent_Timeout != PLC_Comfirm.Last_Timeout)
                            {
                                PLC_Comfirm.PLC_Total_Status_Dictionary[PLC_Comfirm.Curent_Total] = "TIMEOUT";
                            }
                            // cập nhật lại giá trị cuối cùng
                            PLC_Comfirm.Last_Pass = PLC_Comfirm.Curent_Pass;
                            PLC_Comfirm.Last_Fail = PLC_Comfirm.Curent_Fail;
                            PLC_Comfirm.Last_Total = PLC_Comfirm.Curent_Total;
                            PLC_Comfirm.Last_Timeout = PLC_Comfirm.Curent_Timeout;
                        }
                    }
                    
                    await Task.Delay(20, GTask.Task_PLC_Comfirm.Token);
                }
            }
            catch (TaskCanceledException) { }
        }
        private void StartTask()
        {
            Task.Run(PLC_Comfirm_Async, GTask.Task_PLC_Comfirm.Token);
        }
        #endregion
    }
}