using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;  
using System.IO;

namespace MASAN_SERIALIZATION.Helpers
{
    public class DatabaseHelper
    {
        private const string PODatabasePath = @"C:\MasanSerialization\Server_Service\data";
        private const string PODatabasesFolder = @"C:\MasanSerialization\PODatabases\";

        public class POInfo
        {
            public int Id { get; set; }
            public string OrderNo { get; set; }
            public string Site { get; set; }
            public string Factory { get; set; }
            public string ProductionLine { get; set; }
            public string ProductionDate { get; set; }
            public string Shift { get; set; }
            public int OrderQty { get; set; }
            public string LotNumber { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string GTIN { get; set; }
            public string CustomerOrderNo { get; set; }
            public string Uom { get; set; }
            public string LastUpdated { get; set; }
        }

        public class UniqueCode
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string CartonCode { get; set; }
            public int Status { get; set; }
            public string ActivateDate { get; set; }
            public string ProductionDate { get; set; }
            public string ActivateUser { get; set; }
            public string SubCamera_ActivateDate { get; set; }
            public string Send_Status { get; set; }
            public string Recive_Status { get; set; }
            public string Send_Recive_Logs { get; set; }
            public string Duplicate { get; set; }
            public string OrderNo { get; set; }
        }

        public List<POInfo> GetAllPOInfo()
        {
            var poList = new List<POInfo>();
            string folderPath = PODatabasePath;

            try
            {
                if (!Directory.Exists(PODatabasePath))
                {
                    throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
                }

                // Lấy tất cả file .json trong folder
                foreach (var file in Directory.GetFiles(folderPath, "*.json"))
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(file);
                        var po = JsonConvert.DeserializeObject<POInfo>(jsonContent);
                        if (po != null)
                        {
                            poList.Add(po);
                        }
                    }
                    catch (Exception innerEx)
                    {
                        throw new Exception($"Error parsing file {file}: {innerEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading POInfo: {ex.Message}");
            }

            return poList;
        }

        public List<UniqueCode> SearchCodeInAllPODatabases(string searchCode)
        {
            var resultList = new List<UniqueCode>();
            var poList = GetAllPOInfo();

            foreach (var po in poList)
            {
                if (string.IsNullOrEmpty(po.OrderNo)) continue;

                string dbPath = Path.Combine(PODatabasesFolder, $"{po.OrderNo}.db");
                if (!File.Exists(dbPath)) continue;

                try
                {
                    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();
                        string query = "SELECT * FROM UniqueCodes WHERE Code = @searchCode";
                        
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@searchCode", searchCode);
                            
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    resultList.Add(new UniqueCode
                                    {
                                        ID = Convert.ToInt32(reader["ID"]),
                                        Code = reader["Code"]?.ToString(),
                                        CartonCode = reader["cartonCode"]?.ToString(),
                                        Status = reader["Status"] != DBNull.Value ? Convert.ToInt32(reader["Status"]) : 0,
                                        ActivateDate = reader["ActivateDate"]?.ToString(),
                                        ProductionDate = reader["ProductionDate"]?.ToString(),
                                        ActivateUser = reader["ActivateUser"]?.ToString(),
                                        SubCamera_ActivateDate = reader["SubCamera_ActivateDate"]?.ToString(),
                                        Send_Status = reader["Send_Status"]?.ToString(),
                                        Recive_Status = reader["Recive_Status"]?.ToString(),
                                        Send_Recive_Logs = reader["Send_Recive_Logs"]?.ToString(),
                                        Duplicate = reader["Duplicate"]?.ToString(),
                                        OrderNo = po.OrderNo
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue searching other databases
                    System.Diagnostics.Debug.WriteLine($"Error searching in {dbPath}: {ex.Message}");
                }
            }

            return resultList;
        }

        public List<UniqueCode> SearchCodeWithWildcard(string searchPattern)
        {
            var resultList = new List<UniqueCode>();
            var poList = GetAllPOInfo();

            foreach (var po in poList)
            {
                if (string.IsNullOrEmpty(po.OrderNo)) continue;

                string dbPath = Path.Combine(PODatabasesFolder, $"{po.OrderNo}.db");
                if (!File.Exists(dbPath)) continue;

                try
                {
                    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();
                        string query = "SELECT * FROM UniqueCodes WHERE Code LIKE @searchPattern LIMIT 100";
                        
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@searchPattern", $"%{searchPattern}%");
                            
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    resultList.Add(new UniqueCode
                                    {
                                        ID = Convert.ToInt32(reader["ID"]),
                                        Code = reader["Code"]?.ToString(),
                                        CartonCode = reader["cartonCode"]?.ToString(),
                                        Status = reader["Status"] != DBNull.Value ? Convert.ToInt32(reader["Status"]) : 0,
                                        ActivateDate = reader["ActivateDate"]?.ToString(),
                                        ProductionDate = reader["ProductionDate"]?.ToString(),
                                        ActivateUser = reader["ActivateUser"]?.ToString(),
                                        SubCamera_ActivateDate = reader["SubCamera_ActivateDate"]?.ToString(),
                                        Send_Status = reader["Send_Status"]?.ToString(),
                                        Recive_Status = reader["Recive_Status"]?.ToString(),
                                        Send_Recive_Logs = reader["Send_Recive_Logs"]?.ToString(),
                                        Duplicate = reader["Duplicate"]?.ToString(),
                                        OrderNo = po.OrderNo
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error searching in {dbPath}: {ex.Message}");
                }
            }

            return resultList;
        }

        public POInfo GetPOInfoByOrderNo(string orderNo)
        {
            try
            {
                if (!File.Exists(PODatabasePath))
                {
                    throw new FileNotFoundException($"Database not found: {PODatabasePath}");
                }

                using (var connection = new SQLiteConnection($"Data Source={PODatabasePath};Version=3;"))
                {
                    connection.Open();
                    string query = "SELECT * FROM POInfo WHERE orderNo = @orderNo";
                    
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new POInfo
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    OrderNo = reader["orderNo"]?.ToString(),
                                    Site = reader["site"]?.ToString(),
                                    Factory = reader["factory"]?.ToString(),
                                    ProductionLine = reader["productionLine"]?.ToString(),
                                    ProductionDate = reader["productionDate"]?.ToString(),
                                    Shift = reader["shift"]?.ToString(),
                                    OrderQty = reader["orderQty"] != DBNull.Value ? Convert.ToInt32(reader["orderQty"]) : 0,
                                    LotNumber = reader["lotNumber"]?.ToString(),
                                    ProductCode = reader["productCode"]?.ToString(),
                                    ProductName = reader["productName"]?.ToString(),
                                    GTIN = reader["GTIN"]?.ToString(),
                                    CustomerOrderNo = reader["customerOrderNo"]?.ToString(),
                                    Uom = reader["uom"]?.ToString(),
                                    LastUpdated = reader["lastUpdated"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading POInfo by OrderNo: {ex.Message}");
            }

            return null;
        }
    }
}