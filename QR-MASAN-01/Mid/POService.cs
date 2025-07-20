
using MainClass.Enum;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QR_MASAN_01
{
    public class POService
    {
        private string _connectionString_PO_MES;
        private string _codes_Path_CZ_DB_MES;
        string poPath = "Databases/PO.tlog";

        public POService()
        {
            _connectionString_PO_MES = $@"Data Source={Setting.Current.PO_Data_path}po.db;Version=3;";
            _codes_Path_CZ_DB_MES = Setting.Current.PO_Data_path + "codes" ;
        }

        //Đẩy danh sách orderNo từ POInfo của MES vào ComboBox
        public void MES_Load_OrderNo_ToComboBox(UIComboBox comboBox)
        {
            using (var conn = new SQLiteConnection(_connectionString_PO_MES))
            {
                string query = "SELECT DISTINCT orderNo FROM POInfo ORDER BY orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();

                adapter.Fill(table);
                // Thêm một dòng rỗng vào đầu danh sách
                DataRow emptyRow = table.NewRow();
                emptyRow["orderNo"] = "Chọn orderNO"; // Hoặc để trống
                table.Rows.InsertAt(emptyRow, 0);
                // Thiết lập DataSource cho ComboBox
                comboBox.DataSource = table;
                comboBox.DisplayMember = "orderNo";
                comboBox.ValueMember = "orderNo";
            }
        }

        //lấy danh sách PO MES trả về dataTable
        public DataTable MES_Get_PO_List()
        {
            using (var conn = new SQLiteConnection(_connectionString_PO_MES))
            {
                string query = "SELECT * FROM po_records ORDER BY orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        //lấy thông tin PO theo orderNo từ MES trả về dataTable
        public DataTable Get_PO_Info_By_OrderNo(string orderNo)
        {
            using (var conn = new SQLiteConnection(_connectionString_PO_MES))
            {
                string query = "SELECT * FROM POInfo WHERE orderNo = @orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);

                adapter.SelectCommand.Parameters.AddWithValue("@orderNo", orderNo);

                var table = new DataTable();
                adapter.Fill(table);

                //thêm cột số mã CZ

                table.Columns.Add("UniqueCodeCount", typeof(int));
                //lấy số mã CZ trong thư mục _codesPath/<orderNo>.db
                int uniqueCodeCount = Get_Unique_Code_MES_Count(orderNo);
                //cập nhật số mã CZ vào cột UniqueCodeCount
                foreach (DataRow row in table.Rows)
                {
                    row["UniqueCodeCount"] = uniqueCodeCount;
                }

                return table;
            }
        }

        //lấy số count mã CZ nằm trong thư mục _codesPath/<orderNo>.db SELECT COUNT(*) FROM `UniqueCodes`;
        public int Get_Unique_Code_MES_Count(string orderNo)
        {
            try
            {
                string czpath = _codes_Path_CZ_DB_MES + "/" + orderNo + ".db";
                using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM UniqueCodes";
                    var command = new SQLiteCommand(query, conn);
                    command.Parameters.AddWithValue("@orderNo", orderNo);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }
            catch
            {
                return 0;
            }
        }

        
        //lấy số count mã czRun nằm trong thư mục C:/.ABC/MM-yy/<orderNo>.db SELECT COUNT(*) FROM `UniqueCodes`;
        public int Get_CZRun_Count(string orderNo)
        {

            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";
            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                string query = "SELECT COUNT(*) FROM UniqueCodes";
                var command = new SQLiteCommand(query, conn);
                conn.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }

        }

        public DataTable Get_Unique_Codes_Run(string orderNo)
        {
            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";
           
                using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    string query = "SELECT * FROM UniqueCodes";
                    var adapter = new SQLiteDataAdapter(query, conn);
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
        }

        public DataTable Get_Unique_Codes_Run_Send_Pending(string orderNo)
        {
            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";

            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                string query = "SELECT \"_rowid_\",* FROM \"main\".\"UniqueCodes\" WHERE \"Send_Status\" = 'pending'  AND \"Status\" != '0' ";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public DataTable Get_Unique_Codes_Run_Sent_Recive_OK(string orderNo)
        {
            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";

            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                string query = "SELECT \"_rowid_\",* FROM \"main\".\"UniqueCodes\" WHERE \"Send_Status\" = 'sent'  AND \"Status\" != '0' AND \"Recive_Status\" != 'waiting' ";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }
        public DataTable Get_Unique_Codes_MES(string orderNo)
        {
            string czpath = _codes_Path_CZ_DB_MES + "/" + orderNo + ".db";
            using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
            {
                string query = "SELECT * FROM UniqueCodes";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        //tạo PO cho sản xuất gồm bảng lưu thông tin các PO đã dùng sản xuất và lịch sử chốt sổ (tách lô) và lịch sử đổi date
        //kiểm tra PO db đã tồn tại hay chưa, nếu chưa tạo mới
        
        public void RunPO(string orderNo, string productionDate)
        {
            DateTime date = DateTime.ParseExact(productionDate, "yyyy-MM-dd", null);
            // Format lại thành yyyy-MM-dd HH:mm:ss.fff
            string output = date.ToString("yyyy-MM-dd HH:mm:ss.fff");

            Check_PO_Log_File();
            
            Check_Run_File(orderNo); //kiểm tra xem file run đã tồn tại chưa, nếu chưa thì tạo mới

            //lịch sử các po
            DataTable POHis_list = new DataTable();
            POHis_list = Get_PO_Run_History_Info_By_OrderNo(orderNo);

            //nếu chưa từng tồn tại thì tạo mới
            if(POHis_list.Rows.Count == 0)
            {
                using (var conn = new SQLiteConnection($"Data Source={poPath};Version=3;"))
                {
                    conn.Open();
                    string insertQuery = @"
                        INSERT INTO PO (orderNO, productionDate, Action, UserName, Counter, Timestamp, Timeunix)
                        VALUES (@orderNo, @productionDate, 'CREATE', @UserName, '{}', @Timestamp, @Timeunix)";
                    var command = new SQLiteCommand(insertQuery, conn);
                    command.Parameters.AddWithValue("@orderNo", orderNo);
                    command.Parameters.AddWithValue("@productionDate", productionDate);
                    command.Parameters.AddWithValue("@UserName", Globalvariable.CurrentUser.Username);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
                    command.Parameters.AddWithValue("@Timeunix", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
                    command.ExecuteNonQuery();
                }
            }

            //kiểm tra xem PO dùng lần cuối có trùng với PO đang tạo hay không, nếu trùng không làm gì cả, nếu không trùng thì cập nhật lại PO dùng lần cuối
            DataRow last_Used_PO = Get_Last_Used_PO();

            if (last_Used_PO != null)
            {

                //kiểm tra xem đang chọn order khác hay tạo mới, nếu khác thì cập nhật lại PO dùng lần cuối INSERT thêm dòng update chứ không update trực tiếp
                if (last_Used_PO["orderNO"].ToString() != orderNo)
                {
                    using (var conn = new SQLiteConnection($"Data Source={poPath};Version=3;"))
                    {
                        conn.Open();
                        string insertQuery = @"
                            INSERT INTO PO (orderNO, productionDate, Action, UserName, Counter, Timestamp, Timeunix)
                            VALUES (@orderNo, @productionDate, 'UPDATE', @UserName, '{}', @Timestamp, @Timeunix)";
                        var command = new SQLiteCommand(insertQuery, conn);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        command.Parameters.AddWithValue("@productionDate", productionDate);
                        command.Parameters.AddWithValue("@UserName", Globalvariable.CurrentUser.Username);
                        command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
                        command.Parameters.AddWithValue("@Timeunix", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
                        command.ExecuteNonQuery();
                    }
                }

                //kiểm tra xem productionDate có khác với PO dùng lần cuối không, nếu khác thì cập nhật lại productionDate INSERT thêm dòng với action = UPDATE chứ không update
                if (last_Used_PO["productionDate"].ToString() != productionDate)
                {
                    using (var conn = new SQLiteConnection($"Data Source={poPath};Version=3;"))
                    {
                        conn.Open();
                        string insertQuery = @"
                            INSERT INTO PO (orderNO, productionDate, Action, UserName, Counter, Timestamp, Timeunix)
                            VALUES (@orderNo, @productionDate, 'UPDATE', @UserName, '{}', @Timestamp, @Timeunix)";
                        var command = new SQLiteCommand(insertQuery, conn);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        command.Parameters.AddWithValue("@productionDate", productionDate);
                        command.Parameters.AddWithValue("@UserName", Globalvariable.CurrentUser.Username);
                        command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
                        command.Parameters.AddWithValue("@Timeunix", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
                        command.ExecuteNonQuery();
                    }
                }

            }


            //so sánh 2 bảng UniqueCodes trong PO db và CZ run db, nếu có mã nào trong PO db mà không có trong CZ run db thì thêm vào CZ run db
            DataTable poUniqueCodes = Get_Unique_Codes_MES(orderNo);
            DataTable czRunUniqueCodes = Get_Unique_Codes_Run(orderNo);
            List<string> poCodes = poUniqueCodes.AsEnumerable().Select(row => row.Field<string>("Code")).ToList();
            List<string> czRunCodes = czRunUniqueCodes.AsEnumerable().Select(row => row.Field<string>("Code")).ToList();
            List<string> codesToAdd = poCodes.Except(czRunCodes).ToList();

            if (codesToAdd.Count > 0)
            {
                //tạo thư mục nếu chưa tồn tại
                string czRunPath = $"C:/.ABC/{orderNo}.db";
                using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    conn.Open();
                    foreach (string code in codesToAdd)
                    {
                        string insertQuery = "INSERT INTO UniqueCodes (Code) VALUES (@Code)";
                        var command = new SQLiteCommand(insertQuery, conn);
                        command.Parameters.AddWithValue("@Code", code);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        //lấy PO his từ theo orderNo
        public DataTable Get_PO_Run_History_Info_By_OrderNo(string orderNo)
        {
            using (var conn = new SQLiteConnection($"Data Source=Databases/PO.tlog;Version=3;"))
            {
                string query = "SELECT * FROM PO WHERE orderNO = @orderNo ORDER BY Timestamp DESC";
                var adapter = new SQLiteDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@orderNo", orderNo);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        //lấy PO dùng lần cuối
        public DataRow Get_Last_Used_PO()
        {
            using (var conn = new SQLiteConnection($"Data Source=Databases/PO.tlog;Version=3;"))
            {
                string query = "SELECT * FROM PO ORDER BY Timestamp DESC LIMIT 1";
                var command = new SQLiteCommand(query, conn);
                var adapter = new SQLiteDataAdapter(command);
                var table = new DataTable();
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    return table.Rows[0];
                }
                else
                {
                    return null;
                }
            }
        }

        //check PO log
        public void Check_PO_Log_File()
        {
            string poPath = "Databases/PO.tlog";
            if (!System.IO.File.Exists(poPath))
            {
                using (var conn = new SQLiteConnection($"Data Source=Databases/PO.tlog;Version=3;"))
                {
                    conn.Open();
                    string createTableQuery = @"
                        CREATE TABLE    ""PO"" (
	                                    ""ID""	INTEGER NOT NULL UNIQUE,
	                                    ""orderNO""	TEXT NOT NULL,
	                                    ""productionDate""	TEXT NOT NULL,
	                                    ""Action""	TEXT NOT NULL,
	                                    ""UserName""	TEXT NOT NULL,
	                                    ""Counter""	JSON NOT NULL,
	                                    ""Timestamp""	TEXT NOT NULL,
	                                    ""Timeunix""	INTEGER NOT NULL,
	                                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                                    );";
                    var command = new SQLiteCommand(createTableQuery, conn);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Check_Run_File(string orderNo)
        {
            //tạo thư mục nếu chưa tồn tại
            string czRunPath = $"C:/.ABC/{orderNo}.db";
            //kiểm xem folder C:/.ABC/MM-yy đã tồn tại chưa, nếu chưa thì tạo mới
            string folderPath = $"C:/.ABC";

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            //kiểm tra xem file db đã tồn tại chưa, nếu chưa thì tạo mới
            if (!System.IO.File.Exists(czRunPath))
            {
                using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    conn.Open();
                    string createTableQuery = @"CREATE TABLE ""UniqueCodes"" (
	                                        ""ID""	INTEGER NOT NULL UNIQUE,
	                                        ""Code""	TEXT NOT NULL UNIQUE,
	                                        ""Status""	INTEGER NOT NULL DEFAULT 0,
	                                        ""ActivateDate""	TEXT NOT NULL DEFAULT 0,
	                                        ""ProductionDate""	TEXT NOT NULL DEFAULT 0,
	                                        ""ActivateUser""	TEXT NOT NULL DEFAULT 0,
	                                        ""Send_Status""	TEXT NOT NULL DEFAULT pending,
	                                        ""Recive_Status""	TEXT NOT NULL DEFAULT waiting,
	                                        ""Send_Recive_Logs""	JSON,
	                                        ""Duplicate""	JSON,
	                                        PRIMARY KEY(""ID"" AUTOINCREMENT)
                                        );";
                    var command = new SQLiteCommand(createTableQuery, conn);
                    command.ExecuteNonQuery();
                }

                //insert từ bảng CZ nhận từ MES vào bảng CZ run này

                DataTable czCodes = Get_Unique_Codes_MES(orderNo);

                if (czCodes.Rows.Count > 0)
                {
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        foreach (DataRow row in czCodes.Rows)
                        {
                            string insertQuery = "INSERT INTO UniqueCodes (Code) VALUES (@Code)";
                            var command = new SQLiteCommand(insertQuery, conn);
                            command.Parameters.AddWithValue("@Code", row["Code"]);
                            command.ExecuteNonQuery();
                        }
                    }
                }

            }


            //Kiểm tra xem bảng ghi history tất cả các result của PO đã tồn tại hay chưa, nếu chưa thì tạo mới nếu chưa thì tạo mới ở C:/.ABC/MM-yy/Record_<orderNO>.db
            string recordPath = $"C:/.ABC/Record_{orderNo}.db";
            if (!System.IO.File.Exists(recordPath))
            {
                using (var conn = new SQLiteConnection($"Data Source={recordPath};Version=3;"))
                {
                    conn.Open();
                    string createTableQuery = @"CREATE TABLE ""Records"" (
	                                            ""ID""	INTEGER NOT NULL UNIQUE,
	                                            ""Code""	TEXT NOT NULL DEFAULT 'FAIL',
	                                            ""Status""	TEXT NOT NULL DEFAULT 0,
	                                            ""PLC_Status""	TEXT NOT NULL DEFAULT 'FAIL',
	                                            ""ActivateDate""	TEXT NOT NULL DEFAULT 0,
	                                            ""ActivateUser""	TEXT NOT NULL DEFAULT 0,
	                                            ""ProductionDate""	TEXT NOT NULL DEFAULT 0,
	                                            PRIMARY KEY(""ID"" AUTOINCREMENT)
                                            );";
                    var command = new SQLiteCommand(createTableQuery, conn);
                    command.ExecuteNonQuery();
                }
            }


            string recordPath_PLC = $"C:/.ABC/Record_{orderNo}_PLC.db";
            if (!System.IO.File.Exists(recordPath_PLC))
            {
                using (var conn = new SQLiteConnection($"Data Source={recordPath_PLC};Version=3;"))
                {
                    conn.Open();
                    string createTableQuery = @"
                        CREATE TABLE Records (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            codeID INTEGER NOT NULL UNIQUE,
                            Status INTEGER DEFAULT 0
                        );";
                    var command = new SQLiteCommand(createTableQuery, conn);
                    command.ExecuteNonQuery();
                }
            }


            string recordAWS = $"C:/.ABC/Record_{orderNo}_Send_AWS.db";
            if (!System.IO.File.Exists(recordAWS))
            {
                using (var conn = new SQLiteConnection($"Data Source={recordAWS};Version=3;"))
                {
                    conn.Open();
                    string createTableQuery = @"CREATE TABLE ""Records"" (
	                                            ""ID""	INTEGER NOT NULL UNIQUE,
	                                            ""message_id""	TEXT NOT NULL UNIQUE,
	                                            ""orderNo""	TEXT NOT NULL DEFAULT 0,
	                                            ""uniqueCode""	TEXT NOT NULL DEFAULT 'FAIL',
	                                            ""status""	TEXT NOT NULL DEFAULT 0,
	                                            ""activate_datetime""	TEXT NOT NULL DEFAULT 0,
                                                ""production_date""	TEXT NOT NULL DEFAULT 0,
                                                ""thing_name""	TEXT NOT NULL DEFAULT 0, 
                                                ""send_datetime""	TEXT NOT NULL DEFAULT 0,
	                                            PRIMARY KEY(""ID"" AUTOINCREMENT)
                                            );";
                    var command = new SQLiteCommand(createTableQuery, conn);
                    command.ExecuteNonQuery();
                }
            }

            string recive_AWS = $"C:/.ABC/Record_{orderNo}_Recive_AWS.db";
            if (!System.IO.File.Exists(recive_AWS))
            {
                using (var conn = new SQLiteConnection($"Data Source={recive_AWS};Version=3;"))
                {
                    conn.Open();
                    string createTableQuery = @"CREATE TABLE ""Records"" (
	                                            ""ID""	INTEGER NOT NULL UNIQUE,
	                                            ""message_id""	TEXT NOT NULL UNIQUE,
	                                            ""status""	TEXT NOT NULL DEFAULT 0,
	                                            ""error_message""	TEXT NOT NULL DEFAULT '0',
	                                            ""recive_datetime""	TEXT NOT NULL DEFAULT 0,
	                                            PRIMARY KEY(""ID"" AUTOINCREMENT)
                                            );";
                    var command = new SQLiteCommand(createTableQuery, conn);
                    command.ExecuteNonQuery();
                }
            }


        }

        public int Get_ID_RUN(string orderNO)
        {
            string czRunPath = $"C:/.ABC/Record_{orderNO}.db";
            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                string query = "SELECT * FROM Records ORDER BY ID DESC LIMIT 1";
                var command = new SQLiteCommand(query, conn);
                var adapter = new SQLiteDataAdapter(command);
                var table = new DataTable();
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    return int.Parse(table.Rows[0]["ID"].ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        //hàm inser vào bảng Records của CZ run db AWS
        public void Insert_Record_AWS(string orderNo, string message_id, string uniqueCode, string status, string activate_datetime, string production_date, string thing_name, string send_datetime)
        {
            string czRunPath = $"C:/.ABC/Record_{orderNo}.db";
            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                conn.Open();
                string insertQuery = @"
                    INSERT INTO Records (message_id, orderNo, uniqueCode, status, activate_datetime, production_date, thing_name, send_datetime)
                    VALUES (@message_id, @orderNo, @uniqueCode, @status, @activate_datetime, @production_date, @thing_name, @send_datetime)";
                var command = new SQLiteCommand(insertQuery, conn);
                command.Parameters.AddWithValue("@message_id", message_id);
                command.Parameters.AddWithValue("@orderNo", orderNo);
                command.Parameters.AddWithValue("@uniqueCode", uniqueCode);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@activate_datetime", activate_datetime);
                command.Parameters.AddWithValue("@production_date", production_date);
                command.Parameters.AddWithValue("@thing_name", thing_name);
                command.Parameters.AddWithValue("@send_datetime", send_datetime);
                command.ExecuteNonQuery();
            }
        }

        public void Delete_PO (string orderNo, string lydo)
        {
            using (var conn = new SQLiteConnection($"Data Source={poPath};Version=3;"))
            {
                conn.Open();
                string insertQuery = @"
                            INSERT INTO PO (orderNO, productionDate, Action, UserName, Counter, Timestamp, Timeunix)
                            VALUES (@orderNo, @productionDate, 'DELETE', @UserName, '{lydo:"+ lydo + "}', @Timestamp, @Timeunix)";
                var command = new SQLiteCommand(insertQuery, conn);
                command.Parameters.AddWithValue("@orderNo", orderNo);
                command.Parameters.AddWithValue("@productionDate", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
                command.Parameters.AddWithValue("@UserName", Globalvariable.CurrentUser.Username);
                command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
                command.Parameters.AddWithValue("@Timeunix", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
                command.ExecuteNonQuery();
            }
        }

        //hàm inser vào bảng Records của CZ run db Recive AWS
        public void Insert_Record_Recive_AWS(string orderNo, string message_id, string status, string error_message, string recive_datetime)
        {
            string czRunPath = $"C:/.ABC/Record_{orderNo}.db";
            using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
            {
                conn.Open();
                string insertQuery = @"
                    INSERT INTO Records (message_id, status, error_message, recive_datetime)
                    VALUES (@message_id, @status, @error_message, @recive_datetime)";
                var command = new SQLiteCommand(insertQuery, conn);
                command.Parameters.AddWithValue("@message_id", message_id);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@error_message", error_message);
                command.Parameters.AddWithValue("@recive_datetime", recive_datetime);
                command.ExecuteNonQuery();
            }
        }

        public Get get { get; } = new Get();

        public class Get
        {
            string poPath = "Databases/PO.tlog";
            //lấy số lượng Count có Send_Status = 'Sent'
            public int Get_Unique_Codes_Run_Send_Count(string orderNo)
            {
                //tạo thư mục nếu chưa tồn tại
                string czRunPath = $"C:/.ABC/{orderNo}.db";
                using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM \"main\".\"UniqueCodes\" WHERE \"Send_Status\" = 'Sent'  AND \"Status\" != '0' ";
                    var command = new SQLiteCommand(query, conn);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }

            public int Get_Unique_Codes_Run_Sent_Recive_OK_Count(string orderNo)
            {
                //tạo thư mục nếu chưa tồn tại
                string czRunPath = $"C:/.ABC/{orderNo}.db";

                using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM \"main\".\"UniqueCodes\" WHERE \"Send_Status\" = 'sent'  AND \"Status\" != '0' AND \"Recive_Status\" != 'waiting' ";
                    var command = new SQLiteCommand(query, conn);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }

            //lấy số lượng Count có Recive_Status != 'waiting'
            public int Get_Unique_Codes_Run_Recive_Count(string orderNo)
            {
                //tạo thư mục nếu chưa tồn tại
                string czRunPath = $"C:/.ABC/{orderNo}.db";
                using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM \"main\".\"UniqueCodes\" WHERE \"Recive_Status\" != 'waiting'  AND \"Status\" != '0' ";
                    var command = new SQLiteCommand(query, conn);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }

            public int Get_Record_Product_Count(string orderNo, e_Content_Result status)
            {

                try
                {
                    string czpath = "C:/.ABC" + "/Record_" + orderNo + ".db";
                    using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
                    {
                        string query = $"SELECT COUNT(*) FROM Records WHERE Status = '{status.ToString()}'";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        conn.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count;
                    }
                }
                catch
                {
                    return 0;
                }

            }

            public int Get_AWS_Sent_Count(string orderNo)
            {

                try
                {
                    string czpath = "C:/.ABC" + "/" + orderNo + ".db";
                    using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
                    {
                        string query = $"SELECT COUNT(*) FROM UniqueCodes WHERE Send_Status = 'Sent'";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        conn.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count;
                    }
                }
                catch
                {
                    return 0;
                }
            }

            //lấy orderNo các PO có Action = DELETE
            public List<string> Get_PO_Deleted_OrderNo()
            {
                List<string> orderNos = new List<string>();
                using (var conn = new SQLiteConnection($"Data Source={poPath};Version=3;"))
                {
                    string query = "SELECT DISTINCT orderNO FROM PO WHERE Action = 'DELETE'";
                    var command = new SQLiteCommand(query, conn);
                    conn.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderNos.Add(reader["orderNO"].ToString());
                        }
                    }
                }
                return orderNos;
            }

            //kiểm tra xem PO có DELETE hay không
            public bool Is_PO_Deleted(string orderNo)
            {
                using (var conn = new SQLiteConnection($"Data Source={poPath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM PO WHERE orderNO = @orderNo AND Action = 'DELETE'";
                    var command = new SQLiteCommand(query, conn);
                    command.Parameters.AddWithValue("@orderNo", orderNo);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }

            public DataTable Get_Records(string orderNo)
            {
                //tạo thư mục nếu chưa tồn tại
                string czRunPath = $"C:/.ABC/Record_{orderNo}.db";

                using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    string query = "SELECT * FROM Records";
                    var adapter = new SQLiteDataAdapter(query, conn);
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }


        }

    }

    public class PO_Infomation
    {
        public string orderNo { get; set; } = "-";
        public string site { get; set; } = "-";
        public string factory { get; set; } = "-";
        public string productionLine { get; set; } = "-";
        public string productionDate { get; set; } = "-";
        public string shift { get; set; } = "-";
        public string orderQty { get; set; } = "-";
        public string lotNumber { get; set; } = "-";
        public string productCode { get; set; } = "-";
        public string productName { get; set; } = "-";
        public string GTIN { get; set; } = "-";
        public string customerOrderNo { get; set; } = "-";
        public string uom { get; set; } = "-";
        public string CodeCount { get; set; } = "-";//tổng số lượng mã code đã nhận

        // ✅ Thêm property để dùng class con
        public Run_Infomation runInfo { get; set; } = new Run_Infomation();
        public AWS_Infomation awsInfo { get; set; } = new AWS_Infomation();

        public class Run_Infomation
        {
            public int pass { get; set; } = 0;
            public int fail { get; set; } = 0;
            public int duplicate { get; set; } = 0;
            public int total { get; set; } = 0;
        }

        public class AWS_Infomation
        {
            public int sent { get; set; } = 0;//đã gửi
            public int waiting { get; set; } = 0; //đang chờ phản hồi
            public int error { get; set; } = 0; //lỗi phản hồi
            public int pending { get; set; } = 0; //đang chờ gửi
            public int recive { get; set; } = 0; //đã nhận phản hồi
        }

    }

}
