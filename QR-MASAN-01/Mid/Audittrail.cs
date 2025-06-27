using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace QR_MASAN_01
{
    public class SystemLogs
    {
        public enum e_LogType
        {
            SYSTEM, // Sự kiện hệ thống chung
            ALL,
            SYSTEM_ERROR,
            USER_ACTION,
            LOGIN,
            SYSTEM_EVENT,
            SAVE_LOG_EXPORT,
            MFI,
            CAMERA_ERROR,
            PLC_ERROR,
            SERVER,
            SERVER_ERROR
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

        //sử dụng hàm này để lấy log từ sqlite trả về dạng datatable, lấy theo số lượng nhận vào từ page 
        //ví dụ: GetLogsFromSQLite(e_LogType.CAMERA, 10, 100) sẽ lấy bản ghi từ 10 đến 100 của loại CAMERA lưu ý lấy từ ID lớn nhất ngược lại
        //tạo 1 long datfrom =  - 30 ngày
        //tạo 1 long dateto = 0 (hiện tại)

        public static DataTable Get_Logs_From_SQLite(e_LogType logType, int Page,  int Size, long DateFrom, long DateTo, bool getALL = false)
        {
            string dbFilePath = "C:/.ABC/TanTienHiTech.dbmmccmsacc"; // Đường dẫn đến file SQLite
            DataTable dataTable = new DataTable();
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("Thời gian", typeof(string));
            dt.Columns.Add("Loại", typeof(string));
            dt.Columns.Add("Nội dung", typeof(string));
            dt.Columns.Add("Người dùng", typeof(string));
            dt.Columns.Add("Chi tiết", typeof(string));
            
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = @"
                    SELECT * FROM SystemLogs 
                    WHERE LogType = @LogType AND TimeUnix >= @DateFrom AND TimeUnix <= @DateTo
                    ORDER BY ID DESC 
                    LIMIT @page, @size";
                if (logType == e_LogType.ALL)
                {
                    query = @"
                        SELECT * FROM SystemLogs 
                        WHERE TimeUnix >= @DateFrom AND TimeUnix <= @DateTo
                        ORDER BY ID DESC 
                        LIMIT @page, @size";
                }
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LogType", logType.ToString());
                    command.Parameters.AddWithValue("@page", Page);
                    command.Parameters.AddWithValue("@size", Size);
                    command.Parameters.AddWithValue("@DateFrom", DateFrom);
                    command.Parameters.AddWithValue("@DateTo", DateTo);
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                        //chuyêbr
                        //đổ dữ liệu vào DataTable dt
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            DataRow row = dataTable.Rows[i];
                            dt.Rows.Add(row["ID"], row["TimeStamp"], row["LogType"], row["Message"], row["User"], row["Details"]);
                        }

                    }
                }
            }
            return dt;
        }

        //lấy số lượng bản ghi của loại log lấy dạng SQLiteDataAdapter để tránh kẹt
        public static int Get_Log_Count(e_LogType logType)
        {
            string dbFilePath = "C:/.ABC/TanTienHiTech.dbmmccmsacc"; // Đường dẫn đến file SQLite
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = @"
                    SELECT COUNT(*) FROM SystemLogs 
                    WHERE LogType = @LogType";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LogType", logType.ToString());

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(dataTable.Rows[0][0]);
                        }
                        else
                        {
                            return 0; // Không có bản ghi nào
                        }
                    }
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

    public class ActiveLogs
    {
        public enum e_ActiveLogType
        {
            SYSTEM, // Sự kiện hệ thống chung
            ALL,
            CHANGE,
            ACTIVE,
            UNACTIVE
        }
        public string TimeStamp { get; set; } // Thời gian xảy ra sự kiện, định dạng ISO 8601
        public long TimeUnix { get; set; }// Thời gian Unix (số giây kể từ 01/01/1970)
        public e_ActiveLogType LogType { get; set; }
        public string Message { get; set; } // Thông điệp mô tả sự kiện hoặc lỗi
        public string User { get; set; } // Người dùng thực hiện hành động, nếu có
        public string Details { get; set; } // Thông tin chi tiết về sự kiện hoặc lỗi

        public ActiveLogs(string timeStamp, long timeUnix, e_ActiveLogType logType, string message, string user = null, string details = null)
        {
            TimeStamp = timeStamp;
            TimeUnix = timeUnix;
            LogType = logType;
            Message = message;
            User = user;
            Details = details;
        }

        //thêm vào hàng chờ chờ thêm vào sqlite
        public static Queue<ActiveLogs> ActiveLogQueue { get; set; } = new Queue<ActiveLogs>();

        // Thêm một bản ghi vào hàng đợi

        //hàm Insert vào sqlite (sqlite helper)

        public static void ActiveInsertToSQLite(ActiveLogs systemLogs_Data)
        {
            //kiểm tra xem file log đã tồn tại chưa, nếu chưa thì tạo mới
            string dbFilePath = "C:/.ABC/TanTienHiTechA.dbmmccmsacc"; // Đường dẫn đến file SQLite
            if (!System.IO.File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
                // Tạo file SQLite mới nếu chưa tồn tại
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    connection.Open();
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS ActiveLogs (
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
                    INSERT INTO ActiveLogs (TimeStamp, TimeUnix, LogType, Message, User, Details)
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

        //sử dụng hàm này để lấy log từ sqlite trả về dạng datatable, lấy theo số lượng nhận vào từ page 
        //ví dụ: GetLogsFromSQLite(e_LogType.CAMERA, 10, 100) sẽ lấy bản ghi từ 10 đến 100 của loại CAMERA lưu ý lấy từ ID lớn nhất ngược lại
        //tạo 1 long datfrom =  - 30 ngày
        //tạo 1 long dateto = 0 (hiện tại)

        public static DataTable Active_Get_Logs_From_SQLite(e_ActiveLogType logType, int Page, int Size, long DateFrom, long DateTo, bool getALL = false)
        {
            string dbFilePath = "C:/.ABC/TanTienHiTechA.dbmmccmsacc"; // Đường dẫn đến file SQLite
            DataTable dataTable = new DataTable();
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("Thời gian", typeof(string));
            dt.Columns.Add("Loại", typeof(string));
            dt.Columns.Add("Nội dung", typeof(string));
            dt.Columns.Add("Người dùng", typeof(string));
            dt.Columns.Add("Chi tiết", typeof(string));

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = @"
                    SELECT * FROM ActiveLogs 
                    WHERE LogType = @LogType AND TimeUnix >= @DateFrom AND TimeUnix <= @DateTo
                    ORDER BY ID DESC 
                    LIMIT @page, @size";
                if (getALL)
                {
                    query = @"
                        SELECT * FROM ActiveLogs 
                        WHERE TimeUnix >= @DateFrom AND TimeUnix <= @DateTo
                        ORDER BY ID DESC 
                        LIMIT @page, @size";
                }
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LogType", logType.ToString());
                    command.Parameters.AddWithValue("@page", Page);
                    command.Parameters.AddWithValue("@size", Size);
                    command.Parameters.AddWithValue("@DateFrom", DateFrom);
                    command.Parameters.AddWithValue("@DateTo", DateTo);
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                        //chuyêbr
                        //đổ dữ liệu vào DataTable dt
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            DataRow row = dataTable.Rows[i];
                            dt.Rows.Add(row["ID"], row["TimeStamp"], row["LogType"], row["Message"], row["User"], row["Details"]);
                        }

                    }
                }
            }
            return dt;
        }

        //lấy số lượng bản ghi của loại log lấy dạng SQLiteDataAdapter để tránh kẹt
        public static int Active_Get_Log_Count(e_ActiveLogType logType)
        {
            string dbFilePath = "C:/.ABC/TanTienHiTechA.dbmmccmsacc"; // Đường dẫn đến file SQLite
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = @"
                    SELECT COUNT(*) FROM ActiveLogs 
                    WHERE LogType = @LogType";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LogType", logType.ToString());

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(dataTable.Rows[0][0]);
                        }
                        else
                        {
                            return 0; // Không có bản ghi nào
                        }
                    }
                }
            }
        }
    }


    public class ProductionLogs
    {
        public string TimeStampSelected { get; set; } // Thời gian xảy ra sự kiện, định dạng ISO 8601
        public long TimeUnixSelected { get; set; } // Thời gian Unix (số giây kể từ 01/01/1970)
        public string orderNO { get; set; }
        public string uniqueCode { get; set; }
        public string factory { get; set; }
        public string site { get; set; }
        public string productionLine { get; set; }
        public string productionDate { get; set; }
        public string shift { get; set; }
        public int productionOutput { get; set; }
        public int expectedOutput { get; set; }
        public string User { get; set; } // Người dùng thực hiện hành động, nếu có
        public string Details { get; set; } // Thông tin chi tiết về sự kiện hoặc lỗi

        public ProductionLogs(
            string timeStampSelected,
            long timeUnixSelected,
            string orderNO,
            string uniqueCode,
            string factory,
            string site,
            string productionLine,
            string productionDate,
            int productionOutput,
            int expectedOutput,
            string user = null,
            string details = null)
        {
            TimeStampSelected = timeStampSelected;
            TimeUnixSelected = timeUnixSelected;
            this.orderNO = orderNO;
            this.uniqueCode = uniqueCode;
            this.factory = factory;
            this.site = site;
            this.productionLine = productionLine;
            this.productionDate = productionDate;
            this.productionOutput = productionOutput;
            this.expectedOutput = expectedOutput;
            User = user;
            Details = details;
        }

        //thêm vào hàng chờ chờ thêm vào sqlite
        public static Queue<ProductionLogs> PO_LogQueue { get; set; } = new Queue<ProductionLogs>();
        // Thêm một bản ghi vào hàng đợi

        //hàm Insert vào sqlite (sqlite helper)

        public static void InsertToSQLite(ProductionLogs log)
        {
            string dbFilePath = "C:/.ABC/TanTienHiTechPL.dbmmccmsacc";
            if (!File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    connection.Open();
                    string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS ProductionLogs (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        TimeStamp TEXT NOT NULL,
                        TimeUnix INTEGER NOT NULL,
                        orderNO TEXT NOT NULL,
                        uniqueCode TEXT NOT NULL,
                        site TEXT NOT NULL,
                        factory TEXT NOT NULL,
                        productionLine TEXT NOT NULL,
                        productionDate TEXT NOT NULL,
                        shift TEXT NOT NULL,
                        productionOutput INTEGER NOT NULL,
                        expectedOutput INTEGER NOT NULL,
                        User TEXT,
                        Details TEXT
                    )";
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string insertQuery = @"
                INSERT INTO ProductionLogs (
                    TimeStamp, TimeUnix, orderNO, uniqueCode, site, factory,
                    productionLine, productionDate, shift, productionOutput,
                    expectedOutput, User, Details
                ) VALUES (
                    @TimeStamp, @TimeUnix, @orderNO, @uniqueCode, @site, @factory,
                    @productionLine, @productionDate, @shift, @productionOutput,
                    @expectedOutput, @User, @Details
                )";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@TimeStamp", log.TimeStampSelected);
                    command.Parameters.AddWithValue("@TimeUnix", log.TimeUnixSelected);
                    command.Parameters.AddWithValue("@orderNO", log.orderNO);
                    command.Parameters.AddWithValue("@uniqueCode", log.uniqueCode);
                    command.Parameters.AddWithValue("@site", log.site);
                    command.Parameters.AddWithValue("@factory", log.factory);
                    command.Parameters.AddWithValue("@productionLine", log.productionLine);
                    command.Parameters.AddWithValue("@productionDate", log.productionDate);
                    command.Parameters.AddWithValue("@shift", log.shift);
                    command.Parameters.AddWithValue("@productionOutput", log.productionOutput);
                    command.Parameters.AddWithValue("@expectedOutput", log.expectedOutput);
                    command.Parameters.AddWithValue("@User", log.User ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Details", log.Details ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }


        //sử dụng hàm này để lấy log từ sqlite trả về dạng datatable, lấy theo số lượng nhận vào từ page 
        //ví dụ: GetLogsFromSQLite(e_LogType.CAMERA, 10, 100) sẽ lấy bản ghi từ 10 đến 100 của loại CAMERA lưu ý lấy từ ID lớn nhất ngược lại
        //tạo 1 long datfrom =  - 30 ngày
        //tạo 1 long dateto = 0 (hiện tại)

        public static DataTable Get_Logs_From_SQLite(int Page, int Size, long DateFrom, long DateTo, bool getALL = false)
        {
            string dbFilePath = "C:/.ABC/TanTienHiTechPL.dbmmccmsacc"; // Đường dẫn đến file SQLite
            DataTable dataTable = new DataTable();
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("Thời gian", typeof(string));
            dt.Columns.Add("OrderNO", typeof(string));

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = @"
                    SELECT * FROM ProductionLogs 
                    WHERE TimeUnix >= @DateFrom AND TimeUnix <= @DateTo
                    ORDER BY ID DESC 
                    LIMIT @page, @size";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@page", Page);
                    command.Parameters.AddWithValue("@size", Size);
                    command.Parameters.AddWithValue("@DateFrom", DateFrom);
                    command.Parameters.AddWithValue("@DateTo", DateTo);
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                        //chuyêbr
                        //đổ dữ liệu vào DataTable dt
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            DataRow row = dataTable.Rows[i];
                            dt.Rows.Add(row["ID"], row["TimeStamp"], row["orderNO"], row["User"]);
                        }

                    }
                }
            }
            return dt;
        }

        //lấy số lượng bản ghi của loại log lấy dạng SQLiteDataAdapter để tránh kẹt
        public static int Get_Log_Count()
        {
            string dbFilePath = "C:/.ABC/TanTienHiTechPL.dbmmccmsacc"; // Đường dẫn đến file SQLite
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = @"
                    SELECT COUNT(*) FROM ProductionLogs 
                    WHERE LogType = @LogType";

                using (var command = new SQLiteCommand(query, connection))
                {

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(dataTable.Rows[0][0]);
                        }
                        else
                        {
                            return 0; // Không có bản ghi nào
                        }
                    }
                }
            }
        }

        public static DataTable GetByOrderNo_Datatable(string orderNo)
        {
            string dbFilePath = "C:/.ABC/TanTienHiTechPL.dbmmccmsacc";
            DataTable dt = new DataTable();

            if (!File.Exists(dbFilePath))
            {
                return dt; // Trả về bảng rỗng nếu file chưa tồn tại
            }

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = @"
            SELECT 
                ID,
                TimeStamp,
                TimeUnix,
                orderNO,
                uniqueCode,
                factory,
                site,
                productionLine,
                productionDate,
                shift,
                productionOutput,
                expectedOutput,
                User,
                Details
            FROM ProductionLogs
            WHERE orderNO = @orderNO
            ORDER BY ID DESC";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderNO", orderNo);

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public static DataTable GetLastLog_Datatable()
        {
            string dbFilePath = "C:/.ABC/TanTienHiTechPL.dbmmccmsacc";
            DataTable dt = new DataTable();

            if (!File.Exists(dbFilePath))
            {
                return dt; // Trả về bảng rỗng nếu file chưa tồn tại
            }

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = @"
            SELECT 
                ID,
                TimeStamp,
                TimeUnix,
                orderNO,
                uniqueCode,
                factory,
                site,
                productionLine,
                productionDate,
                shift,
                productionOutput,
                expectedOutput,
                User,
                Details
            FROM ProductionLogs
            ORDER BY ID DESC
            LIMIT 1"; // Lấy dòng mới nhất

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }



    }
}
