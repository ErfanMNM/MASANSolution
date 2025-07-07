using DATALOGIC_SCAN;
using QR_MASAN_01;
using SpT;
using Sunny.UI;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Dialogs
{
    public partial class Pom_dialog : Form
    {

        //public event EventHandler OkClicked;
        //public event EventHandler CancelClicked;
        public string Message { get; set; } = string.Empty;
        public string STT { get; set; } = string.Empty;
        public Pom_dialog()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //xác thực mã 2FA
            if (Setting.Current.TwoFA_Enabled_PO)
            {
                if (uiNumPadTextBox1.Text == string.Empty)
                {
                    Invoke(new Action(() => { this.ShowErrorTip("Vui lòng nhập mã xác thực"); }));
                    Message = "Vui lòng nhập mã xác thực";
                    // Kích hoạt sự kiện OkClicked
                    //OkClicked?.Invoke(this, EventArgs.Empty);
                    // Đóng form với kết quả OK
                    DialogResult = DialogResult.Cancel;
                    return;
                }
                else
                {

                    bool isValid = TwoFAHelper.VerifyOTP(Globalvariable.CurrentUser.Key2FA, uiNumPadTextBox1.Text, digits: 6);
                    if (!isValid)
                    {
                        Invoke(new Action(() => { this.ShowErrorTip("Mã xác thực không đúng"); }));
                        Message = "Mã xác thực không đúng";
                        // Kích hoạt sự kiện OkClicked
                        // OkClicked?.Invoke(this, EventArgs.Empty);
                        // Đóng form với kết quả OK
                        DialogResult = DialogResult.Cancel;
                        return;
                    }
                }

            }
            // Kích hoạt sự kiện OkClicked
            // OkClicked?.Invoke(this, EventArgs.Empty);
            // Đóng form với kết quả OK
            DialogResult = DialogResult.OK;


        }
    }
}
