using SpT.Logs;
using System.Threading.Tasks;

namespace MASAN_SERIALIZATION.Utils
{
    /// <summary>
    /// Extension methods cho LogHelper để hỗ trợ mã lỗi có cấu trúc
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Ghi log với mã lỗi có cấu trúc
        /// </summary>
        /// <typeparam name="T">Enum type cho log type</typeparam>
        /// <param name="logHelper">LogHelper instance</param>
        /// <param name="user">Tên người dùng</param>
        /// <param name="logType">Loại log</param>
        /// <param name="errorCode">Mã lỗi có cấu trúc (VD: EM-INIT-001)</param>
        /// <param name="message">Thông điệp bổ sung</param>
        /// <param name="details">Chi tiết lỗi (thường là exception message)</param>
        public static async Task WriteLogWithCodeAsync<T>(
            this LogHelper<T> logHelper,
            string user,
            T logType,
            string errorCode,
            string message = null,
            string details = null) where T : struct, System.Enum // Updated constraint to match LogHelper<TAction>
        {
            var errorDescription = ErrorCodes.GetErrorDescription(errorCode);
            var module = ErrorCodes.GetModule(errorCode);
            var isCritical = ErrorCodes.IsCriticalError(errorCode);

            var logMessage = $"[{errorCode}] [{module}] {errorDescription}";

            if (!string.IsNullOrEmpty(message))
            {
                logMessage += $" - {message}";
            }

            if (!string.IsNullOrEmpty(details))
            {
                logMessage += $" | Chi tiết: {details}";
            }

            if (isCritical)
            {
                logMessage = $"🚨 CRITICAL ERROR: {logMessage}";
            }

            await logHelper.WriteLogAsync(user, logType, logMessage);
        }

        /// <summary>
        /// Ghi log với mã lỗi có cấu trúc (phiên bản đồng bộ)
        /// </summary>
        /// <typeparam name="T">Enum type cho log type</typeparam>
        /// <param name="logHelper">LogHelper instance</param>
        /// <param name="user">Tên người dùng</param>
        /// <param name="logType">Loại log</param>
        /// <param name="errorCode">Mã lỗi có cấu trúc</param>
        /// <param name="message">Thông điệp bổ sung</param>
        /// <param name="details">Chi tiết lỗi</param>
        public static void WriteLogWithCode<T>(
            this LogHelper<T> logHelper,
            string user,
            T logType,
            string errorCode,
            string message = null,
            string details = null) where T : struct, System.Enum // Updated constraint to match LogHelper<TAction>
        {
            WriteLogWithCodeAsync(logHelper, user, logType, errorCode, message, details).Wait();
        }

        /// <summary>
        /// Ghi log lỗi nhanh với Exception
        /// </summary>
        /// <typeparam name="T">Enum type cho log type</typeparam>
        /// <param name="logHelper">LogHelper instance</param>
        /// <param name="user">Tên người dùng</param>
        /// <param name="logType">Loại log</param>
        /// <param name="errorCode">Mã lỗi có cấu trúc</param>
        /// <param name="ex">Exception</param>
        /// <param name="customMessage">Thông điệp tùy chỉnh</param>
        public static async Task WriteErrorLogAsync<T>(
            this LogHelper<T> logHelper,
            string user,
            T logType,
            string errorCode,
            System.Exception ex,
            string customMessage = null) where T : struct, System.Enum // Updated constraint to match LogHelper<TAction>
        {
            var message = customMessage ?? ex.Message;
            var details = $"Exception: {ex.GetType().Name} | StackTrace: {ex.StackTrace}";

            await WriteLogWithCodeAsync(logHelper, user, logType, errorCode, message, details);
        }

        /// <summary>
        /// Ghi log thông tin với mã code
        /// </summary>
        /// <typeparam name="T">Enum type cho log type</typeparam>
        /// <param name="logHelper">LogHelper instance</param>
        /// <param name="user">Tên người dùng</param>
        /// <param name="logType">Loại log</param>
        /// <param name="code">Mã thông tin (VD: INFO-001)</param>
        /// <param name="message">Thông điệp</param>
        public static async Task WriteInfoLogAsync<T>(
            this LogHelper<T> logHelper,
            string user,
            T logType,
            string code,
            string message) where T : struct, System.Enum // Updated constraint to match LogHelper<TAction>
        {
            var logMessage = $"[{code}] {message}";
            await logHelper.WriteLogAsync(user, logType, logMessage);
        }
    }
}