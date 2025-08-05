using System.Collections.Generic;

namespace MASAN_SERIALIZATION.Utils
{
    /// <summary>
    /// Helper class để migration từ hệ thống mã lỗi cũ sang mới
    /// </summary>
    public static class ErrorMigrationHelper
    {
        /// <summary>
        /// Map từ mã lỗi cũ sang mã lỗi mới
        /// </summary>
        private static readonly Dictionary<string, string> OldToNewErrorCodes = new Dictionary<string, string>
        {
            // Main Application Errors
            { "EM01", ErrorCodes.Main.INIT_UI_FAILED },
            { "EM02", ErrorCodes.Main.INIT_CONFIG_FAILED },
            { "EM03", ErrorCodes.Main.INIT_CONTROLS_FAILED },
            { "EM04", ErrorCodes.Main.INIT_PAGES_FAILED },
            { "EM05", ErrorCodes.Main.MAIN_PROCESS_ERROR },

            // Production Process Errors
            { "PP02", ErrorCodes.Production.ORDER_DELETED },
            { "PP03", ErrorCodes.Production.ORDER_COMPLETED },
            { "PP04", ErrorCodes.Production.ORDER_INSUFFICIENT_CZ },
            { "PP041", ErrorCodes.Production.PROCESS_NOT_FOUND },
            { "PP042", ErrorCodes.Production.PROCESS_NO_ORDERS },
            { "PP05", ErrorCodes.Production.ORDER_GET_RECORDS_FAILED },
            { "PP06", ErrorCodes.Production.ORDER_GET_DETAIL_FAILED },
            { "PP07", ErrorCodes.Production.ORDER_GET_COUNT_FAILED },
            { "PP08", ErrorCodes.Production.ORDER_GET_CODE_COUNT_FAILED },
            { "PP088", ErrorCodes.Production.CRITICAL_SAVE_ERROR },
            { "PP096", ErrorCodes.Production.PROCESS_CANNOT_EDIT_DATE },
            { "PP097", ErrorCodes.Production.PROCESS_ALREADY_RUNNING },
            { "PP098", ErrorCodes.Production.PROCESS_RUNNING_CANNOT_EDIT },
            { "PP100", ErrorCodes.Production.PROCESS_SAVE_DATE_FAILED },
            { "PP231", ErrorCodes.Production.QUEUE_PROCESSING },
            { "PP404", ErrorCodes.Production.CRITICAL_DATABASE_ERROR },
            { "PP590", ErrorCodes.Production.APP_NOT_READY },
            { "PP591", ErrorCodes.Production.DEVICE_NOT_READY },
            { "PP909", ErrorCodes.Production.CRITICAL_SAVE_DB_ERROR },

            // Dashboard Errors
            { "D001", ErrorCodes.Dashboard.DEVICE_INIT_FAILED },
            { "D002", ErrorCodes.Dashboard.TASK_INIT_FAILED },
            { "D022", ErrorCodes.Dashboard.PLC_SEND_FAILED },
            { "D023", ErrorCodes.Dashboard.PLC_SEND_EXCEPTION },
            { "DA01068", ErrorCodes.Dashboard.PLC_READ_COUNTER_FAILED },
            { "DA01069", ErrorCodes.Dashboard.GET_PRODUCT_CODES_FAILED },
            { "DA01070", ErrorCodes.Dashboard.GET_CARTON_CODES_FAILED },
            { "DA0107", ErrorCodes.Dashboard.UI_UPDATE_ERROR },
            { "DT004", ErrorCodes.Dashboard.QUEUE_PROCESS_ERROR },

            // Database Errors
            { "P01", ErrorCodes.Database.PO_CHECK_FAILED },
            { "P02", ErrorCodes.Database.PO_GET_DETAIL_FAILED },
            { "P03", ErrorCodes.Database.PO_GET_CODE_COUNT_FAILED },
            { "P04", ErrorCodes.Database.PO_GET_CODES_FAILED },
            { "P05", ErrorCodes.Database.PO_GET_HISTORY_FAILED },
            { "P06", ErrorCodes.Database.PO_SAVE_FAILED },
            { "P07", ErrorCodes.Database.PO_UPDATE_STATUS_FAILED },
            { "P08", ErrorCodes.Database.PO_UPDATE_CARTON_FAILED },

            // Config Errors
            { "GC01", ErrorCodes.Config.LOAD_ORDER_FAILED },
            { "GC02", ErrorCodes.Config.DATABASE_CHECK_FAILED },

            // Carton Errors
            { "PC01", ErrorCodes.Carton.GENERAL_ERROR },
            { "PC012", ErrorCodes.Carton.NO_CARTON_CODE },

            // Statistics Errors
            { "ST001", ErrorCodes.Statistics.CHART_GENERATION_FAILED },
            { "S123", ErrorCodes.Statistics.DATA_UPDATE_FAILED },

            // Critical Errors
            { "EA001", ErrorCodes.Critical.DATABASE_CORRUPTION }
        };

        /// <summary>
        /// Chuyển đổi mã lỗi cũ sang mã lỗi mới
        /// </summary>
        /// <param name="oldErrorCode">Mã lỗi cũ (VD: PP02, EM01)</param>
        /// <returns>Mã lỗi mới có cấu trúc hoặc mã cũ nếu không tìm thấy</returns>
        public static string MigrateErrorCode(string oldErrorCode)
        {
            if (string.IsNullOrEmpty(oldErrorCode))
                return oldErrorCode;

            // Nếu đã là mã lỗi mới (có dấu -) thì trả về luôn
            if (oldErrorCode.Contains("-"))
                return oldErrorCode;

            // Tìm trong dictionary
            return OldToNewErrorCodes.TryGetValue(oldErrorCode, out string newCode) ? newCode : oldErrorCode;
        }

        /// <summary>
        /// Tạo error message theo format mới
        /// </summary>
        /// <param name="oldErrorCode">Mã lỗi cũ</param>
        /// <param name="message">Thông điệp</param>
        /// <returns>Error message với format mới</returns>
        public static string CreateErrorMessage(string oldErrorCode, string message)
        {
            var newErrorCode = MigrateErrorCode(oldErrorCode);
            var description = ErrorCodes.GetErrorDescription(newErrorCode);
            
            return $"[{newErrorCode}] {description}: {message}";
        }

        /// <summary>
        /// Danh sách tất cả các mã lỗi cũ để tìm kiếm và thay thế
        /// </summary>
        public static IEnumerable<string> GetAllOldErrorCodes()
        {
            return OldToNewErrorCodes.Keys;
        }

        /// <summary>
        /// Lấy mapping từ cũ sang mới
        /// </summary>
        public static Dictionary<string, string> GetMigrationMap()
        {
            return new Dictionary<string, string>(OldToNewErrorCodes);
        }
    }
}