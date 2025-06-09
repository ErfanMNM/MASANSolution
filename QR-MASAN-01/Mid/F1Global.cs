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
    public enum SETCODE
    {
        Init,
        PushDataToPrinter,
        StartPrinter
    }

    public class Globalvariable
    {
        public static Color WB_Color { get; set; }
        public static Color NG_Color { get; set; } = Color.FromArgb(255, 128, 0);
        public static Color OK_Color { get; set; } = Color.FromArgb(243, 249, 255);
        public static Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
        public static e_Server_Status Server_Status { get; set;} = e_Server_Status.DISCONNECTED;
        public static bool AllReady {  get; set; } = false;
        public static bool PLCConnect { get; set; } = false;
        public static e_Data_Status Data_Status { get; set; } = e_Data_Status.STARTUP;
        public static string MFI_ID { get; set; } ="0";
        public static bool ISRerun { get; set; } = false;
        public static string Server_Url { get; set; } = "https://sv2.th.io.vn/";
        public static e_Mode APPMODE { get; set; } = e_Mode.OLDMode;

        // Tạo một Queue để lưu trữ các chuỗi
        public static Queue<int> UpdateQueue120 = new Queue<int>();

        public static Queue<string> AddQueue120 = new Queue<string>();

        public static string QRgoc { get; set; } = string.Empty;

        public static int MaxID_QR { get; set; } = 0;
        //public static string 
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
        public string TimeStamp { get; set; }
    }


    public enum e_Server_Status
    {
        CONNECTED,
        DISCONNECTED
    }

    public class GCamera
    {
        public static e_Camera_Status Camera_Status {  get; set; } = e_Camera_Status.DISCONNECTED;
        
    }

    public class GServer
    {
        public static e_Server_Status Server_Status { get; set; } = e_Server_Status.DISCONNECTED;
        public static e_Server_Status Client_QR01 { get; set; } = e_Server_Status.DISCONNECTED;
        public static e_Server_Status Client_QR02 { get; set; } = e_Server_Status.DISCONNECTED;
        public static e_Server_Status Client_QR03 { get; set; } = e_Server_Status.DISCONNECTED;
    }

    public class ClientData
    {
        public string L3_QR01 { get; set; }
        public string L3_QR02 { get; set; }
        public string L3_QR03 { get; set; }
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
}
