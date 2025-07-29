using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Utils;
using Newtonsoft.Json.Linq;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.AWS
{

    public partial class PAws : UIPage
    {
        public AwsIotClientHelper awsClient;
        //khai báo log 
        private LogHelper<e_LogType> AWSLog;

        public PAws()
        {
            InitializeComponent();
        }

        public void INIT()
        {
            //khởi tạo log
            AWSLog = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "Pages", "AWSlog.ptl"));
            Connect_AWS();
        }

        private void Connect_AWS()
        {
            if (AppConfigs.Current.AWS_ENA)
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
                Task.Run(() =>
                {
                    try
                    {
                        awsClient.ConnectAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        AWSLog.WriteLogAsync(Globals.CurrentUser.Username,e_LogType.Error, $"🔴 [{DateTime.Now}] Lỗi kết nối AWS: {ex.Message}");
                        this.InvokeIfRequired(() =>
                        {
                           
                            opConsole.Items.Insert(0, $"🔴 [{DateTime.Now}] Lỗi kết nối AWS: {ex.Message}");

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
            //thêm vào queue
            //GV.AWS_Response_Queue.Enqueue(new AWS_Response
            //{
            //    status = status,
            //    message_id = messageId,
            //    error_message = errorMessage
            //});

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
    }
}
