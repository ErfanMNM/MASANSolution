using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Production
{
    public class ProductionOrder
    {
        //vị trí thư mục lưu trữ thông tin po lấy từ MES
        private static string poAPIServerPath;
        private static string poAPIServerFileName;
        //đường dẫn đến cơ sở dữ liệu PO log
        private static string POLog_dbPath = @"C:\MasanSerialization\Databases\POLog.db"; //đường dẫn đến cơ sở dữ liệu log PO, nơi lưu trữ thông tin PO đã sử dụng trong quá trình sản xuất
        //khởi tạo thông tin đường dẫn đến cơ sở dữ liệu PO
        private static string dataPath = $"C:/MasanSerialization/PODatabases";
        public static string orderNO { get; set; } = string.Empty;

        public ProductionOrder()
        {
            //tao thư mục lưu trữ thông tin po lấy từ MES nằm trong thư mục cài đặt của ứng dụng ở C:\MasanSerialization\Server_Service\
            poAPIServerPath = @"C:\MasanSerialization\Server_Service\";
            poAPIServerFileName = "po.db";
            Create_POLogDatabases();
        }

        #region Các thuộc tính thông tin PO
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
        public string gtin { get; set; } = "-";
        public string customerOrderNo { get; set; } = "-";
        public string uom { get; set; } = "-";
        public string cartonSize { get; set; } = "-";
        public string totalCZCode { get; set; } = "-"; //Tổng số mã CZ đã nhận từ MES
        public Product_Counter counter { get; set; } = new Product_Counter();

        public AWS_Send_Counter awsSendCounter { get; set; } = new AWS_Send_Counter(); //Thông tin bộ đếm gửi AWS
        public AWS_Recived_Counter awsRecivedCounter { get; set; } = new AWS_Recived_Counter(); //Thông tin bộ đếm nhận AWS
        #endregion

        #region Các thống kê
        public class Product_Counter
        {
            public int passCount { get; set; } = 0;//Pass
            public int failCount { get; set; } = 0;//Lỗi
            public int duplicateCount { get; set; } = 0;//Trùng lặp
            public int readfailCount { get; set; } = 0;//Không đọc được
            public int notfoundCount { get; set; } = 0;//Không tồn tại
            public int errorCount { get; set; } = 0;//Tổng số lỗi khác (gửi PLC không được, timeout, v.v.)

            public int totalCount { get; set; } = 0;//Tổng số mã đã quét
            public int totalCartonCount { get; set; } = 0;//Tổng số thùng đã quét
            public int activatedCartonCount { get; set; } = 0; //Tổng số thùng đã kích hoạt
            public int errorCartonCount { get; set; } = 0; //Tổng số thùng lỗi (lỗi kích hoạt, lỗi gửi AWS, v.v.)

            //hàm reset lại các bộ đếm
            public void Reset()
            {
                passCount = 0;
                failCount = 0;
                duplicateCount = 0;
                readfailCount = 0;
                notfoundCount = 0;
                errorCount = 0;
                totalCount = 0;
                totalCartonCount = 0;
                activatedCartonCount = 0;
                errorCartonCount = 0;
            }

        }

        //thông tin bộ đếm gửi nhận aws
        public class AWS_Send_Counter
        {
            //số chưa gửi (chưa kích hoạt)
            public int pendingCount { get; set; } = 0; //Số mã chưa gửi
            //số đã gửi
            public int sentCount { get; set; } = 0; //Số mã đã gửi thành công
            //số gửi lỗi
            public int failedCount { get; set; } = 0; //Số mã gửi lỗi

            //hàm reset lại các bộ đếm
            public void Reset()
            {
                pendingCount = 0;
                sentCount = 0;
                failedCount = 0;
            }
        }

        public class AWS_Recived_Counter
        {
            //số chưa gửi (chưa kích hoạt)
            public int waitingCount { get; set; } = 0; //số mã đang chờ nhận
            //số đã gửi
            public int recivedCount { get; set; } = 0; //đã nhận thành công

            //ham reset lại các bộ đếm
            public void Reset()
            {
                waitingCount = 0;
                recivedCount = 0;
            }
        }

        #endregion

        #region Các phương thức lấy dữ liệu từ MES
        public GetfromMES getfromMES { get; } = new GetfromMES();

        public class GetfromMES
        {
            public (bool issucess, string message, DataTable PO) ProductionOrder_List()
            {
                string dbPath = $@"{poAPIServerPath}{poAPIServerFileName}";
                try
                {
                    if (!File.Exists(dbPath))
                    {
                        return (false, "Cơ sở dữ liệu PO không tồn tại.", null);
                    }

                    using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM po_records ORDER BY orderNo";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, "Lấy dữ liệu thành công.", table) : (false, "Không có dữ liệu PO nào.", null);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi P01 khi kiểm tra cơ sở dữ liệu PO: {ex.Message}", null);
                }

            }
            //lấy thông tin chi tiết của một PO theo orderNo
            public (bool issucess, string message, DataTable PO) ProductionOrder_Detail(string orderNo)
            {
                string dbPath = $@"{poAPIServerPath}{poAPIServerFileName}";
                try
                {
                    if (!File.Exists(dbPath))
                    {
                        return (false, "Cơ sở dữ liệu PO không tồn tại.", null);
                    }
                    using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM POInfo WHERE orderNo = @orderNo";
                        var cmd = new SQLiteCommand(query, conn);
                        cmd.Parameters.AddWithValue("@orderNo", orderNo);
                        var adapter = new SQLiteDataAdapter(cmd);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, "Lấy dữ liệu thành công.", table) : (false, "Không tìm thấy PO với orderNo: " + orderNo, null);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi P02 khi lấy thông tin chi tiết PO: {ex.Message}", null);
                }
            }
            //lấy số count mã CZ nằm trong thư mục _codesPath/<orderNo>.db SELECT COUNT(*) FROM `UniqueCodes`;
            public (bool issucess, string message, int CzCodeCount) Get_Unique_Code_MES_Count(string orderNo)
            {
                try
                {
                    string czCodesPath = $@"{poAPIServerPath}/codes/{orderNo}.db";
                    using (var conn = new SQLiteConnection($"Data Source={czCodesPath};Version=3;"))
                    {
                        string query = "SELECT COUNT(*) FROM UniqueCodes";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        conn.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (true, "Lấy số lượng mã CZ thành công.", count);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi P03 khi lấy số lượng mã CZ: {ex.Message}", 0);
                }
            }

            //lấy danh sách mã CZ từ cơ sở dữ liệu SQLite
            public (bool issucess, string message, DataTable Data) Get_Unique_Codes_MES(string orderNo)
            {
                try
                {
                    string czpath = $@"{poAPIServerPath}/codes/{orderNo}.db";
                    if (!File.Exists(czpath))
                    {
                        return (false, "Cơ sở dữ liệu mã CZ không tồn tại.", null);
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
                    {
                        string query = "SELECT * FROM UniqueCodes";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, "Lấy danh sách mã CZ thành công.", table) : (false, "Không có mã CZ nào trong cơ sở dữ liệu.", null);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi P04 khi lấy danh sách mã CZ: {ex.Message}", null);
                }

            }

            //issucess: true nếu thành công, false nếu thất bại
            //message: thông báo kết quả
            public (bool issucess, string message) MES_Load_OrderNo_ToComboBox(UIComboBox comboBox)
            {
                try
                {
                    string connectionString = $@"Data Source={poAPIServerPath}{poAPIServerFileName};Version=3;";
                    using (var conn = new SQLiteConnection(connectionString))
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

                    return (true, "Tải danh sách orderNo thành công.");
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi GC01 khi tải danh sách orderNo: {ex.Message}");
                }
            }
        }

        #endregion

        #region Các phương thức lấy từ dữ liệu sản xuất (run)
        public GetDataPO getDataPO { get; } = new GetDataPO();
        public class GetDataPO
        {
            //lấy PO dùng lần cuối từ cơ sở dữ liệu SQLite 
            public (bool issucess, DataRow lastPO, string message) GetLastPO()
            {
                try
                {

                    using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM PO ORDER BY Timestamp DESC LIMIT 1";
                        var command = new SQLiteCommand(query, conn);
                        var adapter = new SQLiteDataAdapter(command);
                        var table = new DataTable();
                        adapter.Fill(table);
                        if (table.Rows.Count > 0)
                        {
                            return (true, table.Rows[0], "Lấy PO thành công.");
                        }
                        else
                        {
                            return (false, null, "Không có dữ liệu PO nào.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return (false, null, $"Lỗi P04 khi lấy PO: {ex.Message}");
                }
            }

            public (bool issucess, DataTable logPO, string message) Get_PO_Run_History_By_OrderNo(string orderNo)
            {
                try
                {
                    using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
                    {
                        string query = "SELECT * FROM PO WHERE orderNO = @orderNo ORDER BY Timestamp DESC";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@orderNo", orderNo);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, table, "Lấy lịch sử PO thành công.") : (false, null, "Không có lịch sử PO nào cho orderNo: " + orderNo);
                    }
                }
                catch (Exception ex)
                {
                    return (false, null, $"Lỗi P05 khi lấy lịch sử PO: {ex.Message}");
                }
            }

            //lấy tổng số sản phẩm đã chạy
            public (bool issucess, int RecordCount, string message) Get_Record_Count(string orderNO)
            {
                try
                {
                    string czRunPath = $"{dataPath}/Record_{orderNO}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, 0, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM Records";
                        var command = new SQLiteCommand(query, conn);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (true, count, "Lấy số lượng bản ghi thành công.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, 0, $"Lỗi PH05 khi lấy số lượng bản ghi: {ex.Message}");
                }

            }

            //lấy count theo trạng thái
            public (bool issucess, int Count, string message) Get_Record_Count_By_Status(string orderNO, e_Production_Status Production_Status)
            {
                try
                {
                    string czRunPath = $"{dataPath}/Record_{orderNO}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, 0, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM Records WHERE Status = @Status";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@Status", Production_Status.ToString());
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (true, count, $"Lấy số lượng bản ghi với trạng thái {Production_Status.ToString()} thành công.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, 0, $"Lỗi PH06 khi lấy số lượng bản ghi theo trạng thái: {ex.Message}");
                }
            }

            //lấy số gửi AWS đã nhận recive
            public (bool issucess, int Count, string message) Get_Record_Sent_Recive_Count(string orderNo, e_AWS_Send_Status Send, e_AWS_Recive_Status Recive, string Recive_Conditional = "=", string Conditional = "")
            {
                try
                {   //tạo thư mục nếu chưa tồn tại
                    string czRunPath = $"{dataPath}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, 0, "Cơ sở dữ liệu gửi AWS không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        string query = $"SELECT COUNT(*) FROM UniqueCodes WHERE Send_Status =@send AND Recive_Status {Recive_Conditional} @recive {Conditional}; ";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@send", Send.ToString());
                        command.Parameters.AddWithValue("@recive", Recive.ToString());
                        conn.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (true, count, "Lấy số lượng bản ghi AWS thành công.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, 0, $"Lỗi PH07 khi lấy số lượng bản ghi gửi AWS: {ex.Message}");
                }

            }

            public (bool issucess, DataTable Records, string message) Get_Records(string orderNo)
            {
                try
                {
                    string czRunPath = $"{dataPath}/Record_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, null, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Records";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, table, "Lấy danh sách bản ghi thành công.") : (true, null, "Không có bản ghi nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, null, $"Lỗi PH08 khi lấy danh sách bản ghi: {ex.Message}");
                }
            }


            public (bool issucess, DataTable Codes, string message) Get_Codes(string orderNo)
            {
                try
                {
                    string czRunPath = $"{dataPath}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, null, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM UniqueCodes";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, table, "Lấy danh sách bản ghi thành công.") : (true, null, "Không có bản ghi nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, null, $"Lỗi PH110 khi lấy danh sách bản ghi: {ex.Message}");
                }
            }
            public bool Is_PO_Deleted(string orderNo)
            {
                using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM PO WHERE orderNO = @orderNo AND Action = 'Deleted'";
                    var command = new SQLiteCommand(query, conn);
                    command.Parameters.AddWithValue("@orderNo", orderNo);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }

            public bool Is_PO_Completed(string orderNo)
            {
                using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM PO WHERE orderNO = @orderNo AND Action = 'Completed'";
                    var command = new SQLiteCommand(query, conn);
                    command.Parameters.AddWithValue("@orderNo", orderNo);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }

        }

        #endregion

        #region Các phương thức ghi dữ liệu vào cơ sở dữ liệu

        public (bool issucces, string message) Save_PO(string orderNo, string _productionDate, string userName)
        {
            try
            {
                var POlog = getDataPO.Get_PO_Run_History_By_OrderNo(orderNo);

                //nếu chưa từng tồn tại thì tạo mới
                if (!POlog.issucess)
                {
                    using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
                    {
                        conn.Open();
                        string insertQuery = @"
                        INSERT INTO PO (orderNO, productionDate, Action, UserName, Counter, Timestamp, Timeunix)
                        VALUES (@orderNo, @productionDate, 'Create', @UserName, '{}', @Timestamp, @Timeunix)";
                        var command = new SQLiteCommand(insertQuery, conn);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        command.Parameters.AddWithValue("@productionDate", _productionDate);
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow.ToString("o"));
                        command.Parameters.AddWithValue("@Timeunix", ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds());
                        command.ExecuteNonQuery();
                    }
                }
                //nếu tồn tại rồi thì ghi Update
                else
                {
                    using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
                    {
                        conn.Open();
                        string insertQuery = @"
                        INSERT INTO PO (orderNO, productionDate, Action, UserName, Counter, Timestamp, Timeunix)
                        VALUES (@orderNo, @productionDate, 'Update', @UserName, '{}', @Timestamp, @Timeunix)";
                        var command = new SQLiteCommand(insertQuery, conn);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        command.Parameters.AddWithValue("@productionDate", _productionDate);
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow.ToString("o"));
                        command.Parameters.AddWithValue("@Timeunix", ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds());
                        command.ExecuteNonQuery();
                    }
                }

                //cập nhật lại mã CZ cho chắc
                var getUniqueCodesMes = getfromMES.Get_Unique_Codes_MES(orderNo);
                if (!getUniqueCodesMes.issucess)
                {
                    return (false, getUniqueCodesMes.message);
                }
                DataTable poUniqueCodes = getUniqueCodesMes.Data;

                var getUniqueCodesRun = getDataPO.Get_Codes(orderNo);
                if (!getUniqueCodesRun.issucess)
                {
                    return (false, getUniqueCodesRun.message);
                }
                DataTable czRunUniqueCodes = getUniqueCodesRun.Codes;

                List<string> poCodes = poUniqueCodes.AsEnumerable().Select(row => row.Field<string>("Code")).ToList();
                List<string> czRunCodes = czRunUniqueCodes.AsEnumerable().Select(row => row.Field<string>("Code")).ToList();
                List<string> codesToAdd = poCodes.Except(czRunCodes).ToList();

                if (codesToAdd.Count > 0)
                {
                    //tạo thư mục nếu chưa tồn tại
                    string czRunPath = $"{poAPIServerPath}/codes/{orderNo}.db";
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

                return (true, "Ghi PO thành công vào cơ sở dữ liệu.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi P06 khi ghi PO vào cơ sở dữ liệu: {ex.Message}");
            }
        }

        #endregion

        #region Các chức năng khác dùng nội bộ
        private static void Create_POLogDatabases()
        {
            if (!Directory.Exists(@"C:\MasanSerialization\Databases"))
            {
                Directory.CreateDirectory(@"C:\MasanSerialization\Databases");
            }

            if (!File.Exists(POLog_dbPath))
            {
                using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
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

        public enum ActionType
        {
            Create,
            Update,
            Delete,
            UpdateProductionDate
        }

        #endregion

        #region Các phương thức kiểm tra file
        public (bool issucess, string message) Check_Database_File(string orderNo)
        {
            try
            {//tạo thư mục nếu chưa tồn tại
                string czRunPath = $"{dataPath}/{orderNo}.db";
                //kiểm tra xem thư mục đã tồn tại chưa, nếu chưa thì tạo mới
                string directoryPath = Path.GetDirectoryName(czRunPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                //kiểm tra xem file db đã tồn tại chưa, nếu chưa thì tạo mới
                if (!File.Exists(czRunPath))
                {
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string createTableQuery = @"CREATE TABLE ""UniqueCodes"" (
	                                        ""ID""	INTEGER NOT NULL UNIQUE,
	                                        ""Code""	TEXT NOT NULL UNIQUE,
                                            ""cartonCode""	TEXT NOT NULL DEFAULT 0,
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


                    var getczCodes = getfromMES.Get_Unique_Codes_MES(orderNo);
                    if (!getczCodes.issucess)
                    {
                        return (false, getczCodes.message);
                    }
                    DataTable czCodes = getczCodes.Data;

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
                string recordPath = $"{dataPath}/Record_{orderNo}.db";
                if (!File.Exists(recordPath))
                {
                    using (var conn = new SQLiteConnection($"Data Source={recordPath};Version=3;"))
                    {
                        conn.Open();
                        string createTableQuery = @"CREATE TABLE ""Records"" (
	                                            ""ID""	INTEGER NOT NULL UNIQUE,
	                                            ""Code""	TEXT NOT NULL DEFAULT 'FAIL',
                                                ""cartonCode""	TEXT NOT NULL DEFAULT 0,
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

                //kiểm tra xem bảng ghi history tất cả các result của PO đã tồn tại hay chưa, nếu chưa thì tạo mới nếu chưa thì tạo mới
                string recordPath_PLC = $"{dataPath}/PLC_Record_{orderNo}.db";
                if (!File.Exists(recordPath_PLC))
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

                //kiểm tra xem bảng ghi history tất cả các result của PO đã tồn tại hay chưa, nếu chưa thì tạo mới nếu chưa thì tạo mới
                string recordAWS = $"{dataPath}/Send_AWS_Record_{orderNo}.db";
                if (!File.Exists(recordAWS))
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

                //kiểm tra xem bảng ghi history tất cả các result của PO đã tồn tại hay chưa, nếu chưa thì tạo mới nếu chưa thì tạo mới
                string recive_AWS = $"{dataPath}/Recive_AWS_Record_{orderNo}.db";
                if (!File.Exists(recive_AWS))
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
                return (true, "Kiểm tra và tạo cơ sở dữ liệu thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi GC02 khi kiểm tra file cơ sở dữ liệu: {ex.Message}");
            }




        }
        #endregion
    }

    #region Các enum dùng chung

    public enum e_Production_State
    {
        NoSelectedPO,// không có PO nào được chọn
        Start,// bắt đầu quá trình sản xuất
        Checking_PO_Info, //kiểm tra thông tin PO đã đầy đủ chưa
        Loading,//đang tải thông tin PO từ MES
        Camera_Processing,//đang xử lý camera
        Pushing_new_PO_to_PLC,//đẩy số liệu mới xuống PLC để bắt đầu sản xuất
        Pushing_continue_PO_to_PLC,//đẩy số liệu xuống PLC để tiếp tục sản xuất
        Ready,//trạng thái sẵn sàng để bắt đầu sản xuất
        Running,//trạng thái đang sản xuất
        Completed,// quá trình sản xuất đã hoàn thành
        Editing,// đang chỉnh sửa thông tin sản xuất
        Editting_ProductionDate,//Đang chỉnh sửa ngày sản xuất
        Saving,// đang lưu thông tin sản xuất
        Error// có lỗi xảy ra trong quá trình sản xuất
    }

    public enum e_Production_Status
    {
        Pass = 1, // thành công
        Fail = -1, // thất bại
        Duplicate, // trùng lặp
        ReadFail, // không đọc được
        NotFound, // không tìm thấy
        Error // lỗi khác
    }

    public enum e_AWS_Send_Status
    {
        Pending, // chưa gửi
        Sent, // đã gửi thành công
        Failed // gửi lỗi
    }

    public enum e_AWS_Recive_Status
    {
        Waiting = 0, // đang chờ nhận
        Recived = 200, // đã nhận thành công
        Error = 2, // lỗi khi nhận
        Error_404 = 404,// không tìm thấy
        Error_500 = 500, // lỗi máy chủ
        Error_400 = 400, // lỗi yêu cầu không hợp lệ
        Error_401 = 401, // lỗi xác thực
        Error_403 = 403, // lỗi quyền truy cập
        Error_408 = 408, // lỗi timeout
        Error_409 = 409, // lỗi xung đột
        Error_402 = 402 // yêu cầu thanh toán
    }

    public enum e_Production_Order_Log_Type
    {
        Deleted, // PO đã bị xóa
        Completed, // PO đã hoàn thành
        Create, // PO đang chạy
        Update, // PO đang chờ xử lý
        UpdateProductionDate, // PO đang cập nhật ngày sản xuất
        Error // có lỗi xảy ra với PO
    }

    #endregion
}
