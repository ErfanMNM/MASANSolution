using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpT.Logs
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Threading.Tasks;

    public class LogHelper<TAction> where TAction : Enum
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public LogHelper(string dbPath)
        {
            _dbPath = dbPath;
            _connectionString = $"Data Source={_dbPath};Version=3;";
            Init();
        }

        private void Init()
        {
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Logs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TimeISO TEXT,
                TimestampMs INTEGER,
                User TEXT,
                Action TEXT,
                Description TEXT,
                JsonParams TEXT
            );";
                cmd.ExecuteNonQuery();
            }
        }

        public async Task WriteLogAsync(string user, TAction action, string description, string jsonParams = "")
        {
            await Task.Run(() =>
            {
                var now = DateTime.UtcNow;
                var timestampMs = (long)(now - new DateTime(1970, 1, 1)).TotalMilliseconds;
                var timeISO = now.ToString("o");

                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                INSERT INTO Logs (TimeISO, TimestampMs, User, Action, Description, JsonParams)
                VALUES (@TimeISO, @TimestampMs, @User, @Action, @Description, @JsonParams);";
                    cmd.Parameters.AddWithValue("@TimeISO", timeISO);
                    cmd.Parameters.AddWithValue("@TimestampMs", timestampMs);
                    cmd.Parameters.AddWithValue("@User", user);
                    cmd.Parameters.AddWithValue("@Action", action.ToString());
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@JsonParams", jsonParams ?? "");
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public List<LogEntry<TAction>> GetLogs(int limit = 50, int offset = 0)
        {
            var logs = new List<LogEntry<TAction>>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM Logs ORDER BY Id DESC LIMIT @Limit OFFSET @Offset;";
                cmd.Parameters.AddWithValue("@Limit", limit);
                cmd.Parameters.AddWithValue("@Offset", offset);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(ReadLogEntry(reader));
                    }
                }
            }
            return logs;
        }

        public List<LogEntry<TAction>> GetLogsByTime(DateTime from, DateTime to)
        {
            var logs = new List<LogEntry<TAction>>();
            var fromMs = (long)(from.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
            var toMs = (long)(to.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM Logs WHERE TimestampMs BETWEEN @From AND @To ORDER BY Id DESC;";
                cmd.Parameters.AddWithValue("@From", fromMs);
                cmd.Parameters.AddWithValue("@To", toMs);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(ReadLogEntry(reader));
                    }
                }
            }
            return logs;
        }

        public List<LogEntry<TAction>> GetLogsByAction(TAction action)
        {
            var logs = new List<LogEntry<TAction>>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM Logs WHERE Action = @Action ORDER BY Id DESC;";
                cmd.Parameters.AddWithValue("@Action", action.ToString());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(ReadLogEntry(reader));
                    }
                }
            }
            return logs;
        }

        public List<LogEntry<TAction>> GetLogsByUser(string user)
        {
            var logs = new List<LogEntry<TAction>>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM Logs WHERE User = @User ORDER BY Id DESC;";
                cmd.Parameters.AddWithValue("@User", user);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(ReadLogEntry(reader));
                    }
                }
            }
            return logs;
        }

        private LogEntry<TAction> ReadLogEntry(SQLiteDataReader reader)
        {
            TAction action = default;
            try
            {
                action = (TAction)Enum.Parse(typeof(TAction), reader["Action"].ToString());
            }
            catch
            {
                // fallback nếu parse fail, giữ default
            }

            return new LogEntry<TAction>
            {
                Id = Convert.ToInt32(reader["Id"]),
                TimeISO = reader["TimeISO"].ToString(),
                TimestampMs = Convert.ToInt64(reader["TimestampMs"]),
                User = reader["User"].ToString(),
                Action = action,
                Description = reader["Description"].ToString(),
                JsonParams = reader["JsonParams"].ToString()
            };
        }
    }

    public class LogEntry<TAction> where TAction : Enum
    {
        public int Id { get; set; }
        public string TimeISO { get; set; }
        public long TimestampMs { get; set; }
        public string User { get; set; }
        public TAction Action { get; set; }
        public string Description { get; set; }
        public string JsonParams { get; set; }
    }


}
