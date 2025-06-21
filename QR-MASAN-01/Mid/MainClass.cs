using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainClass
{
    public class MFI_Info
    {
        public string Case_Barcode { get; set; } = "Barcode";
        public string Product_Barcode { get; set; } = "0";
        public string Case_LOT { get; set; } = "16122024";
        public string Batch_Code { get; set; }
        public string Block_Size { get; set; }
        public string Case_Size { get; set; }
        public string Pallet_Size { get; set; }
        public string SanLuong { get; set; }
        public string Operator { get; set; }
        public string Pallet_QR_Type { get; set; }
        public string MFI_ID { get; set; }
        public string Data_Content_Folder { get; set; }
        public string Data_Content_Filename { get; set; }
        public string Data_Content_Filename_C1 { get; set; }
        public string Data_Content_Filename_C2 { get; set; }

        public e_MFI_Status MFI_Status { get; set; } = e_MFI_Status.STARTUP;
    }


    public enum e_MFI_Status
    {
        STARTUP,
        SQLite_LOAD,
        SQLite_SAVE,
        PUSHSERVER,
        FREE
    }

    public class ProductInfo
    {
        public string ProductID { get; set; }
        public string ProductQR { get; set; }
        public int Active { get; set; } // 1: đã kích hoạt, 0: chưa kích hoạt
        public string TimeStamp { get; set; } // thời gian kích hoạt
    }

    public class Counter_Info
    {
        public int Total_Code_C2 { get; set; } = 0;
        public int Total_Return_C2 { get; set; } = 0;
        public int Camera_Fail_C2 { get; set; } = 0;
        public int PLC_0_Fail_C2 { get; set; } = 0;
        public int PLC_0_Pass_C2 { get; set; } = 0;
        public int PLC_1_Fail_C2 { get; set; } = 0;
        public int PLC_1_Pass_C2 { get; set; } = 0;

        public int PLC_0_Fail_C1 { get; set; } = 0;
        public int PLC_0_Pass_C1 { get; set; } = 0;
        public int PLC_1_Fail_C1 { get; set; } = 0;
        public int PLC_1_Pass_C1 { get; set; } = 0;

        public int Total_Code_C1 { get; set; } = 0;
        public int Total_Return_C1 { get; set; } = 0;
        public int Camera_Fail_C1 { get; set; } = 0;
        public int Camera_Pass_C1 { get; set; } = 0;

    }

    public class UI_Info
    {
        public string Curent_Content { get; set; } = "0";
        public bool IsPass { get; set; } = true;
        public bool IsRework { get; set; } = false;
        public bool IsFormat { get; set; } = false;
        public string Last_Content { get; set; } = "0";
    }
}
