using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using Newtonsoft.Json.Linq;
using SPMS1;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                        AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"🔴 [{DateTime.Now}] Lỗi kết nối AWS: {ex.Message}");
                        this.InvokeIfRequired(() =>
                        {
                            //opNotiboardAndSend.Items.Insert(0, $"🔴 [{DateTime.Now}] Lỗi kết nối AWS: {ex.Message}");
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
        }

        private void AWS_Status_Onchange(object sender, AwsIotClientHelper.AWSStatusEventArgs e)
        {
            switch (e.Status)
            {
                case AwsIotClientHelper.e_awsIot_status.Connected:
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
                case AwsIotClientHelper.e_awsIot_status.Disconnected:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Warning, "Đã Ngắt kết nối AWS");
                    this.InvokeIfRequired(() =>
                    {
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã ngắt kết nối AWS IoT Core. Đang thử kết nối lại...");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Connecting:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Đang kết nốilại AWS");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Đang kết nối lại với AWS IoT Core...");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Error:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lỗi kết nối AWS: {e.Message}");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi kết nối AWS IoT Core: {e.Message}");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Subscribed:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Đã đăng ký các topic thành công");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã đăng ký các topic thành công.");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Unsubscribed:
                    //ghi log
                    AWSIoTLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Warning, "Đã hủy đăng ký các topic");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opNotiboardAndSend.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã hủy đăng ký các topic.");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Published:

                    break;
                case AwsIotClientHelper.e_awsIot_status.Unpublished:
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
        }

        private void btnSendFailed_Click(object sender, EventArgs e)
        {

        }

        private void btnGetSendPending_Click(object sender, EventArgs e)
        {
            TResult getCodeSend = Globals.ProductionData.getDataPO.Get_Codes_Send(Globals.ProductionData.orderNo);

            this.InvokeIfRequired(() =>
            {
                opNotiboardAndSend.Items.Insert(0, getCodeSend.message);
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

        }
    }
}
