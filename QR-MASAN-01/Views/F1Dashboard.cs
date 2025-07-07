using HslCommunication;
using MainClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpT;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using static MFI_Service.MFI_Service_Form;
using static QR_MASAN_01.ActiveLogs;
using static QR_MASAN_01.SystemLogs;

namespace QR_MASAN_01
{
    public partial class F1Dashboard : UIPage
    {
        public F1Dashboard()
        {
            InitializeComponent();
            WK_Process_AUData.RunWorkerAsync();
        }

        public void INIT()
        {
            try
            {
                // Khởi tạo các thành phần cần thiết
                WK_Server_check.RunWorkerAsync();
                WK_Update.RunWorkerAsync();
                WK_UI_CAM_Update.RunWorkerAsync();
                Camera.Connect();
                if (Setting.Current.Camera_Slot > 1)
                {
                    Camera_c.Connect();
                }
                PLC.PLC_IP = PLCAddress.Get("PLC_IP");
                PLC.PLC_PORT = Convert.ToInt32(PLCAddress.Get("PLC_PORT"));
                PLC.PLC_Ready_DM = PLCAddress.Get("PLC_Ready_DM");
                PLC.InitPLC();
            }
            catch (Exception ex)
            {
                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_ERROR, "Lỗi khởi tạo Dashboard", "System", ex.Message);
                SystemLogs.InsertToSQLite(systemLogs);

                // Hiển thị thông báo lỗi trên giao diện
                this.ShowErrorDialog("Lỗi khởi tạo Dashboard, Vui lòng tắt máy mở lại", ex.Message);
                //Hiện lên box
                LogUpdate($"Lỗi khởi tạo Dashboard: {ex.Message}");
                LogUpdate($"VUI LÒNG TẮT MÁY MỞ LẠI");
            }

        }

        MFI_Info _clientMFI = new MFI_Info();
        bool ClearPLC = false;

        public string dataBase_FileName = "";
        //lấy xem có thông tin phiên tạo mới hay không

        #region Các chương trình tạo dữ liệu MFI, đồng bộ với máy chủ

        List<string> MFIs = new List<string> { "Case_Barcode", "Product_Barcode", "Case_LOT", "Batch_Code", "Block_Size", "Case_Size", "Pallet_Size", "SanLuong", "Operator", "Pallet_QR_Type", "MFI_ID", "QRCode_Folder", "QRCode_FileName", "MFI_Status" };

        //Luồng chính xử lý sự kiện đồng bộ
        private void WK_Server_check_DoWork(object sender, DoWorkEventArgs e)
        {

            while (!WK_Server_check.CancellationPending)
            {
                try
                {
                    switch (Globalvariable.Data_Status)
                    {
                        //Trạng thái sẵn sàng bình thường
                        case e_Data_Status.READY:
                            Process_MFI_When_Ready();
                            break;
                        //Trạng thái xử lý khi có mã mới
                        case e_Data_Status.NEW:
                            Process_MFI_When_New();
                            break;
                        //Trạng thái khi mở phần mềm lần đầu tiên
                        case e_Data_Status.STARTUP:
                            Process_MFI_When_Startup();
                            break;
                        //Đây dữ liệu vào camera    
                        case e_Data_Status.PUSH:
                            //gửi mã vào dictionary
                            LogUpdate("S2_Đẩy dữ liệu ô nhớ camera");
                            Push_Data_To_Dic();

                            //nếu ở chế độ 2 camera thì đầy thêm 2 camera nữa

                            if (Setting.Current.Camera_Slot > 1)
                            {
                                LogUpdate("S2_Đẩy dữ liệu camera C1");
                                Push_Data_To_Dic_C1();
                                LogUpdate("S2_Đẩy dữ liệu camera C2");
                                Push_Data_To_Dic_C2();
                            }

                            //Chuyển sang trạng thái đẩy dữ liệu cho máy in
                            Globalvariable.Data_Status = e_Data_Status.PRINTER_PUSH;

                            break;
                        //Đẩy dữ liệu cho máy in
                        case e_Data_Status.PRINTER_PUSH:
                            //gửi thông tin qua máy in
                            if (Setting.Current.Printer_name != "NONE")
                            {
                                //Gửi thông tin lên global.
                                Globalvariable.QRCode_Folder = _clientMFI.Data_Content_Folder;
                                Globalvariable.QRCode_FileName = _clientMFI.Data_Content_Filename;
                                Globalvariable.Data_Status = e_Data_Status.PRINTER_PUSH;
                            }
                            else
                            {
                                this.Invoke(new Action(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Ở chế độ không in, bỏ qua máy in ");
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                }));
                                Globalvariable.Data_Status = e_Data_Status.PUSHOK;
                            }

                            break;
                        //Đẩy dữ liệu camera thành công
                        case e_Data_Status.PUSHOK:
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Dữ liệu máy QR số 1 hoàn tất ");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                            // Ghi log vào sys log
                            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_EVENT, "Dữ liệu máy QR số 1 hoàn tất", Globalvariable.CurrentUser.Username, JsonConvert.SerializeObject(_clientMFI));
                            //thêm vào Queue để ghi log
                            SystemLogs.LogQueue.Enqueue(systemLogs);

                            Globalvariable.Data_Status = e_Data_Status.READY;

                            break;
                        //Tạo mới mã QR
                        case e_Data_Status.CREATING:
                            //tạo file trước
                            if (File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename) || File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1) || File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2))
                            {
                                //lỗi nghiêm trọng nè, tức file này đã tồn tại

                                Globalvariable.Data_Status = e_Data_Status.UNKNOWN;
                                break;
                            }
                            //tạo file mới
                            Create_new_Database_SQLite_File();

                            // tạo mã không trùng
                            int count = 1_000_000;
                            //Nếu là chế độ tạo dữ liệu thì tạo 100 mã thôi, nếu không thì tạo 1 triệu mã
                            if (Setting.Current.App_Mode == "ADD_Data")
                            {
                                count = 100;
                            }

                            HashSet<string> uniqueCodes = GenerateUniqueRandomCodes(8, count);
                            Random random = new Random();

                            List<string> filedata = uniqueCodes.ToList();

                            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename};Version=3;"))
                            {
                                string sql = $@" INSERT INTO `QRContent`
                                      (ProductQR)
                              VALUES (@ProductQR);";
                                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                                {
                                    conn.Open();

                                    int totalItems = filedata.Count;
                                    int percentStep = totalItems / 20; // 20 x 5% = 100%
                                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                                    {
                                        this.ShowStatusForm(100, "Đang tạo dữ liệu mã QR", 0);

                                        for (var i = 0; i < totalItems; i++)
                                        {
                                            string[] DATELOT = _clientMFI.Case_LOT.Split("-");
                                            string CODE = "";

                                            CODE = $"i.tcx.com.vn/{_clientMFI.Product_Barcode}0A509{DATELOT[0]}{DATELOT[1]}5{filedata[i]}";
                                            cmd.Parameters.AddWithValue("@ProductQR", CODE);
                                            cmd.ExecuteNonQuery();


                                            if (i % percentStep == 0)
                                            {
                                                int progressValue = (int)((i / (float)totalItems) * 100);
                                                this.SetStatusFormDescription("Đang tạo mã QR chai" + "(" + i + ")");
                                                this.SetStatusFormStepIt(5);
                                            }
                                        }

                                        transaction.Commit();
                                    }
                                    conn.Close();
                                    this.HideStatusForm();
                                }
                            }

                            //sao chép 2 file dữ liệu C1 và C2 từ file chính

                            File.Copy(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename, _clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1, true);

                            File.Copy(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename, _clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2, true);

                            //Tạo xong dữ liệu
                            LogUpdate($"Tạo dữ liệu mã QR thành công, tổng số mã: {filedata.Count}");
                            //Chuyển sang trạng thái đẩy
                            Globalvariable.Data_Status = e_Data_Status.PUSH;

                            break;
                        //Trạng thái bất định
                        case e_Data_Status.UNKNOWN:
                            //Bật siêu biển cảnh báo lỗi
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogUpdate($"Lỗi trong quá trình xử lý: {ex.Message}");
                    SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_ERROR, "Lỗi trong quá trình xử lý MFI", "System", ex.Message);
                    //thêm vào Queue để ghi log
                    SystemLogs.LogQueue.Enqueue(systemLogs);
                }
                Thread.Sleep(5000); // Chờ 2 giây trước khi gửi request tiếp theo
            }
        }

        private void Process_MFI_When_Ready()
        {
            var _gfsv = GetMultipleKeys(MFIs);
            int retryCount = 0;
            if (_gfsv.IsSuccess)
            {
                if (Globalvariable.Server_Status != e_Server_Status.CONNECTED)
                {
                    Globalvariable.Server_Status = e_Server_Status.CONNECTED;
                    this.Invoke(new Action(() =>
                    {
                        opServerStatus.Text = "Kết nối";
                        opServerStatus.FillColor = Globalvariable.OK_Color;
                    }));
                }

                //kiểm tra xem thông tin có khác thông tin đang chạy hay không
                if ( _clientMFI.MFI_ID != _gfsv.Values["MFI_ID"].ToString() || _clientMFI.Batch_Code != _gfsv.Values["Batch_Code"].ToString() ||_clientMFI.Product_Barcode != _gfsv.Values["Product_Barcode"].ToString() )
                {
                    //lụm thông tin mới

                    //các thông tin vào Client_MFI
                    _clientMFI.Case_Barcode = _gfsv.Values["Case_Barcode"].ToString();
                    _clientMFI.Product_Barcode = _gfsv.Values["Product_Barcode"].ToString();
                    _clientMFI.Case_LOT = _gfsv.Values["Case_LOT"].ToString();
                    _clientMFI.Batch_Code = _gfsv.Values["Batch_Code"].ToString();
                    _clientMFI.Block_Size = _gfsv.Values["Block_Size"].ToString();
                    _clientMFI.Case_Size = _gfsv.Values["Case_Size"].ToString();
                    _clientMFI.Pallet_Size = _gfsv.Values["Pallet_Size"].ToString();
                    _clientMFI.SanLuong = _gfsv.Values["SanLuong"].ToString();
                    _clientMFI.Operator = _gfsv.Values["Operator"].ToString();
                    _clientMFI.Pallet_QR_Type = _gfsv.Values["Pallet_QR_Type"].ToString();
                    _clientMFI.MFI_ID = _gfsv.Values["MFI_ID"].ToString();

                    _clientMFI.Data_Content_Folder = $@"Client_Database/{_clientMFI.Case_LOT.Split("-")[2].ToString()}/{_clientMFI.Case_LOT.Split("-")[1].ToString()}/";

                    LogUpdate($"Thông tin sản xuất đã thay đổi, cập nhật lại thông tin mới từ máy chủ");
                    Globalvariable.Data_Status = e_Data_Status.NEW;
                }

            }
            else
            {
                retryCount++;
                if(retryCount > 5)
                {
                    retryCount = 0;
                    //cập nhật sys log
                    SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SERVER_ERROR, "Lỗi kết nối máy chủ", "System", _gfsv.Message);
                    //thêm vào Queue để ghi log
                    SystemLogs.LogQueue.Enqueue(systemLogs);
                }
                if (Globalvariable.Server_Status != e_Server_Status.DISCONNECTED)
                {
                    Globalvariable.Server_Status = e_Server_Status.DISCONNECTED;
                    this.Invoke(new Action(() =>
                    {
                        opServerStatus.Text = "Mất kết nối";
                    }));
                }
                LogUpdate($"Lỗi kết nối máy chủ: {_gfsv.Message}");
            }

            //cập nhật UI
            Update_MFI_HMI();
            //Gửi trạng thái máy ready lên máy chủ
            Push_MFI_To_Server("QR01Status", "1");
        }

        private void Process_MFI_When_New()
        {
            //ghi nhận full MFI mới vào log
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, "MFI mới được tạo", Globalvariable.CurrentUser.Username, JsonConvert.SerializeObject(_clientMFI));
            //thêm vào Queue để ghi log
            SystemLogs.LogQueue.Enqueue(systemLogs);

            //kiểm tra thư mục tồn tại hay chưa
            if (!Directory.Exists(_clientMFI.Data_Content_Filename))
            {
                Directory.CreateDirectory(_clientMFI.Data_Content_Folder);
            }

            _clientMFI.Data_Content_Filename = $"ProductCode_{_clientMFI.Case_LOT}_{_clientMFI.Batch_Code}_{_clientMFI.Product_Barcode}.db";

            //tiếp tục cho các file của các camera còn lại
            _clientMFI.Data_Content_Filename_C1 = $"ProductCode_{_clientMFI.Case_LOT}_{_clientMFI.Batch_Code}_{_clientMFI.Product_Barcode}_C1.db";
            _clientMFI.Data_Content_Filename_C2 = $"ProductCode_{_clientMFI.Case_LOT}_{_clientMFI.Batch_Code}_{_clientMFI.Product_Barcode}_C2.db";

            if (!File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename) || !File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1) || !File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2))
            {
                //Chưa có file nào tồn tại, chuyển qua chế độ tạo mới
                Globalvariable.Data_Status = e_Data_Status.CREATING;
            }
            else
            {

                //Đã có file tồn tại, chuyển qua chế độ đẩy dữ liệu
                Globalvariable.Data_Status = e_Data_Status.PUSH;
                LogUpdate($"Đã có dữ liệu sản xuất, chuyển sang chế độ đẩy dữ liệu");
            }
        }
        int SystemLogsCount = 1;
        int SystemLogsCount_2 = 1;
        private void Process_MFI_When_Startup()
        {
            LogUpdate("Giao diện màn hình máy QR Chai phiên bản 10.23.531");
            LogUpdate("VUI LÒNG CHỜ ĐẾN KHI HOÀN TẤT");

            //Lấy MFI ID từ máy chủ, lấy all không cần quan tâm thứ tự làm gì. Sau đó kiểm tra xem file dữ liệu đã tồn tại hay chưa, nếu chưa thì tạo mới. File dữ liệu tuân thủ quy định sau: BatchCode_Barcode.printerData

            var (isSuccess, message, values) = GetMultipleKeys(MFIs);

            if (isSuccess)
            {
                if (Convert.ToInt32(values["MFI_ID"].ToString()) > 0)
                {
                    LogUpdate($"S1_Lấy dữ liệu thành công: {message}");

                    //các thông tin vào Client_MFI
                    _clientMFI.Case_Barcode = values["Case_Barcode"].ToString();
                    _clientMFI.Product_Barcode = values["Product_Barcode"].ToString();
                    _clientMFI.Case_LOT = values["Case_LOT"].ToString();
                    _clientMFI.Batch_Code = values["Batch_Code"].ToString();
                    _clientMFI.Block_Size = values["Block_Size"].ToString();
                    _clientMFI.Case_Size = values["Case_Size"].ToString();
                    _clientMFI.Pallet_Size = values["Pallet_Size"].ToString();
                    _clientMFI.SanLuong = values["SanLuong"].ToString();
                    _clientMFI.Operator = values["Operator"].ToString();
                    _clientMFI.Pallet_QR_Type = values["Pallet_QR_Type"].ToString();
                    _clientMFI.MFI_ID = values["MFI_ID"].ToString();

                    _clientMFI.Data_Content_Folder = $@"Client_Database/{_clientMFI.Case_LOT.Split("-")[2].ToString()}/{_clientMFI.Case_LOT.Split("-")[1].ToString()}/";

                    //kiểm tra thư mục tồn tại hay chưa
                    if (!Directory.Exists(_clientMFI.Data_Content_Filename))
                    {
                        Directory.CreateDirectory(_clientMFI.Data_Content_Folder);
                    }

                    //kiểm tra và tạo file DB
                    //tạo file dữ liệu CaseQR_Date_Random file lưu theo ngày, table là số lô sản xuất

                    _clientMFI.Data_Content_Filename = $"ProductCode_{_clientMFI.Case_LOT}_{_clientMFI.Batch_Code}_{_clientMFI.Product_Barcode}.db";

                    //tiếp tục cho các file của các camera còn lại
                    _clientMFI.Data_Content_Filename_C1 = $"ProductCode_{_clientMFI.Case_LOT}_{_clientMFI.Batch_Code}_{_clientMFI.Product_Barcode}_C1.db";
                    _clientMFI.Data_Content_Filename_C2 = $"ProductCode_{_clientMFI.Case_LOT}_{_clientMFI.Batch_Code}_{_clientMFI.Product_Barcode}_C2.db";

                    if (!File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename) || !File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1) || !File.Exists(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2))
                    {
                        //Chưa có file nào tồn tại, chuyển qua chế độ tạo mới
                        Globalvariable.Data_Status = e_Data_Status.CREATING;

                        return;
                    }

                    //đủ file chuyển sang đẩy
                    Globalvariable.Data_Status = e_Data_Status.PUSH;
                    LogUpdate($"Đã có dữ liệu sản xuất, chuyển sang chế độ đẩy dữ liệu");
                }
                else
                {
                    //Không có MFI ID, tức là máy chủ chưa phản hồi hoặc chưa có dữ liệu
                    LogUpdate($"Máy chủ chưa phản hồi");
                    SystemLogsCount--;
                    if (SystemLogsCount <= 0)
                    {
                        SystemLogsCount = 10;
                        //ghi log vào sys log
                        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SERVER_ERROR, "Không có MFI ID", "System", "Máy chủ chưa phản hồi hoặc chưa có dữ liệu");
                        //thêm vào Queue để ghi log
                        SystemLogs.LogQueue.Enqueue(systemLogs);
                    }
                    
                }
            }
            else
            {

                LogUpdate($"Lỗi khi lấy dữ liệu từ máy chủ: {message}");
                SystemLogsCount_2--;
                if (SystemLogsCount_2 <= 0)
                {
                    SystemLogsCount_2 = 10;
                    //ghi log vào sys log
                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SERVER_ERROR, "Lỗi khi lấy dữ liệu từ máy chủ", "System", message);
                //thêm vào Queue để ghi log
                SystemLogs.LogQueue.Enqueue(systemLogs);
                }
                
            }
        }

        private void Create_new_Database_SQLite_File()
        {
            SQLiteConnection.CreateFile(_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename);

            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename};Version=3;"))
            {
                conn.Open();

                // Tạo bảng mẫu
                string a = $@"INSERT INTO `ProductDetails` (BatchCode ,CaseCode ,ProductCode ,BlockSize ,CaseSize ,PalletSize ,OperatorName ,TimeStamp ) 
                            VALUES ('{_clientMFI.Batch_Code}','{_clientMFI.Case_Barcode}','{_clientMFI.Product_Barcode}','{_clientMFI.Block_Size}','{_clientMFI.Case_Size}','{_clientMFI.Pallet_Size}','Operator','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');";
                string createTableQuery = $@"
                                    CREATE TABLE IF NOT EXISTS `QRContent` (
                                        ProductID INTEGER NOT NULL UNIQUE,
                                        ProductQR TEXT NOT NULL UNIQUE,
                                        Active INTEGER NOT NULL DEFAULT 0,
                                        TimestampActive TEXT NOT NULL DEFAULT 0,
                                        TimestampPrinted TEXT NOT NULL DEFAULT 0,
                                        TimestampRework TEXT NOT NULL DEFAULT 0,
                                        TimeUnixActive INTEGER NOT NULL DEFAULT 0,
                                        TimeUnixPrinted INTEGER NOT NULL DEFAULT 0,
                                        TimeUnixRework INTEGER NOT NULL DEFAULT 0,
                                        PRIMARY KEY(ProductID AUTOINCREMENT)
                                    );
                                    CREATE TABLE IF NOT EXISTS `ProductDetails` (
                                        ID	INTEGER NOT NULL UNIQUE,
                                        BatchCode	TEXT NOT NULL,
                                        CaseCode	TEXT NOT NULL,
                                        ProductCode	TEXT NOT NULL,
                                        BlockSize	TEXT NOT NULL,
                                        CaseSize	TEXT NOT NULL,
                                        PalletSize	TEXT NOT NULL,
                                        OperatorName TEXT NOT NULL,
                                        TimeStamp TEXT NOT NULL,
                                        PRIMARY KEY(ID AUTOINCREMENT)
                                    );
                                            {a}

                                            ";
                using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        //Chương trình tạo mã không trùng
        static HashSet<string> GenerateUniqueRandomCodes(int length, int count)
        {
            HashSet<string> codes = new HashSet<string>();

            // Kiểm tra xem số lượng yêu cầu có khả thi không
            long maxPossible = (long)Math.Pow(characters.Length, length);
            if (count > maxPossible)
            {
                throw new ArgumentException($"Không thể sinh {count} mã không trùng với độ dài {length} " +
                                          $"vì chỉ có tối đa {maxPossible} tổ hợp.");
            }

            while (codes.Count < count)
            {
                string code = GenerateRandomCode(length);
                codes.Add(code);
            }

            return codes;
        }

        private static readonly Random random = new Random();
        private static readonly char[] characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
            .ToCharArray();

        static string GenerateRandomCode(int length)
        {
            char[] code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = characters[random.Next(characters.Length)];
            }
            return new string(code);
        }

        public (bool IsSuccess, string Message) Push_MFI_To_Server(string k, string v)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var url = "http://localhost:3000/set";
                    // Gộp toàn bộ object thành JSON string
                    // Tạo payload cho key "MFI_Data"
                    var payload = new
                    {
                        key = k,
                        value = JsonConvert.DeserializeObject(v), // giữ dạng object, không double encode
                        ttl = 15000 // Không đặt thời gian sống, nếu cần có thể thay đổi
                    };

                    string finalJson = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(finalJson, Encoding.UTF8, "application/json");


                    // Gọi bất đồng bộ theo cách đồng bộ
                    HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        return (true, $"Đã gửi thành công: {result}");
                    }
                    else
                    {
                        return (false, $"Lỗi HTTP {(int)response.StatusCode}: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi phía máy chủ: {ex.Message}");
            }
        }

        public void Update_MFI_HMI()
        {
            // Cập nhật các trường thông tin MFI trên giao diện
            this.Invoke(new Action(() =>
            {
                opBarcode.Text = _clientMFI.Product_Barcode;
                opCaseBarcode.Text = _clientMFI.Case_Barcode;
                opDateM.Text = _clientMFI.Case_LOT;
                opBatch.Text = _clientMFI.Batch_Code;
            }));
        }

        public (bool IsSuccess, string Message, Dictionary<string, object> Values) GetMultipleKeys(List<string> keys)
        {
            var resultValues = new Dictionary<string, object>();

            if (keys == null || keys.Count == 0)
            {
                return (false, "Danh sách keys không hợp lệ hoặc rỗng.", resultValues);
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var url = "http://localhost:3000/get";
                    var requestBody = new { keys = keys };
                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        return (false, $"Lỗi server: {(int)response.StatusCode} {response.ReasonPhrase}", resultValues);
                    }

                    var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var resultObj = JsonConvert.DeserializeObject<GetMultipleKeysResponse>(responseBody);

                    if (resultObj?.values == null)
                    {
                        return (false, "Phản hồi không hợp lệ từ server (không có trường 'values').", resultValues);
                    }

                    resultValues = resultObj.values;
                    return (true, "Thành công", resultValues);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi gọi API /get: {ex.Message}", resultValues);
            }
        }

        //Đẩy mã mới tạo vào dic
        public void Push_Data_To_Dic()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
           // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
           //Đẩy vào dic chính
            string connectionString = $@"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Câu lệnh SQL để đọc một cột (ví dụ: cột 'Name')
                string query = $"SELECT * FROM `QRContent`;";

                // Sử dụng SQLiteDataAdapter để đổ dữ liệu vào DataTable
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }
                // Duyệt qua các hàng trong DataTable và thêm vào List<string>
                foreach (DataRow row in dataTable.Rows)
                {
                    // Đọc dữ liệu từ SQL Server
                    int ProductID = Convert.ToInt32(row["ProductID"]); // CaseID
                    string ProductQR = row["ProductQR"].ToString(); // CaseQR
                    int active = Convert.ToInt32(row["Active"]); // Active
                    string timeStampA = row["TimestampActive"].ToString(); ; 
                    string timeStampP = row["TimestampPrinted"].ToString(); ;
                    int timeUnixA = Convert.ToInt32(row["TimeUnixActive"]) ; 
                    int timeUnixP = Convert.ToInt32(row["TimeUnixPrinted"]); 


                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                    Globalvariable.Main_Content_Dictionary[ProductQR] = new ProductData
                    {
                        ProductID = ProductID,
                        Active = active,
                        TimeStampActive = timeStampA,
                        TimeStampPrinted = timeStampP,
                        TimeUnixActive = timeUnixA,
                        TimeUnixPrinted = timeUnixP

                    };
                    
                }

                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu C thành công, số lượng: {dataTable.Rows.Count} ");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                connection.Close();
            }
        }

        public void Push_Data_To_Dic_C1()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
            // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
            //Đẩy vào dic chính
            string connectionString = $@"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Câu lệnh SQL để đọc một cột (ví dụ: cột 'Name')
                string query = $"SELECT * FROM `QRContent`;";

                // Sử dụng SQLiteDataAdapter để đổ dữ liệu vào DataTable
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }

                // Duyệt qua các hàng trong DataTable và thêm vào List<string>
                foreach (DataRow row in dataTable.Rows)
                {
                    // Đọc dữ liệu từ SQL Server
                    int ProductID = Convert.ToInt32(row["ProductID"]); // CaseID
                    string ProductQR = row["ProductQR"].ToString(); // CaseQR
                    int active = Convert.ToInt32(row["Active"]); // Active
                    string timeStampA = row["TimestampActive"].ToString(); ;
                    string timeStampP = row["TimestampPrinted"].ToString(); ;
                    int timeUnixA = Convert.ToInt32(row["TimeUnixActive"]);
                    int timeUnixP = Convert.ToInt32(row["TimeUnixPrinted"]);


                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                    Globalvariable.Main_Content_Dictionary[ProductQR] = new ProductData
                    {
                        ProductID = ProductID,
                        Active = active,
                        TimeStampActive = timeStampA,
                        TimeStampPrinted = timeStampP,
                        TimeUnixActive = timeUnixA,
                        TimeUnixPrinted = timeUnixP

                    };
                }

                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu C1 thành công, số lượng: {dataTable.Rows.Count} ");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                connection.Close();
            }
        }

        public void Push_Data_To_Dic_C2()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
            // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
            //Đẩy vào dic chính
            string connectionString = $@"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Câu lệnh SQL để đọc một cột (ví dụ: cột 'Name')
                string query = $"SELECT * FROM `QRContent`;";

                // Sử dụng SQLiteDataAdapter để đổ dữ liệu vào DataTable
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }

                // Duyệt qua các hàng trong DataTable và thêm vào List<string>
                foreach (DataRow row in dataTable.Rows)
                {
                    // Đọc dữ liệu từ SQL Server
                    int ProductID = Convert.ToInt32(row["ProductID"]); // CaseID
                    string ProductQR = row["ProductQR"].ToString(); // CaseQR
                    int active = Convert.ToInt32(row["Active"]); // Active
                    string timeStampA = row["TimestampActive"].ToString(); ;
                    string timeStampP = row["TimestampPrinted"].ToString(); ;
                    int timeUnixA = Convert.ToInt32(row["TimeUnixActive"]);
                    int timeUnixP = Convert.ToInt32(row["TimeUnixPrinted"]);


                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                    Globalvariable.Main_Content_Dictionary[ProductQR] = new ProductData
                    {
                        ProductID = ProductID,
                        Active = active,
                        TimeStampActive = timeStampA,
                        TimeStampPrinted = timeStampP,
                        TimeUnixActive = timeUnixA,
                        TimeUnixPrinted = timeUnixP

                    };
                }

                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu C2 thành công, số lượng: {dataTable.Rows.Count} ");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                connection.Close();
            }
        }
        #endregion

        #region Các cập nhật lên màn hình
        //Gửi lên màn hình và lưu log
        public void LogUpdate(string message)
        {
            this.Invoke(new Action(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: {message}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            }));
        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Update.CancellationPending)
            {
                if (Globalvariable.WB_Color == Globalvariable.OK_Color)
                {
                    Globalvariable.WB_Color = Globalvariable.NG_Color;
                }
                else
                {
                    Globalvariable.WB_Color = Globalvariable.OK_Color;
                }

                if (GCamera.Camera_Status == e_Camera_Status.DISCONNECTED)
                {
                    opCamera.FillColor = Globalvariable.WB_Color;
                }



                if (!Globalvariable.PLCConnect)
                {
                    opPLCStatus.FillColor = Globalvariable.WB_Color;
                }

                if (Globalvariable.Server_Status == e_Server_Status.DISCONNECTED)
                {
                    opServerStatus.FillColor = Globalvariable.WB_Color;
                }

                //chế độ thả lại
                if (Globalvariable.ISRerun)
                {
                    swModeData.Active = true;
                }
                else
                {
                    swModeData.Active = false;
                }

                //chế độ dữ liệu mới cũ

                bool printers = false;

                if (GPrinter.Printer_Status == e_PRINTER_Status.PRINTING)
                {
                    printers = true;
                }
                else
                {
                    printers = false;
                    if (Setting.Current.App_Mode == "ADD_Data")
                    {
                        printers = true;
                    }
                }

                if (Setting.Current.Camera_Slot == 1)
                {
                    GCamera.Camera_Status_02 = e_Camera_Status.CONNECTED;

                    this.Invoke(new Action(() =>
                    {
                        opCMR02Stt.Text = "Không dùng";
                        opCMR02Stt.FillColor = Color.Yellow;
                    }));
                }
                else
                {
                    if (GCamera.Camera_Status_02 == e_Camera_Status.DISCONNECTED)
                    {
                        opCMR02Stt.FillColor = Globalvariable.WB_Color;
                    }
                }
                //Ready
                if(Globalvariable.All_Ready)
                {
                    if (PLC.Ready != 1)
                    {
                        PLC.Ready = 1;
                    }
                }
                else
                {
                    if (PLC.Ready != 0)
                    {
                        PLC.Ready = 0;
                    }
                }
                if (GCamera.Camera_Status == e_Camera_Status.CONNECTED && GCamera.Camera_Status_02 == e_Camera_Status.CONNECTED && Globalvariable.Data_Status == e_Data_Status.READY && Globalvariable.PLCConnect && Globalvariable.setReady)
                {
                    if (!Globalvariable.FDashBoard_Ready)
                    {
                        Globalvariable.FDashBoard_Ready = true;
                    }
                }
                else
                {
                    if (Globalvariable.FDashBoard_Ready)
                    {
                        Globalvariable.FDashBoard_Ready = false;
                    }
                }

                if (APP.ByPass_Ready)
                {
                    PLC.Ready = 1;
                }
                
                if (ClearPLC)
                {
                    this.Invoke(new Action(() =>
                    {
                        btnClearPLC.Enabled = true;
                        btnClearPLC.Text = "Xóa lỗi PLC";
                        ClearPLC = false;
                    }));
                }
                //máy in
                if (Setting.Current.App_Mode == "U_PRINTER")
                {
                    switch (GPrinter.Printer_Status)
                    {
                        case e_PRINTER_Status.CONNECTED:
                            opPrinter.FillColor = Color.Yellow;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Kết nối";
                            }));
                            break;
                        case e_PRINTER_Status.DISCONNECTED:
                            opPrinter.FillColor = Globalvariable.WB_Color;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Mất kết nối";
                            }));
                            break;
                        case e_PRINTER_Status.PRINTING:
                            opPrinter.FillColor = Globalvariable.OK_Color;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Đang in";
                            }));
                            break;
                        case e_PRINTER_Status.JOB_CHANGE:
                            opPrinter.FillColor = Color.Yellow;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Đang chỉnh";
                            }));
                            break;
                        case e_PRINTER_Status.STOPPED:
                            opPrinter.FillColor = Color.Yellow;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Đang dừng";
                            }));
                            break;
                        case e_PRINTER_Status.INK_LOW:
                            opPrinter.FillColor = Globalvariable.WB_Color;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Mực thấp";
                            }));
                            break;
                        case e_PRINTER_Status.DATA_PRINTING:
                            opPrinter.FillColor = Color.Yellow;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Đang chỉnh";
                            }));
                            break;

                        case e_PRINTER_Status.DATA_EMPTY:
                            opPrinter.FillColor = Color.Red;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Lỗi dữ liệu";
                            }));
                            break;
                        case e_PRINTER_Status.UNKNOWN:
                            opPrinter.FillColor = Color.Red;
                            this.Invoke(new Action(() =>
                            {
                                opPrinter.Text = "Lỗi";
                            }));
                            break;
                    }
                }
                else
                {
                    opPrinter.FillColor = Color.Yellow;
                    this.Invoke(new Action(() =>
                    {
                        opPrinter.Text = "Không dùng";
                    }));
                }


                //Kiểm tra PLC_ACTIVE_DM nếu = 1 set Globale ACTIVE = true dùng hsl
                OperateResult<int> read = PLC.plc.ReadInt32(PLCAddress.Get("PLC_Bypass_DM_C1"));
                if (read.IsSuccess)
                {
                    if (read.Content != 1)
                    {
                        if (Globalvariable.ACTIVE_C1 == false)
                        {
                            //ghi log 
                            ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Bật Camera 01", "PLC", "Nhận kích hoạt camera 01 từ PLC, nhận giá trị khác 1");
                            //Ghi vào hàng chờ
                            ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C1 = true;
                        }
                    }
                    else
                    {
                        if (Globalvariable.ACTIVE_C1 == true)
                        {
                            //ghi log
                            ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Tắt Camera 01", "PLC", "Nhận ngừng kích hoạt camera 01 từ PLC, nhận giá trị bằng 1");
                            //Ghi vào hàng chờ
                            ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C1 = false;
                        }
                    }
                }

                OperateResult<int> read1 = PLC.plc.ReadInt32(PLCAddress.Get("PLC_Bypass_DM_C2"));
                if (read1.IsSuccess)
                {
                    if (read1.Content != 1)
                    {
                        if (Globalvariable.ACTIVE_C2 == false)
                        {
                            ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.UNACTIVE, "Bật Camera 02", "PLC", "Nhận kích hoạt camera 02 từ PLC, nhận giá trị khác 1");
                            //Ghi vào hàng chờ
                            ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C2 = true;
                        }
                    }
                    else
                    {
                        if(Globalvariable.ACTIVE_C2 == true)
                        {
                            ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.UNACTIVE, "Bật Camera 02", "PLC", "Nhận kích hoạt camera 02 từ PLC, nhận giá trị bằng 1");
                            //Ghi vào hàng chờ
                            ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C2 = false;
                        }
                        
                    }
                }

                if(Globalvariable.ACTIVE_C1 && Globalvariable.ACTIVE_C2)
                {
                    ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Kích hoạt kiểm", "PLC", "Nhận kích hoạt kiểm từ PLC");
                    //Ghi vào hàng chờ
                    ActiveLogQueue.Enqueue(activeLogs);
                    Globalvariable.ACTIVE = true;
                }
                else
                {
                    ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Dừng kiểm", "PLC", "Nhận kích hoạt từ PLC");
                    //Ghi vào hàng chờ
                    ActiveLogQueue.Enqueue(activeLogs);
                    Globalvariable.ACTIVE = false;
                }

                Thread.Sleep(1000);
            }
        }

        //Cập nhật mã vừa đọc lên màn hình
        private void WK_Update_Result_To_UI_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_UI_CAM_Update.CancellationPending)
            {
                this.Invoke((Action)(() => {
                    opContentC1.Text = Globalvariable.C1_UI.Curent_Content;
                    if (Globalvariable.C1_UI.IsPass)
                    {
                        opResultPassFailC1.Text = "TỐT";
                        opResultPassFailC1.FillColor = Color.Green;
                    }
                    else
                    {
                        opResultPassFailC1.Text = "LỖI";
                        opResultPassFailC1.FillColor = Color.Red;
                    }

                    opContentC2.Text = Globalvariable.C2_UI.Curent_Content;

                    if (Globalvariable.C2_UI.IsPass)
                    {
                        opResultPassFailC2.Text = "TỐT";
                        opResultPassFailC2.FillColor = Color.Green;
                    }
                    else
                    {
                        opResultPassFailC2.Text = "LỖI";
                        opResultPassFailC2.FillColor = Color.Red;
                    }


                    if (Alarm.Alarm1)
                    {
                        lblAlarm.Text = "CẢNH BÁO SAI BARCODE (" + Alarm.Alarm1_Count.ToString() + ")";
                        lblAlarm.FillColor = Globalvariable.NG_Color;
                    }
                }));
                Thread.Sleep(100);
            }
        }

        #endregion

        //Load lần đầu

        public string QR_Content = "Ver 18972";
        public string QR_Content_His = "";
        public string timeProcess = "0";
        public  int QRContentCount = 0;
        public bool ISPass = true;

        #region Xử lý tín hiệu từ camera

        private void Camera_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Sẵn sàng";
                            opCamera.FillColor = Globalvariable.OK_Color;
                        }));
                    }
                    break;
                case SPMS1.enumClient.DISCONNECTED:
                    if (GCamera.Camera_Status != e_Camera_Status.DISCONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.DISCONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Mất kết nối";
                        }));
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:

                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Sẵn sàng";
                            opCamera.FillColor = Globalvariable.OK_Color;
                        }));
                    }
                    if (Globalvariable.Data_Status == e_Data_Status.READY)
                    {
                        Globalvariable.GCounter.Total_C1++;
                        try
                        {
                            if (!WK_CMR1.IsBusy)
                            {
                                WK_CMR1.RunWorkerAsync(_strData);
                            }
                            else if (!WK_CMR2.IsBusy)
                            {
                                WK_CMR2.RunWorkerAsync(_strData);
                            }
                            else if (!WK_CMR3.IsBusy)
                            {
                                WK_CMR3.RunWorkerAsync(_strData);
                            }
                            else
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : Không đủ luồng xử lí");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;

                                //ghi log lỗi
                                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C1", Globalvariable.CurrentUser.Username, "Không đủ luồng xử lí");
                                Send_Result_Content_C1(e_Content_Result.ERROR, "Lỗi khi camera 02 trả về: Không đủ luồng xử lí");
                                //thêm vào Queue để ghi log
                                SystemLogs.LogQueue.Enqueue(systemLogs);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : {ex.Message}");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));

                            //ghi log lỗi
                            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C1", Globalvariable.CurrentUser.Username, ex.Message);
                            //thêm vào Queue để ghi log
                            SystemLogs.LogQueue.Enqueue(systemLogs);
                        }
                    }
                    else
                    {
                        LogUpdate("Máy chưa sẵn sàng");
                    }
                    
                    break;
                case SPMS1.enumClient.RECONNECT:
                    if (GCamera.Camera_Status != e_Camera_Status.RECONNECT)
                    {
                        GCamera.Camera_Status = e_Camera_Status.RECONNECT;

                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Kết nối lại";
                        }));
                    }
                    
                    
                    break;
            }
        }

        //camera 02
        private void Camera_c_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status_02 != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.CONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCMR02Stt.Text = "Sẵn sàng";
                            opCMR02Stt.FillColor = Globalvariable.OK_Color;
                        }));
                    }
                    break;
                case SPMS1.enumClient.DISCONNECTED:
                    if (GCamera.Camera_Status_02 != e_Camera_Status.DISCONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.DISCONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCMR02Stt.Text = "Mất kết nối";
                        }));
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:
                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Sẵn sàng";
                            opCamera.FillColor = Globalvariable.OK_Color;
                        }));
                    }
                    //xử lý dữ liệu nhận về
                    if (!WK_CMR4.IsBusy)
                    {
                        WK_CMR4.RunWorkerAsync(_strData);
                    }
                    else if (!WK_CMR5.IsBusy)
                    {
                        WK_CMR5.RunWorkerAsync(_strData);
                    }
                    else if (!WK_CMR6.IsBusy)
                    {
                        WK_CMR6.RunWorkerAsync(_strData);
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera 02 trả về : Không đủ luồng xử lí");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));

                        Send_Result_Content_C2(e_Content_Result.ERROR, "Lỗi khi camera 02 trả về: Không đủ luồng xử lí");

                        //ghi log lỗi
                        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C2", Globalvariable.CurrentUser.Username, "Không đủ luồng xử lí");
                        //thêm vào Queue để ghi log
                        SystemLogs.LogQueue.Enqueue(systemLogs);
                    }
                    break;
                case SPMS1.enumClient.RECONNECT:

                    Invoke(new Action(() =>
                    {
                        opCMR02Stt.Text = "Kết nối lại";
                        opCMR02Stt.FillColor = Color.Red;
                    }));

                    break;
            }
        }

        public void Camera_01_Data_Recive(string _strData)
        {
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Kiểm tra tính hợp lệ của dữ liệu
            if (!_strData.IsNullOrEmpty())
            {
                string codeClear = _strData.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");


                //bể này chỉ lưu thông tin nhằm so sánh sơ bộ
                if (Setting.Current.Camera_Slot > 1)
                {
                    //Kiểm tra mã đã kích hoạt hay chưa
                    if (Globalvariable.C1_Content_Dictionary.TryGetValue(codeClear, out ProductData C1ProductInfo))
                    {
                        if (C1ProductInfo.Active != 1)
                        {
                            C1ProductInfo.Active = 1;
                            C1ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //chuyển từ chuỗi int time  thành string time
                            C1ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            //lấy giờ hiện tại đếm từ 1970
                            C1ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            C1ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;

                        }
                    }

                    //chưa kích hoạt
                    else
                    {
                        if (Setting.Current.App_Mode == "ADD_Data")
                        {
                            //nếu chưa có thì thêm mới vào C1
                            C1ProductInfo = new ProductData
                            {
                                ProductID = 0,
                                Active = 1,
                                TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                                TimeUnixPrinted = Globalvariable.TimeUnixPrinter
                            };
                            Globalvariable.C1_Content_Dictionary[codeClear] = C1ProductInfo;
                            Globalvariable.C1_Add_Content_To_SQLite_Queue.Enqueue((codeClear, C1ProductInfo));
                        }
                        else
                        {
                            //không làm gì cả
                        }

                    }
                }

                ////////////////////////////////////////
                //kiểm tra xem pass hay fail
                //Fail
                if (codeClear.Contains("FAIL"))
                {
                    //sút
                    Send_Result_Content_C1(e_Content_Result.FAIL, codeClear);
                    return;
                }
                //mã sai cấu trúc
                if (!CheckCodeFormatV2(codeClear, Setting.Current.Code_Content_Pattern).IsOK)
                {
                    //sút
                    Send_Result_Content_C1(e_Content_Result.ERR_FORMAT, codeClear);
                    return;
                }
                //Kiểm tra mã
                if (Globalvariable.Main_Content_Dictionary.TryGetValue(codeClear, out ProductData ProductInfo))
                {
                    //mã đã active
                    if (ProductInfo.Active == 1 && !swModeData.Active)
                    {
                        Send_Result_Content_C1(e_Content_Result.DUPLICATE, codeClear);
                        return;
                    }

                    //mã đã active nhưng ở chế độ thả lại
                    if (ProductInfo.Active == 1 && swModeData.Active)
                    {
                        //loại trùng tắt => chế độ thả lại
                        Send_Result_Content_C1(e_Content_Result.REWORK, codeClear);
                        return;
                    }
                    

                    //cập nhật lại thông tin
                    ProductInfo.Active = 1;
                    ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;
                    //cập nhật SQLite
                    Globalvariable.Update_Content_To_SQLite_Queue.Enqueue(ProductInfo);
                    Send_Result_Content_C1(e_Content_Result.PASS, codeClear);
                    return;

                }
                //nếu mã chưa tồn tại => chưa active
                else
                {
                    //nếu khác mode thêm thì đá ra
                    if (Setting.Current.App_Mode != "ADD_Data")
                    {
                        Send_Result_Content_C1(e_Content_Result.NOT_FOUND, codeClear);
                        return;
                    }

                    //thêm vào Dic
                    ProductInfo = new ProductData
                    {
                        ProductID = 0,
                        Active = 1,
                        TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    };

                    Globalvariable.Main_Content_Dictionary[codeClear] = ProductInfo;
                    //thêm vào hàng đợi để thêm vào SQLite
                    Globalvariable.Add_Content_To_SQLite_Queue.Enqueue((codeClear,ProductInfo));
                    //báo pass xuống PLC
                    Send_Result_Content_C1(e_Content_Result.PASS, codeClear);
                    return;
                }
               
            }
            //Mã rỗng
            else
            {
                Send_Result_Content_C1(e_Content_Result.EMPTY, "MÃ RỖNG");
                return;
            }
        }

        public void Camera_02_Data_Recive(string _strData)
        {
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Kiểm tra tính hợp lệ của dữ liệu
            if (!_strData.IsNullOrEmpty())
            {
                string codeClear = _strData.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                //Kiểm tra trong bể chung

                if(Globalvariable.Main_Content_Dictionary.TryGetValue(codeClear, out ProductData ProductInfo))
                {
                    //nếu đã kích hoạt thì không làm gì cả
                    if (ProductInfo.Active != 1)
                    {
                        //chưa kích hoạt thì cập nhật lại thông tin
                        ProductInfo.Active = 1;
                        ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                        ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;
                        //cập nhật SQLite
                        Globalvariable.Update_Content_To_SQLite_Queue.Enqueue(ProductInfo);
                    }
                }
                //nếu chưa tồn tại
                else
                {
                    if (Setting.Current.App_Mode == "ADD_Data")
                    {
                        //nếu chưa có thì thêm mới vào C1
                        ProductInfo = new ProductData
                        {
                            ProductID = 0,
                            Active = 1,
                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                        };
                        Globalvariable.Main_Content_Dictionary[codeClear] = ProductInfo;
                        Globalvariable.Add_Content_To_SQLite_Queue.Enqueue((codeClear, ProductInfo));
                    }
                }
                //không đọc được
                if (codeClear.Contains("FAIL"))
                {
                    //sút
                    Send_Result_Content_C2(e_Content_Result.FAIL, codeClear);
                    return;
                }

                //sai cấu trúc
                if (!CheckCodeFormatV2(codeClear, Setting.Current.Code_Content_Pattern).IsOK)
                {
                    //sút
                    Send_Result_Content_C2(e_Content_Result.ERR_FORMAT, codeClear);
                    return;
                }

                //Kiểm tra trong bể C2
                if (Globalvariable.C2_Content_Dictionary.TryGetValue(codeClear, out ProductData C2ProductInfo))
                {
                    //chế độ loại trùng bật => kiểm tra active
                    if (C2ProductInfo.Active == 1 && !swModeData.Active)
                    {
                        Send_Result_Content_C2(e_Content_Result.DUPLICATE, codeClear);
                        return;
                    }

                    //chế độ loại trùng tắt => kiểm tra active
                    if (C2ProductInfo.Active == 1 && swModeData.Active)
                    {
                        //loại trùng tắt => chế độ thả lại
                        Send_Result_Content_C2(e_Content_Result.REWORK, codeClear);
                        return;
                    }

                    //mã chưa active thì active

                    //cập nhật SQLite
                    Globalvariable.C2_Update_Content_To_SQLite_Queue.Enqueue(C2ProductInfo);

                    //cập nhật lại thông tin
                    C2ProductInfo.Active = 1;
                    C2ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    C2ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    C2ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    C2ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;


                    //mã chưa active => active
                    Send_Result_Content_C2(e_Content_Result.PASS, codeClear);
                    return;
                }

                //chưa có mã
                else
                {
                    if (Setting.Current.App_Mode != "ADD_Data")
                    {
                        Send_Result_Content_C2(e_Content_Result.NOT_FOUND, codeClear);
                        return;
                    }

                    //thêm vào Dic
                    C2ProductInfo = new ProductData
                    {
                        ProductID = 0,
                        Active = 1,
                        TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                    };
                    Globalvariable.C2_Content_Dictionary[codeClear] = C2ProductInfo;
                    //thêm vào hàng đợi để thêm vào SQLite
                    Globalvariable.C2_Add_Content_To_SQLite_Queue.Enqueue((codeClear, C2ProductInfo));
                    //báo pass xuống PLC
                    Send_Result_Content_C2(e_Content_Result.PASS, codeClear);
                    return;
                }
            }
            //Mã rỗng
            else
            {
                //mã không đúng, in ra màn hình thông báo

                Send_Result_Content_C2(e_Content_Result.EMPTY, "MÃ RỖNG");
            }

        }

        //chương trình xử lý dữ liệu camera 01 khi có dữ liệu cho trước
        //Kiểm tra Pass/Fail.
        //Không tồn tại => loại
        //Đã kích hoạt => loại trừ nếu ở chế độ thả lại

        public void C1_Data_Process(string _strData)
        {
            //Xử lý dữ liệu nhanh nhất có thể
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //kiểm tra chuỗi có hợp lệ hay không
            //Kiểm tra tính hợp lệ của dữ liệu
            if (_strData.IsNullOrEmpty())
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C1(e_Content_Result.EMPTY, "MÃ RỖNG");
                return;
            }
            if (_strData == "FAIL")
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C1(e_Content_Result.FAIL, "Không đọc được");
                return;
            }
            //kiểm tra chuỗi có tồn tại trong bể dữ liệu chính hay không
            if (Globalvariable.C1_Content_Dictionary.TryGetValue(_strData, out ProductData C1ProductInfo))
            {
                //nếu chưa kích hoạt thì kích hoạt
                if (C1ProductInfo.Active != 1)
                {
                    C1ProductInfo.Active = 1;
                    C1ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    C1ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    C1ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    C1ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;

                    //Gửi vào hàng chờ để cập nhật SQLite
                    Globalvariable.C1_Update_Content_To_SQLite_Queue.Enqueue(C1ProductInfo);
                }
                //nếu đã kích hoạt thì đá ra
                else
                {
                    //đá ra
                    Send_Result_Content_C1(e_Content_Result.DUPLICATE, _strData);
                    return;
                }
            }
            //nếu không tồn tại thì đá ra, không cần quan tâm thêm
            else {
                Send_Result_Content_C1(e_Content_Result.NOT_FOUND, _strData);
                return;
            }
        }

        //Camera 2 tương tự camera 01
        public void C2_Data_Process(string _strData)
        {
            //Xử lý dữ liệu nhanh nhất có thể
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //kiểm tra chuỗi có hợp lệ hay không
            //Kiểm tra tính hợp lệ của dữ liệu
            if (_strData.IsNullOrEmpty())
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C2(e_Content_Result.EMPTY, "MÃ RỖNG");
                return;
            }
            if (_strData == "FAIL")
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C2(e_Content_Result.FAIL, "Không đọc được");
                return;
            }
            //kiểm tra chuỗi có tồn tại trong bể dữ liệu chính hay không
            if (Globalvariable.C2_Content_Dictionary.TryGetValue(_strData, out ProductData C2ProductInfo))
            {
                //nếu chưa kích hoạt thì kích hoạt
                if (C2ProductInfo.Active != 1)
                {
                    C2ProductInfo.Active = 1;
                    C2ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    C2ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    C2ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    C2ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;

                    //Gửi vào hàng chờ để cập nhật SQLite
                    Globalvariable.C2_Update_Content_To_SQLite_Queue.Enqueue(C2ProductInfo);
                }
                //nếu đã kích hoạt thì đá ra
                else
                {
                    //đá ra
                    Send_Result_Content_C2(e_Content_Result.DUPLICATE, _strData);
                    return;
                }
            }
            //nếu không tồn tại thì đá ra, không cần quan tâm thêm
            else
            {
                Send_Result_Content_C1(e_Content_Result.NOT_FOUND, _strData);
                return;
            }
        }

        #region Quản lý PLC và gửi tín hiệu PLC


        public enum e_Content_Result
        {
            PASS,//tốt
            FAIL, //lỗi
            REWORK, //thả lại
            DUPLICATE, //trùng
            EMPTY,//không có
            ERR_FORMAT, //lỗi định dạng
            NOT_FOUND, //không tìm thấy mã
            ERROR //lỗi không xác định
        }

        public void Send_Result_Content_C1(e_Content_Result content_Result, string _content)
        {
            switch (content_Result)
            {
                case e_Content_Result.PASS:

                    Globalvariable.GCounter.Total_Pass_C1++;
                    Globalvariable.C1_UI.Curent_Content = _content;
                    Globalvariable.C1_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("1"));
                    if (write.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C1++;
                    }
                    break;

                case e_Content_Result.FAIL:

                    Globalvariable.GCounter.Camera_Read_Fail_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;

                    Globalvariable.C1_UI.Curent_Content = "Không đọc được";
                    Globalvariable.C1_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write1 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));

                    if (write1.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }

                    break;

                case e_Content_Result.REWORK:

                    Globalvariable.GCounter.Rework_C1++; //Cái này không cộng vào số pass nếu phát hiện trùng

                    Globalvariable.C1_UI.Curent_Content = "Thả lại:" + _content;
                    Globalvariable.C1_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write5 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("1"));
                    if (write5.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C1++;
                    }
                    break;

                case e_Content_Result.DUPLICATE:

                    Globalvariable.GCounter.Duplicate_C1++;
                    Globalvariable.GCounter.Total_Failed_C1 += 1;

                    Globalvariable.C1_UI.Curent_Content = "Mã đã kích hoạt (trùng)";
                    Globalvariable.C1_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write4 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write4.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.EMPTY:

                    Globalvariable.GCounter.Empty_C1++;
                    Globalvariable.GCounter.Total_Failed_C1 += 1;

                    Globalvariable.C1_UI.Curent_Content = _content;
                    Globalvariable.C1_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write3 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write3.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.ERR_FORMAT:

                    Globalvariable.GCounter.Format_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;

                    Globalvariable.C1_UI.Curent_Content = "Sai cấu trúc!!!";
                    Globalvariable.C1_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write2 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write2.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.NOT_FOUND:

                    Globalvariable.GCounter.Empty_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;
                    Globalvariable.C1_UI.Curent_Content = "Mã không tồn tại";
                    Globalvariable.C1_UI.IsPass = false;
                    OperateResult write8 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write8.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.ERROR:
                    Globalvariable.C1_UI.Curent_Content = "Lỗi không xác định";
                    Globalvariable.C1_UI.IsPass = false;
                    Globalvariable.GCounter.Error_C1++;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write6 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C1"), short.Parse("0"));
                    if (write6.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;
            }
        }

        public void Send_Result_Content_C2(e_Content_Result content_Result, string _content)
        {
            switch (content_Result)
            {
                case e_Content_Result.PASS:
                    Globalvariable.GCounter.Total_Pass_C2++;
                    Globalvariable.C2_UI.Curent_Content = _content;
                    Globalvariable.C2_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("1"));
                    if (write.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C2++;
                    }
                    break;

                case e_Content_Result.FAIL:

                    Globalvariable.C2_UI.Curent_Content = "Không đọc được";
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Camera_Read_Fail_C2++;
                    Globalvariable.GCounter.Total_Failed_C2++;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write1 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));

                    if (write1.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }

                    break;

                case e_Content_Result.REWORK:

                    Globalvariable.C2_UI.Curent_Content = "Thả lại:" + _content;
                    Globalvariable.C2_UI.IsPass = true;
                    Globalvariable.GCounter.Rework_C2++; //Cái này không cộng vào số pass nếu phát hiện trùng
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write5 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("1"));
                    if (write5.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C2++;
                    }
                    break;

                case e_Content_Result.DUPLICATE:
                    Globalvariable.C2_UI.Curent_Content = "Mã đã kích hoạt (trùng)";
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Duplicate_C2++;
                    Globalvariable.GCounter.Total_Failed_C2 += 1;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write4 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));
                    if (write4.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;

                case e_Content_Result.EMPTY:
                    Globalvariable.C2_UI.Curent_Content = _content;
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Empty_C2++;
                    Globalvariable.GCounter.Total_Failed_C2 += 1;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write3 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));
                    if (write3.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;

                case e_Content_Result.ERR_FORMAT:

                    Globalvariable.C2_UI.Curent_Content = "Sai cấu trúc!!!";
                    Globalvariable.C2_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write2 = PLC.plc.Write(PLCAddress.Get("PLC_C2_RejectDM"), short.Parse("0"));
                    if (write2.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;

                case e_Content_Result.NOT_FOUND:

                    Globalvariable.C2_UI.Curent_Content = "Mã không tồn tại";
                    Globalvariable.C2_UI.IsPass = false;
                    OperateResult write8 = PLC.plc.Write(PLCAddress.Get("PLC_C2_RejectDM"), short.Parse("0"));
                    if (write8.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;

                case e_Content_Result.ERROR:
                    Globalvariable.C2_UI.Curent_Content = "Lỗi không xác định";
                    Globalvariable.C2_UI.IsPass = false;
                    Globalvariable.GCounter.Error_C2++;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write6 = PLC.plc.Write(PLCAddress.Get("PLC_Reject_DM_C2"), short.Parse("0"));
                    if (write6.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;
            }
        }

        private void PLC_PLCStatus_OnChange(object sender, SPMS1.OmronPLC_Hsl.PLCStatusEventArgs e)
        {
            switch (e.Status)
            {
                case SPMS1.OmronPLC_Hsl.PLCStatus.Connecting:
                    Globalvariable.PLCConnect = true;
                    this.Invoke(new Action(() =>
                    {
                        opPLCStatus.Text = "Kết nối";
                        opPLCStatus.FillColor = Globalvariable.OK_Color;
                    }));
                    break;
                case SPMS1.OmronPLC_Hsl.PLCStatus.Disconnect:
                    Globalvariable.PLCConnect = false;
                    this.Invoke(new Action(() =>
                    {
                        opPLCStatus.Text = "Mất kết nối";
                        opPLCStatus.FillColor = Globalvariable.NG_Color;
                    }));
                    break;
            }
        }
        #endregion


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
                return (false, "Sai định dạng");
            }

            return (true, "Mã QR hợp lệ.");
        }

        public void Update_Active_Status_Main(ProductData _productInfo)
        {
            
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `QRContent` SET " +
                               "`Active` = '1', " +
                               "`TimeStampActive` = @TimeStampActive, " +
                               "`TimeUnixActive` = @TimeUnixActive  " +
                               "`TimeStampPrinted` = @TimestampPrinted" +
                               "`TimeUnixPrinted` = @TimeUnixPrinted" +
                               "WHERE _rowid_ = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", _productInfo.ProductID);
                    command.Parameters.AddWithValue("@TimeStampActive", _productInfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnixActive",_productInfo.TimeUnixActive);

                    command.Parameters.AddWithValue("@TimeStampPrinted", _productInfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _productInfo.TimeUnixPrinted);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
        public void Add_Content_To_SQLite_Main((string Content, ProductData _productinfo) _Queue)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO `QRContent` " +
                            "(ProductQR, Active, TimestampActive, TimeUnixActive, TimestampPrinted, TimeUnixPrinted) " +
                    "VALUES (@QR,1,@TimeStamp,@TimeUnix,@TimestampPrinted, @TimeUnixPrinted);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QR", _Queue.Content);
                    command.Parameters.AddWithValue("@TimeStamp", _Queue._productinfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnix", _Queue._productinfo.TimeUnixActive);
                    command.Parameters.AddWithValue("@TimestampPrinted", _Queue._productinfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _Queue._productinfo.TimeUnixPrinted);

                    int rowsAffected = command.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT last_insert_rowid();", connection))
                {
                    long id = (long)cmd.ExecuteScalar();

                    //cập nhật vào Globalvariable.Main_Content_Dictionary
                    if (Globalvariable.Main_Content_Dictionary.TryGetValue(_Queue.Content, out ProductData productData))
                    {
                        // Nếu đã có trong dictionary, cập nhật ProductID
                        productData.ProductID = Convert.ToInt32(id);
                    }
                    else
                    {
                        // Nếu chưa có, thêm mới vào dictionary
                        Globalvariable.Main_Content_Dictionary[_Queue.Content] = new ProductData
                        {
                            ProductID = Convert.ToInt32(id),
                            Active = 1,
                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                        };
                    }
                }
            }
        }

        public void Update_Active_Status_C1(ProductData _productInfo)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `QRContent` SET " +
                               "`Active` = '1', " +
                               "`TimeStampActive` = @TimeStampActive, " +
                               "`TimeUnixActive` = @TimeUnixActive  " +
                               "`TimeStampPrinted` = @TimestampPrinted" +
                               "`TimeUnixPrinted` = @TimeUnixPrinted" +
                               "WHERE _rowid_ = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", _productInfo.ProductID);
                    command.Parameters.AddWithValue("@TimeStampActive", _productInfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnixActive", _productInfo.TimeUnixActive);

                    command.Parameters.AddWithValue("@TimeStampPrinted", _productInfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _productInfo.TimeUnixPrinted);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
        public void Add_Content_To_SQLite_C1((string Content, ProductData _productinfo) _Queue)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO `QRContent` " +
                            "(ProductQR, Active, TimestampActive, TimeUnixActive, TimestampPrinted, TimeUnixPrinted) " +
                    "VALUES (@QR,1,@TimeStamp,@TimeUnix,@TimestampPrinted, @TimeUnixPrinted);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QR", _Queue.Content);
                    command.Parameters.AddWithValue("@TimeStamp", _Queue._productinfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnix", _Queue._productinfo.TimeUnixActive);
                    command.Parameters.AddWithValue("@TimestampPrinted", _Queue._productinfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _Queue._productinfo.TimeUnixPrinted);

                    int rowsAffected = command.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT last_insert_rowid();", connection))
                {
                    long id = (long)cmd.ExecuteScalar();

                    //cập nhật vào Globalvariable.Main_Content_Dictionary
                    if (Globalvariable.C1_Content_Dictionary.TryGetValue(_Queue.Content, out ProductData productData))
                    {
                        // Nếu đã có trong dictionary, cập nhật ProductID
                        productData.ProductID = Convert.ToInt32(id);
                    }
                    else
                    {
                        // Nếu chưa có, thêm mới vào dictionary
                        Globalvariable.C1_Content_Dictionary[_Queue.Content] = new ProductData
                        {
                            ProductID = Convert.ToInt32(id),
                            Active = 1,
                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                        };
                    }
                }
            }
        }

        public void Update_Active_Status_C2(ProductData _productInfo)
        {

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `QRContent` SET " +
                               "`Active` = '1', " +
                               "`TimeStampActive` = @TimeStampActive, " +
                               "`TimeUnixActive` = @TimeUnixActive  " +
                               "`TimeStampPrinted` = @TimestampPrinted" +
                               "`TimeUnixPrinted` = @TimeUnixPrinted" +
                               "WHERE _rowid_ = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", _productInfo.ProductID);
                    command.Parameters.AddWithValue("@TimeStampActive", _productInfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnixActive", _productInfo.TimeUnixActive);

                    command.Parameters.AddWithValue("@TimeStampPrinted", _productInfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _productInfo.TimeUnixPrinted);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
        public void Add_Content_To_SQLite_C2((string Content, ProductData _productinfo) _Queue)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO `QRContent` " +
                            "(ProductQR, Active, TimestampActive, TimeUnixActive, TimestampPrinted, TimeUnixPrinted) " +
                    "VALUES (@QR,1,@TimeStamp,@TimeUnix,@TimestampPrinted, @TimeUnixPrinted);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QR", _Queue.Content);
                    command.Parameters.AddWithValue("@TimeStamp", _Queue._productinfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnix", _Queue._productinfo.TimeUnixActive);
                    command.Parameters.AddWithValue("@TimestampPrinted", _Queue._productinfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _Queue._productinfo.TimeUnixPrinted);

                    int rowsAffected = command.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT last_insert_rowid();", connection))
                {
                    long id = (long)cmd.ExecuteScalar();

                    //cập nhật vào Globalvariable.Main_Content_Dictionary
                    if (Globalvariable.C2_Content_Dictionary.TryGetValue(_Queue.Content, out ProductData productData))
                    {
                        // Nếu đã có trong dictionary, cập nhật ProductID
                        productData.ProductID = Convert.ToInt32(id);
                    }
                    else
                    {
                        // Nếu chưa có, thêm mới vào dictionary
                        Globalvariable.C2_Content_Dictionary[_Queue.Content] = new ProductData
                        {
                            ProductID = Convert.ToInt32(id),
                            Active = 1,
                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                        };
                    }
                }
            }
        }

        #endregion

        private void btnResetCounter_Click(object sender, EventArgs e)
        {

            OperateResult write = PLC.plc.Write("D22", short.Parse("1"));
            if (write.IsSuccess)
            {

                lblFail.Value = 0;
                lblPass.Value = 0;
                lblTotal.Value = 0;
            }
            else
            {

            }
        }

        private void btnClearCmd_Click(object sender, EventArgs e)
        {
            ipConsole.Items.Clear();
        }

        private void btnClearPLC_Click(object sender, EventArgs e)
        {
            btnClearPLC.Enabled = false;
            btnClearPLC.Text = "Đang xóa lỗi";
            OperateResult write = PLC.plc.Write("D18", short.Parse("1"));
            if (write.IsSuccess)
            {
                btnClearPLC.Enabled = true;

                btnClearPLC.Text = "Xóa lỗi PLC";

                Alarm.Alarm1 = false;
                Alarm.Alarm1_Count = 0;

                lblAlarm.Text = "-";
                lblAlarm.FillColor = Globalvariable.OK_Color;

            }
            else
            {
                ClearPLC = true;
                //btnClearPLC.Enabled = true;
            }
        }

        private void WK_Process_SQLite_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_UI_CAM_Update.CancellationPending)
            {
                
                if(Globalvariable.TimeUnixPrinter == 0)
                {
                    Globalvariable.TimeUnixPrinter = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                }
                //Bể chính

                //cập nhật
                if (Globalvariable.Update_Content_To_SQLite_Queue.Count > 0)
                {
                    Update_Active_Status_Main(Globalvariable.Update_Content_To_SQLite_Queue.Dequeue());
                }

                //thêm mới
                if(Globalvariable.Add_Content_To_SQLite_Queue.Count > 0)
                {
                    Add_Content_To_SQLite_Main(Globalvariable.Add_Content_To_SQLite_Queue.Dequeue());
                }

                //Bể C1
                //cập nhật
                if (Globalvariable.C1_Update_Content_To_SQLite_Queue.Count > 0)
                {
                    Update_Active_Status_C1(Globalvariable.C1_Update_Content_To_SQLite_Queue.Dequeue());
                }
                //thêm mới
                if (Globalvariable.C1_Add_Content_To_SQLite_Queue.Count > 0)
                {
                    Add_Content_To_SQLite_C1(Globalvariable.C1_Add_Content_To_SQLite_Queue.Dequeue());
                }
                //Bể C2
                //cập nhật
                if (Globalvariable.C2_Update_Content_To_SQLite_Queue.Count > 0)
                {
                    Update_Active_Status_C2(Globalvariable.C2_Update_Content_To_SQLite_Queue.Dequeue());
                }
                //thêm mới
                if (Globalvariable.C2_Add_Content_To_SQLite_Queue.Count > 0)
                {
                    Add_Content_To_SQLite_C2(Globalvariable.C2_Add_Content_To_SQLite_Queue.Dequeue());
                }
                Thread.Sleep(100);
            }
        }

        #region Các luồng xử lý dữ liệu khi có tín hiệu
        private double maxTimeT1 = 0;
        private double maxTimeT2 = 0;
        private double maxTimeT3 = 0;
        private double maxTimeT4 = 0;
        private double maxTimeT5 = 0;
        private double maxTimeT6 = 0;
        private void WK_CMR1_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            C1_Data_Process(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT1)
                {
                    maxTimeT1 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK1_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT1}" ;

        }
        private void WK_CMR2_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            //WhenDataRecive(inputString);
            //Camera_01_Data_Recive(inputString);
            C1_Data_Process(inputString);

            stopwatch.Stop();

            if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT2)
            {
                maxTimeT2 = stopwatch.Elapsed.TotalMilliseconds;
            }
            Globalvariable.GCounter.WK2_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT2}";

        }
        private void WK_CMR3_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            //WhenDataRecive(inputString);
            C1_Data_Process(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT3)
                {
                    maxTimeT3 = stopwatch.Elapsed.TotalMilliseconds;
                }
            Globalvariable.GCounter.WK3_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT3}";
        }
        

        private void WK_CMR4_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

           // Camera_02_Data_Recive(inputString);
           C2_Data_Process(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT4)
                {
                    maxTimeT4 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK4_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT4}";
            }
        private void WK_CMR5_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            
            C2_Data_Process(inputString);
            
            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT5)
                {
                    maxTimeT5 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK5_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT5}";
            }
        private void WK_CMR6_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            C2_Data_Process(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT6)
                {
                    maxTimeT6 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK5_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT5}";
            }

        #endregion

        private void uiTitlePanel5_Click(object sender, EventArgs e)
        {

        }

        private void swModeData_ValueChanged(object sender, bool value)
        {
            if (swModeData.Active)
            {
                Globalvariable.ISRerun = true;
            }
            else
            {
                Globalvariable.ISRerun = false;
            }
        }
        private void ipConsole_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog(ipConsole.SelectedItem as string);
        }

        public void SendUnActive()
        {
            OperateResult operateResult = PLC.plc.Write(PLCAddress.Get("PLC_Bypass_DM_C1"),int.Parse("1"));
            OperateResult operateResult2 = PLC.plc.Write(PLCAddress.Get("PLC_Bypass_DM_C2"), int.Parse("1"));
            //không cần đợi trả về làm gì
        }
    }
}