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
        public bool AWS_ENA { get; set; } // Bật/Tắt tính năng AWS (Amazon Web Services)
        public string host { get; set; } // Địa chỉ máy chủ AWS
        public string rootCAPath { get; set; } // Khóa truy cập AWS
        public string pfxPath { get; set; } // Khóa bí mật AWS
        public string pfxPassword { get; set; } // Mật khẩu khóa bí mật AWS
        public string clientId { get; set; } // Điểm cuối AWS IoT
        public int cartonPack { get; set; } // Đường dẫn đến chứng chỉ của client
        public int cartonOfset { get; set; } // Số lượng sản phẩm trong một thùng carton


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
            AWS_ENA = true; // Mặc định tắt tính năng AWS
            rootCAPath = @"C:\MIPWP501\AmazonRootCA1.pem"; // Default path for root CA certificate
            pfxPath = @"C:\MIPWP501\client-certificate.pfx"; // Default path for client certificate
            host = "a22qv9bgjnbsae-ats.iot.ap-southeast-1.amazonaws.com"; // Default host address
            pfxPassword = "thuc"; // Default password for client certificate
            clientId = "MIPWP501"; // Default client ID
            cartonPack = 12; // Default path for carton pack certificate
            cartonOfset = 12; // Default path for carton offset certificate
        }
    }
}
