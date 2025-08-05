using System.Collections.Generic;

namespace MASAN_SERIALIZATION.Utils
{
    /// <summary>
    /// Hệ thống mã lỗi có cấu trúc cho ứng dụng MASAN-SERIALIZATION
    /// Format: [Module][Category][Number] - VD: EM-INIT-001
    /// </summary>
    public static class ErrorCodes
    {
        #region Main Application Errors (EM - Error Main)
        public static class Main
        {
            // Initialization Errors
            public const string INIT_UI_FAILED = "EM-INIT-001";           // Lỗi khởi tạo giao diện
            public const string INIT_CONFIG_FAILED = "EM-INIT-002";       // Lỗi khởi tạo cấu hình
            public const string INIT_CONTROLS_FAILED = "EM-INIT-003";     // Lỗi khởi tạo controls
            public const string INIT_PAGES_FAILED = "EM-INIT-004";        // Lỗi khởi tạo pages
            
            // Processing Errors
            public const string MAIN_PROCESS_ERROR = "EM-PROC-005";       // Lỗi trong quá trình xử lý chính
        }
        #endregion

        #region Production Order Errors (PP - Production Process)
        public static class Production
        {
            // Order Management
            public const string ORDER_DELETED = "PP-ORD-001";             // Đơn hàng đã bị xóa
            public const string ORDER_COMPLETED = "PP-ORD-002";           // Đơn hàng đã hoàn thành
            public const string ORDER_NOT_EXISTS = "PP-ORD-003";          // Đơn hàng không tồn tại
            public const string ORDER_INSUFFICIENT_CZ = "PP-ORD-004";     // Số lượng mã CZ chưa đủ
            public const string ORDER_GET_RECORDS_FAILED = "PP-ORD-005";  // Lấy dữ liệu đơn hàng thất bại
            public const string ORDER_GET_DETAIL_FAILED = "PP-ORD-006";   // Lấy thông tin đơn hàng thất bại
            public const string ORDER_GET_COUNT_FAILED = "PP-ORD-007";    // Lấy số lượng record thất bại
            public const string ORDER_GET_CODE_COUNT_FAILED = "PP-ORD-008"; // Lấy số lượng mã code thất bại
            public const string ORDER_GET_STATUS_COUNT_FAILED = "PP-ORD-009"; // Lấy số lượng theo trạng thái thất bại
            
            // Process States
            public const string PROCESS_NOT_FOUND = "PP-PROC-041";        // Đơn hàng không tồn tại trong danh sách
            public const string PROCESS_NO_ORDERS = "PP-PROC-042";        // Không có đơn hàng trong hệ thống
            public const string PROCESS_ALREADY_RUNNING = "PP-PROC-097";  // Đơn hàng đã sản xuất
            public const string PROCESS_CANNOT_EDIT_DATE = "PP-PROC-096"; // Không thể chỉnh ngày sản xuất
            public const string PROCESS_RUNNING_CANNOT_EDIT = "PP-PROC-098"; // Đang chạy không thể chỉnh
            public const string PROCESS_SAVE_DATE_FAILED = "PP-PROC-100"; // Lưu ngày sản xuất thất bại
            
            // Database Queue
            public const string QUEUE_PROCESSING = "PP-QUEUE-231";        // Đang xử lý hàng đợi database
            
            // System Ready Check
            public const string APP_NOT_READY = "PP-SYS-590";             // Ứng dụng chưa sẵn sàng
            public const string DEVICE_NOT_READY = "PP-SYS-591";          // Thiết bị chưa sẵn sàng
            
            // Critical Errors
            public const string CRITICAL_DATABASE_ERROR = "PP-CRIT-404";  // Không tìm thấy đơn hàng (404 error)
            public const string CRITICAL_SAVE_ERROR = "PP-SAVE-088";      // Lỗi khi lưu
            public const string CRITICAL_SAVE_DB_ERROR = "PP-SAVE-909";   // Lỗi lưu database
        }
        #endregion

        #region Dashboard Errors (DA - Dashboard)
        public static class Dashboard
        {
            // Device Initialization
            public const string DEVICE_INIT_FAILED = "DA-DEV-001";        // Lỗi khởi tạo thiết bị
            public const string TASK_INIT_FAILED = "DA-DEV-002";          // Lỗi khởi tạo task
            
            // PLC Communication
            public const string PLC_SEND_FAILED = "DA-PLC-022";           // Lỗi gửi dữ liệu đến PLC
            public const string PLC_SEND_EXCEPTION = "DA-PLC-023";        // Exception khi gửi PLC
            public const string PLC_READ_COUNTER_FAILED = "DA-PLC-068";   // Lỗi đọc counter từ PLC
            
            // Data Processing
            public const string GET_PRODUCT_CODES_FAILED = "DA-DATA-069"; // Lỗi lấy dữ liệu mã sản phẩm
            public const string GET_CARTON_CODES_FAILED = "DA-DATA-070";  // Lỗi lấy dữ liệu mã thùng
            
            // UI Update
            public const string UI_UPDATE_ERROR = "DA-UI-107";            // Lỗi cập nhật UI
            
            // Queue Processing
            public const string QUEUE_PROCESS_ERROR = "DA-QUEUE-004";     // Lỗi xử lý hàng đợi
        }
        #endregion

        #region Database Errors (DB - Database)
        public static class Database
        {
            // Production Order Database
            public const string PO_CHECK_FAILED = "DB-PO-001";            // Lỗi kiểm tra PO database
            public const string PO_GET_DETAIL_FAILED = "DB-PO-002";       // Lỗi lấy chi tiết PO
            public const string PO_GET_CODE_COUNT_FAILED = "DB-PO-003";   // Lỗi lấy số lượng mã CZ
            public const string PO_GET_CODES_FAILED = "DB-PO-004";        // Lỗi lấy danh sách mã CZ
            public const string PO_GET_HISTORY_FAILED = "DB-PO-005";      // Lỗi lấy lịch sử PO
            public const string PO_SAVE_FAILED = "DB-PO-006";             // Lỗi ghi PO vào database
            public const string PO_UPDATE_STATUS_FAILED = "DB-PO-007";    // Lỗi cập nhật trạng thái
            public const string PO_UPDATE_CARTON_FAILED = "DB-PO-008";    // Lỗi cập nhật thông tin thùng
            
            // Record Operations
            public const string GET_RECORD_COUNT_FAILED = "DB-REC-006";   // Lỗi lấy số lượng record
            public const string GET_RECORD_STATUS_FAILED = "DB-REC-007";  // Lỗi lấy record theo status
            public const string GET_AWS_RECORD_FAILED = "DB-REC-008";     // Lỗi lấy AWS record
            public const string GET_RECORDS_FAILED = "DB-REC-009";        // Lỗi lấy danh sách records
            public const string GET_CARTON_ID_FAILED = "DB-REC-010";      // Lỗi lấy carton ID
            public const string GET_CODES_LIST_FAILED = "DB-REC-011";     // Lỗi lấy danh sách codes
        }
        #endregion

        #region Configuration Errors (CF - Config)
        public static class Config
        {
            public const string LOAD_ORDER_FAILED = "CF-LOAD-001";        // Lỗi tải danh sách orderNo
            public const string DATABASE_CHECK_FAILED = "CF-DB-002";      // Lỗi kiểm tra database file
        }
        #endregion

        #region Carton Dashboard Errors (PC - Panel Carton)
        public static class Carton
        {
            public const string GENERAL_ERROR = "PC-GEN-001";             // Lỗi chung carton dashboard
            public const string NO_CARTON_CODE = "PC-CART-012";           // Chưa có mã thùng
            public const string PREVIOUS_CARTON_NOT_READY = "PC-CART-013"; // Thùng trước chưa sẵn sàng
        }
        #endregion

        #region Statistics Errors (ST - Statistics)
        public static class Statistics
        {
            public const string CHART_GENERATION_FAILED = "ST-CHART-001"; // Lỗi tạo biểu đồ
            public const string DATA_UPDATE_FAILED = "ST-DATA-123";       // Lỗi cập nhật dữ liệu
        }
        #endregion

        #region Critical System Errors (EA - Emergency Alert)
        public static class Critical
        {
            public const string DATABASE_CORRUPTION = "EA-CRIT-001";      // Lỗi nghiêm trọng database
        }
        #endregion

        /// <summary>
        /// Lấy thông tin mô tả lỗi từ mã lỗi
        /// </summary>
        public static string GetErrorDescription(string errorCode)
        {
            var descriptions = new Dictionary<string, string>
            {
                // Main Application
                { Main.INIT_UI_FAILED, "Lỗi khởi tạo giao diện người dùng" },
                { Main.INIT_CONFIG_FAILED, "Lỗi khởi tạo cấu hình hệ thống" },
                { Main.INIT_CONTROLS_FAILED, "Lỗi khởi tạo các control giao diện" },
                { Main.INIT_PAGES_FAILED, "Lỗi khởi tạo các trang ứng dụng" },
                { Main.MAIN_PROCESS_ERROR, "Lỗi trong quá trình xử lý chính" },
                
                // Production Order
                { Production.ORDER_DELETED, "Đơn hàng đã bị xóa" },
                { Production.ORDER_COMPLETED, "Đơn hàng đã hoàn thành" },
                { Production.ORDER_NOT_EXISTS, "Đơn hàng không tồn tại trong hệ thống" },
                { Production.ORDER_INSUFFICIENT_CZ, "Số lượng mã CZ gửi xuống chưa đủ" },
                { Production.ORDER_GET_RECORDS_FAILED, "Lấy dữ liệu đơn hàng thất bại" },
                { Production.ORDER_GET_DETAIL_FAILED, "Lấy thông tin chi tiết đơn hàng thất bại" },
                { Production.ORDER_GET_COUNT_FAILED, "Lấy số lượng record thất bại" },
                { Production.ORDER_GET_CODE_COUNT_FAILED, "Lấy thông tin số lượng mã code thất bại" },
                { Production.ORDER_GET_STATUS_COUNT_FAILED, "Lấy số lượng record theo trạng thái thất bại" },
                
                { Production.PROCESS_NOT_FOUND, "Đơn hàng không tồn tại trong danh sách" },
                { Production.PROCESS_NO_ORDERS, "Không có đơn hàng nào trong hệ thống MES" },
                { Production.PROCESS_ALREADY_RUNNING, "Đơn hàng đã sản xuất, không thể đổi đơn khác" },
                { Production.PROCESS_CANNOT_EDIT_DATE, "Đơn hàng chưa chạy, không thể chỉnh ngày sản xuất" },
                { Production.PROCESS_RUNNING_CANNOT_EDIT, "Đang chạy sản xuất, không thể chỉnh ngày sản xuất" },
                { Production.PROCESS_SAVE_DATE_FAILED, "Lưu ngày sản xuất thất bại" },
                
                { Production.QUEUE_PROCESSING, "Đang có dữ liệu được ghi vào cơ sở dữ liệu, vui lòng đợi" },
                
                { Production.APP_NOT_READY, "Ứng dụng chưa sẵn sàng, vui lòng kiểm tra lại" },
                { Production.DEVICE_NOT_READY, "Thiết bị chưa sẵn sàng, vui lòng kiểm tra lại" },
                
                { Production.CRITICAL_DATABASE_ERROR, "Lỗi nghiêm trọng - Không tìm thấy dữ liệu đơn hàng" },
                { Production.CRITICAL_SAVE_ERROR, "Lỗi nghiêm trọng khi lưu dữ liệu" },
                { Production.CRITICAL_SAVE_DB_ERROR, "Lỗi lưu dữ liệu vào cơ sở dữ liệu" },
                
                // Dashboard
                { Dashboard.DEVICE_INIT_FAILED, "Lỗi khởi tạo thiết bị" },
                { Dashboard.TASK_INIT_FAILED, "Lỗi khởi tạo task" },
                { Dashboard.PLC_SEND_FAILED, "Lỗi gửi dữ liệu đến PLC" },
                { Dashboard.PLC_SEND_EXCEPTION, "Exception khi gửi dữ liệu đến PLC" },
                { Dashboard.PLC_READ_COUNTER_FAILED, "Lỗi đọc dữ liệu đếm từ PLC" },
                { Dashboard.GET_PRODUCT_CODES_FAILED, "Lỗi lấy dữ liệu mã sản phẩm" },
                { Dashboard.GET_CARTON_CODES_FAILED, "Lỗi lấy dữ liệu mã thùng" },
                { Dashboard.UI_UPDATE_ERROR, "Lỗi trong quá trình xử lý giao diện" },
                { Dashboard.QUEUE_PROCESS_ERROR, "Lỗi trong quá trình xử lý hàng đợi" },
                
                // Database
                { Database.PO_CHECK_FAILED, "Lỗi kiểm tra cơ sở dữ liệu PO" },
                { Database.PO_GET_DETAIL_FAILED, "Lỗi lấy thông tin chi tiết PO" },
                { Database.PO_GET_CODE_COUNT_FAILED, "Lỗi lấy số lượng mã CZ" },
                { Database.PO_GET_CODES_FAILED, "Lỗi lấy danh sách mã CZ" },
                { Database.PO_GET_HISTORY_FAILED, "Lỗi lấy lịch sử PO" },
                { Database.PO_SAVE_FAILED, "Lỗi ghi PO vào cơ sở dữ liệu" },
                { Database.PO_UPDATE_STATUS_FAILED, "Lỗi cập nhật trạng thái kích hoạt" },
                { Database.PO_UPDATE_CARTON_FAILED, "Lỗi cập nhật thông tin thùng carton" },
                
                // Config
                { Config.LOAD_ORDER_FAILED, "Lỗi tải danh sách orderNo" },
                { Config.DATABASE_CHECK_FAILED, "Lỗi kiểm tra file cơ sở dữ liệu" },
                
                // Carton
                { Carton.GENERAL_ERROR, "Lỗi chung carton dashboard" },
                { Carton.NO_CARTON_CODE, "Chưa có mã thùng, không xử lý tiếp" },
                { Carton.PREVIOUS_CARTON_NOT_READY, "Không có mã thùng trước đó" },
                
                // Statistics
                { Statistics.CHART_GENERATION_FAILED, "Lỗi tạo biểu đồ" },
                { Statistics.DATA_UPDATE_FAILED, "Lỗi cập nhật dữ liệu thống kê" },
                
                // Critical
                { Critical.DATABASE_CORRUPTION, "Lỗi nghiêm trọng - Vui lòng liên hệ nhà cung cấp" }
            };

            return descriptions.TryGetValue(errorCode, out string description) ? description : "Lỗi không xác định";
        }

        /// <summary>
        /// Kiểm tra xem mã lỗi có phải là lỗi nghiêm trọng không
        /// </summary>
        public static bool IsCriticalError(string errorCode)
        {
            return errorCode.StartsWith("EA-") || 
                   errorCode == Production.CRITICAL_DATABASE_ERROR ||
                   errorCode == Production.CRITICAL_SAVE_ERROR ||
                   errorCode == Production.CRITICAL_SAVE_DB_ERROR;
        }

        /// <summary>
        /// Lấy module từ mã lỗi
        /// </summary>
        public static string GetModule(string errorCode)
        {
            if (string.IsNullOrEmpty(errorCode)) return "UNKNOWN";

            var parts = errorCode.Split('-');
            if (parts.Length < 2) return "UNKNOWN";

            switch (parts[0])
            {
                case "EM": return "Main Application";
                case "PP": return "Production Process";
                case "DA": return "Dashboard";
                case "DB": return "Database";
                case "CF": return "Configuration";
                case "PC": return "Carton Management";
                case "ST": return "Statistics";
                case "EA": return "Critical System";
                default: return "Unknown Module";
            }
        }
    }
}