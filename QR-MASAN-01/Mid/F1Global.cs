using MainClass;
using QR_MASAN_01.Auth;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class Globalvariable
    {
        public static Counter_Info GCounter { get; set; } = new Counter_Info();

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
        public static bool AllReady { get; set; } = false;
        public static bool PLCConnect { get; set; } = false;
        public static e_Data_Status Data_Status { get; set; } = e_Data_Status.STARTUP;
        public static string MFI_ID { get; set; } = "0";
        public static bool ISRerun { get; set; } = false;
        public static string Server_Url { get; set; } = "https://sv2.th.io.vn/";

        public static string Laser_Printer_Url { get; set; } = "http://127.0.0.1:9000/get-time";

        // Tạo một Queue để lưu trữ các chuỗi

        public static int MaxID_QR { get; set; } = 0;

        public static string ProductBarcode { get; set; } = string.Empty;

        public static string QRCode_Folder { get; set; } = string.Empty;
        public static string QRCode_FileName { get; set; } = string.Empty;

        public static long TimeUnixPrinter { get; set; } = 0;


        public static UserData CurrentUser { get; set; } = new UserData(); // Lưu thông tin người dùng hiện tại
    }

    public enum e_Mode
    {
        OLDMode,
        NEWMode
    }

    public class Alarm
    {
        public static bool Alarm1 { get; set; } = false;
        public static int Alarm1_Count { get; set; } = 0;
    }

    public class Counter
    {
        public static int Rerun { get; set; } = 0;
        public static int Fail { get; set; } = 0;
        public static int Empty { get; set; } = 0;
        public static int Diff { get; set; } = 0;
        public static int StructERR { get; set; } = 0;
        public static int QRCount { get; set; } = 0;
        public static int Camera120Count { get; set; } = 0;
        public static int Send0ToPLC_Fail { get; set; } = 0;
        public static int Send0ToPLC_OK { get; set; } = 0;
        public static int Send1ToPLC_Fail { get; set; } = 0;
        public static int Send1ToPLC_OK { get; set; } = 0;

        public static double PLCTimeCurrent { get; set; } = 0;
        public static double PLCTimeMax { get; set; } = 0;
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

    public enum e_Server_Status
    {
        CONNECTED,
        DISCONNECTED
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

    public class APP
    {
        public static bool ByPass_Printer_Status { get; set; } = false;
        public static bool ByPass_Ready { get; set; } = false;
    }

    public static class GlobalSettings
    {
        // Dictionary lưu key-value từ Excel
        public static Dictionary<string, string> Settings = new Dictionary<string, string>();

        // Hàm tiện lấy ra kiểu string
        public static string Get(string key)
        {
            if (Settings.ContainsKey(key))
                return Settings[key];
            return string.Empty;
        }

        // Hàm tiện lấy ra kiểu int
        public static int GetInt(string key)
        {
            if (Settings.ContainsKey(key) && int.TryParse(Settings[key], out int value))
                return value;
            return 0;
        }
    }

}
