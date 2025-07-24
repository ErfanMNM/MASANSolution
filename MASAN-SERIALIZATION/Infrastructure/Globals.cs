using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using SpT.Auth;
using SpT.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Biến toàn cục để lưu trữ trạng thái sản xuất
        public static e_Production_State Production_State { get; set; } = e_Production_State.Start;

        // Biến toàn cục để lưu trữ trạng thái sản xuất
        public static bool APP_Ready { get; set; } = false;

        public static e_Camera_State CameraMain_State { get; set; } = e_Camera_State.DISCONNECTED;
        public static e_Camera_State CameraSub_State { get; set; } = e_Camera_State.DISCONNECTED;

        public static bool PLC_Connected { get; set; } = false; // Biến toàn cục để kiểm tra kết nối PLC
    }
}
