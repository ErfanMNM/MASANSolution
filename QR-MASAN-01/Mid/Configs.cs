using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_MASAN_01
{
    [ConfigFile("MSC\\Setting.ini")]
    public class Setting : IniConfig<Setting>
    {
        [ConfigSection("APP")]
        public string SoftName { get; set; }
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string City { get; set; }
        public string App_Mode { get; set; }
        public string rootCAPath { get; set; } // Đường dẫn đến file chứng chỉ root CA
        public string pfxPath { get; set; } // Đường dẫn đến file chứng chỉ client
        public string pfxPassword { get; set; } // Đường dẫn đến file khóa riêng của client
        public string host { get; set; } // Địa chỉ chủ
        public string clientId { get; set; } // ID của client
        public string HandScanCOM { get; set; }
        public bool AWS_ENA { get; set; } // Tốc độ baud rate mặc định cho máy quét tay

        public string User_Database { get; set; } = @"abcc.bcaa"; // Đường dẫn đến file cơ sở dữ liệu người dùng

        public string Printer_name { get; set; }


        public string Laser_printer_server_url { get; set; }


        public int Camera_Slot { get; set; }
        public string IP_Camera_01 { get; set; } // Thêm camera slot 02 nếu cần
        public string IP_Camera_02 { get; set; } // Thêm camera slot 03 nếu cần
        public int Port_Camera_01 { get; set; } // Thêm camera slot 04 nếu cần
        public int Port_Camera_02 { get; set; } // Thêm camera slot 05 nếu cần

        public string Code_Content_Pattern { get; set; }
        public string Production_Mode { get; set; }
        public string PO_Data_path { get; set; }

        public bool TwoFA_Enabled { get; set; }
        public string PO_Edit_AMode { get; set; }
        public bool TwoFA_Enabled_PO { get; set; }

        public override void SetDefault()
        {
            base.SetDefault();
            SoftName = "MS";
            ServerIP = "http://localhost";
            ServerPort = 49211;
            City = "MSI";
            Printer_name = "NONE"; // Default printer name
            Camera_Slot = 2; // Default camera slot
            App_Mode = "ADD_Data"; // NO_ADD
            Laser_printer_server_url = "http://127.0.0.1:9000/get-time";
            Code_Content_Pattern = @"i\.tcx\.com\.vn/.*\d{13}.*[a-zA-Z0-9]";
            Production_Mode = "MFI"; // chạy dạng MFI , không chạy dạng PO
            TwoFA_Enabled = false; // Enable 2FA by default
            PO_Edit_AMode = "NONE"; // Default mode for editing PO
            TwoFA_Enabled_PO = false; // Enable 2FA for PO editing by default
            PO_Data_path = @"C:\Users\DANOMT\source\repos\MASANSolution\Server_Service";
            IP_Camera_01 = "127.0.0.1";
            IP_Camera_02 = "127.0.0.1";
            Port_Camera_01 = 6969; // Default port for camera 01
            Port_Camera_02 = 6968; // Default port for camera 02
            rootCAPath = @"C:\MIPWP501\AmazonRootCA1.pem"; // Default path for root CA certificate
            pfxPath = @"C:\MIPWP501\client-certificate.pfx"; // Default path for client certificate
            host = "a22qv9bgjnbsae-ats.iot.ap-southeast-1.amazonaws.com"; // Default host address
            pfxPassword = "thuc"; // Default password for client certificate
            clientId = "MIPWP501"; // Default client ID
            HandScanCOM = "COM2"; // Default COM port for hand scanner
            AWS_ENA = false; // Enable AWS by default
        }
    }
}
