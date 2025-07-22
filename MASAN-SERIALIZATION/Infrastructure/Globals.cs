using MASAN_SERIALIZATION.Enums;
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

    }
}
