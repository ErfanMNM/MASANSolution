using Diaglogs;
using QR_MASAN_01;
using QR_MASAN_01.Auth;
using SpT;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

                //kiểm tra thông tin đăng nhập trong sqlite abcc.bcaa
                if (ValidateCredentials(username, password))
                {
                    //kiểm tra 2FA nếu cài đặt có yêu cầu 2FA
                    UserData dbUser = UserData.GetUserByUsername(username);

                    if (Setting.Current.TwoFA_Enabled)
                    {
                        bool isValid = TwoFAHelper.VerifyOTP(dbUser.Key2FA, ipOTP.Text, digits: 6);
                        if (!isValid)
                        {
                            this.ShowErrorTip("Mã OTP không hợp lệ hoặc đã hết hạn. Vui lòng thử lại.");
                            SystemLogs systemLogs1 = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.LOGIN, "Người dùng đăng nhập thất bại", "LoginForm", $"Người dùng nhập mã OTP không hợp lệ cho username : {username}");
                            SystemLogs.LogQueue.Enqueue(systemLogs1);
                            return;
                        }
                    }
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
                SystemLogs.LogQueue.Enqueue(new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_ERROR, "Lỗi khi đăng nhập", "LoginForm", ex.Message));
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

        private void ipUserName_DoubleClick_1(object sender, EventArgs e)
        {
            using (Entertext enterText = new Entertext())
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

        DataTable UsersList = new DataTable();
        private void LoginForm_Initialize(object sender, EventArgs e)
        {
            // Tạo DataTable để lưu danh sách người dùng
            UsersList = UserData.GetUserListFromDB();

            //thêm vào cbbox ipUserName
            ipUserName.Items.Clear();
            foreach (DataRow row in UsersList.Rows)
            {
                string username = row["Username"].ToString();
                if (!string.IsNullOrEmpty(username))
                {
                    ipUserName.Items.Add(username);
                }
            }

            if (ipUserName.Items.Count > 0)
            {
                // Chọn mục đầu tiên nếu có
                ipUserName.SelectedIndex = 0;
            }
            else
            {
                // Hiển thị thông báo nếu không có người dùng nào
                this.ShowErrorNotifier("Không có người dùng nào trong hệ thống.");
            }
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
