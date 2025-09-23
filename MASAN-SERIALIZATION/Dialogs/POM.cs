using DATALOGIC_SCAN;
using MASAN_SERIALIZATION.Diaglogs;
using SpT;
using Sunny.UI;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Dialogs
{
    public partial class Pom_dialog : Form
    {

        //public event EventHandler OkClicked;
        //public event EventHandler CancelClicked;
        public string Message { get; set; } = string.Empty;
        public string lydo { get; set; } 
        public string STT { get; set; } = string.Empty;

        public string Key2FA { get; set; } = string.Empty;
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
           
                if (uiNumPadTextBox1.Text == string.Empty)
                {
                    Invoke(new Action(() => { this.ShowErrorTip("Vui lòng nhập mã xác thực"); }));
                    Message = "Vui lòng nhập mã xác thực";
                    DialogResult = DialogResult.Cancel;
                    return;
                }
                else
                {

                    bool isValid = TwoFAHelper.VerifyOTP(Key2FA, uiNumPadTextBox1.Text, digits: 6);
                    if (!isValid)
                    {
                        Invoke(new Action(() => { this.ShowErrorTip("Mã xác thực không đúng"); }));
                        Message = "Mã xác thực không đúng";
                        DialogResult = DialogResult.Cancel;    
                        return;
                    }
                }

            

            if(uiRichTextBox2.TextLength < 30)
            {
                Invoke(new Action(() => { this.ShowErrorTip("Vui lòng nhập nội dung ít nhất 30 ký tự"); }));
                Message = "Vui lòng nhập nội dung ít nhất 30 ký tự";
                DialogResult = DialogResult.Cancel;
                return;
            }
            lydo = uiRichTextBox2.Text;
            DialogResult = DialogResult.OK;


        }

        private void uiRichTextBox2_DoubleClick(object sender, EventArgs e)
        {
            using (var dialog = new Entertext())
            {
                dialog.TextValue = uiRichTextBox2.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    uiRichTextBox2.Text = dialog.TextValue ;
                }
            }
        }
    }
}
