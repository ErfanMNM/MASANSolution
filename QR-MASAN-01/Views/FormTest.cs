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

        private void uiSymbolButton3_Click(object sender, EventArgs e)
        {
            string filePath = @"C:\Users\THUC\Downloads\08936086140878010725BMIP01.csv";
            string value = ReadCsvGetLine2Col3(filePath);


            string encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(uniqueCode));
            var payload = new
            {
                message_id = Guid.NewGuid().ToString(),
                orderNo = "ORDER20250708-001",
                uniqueCode = value,
                status = 1,
                activate_datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"),
                production_date = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"),
                thing_name = "MIPWP501"
            };

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

        private void uiSymbolButton2_Click(object sender, EventArgs e)
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
            

            awsClient.OnStatusChanged += (msg) =>
            {
                if (uiListBox1.InvokeRequired)
                    uiListBox1.Invoke(new Action(() => uiListBox1.Items.Add(msg)));
                else
                    uiListBox1.Items.Add(msg);
            };

            awsClient.OnMessageReceived += (msg) =>
            {
                if (uiListBox1.InvokeRequired)
                    uiListBox1.Invoke(new Action(() => uiListBox1.Items.Add(msg)));
                else
                    uiListBox1.Items.Add(msg);
            };

            awsClient.Connect();
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
                                            "CZ/MIPWP501/response",
                                            "CZ/data"
                                        };

            awsClient.SubscribeMultiple(topicsToSub);
        }
    }
}
