
using HslCommunication;
using Newtonsoft.Json;
using QR_MASAN_01.Diaglogs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Sunny.UI.UIAvatar;

namespace QR_MASAN_01.Views.Settings
{

    public partial class F1PLC : UIPage
    {
        public F1PLC()
        {
            InitializeComponent();
        }
        public string SelectRecipeName = string.Empty;
        public string log_FilePath = "PLC_RECIPEs/log.rlplc";
        public string defaultFilePath = "PLC_RECIPEs/Default.rplc";
        public static PLC_Parameter PLC_Parameter_On_PC { get; set; } = new PLC_Parameter();
        public static PLC_Parameter PLC_Parameter_On_PLC{ get; set; } = new PLC_Parameter();
        private void FPLC_Load(object sender, EventArgs e)
        {
            Check();
            UpdateCBB();
            //Khai báo PLC
            omronPLC_Hsl1.PLC_IP = PLCAddress.Get("PLC_IP");
            omronPLC_Hsl1.PLC_PORT = int.Parse(PLCAddress.Get("PLC_PORT").ToString());
            omronPLC_Hsl1.InitPLC();
            //Update_HMI();
            //if(!WK_PLC.IsBusy)
            //{
            //    WK_PLC.RunWorkerAsync();
            //}
            
            WKUpdate.RunWorkerAsync();
            
        }

        public class PLC_Parameter
        {
            public string DelayCamera { get; set; } = "-1";
            public string DelayReject { get; set; } = "-1";
            public string ProcessTimeOut { get; set; } = "-1";
            public string RejectStreng { get; set; } = "-1";
            public string DeboundSensor { get; set; } = "-1";
        }

        public void Check()
        {
            //kiểm tra lịch sử
            if (!File.Exists(log_FilePath))
            {
                SQLiteConnection.CreateFile(log_FilePath);
                CreateLogTable();

                PLC_Parameter defaultConfig = new PLC_Parameter
                {
                    DelayCamera = "1000",
                    DelayReject = "2000",
                    ProcessTimeOut = "100",
                    RejectStreng = "20",
                    DeboundSensor = "231"
                };

                string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText(defaultFilePath, json);
                SelectRecipeName = "Default.rplc";
                string RecipeValue = $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.ProcessTimeOut},{defaultConfig.RejectStreng},{defaultConfig.DeboundSensor}";
                //thêm logs mặc định
                AddLogRecipe("Default.rplc", RecipeValue, "CREATE", "Operator");
                AddLogRecipe("Default.rplc", RecipeValue, "SELECT", "Operator");

                PLC_Parameter_On_PC = defaultConfig;
            }
            else
            {
                //lấy dòng đầu tiên
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


                //kiểm tra trong danh sách file có Recipe đó hay không, nếu không thì tạo mới

                string[] files = Directory.GetFiles("PLC_RECIPEs", "*.rplc");

                if (files.Length == 0)
                {
                    string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                    File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName}", json);
                    //thêm logs mặc định
                    AddLogRecipe(SelectRecipeName, $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.ProcessTimeOut},{defaultConfig.RejectStreng},{defaultConfig.DeboundSensor}", "CREATE", "Operator");
                    AddLogRecipe(SelectRecipeName, $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.ProcessTimeOut},{defaultConfig.RejectStreng},{defaultConfig.DeboundSensor}", "SELECT", "Operator");
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
                        AddLogRecipe(SelectRecipeName, $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.ProcessTimeOut},{defaultConfig.RejectStreng},{defaultConfig.DeboundSensor}", "CREATE", "Operator");
                        AddLogRecipe(SelectRecipeName, $"{defaultConfig.DelayCamera},{defaultConfig.DelayReject},{defaultConfig.ProcessTimeOut},{defaultConfig.RejectStreng},{defaultConfig.DeboundSensor}", "SELECT", "Operator");

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
        }

        public void UpdateCBB()
        {
            // Lấy danh sách file .rplc và thêm vào ComboBox
            foreach (var file in Directory.GetFiles("PLC_RECIPEs", "*.rplc"))
            {
                cbbRecipe.Items.Add(Path.GetFileName(file));
            }

            // Chọn giá trị mặc định nếu tồn tại
            if (cbbRecipe.Items.Contains(SelectRecipeName))
            {
                cbbRecipe.SelectedItem = SelectRecipeName;
            }
        }

        public void Update_HMI()
        {
            if(!WK_PLC.IsBusy)
            {
                WK_PLC.RunWorkerAsync();
            }
            
            ipDelay_Camera.Value = Convert.ToInt16(PLC_Parameter_On_PC.DelayCamera);
            ipDelay_Reject.Value = Convert.ToInt16(PLC_Parameter_On_PC.DelayReject);
            ipDebounce_Sensor.Value = Convert.ToInt16(PLC_Parameter_On_PC.DeboundSensor);
            ipProcess_TimeOut.Value = Convert.ToInt16(PLC_Parameter_On_PC.ProcessTimeOut);
            ipReject_Streng.Value = Convert.ToInt16(PLC_Parameter_On_PC.RejectStreng);


            opDelayCmr.Text = PLC_Parameter_On_PLC.DelayCamera;
            opDelayReject.Text = PLC_Parameter_On_PLC.DelayReject;
            opDeboundSensor.Text = PLC_Parameter_On_PLC.DeboundSensor;
            opProcessing.Text = PLC_Parameter_On_PLC.ProcessTimeOut;
            opRejectStreng.Text = PLC_Parameter_On_PLC.RejectStreng;
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
        public class Result
        {
            public bool Success { get; set; }
            public string Message { get; set; }

            public Result(bool success, string message)
            {
                Success = success;
                Message = message;
            }
        }

        // Key bí mật để mã hóa và giải mã (bạn có thể lưu ở nơi an toàn hơn)
        private static readonly string encryptionKey = "1234567890asdfghjklpoiuytrewqqaz";

        public Result SaveConfigs(string filePath)
        {
            try
            {
                // Chuyển đổi đối tượng thành JSON
                string json = JsonConvert.SerializeObject(this);

                // Mã hóa JSON
                byte[] encryptedData = EncryptStringToBytes_Aes(json, encryptionKey);

                // Lưu dữ liệu mã hóa vào file
                File.WriteAllBytes(filePath, encryptedData);
                return new Result(true, "Lưu thành công");
            }
            catch (Exception ex)
            {
                return new Result(false, $"Lỗi trong quá trình lưu: {ex.Message}");
            }

        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // Sử dụng IV mặc định là mảng byte 0

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            this.ShowStatusForm(100, "Tải lại dữ liệu", 0);

            Update_HMI();
            for (int i = 0; i < 88; i++)
            {
                SystemEx.Delay(10);
                this.SetStatusFormDescription("Đang tải lại dữ liệu" + "(" + i + "%)......");
                this.SetStatusFormStepIt();
            }

            this.HideStatusForm();
        }

        private void WKUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WKUpdate.CancellationPending)
            {
                //đọc độ dài sản phẩm
                //OperateResult<int[]> read = omronPLC_Hsl1.plc.ReadInt32("D0", 5);
                //if (read.IsSuccess)
                //{
                    
                //}
                Thread.Sleep(1000);
            }
        }

        private void btnCustom_Click(object sender, EventArgs e)
        {
            if(IsRead)
            {
                OperateResult<int> read = omronPLC_Hsl1.plc.ReadInt32(ipDCustom.Text);
                if(read.IsSuccess)
                {
                    ipopValueCustom.Text = read.Content.ToString();
                }
                else
                {
                    this.ShowErrorDialog(read.Message);
                }
            }
            else
            {
                OperateResult write = omronPLC_Hsl1.plc.Write(ipDCustom.Text, Convert.ToInt32(ipopValueCustom.Text));
                if(write.IsSuccess)
                {
                    this.ShowSuccessDialog("Ghi thành công");
                }
                else
                {
                    this.ShowErrorDialog(write.Message);
                }
            }
        }

  
       
        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            btnSave.Text = "Đang lưu";
            WK_Write_To_PLC.RunWorkerAsync();
        }

        private void btnNewRecipe_Click(object sender, EventArgs e)
        {
            c =false;
            using (var dialog = new CreateRecipe())
            {
                dialog.TextValue = "RecipeName";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PLC_Parameter_On_PC.DelayCamera = ipDelay_Camera.Value.ToString();
                    PLC_Parameter_On_PC.DelayReject = ipDelay_Reject.Value.ToString();
                    PLC_Parameter_On_PC.DeboundSensor = ipDebounce_Sensor.Value.ToString();
                    PLC_Parameter_On_PC.ProcessTimeOut = ipProcess_TimeOut.Value.ToString();
                    PLC_Parameter_On_PC.RejectStreng = ipReject_Streng.Value.ToString();
                    SelectRecipeName = $"{dialog.TextValue}.rplc";
                    if (!File.Exists($"PLC_RECIPEs /{ SelectRecipeName}"))
                    {
                        string json = JsonConvert.SerializeObject(PLC_Parameter_On_PC, Formatting.Indented);
                        File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName}", json);
                        string RecipeValue = $"{PLC_Parameter_On_PC.DelayCamera},{PLC_Parameter_On_PC.DelayReject},{PLC_Parameter_On_PC.ProcessTimeOut},{PLC_Parameter_On_PC.RejectStreng},{PLC_Parameter_On_PC.DeboundSensor}";
                        //thêm logs mặc định
                        AddLogRecipe(SelectRecipeName, RecipeValue, "CREATE", "Operator");
                        AddLogRecipe(SelectRecipeName, RecipeValue, "SELECT", "Operator");

                        cbbRecipe.Items.Add(SelectRecipeName);
                        cbbRecipe.SelectedItem = SelectRecipeName;
                        //lấy dữ liệu từ file
                        string jsonContent = File.ReadAllText($"PLC_RECIPEs/{SelectRecipeName}");

                        // Chuyển đổi JSON thành object
                        PLC_Parameter az = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);

                        ipDelay_Camera.Value = Convert.ToInt16(az.DelayCamera);
                        ipDelay_Reject.Value = Convert.ToInt16(az.DelayReject);
                        ipDebounce_Sensor.Value = Convert.ToInt16(az.DeboundSensor);
                        ipProcess_TimeOut.Value = Convert.ToInt16(az.ProcessTimeOut);
                        ipReject_Streng.Value = Convert.ToInt16(az.RejectStreng);
                        c = true;
                    }
                    else
                    {
                        c = true;
                       this.ShowErrorDialog("Tên đã tồn tại, vui lòng chọn tên khác.");
                    }
                        
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            c = false;
            if (cbbRecipe.Items.Count <= 1)
            {
                c= true;
               // this.ShowErrorDialog("Không thể xóa cài đặt duy nhất");
            }
            else
            {
                if (this.ShowAskDialog("Đồng ý xóa?", "Sau khi xóa cài đặt sẽ không thể khôi phục!!!", UIStyle.Red))
                {
                    File.Delete($"PLC_RECIPEs/{cbbRecipe.SelectedText}");
                    // Xóa khỏi ComboBox
                    cbbRecipe.Items.Remove(cbbRecipe.SelectedText);
                    string RecipeValue = $"{PLC_Parameter_On_PC.DelayCamera},{PLC_Parameter_On_PC.DelayReject},{PLC_Parameter_On_PC.ProcessTimeOut},{PLC_Parameter_On_PC.RejectStreng},{PLC_Parameter_On_PC.DeboundSensor}";

                    AddLogRecipe(SelectRecipeName, RecipeValue, "DELETE", "Operator");
                    // Chọn mục đầu tiên nếu danh sách không rỗng
                    if (cbbRecipe.Items.Count > 0)
                    {
                        cbbRecipe.SelectedIndex = 0;
                    }
                    SelectRecipeName = cbbRecipe.SelectedText;

                    //lấy dữ liệu từ file
                    string jsonContent = File.ReadAllText($"PLC_RECIPEs/{SelectRecipeName}");

                    // Chuyển đổi JSON thành object
                    PLC_Parameter az = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);

                    ipDelay_Camera.Value = Convert.ToInt16(az.DelayCamera);
                    ipDelay_Reject.Value = Convert.ToInt16(az.DelayReject);
                    ipDebounce_Sensor.Value = Convert.ToInt16(az.DeboundSensor);
                    ipProcess_TimeOut.Value = Convert.ToInt16(az.ProcessTimeOut);
                    ipReject_Streng.Value = Convert.ToInt16(az.RejectStreng);

                    Update_HMI();
                    c = true;
                }
                else
                {
                   
                    //thêm logs mặc định
                    SelectRecipeName = cbbRecipe.SelectedText;
                    c= true;
                   
                }
            }
            
        }
        public static bool c = true;
        private void cbbRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(c)
            {
                //lấy dữ liệu từ file
                string jsonContent = File.ReadAllText($"PLC_RECIPEs/{SelectRecipeName}");

                // Chuyển đổi JSON thành object
                PLC_Parameter az = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);

                ipDelay_Camera.Value = Convert.ToInt16(az.DelayCamera);
                ipDelay_Reject.Value = Convert.ToInt16(az.DelayReject);
                ipDebounce_Sensor.Value = Convert.ToInt16(az.DeboundSensor);
                ipProcess_TimeOut.Value = Convert.ToInt16(az.ProcessTimeOut);
                ipReject_Streng.Value = Convert.ToInt16(az.RejectStreng);
            }
            
        }
        
        
        private void WK_PLC_DoWork(object sender, DoWorkEventArgs e)
        {
            OperateResult<int[]> read = omronPLC_Hsl1.plc.ReadInt32("D0", 5);
            if (read.IsSuccess)
            {
                PLC_Parameter_On_PLC.DelayCamera = read.Content[0].ToString();
                PLC_Parameter_On_PLC.DelayReject = read.Content[1].ToString();
                PLC_Parameter_On_PLC.RejectStreng = read.Content[2].ToString();
                PLC_Parameter_On_PLC.DeboundSensor = read.Content[3].ToString();
                PLC_Parameter_On_PLC.ProcessTimeOut = read.Content[4].ToString();

            }
            else
            {
                PLC_Parameter_On_PLC.DelayCamera = "-1";
                PLC_Parameter_On_PLC.DelayReject = "-1";
                PLC_Parameter_On_PLC.RejectStreng = "-1";
                PLC_Parameter_On_PLC.DeboundSensor = "-1";
                PLC_Parameter_On_PLC.ProcessTimeOut = "-1";
            }
            Thread.Sleep(5000); // Đợi 1 giây trước khi đọc lại
        }

        private void WK_PLC_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Update_HMI();
        }

        private void WK_Write_To_PLC_DoWork(object sender, DoWorkEventArgs e)
        {
            //"PLCDelayCameraMemory": "D0",
            //"PLCDelayRejectMemory": "D2",
            //"PLCRejectStrengMemory" : "D4",
            //"PLCDeboundSensorMemory": "D6",
            //"PLCProcessTimeoutMemory": "D8",

            PLC_Parameter_On_PC.DelayCamera = ipDelay_Camera.Value.ToString();
            PLC_Parameter_On_PC.DelayReject = ipDelay_Reject.Value.ToString();
            PLC_Parameter_On_PC.DeboundSensor = ipDebounce_Sensor.Value.ToString();
            PLC_Parameter_On_PC.ProcessTimeOut = ipProcess_TimeOut.Value.ToString();
            PLC_Parameter_On_PC.RejectStreng = ipReject_Streng.Value.ToString();

            OperateResult write = omronPLC_Hsl1.plc.Write("D0", $"[{PLC_Parameter_On_PC.DelayCamera},{PLC_Parameter_On_PC.DelayReject},{PLC_Parameter_On_PC.RejectStreng},{PLC_Parameter_On_PC.DeboundSensor},{PLC_Parameter_On_PC.ProcessTimeOut}]".ToStringArray<int>());
            if (write.IsSuccess)
            {
                PLC_Parameter_On_PC.DelayCamera = ipDelay_Camera.Value.ToString();
                PLC_Parameter_On_PC.DelayReject = ipDelay_Reject.Value.ToString();
                PLC_Parameter_On_PC.DeboundSensor = ipDebounce_Sensor.Value.ToString();
                PLC_Parameter_On_PC.ProcessTimeOut = ipProcess_TimeOut.Value.ToString();
                PLC_Parameter_On_PC.RejectStreng = ipReject_Streng.Value.ToString();

                string json = JsonConvert.SerializeObject(PLC_Parameter_On_PC, Formatting.Indented);
                string RecipeValue = $"{PLC_Parameter_On_PC.DelayCamera},{PLC_Parameter_On_PC.DelayReject},{PLC_Parameter_On_PC.ProcessTimeOut},{PLC_Parameter_On_PC.RejectStreng},{PLC_Parameter_On_PC.DeboundSensor}";

                if (SelectRecipeName != cbbRecipe.SelectedText)
                {
                    SelectRecipeName = cbbRecipe.SelectedText;
                    AddLogRecipe(SelectRecipeName, RecipeValue, "SELECT", "Operator");
                }

                File.WriteAllText($"PLC_RECIPEs/{SelectRecipeName}", json);
                //thêm logs mặc định
                AddLogRecipe(SelectRecipeName, RecipeValue, "UPDATE", "Operator");
              this.ShowSuccessDialog("Lưu thành công");
            }
            else
            {
                AddLogRecipe(SelectRecipeName, write.Message, "UPDATE_FAIL", "Operator");
              this.ShowErrorDialog("Lưu thất bại");
            }
        }

        private void WK_Write_To_PLC_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Update_HMI();
            btnSave.Enabled = true;
            btnSave.Text = "Lưu lại";
        }


        //custom mode

        bool IsRead = false;
        private void ipChange_Read_Write_Mode_Click(object sender, EventArgs e)
        {
            IsRead = !IsRead;
            if (IsRead) { ipChange_Read_Write_Mode.Text = "ĐỌC"; }
            else { ipChange_Read_Write_Mode.Text = "GHI"; }
        }
    }
}
