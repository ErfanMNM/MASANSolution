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
        [ConfigSection("SETUP")]
        public string Camera_Main_IP { get; set; } // Địa chỉ IP của camera chính
        public string Camera_Sub_IP { get; set; } // Địa chỉ IP của camera phụ
        public int Camera_Main_Port { get; set; } // Cổng kết nối của camera chính
        public int Camera_Sub_Port { get; set; } // Cổng kết nối của camera phụ
        public string HandScanCOM01 { get; set; } // Tên đăng nhập của camera chính
        public string HandScanCOM02 { get; set; } // Tên đăng nhập của camera phụ

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
        }
    }
}
