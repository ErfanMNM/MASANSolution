
using MainClass;
using QR_MASAN_01.Auth;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MainClass.Counter_Info;

namespace QR_MASAN_01
{
    
    public class GPrinter
    {
        public static bool IsPush { get; set; } = false;
        public static bool IsPushPrint { get; set; } = false;
        public static bool IsData { get; set; } = false;
        public static List<string> QRdata { get; set; } = new List<string>();
        public static e_PRINTER_Status Printer_Status { get; set; } = e_PRINTER_Status.DISCONNECTED;

    }

    public static class PLC_Comfirm
    {
        public static int Curent_Total { get; set; } = 0;
        public static int Last_Total { get; set; } = 0;

        public static int Curent_Pass { get; set; } = 0;
        public static int Last_Pass { get; set; } = 0;

        public static int Curent_Fail { get; set; } = 0;
        public static int Last_Fail { get; set; } = 0;

        public static int Curent_Timeout { get; set; } = 0;
        public static int Last_Timeout { get; set; } = 0;

        public static int Curent_Status { get; set; } = 0;

        //dictionary để lưu trữ các giá trị của PLC Total - trạng thái
        public static Dictionary<int, string> PLC_Total_Status_Dictionary { get; set; } = new Dictionary<int, string>();

    }



    public static class Globalvariable
    {
        public static Counter_Info GCounter { get; set; } = new Counter_Info();
        public static PLC_Counter_Info PLCCounter { get; set; } = new PLC_Counter_Info();

        public static UI_Info C1_UI { get; set; } = new UI_Info();
        public static UI_Info C2_UI { get; set; } = new UI_Info();
        public static Color WB_Color { get; set; }
        public static Color NG_Color { get; set; } = Color.FromArgb(255, 128, 0);
        public static Color OK_Color { get; set; } = Color.FromArgb(243, 249, 255);


        public static Dictionary<string, ProductData> Main_Content_Dictionary = new Dictionary<string, ProductData>();

        public static Dictionary<string, ProductData> C1_Content_Dictionary = new Dictionary<string, ProductData>();
        public static Dictionary<string, ProductData> C2_Content_Dictionary = new Dictionary<string, ProductData>();

        public static Queue <(string, ProductData)> Add_Content_To_SQLite_Queue = new Queue<(string, ProductData)>();
        public static Queue<ProductData> Update_Content_To_SQLite_Queue = new Queue<ProductData>();
        public static Queue<ProductData> Rework_Content_To_SQLite_Queue = new Queue<ProductData>();

        public static Queue<(string, ProductData)> C1_Add_Content_To_SQLite_Queue = new Queue<(string, ProductData)>();
        public static Queue<ProductData> C1_Update_Content_To_SQLite_Queue = new Queue<ProductData>();
        public static Queue<ProductData> C1_Rework_Content_To_SQLite_Queue = new Queue<ProductData>();

        public static Queue<(string, ProductData)> C2_Add_Content_To_SQLite_Queue = new Queue<(string, ProductData)>();
        public static Queue<ProductData> C2_Update_Content_To_SQLite_Queue = new Queue<ProductData>();
        public static Queue<ProductData> C2_Rework_Content_To_SQLite_Queue = new Queue<ProductData>();

        public static int MaxID_Content { get; set; } = 0;

        public static e_Server_Status Server_Status { get; set; } = e_Server_Status.DISCONNECTED;
        public static bool FDashBoard_Ready { get; set; } = false;
        public static bool All_Ready { get; set; } = false;
        public static bool setReady { get; set; } = true;
        public static bool PLCConnect { get; set; } = false;
        public static e_Data_Status Data_Status { get; set; } = e_Data_Status.STARTUP;
        public static string MFI_ID { get; set; } = "0";
        public static bool ISRerun { get; set; } = false;
        public static string Server_Url { get; set; } = "https://sv2.th.io.vn/";

        // Tạo một Queue để lưu trữ các chuỗi

        public static int MaxID_QR { get; set; } = 0;

        public static string ProductBarcode { get; set; } = string.Empty;

        public static string QRCode_Folder { get; set; } = string.Empty;
        public static string QRCode_FileName { get; set; } = string.Empty;

        public static long TimeUnixPrinter { get; set; } = 0;


        public static UserData CurrentUser { get; set; } = new UserData(); // Lưu thông tin người dùng hiện tại

        public static bool ACTIVE { get; set; } = true;
        public static bool ACTIVE_C1 { get; set; } = true;
        public static bool ACTIVE_C2 { get; set; } = true;

        //PO
        public static e_PI_Status PI_Status { get; set; } = e_PI_Status.NOPO; // Trạng thái của PO (không có PO hoặc đang chỉnh sửa)
        public static int Product_Active_Count { get; set; } = 0; // Số lượng sản phẩm đã kích hoạt

        //V2
        

    }

    public static class GV
    {
        //bể dữ liệu chính của C2
        public static Dictionary<string, CodeData> C2_CodeData_Dictionary = new Dictionary<string, CodeData>();
        public static Dictionary<string, CodeData> C1_CodeData_Dictionary = new Dictionary<string, CodeData>();

        public static Queue<CodeData> C1_Update_Content_To_SQLite_Queue = new Queue<CodeData>();
        public static Queue<CodeData> C2_Update_Content_To_SQLite_Queue = new Queue<CodeData>();

        public static Queue<DataResultSave> C2_Save_Result_To_SQLite_Queue = new Queue<DataResultSave>();
        public static Queue<AWS_Response> AWS_Response_Queue = new Queue<AWS_Response>();
        public static int ID { get; set; } = 0; // ID của sản phẩm hiện tại

        public static e_Production_Status Production_Status { get; set; } = e_Production_Status.STARTUP;
        public static PO_Infomation Selected_PO { get; set; } = new PO_Infomation(); // Thông tin PO được chọn

        public static int Pass_Product_Count { get; set; } = 0;

        //số lượng mã send
        public static int Sent_Count { get; set; } = 0;
        //số lượng mã đã nhận response từ AWS
        public static int Received_Count { get; set; } = 0;
    }

    public static class GTask
    {
       public static CancellationTokenSource Task_PLC_Comfirm = new CancellationTokenSource();
        public static CancellationTokenSource Task_Setting_PLC_Load_Parameter = new CancellationTokenSource();
    }

    public class Alarm
    {
        public static bool Alarm1 { get; set; } = false;
        public static int Alarm1_Count { get; set; } = 0;
    }

    public class ProductData
    {
        public int ProductID { get; set; }
        public int Active { get; set; }
        public string TimeStampActive { get; set; }
        public long TimeUnixActive { get; set; }
        public string TimeStampPrinted { get; set; }
        public long TimeUnixPrinted { get; set; }
    }

    public class CodeData
    {
        public int ID { get; set; }
        public string orderNo { get; set; } // Số đơn hàng
        public string Status { get; set; } // 0: Chưa kích hoạt,Active là 1, reject là -1
        public string Activate_Datetime { get; set; } // Thời gian kích hoạt
        public string Production_Datetime { get; set; } // Thời gian sản xuất
    }

    public class DataResultSave
    {
        public int ID { get; set; }
        public string Code { get; set; } // Mã code
        public string orderNo { get; set; } // Số đơn hàng
        public string Status { get; set; } //PASS, FAIL, TIMEOUT, REJECT, ACTIVE, INACTIVE
        public string PLC_Send_Status { get; set; } // Trạng thái gửi PLC (0: Chưa gửi, 1: Đã gửi, -1: Lỗi gửi)
        public string Activate_Datetime { get; set; } // Thời gian kích hoạt
        public string Production_Datetime { get; set; } // Thời gian sản xuất
    }

    public class AWS_Response
    {
        public string status { get; set; } // Trạng thái trả về từ AWS
        public string message_id { get; set; } // Thông điệp trả về từ AWS
        public string error_message { get; set; } // Dữ liệu trả về từ AWS
    }

    public enum e_Server_Status
    {
        CONNECTED,
        DISCONNECTED
    }

    public enum e_Production_Status
    {
        EDITING, // Đang chỉnh sửa PO
        DATE_EDITING, // Đang chỉnh sửa ngày sản xuất
        PLC_NEW_PO, // Đang đẩy dữ liệu lên server
        STOPPED, // Dừng sản xuất
        RUNNING, // Đang sản xuất
        PAUSED, // Tạm dừng sản xuất
        READY, // Sẵn sàng sản xuất
        NOPO, // Không có PO
        STARTUP, // Bắt đầu
        LOAD, // Tải dữ liệu
        CHECKING, // Đang kiểm tra
        COMPLETE, // Hoàn thành
        TESTING, // Đang kiểm tra
        FINALTESTING, // Đã test xong
        PLC_CON_PO, //tiếp tục PO cũ
        UNKNOWN // Trạng thái không xác định
    }

    public class GCamera
    {
        public static e_Camera_Status Camera_Status { get; set; } = e_Camera_Status.DISCONNECTED;
        public static e_Camera_Status Camera_Status_02 { get; set; } = e_Camera_Status.DISCONNECTED;

    }

    public class GServer
    {
        public static e_Server_Status Server_Status { get; set; } = e_Server_Status.DISCONNECTED;
        public static e_Server_Status Client_QR01 { get; set; } = e_Server_Status.DISCONNECTED;
        public static e_Server_Status Client_QR02 { get; set; } = e_Server_Status.DISCONNECTED;
        public static e_Server_Status Client_QR03 { get; set; } = e_Server_Status.DISCONNECTED;


    }

    public enum e_PRINTER_Status
    {
        CONNECTED,
        DISCONNECTED,
        PRINTING,
        JOB_CHANGE,
        STOPPED,
        INK_LOW,
        DATA_PRINTING,
        DATA_EMPTY,
        UNKNOWN,
        NOUSE
    }

    public enum e_Data_Status
    {
        READY,
        NEW,
        PUSHOK,
        STARTUP,
        GET,
        PUSH,
        PRINTER_PUSH,
        CREATING,
        CREATE,
        UNKNOWN
    }

    public enum e_Camera_Status
    {
        CONNECTED,
        DISCONNECTED,
        NOTREADY,
        READY,
        UNKNOWN,
        RECONNECT
    }

    public enum e_PI_Status
    {
        NOPO,
        EDITING,
        READY,
        PUSHOK,
        PUSH
    }

    public class APP
    {
        public static bool ByPass_Printer_Status { get; set; } = false;
        public static bool ByPass_Ready { get; set; } = false;
    }

    

}
