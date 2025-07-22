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
    public class Configs : IniConfig<Configs>
    {

        [ConfigSection("APP")]
        public bool TwoFA_Enabled { get; set; } // Bật/Tắt tính năng xác thực hai yếu tố

        public override void SetDefault()
        {
            // Thiết lập giá trị mặc định cho các thuộc tính
            base.SetDefault();
            TwoFA_Enabled = false; // Mặc định tắt tính năng xác thực hai yếu tố
        }
    }
}
