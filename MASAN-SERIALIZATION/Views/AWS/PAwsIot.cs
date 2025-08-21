using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SPMS1;
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
using static AwsIotClientHelper;
using static System.Net.Mime.MediaTypeNames;

namespace MASAN_SERIALIZATION.Views.AWS
{
    public partial class PAwsIot : UIPage
    {
        public PAwsIot()
        {
            InitializeComponent();
        }
        public AwsIotClientHelper awsClient;
        //khai báo log 
        private LogHelper<e_LogType> AWSIoTLog;

        private BackgroundWorker bgw_send;
        private BackgroundWorker bgw_process;

        private DataTable dtSends;
        private DataTable dtResend;
        private DataTable dtTimeout;

        private void Connect_AWS()
        {
            if (AppConfigs.Current.AWS_ENA)
            {
                try
                {
                    //kết nối MQTT
                    string host = AppConfigs.Current.host;
                    string clientId = AppConfigs.Current.clientId;
                    string rootCAPath = AppConfigs.Current.rootCAPath;
                    string pfxPath = AppConfigs.Current.pfxPath;
                    string pfxPassword = AppConfigs.Current.pfxPassword;

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
                }
                catch (Exception ex)
                {
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"🔴 [{DateTime.Now}] Lỗi cấu hình AWS: {ex.Message}");
                    this.InvokeIfRequired(() =>
                    {
                        opNotiboardAndSend.Items.Insert(0, $"🔴 [{DateTime.Now}] Lỗi cấu hình AWS: {ex.Message}");
                    });
                    return;
                }
                

                Task.Run(() =>
                {
                    try
                    {
                        awsClient.ConnectAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"🔴 [{DateTime.Now}] Lỗi kết nối AWS: {ex.Message}");
                        this.InvokeIfRequired(() =>
                        {
                            opNotiboardAndSend.Items.Insert(0, $"🔴 [{DateTime.Now}] Lỗi kết nối AWS: {ex.Message}");
                        });

                    }
                });
            }

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

            this.InvokeIfRequired(() =>
            {
                //cập nhật giao diện
                opRecive.Items.Insert(0, $"{DateTime.Now:HH:mm:ss}: Nhận dữ liệu từ AWS IoT Core: {e.Topic} - {e.Payload}");
            });

            AWS_Recive_Data aWS_R_Data = new AWS_Recive_Data
            {
                ID = messageId.Split('-')[0].ToInt32(),
                recive_Status = status,
            };
            //thêm vào hàng chờ xử lý cập nhật trạng thái
            Globals_Database.aWS_Recive_Datas.Enqueue(aWS_R_Data);
            
            // Ghi Receive Record vào database
            Insert_AWS_Receive_Record(messageId, status, errorMessage);
        }

        private void AWS_Status_Onchange(object sender, AWSStatusEventArgs e)
        {
            Globals.AWS_IoT_Status = e.Status; //cập nhật trạng thái kết nối AWS IoT
            switch (e.Status)
            {
                case e_awsIot_status.Connected:
                    // ghi log
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Đã Kết nối AWS");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết nối thành công với AWS IoT Core.");
                    });

                    string[] topicsToSub = new[]
                                      {
                                            "CZ/MIPWP501/response"
                                        };

                    awsClient.SubscribeMultiple(topicsToSub);

                    break;
                case e_awsIot_status.Disconnected:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Warning, "Đã Ngắt kết nối AWS");
                    this.InvokeIfRequired(() =>
                    {
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã ngắt kết nối AWS IoT Core. Đang thử kết nối lại...");
                    });
                    break;
                case e_awsIot_status.Connecting:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Đang kết nốilại AWS");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Đang kết nối lại với AWS IoT Core...");
                    });
                    break;
                case e_awsIot_status.Error:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lỗi kết nối AWS: {e.Message}");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi kết nối AWS IoT Core: {e.Message}");
                    });
                    break;
                case e_awsIot_status.Subscribed:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Đã đăng ký các topic thành công");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã đăng ký các topic thành công.");
                    });
                    break;
                case e_awsIot_status.Unsubscribed:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Warning, "Đã hủy đăng ký các topic");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã hủy đăng ký các topic.");
                    });
                    break;
                case e_awsIot_status.Published:

                    break;
                case e_awsIot_status.Unpublished:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Warning, "Không thể publish dữ liệu");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Không thể publish dữ liệu, chưa kết nối.");
                    });
                    break;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect_AWS();
            bgw_send = new BackgroundWorker();
            bgw_send.WorkerSupportsCancellation = true;
            bgw_send.DoWork += Bgw_send_DoWork;
        }

        private void Bgw_send_DoWork(object sender, DoWorkEventArgs e)
        {
            if (dtSends.Rows.Count > 0)
            {
                AWS_Send_Datatable(dtSends);
            }

            if (dtResend.Rows.Count > 0)
            {
                AWS_Send_Datatable(dtResend);
            }

            if(dtTimeout.Rows.Count > 0)
            {
                AWS_Send_Datatable(dtTimeout);
            }

        }

        private void btnSendFailed_Click(object sender, EventArgs e)
        {
            TResult getCodeResend = Globals.ProductionData.getDataPO.Get_Codes_Send_Failed(Globals.ProductionData.orderNo);
            if (getCodeResend.issuccess)
            {
                dtResend = getCodeResend.data;
            }
            this.InvokeIfRequired(() =>
            {
                opNotiboardAndSend.Items.Insert(0, getCodeResend.message);
            });

        }

        private void btnGetSendPending_Click(object sender, EventArgs e)
        {
            TResult getCodeSend = Globals.ProductionData.getDataPO.Get_Codes_Send(Globals.ProductionData.orderNo);

            if (getCodeSend.issuccess)
            {
                dtSends = getCodeSend.data;
            }

            this.InvokeIfRequired(() =>
            {
                opNotiboardAndSend.Items.Insert(0, "Số mã :" + getCodeSend.count+ getCodeSend.message);
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            // run task hàm AWS_Send_Datatable
            if(!bgw_send.IsBusy)
            {
                bgw_send.RunWorkerAsync();
            }
        }

        private void AWS_Send_Datatable(DataTable SendCodes)
        {
            for(int i = 0; i < SendCodes.Rows.Count; i++)
            {
                Thread.Sleep(300);
                string orderNO = Globals.ProductionData.orderNo;
                string ID = SendCodes.Rows[i]["ID"].ToString();
                string code = SendCodes.Rows[i]["Code"].ToString();
                string Status = SendCodes.Rows[i]["Status"].ToString();
                string activateDate = SendCodes.Rows[i]["ActivateDate"].ToString();
                string productionDate = SendCodes.Rows[i]["ProductionDate"].ToString();
                string cartonCode = SendCodes.Rows[i]["cartonCode"].ToString();
                //tạo dữ liệu gửi
                AWSSendPayload payload = new AWSSendPayload
                {
                    message_id = $"{ID}-{orderNO}-{DateTime.Now.ToString("o")}",
                    orderNo = orderNO,
                    uniqueCode = code,
                    cartonCode = cartonCode,
                    status = Status.ToInt32(),
                    activate_datetime = activateDate,
                    production_date = productionDate,
                    thing_name = "MIPWP501"
                };
                string json = JsonConvert.SerializeObject(payload);
                var rs = awsClient.Publish_V2("CZ/data", json);

                if (rs.Issuccess)
                {
                    AWS_Send_Data aWS_Send_Data = new AWS_Send_Data
                    {
                        ID = ID.ToInt32(),
                        send_Status = e_AWS_Send_Status.Sent.ToString(),
                    };
                    Globals_Database.aWS_Send_Datas.Enqueue(aWS_Send_Data);
                    
                    // Ghi Send Record vào database
                    Insert_AWS_Send_Record(payload.message_id, payload.orderNo, payload.uniqueCode, 
                                         payload.status.ToString(), payload.activate_datetime, 
                                         payload.production_date, payload.thing_name);

                    //ghi dữ liệu đã gửi vào file txt
                    

                    //cập nhật trạng thái đã gửi
                    this.InvokeIfRequired(() =>
                    {
                        opNotiboardAndSend.Items.Insert(0, $"✅ [{DateTime.Now}] Đã gửi dữ liệu thành công: {json}");
                    });
                }
                else
                {
                    AWS_Send_Data aWS_Send_Data = new AWS_Send_Data
                    {
                        ID = ID.ToInt32(),
                        send_Status = e_AWS_Send_Status.Failed.ToString(),
                    };

                    Globals_Database.aWS_Send_Datas.Enqueue(aWS_Send_Data);
                    //ghi log lỗi không gửi được
                    this.InvokeIfRequired(() =>
                    {
                        opNotiboardAndSend.Items.Insert(0, $"❌ [{DateTime.Now}] Lỗi gửi dữ liệu: {rs.msg}");
                    });
                }
            }
        }

        private void opNotiboardAndSend_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog("Thông báo", opNotiboardAndSend.SelectedItem.ToString());
        }

        private void PAwsIot_Initialize(object sender, EventArgs e)
        {
           //ghi log bật page
           Globals.Log.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.UserAction, $"🔵 [{DateTime.Now}] Đã mở trang AWS IoT");
        }

        private void PAwsIot_Load(object sender, EventArgs e)
        {
            AWSIoTLog = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "Pages", "AWSlog.ptl"));

            if (AppConfigs.Current.Auto_Send_AWS)
            {
                //nếu bật AWS thì kết nối
                Connect_AWS();

                //khởi tạo background worker để gửi dữ liệu
                bgw_process = new BackgroundWorker();
                bgw_send = new BackgroundWorker();

                bgw_process.WorkerSupportsCancellation = true;
                bgw_process.DoWork += Bgw_process_DoWork;

                bgw_process.RunWorkerAsync();

                bgw_send = new BackgroundWorker();
                bgw_send.WorkerSupportsCancellation = true;
                bgw_send.DoWork += Bgw_send_DoWork;
                
            }
        }

        private void Bgw_process_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!bgw_process.CancellationPending)
            {
                TResult getCodeSend = Globals.ProductionData.getDataPO.Get_Codes_Send(Globals.ProductionData.orderNo);

                if (getCodeSend.issuccess)
                {
                    dtSends = getCodeSend.data;
                }

                TResult getCodeResend = Globals.ProductionData.getDataPO.Get_Codes_Send_Failed(Globals.ProductionData.orderNo);
                if (getCodeResend.issuccess)
                {
                    dtResend = getCodeResend.data;
                }

                TResult getCodeTimeOut = Globals.ProductionData.getDataPO.Get_Codes_Sent_Timeout(Globals.ProductionData.orderNo);

                if (getCodeTimeOut.issuccess)
                {
                    dtTimeout = getCodeTimeOut.data;
                }

                this.InvokeIfRequired(() =>
                {
                    //cập nhật giao diện
                    opNotiboardAndSend.Items.Insert(0, $"🔄 [{DateTime.Now}] Đã kiểm tra dữ liệu gửi AWS.");
                    if (opNotiboardAndSend.Items.Count > 50)
                    {
                        // Giới hạn số lượng mục hiển thị trong opNotiboardAndSend
                        opNotiboardAndSend.Items.RemoveAt(opNotiboardAndSend.Items.Count - 1);
                    }

                    if (opRecive.Items.Count > 50)
                    {
                        // Giới hạn số lượng mục hiển thị trong opNotiboardAndSend
                        opNotiboardAndSend.Items.RemoveAt(opNotiboardAndSend.Items.Count - 1);
                    }

                });

                if (Globals.AWS_IoT_Status != e_awsIot_status.Disconnected && Globals.AWS_IoT_Status != e_awsIot_status.Connecting )
                {
                    //nếu có dữ liệu gửi thì gửi
                    if (dtSends != null)
                    {
                        AWS_Send_Datatable(dtSends);
                        dtSends = null; // Xóa dữ liệu resend sau khi gửi thành công
                    }

                    if (dtResend != null)
                    {
                        AWS_Send_Datatable(dtResend);
                        dtResend = null; // Xóa dữ liệu resend sau khi gửi thành công
                    }
                }

                Thread.Sleep(10000); // Giữ cho vòng lặp chạy liên tục
            }
        }

        // Thêm Send Record vào database
        public void Insert_AWS_Send_Record(string messageId, string orderNo, string uniqueCode, string status, 
                                          string activateDatetime, string productionDate, string thingName)
        {
            try
            {
                string recordAWSPath = $@"C:/MasanSerialization/PODatabases/Send_AWS_Record_{orderNo}.db";
                using (var conn = new SQLiteConnection($"Data Source={recordAWSPath};Version=3;"))
                {
                    conn.Open();
                    string insertQuery = @"INSERT INTO Records (message_id, orderNo, uniqueCode, status, 
                                         activate_datetime, production_date, thing_name, send_datetime) 
                                         VALUES (@message_id, @orderNo, @uniqueCode, @status, 
                                         @activate_datetime, @production_date, @thing_name, @send_datetime)";
                    
                    using (var command = new SQLiteCommand(insertQuery, conn))
                    {
                        command.Parameters.AddWithValue("@message_id", messageId);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        command.Parameters.AddWithValue("@uniqueCode", uniqueCode);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@activate_datetime", activateDatetime);
                        command.Parameters.AddWithValue("@production_date", productionDate);
                        command.Parameters.AddWithValue("@thing_name", thingName);
                        command.Parameters.AddWithValue("@send_datetime", DateTime.Now.ToString("o"));
                        command.ExecuteNonQuery();
                    }
                }
                
                this.InvokeIfRequired(() =>
                {
                    opNotiboardAndSend.Items.Insert(0, $"📝 [{DateTime.Now}] Đã ghi Send Record: {messageId}");
                });
            }
            catch (Exception ex)
            {
                this.InvokeIfRequired(() =>
                {
                    opNotiboardAndSend.Items.Insert(0, $"❌ [{DateTime.Now}] Lỗi ghi Send Record: {ex.Message}");
                });
            }
        }

        // Thêm Receive Record vào database
        public void Insert_AWS_Receive_Record(string messageId, string status, string errorMessage)
        {
            try
            {
                string orderNo = Globals.ProductionData.orderNo;
                string reciveAWSPath = $@"C:/MasanSerialization/PODatabases/Recive_AWS_Record_{orderNo}.db";
                using (var conn = new SQLiteConnection($"Data Source={reciveAWSPath};Version=3;"))
                {
                    conn.Open();
                    string insertQuery = @"INSERT INTO Records (message_id, status, error_message, recive_datetime) 
                                         VALUES (@message_id, @status, @error_message, @recive_datetime)";
                    
                    using (var command = new SQLiteCommand(insertQuery, conn))
                    {
                        command.Parameters.AddWithValue("@message_id", messageId);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@error_message", errorMessage ?? "");
                        command.Parameters.AddWithValue("@recive_datetime", DateTime.Now.ToString("o"));
                        command.ExecuteNonQuery();
                    }
                }
                
                this.InvokeIfRequired(() =>
                {
                    opRecive.Items.Insert(0, $"📝 [{DateTime.Now}] Đã ghi Receive Record: {messageId} - Status: {status}");
                });
            }
            catch (Exception ex)
            {
                this.InvokeIfRequired(() =>
                {
                    opRecive.Items.Insert(0, $"❌ [{DateTime.Now}] Lỗi ghi Receive Record: {ex.Message}");
                });
            }
        }

        private void opRecive_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog("Thông báo", opRecive.SelectedItem.ToString());
        }

        private void uiTitlePanel7_Click(object sender, EventArgs e)
        {

        }
    }
}
