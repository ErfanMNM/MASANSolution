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

namespace SpT.Auth
{
    public partial class ucUser_Login : UserControl
    {
        public ucUser_Login()
        {
            InitializeComponent();
        }

        private void uiTableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private enum LoginAction
        {
            Login,
            Logout,
            UpdateProfile,
            DeleteAccount,
            ViewData
        }

        public void INIT()
        {
            //tạo thông tin log

            
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                //lưu lại thông tin đăng nhập vào hàng đợi log
                

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
    }
}
