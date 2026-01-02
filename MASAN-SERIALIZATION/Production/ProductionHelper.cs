using MASAN_SERIALIZATION.Configs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace MASAN_SERIALIZATION.Production
{
    /// <summary>
    /// ProductionOrder Class - Quản lý Production Order và Codes
    ///
    /// CƠ CHẾ LƯU TRỮ MÃ THEO GTIN (Cập nhật mới):
    /// - API lưu uniqueCode theo GTIN thay vì theo orderNo
    /// - Nhiều PO có cùng GTIN sẽ dùng chung 1 bộ mã
    /// - Mỗi PO vẫn có database riêng để tracking (records, cartons...)
    /// - Cấu trúc thư mục: yyyy-MM/GTIN/
    ///   + Ví dụ: 2025-01/8931234567890/
    ///     - PO001.db (database của PO001, chứa codes từ GTIN)
    ///     - PO002.db (database của PO002, cũng chứa codes từ GTIN)
    ///     - Record_PO001.db, carton_PO001.db, etc.
    ///
    /// LƯU Ý QUAN TRỌNG:
    /// - Get_Unique_Codes_MES() và Get_Unique_Code_MES_Count() lấy codes theo GTIN
    /// - Khi tạo database cho PO mới, codes sẽ được load từ file GTIN.json
    /// - Khi gửi lên API vẫn theo PO, nhưng API sẽ lưu theo GTIN
    /// </summary>
    public class ProductionOrder
    {
        #region Private Fields & Constants
        private static string poAPIServerPath;
        private static string poAPIServerFileName;
        private static string POLog_dbPath = @"C:\MasanSerialization_v2\Databases\POLog.db";
        public static string dataPath = $"C:/MasanSerialization_v2/PODatabases";

        private static string poMesJsonCodesPath = @"C:\MasanSerialization_v2\Server_Service\codes_json";
        private static string poMesJsonPODataPath = @"C:/MasanSerialization_v2/Server_Service/data";
        public static string orderNO { get; set; } = string.Empty;
        #endregion

        #region Constructor
        public ProductionOrder()
        {
            poAPIServerPath = @"C:\MasanSerialization_v2\Server_Service\";
            poAPIServerFileName = "po1.db";
            Create_POLogDatabases();
        }
        #endregion

        #region Production Order Properties
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
        public string totalCZCode { get; set; } = "-";
        public Product_Counter counter { get; set; } = new Product_Counter();
        public AWS_Send_Counter awsSendCounter { get; set; } = new AWS_Send_Counter();
        public AWS_Recived_Counter awsRecivedCounter { get; set; } = new AWS_Recived_Counter();
        public string dbPath
        {
            get
            {
                // Nếu có orderNo thì dùng cấu trúc mới theo tháng/GTIN
                if (!string.IsNullOrEmpty(orderNo) && orderNo != "-")
                {
                    return GetOrderBasePath(orderNo);
                }
                // Fallback về đường dẫn cũ nếu chưa có orderNo
                return dataPath;
            }
        }
        #endregion

        #region Data Access Objects
        public GetfromMES getfromMES { get; } = new GetfromMES();
        public GetDataPO getDataPO { get; } = new GetDataPO();
        public PostDB setDB { get; set; } = new PostDB();
        #endregion

        #region Counter Classes
        public class Product_Counter
        {
            public int passCount { get; set; } = 0;
            public int failCount { get; set; } = 0;
            public int duplicateCount { get; set; } = 0;
            public int readfailCount { get; set; } = 0;
            public int notfoundCount { get; set; } = 0;
            public int errorCount { get; set; } = 0;
            public int totalCount { get; set; } = 0;
            public int totalCartonCount { get; set; } = 0;
            public int activatedCartonCount { get; set; } = 0;
            public int errorCartonCount { get; set; } = 0;
            public int cartonID { get; set; } = 0;
            public string carton_Packing_Code { get; set; } = "";
            public int carton_Packing_ID { get; set; } = 0;
            public int carton_Packing_Count { get; set; } = 0;

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
                cartonID = 0;
                carton_Packing_Code = "";
                carton_Packing_ID = 0;
                carton_Packing_Count = 0;
            }
        }

        public class AWS_Send_Counter
        {
            public int pendingCount { get; set; } = 0;
            public int sentCount { get; set; } = 0;
            public int failedCount { get; set; } = 0;

            public void Reset()
            {
                pendingCount = 0;
                sentCount = 0;
                failedCount = 0;
            }
        }

        public class AWS_Recived_Counter
        {
            public int waitingCount { get; set; } = 0;
            public int recivedCount { get; set; } = 0;

            public void Reset()
            {
                waitingCount = 0;
                recivedCount = 0;
            }
        }
        #endregion

        #region MES Data Access
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
                        return (table.Rows.Count > 0) ? 
                            (true, "Lấy dữ liệu thành công.", table) : 
                            (false, "Không có dữ liệu PO nào.", null);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi P01 khi kiểm tra cơ sở dữ liệu PO: {ex.Message}", null);
                }
            }
            public (bool issucess, string message) MES_Load_OrderNo_ToComboBox_V1(UIComboBox comboBox)
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

                        DataRow emptyRow = table.NewRow();
                        emptyRow["orderNo"] = "Chọn orderNO";
                        table.Rows.InsertAt(emptyRow, 0);

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
            public TResult ProductionOrder_Detail_V1(string orderNo)
            {
                string dbPath = $@"{poAPIServerPath}{poAPIServerFileName}";
                try
                {
                    if (!File.Exists(dbPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu PO không tồn tại.");
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

                        return (table.Rows.Count > 0)
                            ? new TResult(true, "Lấy thông tin chi tiết PO thành công.", 0, table)
                            : new TResult(false, "Không có thông tin chi tiết cho orderNo: " + orderNo);
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P02 khi lấy thông tin chi tiết PO: {ex.Message}");
                }
            }
            public TResult Get_Unique_Code_MES_Count_V1(string orderNo)
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
                        return (count > 0)
                            ? new TResult(true, "Lấy số lượng mã CZ thành công.", count)
                            : new TResult(false, "Không có mã CZ nào trong cơ sở dữ liệu.", 0);
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P03 khi lấy số lượng mã CZ: {ex.Message}");
                }
            }
            public TResult Get_Unique_Codes_MES_V1(string orderNo)
            {
                try
                {
                    string czpath = $@"{poAPIServerPath}/codes/{orderNo}.db";
                    if (!File.Exists(czpath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu mã CZ không tồn tại.");
                    }

                    using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
                    {
                        string query = "SELECT * FROM UniqueCodes";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ?
                           new TResult(true, "Lấy danh sách mã CZ thành công.", table.Rows.Count, table) :
                           new TResult(false, "Không có mã CZ nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P04 khi lấy danh sách mã CZ: {ex.Message}");
                }
            }

            public TResult ProductionOrder_Detail(string orderNo)
            {
                string filePath = Path.Combine(poMesJsonPODataPath, orderNo + ".json");

                try
                {
                    if (!File.Exists(filePath))
                    {
                        return new TResult(false, $"PH019 Không tìm thấy file dữ liệu PO: {orderNo}.json");
                    }

                    // Đọc nội dung file
                    string fileContent = File.ReadAllText(filePath).Trim();

                    // Parse JSON
                    var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContent);

                    if (jsonObj == null || jsonObj.Count == 0)
                    {
                        return new TResult(false, $"File dữ liệu PO rỗng hoặc không hợp lệ: {orderNo}.json");
                    }

                    // Tạo DataTable ngang: mỗi key là một cột
                    var table = new DataTable();

                    // Tạo các cột
                    foreach (var kv in jsonObj)
                    {
                        table.Columns.Add(kv.Key);
                    }

                    // Tạo một dòng dữ liệu
                    var row = table.NewRow();
                    foreach (var kv in jsonObj)
                    {
                        row[kv.Key] = kv.Value ?? DBNull.Value;
                    }
                    table.Rows.Add(row);

                    return new TResult(true, "Lấy thông tin chi tiết PO thành công.", 0, table);
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P02 khi lấy thông tin chi tiết PO: {ex.Message}");
                }
            }

            public TResult Get_Unique_Code_MES_Count(string orderNo)
            {
                try
                {
                    // Lấy GTIN từ orderNo
                    string gtin = GetGTIN(orderNo);
                    if (string.IsNullOrEmpty(gtin))
                    {
                        return new TResult(false, $"Không tìm thấy GTIN cho orderNo: {orderNo}", 0);
                    }

                    // Đường dẫn file codes theo GTIN (API mới lưu theo GTIN)
                    string jsonPath = poMesJsonCodesPath + "/GTIN_" + gtin + ".json";

                    if (!File.Exists(jsonPath))
                    {
                        return new TResult(false, $"Không tìm thấy file codes: {gtin}.json", 0);
                    }

                    string jsonText = File.ReadAllText(jsonPath);
                    var root = JObject.Parse(jsonText);

                    var blocks = root["blocks"] as JObject;
                    if (blocks == null || !blocks.HasValues)
                    {
                        return new TResult(false, "Không có khối (blocks) nào trong file.", 0);
                    }

                    // Đếm tổng số codes trong tất cả các block
                    int count = blocks
                        .Properties()
                        .Select(p => p.Value["codes"] as JArray)
                        .Where(arr => arr != null)
                        .Sum(arr => arr.Count);

                    return (count > 0)
                        ? new TResult(true, $"Lấy số lượng mã CZ thành công cho GTIN {gtin}.", count)
                        : new TResult(false, "Không có mã CZ nào trong file codes.", 0);
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P03 khi lấy số lượng mã CZ: {ex.Message}");
                }
            }
            public TResult Get_Unique_Codes_MES(string orderNo)
            {
                try
                {
                    // Lấy GTIN từ orderNo
                    string gtin = GetGTIN(orderNo);
                    if (string.IsNullOrEmpty(gtin))
                    {
                        return new TResult(false, $"Không tìm thấy GTIN cho orderNo: {orderNo}");
                    }

                    // Đường dẫn file codes theo GTIN
                    string jsonPath = Path.Combine(poMesJsonCodesPath, "GTIN_"+gtin + ".json");
                    if (!File.Exists(jsonPath))
                    {
                        return new TResult(false, $"File mã CZ không tồn tại cho GTIN: {gtin}");
                    }

                    string jsonText = File.ReadAllText(jsonPath);
                    var root = JObject.Parse(jsonText);

                    var blocks = root["blocks"] as JObject;
                    if (blocks == null || !blocks.HasValues)
                    {
                        return new TResult(false, "Không có mã CZ nào trong file codes.");
                    }

                    // Tạo DataTable kết quả (tên cột theo dữ liệu JSON)
                    var table = new DataTable();
                    table.Columns.Add("code", typeof(string));
                    table.Columns.Add("createdAt", typeof(string)); // để nguyên dạng ISO string cho an toàn
                    table.Columns.Add("blockNo", typeof(int));
                    table.Columns.Add("blockKey", typeof(string));  // key của block trong JSON, ví dụ "0","1",...

                    int total = 0;

                    foreach (var prop in blocks.Properties())
                    {
                        string blockKey = prop.Name; // "0","1",...
                        var codesArr = prop.Value["codes"] as JArray;
                        if (codesArr == null) continue;

                        foreach (var item in codesArr)
                        {
                            string code = (string)(item["code"] ?? string.Empty);
                            string createdAt = (string)(item["createdAt"] ?? string.Empty);

                            int blockNo = 0;
                            // ưu tiên lấy blockNo trong từng item; nếu thiếu thì thử parse từ blockKey
                            JToken bnToken = item["blockNo"];
                            if (bnToken != null && bnToken.Type != JTokenType.Null)
                            {
                                int.TryParse(bnToken.ToString(), out blockNo);
                            }
                            else
                            {
                                int.TryParse(blockKey, out blockNo);
                            }

                            table.Rows.Add(code, createdAt, blockNo, blockKey);
                            total++;
                        }
                    }

                    return (total > 0)
                        ? new TResult(true, $"Lấy danh sách mã CZ thành công cho GTIN {gtin}.", total, table)
                        : new TResult(false, "Không có mã CZ nào trong file codes.");
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P04 khi lấy danh sách mã CZ: {ex.Message}");
                }
            }
            public (bool issucess, string message) MES_Load_OrderNo_ToComboBox(UIComboBox comboBox)
            {
                try
                {
                    //lấy danh sách file trong thư mục poMesJsonPOPath
                    if (!Directory.Exists(poMesJsonPODataPath))
                    {
                        return (false, "Thư mục chứa file JSON không tồn tại.");
                    }
                    var files = Directory.GetFiles(poMesJsonPODataPath, "*.json");
                    if (files.Length == 0)
                    {
                        return (false, "Không có file JSON nào trong thư mục.");
                    }
                    DataTable table = new DataTable();
                    table.Columns.Add("orderNo", typeof(string));
                    foreach (var file in files)
                    {
                        string orderNo = Path.GetFileNameWithoutExtension(file);

                        //kiểm tra xem có chữ log không
                        if (orderNo.ToLower().Contains("log")) continue;

                        //kiểm tra xem có chữ delete không
                        if (orderNo.ToLower().Contains("delete")) continue;

                        //kiểm tra xem có chữ complete không
                        if (orderNo.ToLower().Contains("complete")) continue;
                        DataRow row = table.NewRow();
                            row["orderNo"] = orderNo;
                        //kiểm tra xem có bị hủy không
                        using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
                        {
                            string query = "SELECT COUNT(*) FROM PO WHERE orderNO = @orderNo AND Action = 'Deleted'";
                            var command = new SQLiteCommand(query, conn);
                            command.Parameters.AddWithValue("@orderNo", orderNo);
                            conn.Open();
                            int count = Convert.ToInt32(command.ExecuteScalar());
                            if (count > 0)
                            {
                                //nếu bị hủy thì bỏ qua
                                continue;
                            }
                        }

                        //Kiểm tra số lượng sản phẩm đã gửi lên MES thành công
                        try
                        {
                            //Đọc orderQty từ file JSON
                            string jsonContent = File.ReadAllText(file);
                            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);

                            if (jsonObj != null && jsonObj.ContainsKey("orderQty"))
                            {
                                int orderQty = Convert.ToInt32(jsonObj["orderQty"]);

                                //Kiểm tra database có tồn tại không
                                string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                                if (File.Exists(czRunPath))
                                {
                                    //Đếm số lượng sản phẩm đã gửi thành công (Send_Status = 'Sent' và Recive_Status = 'Sent' hoặc 200)
                                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                                    {
                                        conn.Open();
                                        string query = @"SELECT COUNT(*) FROM UniqueCodes
                                                        WHERE Send_Status = 'Sent'
                                                        AND (Recive_Status = 'Sent' OR Recive_Status = '200')";
                                        var command = new SQLiteCommand(query, conn);
                                        int sentSuccessCount = Convert.ToInt32(command.ExecuteScalar());

                                        //Nếu số lượng đã gửi >= orderQty thì bỏ qua PO này
                                        if (sentSuccessCount >= orderQty)
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            //Nếu có lỗi khi kiểm tra thì vẫn hiển thị PO này
                        }

                        table.Rows.Add(row);
                        //}

                    }
                    DataRow emptyRow = table.NewRow();
                    emptyRow["orderNo"] = "PO001";
                    table.Rows.InsertAt(emptyRow, 0);
                    comboBox.DataSource = table;
                    comboBox.DisplayMember = "orderNo";
                    comboBox.ValueMember = "orderNo";

                    return (true, "Tải danh sách orderNo thành công.");
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi GCJ001 khi tải danh sách orderNo: {ex.Message}");
                }
            }
        }

        #endregion

        #region Production Data Access
        public class GetDataPO
        {
            public TResult getCodeInfo(string Code, string OrderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(OrderNo)}/{OrderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu ghi không tồn tại.");
                    }

                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM UniqueCodes WHERE Code Like @Code";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@Code", Code);
                        var adapter = new SQLiteDataAdapter(command);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0)
                            ? new TResult(true, "Lấy thông tin mã CZ thành công.", table.Rows.Count, table)
                            : new TResult(false, "Không tìm thấy mã CZ: " + Code);
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P05401 khi lấy thông tin mã CZ: {ex.Message}");
                }
            }

            /// <summary>
            /// Kiểm tra xem code đã được activate ở PO khác cùng GTIN chưa
            /// </summary>
            public TResult CheckCodeActivatedInOtherPO(string Code, string CurrentOrderNo)
            {
                try
                {
                    // Lấy GTIN của PO hiện tại
                    string gtin = GetGTIN(CurrentOrderNo);
                    if (string.IsNullOrEmpty(gtin))
                    {
                        return new TResult(false, "Không tìm thấy GTIN cho orderNo hiện tại.");
                    }

                    // Lấy đường dẫn base (yyyy-MM/GTIN/)
                    string basePath = GetOrderBasePath(CurrentOrderNo);
                    if (!Directory.Exists(basePath))
                    {
                        return new TResult(false, "Thư mục base không tồn tại.");
                    }

                    // Lấy tất cả các file .db trong thư mục (trừ file hiện tại)
                    var allDbFiles = Directory.GetFiles(basePath, "*.db")
                        .Where(f => !f.Contains("Record_") && !f.Contains("carton_") &&
                                   !f.Contains("Send_AWS_") && !f.Contains("Recive_AWS_"))
                        .Where(f => !Path.GetFileName(f).Equals($"{CurrentOrderNo}.db", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    // Kiểm tra từng database xem code đã được activate chưa
                    foreach (var dbFile in allDbFiles)
                    {
                        try
                        {
                            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
                            {
                                conn.Open();
                                string query = "SELECT * FROM UniqueCodes WHERE Code = @Code AND Status != 0";
                                var command = new SQLiteCommand(query, conn);
                                command.Parameters.AddWithValue("@Code", Code);
                                var adapter = new SQLiteDataAdapter(command);
                                var table = new DataTable();
                                adapter.Fill(table);

                                if (table.Rows.Count > 0)
                                {
                                    // Tìm thấy code đã được activate ở PO khác
                                    string otherPO = Path.GetFileNameWithoutExtension(dbFile);
                                    string activateDate = table.Rows[0]["ActivateDate"].ToString();
                                    string activateUser = table.Rows[0]["ActivateUser"].ToString();

                                    return new TResult(false,
                                        $"Mã đã được sử dụng ở PO: {otherPO} | Thời gian: {activateDate} | User: {activateUser} | GTIN: {gtin}",
                                        0, table);
                                }
                            }
                        }
                        catch
                        {
                            // Bỏ qua lỗi với database cụ thể, tiếp tục kiểm tra các database khác
                            continue;
                        }
                    }

                    // Không tìm thấy code được activate ở PO nào khác
                    return new TResult(true, "Mã chưa được sử dụng ở PO nào khác.");
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi PH05402 khi kiểm tra code ở các PO khác: {ex.Message}");
                }
            }

            /// <summary>
            /// Lấy tất cả các mã đã được activate từ các PO khác cùng GTIN
            /// Dùng để lọc mã khi tạo DB mới
            /// </summary>
            public (bool success, string message, HashSet<string> activatedCodes) GetAllActivatedCodesFromOtherPOs(string currentOrderNo)
            {
                var activatedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    // Lấy GTIN của PO hiện tại
                    string gtin = GetGTIN(currentOrderNo);
                    if (string.IsNullOrEmpty(gtin))
                    {
                        return (false, $"Không tìm thấy GTIN cho orderNo: {currentOrderNo}", activatedCodes);
                    }

                    // Lấy đường dẫn base (yyyy-MM/GTIN/)
                    string basePath = GetOrderBasePath(currentOrderNo);
                    if (!Directory.Exists(basePath))
                    {
                        // Nếu thư mục chưa tồn tại, nghĩa là đây là PO đầu tiên của GTIN này
                        return (true, "Thư mục GTIN chưa tồn tại, đây là PO đầu tiên.", activatedCodes);
                    }

                    // Lấy tất cả các file .db trong thư mục (trừ file hiện tại và các file phụ)
                    var allDbFiles = Directory.GetFiles(basePath, "*.db")
                        .Where(f => !f.Contains("Record_") && !f.Contains("carton_") &&
                                   !f.Contains("Send_AWS_") && !f.Contains("Recive_AWS_"))
                        .Where(f => !Path.GetFileName(f).Equals($"{currentOrderNo}.db", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (allDbFiles.Count == 0)
                    {
                        return (true, "Không có PO nào khác cùng GTIN.", activatedCodes);
                    }

                    // Đọc tất cả các mã đã được activate (Status != 0) từ các PO khác
                    foreach (var dbFile in allDbFiles)
                    {
                        try
                        {
                            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
                            {
                                conn.Open();
                                string query = "SELECT Code FROM UniqueCodes WHERE Status != 0";
                                var command = new SQLiteCommand(query, conn);

                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string code = reader["Code"].ToString();
                                        if (!string.IsNullOrEmpty(code))
                                        {
                                            activatedCodes.Add(code);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log lỗi nhưng tiếp tục với các database khác
                            Console.WriteLine($"Lỗi khi đọc database {dbFile}: {ex.Message}");
                            continue;
                        }
                    }

                    string message = activatedCodes.Count > 0
                        ? $"Đã tìm thấy {activatedCodes.Count} mã đã được Active từ {allDbFiles.Count} PO khác cùng GTIN {gtin}"
                        : $"Không có mã nào đã được Active từ {allDbFiles.Count} PO khác cùng GTIN {gtin}";

                    return (true, message, activatedCodes);
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi PH05403 khi lấy danh sách mã đã Active: {ex.Message}", activatedCodes);
                }
            }

            public TResult getCodeInfoWithCartonCode(string OrderNo, string cartonCode)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(OrderNo)}/{OrderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu ghi không tồn tại.");
                    }

                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM UniqueCodes WHERE cartonCode = @carton";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@carton", cartonCode);
                        var adapter = new SQLiteDataAdapter(command);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0)
                            ? new TResult(true, "Lấy thông tin mã CZ thành công.", table.Rows.Count, table)
                            : new TResult(false, "Không tìm thấy");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P05401 khi lấy thông tin mã CZ: {ex.Message}");
                }
            }
            public TResult GetLastPO()
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
                            return new TResult(true, "Lấy PO thành công.", table.Rows.Count, table);
                        }
                        else
                        {
                            return new TResult(true, "Không có PO nào trong cơ sở dữ liệu.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P03 khi lấy PO: {ex.Message}");
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
                        return (table.Rows.Count > 0) ? 
                            (true, table, "Lấy lịch sử PO thành công.") : 
                            (false, null, "Không có lịch sử PO nào cho orderNo: " + orderNo);
                    }
                }
                catch (Exception ex)
                {
                    return (false, null, $"Lỗi P05 khi lấy lịch sử PO: {ex.Message}");
                }
            }

            public TResult Get_Record_Count(string orderNO)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNO)}/Record_{orderNO}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Không có PO nào trong cơ sở dữ liệu.");
                    }
                    
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM Records";
                        var command = new SQLiteCommand(query, conn);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (count > 0)
                            ? new TResult(true, "Lấy số lượng bản ghi thành công.", count)
                            : new TResult(true, "Không có bản ghi nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P03 khi lấy PO: {ex.Message}");
                }
            }

            public TResult Get_Record_Count_By_Status(string orderNO, e_Production_Status Production_Status)
            {
                string czRunPath = "";
                try
                {
                     czRunPath = $"{GetOrderBasePath(orderNO)}/Record_{orderNO}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(true, "Cơ sở dữ liệu ghi không tồn tại.");

                    }
                    
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM `Records` WHERE Status = @Status";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@Status", Production_Status.ToString());
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (count > 0)
                            ? new TResult(true, "Lấy số lượng bản ghi theo trạng thái thành công.", count)
                            : new TResult(true, "Không có bản ghi nào trong cơ sở dữ liệu với trạng thái: " + Production_Status.ToString());
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P06 khi lấy số lượng bản ghi theo trạng thái: {ex.Message}, {czRunPath}");
                }
            }

            public TResult Get_Record_Count_CameraSub_By_Status(string orderNO, e_Production_Status Production_Status)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNO)}/Record_CameraSub_{orderNO}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(true, "Cơ sở dữ liệu ghi không tồn tại.");

                    }

                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM Records_CameraSub WHERE Status = @Status";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@Status", Production_Status.ToString());
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (count > 0)
                            ? new TResult(true, "Lấy số lượng bản ghi theo trạng thái thành công.", count)
                            : new TResult(true, "Không có bản ghi nào trong cơ sở dữ liệu với trạng thái: " + Production_Status.ToString());
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P061 khi lấy số lượng bản ghi theo trạng thái: {ex.Message}");
                }
            }

            //lấy số gửi AWS đã nhận recive
            public TResult Get_Record_Sent_Recive_Count(string orderNo, e_AWS_Send_Status Send, e_AWS_Recive_Status Recive, string Recive_Conditional = "=", string Conditional = "")
            {
                try
                {   //tạo thư mục nếu chưa tồn tại
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(true, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        string query = $"SELECT COUNT(*) FROM UniqueCodes WHERE Send_Status =@send AND Recive_Status {Recive_Conditional} @recive {Conditional}; ";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@send", Send.ToString());
                        command.Parameters.AddWithValue("@recive", Recive.ToString());
                        conn.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (count > 0)
                            ? new TResult(true, "Lấy số lượng bản ghi gửi AWS thành công.", count)
                            : new TResult(true, "Không có bản ghi nào trong cơ sở dữ liệu với trạng thái gửi: " + Send.ToString() + " và nhận: " + Recive.ToString());
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi P07 khi lấy số lượng bản ghi gửi AWS: {ex.Message}");
                }

            }


            public TResult Get_Records(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/Record_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu ghi không tồn tại.");
                    }

                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Records";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0)
                            ? new TResult(true, "Lấy danh sách bản ghi thành công.", 0, table)
                            : new TResult(true, "Không có bản ghi nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi PH08 khi lấy danh sách bản ghi: {ex.Message}");
                }
            }

            public TResult Get_Records_CameraSub(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/Record_CameraSub_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu ghi camera phụ không tồn tại.");
                    }

                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Records_CameraSub";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0)
                            ? new TResult(true, "Lấy danh sách bản ghi camera phụ thành công.", 0, table)
                            : new TResult(true, "Không có bản ghi nào trong cơ sở dữ liệu camera phụ.");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi PH08CS khi lấy danh sách bản ghi camera phụ: {ex.Message}");
                }
            }

            //lấy danh sách thùng 
            public TResult Get_Cartons(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/carton_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Carton";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? new TResult (true, "Lấy danh sách bản ghi thành công.", table.Rows.Count, table) : new TResult (true, "Không có bản ghi nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult (false, $"Lỗi PH10 khi lấy danh sách bản ghi: {ex.Message}");
                }
            }

            public (bool issucess, DataTable Codes, string message) Get_Codes(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
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

            /// <summary>
            /// Kiểm tra mã có tồn tại trong old_database.db không
            /// Trả về: (exists, cartonCode, message)
            /// - exists: true nếu mã tồn tại, false nếu không
            /// - cartonCode: giá trị cartonCode của mã trong old_database (nếu tồn tại)
            /// </summary>
            public (bool exists, string cartonCode, string message) Check_Code_In_Old_Database(string code)
            {
                try
                {
                    string oldDbPath = @"C:\MasanSerialization_v2\Databases\old_database.db";
                    if (!File.Exists(oldDbPath))
                    {
                        return (false, null, "File old_database.db không tồn tại.");
                    }

                    using (var conn = new SQLiteConnection($"Data Source={oldDbPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT cartonCode FROM UniqueCodes WHERE Code = @Code";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@Code", code);
                        
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            string cartonCode = result.ToString();
                            return (true, cartonCode, "Mã tồn tại trong old_database.");
                        }
                        else
                        {
                            return (false, null, "Mã không tồn tại trong old_database.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return (false, null, $"Lỗi PH_OLD_DB khi kiểm tra mã trong old_database: {ex.Message}");
                }
            }

            //lấy mã code với điều kiện status !=0, cartonCode != 0 và send_status = 'Pending'
            /// <summary>
            /// Lấy danh sách codes cần gửi lên AWS
            /// LƯU Ý: Method này chỉ lấy codes từ database của PO hiện tại.
            /// Với cơ chế GTIN mới, các PO cùng GTIN dùng chung bộ mã nhưng có database riêng.
            /// Cross-PO duplicate đã được kiểm tra ở CameraMain_Process, nên mã không thể activate lại ở PO khác.
            /// </summary>
            public TResult Get_Codes_Send(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM UniqueCodes WHERE Status != 0 AND Send_Status != 'Sent' AND cartonCode != 'pending' AND cartonCode != '0' ";

                        var command = new SQLiteCommand(query, conn);
                        var adapter = new SQLiteDataAdapter(command);
                        var table = new DataTable();
                        adapter.Fill(table);

                        return (table.Rows.Count > 0)
                            ? new TResult(true, "Lấy danh sách mã code thành công.", table.Rows.Count, table)
                            : new TResult(false, "Không có mã code nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi PH11 khi lấy danh sách mã code: {ex.Message}");
                }
            }

            public TResult Get_Codes_Send_Failed(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = $"SELECT * FROM UniqueCodes WHERE Status != 0 AND Send_Status == 'Failed' AND cartonCode != 'pending'";

                        var command = new SQLiteCommand(query, conn);
                        var adapter = new SQLiteDataAdapter(command);
                        var table = new DataTable();
                        adapter.Fill(table);

                        return (table.Rows.Count > 0)
                            ? new TResult(true, "Lấy danh sách mã code thành công.", 0, table)
                            : new TResult(false, "Không có mã code nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi PH1123 khi lấy danh sách mã code: {ex.Message}");
                }
            }

            public TResult Get_Codes_Sent_Timeout(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = $"SELECT * FROM UniqueCodes WHERE Status != 0 AND Send_Status == 'Sent' AND Recive_Status = 'Pending'";

                        var command = new SQLiteCommand(query, conn);
                        var adapter = new SQLiteDataAdapter(command);
                        var table = new DataTable();
                        adapter.Fill(table);

                        return (table.Rows.Count > 0)
                            ? new TResult(true, "Lấy danh sách mã code thành công.", 0, table)
                            : new TResult(false, "Không có mã code nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi PH1123 khi lấy danh sách mã code: {ex.Message}");
                }
            }
            //lấy mã thùng lớn nhất
            public (bool issucess, int MaxCartonID, string message) Get_Max_Carton_ID(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/carton_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, 0, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT MAX(ID) FROM Carton";
                        var command = new SQLiteCommand(query, conn);
                        int maxCartonID = Convert.ToInt32(command.ExecuteScalar());
                        return (true, maxCartonID, "Lấy ID thùng lớn nhất thành công.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, 0, $"Lỗi PH09 khi lấy ID thùng lớn nhất: {ex.Message}");
                }
            }

            //lấy thông tin thùng từ mã
            public (bool issucess, DataRow Carton, string message) Get_Carton_By_Code(string orderNo, string cartonCode)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/carton_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, null, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Carton WHERE cartonCode = @cartonCode";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@cartonCode", cartonCode);
                        var adapter = new SQLiteDataAdapter(command);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, table.Rows[0], "Lấy thông tin thùng thành công.") : (false, null, "Không tìm thấy thùng với mã: " + cartonCode);
                    }
                }
                catch (Exception ex)
                {
                    return (false, null, $"Lỗi PH10 khi lấy thông tin thùng: {ex.Message}");
                }
            }

            //lấy id thùng từ mã
            public (bool issucess, int CartonID, string message) Get_Carton_ID_By_Code(string orderNo, string cartonCode)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/carton_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, 0, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT ID FROM Carton WHERE Code = @cartonCode";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@cartonCode", cartonCode);
                        int cartonID = Convert.ToInt32(command.ExecuteScalar());
                        return (true, cartonID, "Lấy ID thùng thành công.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, 0, $"Lỗi PH10 khi lấy ID thùng từ mã: {ex.Message}");
                }
            }

            //lấy số lượng chai trong thùng theo id thùng
            public (bool issucess, int Count, string message) Get_Product_Carton_Count(string orderNo, int cartonID)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/Record_CameraSub_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, 0, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM Carton WHERE ID = @cartonID AND Status = 'Pass'";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@cartonID", cartonID);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (true, count, "Lấy số lượng chai trong thùng thành công.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, 0, $"Lỗi PH10 khi lấy số lượng chai trong thùng: {ex.Message}");
                }
            }

            //lấy tất cả các sp trong record camer sub có status = pass và carton id = 
            public (bool issucess, DataTable Records, string message) Get_Product_Carton_Records(string orderNo, int cartonID)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/Record_CameraSub_{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return (false, null, "Cơ sở dữ liệu ghi không tồn tại.");
                    }
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Records_CameraSub WHERE CartonID = @cartonID AND Status = 'Pass'";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@cartonID", cartonID);
                        var adapter = new SQLiteDataAdapter(command);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, table, "Lấy danh sách bản ghi thành công.") : (true, null, "Không có bản ghi nào trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    return (false, null, $"Lỗi PH11 khi lấy danh sách bản ghi: {ex.Message}");
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

            /// <summary>
            /// Kiểm tra số lượng mã trong dictionary (UniqueCodes) với số lượng từ MES
            /// </summary>
            public TResult Check_Dictionary_Codes_Count(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Database dictionary chưa tồn tại.", 0);
                    }

                    // Lấy số lượng mã trong dictionary
                    int dictionaryCount = 0;
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM UniqueCodes";
                        var command = new SQLiteCommand(query, conn);
                        dictionaryCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    return new TResult(true, $"Dictionary có {dictionaryCount} mã.", dictionaryCount);
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi khi kiểm tra dictionary: {ex.Message}", 0);
                }
            }

            /// <summary>
            /// Lấy danh sách các mã đã có trong dictionary
            /// </summary>
            public TResult Get_Existing_Codes_In_Dictionary(string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Database dictionary chưa tồn tại.");
                    }

                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT Code FROM UniqueCodes";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);

                        return new TResult(true, $"Lấy {table.Rows.Count} mã từ dictionary thành công.", table.Rows.Count, table);
                    }
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi khi lấy danh sách mã từ dictionary: {ex.Message}");
                }
            }

            /// <summary>
            /// Kiểm tra và bổ sung mã thiếu vào dictionary từ MES
            /// Lưu ý: Codes được lưu theo GTIN, nên các PO có cùng GTIN sẽ dùng chung bộ mã
            /// </summary>
            public TResult Check_And_Add_Missing_Codes_To_Dictionary(string orderNo, GetfromMES getfromMES)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                    if (!File.Exists(czRunPath))
                    {
                        return new TResult(false, "Database dictionary chưa tồn tại.");
                    }

                    // Lấy số lượng mã từ MES (theo GTIN)
                    var mesCountResult = getfromMES.Get_Unique_Code_MES_Count(orderNo);
                    if (!mesCountResult.issuccess)
                    {
                        return new TResult(false, $"Không thể lấy số lượng mã từ MES: {mesCountResult.message}");
                    }
                    int mesCount = mesCountResult.count;

                    // Lấy số lượng mã trong dictionary
                    var dictCountResult = Check_Dictionary_Codes_Count(orderNo);
                    if (!dictCountResult.issuccess)
                    {
                        return new TResult(false, $"Không thể kiểm tra dictionary: {dictCountResult.message}");
                    }
                    int dictCount = dictCountResult.count;

                    // Nếu đủ số lượng thì không cần làm gì
                    if (dictCount >= mesCount)
                    {
                        string gtin = GetGTIN(orderNo);
                        return new TResult(true, $"Dictionary đã đủ số lượng mã ({dictCount}/{mesCount}) cho GTIN {gtin}.", dictCount);
                    }

                    // Nếu thiếu, lấy danh sách mã từ MES (theo GTIN)
                    var mesCodesResult = getfromMES.Get_Unique_Codes_MES(orderNo);
                    if (!mesCodesResult.issuccess)
                    {
                        return new TResult(false, $"Không thể lấy danh sách mã từ MES: {mesCodesResult.message}");
                    }

                    // Lấy danh sách mã đã có trong dictionary
                    var existingCodesResult = Get_Existing_Codes_In_Dictionary(orderNo);
                    HashSet<string> existingCodes = new HashSet<string>();
                    if (existingCodesResult.issuccess && existingCodesResult.data != null)
                    {
                        foreach (DataRow row in existingCodesResult.data.Rows)
                        {
                            existingCodes.Add(row["Code"].ToString());
                        }
                    }

                    // Thêm các mã thiếu vào dictionary
                    int addedCount = 0;
                    using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        conn.Open();
                        using (var transaction = conn.BeginTransaction())
                        {
                            foreach (DataRow row in mesCodesResult.data.Rows)
                            {
                                // Dữ liệu từ MES trả về cột "Code" (chữ hoa). Dùng đúng tên cột để tránh chèn rỗng.
                                string code = row["Code"].ToString();

                                // Chỉ thêm nếu chưa tồn tại
                                if (!existingCodes.Contains(code))
                                {
                                    string insertQuery = "INSERT INTO UniqueCodes (Code) VALUES (@Code)";
                                    var command = new SQLiteCommand(insertQuery, conn, transaction);
                                    command.Parameters.AddWithValue("@Code", code);
                                    command.ExecuteNonQuery();
                                    addedCount++;
                                }
                            }
                            transaction.Commit();
                        }
                    }

                    string gtinInfo = GetGTIN(orderNo);
                    return new TResult(true, $"Đã bổ sung {addedCount} mã thiếu vào dictionary cho GTIN {gtinInfo}. Tổng: {dictCount + addedCount}/{mesCount}", addedCount);
                }
                catch (Exception ex)
                {
                    return new TResult(false, $"Lỗi khi kiểm tra và bổ sung mã: {ex.Message}");
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

                return (true, "Ghi PO thành công vào cơ sở dữ liệu.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi P06 khi ghi PO vào cơ sở dữ liệu: {ex.Message}");
            }
        }

        public TResult Delete_PO(string orderNo, string userName)
        {
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={POLog_dbPath};Version=3;"))
                {
                    conn.Open();
                    string insertQuery = @"
                        INSERT INTO PO (orderNO, productionDate, Action, UserName, Counter, Timestamp, Timeunix)
                        VALUES (@orderNo, '', 'Deleted', @UserName, '{}', @Timestamp, @Timeunix)";
                    var command = new SQLiteCommand(insertQuery, conn);
                    command.Parameters.AddWithValue("@orderNo", orderNo);
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow.ToString("o"));
                    command.Parameters.AddWithValue("@Timeunix", ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds());
                    command.ExecuteNonQuery();
                }
                return new TResult(true, "Xóa PO thành công vào cơ sở dữ liệu.");
            }
            catch (Exception ex)
            {
                return new TResult(false, $"Lỗi P08 khi xóa PO vào cơ sở dữ liệu: {ex.Message}");
            }
        }
        public class PostDB
        {

            public void Update_Active_Status(ProductionCodeData productionCodeData, string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        connection.Open();
                        string query = "UPDATE `UniqueCodes` SET " +
                                       "`Status` = '1', " +
                                       "`cartonCode` = @cartonCode, " +
                                       "`ActivateDate` = @activateDate, " +
                                       "`ProductionDate` = @productionDate,  " +
                                       "`ActivateUser` = @UserName,  " +
                                       "`SubCamera_ActivateDate` = @subCamera_ActivateDate" +
                                       " WHERE `ID` = @RowId ;";

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@RowId", productionCodeData.codeID);
                            command.Parameters.AddWithValue("@cartonCode", productionCodeData.cartonCode);
                            command.Parameters.AddWithValue("@activateDate", productionCodeData.Activate_Datetime);
                            command.Parameters.AddWithValue("@productionDate", productionCodeData.Production_Datetime);
                            command.Parameters.AddWithValue("@UserName", productionCodeData.Activate_User);
                            command.Parameters.AddWithValue("@subCamera_ActivateDate", productionCodeData.Sub_Camera_Activate_Datetime);
                            int rowsAffected = command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi P07 khi cập nhật trạng thái kích hoạt: {ex.Message}");
                }

            }

            public void Update_Active_Status_With_KV_where_KV (string key, string value, string wkey, string wvalue, string orderNo)
            {
                string czRunPath = $"{GetOrderBasePath(orderNo)}/{orderNo}.db";
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    connection.Open();
                    string query = $"UPDATE `UniqueCodes` SET `{key}` = @value WHERE `{wkey}` = @wvalue";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@wvalue", wvalue);
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }

            public void Insert_Record (ProductionCodeData_Record productionCodeData_Record, string orderNo)
            {
                string czRunPath = $"{GetOrderBasePath(orderNo)}/Record_{orderNo}.db";
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    connection.Open();
                    string query = "INSERT INTO Records (Code, cartonCode, Status, PLC_Status, ActivateDate, ActivateUser, ProductionDate) " +
                                   "VALUES (@Code, @cartonCode, @Status, @PLC_Status, @ActivateDate, @ActivateUser, @ProductionDate)";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Code", productionCodeData_Record.code);
                        command.Parameters.AddWithValue("@cartonCode", productionCodeData_Record.cartonCode);
                        command.Parameters.AddWithValue("@Status", productionCodeData_Record.status.ToString());
                        command.Parameters.AddWithValue("@PLC_Status", productionCodeData_Record.PLCStatus.ToString());
                        command.Parameters.AddWithValue("@ActivateDate", productionCodeData_Record.Activate_Datetime);
                        command.Parameters.AddWithValue("@ActivateUser", productionCodeData_Record.Activate_User);
                        command.Parameters.AddWithValue("@ProductionDate", productionCodeData_Record.Production_Datetime);
                        command.ExecuteNonQuery();
                    }
                }
            }

            public void Insert_Carton (ProductionCartonData productionCartonData, string orderNo)
            {
                string czRunPath = $"{GetOrderBasePath(orderNo)}/carton_{orderNo}.db";
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    connection.Open();
                    string query = "INSERT INTO Carton (cartonCode, Start_Datetime, Activate_Datetime, ActivateUser, ProductionDate) " +
                                   "VALUES (@cartoncode, @startime, @activatetime, @ActivateUser, @ProductionDate)";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cartoncode", productionCartonData.cartonCode);
                        command.Parameters.AddWithValue("@startime", productionCartonData.Start_Datetime);
                        command.Parameters.AddWithValue("@activatetime", productionCartonData.Activate_Datetime);
                        command.Parameters.AddWithValue("@ActivateUser", productionCartonData.Activate_User);
                        command.Parameters.AddWithValue("@ProductionDate", productionCartonData.Production_Datetime);
                        command.ExecuteNonQuery();
                    }
                }
            }

            //update thông tin của thùng carton

            public void Update_Carton(ProductionCartonData productionCartonData, string orderNo)
            {
                try
                {
                    string czRunPath = $"{GetOrderBasePath(orderNo)}/carton_{orderNo}.db";
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                    {
                        connection.Open();
                        string query = "UPDATE Carton SET cartonCode = @cartonCode, Start_Datetime = @startime, Activate_Datetime = @activatetime, ActivateUser = @ActivateUser, ProductionDate = @ProductionDate WHERE ID = @ID";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ID", productionCartonData.cartonID);
                            command.Parameters.AddWithValue("@cartonCode", productionCartonData.cartonCode);
                            command.Parameters.AddWithValue("@startime", productionCartonData.Start_Datetime);
                            command.Parameters.AddWithValue("@activatetime", productionCartonData.Activate_Datetime);
                            command.Parameters.AddWithValue("@ActivateUser", productionCartonData.Activate_User);
                            command.Parameters.AddWithValue("@ProductionDate", productionCartonData.Production_Datetime);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi P08 khi cập nhật thông tin thùng carton: {ex.Message}");
                }

            }

            public void Insert_Record_Camera_Sub(ProductionCodeData_Record productionCodeData_Record, string orderNo)
            {
                string czRunPath = $"{GetOrderBasePath(orderNo)}/Record_CameraSub_{orderNo}.db";
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                {
                    connection.Open();
                    string query = "INSERT INTO Records_CameraSub (Code, cartonID, Status, PLC_Status, ActivateDate, ActivateUser, ProductionDate) " +
                                   "VALUES (@Code, @cartonID, @Status, @PLC_Status, @ActivateDate, @ActivateUser, @ProductionDate)";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Code", productionCodeData_Record.code);
                        command.Parameters.AddWithValue("@cartonID", productionCodeData_Record.cartonID);
                        command.Parameters.AddWithValue("@Status", productionCodeData_Record.status.ToString());
                        command.Parameters.AddWithValue("@PLC_Status", productionCodeData_Record.PLCStatus.ToString());
                        command.Parameters.AddWithValue("@ActivateDate", productionCodeData_Record.Activate_Datetime);
                        command.Parameters.AddWithValue("@ActivateUser", productionCodeData_Record.Activate_User);
                        command.Parameters.AddWithValue("@ProductionDate", productionCodeData_Record.Production_Datetime);
                        command.ExecuteNonQuery();
                    }
                }
            }

        }

        #endregion

        #region Internal Helper Methods
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
                        CREATE TABLE ""PO"" (
                            ""ID"" INTEGER NOT NULL UNIQUE,
                            ""orderNO"" TEXT NOT NULL,
                            ""productionDate"" TEXT NOT NULL,
                            ""Action"" TEXT NOT NULL,
                            ""UserName"" TEXT NOT NULL,
                            ""Counter"" JSON NOT NULL,
                            ""Timestamp"" TEXT NOT NULL,
                            ""Timeunix"" INTEGER NOT NULL,
                            PRIMARY KEY(""ID"" AUTOINCREMENT)
                        );";
                    var command = new SQLiteCommand(createTableQuery, conn);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Lấy thông tin productionDate và GTIN từ orderNo
        /// </summary>
        private static (string productionDate, string gtin) GetPOInfo(string orderNo)
        {
            try
            {
                string jsonPath = Path.Combine(poMesJsonPODataPath, orderNo + ".json");
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);

                    string productionDate = jsonObj.ContainsKey("productionDate") ? jsonObj["productionDate"].ToString() : string.Empty;
                    string gtin = jsonObj.ContainsKey("GTIN") ? jsonObj["GTIN"].ToString() : string.Empty;

                    return (productionDate, gtin);
                }
            }
            catch { }

            return (string.Empty, string.Empty);
        }

        /// <summary>
        /// Lấy GTIN từ orderNo
        /// </summary>
        private static string GetGTIN(string orderNo)
        {
            var (_, gtin) = GetPOInfo(orderNo);
            return gtin;
        }

        /// <summary>
        /// Lấy đường dẫn base cho database theo cấu trúc yyyy-MM/GTIN/
        /// </summary>
        public static string GetOrderBasePath(string orderNo)
        {
            var (productionDate, gtin) = GetPOInfo(orderNo);

            if (string.IsNullOrEmpty(productionDate) || string.IsNullOrEmpty(gtin))
            {
                // Fallback về đường dẫn cũ nếu không lấy được thông tin
                return dataPath;
            }

            try
            {
                // Parse productionDate để lấy yyyy-MM
                DateTime date = DateTime.Parse(productionDate);
                string monthFolder = date.ToString("yyyy-MM");

                // Tạo đường dẫn: dataPath/yyyy-MM/GTIN/
                string basePath = Path.Combine(dataPath, monthFolder, gtin);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                return basePath;
            }
            catch
            {
                // Fallback về đường dẫn cũ nếu có lỗi
                return dataPath;
            }
        }
        #endregion

        #region Database File Management
        public (bool issucess, string message) Check_Database_File(string orderNo, string orderQty)
        {
            try
            {
                // Bước 1: Kiểm tra file JSON PO data tồn tại
                string jsonPath = Path.Combine(poMesJsonPODataPath, orderNo + ".json");
                if (!File.Exists(jsonPath))
                {
                    return (false, $"PH001: Không tìm thấy file JSON PO: {jsonPath}");
                }

                //// Bước 2: Kiểm tra file codes JSON tồn tại
                //string codesJsonPath = Path.Combine(poMesJsonCodesPath, orderNo + ".json");
                //if (!File.Exists(codesJsonPath))
                //{
                //    return (false, $"PH002: Không tìm thấy file codes JSON: {codesJsonPath}");
                //}

                // Bước 3: Lấy đường dẫn base (có thể tạo thư mục)
                string basePath = GetOrderBasePath(orderNo);
                if (string.IsNullOrEmpty(basePath))
                {
                    return (false, "PH003: Không thể xác định đường dẫn lưu trữ");
                }

                // Bước 4: Tạo đường dẫn file database chính
                string czRunPath = Path.Combine(basePath, $"{orderNo}.db");
                // Bước 5: Đảm bảo thư mục tồn tại
                string directoryPath = Path.GetDirectoryName(czRunPath);
                if (!Directory.Exists(directoryPath))
                {
                    try
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    catch (Exception ex)
                    {
                        return (false, $"PH004: Không thể tạo thư mục {directoryPath}: {ex.Message}");
                    }
                }

                // Bước 6: Tạo file database chính nếu chưa tồn tại
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
                                            ""SubCamera_ActivateDate""	TEXT NOT NULL DEFAULT 0,
	                                        ""Send_Status""	TEXT NOT NULL DEFAULT Pending,
	                                        ""Recive_Status""	TEXT NOT NULL DEFAULT Pending,
	                                        ""Send_Recive_Logs""	JSON,
	                                        ""Duplicate""	JSON,
	                                        PRIMARY KEY(""ID"" AUTOINCREMENT)
                                        );
                                        PRAGMA journal_mode = WAL;
                                        PRAGMA synchronous = NORMAL;
                                        PRAGMA cache_size = 1000000;
                                        PRAGMA temp_store = memory;
                                        ";
                        var command = new SQLiteCommand(createTableQuery, conn);
                        command.ExecuteNonQuery();
                    }

                    //insert từ bảng CZ nhận từ MES vào bảng CZ run này
                    // CHÚ Ý: Lấy codes theo GTIN thay vì orderNo
                    // Các PO có cùng GTIN sẽ dùng chung 1 bộ mã

                    // Bước 6.1: Lấy danh sách mã đã được Active từ các PO khác cùng GTIN
                    var getActivatedCodes = getDataPO.GetAllActivatedCodesFromOtherPOs(orderNo);
                    if (!getActivatedCodes.success)
                    {
                        // Nếu có lỗi khi lấy danh sách mã đã Active, return lỗi
                        return (false, $"PH005: {getActivatedCodes.message}");
                    }
                    var activatedCodes = getActivatedCodes.activatedCodes;

                    // Bước 6.2: Lấy tất cả mã từ MES (theo GTIN)
                    var getczCodes = getfromMES.Get_Unique_Codes_MES(orderNo);
                    if (!getczCodes.issuccess)
                    {
                        return (false, getczCodes.message);
                    }
                    DataTable czCodes = getczCodes.data;

                    // Bước 6.3: Lọc ra các mã chưa được Active
                    List<string> availableCodes = new List<string>();
                    foreach (DataRow row in czCodes.Rows)
                    {
                        string code = row["Code"].ToString();
                        if (!activatedCodes.Contains(code))
                        {
                            availableCodes.Add(code);
                        }
                    }

                    // Bước 6.4: Kiểm tra số lượng mã còn lại có đủ không
                    int requiredQty = orderQty.ToInt32();
                    if (availableCodes.Count < requiredQty)
                    {
                        return (false,
                            $"PH006: Không đủ mã! Cần: {requiredQty} | Còn lại: {availableCodes.Count} | " +
                            $"Đã dùng từ PO khác: {activatedCodes.Count} | Tổng: {czCodes.Rows.Count}\n" +
                            $"Thông tin: {getActivatedCodes.message}");
                    }

                    // Bước 6.5: Insert các mã còn lại vào database
                    if (availableCodes.Count > 0)
                    {
                        using (var conn = new SQLiteConnection($"Data Source={czRunPath};Version=3;"))
                        {
                            conn.Open();

                            // Sử dụng transaction để tăng tốc độ insert
                            using (var transaction = conn.BeginTransaction())
                            {
                                string insertQuery = "INSERT INTO UniqueCodes (Code) VALUES (@Code)";
                                var command = new SQLiteCommand(insertQuery, conn, transaction);

                                foreach (string code in availableCodes)
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@Code", code);
                                    command.ExecuteNonQuery();
                                }

                                transaction.Commit();
                            }
                        }

                        // Log thông tin
                        Console.WriteLine($"=== Thống kê tạo DB cho PO {orderNo} ===");
                        Console.WriteLine($"Tổng mã từ MES (GTIN): {czCodes.Rows.Count}");
                        Console.WriteLine($"Đã Active từ PO khác: {activatedCodes.Count}");
                        Console.WriteLine($"Còn lại khả dụng: {availableCodes.Count}");
                        Console.WriteLine($"Yêu cầu orderQty: {requiredQty}");
                        Console.WriteLine($"Trạng thái: ✓ ĐỦ MÃ");
                        Console.WriteLine($"{getActivatedCodes.message}");
                    }
                }

                // Bước 7: Kiểm tra và tạo các file database khác
                string recordPath = Path.Combine(basePath, $"Record_{orderNo}.db");
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
                                            );
                                        PRAGMA journal_mode = WAL;
                                        PRAGMA synchronous = NORMAL;
                                        PRAGMA cache_size = 1000000;
                                        PRAGMA temp_store = memory;";
                        var command = new SQLiteCommand(createTableQuery, conn);
                        command.ExecuteNonQuery();
                    }
                }

                string recordCSPath = Path.Combine(basePath, $"Record_CameraSub_{orderNo}.db");
                if (!File.Exists(recordCSPath))
                {
                    using (var conn = new SQLiteConnection($"Data Source={recordCSPath};Version=3;"))
                    {
                        conn.Open();
                        string createTableQuery = @"CREATE TABLE ""Records_CameraSub"" (
	                                            ""ID""	INTEGER NOT NULL UNIQUE,
	                                            ""Code""	TEXT NOT NULL DEFAULT 'FAIL',
                                                ""cartonID""	INTERGER NOT NULL DEFAULT 0,
	                                            ""Status""	TEXT NOT NULL DEFAULT 0,
	                                            ""PLC_Status""	TEXT NOT NULL DEFAULT 'FAIL',
	                                            ""ActivateDate""	TEXT NOT NULL DEFAULT 0,
	                                            ""ActivateUser""	TEXT NOT NULL DEFAULT 0,
	                                            ""ProductionDate""	TEXT NOT NULL DEFAULT 0,
	                                            PRIMARY KEY(""ID"" AUTOINCREMENT)
                                            );
                                        PRAGMA journal_mode = WAL;
                                        PRAGMA synchronous = NORMAL;
                                        PRAGMA cache_size = 1000000;
                                        PRAGMA temp_store = memory;";
                        var command = new SQLiteCommand(createTableQuery, conn);
                        command.ExecuteNonQuery();
                    }
                }

                string CartonPath = Path.Combine(basePath, $"carton_{orderNo}.db");
                if (!File.Exists(CartonPath))
                {
                using (var conn = new SQLiteConnection($"Data Source={CartonPath};Version=3;"))
                {
                    conn.Open();

                    // Tạo bảng
                    string createTableQuery = @"
                                                CREATE TABLE IF NOT EXISTS Carton (
                                                    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                    cartonCode TEXT NOT NULL DEFAULT '0',
                                                    Start_Datetime TEXT NOT NULL DEFAULT '0',
                                                    Activate_Datetime TEXT NOT NULL DEFAULT 'FAIL',
                                                    ActivateUser TEXT NOT NULL DEFAULT '0',
                                                    ProductionDate TEXT NOT NULL DEFAULT '0'
                                                );
                                        PRAGMA journal_mode = WAL;
                                        PRAGMA synchronous = NORMAL;
                                        PRAGMA cache_size = 1000000;
                                        PRAGMA temp_store = memory;
                                            ";

                    using (var createCmd = new SQLiteCommand(createTableQuery, conn))
                    {
                        createCmd.ExecuteNonQuery();
                    }

                    // Tạo số thùng = orderQty / 24
                    int orderCartonQty = orderQty.ToInt32() / AppConfigs.Current.cartonPack;

                    using (var tran = conn.BeginTransaction())
                    using (var insertCmd = new SQLiteCommand(@"
                                                                INSERT INTO Carton (cartonCode, Start_Datetime, Activate_Datetime, ActivateUser, ProductionDate) 
                                                                VALUES ('0', '0', '0', 'System', '0');", conn, tran))
                    {
                        for (int i = 0; i < orderCartonQty; i++)
                        {
                            insertCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                }



            }

            //kiểm tra xem bảng ghi history tất cả các result của PO đã tồn tại hay chưa, nếu chưa thì tạo mới nếu chưa thì tạo mới
            string recordAWS = Path.Combine(basePath, $"Send_AWS_Record_{orderNo}.db");
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
                                            );
                                        PRAGMA journal_mode = WAL;
                                        PRAGMA synchronous = NORMAL;
                                        PRAGMA cache_size = 1000000;
                                        PRAGMA temp_store = memory;";
                        var command = new SQLiteCommand(createTableQuery, conn);
                        command.ExecuteNonQuery();
                    }
                }

                //kiểm tra xem bảng ghi history tất cả các result của PO đã tồn tại hay chưa, nếu chưa thì tạo mới nếu chưa thì tạo mới
                string recive_AWS = Path.Combine(basePath, $"Recive_AWS_Record_{orderNo}.db");
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
                                            );
                                        PRAGMA journal_mode = WAL;
                                        PRAGMA synchronous = NORMAL;
                                        PRAGMA cache_size = 1000000;
                                        PRAGMA temp_store = memory;";
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

    #region Enums & Data Models
    public enum ActionType
    {
        Create,
        Update,
        Delete,
        UpdateProductionDate
    }

    public enum e_Production_State
    {
        NoSelectedPO,
        Start,
        Checking_PO_Info,
        Loading,
        Camera_Processing,
        Pushing_new_PO_to_PLC,
        Pushing_continue_PO_to_PLC,
        Ready,
        Running,
        Completed,
        Editing,
        Editting_ProductionDate,
        Saving,
        Error,
        DuSanPham,
        ThieuSanPham,
        Pushing_to_Dic,
        Checking_Queue,
        Pause,
        Waiting_Stop,
        Check_After_Completed,
    }

    public enum e_Production_Status
    {
        Pass = 1,
        Fail = -1,
        Duplicate = -3,
        ReadFail = -2,
        NotFound = -4,
        Error = -5
    }

    public enum e_AWS_Send_Status
    {
        Pending,
        Sent,
        Failed
    }

    public enum e_AWS_Recive_Status
    {
        Waiting = 0,
        Pending,
        Sent =200,
        Error = 2,
        Error_404 = 404,
        Error_500 = 500,
        Error_400 = 400,
        Error_401 = 401,
        Error_403 = 403,
        Error_408 = 408,
        Error_409 = 409,
        Error_402 = 402
    }

    public enum e_Production_Order_Log_Type
    {
        Deleted,
        Completed,
        Create,
        Update,
        UpdateProductionDate,
        Error
    }

    public class TResult
    {
        public bool issuccess { get; set; }
        public string message { get; set; }
        public DataTable data { get; set; }
        public int count { get; set; }
        
        public TResult(bool issuccess, string message, int count = 0, DataTable data = null)
        {
            this.issuccess = issuccess;
            this.message = message;
            this.data = data;
            this.count = count;
        }
    }
    #endregion
}
