using DATALOGIC_SCAN;
using Diaglogs;
using Dialogs;
using DocumentFormat.OpenXml.Office.Word;
using HslCommunication;
using MainClass;
using QR_MASAN_01.Utils;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace QR_MASAN_01
{
    public partial class ScanQR_V2 : UIPage
    {
        public static bool formOpen { get; set; } = true;
        Connection _ScanConection = new Connection();

    
        public ScanQR_V2()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            btnKeyBoard.Enabled = false;

                    if (!WK_Check.IsBusy)
                    {
                        WK_Check.RunWorkerAsync(ipQRContent.Text);
                    }
                    else
                    {
                        this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
                    }
                

        }

        public void INIT()
        {
            _ScanConection.SERIALPORT = serialPort1;
            _ScanConection.EVENT += _ScanConection_EVENT; ;
            _ScanConection.LOAD();
            _ScanConection.CONNECT(Setting.Current.HandScanCOM);
        }

        private void _ScanConection_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    Invoke(new Action(() => { opScanerSTT.Text = "Kết nối"; opScanerSTT.FillColor = Globalvariable.OK_Color; }));
                    break;
                case e_Serial.Disconnected:
                    Invoke(new Action(() => { opScanerSTT.Text = "Mất kết nối"; opScanerSTT.FillColor = Globalvariable.NG_Color; }));
                    break;
                case e_Serial.Recive:
                    string content = s;

                            if (!WK_Check.IsBusy)
                            {
                                Invoke(new Action(() => { 
                                    
                                    ipQRContent.Text = content;
                                    btnSearch.Enabled = false;
                                    btnKeyBoard.Enabled = false;

                                }));
                                WK_Check.RunWorkerAsync(content);
                            }
                            else
                            {
                                this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
                            }
                    break;
                default:
                    break;
            }
        }
        public void SearchCode(string searchQR)
        {
            //tìm kiếm mã QR 
            //tạo vòng lặp duyệt qua các file .db không có chứa Record trong tên trong C:/.ABC tìm giá trị searchQR trong bảng UniqueCodes cột Code, nếu tìm thấy thì render dữ liệu vào uiDataGridView1
            this.InvokeIfRequired(() =>
            {
                uiDataGridView1.DataSource = null; // Xóa dữ liệu cũ
            });
            try
            {
                string[] dbFiles = Directory.GetFiles(@"C:\.ABC", "*.db", SearchOption.AllDirectories)
                                            .Where(f => !f.Contains("Record")).ToArray();
                if (dbFiles.Length == 0)
                {
                    this.ShowErrorDialog("Không tìm thấy file cơ sở dữ liệu nào.");
                    return;
                }
                bool found = false;
                foreach (string dbFile in dbFiles)
                {
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
                    {
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM UniqueCodes WHERE Code = @Code", connection))
                        {
                            command.Parameters.AddWithValue("@Code", searchQR);
                            using (SQLiteDataAdapter da = new SQLiteDataAdapter(command))
                            {
                                DataTable dataTable = new DataTable();
                                da.Fill(dataTable);
                                if (dataTable.Rows.Count > 0)
                                {
                                    //thêm 1 cột lên đầu
                                    

                                    // Render dữ liệu vào uiDataGridView1
                                    Invoke(new Action(() =>
                                    {
                                        uiDataGridView1.DataSource = null; // Xóa dữ liệu cũ
                                        uiDataGridView1.DataSource = dataTable;
                                        oporderNo.Text = dbFile.Substring(dbFile.LastIndexOf('\\') + 1, dbFile.Length - dbFile.LastIndexOf('\\') - 5); // Lấy tên file không có đuôi .db
                                        opCMD.Items.Add("Tìm thấy mã: " + searchQR + " trong file: " + dbFile);
                                        opCMD.SelectedIndex = opCMD.Items.Count - 1; // Chọn mục cuối cùng để hiển thị thông báo
                                        opCMD.SelectedIndex = 0; // Chọn mục đầu tiên để hiển thị thông báo
                                    }));
                                    found = true;
                                    break; // Dừng vòng lặp nếu tìm thấy
                                }
                            }
                        }
                    }
                }
                if (!found)
                {
                    Invoke(new Action(() => { 
                        
                        this.ShowErrorDialog("Không tìm thấy mã QR trong cơ sở dữ liệu.");
                        opCMD.Items.Add("Không tìm thấy mã: " + searchQR + " trong bất kỳ file nào.");
                        uiDataGridView1.DataSource = null; // Xóa dữ liệu cũ
                        oporderNo.Text = "Không tìm thấy mã QR"; // Cập nhật thông báo
                        opCMD.SelectedIndex = opCMD.Items.Count - 1; // Chọn mục cuối cùng để hiển thị thông báo lỗi
                    }));
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => { this.ShowErrorDialog($"Lỗi khi tìm kiếm mã QR: {ex.Message}"); }));
            }
            finally
            {
                Invoke(new Action(() => { btnSearch.Enabled = true; btnKeyBoard.Enabled = true; }));
            }
        }

        private void WK_Check_DoWork(object sender, DoWorkEventArgs e)
        {
            string searchQR = e.Argument as string; // Nhận giá trị truyền vào
            SearchCode(searchQR);

        }
        private void ScanQR_Initialize(object sender, EventArgs e)
        {
            formOpen = true;
            this.InvokeIfRequired(() =>
            {
                INIT();
                opScanerSTT.Text = "Chưa kết nối";
                opScanerSTT.FillColor = Globalvariable.NG_Color;
                oporderNo.Text = "Chưa có mã QR";
                ipQRContent.Text = string.Empty;
                uiDataGridView1.DataSource = null; // Xóa dữ liệu cũ
                opCMD.Items.Clear(); // Xóa các mục trong ComboBox

                opScanerCOM.Text = Setting.Current.HandScanCOM;
            });
        }

        private void ScanQR_Finalize(object sender, EventArgs e)
        {
            formOpen = false;
        }

        private void btnKeyBoard_Click(object sender, EventArgs e)
        {
            using (var dialog = new Entertext())
            {
                dialog.TextValue = ipQRContent.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ipQRContent.Text = dialog.TextValue;
                }
            }
        }

        private void ipSWMode_ValueChanged(object sender, bool value)
        {
            if(ipSWMode.Active)
            {
                opModeMess.Text = "Phần mềm sẽ tự kích hoạt mã mới lưu vào csdl khi quét";
            }
            else
            {
                opModeMess.Text= "Phần mềm chỉ hiện thị trạng thái mã, gạt sw để thay đổi";
            }
        }

        private void opCMD_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog(opCMD.SelectedItem?.ToString() ?? "Không có thông tin nào được chọn.");
        }
    }
}
