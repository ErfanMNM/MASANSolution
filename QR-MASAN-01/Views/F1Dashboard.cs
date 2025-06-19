using HslCommunication;
using MainClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Security.Policy;
using System.Text;
using System.Threading;
using static MFI_Service.MFI_Service_Form;

namespace QR_MASAN_01
{
    public partial class F1Dashboard : UIPage
    {
        public F1Dashboard()
        {
            InitializeComponent();
            WK_Process_AUData.RunWorkerAsync();
        }
        
        MFI_Info _clientMFI = new MFI_Info();
        bool ClearPLC = false;

        public string dataBase_FileName = "";
        //lấy xem có thông tin phiên tạo mới hay không

        #region Các chương trình tạo dữ liệu MFI, đồng bộ với máy chủ

        List<string> keys = new List<string> { "Case_Barcode", "Product_Barcode", "Case_LOT", "Batch_Code", "Block_Size", "Case_Size", "Pallet_Size", "SanLuong", "Operator", "Pallet_QR_Type", "MFI_ID", "QRCode_Folder", "QRCode_FileName", "MFI_Status" };

        //Luồng chính xử lý sự kiện đồng bộ
        private void WK_Server_check_DoWork(object sender, DoWorkEventArgs e)
        {

            while (!WK_Server_check.CancellationPending)
            {

                switch (Globalvariable.Data_Status)
                {
                    //Trạng thái sẵn sàng bình thường
                    case e_Data_Status.READY:
                        var _gfsv = GetMultipleKeys(keys);
                        if(_gfsv.IsSuccess)
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

                            _clientMFI.QRCode_Folder = $@"Client_Database/{_clientMFI.Case_LOT.Split("-")[2].ToString()}/{_clientMFI.Case_LOT.Split("-")[1].ToString()}/";

                            Globalvariable.ProductBarcode = _clientMFI.Product_Barcode;

                            if (Directory.Exists(_clientMFI.QRCode_Folder))
                            {

                            }
                            else
                            {
                                Directory.CreateDirectory(_clientMFI.QRCode_Folder);
                            }

                            //kiểm tra và tạo file DB
                            //tạo file dữ liệu CaseQR_Date_Random file lưu theo ngày, table là số lô sản xuất

                            _clientMFI.QRCode_FileName = $"ProductCode_{_clientMFI.Case_LOT}_{_clientMFI.Batch_Code}_{_clientMFI.Product_Barcode}.db";

                            //nếu chưa có
                            if (!File.Exists(_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName))
                            {
                                SQLiteConnection.CreateFile(_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName);
                                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName};Version=3;"))
                                {
                                    conn.Open();

                                    // Tạo bảng mẫu
                                    string a = $@"INSERT INTO `ProductDetails` (BatchCode ,CaseCode ,ProductCode ,BlockSize ,CaseSize ,PalletSize ,OperatorName ,TimeStamp ) 
                                  VALUES ('{_clientMFI.Batch_Code}','{_clientMFI.Case_Barcode}','{_clientMFI.Product_Barcode}','{_clientMFI.Block_Size}','{_clientMFI.Case_Size}','{_clientMFI.Pallet_Size}','Operator','{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}');";
                                    string createTableQuery = $@"
                                        CREATE TABLE IF NOT EXISTS `QRContent` (
                                            ProductID INTEGER NOT NULL UNIQUE,
                                            ProductQR TEXT NOT NULL UNIQUE,
                                            Active INTEGER NOT NULL DEFAULT 0,
                                            TimestampActive TEXT NOT NULL DEFAULT 0,
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

                                //tạo mã QR
                                Globalvariable.Data_Status = e_Data_Status.CREATING;

                            }
                            else
                            {
                                //nếu file đã có thì không làm gì cả//
                            }
                        }
                        else
                        {
                            if(Globalvariable.Server_Status != e_Server_Status.DISCONNECTED)
                            {
                                Globalvariable.Server_Status = e_Server_Status.DISCONNECTED;
                                this.Invoke(new Action(() =>
                                {
                                    opServerStatus.Text = "Mất kết nối";
                                }));
                            }
                            LogUpdate($"Lỗi kết nối máy chủ: {_gfsv.Message}");
                        }
                        Update_MFI_HMI();
                        Push_MFI_To_Server("QR01Status", "1");
                        break;
                    //Trạng thái khi mở phần mềm lần đầu tiên
                    case e_Data_Status.STARTUP:

                        LogUpdate("Giao diện màn hình máy QR Chai phiên bản 10.23.531");
                        LogUpdate("Khởi động chương trình đồng bộ dữ liệu");

                        //Lấy MFI ID từ máy chủ, lấy all không cần quan tâm thứ tự làm gì. Sau đó kiểm tra xem file dữ liệu đã tồn tại hay chưa, nếu chưa thì tạo mới. File dữ liệu tuân thủ quy định sau: BatchCode_Barcode.printerData
                       
                        var (isSuccess, message, values) = GetMultipleKeys(keys);

                        if (isSuccess)
                        {
                            if( Convert.ToInt32(values["MFI_ID"].ToString()) > 0 )
                            {
                                LogUpdate($"Lấy dữ liệu MFI thành công từ máy chủ: {message}");

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

                                _clientMFI.QRCode_Folder = $@"Client_Database/{_clientMFI.Case_LOT.Split("-")[2].ToString()}/{_clientMFI.Case_LOT.Split("-")[1].ToString()}/";



                                if (Directory.Exists(_clientMFI.QRCode_Folder))
                                {

                                }
                                else
                                {
                                    Directory.CreateDirectory(_clientMFI.QRCode_Folder);
                                }

                                //kiểm tra và tạo file DB
                                //tạo file dữ liệu CaseQR_Date_Random file lưu theo ngày, table là số lô sản xuất

                                _clientMFI.QRCode_FileName = $"ProductCode_{_clientMFI.Case_LOT}_{_clientMFI.Batch_Code}_{_clientMFI.Product_Barcode}.db";

                                //nếu chưa có
                                if (!File.Exists(_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName))
                                {
                                    SQLiteConnection.CreateFile(_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName);
                                    using (SQLiteConnection conn = new SQLiteConnection($"Data Source={_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName};Version=3;"))
                                    {
                                        conn.Open();

                                        // Tạo bảng mẫu
                                        string a = $@"INSERT INTO `ProductDetails` (BatchCode ,CaseCode ,ProductCode ,BlockSize ,CaseSize ,PalletSize ,OperatorName ,TimeStamp ) 
                                  VALUES ('{_clientMFI.Batch_Code}','{_clientMFI.Case_Barcode}','{_clientMFI.Product_Barcode}','{_clientMFI.Block_Size}','{_clientMFI.Case_Size}','{_clientMFI.Pallet_Size}','Operator','{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}');";
                                        string createTableQuery = $@"
                                        CREATE TABLE IF NOT EXISTS `QRContent` (
                                            ProductID INTEGER NOT NULL UNIQUE,
                                            ProductQR TEXT NOT NULL UNIQUE,
                                            Active INTEGER NOT NULL DEFAULT 0,
                                            TimestampActive TEXT NOT NULL DEFAULT 0,
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

                                    //tạo mã QR
                                    Globalvariable.Data_Status = e_Data_Status.CREATING;
                                }
                                else
                                {
                                    //nếu file đã có thì không làm gì cả, đẩy trạng thái lên push
                                    Globalvariable.Data_Status = e_Data_Status.PUSH;
                                }


                            }
                            else
                            {
                                
                                LogUpdate($"Máy chủ chưa phản hồi");
                            }
                        }
                        else
                        {
                            LogUpdate($"Lỗi khi lấy dữ liệu từ máy chủ: {message}");
                        }
                        break;
                    //Đây dữ liệu vào camera    
                    case e_Data_Status.PUSH:
                        //gửi mã vào dictionary
                        if (!WK_Push_Data_To_Dic.IsBusy)
                        {
                            LogUpdate("Đẩy dữ liệu ô nhớ camera");
                            WK_Push_Data_To_Dic.RunWorkerAsync();
                        }

                        break;
                    //Đẩy dữ liệu cho máy in
                    case e_Data_Status.PRINTER_PUSH:
                        //gửi thông tin qua máy in
                        if (GlobalSettings.Get("APPMODE") == "NO_ADD")
                        {
                            Globalvariable.QRCode_Folder = _clientMFI.QRCode_Folder;
                            Globalvariable.QRCode_FileName = _clientMFI.QRCode_FileName;
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
                        Globalvariable.Data_Status = e_Data_Status.READY;

                        break;
                    //Tạo mới mã QR
                    case e_Data_Status.CREATING:
                        // tạo mã không trùng
                        int count = 1_000_000;
                        if (GlobalSettings.Get("APPMODE") == "ADD_Data")
                        {
                            count = 100;
                        }
                    
                        HashSet<string> uniqueCodes = GenerateUniqueRandomCodes(8, count);
                        Random random = new Random();

                        List<string> filedata = uniqueCodes.ToList();
                        using (SQLiteConnection conn = new SQLiteConnection($"Data Source={_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName};Version=3;"))
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


                        //Tạo xong dữ liệu
                        LogUpdate($"Tạo dữ liệu mã QR thành công, tổng số mã: {filedata.Count}");
                        Globalvariable.Data_Status = e_Data_Status.PUSH;
                        break;
                    //Trạng thái bất định
                    case e_Data_Status.UNKNOWN:
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Có vấn đề xảy ra trong quá trình đẩy dữ liệu, vui lòng kiểm tra lại. ");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));

                        break;
                }

                Thread.Sleep(5000); // Chờ 2 giây trước khi gửi request tiếp theo
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
            string connectionString = $@"Data Source={_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName};Version=3;";

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
                Globalvariable.MaxID_Content = Convert.ToInt32(dataTable.Rows[dataTable.Rows.Count - 1]["ProductID"]);
                // Duyệt qua các hàng trong DataTable và thêm vào List<string>
                foreach (DataRow row in dataTable.Rows)
                {
                    // Đọc dữ liệu từ SQL Server
                    int ProductID = Convert.ToInt32(row["ProductID"]); // CaseID
                    string ProductQR = row["ProductQR"].ToString(); // CaseQR
                    int active = Convert.ToInt32(row["Active"]); // Active
                    string timeStamp = row["TimestampActive"].ToString(); ; // TimeStamp

                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                    Globalvariable.Main_Content_Dictionary[ProductQR] = new ProductData
                    {
                        ProductID = ProductID,
                        Active = active,
                        TimeStamp = timeStamp
                    };
                    
                    //nếu có 2 camera thì đổ thêm cho vui
                    if (GlobalSettings.GetInt("CAMERA_SLOT") > 1 )
                    {
                        Globalvariable.C1_Content_Dictionary[ProductQR] = new ProductData
                        {
                            ProductID = ProductID,
                            Active = active,
                            TimeStamp = timeStamp
                        };

                        Globalvariable.C2_Content_Dictionary[ProductQR] = new ProductData
                        {
                            ProductID = ProductID,
                            Active = active,
                            TimeStamp = timeStamp
                        };
                    }
                }

                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu thành công, số lượng: {dataTable.Rows.Count} ");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                connection.Close();
            }
        }
        private void WK_Push_Data_To_Dic_DoWork(object sender, DoWorkEventArgs e)
        {
            Push_Data_To_Dic();
        }
        private void WK_Push_Data_To_Dic_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            //đẩy dữ liệu vào máy in
            Globalvariable.Data_Status = e_Data_Status.PRINTER_PUSH;
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
        #endregion

        //Load lần đầu
        private void FDashboard_Initialize(object sender, EventArgs e)
        {
            WK_Server_check.RunWorkerAsync();
            WK_Update.RunWorkerAsync();
            WK_UI_CAM_Update.RunWorkerAsync();
            Camera.Connect();
            PLC.PLC_IP = GlobalSettings.Get("PLC_IP");
            PLC.PLC_PORT = GlobalSettings.GetInt("PLC_PORT");
            PLC.InitPLC();
        }

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

                    if(Globalvariable.Data_Status == e_Data_Status.READY)
                    {
                        Counter.Camera120Count++;
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
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : {ex.Message}");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
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

        double TimeSendPLC = 0;

        public void Camera_01_Data_Recive(string _strData)
        {
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Kiểm tra tính hợp lệ của dữ liệu
            if (!_strData.IsNullOrEmpty())
            {
                string codeClear = _strData.Replace("\r\n","").Replace("\r","").Replace("\n","");

                //kiểm tra xem pass hay fail
                if (codeClear.Contains("FAIL"))
                {
                    Globalvariable.C1_UI.Curent_Content = "Không đọc được";
                    Globalvariable.C1_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("0"));
                    if (write.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                }
                else
                {
                    //Kiểm tra cấu trúc mã
                    var _cContentR = CheckCodeFormatV2(codeClear, GlobalSettings.Get("Code_Content_Pattern"));

                    if (_cContentR.IsOK)
                    {
                        //kiểm tra barcode xem đúng hay không
                        if (codeClear.Split("/")[1].Contains(_clientMFI.Product_Barcode))
                        {
                            //Kiểm tra mã
                            if (Globalvariable.Main_Content_Dictionary.TryGetValue(codeClear, out ProductData ProductInfo))
                            {
                                //mã đã active
                                if (ProductInfo.Active == 1)
                                {
                                    //chế độ loại trùng bật => đá ra
                                    if (!swModeData.Active)
                                    {
                                        Globalvariable.C1_UI.Curent_Content = "Mã đã kích hoạt (trùng)";
                                        Globalvariable.C1_UI.IsPass = false;
                                        //gửi xuống PLC và xử lý tại đây
                                        OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("0"));
                                        if (write.IsSuccess)
                                        {
                                            Globalvariable.GCounter.PLC_0_Pass_C1++;
                                        }
                                        else
                                        {
                                            Globalvariable.GCounter.PLC_0_Fail_C1++;
                                        }
                                    }

                                    //loại trùng tắt => chế độ thả lại
                                    else
                                    {
                                        Globalvariable.C1_UI.Curent_Content = "Thả lại:" + codeClear;
                                        Globalvariable.C1_UI.IsPass = true;
                                        //gửi xuống PLC và xử lý tại đây
                                        OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("1"));
                                        if (write.IsSuccess)
                                        {
                                            Globalvariable.GCounter.PLC_1_Pass_C1++;
                                        }
                                        else
                                        {
                                            Globalvariable.GCounter.PLC_1_Fail_C1++;
                                        }
                                    }

                                }
                                //mã chưa active => active
                                else
                                {
                                    Globalvariable.C1_UI.Curent_Content = codeClear;
                                    Globalvariable.C1_UI.IsPass = true;
                                    //gửi xuống PLC và xử lý tại đây
                                    OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("1"));
                                    if (write.IsSuccess)
                                    {
                                        Globalvariable.GCounter.PLC_1_Pass_C1++;
                                    }
                                    else
                                    {
                                        Globalvariable.GCounter.PLC_1_Fail_C1++;
                                    }

                                    Globalvariable.Update_Content_To_SQLite_Queue.Enqueue(ProductInfo.ProductID);

                                    //cập nhật lại thông tin
                                    ProductInfo.Active = 1;
                                    ProductInfo.TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                }
                            }
                            //nếu mã chưa tồn tại => chưa active
                            else
                            {
                                Globalvariable.C1_UI.Curent_Content = codeClear;
                                Globalvariable.C1_UI.IsPass = true;
                                //ở mode nonprinter thì nếu chưa có tự thêm mới
                                if (GlobalSettings.Get("APPMODE") == "ADD_Data")
                                {
                                    Globalvariable.MaxID_Content++;
                                    ProductInfo = new ProductData
                                    {
                                        ProductID = Globalvariable.MaxID_Content,
                                        Active = 1,
                                        TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                                    };
                                    Globalvariable.Main_Content_Dictionary[codeClear] = ProductInfo;

                                    //báo pass xuống PLC
                                    OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("0"));
                                    if (write.IsSuccess)
                                    {
                                        Globalvariable.GCounter.PLC_1_Pass_C1++;
                                    }
                                    else
                                    {
                                        Globalvariable.GCounter.PLC_1_Fail_C1++;
                                    }

                                    //thêm vào hàng đợi để thêm vào SQLite
                                    Globalvariable.Add_Content_To_SQLite_Queue.Enqueue(codeClear);

                                }

                                //ở chế độ không add thì đá ra
                                else
                                {
                                    Globalvariable.C1_UI.Curent_Content = "Mã không tồn tại";
                                    Globalvariable.C1_UI.IsPass = false;
                                    OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("0"));
                                    if (write.IsSuccess)
                                    {
                                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                                    }
                                    else
                                    {
                                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                                    }
                                }

                            }

                            //Xử lý ở bể data riêng
                            //chế độ 2 camera thì camera này là camera chính
                            if (GlobalSettings.GetInt("CAMERA_SLOT") > 1)
                            {
                                //Kiểm tra mã đã kích hoạt hay chưa
                                if(Globalvariable.C1_Content_Dictionary.TryGetValue(codeClear, out ProductData C1ProductInfo))
                                {
                                    //nếu đã kích hoạt thì không làm gì cả
                                    if (C1ProductInfo.Active == 1)
                                    {
                                        //free
                                    }
                                    else
                                    {
                                        //chưa kích hoạt thì cập nhật lại thông tin
                                        C1ProductInfo.Active = 1;
                                        C1ProductInfo.TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                    }
                                }
                                else
                                {
                                    if(GlobalSettings.Get("APPMODE")== "ADD_Data")
                                    {
                                        //nếu chưa có thì thêm mới vào C1
                                        C1ProductInfo = new ProductData
                                        {
                                            ProductID = Globalvariable.MaxID_Content,
                                            Active = 1,
                                            TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                                        };
                                        Globalvariable.C1_Content_Dictionary[codeClear] = C1ProductInfo;
                                    }
                                    else
                                    {
                                        //không làm gì cả
                                    }
                                    
                                }

                            }

                        }
                        //sai mã vạch
                        else
                        {
                            Globalvariable.C1_UI.Curent_Content = "Sai mã vạch";
                            Globalvariable.C1_UI.IsPass = false;
                            //gửi xuống PLC và xử lý tại đây
                            OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("0"));
                            if (write.IsSuccess)
                            {
                                Globalvariable.GCounter.PLC_0_Pass_C1++;
                            }
                            else
                            {
                                Globalvariable.GCounter.PLC_0_Fail_C1++;
                            }

                            Alarm.Alarm1 = true;
                            Alarm.Alarm1_Count++;
                        }
                    }
                    //Sai cấu trúc cơ bản
                    else
                    {
                        Globalvariable.C1_UI.Curent_Content = "Sai cấu trúc!!!";
                        Globalvariable.C1_UI.IsPass = false;
                        //gửi xuống PLC và xử lý tại đây
                        OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("0"));
                        if (write.IsSuccess)
                        {
                            Globalvariable.GCounter.PLC_0_Pass_C1++;
                        }
                        else
                        {
                            Globalvariable.GCounter.PLC_0_Fail_C1++;
                        }
                    }
                }
               
            }
            //Mã rỗng
            else
            {
                //mã không đúng, in ra màn hình thông báo

                Globalvariable.C1_UI.Curent_Content = "Dữ liệu lỗi";
                Globalvariable.C1_UI.IsPass = false;

                //gửi xuống PLC và xử lý tại đây
                OperateResult write = PLC.plc.Write(GlobalSettings.Get("PLC_C1_RejectDM"), short.Parse("1"));
                if (write.IsSuccess)
                {
                    Globalvariable.GCounter.PLC_0_Pass_C1++;
                }
                else
                {
                    Globalvariable.GCounter.PLC_0_Fail_C1++;
                }
            }
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
                return (false, "Sai định dạng");
            }

            return (true, "Mã QR hợp lệ.");
        }

        //send to PLC

        private void Camera_c_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status_02 == e_Camera_Status.DISCONNECTED)
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
                    if (GCamera.Camera_Status_02 == e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.DISCONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCMR02Stt.Text = "Mất kết nối";
                        }));
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:
                    //xử lý dữ liệu nhận về
                    if(WK_CMR4.IsBusy)
                    {
                        LogUpdate("Camera 2 đang bận xử lý dữ liệu, vui lòng đợi");
                    }
                    else
                    {
                        Camera2DataProcess(_strData);
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

        public void Camera2DataProcess(string inputData)
        {
            Globalvariable.GCounter.Total_Return_C2++;
            //kiểm tra xem đã tồn tại ở CMR1 hay chưa
            //Khi nhận được tín hiệu bật bộ đếm thời gian
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //kiểm tra xem dữ liệu có đúng hay chưa
            if (!inputData.IsNullOrEmpty() && inputData.Contains(";"))
            {
                //tách lấy dữ liệu thô
                string[] codes = inputData.Split(';');
                if (codes.Length > 1)
                {
                    string cleanedCode = codes[0].Trim().Replace("\n", "").Replace("\r", "").Replace("\r\n","");
                    //tăng bộ đếm QR
                    Globalvariable.GCounter.Total_Code_C2++;

                    if (cleanedCode.Contains("FAIL"))
                    {
                        QR_Content = "Không đọc được";
                        
                        ISPass = false;
                        OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                        if (write.IsSuccess)
                        {
                            Globalvariable.GCounter.PLC_0_Pass_C2++;
                        }
                        else
                        {
                            Globalvariable.GCounter.PLC_0_Fail_C2++;
                        }

                        //stopwatch.Stop();
                        //TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);

                    }
                    //
                }
            }
        }

        #endregion
        public void UpdateActiveStatus(int rowId)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `QRContent` SET `Active` = '1', `TimeStampActive` = @TimeStamp  WHERE _rowid_ = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", rowId);
                    command.Parameters.AddWithValue("@TimeStamp", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        public void Add_QR_To_SQLite(string QR)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.QRCode_Folder + _clientMFI.QRCode_FileName};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO `QRContent` (ProductQR,Active, TimestampActive) VALUES (@QR,1,@TimeStamp);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QR", QR);
                    command.Parameters.AddWithValue("@TimeStamp", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        public double MaxTimeSend = 0;
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

                //if (Globalvariable.APPMODE == e_Mode.OLDMode)
                //{
                //    swMode.Active = true;
                //}
                //else
                //{
                //    swMode.Active = false;
                //}
                bool printers = false;

                if (GPrinter.Printer_Status == e_PRINTER_Status.PRINTING)
                {
                    printers = true;
                }
                else
                {
                    printers = false;
                    if(GlobalSettings.Get("APPMODE") == "ADD_Data")
                    {
                        printers = true;
                    }
                }

                if(GlobalSettings.GetInt("CAMERA_SLOT") == 1)
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
                    if (GCamera.Camera_Status == e_Camera_Status.CONNECTED && GCamera.Camera_Status_02 == e_Camera_Status.CONNECTED && Globalvariable.Data_Status == e_Data_Status.READY && Globalvariable.PLCConnect)
                {
                    if (Globalvariable.AllReady)
                    {
                        if (PLC.Ready != 1)
                        {
                            PLC.Ready = 1;
                        }

                    }
                    else
                    {
                        Globalvariable.AllReady = true;
                        if (PLC.Ready != 0)
                        {
                            PLC.Ready = 0;
                        }
                       
                    }
                }
                else
                {

                    if (!Globalvariable.AllReady)
                    {

                    }
                    else
                    {
                        Globalvariable.AllReady = false;
                    }
                }

                if (APP.ByPass_Ready)
                {
                    PLC.Ready = 1;
                }
                if(TimeSendPLC >= MaxTimeSend)
                {
                    MaxTimeSend = TimeSendPLC;
                }
                this.Invoke(new Action(() =>
                {
                    lblTotal.Value = PLC.plc.ReadInt32("D40").Content;
                    lblPass.Value = PLC.plc.ReadInt32("D34").Content;
                    lblFail.Value = PLC.plc.ReadInt32("D32").Content;
                    lblTimeOut.Value = PLC.plc.ReadInt32("D38").Content;
                    lblLineSpeed.Value = PLC.plc.ReadInt32("D70").Content;

                    //Bộ đếm phần mềm
                    opFail.Text = Counter.Fail.ToString();
                    opDiff.Text = Counter.Diff.ToString();
                    opEmpty.Text = Counter.Empty.ToString();
                    opRerun.Text = Counter.Rerun.ToString();
                    opQRCount.Text  = Counter.QRCount.ToString();
                    opCamer120Count.Text = Counter.Camera120Count.ToString();

                    opPLCSend1OK.Text = Counter.Send1ToPLC_OK.ToString();
                    opPLCSend1Fail.Text = Counter.Send1ToPLC_Fail.ToString();

                    opPLCSend0OK.Text = Counter.Send0ToPLC_OK.ToString();
                    opPLCSend0Fail.Text = Counter.Send0ToPLC_Fail.ToString();

                    opPLCTime.Text = TimeSendPLC.ToString() +"/"+MaxTimeSend.ToString();


                }));

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
                if(GlobalSettings.Get("APPMODE") == "U_PRINTER")
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
                

               
                Thread.Sleep(1000);
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


        //Cập nhật mã vừa đọc lên màn hình
        private void WK_120Update_DoWork(object sender, DoWorkEventArgs e)
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

                    //if(QRconten_His_Last <= QRContentCount)
                    //{
                    //    QRconten_His_Last++;
                    //    opHisConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: #{QRconten_His_Last} {QR_Content_His} ({timeProcess}) ");
                    //    opHisConsole.SelectedIndex = ipConsole.Items.Count - 1;
                    //    if (opHisConsole.Count > 10)
                    //    {
                    //        opHisConsole.Items.RemoveAt(0);
                    //    }
                    //}

                    if(Alarm.Alarm1)
                    {
                        lblAlarm.Text = "CẢNH BÁO SAI BARCODE (" + Alarm.Alarm1_Count.ToString() + ")";
                        lblAlarm.FillColor = Globalvariable.NG_Color;
                    }
                }));
                Thread.Sleep(100);
            }
        }

        private void btnResetCounter_Click(object sender, EventArgs e)
        {
            //btnResetCounter.Enabled = false;
            OperateResult write = PLC.plc.Write("D22", short.Parse("1"));
            if (write.IsSuccess)
            {
                //btnResetCounter.Enabled = true;
                lblFail.Value = 0;
                lblPass.Value = 0;
                lblTotal.Value = 0;

                Counter.Rerun = 0;
                Counter.Diff = 0;
                Counter.Empty = 0;
                Counter.Fail = 0;
                Counter.QRCount = 0;
                Counter.StructERR = 0;

                Counter.Send0ToPLC_OK = 0;
                Counter.Send0ToPLC_Fail = 0;
                Counter.Camera120Count = 0;
                Counter.Send1ToPLC_OK = 0;
                Counter.Send1ToPLC_Fail = 0;
            }
            else
            {
               // btnResetCounter.Enabled = true;
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

        private void WK_AddSQLite_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_AddSQLite.CancellationPending)
            {

                Thread.Sleep(1000000000);
            }
        }

        private void WK_Update120_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_UI_CAM_Update.CancellationPending)
            {
                //cập nhật
                if (Globalvariable.Update_Content_To_SQLite_Queue.Count > 0)
                {
                    UpdateActiveStatus(Globalvariable.Update_Content_To_SQLite_Queue.Dequeue());
                }

                //thêm mới
                if(Globalvariable.Add_Content_To_SQLite_Queue.Count > 0)
                {
                    string code = Globalvariable.Add_Content_To_SQLite_Queue.Dequeue();
                    Add_QR_To_SQLite(code);
                    Globalvariable.MaxID_QR++;

                    Globalvariable.Main_Content_Dictionary[code] = new ProductData
                    {
                        ProductID = Globalvariable.MaxID_QR,
                        Active = 1,
                        TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                    };
                }


                Thread.Sleep(100);
            }
        }

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
            opWK1.FillColor = Color.Green;
            //WhenDataRecive(inputString);
            Camera_01_Data_Recive(inputString);
            opWK1.FillColor = Color.White;
            stopwatch.Stop();
            this.Invoke(new Action(() =>
            {
                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT1)
                {
                    maxTimeT1 = stopwatch.Elapsed.TotalMilliseconds;
                }
                opWK1.Text = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT1}" ;
            }));
        }
        private void WK_CMR2_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            opWK2.FillColor = Color.Green;
            //WhenDataRecive(inputString);
            Camera2DataProcess(inputString);
            opWK2.FillColor = Color.White;
            stopwatch.Stop();
            this.Invoke(new Action(() =>
            {
                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT2)
                {
                    maxTimeT2 = stopwatch.Elapsed.TotalMilliseconds;
                }
                opWK2.Text = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT2}";
            }));
        }
        private void WK_CMR3_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            opWK3.FillColor = Color.Green;
            //WhenDataRecive(inputString);
            Camera2DataProcess(inputString);
            opWK3.FillColor = Color.White;
            stopwatch.Stop();
            this.Invoke(new Action(() =>
            {
                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT3)
                {
                    maxTimeT3 = stopwatch.Elapsed.TotalMilliseconds;
                }
                opWK3.Text = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT3}";
            }));
        }
        private void swModeData_ValueChanged(object sender, bool value)
        {
            if(swModeData.Active)
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

        private void WK_CMR4_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            opWK4.FillColor = Color.Green;
            Camera2DataProcess(inputString);
            opWK4.FillColor = Color.White;
            stopwatch.Stop();
            this.Invoke(new Action(() =>
            {
                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT4)
                {
                    maxTimeT4 = stopwatch.Elapsed.TotalMilliseconds;
                }
                opWK4.Text = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT4}";
            }));
        }

        private void WK_CMR5_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            opWK5.FillColor = Color.Green;
            Camera2DataProcess(inputString);
            opWK5.FillColor = Color.White;
            stopwatch.Stop();
            this.Invoke(new Action(() =>
            {
                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT5)
                {
                    maxTimeT5 = stopwatch.Elapsed.TotalMilliseconds;
                }
                opWK5.Text = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT5}";
            }));
        }

        private void WK_CMR6_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            opWK6.FillColor = Color.Green;
            Camera2DataProcess(inputString);
            opWK6.FillColor = Color.White;
            stopwatch.Stop();
            this.Invoke(new Action(() =>
            {
                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT6)
                {
                    maxTimeT6 = stopwatch.Elapsed.TotalMilliseconds;
                }
                opWK6.Text = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT6}";
            }));
        }

        

        private void uiTitlePanel5_Click(object sender, EventArgs e)
        {

        }
    }
}