using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_MASAN_01.Mid
{
    public class Settings
    {

        public void LoadSettings(string filePath)
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                int row = 2; // Bỏ dòng tiêu đề
                while (true)
                {
                    var key = worksheet.Cell($"A{row}").GetString();
                    var value = worksheet.Cell($"C{row}").GetString();

                    if (string.IsNullOrWhiteSpace(key))
                        break; // Hết dữ liệu

                    GlobalSettings.Settings[key] = value;
                    row++;
                }
            }
        }
    }

}

