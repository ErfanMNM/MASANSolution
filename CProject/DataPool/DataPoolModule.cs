using Microsoft.Data.Sqlite;
using System.Data;

namespace CProject.Module
{
    public class DataPoolModule
    {
        private string _databasePath = @"C:\CProject\DataPool";

        //thông tin một pool
        public class PoolInfo
        {
            public double ID { get; set; }
            public string PoolName { get; set; } = string.Empty;
            public string PoolDescription { get; set; } = string.Empty;
            public string PoolCreateID { get; set; } = string.Empty;
            public string PoolNote { get; set; } = string.Empty;
            public string PoolCreatedBy { get; set; } = string.Empty;
            public string PoolCreateDatetime { get; set; } = string.Empty;

            public class PoolCount 
            {
                public int TotalCount { get; set; } = 0;
                public int UsedCount { get; set; } = 0;
                public int UnusedCount { get; set; } = 0;
                public int ErrorCount { get; set; } = 0;
                public PoolCount(int total, int used, int unused, int error)
                {
                    TotalCount = total;
                    UsedCount = used;
                    UnusedCount = unused;
                    ErrorCount = error;
                }
            }

            public PoolInfo(double id, string name, string description, string batchID, string createID, string note, string createdBy, string createDatetime)
            {
                ID = id;
                PoolName = name;
                PoolDescription = description;
                PoolCreateID = createID;
                PoolNote = note;
                PoolCreatedBy = createdBy;
                PoolCreateDatetime = createDatetime;
            }
        }

        //thông tin về mã của pool
        public class PoolCodeInfo
        {
            public double ID { get; set; }
            public string PoolCode { get; set; } = string.Empty;
            public int  PoolCodeStatus { get; set; } = 0;
            public string PoolCodeUsedBatchID { get; set; } = string.Empty;
            public string PoolCodeUsedDatetime { get; set; } = string.Empty;
            public string PoolCodeNote { get; set; } = string.Empty;
            public string PoolCodeCreateID { get; set; } = string.Empty;
            public string PoolCodeCreatedBy { get; set; } = string.Empty;
            public string PoolCodeCreateDatetime { get; set; } = string.Empty;

            public PoolCodeInfo(double id, string code, int status, string usedBatchID, string usedDatetime, string note, string createID, string createdBy, string createDatetime)
            {
                ID = id;
                PoolCode = code;
                PoolCodeStatus = status;
                PoolCodeUsedBatchID = usedBatchID;
                PoolCodeUsedDatetime = usedDatetime;
                PoolCodeNote = note;
            }
        }

        //Lấy đường dẫn pool theo tên pool
        public DataPoolResultString GetPoolPath(string poolName)
        {
            if (string.IsNullOrWhiteSpace(poolName))
            {
                return new DataPoolResultString(false, "Tên Pool không được trống.", string.Empty);
            }
            return new DataPoolResultString(true, "Success", Path.Combine(_databasePath, poolName + ".db"));
        }

        //tạo pool mới
        public DataPoolResultString CreatePool(PoolInfo poolInfo)
        {
            if (poolInfo == null)
            {
                return new DataPoolResultString(false, "Không hợp lệ, class PoolInfo là null", string.Empty);
            }
            if (string.IsNullOrWhiteSpace(poolInfo.PoolName))
            {
                return new DataPoolResultString(false, "Không hợp lệ, PoolName là rỗng", string.Empty);
            }
            string poolPath = GetPoolPath(poolInfo.PoolName).Data;
            if (File.Exists(poolPath))
            {
                return new DataPoolResultString(false, "Pool đã tồn tại", string.Empty);
            }
            if (!Directory.Exists(_databasePath))
            {
                Directory.CreateDirectory(_databasePath);
            }
            const string sql = @"
            CREATE TABLE IF NOT EXISTS Pool (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                PoolName TEXT NOT NULL UNIQUE,
                PoolDescription TEXT NOT NULL,
                PoolCreateID TEXT NOT NULL,
                PoolNote TEXT NOT NULL,
                PoolCreatedBy TEXT NOT NULL,
                PoolCreateDatetime TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Codes (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                PoolCode TEXT NOT NULL UNIQUE,
                Status INTEGER NOT NULL DEFAULT 0,
                PoolCodeUsedBatchID TEXT NOT NULL DEFAULT '',
                PoolCodeUsedDatetime TEXT NOT NULL,
                PoolCodeNote TEXT NOT NULL,
                PoolCodeCreateID TEXT NOT NULL,
                PoolCodeCreatedBy TEXT NOT NULL,
                PoolCodeCreateDatetime TEXT NOT NULL
            );

            -- Indexes
            CREATE INDEX IF NOT EXISTS IDX_Codes_Status ON Codes(Status);
            CREATE INDEX IF NOT EXISTS IDX_Codes_UsedBatchID ON Codes(PoolCodeUsedBatchID);
            CREATE INDEX IF NOT EXISTS IDX_Codes_CreateID ON Codes(PoolCodeCreateID);
            CREATE INDEX IF NOT EXISTS IDX_Codes_CreatedBy ON Codes(PoolCodeCreatedBy);
            CREATE INDEX IF NOT EXISTS IDX_Codes_CreateDatetime ON Codes(PoolCodeCreateDatetime);

            -- Composite indexes (khuyến nghị)
            CREATE INDEX IF NOT EXISTS IDX_Codes_Status_CreateDatetime ON Codes(Status, PoolCodeCreateDatetime);
            CREATE INDEX IF NOT EXISTS IDX_Codes_Batch_Status ON Codes(PoolCodeUsedBatchID, Status);
            PRAGMA journal_mode=WAL;
        ";

            using var con = new SqliteConnection($"Data Source={poolPath}");
            con.Open();
            using var cmd = new SqliteCommand(sql, con);
            cmd.ExecuteNonQuery();
            return new DataPoolResultString(true, "Pool created successfully", poolPath);
        }

        //Thêm mã vào pool 3 mode 0: nhập file, 1: gửi 1 cái, 2: gửi DataTable
        public DataPoolAddCodesResult AddCodes(
            string poolName,
            int mode,
            string? filePath,
            string? singleCode,
            DataTable? dataTable,
            string createID,
            string createdBy)
        {
            var result = new DataPoolAddCodesResult();

            if (string.IsNullOrWhiteSpace(poolName))
            {
                result.Message = "Tên Pool không được trống.";
                return result;
            }

            if (mode < 0 || mode > 2)
            {
                result.Message = "Mode không hợp lệ. Chỉ chấp nhận 0 (file), 1 (1 code), hoặc 2 (DataTable).";
                return result;
            }

            var poolPathResult = GetPoolPath(poolName);
            if (!poolPathResult.Success)
            {
                result.Message = poolPathResult.Message;
                return result;
            }

            string poolPath = poolPathResult.Data;
            if (!File.Exists(poolPath))
            {
                result.Message = "Pool không tồn tại.";
                return result;
            }

            List<string> codesToAdd = new();

            switch (mode)
            {
                case 0: // File CSV
                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        result.Message = "Đường dẫn file không được trống khi mode = 0.";
                        return result;
                    }
                    if (!File.Exists(filePath))
                    {
                        result.Message = $"File không tồn tại: {filePath}";
                        return result;
                    }
                    try
                    {
                        var lines = File.ReadAllLines(filePath);
                        foreach (var line in lines)
                        {
                            var trimmed = line.Trim();
                            if (!string.IsNullOrEmpty(trimmed))
                            {
                                codesToAdd.Add(trimmed);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Message = $"Lỗi khi đọc file: {ex.Message}";
                        return result;
                    }
                    break;

                case 1: // 1 code
                    if (string.IsNullOrWhiteSpace(singleCode))
                    {
                        result.Message = "Code không được trống khi mode = 1.";
                        return result;
                    }
                    codesToAdd.Add(singleCode.Trim());
                    break;

                case 2: // DataTable
                    if (dataTable == null || dataTable.Rows.Count == 0)
                    {
                        result.Message = "DataTable không hợp lệ hoặc không có dữ liệu khi mode = 2.";
                        return result;
                    }
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var code = row[0]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(code))
                        {
                            codesToAdd.Add(code);
                        }
                    }
                    break;
            }

            if (codesToAdd.Count == 0)
            {
                result.Message = "Không có code nào để thêm.";
                return result;
            }

            result.TotalCount = codesToAdd.Count;
            string createDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var con = new SqliteConnection($"Data Source={poolPath}");
            con.Open();
            using var transaction = con.BeginTransaction();

            try
            {
                foreach (var code in codesToAdd)
                {
                    if (string.IsNullOrWhiteSpace(code)) continue;

                    try
                    {
                        using var checkCmd = new SqliteCommand(
                            "SELECT COUNT(*) FROM Codes WHERE PoolCode = @code", con, transaction);
                        checkCmd.Parameters.AddWithValue("@code", code);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            result.DuplicateCount++;
                            continue;
                        }

                        using var insertCmd = new SqliteCommand(@"
                            INSERT INTO Codes (PoolCode, Status, PoolCodeCreateID, PoolCodeCreatedBy, PoolCodeCreateDatetime)
                            VALUES (@code, 0, @createID, @createdBy, @createDatetime)", con, transaction);
                        insertCmd.Parameters.AddWithValue("@code", code);
                        insertCmd.Parameters.AddWithValue("@createID", createID);
                        insertCmd.Parameters.AddWithValue("@createdBy", createdBy);
                        insertCmd.Parameters.AddWithValue("@createDatetime", createDatetime);
                        insertCmd.ExecuteNonQuery();
                        result.AddedCount++;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorCount++;
                        if (result.Errors.Count < 10)
                        {
                            result.Errors.Add($"Code '{code}': {ex.Message}");
                        }
                    }
                }

                transaction.Commit();
                result.Success = true;
                result.Message = $"Hoàn tất. Thêm mới: {result.AddedCount}, Trùng: {result.DuplicateCount}, Lỗi: {result.ErrorCount}";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.Success = false;
                result.Message = $"Lỗi transaction: {ex.Message}";
            }

            return result;
        }

        //Cập nhật trạng thái mã trong pool : Status = 0: chưa sử dụng, 1: đã sử dụng, -1: lỗi : Cập nhật theo PoolCode hoặc theo ID, nếu PoolCode và ID đều có thì ưu tiên PoolCode, nếu không có thì báo lỗi.

        //Lấy thông tin pool theo tên pool (trả về thông tin pool và Count số lượng mã code trong pool, số lượng mã code đã sử dụng, số lượng mã code chưa sử dụng, số lượng mã code lỗi).

        //Lấy mã code trong pool theo PoolCode hoặc ID, nếu PoolCode và ID đều có thì ưu tiên PoolCode, nếu không có thì báo lỗi.

        //Lấy danh sách mã code trong theo số lượng, trạng thái, ngày tạo, ngày sử dụng, batchID, người tạo, người sử dụng. Có phân trang lấy 100 Records 1 lần, nếu muốn lấy tiếp thì truyền pageIndex = 2, pageIndex = 3, ... Nếu không có dữ liệu thì trả về rỗng.

        //Lấy số đếm mã code : Tổng số, số lượng đã dùng.

        //Lấy toàn bộ code trong pool theo trạng thái. (trả về DataTable)

        //Lấy danh sách Pool trong thư mục databasePath, có phân trang lấy 100 Records 1 lần, nếu muốn lấy tiếp thì truyền pageIndex = 2, pageIndex = 3, ... Nếu không có dữ liệu thì trả về rỗng.
    }

    public class DataPoolAddCodesResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int AddedCount { get; set; }
        public int DuplicateCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new();

        public DataPoolAddCodesResult() { }

        public DataPoolAddCodesResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class DataPoolResultString
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DataPoolResultString (bool success, string message, string data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

}
