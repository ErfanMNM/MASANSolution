using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace SPMS1
{
    public partial class Label_Printer : Component
    {
        private PrintDocument printDocument;
        public string Printer_Name { get; set; }
        public string BatchNO { get; set; } = "123xyz";
        public string DateM { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
        public int Pallet_Size { get; set; } = 20;
        public string QRContent { get; set; } = "Default Code";
        public string LR { get; set; }  = string.Empty;
        public Label_Printer()
        {
            InitializeComponent();
        }

        public Label_Printer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Print()
        {
            using (PrintDocument printDocument = new PrintDocument())
            {
                printDocument.PrinterSettings.PrinterName = Printer_Name;

                if (printDocument.PrinterSettings.IsValid)
                {
                    printDocument.PrintPage += PrintDocument_PrintPage; ;
                    printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custom", 392, 590); // Kích thước trang in
                    printDocument.Print();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy máy in. Vui lòng kiểm tra lại tên máy in.");
                }
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Vẽ bitmap nhãn lên trang in
            e.Graphics.DrawString($"TƯƠNG ỚT CHINSU", new Font("Arial", 22, FontStyle.Bold), Brushes.Black, new PointF(24, 61));
            e.Graphics.DrawString($"SKU: 250g", new Font("Arial", 22, FontStyle.Bold), Brushes.Black, new PointF(24, 122));
            e.Graphics.DrawString($"SỐ LƯỢNG: {Pallet_Size}", new Font("Arial", 22, FontStyle.Bold), Brushes.Black, new PointF(24, 182));
            e.Graphics.DrawString($"NSX: {DateM}", new Font("Arial", 22, FontStyle.Bold), Brushes.Black, new PointF(24, 242));
            e.Graphics.DrawString($"SỐ LÔ: {BatchNO}", new Font("Arial", 22, FontStyle.Bold), Brushes.Black, new PointF(24, 302));
            e.Graphics.DrawString($"{LR}", new Font("Arial", 14), Brushes.Black, new PointF(24, 352));
            e.Graphics.DrawRectangle(Pens.Black, 10, 10, 590 - 30, 392 - 30); // Vẽ khung viền 
            // Tạo đối tượng BarcodeWriter từ ZXing để tạo mã QR
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 150,  // Kích thước chiều rộng mã QR
                    Height = 150  // Kích thước chiều cao mã QR
                }
            };
            using (Bitmap qrCodeImage = barcodeWriter.Write(QRContent))
            {
                e.Graphics.DrawImage(qrCodeImage, new Rectangle(350, 61, 173, 173));
            }
        }
    }
}
