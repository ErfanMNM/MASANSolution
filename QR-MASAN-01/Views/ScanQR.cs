using DATALOGIC_SCAN;
using Diaglogs;
using Dialogs;
using HslCommunication;
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
    public partial class ScanQR : UIPage
    {
        public static bool formOpen { get; set; } = true;
        Connection _ScanConection = new Connection();

    
        public ScanQR()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if (!WK_Check.IsBusy)
            {
                if (Globalvariable.Data_Status == e_Data_Status.READY)
                {
                    if (!WK_Check.IsBusy)
                    {
                        WK_Check.RunWorkerAsync(ipQRContent.Text);
                    }
                    else
                    {
                        this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
                    }
                }
                else
                {
                   this.ShowErrorDialog("Vui lòng chờ hệ thống tải xong dữ liệu sản xuất!");
                }
            }
            else
            {
               this.ShowErrorDialog("LỖI!!!!");
            }
        }

        public void INIT()
        {
            //opScanerCOM.Text = Globalvariable.config.ScanerCOM;
            _ScanConection.SERIALPORT = serialPort1;
            _ScanConection.EVENT += _ScanConection_EVENT; ;
            _ScanConection.LOAD();
            _ScanConection.CONNECT("COM2");
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
                    if(formOpen)
                    {
                        if (Globalvariable.Data_Status == e_Data_Status.READY)
                        {
                            if (!WK_Check.IsBusy)
                            {
                                Invoke(new Action(() => { ipQRContent.Text = content; }));
                                WK_Check.RunWorkerAsync(content);
                            }
                            else
                            {
                                this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
                            }
                        }
                        else
                        {
                            this.ShowErrorDialog("Vui lòng chờ hệ thống tải xong dữ liệu sản xuất!");
                        }
                    }
                    else
                    {
                        this.ShowErrorDialog("Vui lòng bật sang bảng Quét QR!!!!");
                    }
                    
                    break;
                default:
                    break;
            }
        }
        bool AutoActive = false;

        ProductData _productData { get; set; } = new ProductData();
        public void SearchCode(string searchQR)
        {
            //tìm kiếm mã QR 
            if (Globalvariable.Data_Status == e_Data_Status.READY)
            {
                if(AutoActive)
                {

                }
                else
                {
                    var _gdtf = GetDatabaseFilesByMonth(ipDateSearch.Value.ToString("MM/yyyy"));

                    if(_gdtf.IsSuccess)
                    {
                        foreach (string file in _gdtf.FilesList)
                        {
                            var _sqr = Get_QR_From_SQLite(Path.Combine("Client_Database", ipDateSearch.Value.Year.ToString(), ipDateSearch.Value.Month.ToString("D2"), file), searchQR);
                            if(_sqr.Issucces)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: {_sqr.PData.ProductID} - {_sqr.PData.ProductQR} - {_sqr.PData.Active} - {_sqr.PData.TimeStamp}");
                                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                                }));
                                _productData = _sqr.PData;
                                return;
                            }
                            else
                            {

                            }

                            
                        }
                        LogUpdate($"{DateTime.Now:HH:mm:ss}: Mã không tồn tại. {searchQR}");
                    }
                    else
                    {
                        LogUpdate($"{DateTime.Now:HH:mm:ss}: {searchQR} - {_gdtf.Message}");
                        return;
                    }
                }
                //if (!string.IsNullOrEmpty(searchQR) && searchQR.Contains(Globalvariable.QRgoc))
                //{
                //    if (Globalvariable.ProductQR_Dictionary.TryGetValue(searchQR, out ProductData ProductInfo))
                //    {
                //        if (ProductInfo.Active != 1)
                //        {
                //            if(Mode)
                //            {
                //                ProductInfo.Active = 1;
                //                ProductInfo.TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                //                //đưa vào hàng chờ để xử lí data
                //                Globalvariable.UpdateQueue120.Enqueue(ProductInfo.ProductID);

                //                this.Invoke(new Action(() =>
                //                {
                //                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` Đã đưa vào hàng chờ kích hoạt thành công");
                //                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                //                }));
                //            }
                //            else
                //            {
                //                this.Invoke(new Action(() =>
                //                {
                //                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` Chưa được kích hoạt");
                //                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                //                }));
                //            }    
                            
                //        }
                //        else
                //        {
                //            this.Invoke(new Action(() =>
                //            {
                //                opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` ĐÃ KÍCH HOẠT lúc : {ProductInfo.TimeStamp}");
                //                opCMD.SelectedIndex = opCMD.Items.Count - 1;
                //            }));
                //        }
                //    }
                //    else
                //    {
                //        if (Globalvariable.APPMODE != e_Mode.OLDMode)
                //        {
                //            this.Invoke(new Action(() =>
                //            {
                //                opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: KHÔNG TÌM THẤY sản phẩm: `{searchQR}` : {ProductInfo.TimeStamp}");
                //                opCMD.SelectedIndex = opCMD.Items.Count - 1;
                //            }));
                //        }
                //        else
                //        {
                //            if (Mode)
                //            {
                //                Globalvariable.AddQueue120.Enqueue(searchQR);

                //                this.Invoke(new Action(() =>
                //                {
                //                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` Đã đưa vào hàng chờ kích hoạt thành công");
                //                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                //                }));
                //            }
                //            else
                //            {
                //                this.Invoke(new Action(() =>
                //                {
                //                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` Chưa được kích hoạt");
                //                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                //                }));
                //            }
                                
                //        }
                //    }
                //}
                //else
                //{
                //    this.Invoke(new Action(() =>
                //    {
                //        opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Mã QR đầu vào không đúng định dạng sản xuất hôm nay.");
                //        opCMD.SelectedIndex = opCMD.Items.Count - 1;
                //    }));
                //}
                
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Vui lòng chờ máy lấy dữ liệu sản xuất hoàn tất rồi thử lại.");
                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                }));
            }

        }


        //Chương trình 
        public static (bool IsSuccess, string Message, List<string> FilesList) GetDatabaseFilesByMonth(string ipDateSearch)
        {
            var fileList = new List<string>();

            if (!DateTime.TryParseExact(ipDateSearch, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return (false, "Ngày tháng không hợp lệ. Vui lòng nhập theo định dạng MM/yyyy.", fileList);
            }

            string folderPath = Path.Combine("Client_Database", date.Year.ToString(), date.Month.ToString("D2"));

            if (!Directory.Exists(folderPath))
            {
                return (false,"Không tìm thấy tệp dữ liệu nào", fileList); // Trả về list rỗng nếu thư mục không tồn tại
            }

            // Lấy tất cả file (hoặc lọc định dạng cụ thể nếu muốn)
            string[] files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                fileList.Add(Path.GetFileName(file));
            }

            return (true, "Thành công", fileList);
        }

        //tìm mã trong SQLite
        public (bool Issucces, string message, ProductData PData) Get_QR_From_SQLite(string file, string code)
        {
            ProductData _prD = new ProductData();

            _prD.ProductQR = code;
            _prD.ProductID = "0"; // Mặc định ID là 0 nếu không tìm thấy
            _prD.Active = 0; // Mặc định chưa kích hoạt
            _prD.TimeStamp = "-"; // Thời gian 
            try
            {
                code = code.Replace(";", "").Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\r\n","");
                string connectionString = $@"Data Source={file};Version=3;";
                string query = $"SELECT * FROM `QRContent` WHERE `ProductQR` LIKE '%{code}%';";

                

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {

                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                _prD.ProductID = dt.Rows[0]["ProductID"].ToString();
                                _prD.Active = Convert.ToInt32(dt.Rows[0]["Active"]);
                                _prD.TimeStamp = dt.Rows[0]["TimestampActive"].ToString();
                                _prD.ProductQR = dt.Rows[0]["ProductQR"].ToString();
                                return (true, "Tải thông tin MFI thành công", _prD);
                            }
                            else
                            {
                                return (false, "Không có thông tin MFI", _prD);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return (false, ex.Message, _prD);
            }
        }
        //thêm mã
        public void LogUpdate (string mess)
        {
            this.Invoke(new Action(() =>
            {
                opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: {mess}");
                opCMD.SelectedIndex = opCMD.Items.Count - 1;
            }));
        }
        public class ProductData
        {
            public string ProductID { get; set; }
            public string ProductQR { get; set; }
            public int Active { get; set; } // 1: đã kích hoạt, 0: chưa kích hoạt
            public string TimeStamp { get; set; } // thời gian kích hoạt
        }
        private void WK_Check_DoWork(object sender, DoWorkEventArgs e)
        {
            string searchQR = e.Argument as string; // Nhận giá trị truyền vào
            SearchCode(searchQR);
        }
        private void ScanQR_Initialize(object sender, EventArgs e)
        {
            ipDateSearch.Value = DateTime.Now;
            formOpen = true;
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
                AutoActive = true;
            }
            else
            {
                opModeMess.Text= "Phần mềm chỉ hiện thị trạng thái mã, gạt sw để thay đổi";
                AutoActive = false;
            }
        }
    }
}
