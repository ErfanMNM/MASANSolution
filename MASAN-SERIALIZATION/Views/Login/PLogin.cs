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
using MASAN_SERIALIZATION.Configs;

namespace MASAN_SERIALIZATION.Views.Login
{
    public partial class PLogin : UIPage
    {
        public PLogin()
        {
            InitializeComponent();
        }

        public void INIT()
        {
            ucUser_Login1.IS2FAEnabled = Configs.Configs.Current.TwoFA_Enabled;
        }

        private void PLogin_Initialize(object sender, EventArgs e)
        {
            ucUser_Login1.INIT();
        }

        private void ucUser_Login1_OnLoginAction(object sender, SpT.Auth.LoginActionEventArgs e)
        {
            if (e.Status)
            {
                // Hiển thị thông báo đăng nhập thành công
                this.ShowSuccessTip($"Đăng nhập thành công, vui lòng chờ trong giây lát");
                //ghi thông tin user
                Globals.CurrentUser = ucUser_Login1.CurrentUser;
            }
            else
            {
                this.ShowErrorTip($"{e.Message}");
            }
        }
    }
}
