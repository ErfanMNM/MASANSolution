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
            Auth.UserInfo.UserName = "A";
        }
    }
}
