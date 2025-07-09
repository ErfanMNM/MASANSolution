using HslCommunication.Profinet.Omron;
using iTextSharp.text.pdf.qrcode;
using Newtonsoft.Json;
using SPMS1.MQTT;
using SpT;
using Sunny;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using ZXing;

namespace QR_MASAN_01.Views
{
    public partial class FormTest : UIPage
    {
        public OmronFinsUdpServer omronFinsServer = new OmronFinsUdpServer();
        private AwsIotClientHelper awsClient; // để global giữ kết nối
        public FormTest()
        {
            InitializeComponent();
           
        }

        private void FormTest_Load(object sender, EventArgs e)
        {

        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            string secret = TwoFAHelper.GenerateSecret(20);
            string qrUrl = TwoFAHelper.GenerateQrCodeUri(secret, "admin", "MS");
            Bitmap qrBitmap = GenerateQRCodeBitmap(qrUrl);
            pictureBox1.Image = qrBitmap;
        }

        public static Bitmap GenerateQRCodeBitmap(string text, int width = 200, int height = 200)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 1
                }
            };
            return writer.Write(text);
        }
        int lines = 1;
        private void uiSymbolButton3_Click(object sender, EventArgs e)
        {
            lines++;
            string filePath = @"C:\Users\THUC\Downloads\08936086140878010725BMIP01.csv";
            string value = ReadCsvGetLineCol(filePath, lines);


            //string encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(uniqueCode));
            var payload = new
            {
                message_id = $"{lines}-08936086140878010725BMIP01",
                orderNo = "08936086140878010725BMIP01",
                uniqueCode = value,
                status = 1,
                activate_datetime = DateTime.Now.ToString("o"),
                production_date = "2025-06-30T17:00:00.000Z",
                thing_name = "MIPWP501"
            };
            //var payload = new
            //{
            // message_id =  "1752049307503-nd0akvgon1",
            //  orderNo= "08936086140878010725BMIP01",
            //  uniqueCode= "1752049245939-omj1toftcqa7\x1D91EE11\x1D92z4",
            //  status= 1,
            //  activate_datetime= "2025-06-18 11:04:05.064 +0700",
            //  production_date= "2025-06-18 00:00:00.000 +0700",
            //  thing_name= "MIPWP501",
            //  date_time = "2025-7-9 15:20:45"
            //};
            string json = JsonConvert.SerializeObject(payload);

            awsClient.Publish("CZ/data", json);

        }

        public string ReadCsvGetLine2Col3(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    int currentLine = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        currentLine++;

                        if (currentLine == 2) // dòng thứ 2
                        {
                            var parts = line.Split(',');

                            if (parts.Length >= 3)
                            {
                                return parts[2].Trim(); // cột thứ 3
                            }
                            else
                            {
                                throw new Exception("❌ File không đủ cột.");
                            }
                        }
                    }

                    throw new Exception("❌ File không đủ dòng.");
                }
            }
            catch (Exception ex)
            {
                return $"❌ Lỗi: {ex.Message}";
            }
        }

        public string ReadCsvGetLineCol(string filePath, int lines)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    int currentLine = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        currentLine++;

                        if (currentLine == lines) // dòng thứ 2
                        {
                            var parts = line.Split(',');

                            if (parts.Length >= 3)
                            {
                                return parts[2].Trim(); // cột thứ 3
                            }
                            else
                            {
                                throw new Exception("❌ File không đủ cột.");
                            }
                        }
                    }

                    throw new Exception("❌ File không đủ dòng.");
                }
            }
            catch (Exception ex)
            {
                return $"❌ Lỗi: {ex.Message}";
            }
        }

        private  void uiSymbolButton2_Click(object sender, EventArgs e)
        {
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

        private void AWS_Status_OnReceive(object sender, AwsIotClientHelper.AWSStatusReceiveEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                uiListBox1.Items.Add($"📩 [{DateTime.Now:HH:mm:ss}] Nhận từ topic {e.Topic}: {e.Payload}");
            }));
        }

        void SafeInvoke(Action action)
        {
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }

        private void AWS_Status_Onchange(object sender, AwsIotClientHelper.AWSStatusEventArgs e)
        {
            switch (e.Status)
            {
                case AwsIotClientHelper.e_awsIot_status.Connected:
                    uiListBox1.Items.Add("✅ Đã kết nối AWS IoT Core.");
                    break;
                case AwsIotClientHelper.e_awsIot_status.Disconnected:
                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add("❌ Mất kết nối AWS IoT Core.");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Connecting:
                    uiListBox1.Items.Add("🔄 Đang kết nối AWS IoT Core...");
                    break;
                case AwsIotClientHelper.e_awsIot_status.Error:
                    uiListBox1.Items.Add($"⚠️ Lỗi: {e.Message}");
                    break;
                case AwsIotClientHelper.e_awsIot_status.Subscribed:
                    uiListBox1.Items.Add(e.Message);
                    break;
                case AwsIotClientHelper.e_awsIot_status.Unsubscribed:
                    uiListBox1.Items.Add("❌ Đã hủy đăng ký các topic.");
                    break;
                case AwsIotClientHelper.e_awsIot_status.Published:
                    uiListBox1.Items.Add($"📤 Đã publish: {e.Message}");
                    break;
                case AwsIotClientHelper.e_awsIot_status.Unpublished:
                        SafeInvoke(() =>
                        {
                            uiListBox1.Items.Add($"❌ Không thể publish: {e.Message}");
                        });
                    break;
            }
        }

        private void uiSymbolButton5_Click(object sender, EventArgs e)
        {
            if (awsClient != null)
            {
                awsClient.Disconnect();
                uiListBox1.Items.Add("✅ Đã ngắt kết nối AWS IoT Core.");
            }
        }

        private void uiSymbolButton4_Click(object sender, EventArgs e)
        {
            string[] topicsToSub = new[]
                                        {
                                            "CZ/MIPWP501/response"
                                        };

            awsClient.SubscribeMultiple(topicsToSub);
        }

        private void uiListBox1_DoubleClick(object sender, EventArgs e)
        {
            //show selected item in a message box
            if (uiListBox1.SelectedItem != null)
            {
                string selectedItem = uiListBox1.SelectedItem.ToString();
                MessageBox.Show(selectedItem, "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
