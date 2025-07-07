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

        public POService(string dbPath_PO_MES = @"C:\Users\THUC\source\repos\ErfanMNM\MASANSolution\Server_Service\po.db", string codes_Path_CZ_DB_MES = @"C:\Users\THUC\source\repos\ErfanMNM\MASANSolution\Server_Service\codes")
        {
            _connectionString_PO_MES = $"Data Source={dbPath_PO_MES};Version=3;";
            _codes_Path_CZ_DB_MES = codes_Path_CZ_DB_MES;
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
            string poPath = "Databases/PO.tlog";
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
                    string createTableQuery = @"
                        CREATE TABLE    ""UniqueCodes"" (
	                                    ""ID""	INTEGER NOT NULL UNIQUE,
	                                    ""Code""	TEXT NOT NULL UNIQUE,
	                                    ""Status""	INTEGER NOT NULL DEFAULT 0,
	                                    ""ActivateDate""	TEXT NOT NULL DEFAULT 0,
	                                    ""ActivateUser""	TEXT NOT NULL DEFAULT 0,
                                        ""Send_Status""	    TEXT NOT NULL DEFAULT ""pending"",
                                        ""Recive_Status""	TEXT NOT NULL DEFAULT ""waiting"",
                                        ""Send_Recive_Logs"" JSON,
	                                    ""Timestamp""	TEXT NOT NULL DEFAULT 0,
	                                    ""Timeunix""	INTEGER NOT NULL DEFAULT 0,
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
                    string createTableQuery = @"
                        CREATE TABLE Records (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Code TEXT NOT NULL UNIQUE,
                            Status INTEGER DEFAULT 0,
                            ActivateDate TEXT DEFAULT 0,
                            ActivateUser TEXT DEFAULT 0,
                            Timestamp TEXT NOT NULL,
                            Timeunix INTEGER NOT NULL
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


        }

    }

   
}
