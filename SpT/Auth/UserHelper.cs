using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpT.Auth
{
    public enum LoginAction
    {
        Login,
        Logout,
        UpdateProfile,
        DeleteAccount,
        AddAccount, // Thêm hành động thêm tài khoản
        ViewData,
        ChangePassword, // Thêm hành động đổi mật khẩu
        ResetPassword, // Thêm hành động đặt lại mật khẩu
        TwoFactorAuthentication, // Thêm hành động xác thực hai yếu tố
        AdminPrivileges // Thêm hành động quản trị viên
    }

    public class UserData
    {
        public string Username { get; set; } = string.Empty;  // Tên user
        public string Password { get; set; }  // Hash password
        public string Salt { get; set; }      // Salt
        public string Role { get; set; }      // Quyền
        public string Key2FA { get; set; }    // Khóa 2FA

        public override string ToString()
        {
            return $"User[Username={Username}, Role={Role}]";
        }

        //lấy danh sách user từ sqlite trong table users cột Username
        public static DataTable GetUserListFromDB()
        {
            var dataTable = new DataTable();
            using (var db = new SQLiteConnection("Data Source=abcc.bcaa;Version=3;"))
            {
                db.Open();
                using (var command = new SQLiteCommand("SELECT Username FROM users", db))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }

        //lấy user từ sqlite trong table users theo Username
        public static UserData GetUserByUsername(string username, string data_file_path)
        {
            Auth.UserData user = null;

            using (var conn = new SQLiteConnection($"Data Source={data_file_path};Version=3;"))
            {
                conn.Open();

                string sql = @"SELECT ID, Username, Password, Salt, Role, Key2FA 
                       FROM users WHERE Username = @username LIMIT 1";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            user = new UserData
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
    }
}
