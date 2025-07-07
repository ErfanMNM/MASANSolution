using HslCommunication.Profinet.Omron;
using iTextSharp.text.pdf.qrcode;
using SPMS1.MQTT;
using SpT;
using Sunny;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        }
    }
}
