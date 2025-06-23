using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_MASAN_01
{
    public class SystemLogs
    {
        public enum e_LogType
        {
            CAMERA,
            PLC,
            ERROR,
            USER_ACTION,
            SYSTEM_EVENT
        }

        public string TimeStamp { get; set; } // Thời gian xảy ra sự kiện, định dạng ISO 8601
        public long TimeUnix { get;set; }// Thời gian Unix (số giây kể từ 01/01/1970)
        public e_LogType LogType { get; set; }
        public string Message { get; set; } // Thông điệp mô tả sự kiện hoặc lỗi
        public string User { get; set; } // Người dùng thực hiện hành động, nếu có
        public string Details { get; set; } // Thông tin chi tiết về sự kiện hoặc lỗi

        public SystemLogs(string timeStamp, long timeUnix , e_LogType logType, string message, string user = null, string details = null)
        {
            TimeStamp = timeStamp;
            TimeUnix = timeUnix;
            LogType = logType;
            Message = message;
            User = user;
            Details = details;
        }

        //thêm vào hàng chờ chờ thêm vào sqlite
        public static Queue<SystemLogs> LogQueue { get; set; } = new Queue<SystemLogs>();
        // Thêm một bản ghi vào hàng đợi

        //hàm Insert vào sqlite (sqlite helper)
        
        public static void InsertToSQLite(SystemLogs systemLogs_Data)
        {
            //kiểm tra xem file log đã tồn tại chưa, nếu chưa thì tạo mới
            string dbFilePath = "C:/.ABC/TanTienHiTech.dbmmccmsacc"; // Đường dẫn đến file SQLite
            if (!System.IO.File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
                // Tạo file SQLite mới nếu chưa tồn tại
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    connection.Open();
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS SystemLogs (
                            ID INTEGER NOT NULL UNIQUE,
                            TimeStamp TEXT NOT NULL,
                            TimeUnix INTEGER NOT NULL,
                            LogType TEXT NOT NULL,
                            Message TEXT NOT NULL,
                            User TEXT NOT NULL,
                            Details TEXT NOT NULL,
                            PRIMARY KEY(ID AUTOINCREMENT)
                        )";
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            //thêm bản ghi vào bảng SystemLogs
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string insertQuery = @"
                    INSERT INTO SystemLogs (TimeStamp, TimeUnix, LogType, Message, User, Details)
                    VALUES (@TimeStamp, @TimeUnix, @LogType, @Message, @User, @Details)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@TimeStamp", systemLogs_Data.TimeStamp);
                    command.Parameters.AddWithValue("@TimeUnix", systemLogs_Data.TimeUnix);
                    command.Parameters.AddWithValue("@LogType", systemLogs_Data.LogType.ToString());
                    command.Parameters.AddWithValue("@Message", systemLogs_Data.Message);
                    command.Parameters.AddWithValue("@User", systemLogs_Data.User ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Details", systemLogs_Data.Details ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }

        }

    }

    public class MFI_Logs
    {
        public enum e_LogType
        {
            SERVER,
            SET,
            GET,
            USER_ACTION,
            COUTN_SAVE
        }
        public string TimeStamp { get; set; } // Thời gian xảy ra sự kiện, định dạng ISO 8601
        public long TimeUnix { get; set; } // Thời gian Unix (số giây kể từ 01/01/1970)
        public e_LogType LogType { get; set; }
        public string Message { get; set; } // Thông điệp mô tả sự kiện hoặc lỗi
        public string User { get; set; } // Người dùng thực hiện hành động, nếu có
        public string Details { get; set; } // Thông tin chi tiết về sự kiện hoặc lỗi
        public MFI_Logs(string timeStamp, long timeUnix, e_LogType logType, string message, string user = null, string details = null)
        {
            TimeStamp = timeStamp;
            TimeUnix = timeUnix;
            LogType = logType;
            Message = message;
            User = user;
            Details = details;
        }
    }
}
