using HslCommunication;
using Newtonsoft.Json;
using QR_MASAN_01.Utils;
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

        public bool isOpen { get; set; } = false;
        public bool isLoading { get; set; } = false;

        public PLCSetting()
        {
            InitializeComponent();
        }

        private void uiTableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public void INIT()
        {
            isLoading = true;
            // Kiểm tra và tạo các file cần thiết
            FirstCheck();
            //kết nối PLC
            omronPLC_Hsl1.PLC_IP = PLCAddress.Get("PLC_IP");
            omronPLC_Hsl1.PLC_PORT = int.Parse(PLCAddress.Get("PLC_PORT").ToString());
            omronPLC_Hsl1.InitPLC();

            StartTask();
            UpdateCBB();

            isLoading = false;

        }

        public void FirstCheck()
        {
            //kiểm tra xem có thư mục PLC_RECIPEs hay không
            if (!Directory.Exists("PLC_RECIPEs"))
            {
                //nếu không có thì tạo mới
                Directory.CreateDirectory("PLC_RECIPEs");
            }
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
                    RejectStreng = valueR[2]
                };
            }
            else
            {
                SelectRecipeName = "Default.rplc";
                defaultConfig = new PLC_Parameter
                {
                    DelayCamera = "1000",
                    DelayReject = "2000",
                    RejectStreng = "20",
                };
            }

            //kiểm tra trong danh sách file có Recipe đó hay không, nếu không thì tạo mới

            Check_Recipe(SelectRecipeName, defaultConfig);
            
        }

        //lấy parameter từ PLC
        public void GetParameterFromPLC()
        {
            OperateResult<int[]> read = omronPLC_Hsl1.plc.ReadInt32("D0", 3);
            if (read.IsSuccess)
            {
                PLC_Parameter_On_PLC.DelayCamera = read.Content[0].ToString();
                PLC_Parameter_On_PLC.DelayReject = read.Content[1].ToString();
                PLC_Parameter_On_PLC.RejectStreng = read.Content[2].ToString();

            }
            else
            {
                PLC_Parameter_On_PLC.DelayCamera = "-1";
                PLC_Parameter_On_PLC.DelayReject = "-1";
                PLC_Parameter_On_PLC.RejectStreng = "-1";
            }

        }


        #region Database
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

        public void Write_Recipe_To_File (string json)
        {
            //ghi dữ liệu vào file
            File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName}", json);
            //ghi log
            AddLogRecipe(SelectRecipeName, json, "UPDATE", "Operator");
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
        public void Check_Recipe (string recipeName, PLC_Parameter configs)
            {
            //kiểm tra trong danh sách file có Recipe đó hay không, nếu không thì tạo mới

            string[] files = Directory.GetFiles("PLC_RECIPEs", "*.rplc");

            if (files.Length == 0)
            {
                string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName}", json);
                //thêm logs mặc định
                AddLogRecipe(SelectRecipeName, $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.RejectStreng}", "CREATE", "Operator");
                AddLogRecipe(SelectRecipeName, $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.RejectStreng}", "SELECT", "Operator");
                PLC_Parameter_On_PC = defaultConfig;
            }
            else
            {
                bool fileExists = files.Any(file => Path.GetFileName(file).Equals(SelectRecipeName, StringComparison.OrdinalIgnoreCase));

                if (!fileExists)
                {
                    string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                    File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName}", json);
                    //thêm logs mặc định
                    AddLogRecipe(SelectRecipeName, $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.RejectStreng}", "CREATE", "Operator");
                    AddLogRecipe(SelectRecipeName, $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.RejectStreng}", "SELECT", "Operator");

                    PLC_Parameter_On_PC = defaultConfig;
                }
                else
                {
                    //lấy dữ liệu từ file
                    string jsonContent = File.ReadAllText($"PLC_RECIPEs/{SelectRecipeName}");

                    // Chuyển đổi JSON thành object
                    PLC_Parameter_On_PC = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);
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
        public DataTable Get_Last_Select_Recipe()
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
        #endregion

        public void UpdateCBB()
        {
            // Lấy danh sách file .rplc và thêm vào ComboBox
            foreach (var file in Directory.GetFiles("PLC_RECIPEs", "*.rplc"))
            {
                ipRecipe.Items.Add(Path.GetFileName(file));
            }

            // Nếu không có file nào, thêm giá trị mặc định
            if (ipRecipe.Items.Count == 0)
            {
                ipRecipe.Items.Add("Default.rplc");
            }
            // Đặt giá trị mặc định cho ComboBox
            ipRecipe.SelectedIndex = 0;


            // Chọn giá trị mặc định nếu tồn tại
            if (ipRecipe.Items.Contains(SelectRecipeName))
            {
                ipRecipe.SelectedItem = SelectRecipeName;
            }
            else
            {
                ipRecipe.SelectedItem = "Default.rplc"; // Chọn giá trị mặc định nếu không tìm thấy
            }
        }

        public class PLC_Parameter
        {
            public string DelayCamera { get; set; } = "0";
            public string DelayReject { get; set; } = "0";
            public string RejectStreng { get; set; } = "0";
        }

        private async Task PLC_Comfirm_Async()
        {

            // Bắt đầu nhiệm vụ
            try
            {
                while (!GTask.Task_PLC_Comfirm.Token.IsCancellationRequested)
                {
                    if (isOpen)
                    {
                        GetParameterFromPLC();

                        this.InvokeIfRequired(() =>
                        {
                            // Cập nhật giao diện người dùng với giá trị từ PLC
                            opDelayTriger.Text = PLC_Parameter_On_PLC.DelayCamera;
                            opDelayReject.Text = PLC_Parameter_On_PLC.DelayReject;
                            opRejectStreng.Text = PLC_Parameter_On_PLC.RejectStreng;
                        });
                    }

                    await Task.Delay(1000, GTask.Task_PLC_Comfirm.Token);
                }
            }
            catch (TaskCanceledException) { }
        }

        private void StartTask()
        {
            Task.Run(PLC_Comfirm_Async, GTask.Task_PLC_Comfirm.Token);
        }



        private void PLCSetting_Initialize(object sender, EventArgs e)
        {
            //ghi log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("O"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Open PLC Setting",Globalvariable.CurrentUser.Username, "Người dùng mở bảng PLC setting");
            SystemLogs.LogQueue.Enqueue(systemLogs);

            isOpen = true;
        }   
        private void PLCSetting_Finalize(object sender, EventArgs e)
        {
            // ghi log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("O"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Close PLC Setting", Globalvariable.CurrentUser.Username, "Người dùng đóng bảng PLC setting");
            SystemLogs.LogQueue.Enqueue(systemLogs);
            isOpen = false;
        }

        private void ipRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectRecipeName.Length > 3)
            {
                //lấy dữ liệu từ file
                string jsonContent = File.ReadAllText($"PLC_RECIPEs/{SelectRecipeName}");

                // Chuyển đổi JSON thành object
                PLC_Parameter az = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);

                ipDelayTriger.Text = az.DelayCamera;
                ipDelayReject.Text = az.DelayReject;
                ipRejectStreng.Text = az.RejectStreng;
            }

            if (isLoading)
                return;
            // Lấy tên công thức đã chọn
            SelectRecipeName = ipRecipe.SelectedItem.ToString(); 
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //ghi log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("O"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Update PLC Parameter", Globalvariable.CurrentUser.Username, "Người dùng cập nhật tham số PLC");
            SystemLogs.LogQueue.Enqueue(systemLogs);
            //tạo task write to PLC
            Task.Run(() =>
            {
                try
                {
                    // Lấy giá trị từ các trường nhập liệu
                    string delayCamera = ipDelayTriger.Text;
                    string delayReject = ipDelayReject.Text;
                    string rejectStreng = ipRejectStreng.Text;
                    // Cập nhật giá trị vào PLC_Parameter_On_PC
                    PLC_Parameter_On_PC.DelayCamera = delayCamera;
                    PLC_Parameter_On_PC.DelayReject = delayReject;
                    PLC_Parameter_On_PC.RejectStreng = rejectStreng;
                    // Chuyển đổi thành JSON
                    string json = JsonConvert.SerializeObject(PLC_Parameter_On_PC, Formatting.Indented);
                    // Ghi vào file
                    Write_Recipe_To_File(json);
                    // Ghi vào PLC
                    OperateResult operateResult =  omronPLC_Hsl1.plc.Write("D0", new int[] { int.Parse(delayCamera), int.Parse(delayReject), int.Parse(rejectStreng) });
                    if (!operateResult.IsSuccess)
                    {
                        this.ShowErrorNotifier($"Lỗi khi ghi vào PLC: {operateResult.Message}");
                        return;
                    }
                    // Ghi log
                    AddLogRecipe(SelectRecipeName, $"{delayCamera},{delayReject},{rejectStreng}", "UPDATE", Globalvariable.CurrentUser.Username);
                    this.ShowSuccessNotifier("Cập nhật thành công!");

                }
                catch (Exception ex)
                {
                    this.ShowErrorNotifier($"Lỗi khi cập nhật: {ex.Message}");
                }
            });
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            //ghi log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("O"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Undo PLC Parameter", Globalvariable.CurrentUser.Username, "Người dùng hoàn tác tham số PLC");
            SystemLogs.LogQueue.Enqueue(systemLogs);
            //cập nhật PLC parameter từ PLC
            ipDelayReject.Text = PLC_Parameter_On_PLC.DelayReject;
            ipDelayTriger.Text = PLC_Parameter_On_PLC.DelayCamera;
            ipRejectStreng.Text = PLC_Parameter_On_PLC.RejectStreng;
            //cập nhật PLC parameter trên PC
        }
    }
}
