using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using SpT.Auth;
using SpT.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AwsIotClientHelper;

namespace MASAN_SERIALIZATION
{
    public static class Globals
    {
        // Biến toàn cục để lưu trữ thông tin người dùng hiện tại
        public static UserData CurrentUser { get; set; } = new UserData();

        // Biến toàn cục để lưu trữ trạng thái ứng dụng
        public static e_App_State AppState { get; set; } = e_App_State.LOGIN;

        // Biến toàn cục để lưu trữ trạng thái giao diện ứng dụng
        public static e_App_Render_State AppRenderState { get; set; } = e_App_Render_State.LOGIN;

        public static bool ACTIVE_State { get; set; } = true; // Biến toàn cục để kiểm soát hoạt động của ứng dụng

        //biến lưu nhật ký hệ thống
        public static LogHelper<e_LogType> Log { get; set; }

        public static ProductionOrder ProductionData { get; set; } = new ProductionOrder();

        public static ProductionOrder productionData_Cs { get; set; } = new ProductionOrder(); // Biến toàn cục để lưu trữ thông tin đơn hàng sản xuất hiện tại

        // Biến toàn cục để lưu trữ trạng thái sản xuất
        public static e_Production_State Production_State { get; set; } = e_Production_State.Start;

        // Biến toàn cục để lưu trữ trạng thái sản xuất
        public static bool APP_Ready { get; set; } = false;
        public static bool Device_Ready { get; set; } = false; // Biến toàn cục để kiểm tra trạng thái thiết bị

        public static e_Camera_State CameraMain_State { get; set; } = e_Camera_State.DISCONNECTED;
        public static e_Camera_State CameraSub_State { get; set; } = e_Camera_State.DISCONNECTED;

        public static bool HandScan01_Connected { get; set; } = false;
        public static bool HandScan02_Connected { get; set; } = false;

        public static bool PLC_Connected { get; set; } = false; // Biến toàn cục để kiểm tra kết nối PLC
        public static PLCCounter CameraMain_PLC_Counter { get; set; } = new PLCCounter(); // Biến toàn cục để lưu trữ thông tin đếm sản phẩm từ camera chính

        public static  string Canhbao { get; set; } = string.Empty; // Biến toàn cục để lưu trữ cảnh báo từ camera chính

        public static bool test { get; set; } = false; // Biến toàn cục để kiểm tra trạng thái test

        public static int test2 { get; set; } = 0; // Biến toàn cục để kiểm tra trạng thái test 2

        public static e_awsIot_status AWS_IoT_Status { get; set; } = e_awsIot_status.Disconnected; // Biến toàn cục để kiểm tra trạng thái kết nối AWS IoT
    }

    public static class Globals_Database
    {
        public static Dictionary<string, ProductionCodeData> Dictionary_ProductionCode_Data { get; set; } = new Dictionary<string, ProductionCodeData>(); // Lưu trữ dữ liệu mã sản xuất

        public static Dictionary<int, ProductionCartonData> Dictionary_ProductionCarton_Data { get; set; } = new Dictionary<int, ProductionCartonData>(); // Lưu trữ dữ liệu mã thùng sản xuất

        public static Queue<(string conten, ProductionCodeData data, bool duplicate)> Update_Product_To_SQLite_Queue = new Queue<(string content, ProductionCodeData data, bool duplicate)>();

        public static Queue<ProductionCodeData_Record> Insert_Product_To_Record_Queue = new Queue<ProductionCodeData_Record>();

        public static Queue<ProductionCodeData_Record> Insert_Product_To_Record_CS_Queue = new Queue<ProductionCodeData_Record>();

        public static Queue<ProductionCartonData> Update_Product_To_Record_Carton_Queue = new Queue<ProductionCartonData>();

        public static Queue<string> Activate_Carton = new Queue<string>(); // Hàng đợi kích hoạt mã thùng sản xuất

        public static Queue<AWS_Recive_Data> aWS_Recive_Datas = new Queue<AWS_Recive_Data>(); // Hàng đợi dữ liệu AWS IoT
        public static Queue<AWS_Send_Data> aWS_Send_Datas = new Queue<AWS_Send_Data>(); // Hàng đợi dữ liệu AWS IoT
    }

    #region Các lớp dữ liệu liên quan đến sản xuất

    public class ProductionCodeData
    {
        public string orderNo { get; set; } // Số đơn hàng
        public string Code { get; set; } // Mã sản phẩm
        public int codeID { get; set; } // mã id trong csdl
        public string cartonCode { get; set; } // Mã code thùng
        public string Activate_User { get; set; } // Mã id thùng trong csdl
        public string Main_Camera_Status { get; set; } // 0: Chưa kích hoạt,Active là 1, reject là -1
        public string Sub_Camera_Activate_Datetime { get; set; } // 0: Chưa kích hoạt,Active là 1, reject là -1
        public string Activate_Datetime { get; set; } // Thời gian kích hoạt
        public string Production_Datetime { get; set; } // Thời gian sản xuất
    }

    public class ProductionCodeData_Record
    {
        public string code { get; set; } // Mã sản xuất
        public string cartonCode { get; set; } // Mã code thùng 

        public int cartonID { get; set; } // Mã id thùng trong csdl
        public  e_Production_Status status { get; set; } // Trạng thái sản xuất
        public bool PLCStatus { get; set; } // Trạng thái PLC
        public string Activate_Datetime { get; set; } // Thời gian kích hoạt
        public string Activate_User { get; set; } // Người kích hoạt
        public string Production_Datetime { get; set; } // Thời gian sản xuất
    }

    public class ProductionCartonData
    {
        public int cartonID { get; set; } // Mã id thùng trong csdl
        public string orderNo { get; set; } // Số đơn hàng
        public string cartonCode { get; set; } // Mã code thùng
        public string Activate_User { get; set; } // Người kích hoạt
        public string Start_Datetime { get; set; } // Thời gian kích hoạt
        public string Activate_Datetime { get; set; } // Thời gian kích hoạt
        public string Production_Datetime { get; set; } // Thời gian sản xuất
    }

    #endregion

    #region Các couter lưu tạm
    public class PLCCounter
    {
        public int total { get; set; } = 0; // Tổng số sản phẩm đã sản xuất
        public int total_pass { get; set; } = 0; // Tổng số sản phẩm đã sản xuất thành công
        public int total_failed { get; set; } = 0; // Tổng số sản phẩm đã sản xuất thất bại
        public int camera_read_fail { get; set; } = 0; // Số lượng sản phẩm không đọc được từ camera
    }

    public static class CameraMain_HMI
    {
        public static string Camera_Content { get; set; } = string.Empty; // Nội dung camera chính
        public static e_Production_Status Camera_Status { get; set; } = e_Production_Status.Fail; // Nội dung camera phụ
        public static int ID { get; set; } = 0; // ID của sản phẩm

    }

    public static class CameraSub_HMI
    {
        public static string Camera_Content { get; set; } = string.Empty; // Nội dung camera phụ
        public static e_Production_Status Camera_Status { get; set; } = e_Production_Status.Fail; // Trạng thái sản phẩm từ camera phụ
        public static int ID { get; set; } = 0; // ID của sản phẩm
    }

    public class AWS_Recive_Data
    {
        public int ID { get; set; } // Chủ đề của AWS IoT
        public string recive_Status { get; set; } // Thông báo lỗi nếu có
    }

    public class AWS_Send_Data
    {
        public int ID { get; set; } // Chủ đề của AWS IoT
        public string send_Status { get; set; } // Thông báo lỗi nếu có
    }
    #endregion
}
