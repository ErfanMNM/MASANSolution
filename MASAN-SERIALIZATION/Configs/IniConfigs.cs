using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASAN_SERIALIZATION.Configs
{
    //tạo đường dẫn đến file cấu hình Setting.ini tại thư mục AppData/Local/MASAN-SERIALIZATION/MSC
    [ConfigFile("Configs\\MSC.ini")]
    public class AppConfigs : IniConfig<AppConfigs>
    {

        [ConfigSection("APP")]
        public bool TwoFA_Enabled { get; set; } // Bật/Tắt tính năng xác thực hai yếu tố
        public string Camera_Main_IP { get; set; } // Địa chỉ IP của camera chính
        public string Camera_Sub_IP { get; set; } // Địa chỉ IP của camera phụ
        public int Camera_Main_Port { get; set; } // Cổng kết nối của camera chính
        public int Camera_Sub_Port { get; set; } // Cổng kết nối của camera phụ
        public string HandScanCOM01 { get; set; } // Tên đăng nhập của camera chính
        public string HandScanCOM02 { get; set; } // Tên đăng nhập của camera phụ
        public string HandScanCOMMain { get; set; } // Tên đăng nhập của camera phụ thứ ba (nếu có, có thể để trống nếu không sử dụng)
        public bool AWS_ENA { get; set; } // Bật/Tắt tính năng AWS (Amazon Web Services)
        public string host { get; set; } // Địa chỉ máy chủ AWS
        public string rootCAPath { get; set; } // Khóa truy cập AWS
        public string pfxPath { get; set; } // Khóa bí mật AWS
        public string pfxPassword { get; set; } // Mật khẩu khóa bí mật AWS
        public string clientId { get; set; } // Điểm cuối AWS IoT
        public int cartonPack { get; set; } // Đường dẫn đến chứng chỉ của client
        public int cartonOfset { get; set; } // Số lượng sản phẩm trong một thùng carton

        public int cartonScanerMode { get; set; } // Chế độ quét mã thùng carton (0: Manual, 1: Auto)

        public bool cartonScaner_Only_Once { get; set; } // Bật/Tắt tính năng quét mã thùng carton qua cổng COM
        public string cartonCode_Line01 { get; set; }
        public string cartonCode_Line02 { get; set; }

        public string cartonScanerTCP_IP { get; set; } // Địa chỉ IP của máy quét mã thùng carton TCP
        public int cartonScanerTCP_Port { get; set; } // Cổng kết nối của máy quét mã thùng carton TCP

        public bool Auto_Send_AWS { get; set; } // Tự động gửi dữ liệu lên AWS
        public int APP_Mode { get; set; } // Chế độ ứng dụng (0: Normal, 1: Test, 2: Debug)
        public bool cartonAutoStart { get; set; } // Tự động kích hoạt mã thùng carton

        public bool PLC_Test_Mode { get; set; } // Biến để kiểm tra kết nối PLC, mặc định là false
        public bool PLC_Duo_Mode { get; set; } // Biến để kiểm tra kết nối PLC, mặc định là false

        public string PLC_Address_Sheet_Name { get; set; }




        public override void SetDefault()
        {
            // Thiết lập giá trị mặc định cho các thuộc tính
            base.SetDefault();
            TwoFA_Enabled = false; // Mặc định tắt tính năng xác thực hai yếu tố
            Camera_Main_IP =@"127.0.0.1";// Địa chỉ IP mặc định của camera chính
            Camera_Sub_IP =@"127.0.0.1";// Địa chỉ IP mặc định của camera phụ
            Camera_Main_Port = 51236; // Cổng kết nối mặc định của camera chính
            Camera_Sub_Port = 51237; // Cổng kết nối mặc định của camera phụ
            HandScanCOM01 = "COM2"; // Tên đăng nhập mặc định của camera chính
            HandScanCOM02 = "COM3"; // Tên đăng nhập mặc định của camera phụ
            HandScanCOMMain = "COM4"; // Tên đăng nhập mặc định của camera phụ thứ ba (nếu có, có thể để trống nếu không sử dụng)
            AWS_ENA = true; // Mặc định tắt tính năng AWS
            rootCAPath = @"C:\MIPWP501\AmazonRootCA1.pem"; // Default path for root CA certificate
            pfxPath = @"C:\MIPWP501\client-certificate.pfx"; // Default path for client certificate
            host = "a22qv9bgjnbsae-ats.iot.ap-southeast-1.amazonaws.com"; // Default host address
            pfxPassword = "thuc"; // Default password for client certificate
            clientId = "MIPWP501"; // Default client ID
            cartonPack = 24; // Default path for carton pack certificate
            cartonOfset = 2; // Default path for carton offset certificate
            Auto_Send_AWS = false; // Mặc định tự động gửi dữ liệu lên AWS
            APP_Mode = 0; // Mặc định chế độ ứng dụng là Normal (0)
            cartonAutoStart = false; // Mặc định tự động kích hoạt mã thùng carton
            PLC_Test_Mode = true; // Mặc định kiểm tra kết nối PLC là false
            PLC_Duo_Mode = false; // Mặc định kiểm tra kết nối PLC là false

            cartonScanerMode = 0; // Mặc định chế độ quét mã thùng carton là Manual (0)
            cartonScanerTCP_IP = "192.168.250.14";
            cartonScanerTCP_Port = 5566;
            PLC_Address_Sheet_Name = "PLC";
            cartonCode_Line01 = "0";
            cartonCode_Line02 = "1";
        }
    }
}
