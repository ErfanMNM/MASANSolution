using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using SpT.Auth;
using SpT.Logs;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AwsIotClientHelper;
using HslCommunication;
using SpT;

namespace MASAN_SERIALIZATION
{
    public static class Globals
    {
        // Includes admitted camera workers that have not yet produced persistence work.
        public static int PendingCameraRequests;
        public static readonly object PoTransitionLock = new object();

        public static int CurrentPLCID { get; set; } = 1; // Biến toàn cục để lưu trữ phiên bản hiện tại của ứng dụng
        public static int currentPLCStatus { get; set; } = 5; // Biến toàn cục để lưu trữ phiên bản hiện tại của ứng dụng
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

        public static e_Camera_State e_Hand_Scan_Carton_State { get; set; } = e_Camera_State.DISCONNECTED;

        public static bool PLC_Connected { get; set; } = false; // Biến toàn cục để kiểm tra kết nối PLC

        public static bool PLC_Connected_02 { get; set; } = false;
        public static PLCCounter CameraMain_PLC_Counter { get; set; } = new PLCCounter(); // Biến toàn cục để lưu trữ thông tin đếm sản phẩm từ camera chính

        // Static references to PLC instances (initialized from FDashboard)
        public static OmronPLC_Hsl PLC_Instance { get; set; }
        public static OmronPLC_Hsl PLC_Instance_02 { get; set; }
        public static HslCommunication.Core.IReadWriteNet PLC { get { return PLC_Instance?.plc; } }
        public static HslCommunication.Core.IReadWriteNet PLC_02 { get { return PLC_Instance_02?.plc; } }

        public static PLCCounter CameraSub_PLC_Counter { get; set; } = new PLCCounter(); // Biến toàn cục để lưu trữ thông tin đếm sản phẩm từ camera phụ

        public static  string Canhbao { get; set; } = string.Empty; // Biến toàn cục để lưu trữ cảnh báo từ camera chính

        public static bool test { get; set; } = false; // Biến toàn cục để kiểm tra trạng thái test

        public static int test2 { get; set; } = 0; // Biến toàn cục để kiểm tra trạng thái test 2

        public static e_awsIot_status AWS_IoT_Status { get; set; } = e_awsIot_status.Disconnected; // Biến toàn cục để kiểm tra trạng thái kết nối AWS IoT
    }

    public static class Globals_Database
    {
        public static bool HasPendingWork => System.Threading.Volatile.Read(ref Globals.PendingCameraRequests) > 0 ||
            !Update_Product_To_SQLite_Queue.IsEmpty || !Insert_Product_To_Record_Queue.IsEmpty ||
            !Insert_Product_To_Record_CS_Queue.IsEmpty || !Update_Product_To_Record_Carton_Queue.IsEmpty ||
            !Activate_Carton.IsEmpty || !aWS_Recive_Datas.IsEmpty || !aWS_Send_Datas.IsEmpty;

        public static ProductionCartonData SnapshotCarton(ProductionCartonData value)
        {
            return new ProductionCartonData
            {
                orderNo = value.orderNo, cartonID = value.cartonID, cartonCode = value.cartonCode,
                Activate_User = value.Activate_User, Start_Datetime = value.Start_Datetime,
                Activate_Datetime = value.Activate_Datetime, Production_Datetime = value.Production_Datetime
            };
        }

        public static void EnqueueCartonActivation(string code, string orderNo, string productionDate, int cartonPack)
        {
            lock (Globals.PoTransitionLock)
            {
                if (string.IsNullOrEmpty(productionDate) || productionDate == "0")
                {
                    if (Globals.ProductionData.orderNo != orderNo)
                        throw new InvalidOperationException("Missing production date for captured carton PO.");
                    productionDate = Globals.ProductionData.productionDate;
                }
                Activate_Carton.Enqueue(new CartonActivationRequest
                { cartonCode = code, orderNo = orderNo, productionDate = productionDate, cartonPack = cartonPack });
            }
        }

        public static Dictionary<string, ProductionCodeData> Dictionary_ProductionCode_Data { get; set; } = new Dictionary<string, ProductionCodeData>(); // Lưu trữ dữ liệu mã sản xuất

        public static Dictionary<string, ProductionCodeData> Dictionary_ProductionCode_CameraSub_Data { get; set; } = new Dictionary<string, ProductionCodeData>();

        public static Dictionary<int, ProductionCartonData> Dictionary_ProductionCarton_Data { get; set; } = new Dictionary<int, ProductionCartonData>(); // Lưu trữ dữ liệu mã thùng sản xuất

        public static ConcurrentQueue<(string conten, ProductionCodeData data, bool duplicate)> Update_Product_To_SQLite_Queue = new ConcurrentQueue<(string content, ProductionCodeData data, bool duplicate)>();

        public static ConcurrentQueue<ProductionCodeData_Record> Insert_Product_To_Record_Queue = new ConcurrentQueue<ProductionCodeData_Record>();

        public static ConcurrentQueue<ProductionCodeData_Record> Insert_Product_To_Record_CS_Queue = new ConcurrentQueue<ProductionCodeData_Record>();

        public static ConcurrentQueue<ProductionCartonData> Update_Product_To_Record_Carton_Queue = new ConcurrentQueue<ProductionCartonData>();

        public static ConcurrentQueue<CartonActivationRequest> Activate_Carton = new ConcurrentQueue<CartonActivationRequest>(); // Hàng đợi kích hoạt mã thùng sản xuất

        public static ConcurrentQueue<AWS_Recive_Data> aWS_Recive_Datas = new ConcurrentQueue<AWS_Recive_Data>(); // Hàng đợi dữ liệu AWS IoT
        public static ConcurrentQueue<AWS_Send_Data> aWS_Send_Datas = new ConcurrentQueue<AWS_Send_Data>(); // Hàng đợi dữ liệu AWS IoT
    }

    public sealed class CartonActivationRequest
    {
        public string orderNo { get; set; }
        public string cartonCode { get; set; }
        public string productionDate { get; set; }
        public int cartonPack { get; set; }
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
        public string Sub_Camera_Status { get; set; } // 0: Chưa kích hoạt từ camera phụ, 1: Đã kích hoạt, -1: Lỗi
        public string Sub_Camera_Activate_Datetime { get; set; } // 0: Chưa kích hoạt,Active là 1, reject là -1
        public string Activate_Datetime { get; set; } // Thời gian kích hoạt
        public string Production_Datetime { get; set; } // Thời gian sản xuất
    }

    public class ProductionCodeData_Record
    {
        public string orderNo { get; set; }
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
        public int timeout { get; set; } = 0; // Số lượng sản phẩm bị timeout
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
        public string orderNo { get; set; }
        public int ID { get; set; } // Chủ đề của AWS IoT
        public string recive_Status { get; set; } // Thông báo lỗi nếu có
    }

    public class AWS_Send_Data
    {
        public string orderNo { get; set; }
        public int ID { get; set; } // Chủ đề của AWS IoT
        public string send_Status { get; set; } // Thông báo lỗi nếu có
    }
    #endregion
}
