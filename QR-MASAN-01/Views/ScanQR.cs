using DATALOGIC_SCAN;
using Diaglogs;
using Dialogs;
using HslCommunication;
using MainClass;
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

        ProductInfo _productData { get; set; } = new ProductInfo();
        public void SearchCode(string searchQR)
        {
            //tìm kiếm mã QR 
            if (Globalvariable.Data_Status == e_Data_Status.READY)
            {
                if (AutoActive)
                {
                    //ở chế độ tự động thêm
                    //Kiểm tra xem đúng mã hôm nay hay không
                    if (Setting.Current.App_Mode == "ADD_Data")
                    {
                        //chỉ cần kiểm đúng định dạng là được
                        string pattern = @"i\.tcx\.com\.vn/.*\d{13}.*[a-zA-Z0-9]";
                        var _checkFormat = CheckCodeFormatV2(searchQR, pattern);
                        if (_checkFormat.IsOK)
                        {
                            if (searchQR.Contains(Globalvariable.ProductBarcode))
                            {
                                //tìm trong Dictionary
                                if (Globalvariable.Main_Content_Dictionary.TryGetValue(searchQR, out ProductData ProductInfo))
                                {
                                    if (ProductInfo.Active == 1)
                                    {
                                        LogUpdate($"Mã này đã Kích hoạt hôm nay - {searchQR}");
                                        return;
                                    }
                                    else
                                    {
                                        //thêm mã vào hàng chờ
                                        Globalvariable.Update_Content_To_SQLite_Queue.Enqueue(ProductInfo);
                                        LogUpdate($"Mã này chưa Kích hoạt hôm nay - {searchQR} - Thêm vào hàng chờ kích hoạt");
                                        return;
                                    }
                                }
                                else
                                {
                                    //thêm mã vào hàng chờ
                                    ProductData newProductData = new ProductData
                                    {
                                        ProductID = 0,
                                        Active = 0, // Chưa kích hoạt
                                        TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        TimeUnixActive = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                        TimeStampPrinted = "ReActive",
                                        TimeUnixPrinted = 0,
                                    };
                                    Globalvariable.Add_Content_To_SQLite_Queue.Enqueue((searchQR,newProductData));
                                    LogUpdate($"Mã này chưa Kích HOẠT - {searchQR} - Thêm vào hàng chờ kích hoạt");
                                    return;
                                }
                            }
                            else
                            {
                                LogUpdate($"Mã không đúng định dạng: {searchQR} - {_checkFormat.Message}");
                                return;
                            }
                        }
                        else
                        {
                            LogUpdate($"Mã không đúng định dạng: {searchQR} - {_checkFormat.Message}");
                        }

                    }
                    else if (Setting.Current.App_Mode == "NO_ADD")
                    {
                        var _checkFormat = CheckCodeFormat(searchQR);
                        if (_checkFormat.IsOK)
                        {
                            if (searchQR.Contains(Globalvariable.ProductBarcode))
                            {
                                //tìm trong Dictionary
                                if (Globalvariable.Main_Content_Dictionary.TryGetValue(searchQR, out ProductData ProductInfo))
                                {
                                    if (ProductInfo.Active == 1)
                                    {
                                        LogUpdate($"Mã này đã Kích hoạt hôm nay - {searchQR}");
                                        return;
                                    }
                                    else
                                    {
                                        //thêm mã vào hàng chờ
                                        //thêm mã vào hàng chờ
                                        ProductData newProductData = new ProductData
                                        {
                                            ProductID = 0,
                                            Active = 0, // Chưa kích hoạt
                                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            TimeUnixActive = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                            TimeStampPrinted = "ReActive",
                                            TimeUnixPrinted = 0,
                                        };
                                        Globalvariable.Add_Content_To_SQLite_Queue.Enqueue((searchQR, newProductData));
                                        LogUpdate($"Mã này chưa Kích hoạt hôm nay - {searchQR} - Thêm vào hàng chờ kích hoạt");
                                        return;
                                    }
                                }
                                else
                                {
                                    LogUpdate($"Mã không bao gồm mã sản phẩm đang sản xuất - {searchQR}");
                                    return;
                                }
                            }
                            else
                            {
                                LogUpdate($"Mã không đúng định dạng: {searchQR} - {_checkFormat.Message}");
                                return;
                            }
                        }
                        else
                        {
                            LogUpdate($"Mã không đúng định dạng: {searchQR} - {_checkFormat.Message}");
                            return;
                        }
                    }
                }
                else
                {
                    var _checkFormat = CheckCodeFormat(searchQR);
                    if (_checkFormat.IsOK)
                    {
                        var _gdtf = GetDatabaseFilesByMonth(ipDateSearch.Value.ToString("MM/yyyy"));

                        if (_gdtf.IsSuccess)
                        {
                            foreach (string file in _gdtf.FilesList)
                            {
                                var _sqr = Get_QR_From_SQLite(Path.Combine("Client_Database", ipDateSearch.Value.Year.ToString(), ipDateSearch.Value.Month.ToString("D2"), file), searchQR);
                                if (_sqr.Issucces)
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: TÌM THẤY MÃ {_sqr.PData.ProductID} - {_sqr.PData.ProductQR} - {_sqr.PData.Active} - {_sqr.PData.TimeStamp}");
                                        opCMD.SelectedIndex = opCMD.Items.Count - 1;
                                    }));
                                    _productData = _sqr.PData;
                                    return;
                                }
                                else
                                {
                                    //không làm gì cả
                                }


                            }
                            LogUpdate($"{DateTime.Now:HH:mm:ss}: Mã không tồn tại: {searchQR}");
                        }
                        else
                        {
                            LogUpdate($"{DateTime.Now:HH:mm:ss}: Không tìm thấy: {searchQR} - {_gdtf.Message}");
                            return;
                        }
                    }
                    else
                    {
                        LogUpdate($"Mã không đúng định dạng: {searchQR} - {_checkFormat.Message}");
                    }

                }


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
        public (bool Issucces, string message, ProductInfo PData) Get_QR_From_SQLite(string file, string code)
        {
            ProductInfo _prD = new ProductInfo();

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

        //Check cấu trúc mã

        public (bool IsOK, string Message) CheckCodeFormat(string code)
        {
            // Kiểm tra định dạng mã QR
            if (string.IsNullOrEmpty(code))
            {
                return (false, "Mã QR không được để trống.");
            }
            // i.tcx.com.vn/[N13]0A509[N5][SN8] : các ký tự trong ngoặc [] là các ký tự quy định ví dụ N13 là 13 số, S8 là 8 ký tự chữ, SN8 là 8 ký tự chữ và số, dự vào đây kiểm tra định dạng mã
            string pattern = @"i\.tcx\.com\.vn/.*\d{13}.*0A509.*\d{5}.*[a-zA-Z0-9]{8}";
            if (!System.Text.RegularExpressions.Regex.IsMatch(code, pattern))
            {
                return (false, "Sai định dạng chuỗi");
            }

            return (true, "Mã QR hợp lệ.");
        }
        public (bool IsOK, string Message) CheckCodeFormatV2(string code, string pattern)
        {
            // Kiểm tra định dạng mã QR
            if (string.IsNullOrEmpty(code))
            {
                return (false, "Mã QR không được để trống.");
            }
            // i.tcx.com.vn/[N13]0A509[N5][SN8] : các ký tự trong ngoặc [] là các ký tự quy định ví dụ N13 là 13 số, S8 là 8 ký tự chữ, SN8 là 8 ký tự chữ và số, dự vào đây kiểm tra định dạng mã
            if (!System.Text.RegularExpressions.Regex.IsMatch(code, pattern))
            {
                return (false, "Sai định dạng chuỗi");
            }

            return (true, "Mã QR hợp lệ.");
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

        private void opCMD_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog(opCMD.SelectedItem?.ToString() ?? "Không có thông tin nào được chọn.");
        }
    }
}
