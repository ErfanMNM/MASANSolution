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

namespace SPMS1
{
    public partial class ucplcsetting : UserControl
    {
        public string logfile_Path { get; set; } = "PLC_RECIPEs/log.rlplc";
        public string Datafile_Path { get; set; } = "PLC_RECIPEs/Default.rplc";

        public static PLC_Parameter PLC_Parameter_On_PC { get; set; } = new PLC_Parameter();
        public static PLC_Parameter PLC_Parameter_On_PLC { get; set; } = new PLC_Parameter();

        public string SelectRecipeName = string.Empty;
        public string EditRecipeName = string.Empty;

        private BackgroundWorker bgwSave;
        private BackgroundWorker bgwUpdate;

        public ucplcsetting()
        {
            InitializeComponent();

            // Khởi tạo BackgroundWorker cho lưu
            bgwSave = new BackgroundWorker();
            bgwSave.DoWork += bgwSave_DoWork;
            bgwSave.RunWorkerCompleted += bgwSave_RunWorkerCompleted;

            // BackgroundWorker để cập nhật thông số từ PLC
            bgwUpdate = new BackgroundWorker();
            bgwUpdate.WorkerSupportsCancellation = true;
            bgwUpdate.DoWork += bgwUpdate_DoWork;

            // Event handlers
            btnSave.Click += btnSave_Click;
            btnUndo.Click += btnUndo_Click;
            btnDelete.Click += btnDelete_Click;
            btnNewRecipe.Click += btnNewRecipe_Click;
            ipRecipe.SelectedIndexChanged += ipRecipe_SelectedIndexChanged;
        }

        public void Loadplcsetting()
        {
            FirstCheck();
            UpdateRecipeComboBox();

            // Bắt đầu cập nhật từ PLC (nếu cần)
            if (!bgwUpdate.IsBusy)
            {
                bgwUpdate.RunWorkerAsync();
            }
        }

        private void FirstCheck()
        {
            PLC_Parameter defaultConfig = new PLC_Parameter
            {
                DelayCamera = "1000",
                DelayReject = "2000",
                RejectStreng = "20",
            };

            // Tạo thư mục nếu chưa có
            if (!Directory.Exists("PLC_RECIPEs"))
            {
                Directory.CreateDirectory("PLC_RECIPEs");
            }

            // Tạo file log nếu chưa có
            if (!File.Exists(logfile_Path))
            {
                SQLiteConnection.CreateFile(logfile_Path);
                CreateLogTable(logfile_Path);
            }

            // Tạo file default nếu chưa có
            if (!File.Exists(Datafile_Path))
            {
                File.WriteAllText(Datafile_Path, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
            }

            // Lấy recipe được chọn lần cuối
            DataTable datatable = Get_Last_Select_Recipe();

            if (datatable.Rows.Count >= 1)
            {
                SelectRecipeName = datatable.Rows[0]["RecipeName"].ToString();
                string[] valueR = datatable.Rows[0]["RecipeValue"].ToString().Split(',');
                defaultConfig = new PLC_Parameter
                {
                    DelayCamera = valueR[0],
                    DelayReject = valueR[1],
                    RejectStreng = valueR[2]
                };
            }
            else
            {
                SelectRecipeName = "Default";
            }

            LoadRecipe(SelectRecipeName, defaultConfig);
        }

        private void LoadRecipe(string recipeName, PLC_Parameter config)
        {
            string filePath = $"PLC_RECIPEs/{recipeName}.rplc";

            if (!File.Exists(filePath))
            {
                // Tạo file recipe mới
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(filePath, json);
                AddLogRecipe(recipeName, $"{config.DelayCamera},{config.DelayReject},{config.RejectStreng}", "CREATE", "Operator");
                PLC_Parameter_On_PC = config;
            }
            else
            {
                // Load từ file
                string jsonContent = File.ReadAllText(filePath);
                PLC_Parameter_On_PC = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);
            }

            // Cập nhật UI
            this.InvokeIfRequired(() =>
            {
                ipDelayTriger.Text = PLC_Parameter_On_PC.DelayCamera;
                ipDelayReject.Text = PLC_Parameter_On_PC.DelayReject;
                ipRejectStreng.Text = PLC_Parameter_On_PC.RejectStreng;
            });
        }

        private void UpdateRecipeComboBox()
        {
            ipRecipe.Items.Clear();

            if (Directory.Exists("PLC_RECIPEs"))
            {
                foreach (var file in Directory.GetFiles("PLC_RECIPEs", "*.rplc"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    ipRecipe.Items.Add(fileName);
                }
            }

            if (ipRecipe.Items.Count == 0)
            {
                ipRecipe.Items.Add("Default");
            }

            ipRecipe.SelectedIndex = 0;

            if (!string.IsNullOrEmpty(SelectRecipeName) && ipRecipe.Items.Contains(SelectRecipeName))
            {
                ipRecipe.SelectedItem = SelectRecipeName;
            }
        }

        private void CreateLogTable(string dbPath)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
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

        private void AddLogRecipe(string recipeName, string recipeValue, string action, string userName)
        {
            using (var conn = new SQLiteConnection($"Data Source={logfile_Path};Version=3;"))
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

        private DataTable Get_Last_Select_Recipe()
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={logfile_Path};Version=3;"))
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

        // Event Handlers
        private void ipRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ipRecipe.SelectedItem == null) return;

            SelectRecipeName = ipRecipe.SelectedItem.ToString();
            string filePath = $"PLC_RECIPEs/{SelectRecipeName}.rplc";

            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                PLC_Parameter recipe = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);

                ipDelayTriger.Text = recipe.DelayCamera;
                ipDelayReject.Text = recipe.DelayReject;
                ipRejectStreng.Text = recipe.RejectStreng;

                AddLogRecipe(SelectRecipeName, $"{recipe.DelayCamera},{recipe.DelayReject},{recipe.RejectStreng}", "SELECT", "Operator");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!bgwSave.IsBusy)
            {
                bgwSave.RunWorkerAsync();
            }
        }

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string delayCamera = ipDelayTriger.Text;
                string delayReject = ipDelayReject.Text;
                string rejectStreng = ipRejectStreng.Text;

                PLC_Parameter_On_PC.DelayCamera = delayCamera;
                PLC_Parameter_On_PC.DelayReject = delayReject;
                PLC_Parameter_On_PC.RejectStreng = rejectStreng;

                string json = JsonConvert.SerializeObject(PLC_Parameter_On_PC, Formatting.Indented);
                string filePath = $"PLC_RECIPEs/{SelectRecipeName}.rplc";
                File.WriteAllText(filePath, json);

                AddLogRecipe(SelectRecipeName, $"{delayCamera},{delayReject},{rejectStreng}", "UPDATE", "Operator");

                e.Result = "SUCCESS";
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is string && e.Result.ToString() == "SUCCESS")
            {
                UIMessageBox.ShowSuccess("Lưu thành công!");
            }
            else if (e.Result is Exception ex)
            {
                UIMessageBox.ShowError($"Lỗi khi lưu: {ex.Message}");
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            // Load lại từ PLC (nếu có kết nối PLC)
            // Hoặc load lại từ file
            string filePath = $"PLC_RECIPEs/{SelectRecipeName}.rplc";
            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                PLC_Parameter recipe = JsonConvert.DeserializeObject<PLC_Parameter>(jsonContent);

                ipDelayTriger.Text = recipe.DelayCamera;
                ipDelayReject.Text = recipe.DelayReject;
                ipRejectStreng.Text = recipe.RejectStreng;
            }
        }

        private void btnNewRecipe_Click(object sender, EventArgs e)
        {
            //string newRecipeName = UIInputDialog.ShowInputDialog("Nhập tên Recipe mới:", "Tạo Recipe Mới", "New Recipe");

            if (string.IsNullOrEmpty(newRecipeName))
            {
                return;
            }

            string filePath = $"PLC_RECIPEs/{newRecipeName}.rplc";

            if (File.Exists(filePath))
            {
                UIMessageBox.ShowError("Tên Recipe đã tồn tại, vui lòng chọn tên khác.");
                return;
            }

            PLC_Parameter newRecipe = new PLC_Parameter
            {
                DelayCamera = PLC_Parameter_On_PC.DelayCamera,
                DelayReject = PLC_Parameter_On_PC.DelayReject,
                RejectStreng = PLC_Parameter_On_PC.RejectStreng
            };

            string json = JsonConvert.SerializeObject(newRecipe, Formatting.Indented);
            File.WriteAllText(filePath, json);

            AddLogRecipe(newRecipeName, $"{newRecipe.DelayCamera},{newRecipe.DelayReject},{newRecipe.RejectStreng}", "CREATE", "Operator");
            AddLogRecipe(newRecipeName, $"{newRecipe.DelayCamera},{newRecipe.DelayReject},{newRecipe.RejectStreng}", "SELECT", "Operator");

            ipRecipe.Items.Add(newRecipeName);
            SelectRecipeName = newRecipeName;
            ipRecipe.SelectedItem = SelectRecipeName;

            UIMessageBox.ShowSuccess($"Đã tạo Recipe mới: {newRecipeName}");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectRecipeName) || SelectRecipeName == "Default")
            {
                UIMessageBox.ShowError("Không thể xóa Recipe mặc định.");
                return;
            }

            if (!UIMessageBox.ShowAsk($"Bạn có chắc muốn xóa Recipe '{SelectRecipeName}'?"))
            {
                return;
            }

            string filePath = $"PLC_RECIPEs/{SelectRecipeName}.rplc";

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    ipRecipe.Items.Remove(SelectRecipeName);

                    AddLogRecipe(SelectRecipeName, "N/A", "DELETE", "Operator");

                    SelectRecipeName = "Default";
                    ipRecipe.SelectedItem = SelectRecipeName;

                    UIMessageBox.ShowSuccess($"Đã xóa Recipe");
                }
                catch (Exception ex)
                {
                    UIMessageBox.ShowError($"Lỗi khi xóa Recipe: {ex.Message}");
                }
            }
            else
            {
                UIMessageBox.ShowError("Recipe không tồn tại.");
            }
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            // Background worker để cập nhật thông số từ PLC
            // Nếu cần kết nối PLC, implement tại đây
            while (!bgwUpdate.CancellationPending)
            {
                // TODO: Đọc giá trị từ PLC và cập nhật vào PLC_Parameter_On_PLC
                // Ví dụ:
                // PLC_Parameter_On_PLC.DelayCamera = ReadFromPLC(...);

                this.InvokeIfRequired(() =>
                {
                    opDelayTriger.Text = PLC_Parameter_On_PLC.DelayCamera;
                    opDelayReject.Text = PLC_Parameter_On_PLC.DelayReject;
                    opRejectStreng.Text = PLC_Parameter_On_PLC.RejectStreng;
                });

                System.Threading.Thread.Sleep(500); // Cập nhật mỗi 0.5 giây
            }
        }

        public class PLC_Parameter
        {
            public string DelayCamera { get; set; } = "0";
            public string DelayReject { get; set; } = "0";
            public string RejectStreng { get; set; } = "0";
        }
    }

    // Extension method để invoke trên UI thread
    public static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
