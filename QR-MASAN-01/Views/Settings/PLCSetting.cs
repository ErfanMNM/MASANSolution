using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QR_MASAN_01.Views.Settings
{
    public partial class PLCSetting : UIPage
    {
        public string SelectRecipeName = string.Empty;
        public string log_FilePath = "PLC_RECIPEs/log.rlplc";
        public string defaultFilePath = "PLC_RECIPEs/Default.rplc";
        public static PLC_Parameter PLC_Parameter_On_PC { get; set; } = new PLC_Parameter();
        public static PLC_Parameter PLC_Parameter_On_PLC { get; set; } = new PLC_Parameter();

        public PLCSetting()
        {
            InitializeComponent();
        }

        private void uiTableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public void FirstCheck()
        {
            //kiểm tra xem có file log hay không
            if (!File.Exists(log_FilePath))
            {
                //nếu không có thì tạo mới
                SQLiteConnection.CreateFile(log_FilePath);
                CreateLogTable();
            }

            //kiểm tra xem có file Default.rplc hay không
            if (!File.Exists(defaultFilePath))
            {
                //nếu không có thì tạo mới
                SQLiteConnection.CreateFile(defaultFilePath);
                CreateDefaultRecipeTable();
                return;
            }

            //lấy dòng cuôi cùng trong file log
            DataTable datatable = Get_Last_Select_Recipe();
            PLC_Parameter defaultConfig;
            if (datatable.Rows.Count >= 1)
            {
                SelectRecipeName = datatable.Rows[0]["RecipeName"].ToString();
                string[] valueR = datatable.Rows[0]["RecipeValue"].ToString().Split(",");
                defaultConfig = new PLC_Parameter
                {
                    DelayCamera = valueR[0],
                    DelayReject = valueR[1],
                    ProcessTimeOut = valueR[2],
                    RejectStreng = valueR[3],
                    DeboundSensor = valueR[4]
                };
            }
            else
            {
                SelectRecipeName = "Default.rplc";
                defaultConfig = new PLC_Parameter
                {
                    DelayCamera = "1000",
                    DelayReject = "2000",
                    ProcessTimeOut = "100",
                    RejectStreng = "20",
                    DeboundSensor = "231"
                };
            }

        }

        PLC_Parameter defaultConfig = new PLC_Parameter
        {
            DelayCamera = "1000",
            DelayReject = "2000",
            RejectStreng = "20",
        };

        public void CreateDefaultRecipeTable()
        {
            
            string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
            File.WriteAllText(defaultFilePath, json);
            SelectRecipeName = "Default.rplc";
            string RecipeValue = $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.RejectStreng}";
            //thêm logs mặc định
            AddLogRecipe("Default.rplc", RecipeValue, "CREATE", "Operator");
            AddLogRecipe("Default.rplc", RecipeValue, "SELECT", "Operator");

            PLC_Parameter_On_PC = defaultConfig;
        }
        public void CreateLogTable()
        {
            using (var conn = new SQLiteConnection("Data Source=PLC_RECIPEs/log.rlplc;Version=3;"))
            {
                conn.Open();
                string createTableQuery = @"CREATE TABLE ""Log"" (
	                                        ""ID""	INTEGER NOT NULL UNIQUE,
	                                        ""RecipeName""	TEXT,
                                            ""RecipeValue""	TEXT,
	                                        ""Action""	TEXT,
	                                        ""Timestamp""	TEXT,
	                                        ""UserName""	TEXT,
	                                        PRIMARY KEY(""ID"" AUTOINCREMENT)
                                            )";
                using (var cmd = new SQLiteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddLogRecipe(string recipeName, string recipeValue, string action, string userName)
        {
            using (var conn = new SQLiteConnection($"Data Source={log_FilePath};Version=3;"))
            {
                conn.Open();
                string insertQuery = @"INSERT INTO Log (RecipeName, RecipeValue, Action, Timestamp, UserName) 
                                   VALUES (@RecipeName, @RecipeValue, @Action, @Timestamp, @UserName)";

                using (var cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@RecipeName", recipeName);
                    cmd.Parameters.AddWithValue("@RecipeValue", recipeValue);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@UserName", userName);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public string Get_Last_Select_Recipe()
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=PLC_RECIPEs/log.rlplc;Version=3;"))
            {
                connection.Open();
                string query = $"SELECT * FROM Log WHERE Action = 'SELECT' ORDER BY `ID` DESC LIMIT 1;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public class PLC_Parameter
        {
            public string DelayCamera { get; set; } = "0";
            public string DelayReject { get; set; } = "0";
            public string RejectStreng { get; set; } = "0";
        }
    }
}
