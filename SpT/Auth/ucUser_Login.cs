using SpT;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpT.Auth
{
    public partial class ucUser_Login : UIUserControl
    {
        public ucUser_Login()
        {
            InitializeComponent();
        }

        public bool IS2FAEnabled { get; set; } = true; // Biến để kiểm tra xem 2FA có được bật hay không

        private void uiTableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        public void INIT()
        {
            //tạo thông tin log
            var log = new LogHelper<LoginAction>("userlog.db");

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                //lưu lại thông tin đăng nhập vào hàng đợi log
                

                // Kiểm tra thông tin đăng nhập
                string username = ipUserName.Text.Trim();
                string password = ipPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    
                    return;
                }

                //kiểm tra thông tin đăng nhập trong sqlite abcc.bcaa
                if (ValidateCredentials(username, password))
                {
                    //kiểm tra 2FA nếu cài đặt có yêu cầu 2FA
                    UserData dbUser = UserData.GetUserByUsername(username);

                    if (IS2FAEnabled)
                    {
                        bool isValid = TwoFAHelper.VerifyOTP(dbUser.Key2FA, ipOTP.Text, digits: 6);
                        if (!isValid)
                        {
                            //ghi log, trả sự kiện
                            return;
                        }
                    }
                    //trả lại thông tin user

                    //ghi log

                    //trả về sự kiện

                    return;
                }
                else
                {
                    //trả về sự kiện lỗi, ghi log
                }

            }
            catch (Exception ex)
            {
                // Ghi log lỗi vào hàng đợi
                
                // Hiển thị thông báo lỗi cho người dùng
                
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            // File SQLite đặt cạnh file exe
            string dbFile = "abcc.bcaa";
            if (!System.IO.File.Exists(dbFile))
            {
                //trả về sự kiện
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
    }
}
