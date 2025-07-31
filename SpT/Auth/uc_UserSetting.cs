using SpT.Diaglogs;
using SpT.Logs;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SpT.Auth
{
    public partial class uc_UserSetting : UIUserControl
    {
        public uc_UserSetting()
        {
            InitializeComponent();
        }

        public UserData userData { get; set; } = new UserData();
        public string CurrentUserName { get; set; }
        public event EventHandler<LoginActionEventArgs> OnUserAction; // Sự kiện đăng nhập, đăng xuất, cập nhật thông tin người dùng, v.v.
        public bool IS2FAEnabled { get; set; } = false; // Biến để kiểm tra xem 2FA có được bật hay không
        // Đường dẫn đến file dữ liệu SQLite, có thể thay đổi theo nhu cầu, mặc dịnh trong Appdata Local TanTien/Users
        public string data_file_path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TanTien", "Users", "users.database");
        // Biến để lưu log
        public LogHelper<LoginAction> log;

        public void INIT()
        {
            //xóa rỗng các textbox ip
            ipUserName.Text = string.Empty;
            ipOldPassword.Text = string.Empty;
            ipNewPassword.Text = string.Empty;
            ipComfirmNewPassword.Text = string.Empty;

            //tạo đường dẫn đến Appdata Local
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //tạo đường dẫn đến file lưu log
            string logFilePath = System.IO.Path.Combine(appDataPath, "TanTien", "Logs", "userlog.logs");
            //kiểm tra thư mục có tồn tại không
            string directoryPath = System.IO.Path.GetDirectoryName(data_file_path);
            if (!System.IO.Directory.Exists(directoryPath))
            {
                //nếu không tồn tại thì tạo mới
                System.IO.Directory.CreateDirectory(directoryPath);
            }
            //tạo thông tin log
            log = new LogHelper<LoginAction>(logFilePath);
            //kiểm tra thư mục có tồn tại không

            //lấy thong tin user từ file dữ liệu
            userData = UserData.GetUserByUsername(CurrentUserName, data_file_path);

            //load thông tin user vào các textbox
            if (userData.Username != null)
            {
                ipUserName.Text = userData.Username;
                //ipOldPassword.Text = userData.Password; // Không hiển thị mật khẩu cũ
                //ipNewPassword.Text = userData.Password; // Không hiển thị mật khẩu mới
                //ipComfirmNewPassword.Text = userData.Password; // Không hiển thị xác nhận mật khẩu mới
                opRole.Text = userData.Role;
            }
            else
            {
                OnUserAction?.Invoke(this, new LoginActionEventArgs
                {
                    Status = false,
                    Message = "Không tìm thấy thông tin người dùng."
                });
                //ghi log lỗi
                Task.Run(async () =>
                {
                    try
                    {
                        await log.WriteLogAsync(CurrentUserName, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Không tìm thấy thông tin người dùng.");
                    }
                    catch (Exception ex)
                    {
                        // log hoặc ignore
                        Console.WriteLine($"Error logging: {ex.Message}");
                    }
                });
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false; //vô hiệu hóa nút Save để tránh nhấn nhiều lần
            btnSave.Text = "Đang lưu..."; //thay đổi chữ trên nút Save
            try
            {
                if (UserHelper.ValidateCredentials(userData.Username, ipOldPassword.Text, data_file_path))
                {
                    //kiểm tra có khóa 2FA không
                    if (IS2FAEnabled)
                    {
                        //kiểm tra mã OTP có rỗng không
                        if (string.IsNullOrWhiteSpace(ipOTP.Text))
                        {
                            OnUserAction?.Invoke(this, new LoginActionEventArgs
                            {
                                Status = false,
                                Message = "Mã OTP không được để trống."
                            });
                            //ghi log lỗi
                            Task.Run(async () =>
                            {
                                try
                                {
                                    await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mã OTP không được để trống.");
                                }
                                catch (Exception ex)
                                {
                                    // log hoặc ignore
                                    Console.WriteLine($"Error logging: {ex.Message}");
                                }
                            });
                            //await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mã OTP không được để trống.");
                            return;
                        }
                        bool isValid = TwoFAHelper.VerifyOTP(userData.Key2FA, ipOTP.Text, digits: 6);
                        if (!isValid)
                        {
                            //ghi log, trả sự kiện
                            OnUserAction?.Invoke(this, new LoginActionEventArgs
                            {
                                Status = false,
                                Message = "Mã OTP không hợp lệ."
                            });

                            //ghi log lỗi
                            Task.Run(async () =>
                            {
                                try
                                {
                                    await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mã OTP không hợp lệ.");
                                }
                                catch (Exception ex)
                                {
                                    // log hoặc ignore
                                    Console.WriteLine($"Error logging: {ex.Message}");
                                }
                            });
                            //await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mã OTP không hợp lệ.");
                            return;
                        }
                    }
                    //kiểm tra mật khẩu mới có rỗng không
                    if (string.IsNullOrWhiteSpace(ipNewPassword.Text) || string.IsNullOrWhiteSpace(ipComfirmNewPassword.Text))
                    {
                        OnUserAction?.Invoke(this, new LoginActionEventArgs
                        {
                            Status = false,
                            Message = "Mật khẩu mới và xác nhận mật khẩu mới không được để trống."
                        });
                        //ghi log lỗi
                        Task.Run(async () =>
                        {
                            try
                            {
                                await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mật khẩu mới và xác nhận mật khẩu mới không được để trống.");
                            }
                            catch (Exception ex)
                            {
                                // log hoặc ignore
                                Console.WriteLine($"Error logging: {ex.Message}");
                            }
                        });
                        // await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mật khẩu mới và xác nhận mật khẩu mới không được để trống.");
                        return;
                    }

                    //kiểm tra mật khẩu mới và xác nhận mật khẩu mới có giống nhau không
                    if (ipNewPassword.Text != ipComfirmNewPassword.Text)
                    {
                        OnUserAction?.Invoke(this, new LoginActionEventArgs
                        {
                            Status = false,
                            Message = "Mật khẩu mới và xác nhận mật khẩu mới không giống nhau."
                        });
                        //ghi log lỗi
                        Task.Run(async () =>
                        {
                            try
                            {
                                await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mật khẩu mới và xác nhận mật khẩu mới không giống nhau.");
                            }
                            catch (Exception ex)
                            {
                                // log hoặc ignore
                                    Console.WriteLine($"Error logging: {ex.Message}");
                            }
                        });
                        //await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mật khẩu mới và xác nhận mật khẩu mới không giống nhau.");
                        return;
                    }

                    //cập nhật thông tin người dùng

                    if (UserHelper.UpdatePassword(userData.Username, ipNewPassword.Text, data_file_path))
                    {
                        //cập nhật thông tin người dùng thành công
                        userData.Password = UserHelper.HashPassword(ipNewPassword.Text, userData.Salt);
                        //xóa textbox mật khẩu cũ, mới, xác nhận mật khẩu
                        ipOldPassword.Text = string.Empty;
                        ipNewPassword.Text = string.Empty;
                        ipComfirmNewPassword.Text = string.Empty;

                        OnUserAction?.Invoke(this, new LoginActionEventArgs
                        {
                            Status = true,
                            Message = "Cập nhật thông tin người dùng thành công."
                        });
                        //ghi log thành công
                        Task.Run(async () =>
                        {
                            try
                            {
                                await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thành công.");
                            }
                            catch (Exception ex)
                            {
                                // log hoặc ignore
                                Console.WriteLine($"Error logging: {ex.Message}");
                            }
                        });
                        //await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thành công.");
                    }
                    else
                    {
                        OnUserAction?.Invoke(this, new LoginActionEventArgs
                        {
                            Status = false,
                            Message = "Cập nhật thông tin người dùng thất bại."
                        });
                        //ghi log lỗi
                        Task.Run(async () =>
                        {
                            try
                            {
                                await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Không thể cập nhật mật khẩu.");
                            }
                            catch (Exception ex)
                            {
                                // log hoặc ignore
                                Console.WriteLine($"Error logging: {ex.Message}");
                            }
                        });
                        //await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Không thể cập nhật mật khẩu.");
                    }

                    return;
                }
                else
                {
                    OnUserAction?.Invoke(this, new LoginActionEventArgs
                    {
                        Status = false,
                        Message = "Mật khẩu cũ không đúng."
                    });
                    //ghi log lỗi
                    Task.Run(async () =>
                    {
                        try
                        {
                            await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mật khẩu cũ không đúng.");
                        }
                        catch (Exception ex)
                        {
                            // log hoặc ignore
                            Console.WriteLine($"Error logging: {ex.Message}");
                        }
                    });
                    // await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, "Cập nhật thông tin người dùng thất bại: Mật khẩu cũ không đúng.");
                }
            }
            catch (Exception ex)
            {
                OnUserAction?.Invoke(this, new LoginActionEventArgs
                {
                    Status = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                });
                //ghi log lỗi
                Task.Run(async () =>
                {
                    try
                    {
                        await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, $"Cập nhật thông tin người dùng thất bại: {ex.Message}");
                    }
                    catch (Exception logEx)
                    {
                        // log hoặc ignore
                        Console.WriteLine($"Error logging: {logEx.Message}");   
                    }
                });
                //await log.WriteLogAsync(userData.Username, LoginAction.UpdateProfile, $"Cập nhật thông tin người dùng thất bại: {ex.Message}");
            }
            finally
            {
                btnSave.Enabled = true; //bật lại nút Save
                btnSave.Text = "Lưu lại"; //đặt lại chữ trên nút Save
            }
            
        }

        private void ipOldPassword_DoubleClick(object sender, EventArgs e)
        {
            using (Entertext enterText = new Entertext())
            {
                enterText.TileText = "Nhập mật khẩu hiện tại";
                enterText.TextValue = ipOldPassword.Text;
                enterText.EnterClicked += (s, args) =>
                {
                    ipOldPassword.Text = enterText.TextValue;
                };
                enterText.ShowDialog();
            }
        }

        private void ipNewPassword_DoubleClick(object sender, EventArgs e)
        {
            using (Entertext enterText = new Entertext())
            {
                enterText.TileText = "Nhập mật khẩu mới";
                enterText.TextValue = ipNewPassword.Text;
                enterText.EnterClicked += (s, args) =>
                {
                    ipNewPassword.Text = enterText.TextValue;
                };
                enterText.ShowDialog();
            }
        }

        private void ipComfirmNewPassword_DoubleClick(object sender, EventArgs e)
        {
            using (Entertext enterText = new Entertext())
            {
                enterText.TileText = "Xác nhận mật khẩu mới";
                enterText.TextValue = ipComfirmNewPassword.Text;
                enterText.EnterClicked += (s, args) =>
                {
                    ipComfirmNewPassword.Text = enterText.TextValue;
                };
                enterText.ShowDialog();
            }
        }
    }
}
