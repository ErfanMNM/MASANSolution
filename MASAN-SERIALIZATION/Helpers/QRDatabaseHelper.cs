using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASAN_SERIALIZATION.Helpers
{
    public static class QRDatabaseHelper
    {
        // Đường dẫn cố định như yêu cầu
        public const string DefaultDbPath = @"C:\MASAN\QRDatabase.db";

        // SQL tạo bảng
        private const string CREATE_TABLE_SQL = @"
        CREATE TABLE IF NOT EXISTS QRProducts (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            QRContent TEXT NOT NULL,
            BatchCode TEXT NOT NULL,
            Barcode TEXT NOT NULL,
            Status INTEGER NOT NULL,
            UserName TEXT NOT NULL,
            TimeStampActive TEXT NOT NULL,
            TimeUnixActive INTEGER NOT NULL,
            ProductionDatetime TEXT NOT NULL
        );
        CREATE INDEX IF NOT EXISTS IDX_QR_QRContent ON QRProducts(QRContent);
        CREATE INDEX IF NOT EXISTS IDX_QR_BatchCode ON QRProducts(BatchCode);
    ";

        /// <summary>
        /// Tạo bể CSDL nếu chưa có file + tạo bảng nếu chưa có.
        /// </summary>
        public static void EnsureDatabase(string dbPath = DefaultDbPath)
        {
            string folder = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (var con = new SQLiteConnection($"Data Source={dbPath}"))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(CREATE_TABLE_SQL, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Kiểm tra xem "bể" DB đã tồn tại chưa. 
        /// Nếu chưa, tạo mới ở C:\MASAN\QRDatabase.db.
        /// Trả về true nếu file đã tồn tại, false nếu vừa tạo mới.
        /// </summary>
        public static bool CheckAndCreateDatabaseForBatch(string batchCode, string dbPath = DefaultDbPath)
        {
            bool existed = File.Exists(dbPath);
            EnsureDatabase(dbPath);
            return existed;
        }

        /// <summary>
        /// Kiểm tra batch đã có dữ liệu trong bể chưa.
        /// </summary>
        public static bool BatchHasData(string batchCode, string dbPath = DefaultDbPath)
        {
            EnsureDatabase(dbPath);

            using (var con = new SQLiteConnection($"Data Source={dbPath}"))
            {
                con.Open();
                string sql = "SELECT COUNT(1) FROM QRProducts WHERE BatchCode = @BatchCode;";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Thêm mã vào DB ở trạng thái Active.
        /// Nếu QRContent đã tồn tại, có thể chọn: update lại hoặc bỏ qua (ở đây tao cho update).
        /// </summary>
        public static void AddOrActivateCode(
            string qrContent,
            string batchCode,
            string barcode,
            string userName,
            DateTime productionDateTime,
            string dbPath = DefaultDbPath)
        {
            EnsureDatabase(dbPath);

            long unixNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string timeStampNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string prodTime = productionDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            using (var con = new SQLiteConnection($"Data Source={dbPath}"))
            {
                con.Open();

                // Nếu đã có QRContent thì update, ngược lại insert
                string checkSql = "SELECT COUNT(1) FROM QRProducts WHERE QRContent = @QRContent;";
                using (var checkCmd = new SQLiteCommand(checkSql, con))
                {
                    checkCmd.Parameters.AddWithValue("@QRContent", qrContent);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        string updateSql = @"
                        UPDATE QRProducts
                        SET BatchCode = @BatchCode,
                            Barcode = @Barcode,
                            Status = 1,
                            UserName = @UserName,
                            TimeStampActive = @TimeStampActive,
                            TimeUnixActive = @TimeUnixActive,
                            ProductionDatetime = @ProductionDatetime
                        WHERE QRContent = @QRContent;
                    ";

                        using (var cmd = new SQLiteCommand(updateSql, con))
                        {
                            cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                            cmd.Parameters.AddWithValue("@Barcode", barcode);
                            cmd.Parameters.AddWithValue("@Status", 1);
                            cmd.Parameters.AddWithValue("@UserName", userName);
                            cmd.Parameters.AddWithValue("@TimeStampActive", timeStampNow);
                            cmd.Parameters.AddWithValue("@TimeUnixActive", unixNow);
                            cmd.Parameters.AddWithValue("@ProductionDatetime", prodTime);
                            cmd.Parameters.AddWithValue("@QRContent", qrContent);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string insertSql = @"
                        INSERT INTO QRProducts
                        (QRContent, BatchCode, Barcode, Status, UserName, TimeStampActive, TimeUnixActive, ProductionDatetime)
                        VALUES
                        (@QRContent, @BatchCode, @Barcode, 1, @UserName, @TimeStampActive, @TimeUnixActive, @ProductionDatetime);
                    ";

                        using (var cmd = new SQLiteCommand(insertSql, con))
                        {
                            cmd.Parameters.AddWithValue("@QRContent", qrContent);
                            cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                            cmd.Parameters.AddWithValue("@Barcode", barcode);
                            cmd.Parameters.AddWithValue("@UserName", userName);
                            cmd.Parameters.AddWithValue("@TimeStampActive", timeStampNow);
                            cmd.Parameters.AddWithValue("@TimeUnixActive", unixNow);
                            cmd.Parameters.AddWithValue("@ProductionDatetime", prodTime);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Hủy (deactive) một mã theo QRContent.
        /// Trả về true nếu có dòng bị ảnh hưởng.
        /// </summary>
        public static bool DeactivateCode(string qrContent, string userName, string dbPath = DefaultDbPath)
        {
            EnsureDatabase(dbPath);

            using (var con = new SQLiteConnection($"Data Source={dbPath}"))
            {
                con.Open();

                string sql = @"
                UPDATE QRProducts
                SET Status = 0,
                    UserName = @UserName
                WHERE QRContent = @QRContent;
            ";

                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@QRContent", qrContent);

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        /// <summary>
        /// Lấy thông tin 1 mã theo QRContent. Trả về null nếu không có.
        /// </summary>
        public static QRProductRecord GetByQRContent(string qrContent, string dbPath = DefaultDbPath)
        {
            EnsureDatabase(dbPath);

            using (var con = new SQLiteConnection($"Data Source={dbPath}"))
            {
                con.Open();

                string sql = @"
                SELECT ID, QRContent, BatchCode, Barcode, Status, UserName,
                       TimeStampActive, TimeUnixActive, ProductionDatetime
                FROM QRProducts
                WHERE QRContent = @QRContent
                LIMIT 1;
            ";

                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@QRContent", qrContent);

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                            return null;

                        return new QRProductRecord
                        {
                            ID = rd.GetInt32(0),
                            QRContent = rd.GetString(1),
                            BatchCode = rd.GetString(2),
                            Barcode = rd.GetString(3),
                            Status = rd.GetInt32(4),
                            UserName = rd.GetString(5),
                            TimeStampActive = rd.GetString(6),
                            TimeUnixActive = rd.GetInt64(7),
                            ProductionDatetime = rd.GetString(8)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Load dữ liệu vào Dictionary với key là QRContent.
        /// onlyActive = true -> chỉ lấy Status = 1.
        /// </summary>
        public static Dictionary<string, QRProductRecord> LoadToDictionary(
            bool onlyActive = false,
            string dbPath = DefaultDbPath)
        {
            EnsureDatabase(dbPath);

            var dict = new Dictionary<string, QRProductRecord>(StringComparer.OrdinalIgnoreCase);

            using (var con = new SQLiteConnection($"Data Source={dbPath}"))
            {
                con.Open();

                string sql = @"
                SELECT ID, QRContent, BatchCode, Barcode, Status, UserName,
                       TimeStampActive, TimeUnixActive, ProductionDatetime
                FROM QRProducts
            ";

                if (onlyActive)
                    sql += " WHERE Status = 1;";

                using (var cmd = new SQLiteCommand(sql, con))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var rec = new QRProductRecord
                        {
                            ID = rd.GetInt32(0),
                            QRContent = rd.GetString(1),
                            BatchCode = rd.GetString(2),
                            Barcode = rd.GetString(3),
                            Status = rd.GetInt32(4),
                            UserName = rd.GetString(5),
                            TimeStampActive = rd.GetString(6),
                            TimeUnixActive = rd.GetInt64(7),
                            ProductionDatetime = rd.GetString(8)
                        };

                        dict[rec.QRContent] = rec;
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Lấy số dòng trong bảng.
        /// Nếu batchCode != null -> đếm theo batch.
        /// </summary>
        public static int GetRowCount(string batchCode = null, string dbPath = DefaultDbPath)
        {
            EnsureDatabase(dbPath);

            using (var con = new SQLiteConnection($"Data Source={dbPath}"))
            {
                con.Open();

                string sql;
                if (string.IsNullOrEmpty(batchCode))
                    sql = "SELECT COUNT(*) FROM QRProducts;";
                else
                    sql = "SELECT COUNT(*) FROM QRProducts WHERE BatchCode = @BatchCode;";

                using (var cmd = new SQLiteCommand(sql, con))
                {
                    if (!string.IsNullOrEmpty(batchCode))
                        cmd.Parameters.AddWithValue("@BatchCode", batchCode);

                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        /// <summary>
        /// Sao lưu file DB hiện tại, rồi tạo file mới (trống) lại từ đầu.
        /// Trả về đường dẫn file backup.
        /// </summary>
        public static string BackupAndRecreate(string dbPath = DefaultDbPath)
        {
            EnsureDatabase(dbPath); // để chắc chắn file + table tồn tại

            if (!File.Exists(dbPath))
                throw new FileNotFoundException("Không tìm thấy file DB để backup.", dbPath);

            string folder = Path.GetDirectoryName(dbPath);
            string fileNameNoExt = Path.GetFileNameWithoutExtension(dbPath);
            string ext = Path.GetExtension(dbPath);

            string time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupPath = Path.Combine(folder, $"{fileNameNoExt}_{time}.bak{ext}");

            // Sao lưu
            File.Copy(dbPath, backupPath, overwrite: false);

            // Xóa file cũ
            File.Delete(dbPath);

            // Tạo lại bể mới tinh
            EnsureDatabase(dbPath);

            return backupPath;
        }
    }
}
