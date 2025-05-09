using System;
using System.Drawing;
using System.Text.RegularExpressions;
using Sunny.UI;
using System.Windows.Forms;
using DATALOGIC_SCAN;
using System.IO.Ports;

namespace Dialogs
{
    public partial class Scaner : Form
    {
        Connection _ScanConection = new Connection();
        public string TextValue { get; private set; }
        public string _Title { get; set; } = "SCANER";

        public event EventHandler OkClicked;
        public Scaner()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            _ScanConection.DISCONNECT();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Regex: ^\d{13}$ nghĩa là chuỗi chứa đúng 13 ký tự số
            bool isValid = Regex.IsMatch(uiRichTextBox1.Text, @"^\d{1,15}$");

            if (isValid)
            {
                TextValue = uiRichTextBox1.Text;
                // Kích hoạt sự kiện OkClicked
                OkClicked?.Invoke(this, EventArgs.Empty);

                // Đóng form với kết quả OK
                DialogResult = DialogResult.OK;

                this.Close();
                _ScanConection.DISCONNECT();
            }
            else
            {
                Invoke(new Action(() => { this.ShowErrorTip("Nội dung không hợp lệ"); }));

            }
            
        }

        private void Scaner_Load(object sender, EventArgs e)
        {
            ScanCOMPorts();
            _ScanConection.SERIALPORT = serialPort1;
            _ScanConection.EVENT += _ScanConection_EVENT; ;
            _ScanConection.LOAD();
            
            uiTitlePanel1.Text= _Title;
        }

        private void _ScanConection_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    pnConnect.FillColor = Color.Green;
                    pnConnect.Text = "Scaner kết nối";
                    break;
                case e_Serial.Disconnected:
                    pnConnect.FillColor = Color.Red;
                    pnConnect.Text = "Scaner mất kết nối";
                    break;
                case e_Serial.Recive:
                    string content = s;
                    Invoke(new Action(() => { uiRichTextBox1.Text = content; }));
                    break;
                default:
                    break;
            }
        }

        bool scanning = true;
        private void ScanCOMPorts()
        {
            try
            {
                scanning = true;
                string[] portNames = SerialPort.GetPortNames(); // Lấy danh sách COM port

                ipCOM.Items.Clear(); // Xóa dữ liệu cũ trong ComboBox
                foreach (string port in portNames)
                {
                    ipCOM.Items.Add(port); // Thêm từng COM port vào ComboBox
                }

                if (ipCOM.Items.Count > 0)
                {
                    ipCOM.SelectedIndex = 0; // Chọn COM đầu tiên nếu có
                }
                else
                {
                    ipCOM.Text = "No COM ports found";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi quét cổng COM: " + ex.Message);
            }
            finally
            {
                scanning = false;
            }
        }

        private void ipCOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ScanConection.CONNECT(ipCOM.SelectedText);
        }
    }
}
