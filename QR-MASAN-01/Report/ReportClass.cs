using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using iTextSharp.text.pdf.draw;
using SpT;

namespace QR_MASAN_01.Report
{
    public static class ReportClass
    {
        public static (string HashCode, bool IsSucces, string FilePath)  ExportReportToPDF(DataTable table, string filePath , string LogType)
        {
            // 1️⃣ Nhúng font
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font vnFont = new Font(baseFont, 12, Font.NORMAL);

            // 1️⃣ Tạo file PDF
            Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

            doc.Open();

            // 2️⃣ Tiêu đề trên cùng
            Font headerFont = new Font(baseFont, 14, Font.BOLD);
            Paragraph header = new Paragraph("Phần Mềm Quản Lý Hệ Thống Kích Hoạt Mã 2D Trên Nhãn Sản Phẩm", headerFont);
            header.Alignment = Element.ALIGN_CENTER;
            doc.Add(header);

            // Dòng kẻ
            doc.Add(new Paragraph(new Chunk(new LineSeparator())));

            // 3️⃣ Tiêu đề báo cáo
            Font titleFont =  new Font(baseFont, 25, Font.BOLD);
            Paragraph title = new Paragraph("BÁO CÁO", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingBefore = 20;
            doc.Add(title);

            // 4️⃣ Meta info

            Font metaFont = new Font(baseFont, 12, Font.NORMAL);
            StringBuilder meta = new StringBuilder();
            meta.AppendLine($"Loại báo cáo :");
            meta.Append("Truy vết hệ thống - " + LogType);
            meta.AppendLine($"Thời gian xuất : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            meta.AppendLine($"Người xuất : {Environment.UserName}");

            // Tạm thời gán chuỗi rỗng cho Hash, lát nữa tính rồi update.
            meta.AppendLine($"Mã xác thực báo cáo : Tạm tính...");

            Paragraph metaInfo = new Paragraph(meta.ToString(), metaFont);
            metaInfo.SpacingBefore = 20;
            doc.Add(metaInfo);

            // 5️⃣ Nội dung bảng
            Paragraph contentTitle = new Paragraph("Nội dung :", metaFont);
            contentTitle.SpacingBefore = 20;
            contentTitle.SpacingAfter = 20;
            doc.Add(contentTitle);

            PdfPTable pdfTable = new PdfPTable(table.Columns.Count);
            pdfTable.WidthPercentage = 100;

            // Header
            foreach (DataColumn col in table.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName, metaFont));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfTable.AddCell(cell);
            }

            // Data rows
            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    pdfTable.AddCell(new Phrase(item?.ToString(), metaFont));
                }
            }

            doc.Add(pdfTable);

            doc.Close();
            writer.Close();

            // 6️⃣ Tính Hash file PDF sau khi xuất
            string hash = HashHelper.ComputeFileHash(filePath,"SHA256");

            // 7️⃣ Ghi đè file PDF để update hash trong meta
            // (Mở lại file, thêm dòng hash đúng)
            string newFilePath = filePath; // Bạn có thể ghi đè luôn
            doc = new Document(PageSize.A4, 50, 50, 50, 50);
            writer = PdfWriter.GetInstance(doc, new FileStream(newFilePath, FileMode.Create));
            doc.Open();

            // Add lại phần giống cũ:
            doc.Add(header);
            doc.Add(new Paragraph(new Chunk(new LineSeparator())));
            doc.Add(title);

            meta.Clear();
            meta.AppendLine($"Loại báo cáo : Lịch sử hệ thống – LOGIN");
            meta.AppendLine($"Thời gian xuất : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            meta.AppendLine($"Người xuất : {Environment.UserName}");
            meta.AppendLine($"Mã xác thực báo cáo : {hash}");

            metaInfo = new Paragraph(meta.ToString(), metaFont);
            metaInfo.SpacingBefore = 20;
            doc.Add(metaInfo);

            doc.Add(contentTitle);
            doc.Add(pdfTable);

            doc.Close();
            writer.Close();
            return (hash, true, filePath);
        }
    }
}
