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

    public class PO_Info
    {
        public string OrderNO { get; set;}
        public string UniqueCode { get; set;}
        public string Site { get; set;}
        public string Factory { get; set;}
        public string ProductionLine { get; set;}
        public string ProductionDate { get; set;}
        public string Shift { get; set;}
        public string czFile { get; set;}
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
        public int Total_C2 { get; set; } = 0;
        public int Total_Pass_C2 { get; set; } = 0;
        public int Total_Failed_C2 { get; set; } = 0;

        public int Camera_Read_Fail_C2 { get; set; } = 0;
        public int Duplicate_C2 { get; set; } = 0;
        public int Format_C2 { get; set; } = 0;

        public int Empty_C2 { get; set; } = 0;
        public int NotFound_C2 { get; set; } = 0;
        public int Rework_C2 { get; set; } = 0;
        public int Unknown_C2 { get; set; } = 0;

        public int Error_C2 { get; set; } = 0; // Thêm biến Error_C2 để đếm lỗi chung cho C2



        public int PLC_0_Fail_C2 { get; set; } = 0;
        public int PLC_0_Pass_C2 { get; set; } = 0;
        public int PLC_1_Pass_C2 { get; set; } = 0;

        public int PLC_1_Fail_C2 { get; set; } = 0;
        public string TimeSendPLC_C2 { get; set; } = "0/0";
        public string TimeProcess_C2 { get; set; } = "0/0";

        public string WK4_TimeProcess_C2 { get; set; } = "0/0";
        public string WK5_TimeProcess_C2 { get; set; } = "0/0";
        public string WK6_TimeProcess_C2 { get; set; } = "0/0";


        public int Total_C1 { get; set; } = 0;
        public int Total_Pass_C1 { get; set; } = 0;
        public int Total_Failed_C1 { get; set; } = 0;

        public int Camera_Read_Fail_C1 { get; set; } = 0;
        public int Duplicate_C1 { get; set; } = 0;
        public int Format_C1 { get; set; } = 0;

        public int Empty_C1 { get; set; } = 0;
        public int NotFound_C1 { get; set; } = 0;
        public int Rework_C1 { get; set; } = 0;
        public int Unknown_C1 { get; set; } = 0;

        public int Error_C1 { get; set; } = 0; // Thêm biến Error_C1 để đếm lỗi chung cho C1


        public int PLC_0_Fail_C1 { get; set; } = 0;
        public int PLC_0_Pass_C1 { get; set; } = 0;
        public int PLC_1_Pass_C1 { get; set; } = 0;

        public int PLC_1_Fail_C1 { get; set; } = 0;
        public string TimeSendPLC_C1 { get; set; } = "0/0";
        public string TimeProcess_C1 { get; set; } = "0/0";

        public string WK1_TimeProcess_C1 { get; set; } = "0/0";
        public string WK2_TimeProcess_C1 { get; set; } = "0/0";
        public string WK3_TimeProcess_C1 { get; set; } = "0/0";

        public int AWS_Sent_Count { get; set; } = 0; // Biến để đếm số lượng gửi AWS
        public int AWS_Recive_Count { get; set; } = 0; // Biến để đếm thời gian gửi AWS

        //thêm hàm để reset các giá trị về 0
        public void Reset()
        {
            Total_C2 = 0;
            Total_Pass_C2 = 0;
            Total_Failed_C2 = 0;

            Camera_Read_Fail_C2 = 0;
            Duplicate_C2 = 0;
            Format_C2 = 0;

            Empty_C2 = 0;
            NotFound_C2 = 0;
            Rework_C2 = 0;
            Unknown_C2 = 0;

            PLC_0_Fail_C2 = 0;
            PLC_0_Pass_C2 = 0;
            PLC_1_Pass_C2 = 0;
            PLC_1_Fail_C2 = 0;

            TimeSendPLC_C2 = "0/0";
            TimeProcess_C2 = "0/0";

            WK4_TimeProcess_C2 = "0/0";
            WK5_TimeProcess_C2 = "0/0";
            WK6_TimeProcess_C2 = "0/0";

            Total_C1 = 0;
            Total_Pass_C1 = 0;
            Total_Failed_C1 = 0;

            Camera_Read_Fail_C1 = 0;
            Duplicate_C1 = 0;
            Format_C1 = 0;

            Empty_C1 = 0;
            NotFound_C1 = 0;
            Rework_C1 = 0;
            Unknown_C1 = 0;

            PLC_0_Fail_C1 = 0;
            PLC_0_Pass_C1 = 0;
            PLC_1_Pass_C1 = 0;
            PLC_1_Fail_C1 = 0;

            TimeSendPLC_C1 = "0/0";
            TimeProcess_C1 = "0/0";
            WK1_TimeProcess_C1 = "0/0";
            WK2_TimeProcess_C1 = "0/0";
            WK3_TimeProcess_C1 = "0/0";

        }
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

namespace MainClass.Enum
{
    public enum e_Content_Result
    {
        PASS,//tốt
        FAIL, //lỗi
        REWORK, //thả lại
        DUPLICATE, //trùng
        EMPTY,//không có
        ERR_FORMAT, //lỗi định dạng
        NOT_FOUND, //không tìm thấy mã
        ERROR //lỗi không xác định
    }
}