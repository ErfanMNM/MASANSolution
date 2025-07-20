using Diaglogs;
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
        

        public PLCSetting()
        {
            InitializeComponent();
        }
        public string SelectRecipeName = string.Empty;
        public string log_FilePath = "PLC_RECIPEs/log.rlplc";
        public string defaultFilePath = "PLC_RECIPEs/Default.rplc";
        public static PLC_Parameter PLC_Parameter_On_PC { get; set; } = new PLC_Parameter();
        public static PLC_Parameter PLC_Parameter_On_PLC { get; set; } = new PLC_Parameter();

        public bool isOpen { get; set; } = false;
        public bool isLoading { get; set; } = false;

        public string SelectRecipeName_CS = string.Empty;
        public string log_FilePath_CS = "PLC_RECIPEs_CS/log.rlplc";
        public string defaultFilePath_CS = "PLC_RECIPEs_CS/Default.rplc";
        public static PLC_Parameter PLC_Parameter_On_PC_CS { get; set; } = new PLC_Parameter();
        public static PLC_Parameter PLC_Parameter_On_PLC_CS { get; set; } = new PLC_Parameter();
        public void INIT()
        {
            isLoading = true;
            // Kiểm tra và tạo các file cần thiết
            FirstCheck();
            FirstCheck_CS();
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
                CreateLogTable(log_FilePath);
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

        public void FirstCheck_CS()
        {
            //kiểm tra xem có thư mục PLC_RECIPEs hay không
            if (!Directory.Exists("PLC_RECIPEs_CS"))
            {
                //nếu không có thì tạo mới
                Directory.CreateDirectory("PLC_RECIPEs_CS");
            }
            //kiểm tra xem có file log hay không
            if (!File.Exists(log_FilePath_CS))
            {
                //nếu không có thì tạo mới
                SQLiteConnection.CreateFile(log_FilePath_CS);
                CreateLogTable(log_FilePath_CS);
            }

            //kiểm tra xem có file Default.rplc hay không
            if (!File.Exists(defaultFilePath_CS))
            {
                //nếu không có thì tạo mới
                SQLiteConnection.CreateFile(defaultFilePath_CS);
                CreateDefaultRecipeTable_CS();
                return;
            }

            //lấy dòng cuôi cùng trong file log
            DataTable datatable = Get_Last_Select_Recipe_CS();
            PLC_Parameter defaultConfig_CS;
            if (datatable.Rows.Count >= 1)
            {
                SelectRecipeName_CS = datatable.Rows[0]["RecipeName"].ToString();
                string[] valueR = datatable.Rows[0]["RecipeValue"].ToString().Split(",");
                defaultConfig_CS = new PLC_Parameter
                {
                    DelayCamera = valueR[0],
                    DelayReject = valueR[1],
                    RejectStreng = valueR[2]
                };
            }
            else
            {
                SelectRecipeName_CS = "Default.rplc";
                defaultConfig_CS = new PLC_Parameter
                {
                    DelayCamera = "1000",
                    DelayReject = "2000",
                    RejectStreng = "20",
                };
            }

            //kiểm tra trong danh sách file có Recipe đó hay không, nếu không thì tạo mới

            Check_Recipe_CS(SelectRecipeName_CS, defaultConfig_CS);

        }

        #region Camera Trước

        private void uiTableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        

        //lấy parameter từ PLC
        public void GetParameterFromPLC()
        {
            OperateResult<int[]> read = omronPLC_Hsl1.plc.ReadInt32(PLCAddress.Get("PLC_Delay_Camera_DM_C2"), 3);
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
        public void CreateLogTable( string Camera_Folder)
        {
            using (var conn = new SQLiteConnection($"Data Source={Camera_Folder};Version=3;"))
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

            //cập nhật lên màn hình
            this.InvokeIfRequired(() =>
            {
                ipDelayTriger.Text = PLC_Parameter_On_PC.DelayCamera;
                ipDelayReject.Text = PLC_Parameter_On_PC.DelayReject;
                ipRejectStreng.Text = PLC_Parameter_On_PC.RejectStreng;
            });
            

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

            // Lấy danh sách file .rplc và thêm vào ComboBox
            foreach (var file in Directory.GetFiles("PLC_RECIPEs_CS", "*.rplc"))
            {
                ipRecipe_CS.Items.Add(Path.GetFileName(file));
            }

            // Nếu không có file nào, thêm giá trị mặc định
            if (ipRecipe_CS.Items.Count == 0)
            {
                ipRecipe_CS.Items.Add("Default.rplc");
            }
            // Đặt giá trị mặc định cho ComboBox
            ipRecipe_CS.SelectedIndex = 0;


            // Chọn giá trị mặc định nếu tồn tại
            if (ipRecipe_CS.Items.Contains(SelectRecipeName_CS))
            {
                ipRecipe_CS.SelectedItem = SelectRecipeName_CS;
            }
            else
            {
                ipRecipe_CS.SelectedItem = "Default.rplc"; // Chọn giá trị mặc định nếu không tìm thấy
            }
        }

        public class PLC_Parameter
        {
            public string DelayCamera { get; set; } = "0";
            public string DelayReject { get; set; } = "0";
            public string RejectStreng { get; set; } = "0";
        }

        
        private void PLCSetting_Initialize(object sender, EventArgs e)
        {
            //ghi log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("O"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Open PLC Setting",Globalvariable.CurrentUser.Username, "Người dùng mở bảng PLC setting");
            SystemLogs.LogQueue.Enqueue(systemLogs);

            isOpen = true;

            Uri uri = new Uri($"http://{Setting.Current.IP_Camera_02}:{Setting.Current.Port_Camera_02}/monitor");
            Uri uri1 = new Uri($"https://google.com");
            webView21.Source = uri1;

            Uri uri2 = new Uri($"http://{Setting.Current.IP_Camera_01}:{Setting.Current.Port_Camera_01}/monitor");
            Uri uri21 = new Uri($"https://google.com");
            webView22.Source = uri2;
        }   
        private void PLCSetting_Finalize(object sender, EventArgs e)
        {
            // ghi log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("O"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Close PLC Setting", Globalvariable.CurrentUser.Username, "Người dùng đóng bảng PLC setting");
            SystemLogs.LogQueue.Enqueue(systemLogs);
            isOpen = false;
            webView21.Source = new Uri("https://google.com");
            webView22.Source = new Uri("https://google.com");
        }


        #region Event Handlers
        private void ipRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ipRecipe.SelectedText.Length > 3)
            {
                // Lấy tên công thức đã chọn
                SelectRecipeName = ipRecipe.SelectedText;
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
                    OperateResult operateResult =  omronPLC_Hsl1.plc.Write(PLCAddress.Get("PLC_Delay_Camera_DM_C2"), new int[] { int.Parse(delayCamera), int.Parse(delayReject), int.Parse(rejectStreng) });
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

            this.ShowSuccessNotifier("Cập nhật thành công!");
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

        private void btnNewRecipe_Click(object sender, EventArgs e)
        {
            using (Entertext enterText = new Entertext())
            {
                enterText.TileText = "Nhập tên Recipe";
                enterText.TextValue = "New Recipe";
                enterText.EnterClicked += (s, args) =>
                {
                    //ipUserName.Text = enterText.TextValue;

                    //kiểm tra xem tên Recipe đã tồn tại hay chưa
                    if (File.Exists($"PLC_RECIPEs/{enterText.TextValue}.rplc"))
                    {
                        this.ShowErrorNotifier("Tên Recipe đã tồn tại, vui lòng chọn tên khác.");
                        return;
                    }
                    //nếu chưa tồn tại thì tạo mới gán PC value vào Recipe

                    PLC_Parameter newRecipe = new PLC_Parameter
                    {
                        DelayCamera = PLC_Parameter_On_PC.DelayCamera,
                        DelayReject = PLC_Parameter_On_PC.DelayReject,
                        RejectStreng = PLC_Parameter_On_PC.RejectStreng
                    };
                    //ghi dữ liệu vào file
                    string json = JsonConvert.SerializeObject(newRecipe, Formatting.Indented);
                    File.WriteAllText($"PLC_RECIPEs/{enterText.TextValue}.rplc", json);
                    //thêm logs mặc định
                    AddLogRecipe(enterText.TextValue, $"{newRecipe.DelayCamera},{newRecipe.DelayReject},{newRecipe.RejectStreng}", "CREATE", Globalvariable.CurrentUser.Username);
                    AddLogRecipe(enterText.TextValue, $"{newRecipe.DelayCamera},{newRecipe.DelayReject},{newRecipe.RejectStreng}", "SELECT", Globalvariable.CurrentUser.Username);
                    //cập nhật danh sách Recipe
                    ipRecipe.Items.Add(enterText.TextValue);
                    //cập nhật Recipe đã chọn
                    SelectRecipeName = enterText.TextValue + ".rplc";
                    ipRecipe.SelectedItem = SelectRecipeName;
                    //gưii thông báo thành công
                    this.ShowSuccessNotifier($"Đã tạo Recipe mới: {enterText.TextValue}");

                };
                enterText.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //xóa Recipe đã chọn
            if (string.IsNullOrEmpty(SelectRecipeName) || SelectRecipeName == "Default.rplc")
            {
                this.ShowErrorNotifier("Không thể xóa Recipe mặc định.");
                return;
            }
            //xoá file Recipe
            string filePath = $"PLC_RECIPEs/{SelectRecipeName}";
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    //cập nhật danh sách Recipe
                    ipRecipe.Items.Remove(SelectRecipeName);
                    //chọn Recipe mặc định
                    SelectRecipeName = "Default.rplc";
                    ipRecipe.SelectedItem = SelectRecipeName;
                    //cập nhật PLC parameter

                    PLC_Parameter_On_PC.DelayCamera = defaultConfig.DelayCamera;
                    PLC_Parameter_On_PC.DelayReject = defaultConfig.DelayReject;
                    PLC_Parameter_On_PC.RejectStreng = defaultConfig.RejectStreng;

                    //ghi log
                    AddLogRecipe(SelectRecipeName, "N/A", "DELETE", Globalvariable.CurrentUser.Username);
                    this.ShowSuccessNotifier($"Đã xóa Recipe: {SelectRecipeName}");
                }
                catch (Exception ex)
                {
                    this.ShowErrorNotifier($"Lỗi khi xóa Recipe: {ex.Message}");
                }
            }
            else
            {
                this.ShowErrorNotifier("Recipe không tồn tại.");
            }

        }
        #endregion

        #endregion

        #region Sau
        PLC_Parameter defaultConfig_CS = new PLC_Parameter
        {
            DelayCamera = "1000",
            DelayReject = "2000",
            RejectStreng = "20",
        };
        public void CreateDefaultRecipeTable_CS()
        {

            string json = JsonConvert.SerializeObject(defaultConfig_CS, Formatting.Indented);
            File.WriteAllText(defaultFilePath_CS, json);
            SelectRecipeName_CS = "Default.rplc";
            string RecipeValue = $"{defaultConfig_CS.DelayCamera},{defaultConfig_CS.DelayReject},{defaultConfig_CS.RejectStreng}";
            //thêm logs mặc định
            AddLogRecipe("Default.rplc", RecipeValue, "CREATE", "Operator");
            AddLogRecipe("Default.rplc", RecipeValue, "SELECT", "Operator");

            PLC_Parameter_On_PC_CS = defaultConfig_CS;
        }

        public DataTable Get_Last_Select_Recipe_CS()
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=PLC_RECIPEs_CS/log.rlplc;Version=3;"))
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

        public void Check_Recipe_CS(string recipeName, PLC_Parameter configs)
        {
            //kiểm tra trong danh sách file có Recipe đó hay không, nếu không thì tạo mới

            string[] files = Directory.GetFiles("PLC_RECIPEs", "*.rplc");

            if (files.Length == 0)
            {
                string json = JsonConvert.SerializeObject(defaultConfig_CS, Formatting.Indented);
                File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName_CS}", json);
                //thêm logs mặc định
                AddLogRecipe(SelectRecipeName_CS, $"{defaultConfig_CS.DelayCamera},{defaultConfig_CS.DelayReject},{defaultConfig_CS.RejectStreng}", "CREATE", "Operator");
                AddLogRecipe(SelectRecipeName_CS, $"{defaultConfig_CS.DelayCamera},{defaultConfig_CS.DelayReject},{defaultConfig_CS.RejectStreng}", "SELECT", "Operator");
                PLC_Parameter_On_PC_CS = defaultConfig_CS;
            }
            else
            {
                bool fileExists = files.Any(file => Path.GetFileName(file).Equals(SelectRecipeName_CS, StringComparison.OrdinalIgnoreCase));

                if (!fileExists)
                {
                    string json = JsonConvert.SerializeObject(defaultConfig_CS, Formatting.Indented);
                    File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName_CS}", json);
                    //thêm logs mặc định
                    AddLogRecipe(SelectRecipeName_CS, $"{defaultConfig_CS.DelayCamera},{defaultConfig_CS.DelayReject},{defaultConfig_CS.RejectStreng}", "CREATE", "Operator");
                    AddLogRecipe(SelectRecipeName_CS, $"{defaultConfig_CS.DelayCamera},{defaultConfig_CS.DelayReject},{defaultConfig_CS.RejectStreng}", "SELECT", "Operator");

                    PLC_Parameter_On_PC_CS = defaultConfig_CS;
                }
                else
                {
                    //lấy dữ liệu từ file
                    string jsonContent = File.ReadAllText($"PLC_RECIPEs/{SelectRecipeName_CS}");

                    // Chuyển đổi JSON thành object
                    PLC_Parameter_On_PC_CS = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);
                }
            }

            //cập nhật lên màn hình
            this.InvokeIfRequired(() =>
            {
                ipDelayTriger_CS.Text = PLC_Parameter_On_PC_CS.DelayCamera;
                ipDelayReject_CS.Text = PLC_Parameter_On_PC_CS.DelayReject;
                ipRejectStreng_CS.Text = PLC_Parameter_On_PC_CS.RejectStreng;
            });


        }

        public void GetParameterFromPLC_CS()
        {
            OperateResult<int[]> read = omronPLC_Hsl1.plc.ReadInt32(PLCAddress.Get("PLC_Delay_Camera_DM_C1"), 3);
            if (read.IsSuccess)
            {
                PLC_Parameter_On_PLC_CS.DelayCamera = read.Content[0].ToString();
                PLC_Parameter_On_PLC_CS.DelayReject = read.Content[1].ToString();
                PLC_Parameter_On_PLC_CS.RejectStreng = read.Content[2].ToString();

            }
            else
            {
                PLC_Parameter_On_PLC_CS.DelayCamera = "-1";
                PLC_Parameter_On_PLC_CS.DelayReject = "-1";
                PLC_Parameter_On_PLC_CS.RejectStreng = "-1";
            }

        }

        public void AddLogRecipe_CS(string recipeName, string recipeValue, string action, string userName)
        {
            using (var conn = new SQLiteConnection($"Data Source={log_FilePath_CS};Version=3;"))
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

        public void Write_Recipe_To_File_CS(string json)
        {
            //ghi dữ liệu vào file
            File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName_CS}", json);
            //ghi log
            AddLogRecipe(SelectRecipeName, json, "UPDATE", "Operator");
        }
        #endregion

        private async Task PLC_Comfirm_Async()
        {

            // Bắt đầu nhiệm vụ
            try
            {
                while (!GTask.Task_PLC_Comfirm.Token.IsCancellationRequested)
                {
                    if (isOpen)
                    {
                        GetParameterFromPLC_CS();
                        GetParameterFromPLC();

                        this.InvokeIfRequired(() =>
                        {
                            // Cập nhật giao diện người dùng với giá trị từ PLC
                            opDelayTriger_CS.Text = PLC_Parameter_On_PLC_CS.DelayCamera;
                            opDelayReject_CS.Text = PLC_Parameter_On_PLC_CS.DelayReject;
                            opRejectStreng_CS.Text = PLC_Parameter_On_PLC_CS.RejectStreng;

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

        private void btnNewCS_Click(object sender, EventArgs e)
        {
            using (Entertext enterText = new Entertext())
            {
                enterText.TileText = "Nhập tên Recipe";
                enterText.TextValue = "New Recipe Camera Sau";
                enterText.EnterClicked += (s, args) =>
                {
                    //ipUserName.Text = enterText.TextValue;

                    //kiểm tra xem tên Recipe đã tồn tại hay chưa
                    if (File.Exists($"PLC_RECIPEs_CS/{enterText.TextValue}.rplc"))
                    {
                        this.ShowErrorNotifier("Tên Recipe đã tồn tại, vui lòng chọn tên khác.");
                        return;
                    }
                    //nếu chưa tồn tại thì tạo mới gán PC value vào Recipe

                    PLC_Parameter newRecipe = new PLC_Parameter
                    {
                        DelayCamera = PLC_Parameter_On_PC_CS.DelayCamera,
                        DelayReject = PLC_Parameter_On_PC_CS.DelayReject,
                        RejectStreng = PLC_Parameter_On_PC_CS.RejectStreng
                    };
                    //ghi dữ liệu vào file
                    string json = JsonConvert.SerializeObject(newRecipe, Formatting.Indented);
                    File.WriteAllText($"PLC_RECIPEs_CS/{enterText.TextValue}.rplc", json);
                    //thêm logs mặc định
                    AddLogRecipe(enterText.TextValue, $"{newRecipe.DelayCamera},{newRecipe.DelayReject},{newRecipe.RejectStreng}", "CREATE", Globalvariable.CurrentUser.Username);
                    AddLogRecipe(enterText.TextValue, $"{newRecipe.DelayCamera},{newRecipe.DelayReject},{newRecipe.RejectStreng}", "SELECT", Globalvariable.CurrentUser.Username);
                    //cập nhật danh sách Recipe
                    ipRecipe_CS.Items.Add(enterText.TextValue);
                    //cập nhật Recipe đã chọn
                    SelectRecipeName_CS = enterText.TextValue + ".rplc";
                    ipRecipe_CS.SelectedItem = SelectRecipeName;
                    //gưii thông báo thành công
                    this.ShowSuccessNotifier($"Đã tạo Recipe mới: {enterText.TextValue}");

                };
                enterText.ShowDialog();
            }
        }

        private void ipRecipe_CS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ipRecipe_CS.SelectedText.Length > 3)
            {
                // Lấy tên công thức đã chọn
                SelectRecipeName_CS = ipRecipe_CS.SelectedText;
                //lấy dữ liệu từ file
                string jsonContent = File.ReadAllText($"PLC_RECIPEs_CS/{SelectRecipeName_CS}");

                // Chuyển đổi JSON thành object
                PLC_Parameter az = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);

                ipDelayTriger_CS.Text = az.DelayCamera;
                ipDelayReject_CS.Text = az.DelayReject;
                ipRejectStreng_CS.Text = az.RejectStreng;
            }

            if (isLoading)
                return;
        }

        private void btnSaveCS_Click(object sender, EventArgs e)
        {
            //ghi log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("o"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Update PLC Parameter", Globalvariable.CurrentUser.Username, "Người dùng cập nhật tham số PLC Camera Sau");
            SystemLogs.LogQueue.Enqueue(systemLogs);
            //tạo task write to PLC
            Task.Run(() =>
            {
                try
                {
                    // Lấy giá trị từ các trường nhập liệu
                    string delayCamera = ipDelayTriger_CS.Text;
                    string delayReject = ipDelayReject_CS.Text;
                    string rejectStreng = ipRejectStreng_CS.Text;
                    // Cập nhật giá trị vào PLC_Parameter_On_PC
                    PLC_Parameter_On_PC_CS.DelayCamera = delayCamera;
                    PLC_Parameter_On_PC_CS.DelayReject = delayReject;
                    PLC_Parameter_On_PC_CS.RejectStreng = rejectStreng;
                    // Chuyển đổi thành JSON
                    string json = JsonConvert.SerializeObject(PLC_Parameter_On_PC_CS, Formatting.Indented);
                    // Ghi vào file
                    Write_Recipe_To_File_CS(json);
                    // Ghi vào PLC
                    OperateResult operateResult = omronPLC_Hsl1.plc.Write(PLCAddress.Get("PLC_Delay_Camera_DM_C1"), new int[] { int.Parse(delayCamera), int.Parse(delayReject), int.Parse(rejectStreng) });
                    if (!operateResult.IsSuccess)
                    {
                        this.ShowErrorNotifier($"Lỗi khi ghi vào PLC: {operateResult.Message}");
                        return;
                    }
                    // Ghi log
                    AddLogRecipe_CS(SelectRecipeName_CS, $"{delayCamera},{delayReject},{rejectStreng}", "UPDATE", Globalvariable.CurrentUser.Username);

                    this.ShowSuccessNotifier("Cập nhật Camera Sau thành công!");

                }
                catch (Exception ex)
                {
                    this.ShowErrorNotifier($"Lỗi khi cập nhật Camera Sau: {ex.Message}");

                }
            });
        }

        private void btnUpCS_Click(object sender, EventArgs e)
        {
            //ghi log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("O"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Undo PLC Parameter", Globalvariable.CurrentUser.Username, "Người dùng hoàn tác tham số PLC Camera Sau");
            SystemLogs.LogQueue.Enqueue(systemLogs);
            //cập nhật PLC parameter từ PLC
            ipDelayReject_CS.Text = PLC_Parameter_On_PLC_CS.DelayReject;
            ipDelayTriger_CS.Text = PLC_Parameter_On_PLC_CS.DelayCamera;
            ipRejectStreng_CS.Text = PLC_Parameter_On_PLC_CS.RejectStreng;
            //cập nhật PLC parameter trên PC
        }
    }
}
