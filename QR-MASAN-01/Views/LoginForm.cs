using DocumentFormat.OpenXml.Spreadsheet;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QR_MASAN_01;
using Diaglogs;
using System.Data.SQLite;
using System.IO;
using Sunny.UI.Win32;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace QR_MASAN_01.Views
{
    public partial class LoginForm : UIPage
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            
            try
            {
                //lưu lại thông tin đăng nhập vào hàng đợi log
                SystemLogs.LogQueue.Enqueue(new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Người dùng nhấn nút đăng nhập", "LoginForm", "Bắt đầu quá trình đăng nhập"));

                // Kiểm tra thông tin đăng nhập
                string username = ipUserName.Text.Trim();
                string password = ipPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    this.ShowErrorNotifier("Vui lòng nhập tên đăng nhập và mật khẩu.");

                    SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.LOGIN, "Người dùng đăng nhập thất bại", "LoginForm", $"Người dùng nhập username : {username}, password : {password}");
                    SystemLogs.LogQueue.Enqueue(systemLogs);

                    return;
                }

                // Giả sử bạn có một phương thức ValidateCredentials để kiểm tra thông tin đăng nhập
                if (ValidateCredentials(username, password))
                {
                  Auth.UserData dbUser = GetUserByUsername(username);
                  Globalvariable.CurrentUser = dbUser; // Lưu thông tin người dùng vào GlobalSettings

                    this.ShowSuccessTip("Đăng nhập thành công, vui lòng chờ.");
                    SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.LOGIN, "Người dùng đăng nhập thành công", "LoginForm", $"Người dùng đăng nhập với username : {username}");
                    SystemLogs.LogQueue.Enqueue(systemLogs);

                    return;
                }
                else
                {
                    this.ShowErrorTip("Tên đăng nhập hoặc mật khẩu không đúng.");
                    SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.LOGIN, "Người dùng đăng nhập thất bại", "LoginForm", $"Người dùng nhập username : {username}, password : {password}");
                    SystemLogs.LogQueue.Enqueue(systemLogs);
                }

            }
            catch (Exception ex)
            {
                // Ghi log lỗi vào hàng đợi
                SystemLogs.LogQueue.Enqueue(new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.ERROR, "Lỗi khi đăng nhập", "LoginForm", ex.Message));
                // Hiển thị thông báo lỗi cho người dùng
                this.ShowErrorDialog("Lỗi khi đăng nhập", ex.Message, UIStyle.Red);
            }
            
        }

        //hàm ValidateCredentials kiểm tra thông tin đăng nhập trong sqlite abcc.bcaa gồm ID, Username, Password, Salt, Role, Key2FA

        private bool ValidateCredentials(string username, string password)
        {
            // File SQLite đặt cạnh file exe
            string dbFile = "abcc.bcaa";
            if (!System.IO.File.Exists(dbFile))
            {
                this.ShowErrorNotifier("Cơ sở dữ liệu không tồn tại.");
                return false;
            }
            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT Password, Salt FROM users WHERE Username = @username";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader.GetString(0);
                            string salt = reader.GetString(1);
                            string hashedPassword = HashPassword(password, salt);
                            return storedPassword == hashedPassword;
                        }
                    }
                }
                conn.Close();
            }
            return false;
        }

        // Hàm HashPassword để hash mật khẩu với salt
        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }


        private void ipUserName_DoubleClick(object sender, EventArgs e)
        {
            using(Entertext enterText = new Entertext())
            {
                enterText.TileText = "Nhập tên đăng nhập";
                enterText.TextValue = ipUserName.Text;
                enterText.EnterClicked += (s, args) =>
                {
                    ipUserName.Text = enterText.TextValue;
                };
                enterText.ShowDialog();
            }
        }

        private void ipPassword_DoubleClick(object sender, EventArgs e)
        {
            using (Entertext enterText = new Entertext())
            {
                enterText.TileText = "Nhập mật khẩu";
                enterText.TextValue = ipPassword.Text;
                enterText.IsPassword = true; // Thiết lập chế độ nhập mật khẩu
                enterText.EnterClicked += (s, args) =>
                {
                    ipPassword.Text = enterText.TextValue;
                };
                enterText.ShowDialog();
            }
        }

        // Hàm GetUserByUsername để lấy thông tin người dùng từ SQLite
        private Auth.UserData GetUserByUsername(string username)
        {
            Auth.UserData user = null;

            using (var conn = new SQLiteConnection($"Data Source=abcc.bcaa;Version=3;"))
            {
                conn.Open();

                string sql = @"SELECT ID, Username, Password, Salt, Role, Key2FA 
                       FROM users WHERE Username = @username LIMIT 1";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new System.Data.DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            user = new Auth.UserData
                            {
                                Username = row["Username"].ToString(),
                                Password = row["Password"].ToString(),
                                Salt = row["Salt"].ToString(),
                                Role = row["Role"].ToString(),
                                Key2FA = row["Key2FA"].ToString()
                            };
                        }
                    }
                }

                conn.Close();
            }

            return user;
        }

        #region tạo user

        //// File SQLite đặt cạnh file exe
        //private string dbFile = "abcc.bcaa";
        //private void uiSymbolButton1_Click(object sender, EventArgs e)
        //{
        //    CreateDatabaseIfNotExists();

        //    string username = uiTextBox2.Text.Trim();
        //    string password = uiTextBox1.Text.Trim();
        //    string role = "user";
        //    string key2FA = Guid.NewGuid().ToString();

        //    // 1️⃣ Tạo Salt random
        //    byte[] saltBytes = new byte[16];
        //    using (var rng = new RNGCryptoServiceProvider())
        //    {
        //        rng.GetBytes(saltBytes);
        //    }
        //    string salt = Convert.ToBase64String(saltBytes);

        //    // 2️⃣ Hash Password + Salt
        //    string hashedPassword = HashPassword(password, salt);

        //    using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        //    {
        //        conn.Open();
        //        string sql = "INSERT INTO users (Username, Password, Salt, Role, Key2FA) VALUES (@username, @password, @salt, @role, @key2fa)";
        //        using (var cmd = new SQLiteCommand(sql, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@username", username);
        //            cmd.Parameters.AddWithValue("@password", hashedPassword);
        //            cmd.Parameters.AddWithValue("@salt", salt);
        //            cmd.Parameters.AddWithValue("@role", role);
        //            cmd.Parameters.AddWithValue("@key2fa", key2FA);
        //            cmd.ExecuteNonQuery();
        //        }
        //        conn.Close();
        //    }

        //    MessageBox.Show("User đã được thêm OK (Mật khẩu đã được hash + salt)!");

        //}



        //private void CreateDatabaseIfNotExists()
        //{
        //    if (!System.IO.File.Exists(dbFile))
        //    {
        //        SQLiteConnection.CreateFile(dbFile);
        //    }

        //    using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        //    {
        //        conn.Open();
        //        string sql = @"
        //        CREATE TABLE IF NOT EXISTS users (
        //            ID INTEGER PRIMARY KEY AUTOINCREMENT,
        //            Username TEXT NOT NULL,
        //            Password TEXT NOT NULL,
        //            Salt TEXT NOT NULL,
        //            Role TEXT NOT NULL,
        //            Key2FA TEXT NOT NULL

        //        )";
        //        using (var cmd = new SQLiteCommand(sql, conn))
        //        {
        //            cmd.ExecuteNonQuery();
        //        }
        //        conn.Close();
        //    }
        //}

        #endregion
    }
}
