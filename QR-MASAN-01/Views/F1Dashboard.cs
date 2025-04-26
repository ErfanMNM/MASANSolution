using HslCommunication;
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
using System.Threading;

namespace QR_MASAN_01
{
    public partial class F1Dashboard : UIPage
    {
        public F1Dashboard()
        {
            InitializeComponent();
            WK_Update120.RunWorkerAsync();
        }
        
        bool ClearPLC = false;
        //lấy xem có thông tin phiên tạo mới hay không

        #region Các chương trình tạo dữ liệu MFI
        public void Get_Server_MFI_ID() {

            try
            {
                // Gửi HTTP GET request đồng bộ (không sử dụng await)
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync($"{Globalvariable.Server_Url}sv1/L3/QR01/MFI_ID").Result; // Chạy đồng bộ

                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result; // Chạy đồng bộ
                        if (Globalvariable.Server_Status == e_Server_Status.DISCONNECTED)
                        {
                            Globalvariable.Server_Status = e_Server_Status.CONNECTED;
                            this.Invoke(new Action(() =>
                            {
                                opServerStatus.Text = "Đang kết nối";
                                opServerStatus.FillColor = Color.White;

                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết nối máy chủ thành công");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;

                            }));
                        }

                        //khi có phiên làm việc khác
                        if (result != Globalvariable.MFI_ID)
                        {
                            Globalvariable.MFI_ID = result;
                            Globalvariable.Data_Status = e_Data_Status.GET;
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Phát hiện phiên giao dịch mới, tiến hành lấy mã...");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                        }
                        else
                        {

                            if(Globalvariable.Data_Status != e_Data_Status.READY)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Không thấy phiên mới, tiến hành kiểm tra dữ liệu cũ...");
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                }));

                                Globalvariable.Data_Status = e_Data_Status.GET;
                            }
                            
                        }
                    }
                    else
                    {
                        if (Globalvariable.Server_Status == e_Server_Status.CONNECTED)
                        {
                            Globalvariable.Server_Status = e_Server_Status.DISCONNECTED;
                            this.Invoke(new Action(() =>
                            {
                                opServerStatus.Text = "Mất kết nối";
                            }));
                        }
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi máy chủ: {response.StatusCode}");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));

                        
                    }
                }
            }
            catch (Exception ex)
            {
                if (Globalvariable.Server_Status == e_Server_Status.CONNECTED)
                {
                    Globalvariable.Server_Status = e_Server_Status.DISCONNECTED;
                    this.Invoke(new Action(() =>
                    {
                        opServerStatus.Text = "Mất kết nối";
                    }));
                }
                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi máy chủ: {ex.Message}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
            }
        }
        public void Get_Server_MFI_DATA()
        {
            try
            {
                // Gửi HTTP GET request đồng bộ (không sử dụng await)
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync($"{Globalvariable.Server_Url}sv1/GET/MFI_"+Globalvariable.MFI_ID).Result; // Chạy đồng bộ

                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result; // Chạy đồng bộ
                        if (result != "0")
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Nhận thông tin sản xuất: {result} ");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));

                            string[] MFIData = result.Split(',');
                            Client_MFI.Product_Barcode = MFIData[0].ToString();
                            Client_MFI.Case_Barcode = MFIData[1].ToString();
                            Client_MFI.Case_LOT = MFIData[2].ToString();
                            Client_MFI.Batch_Code = MFIData[3].ToString();
                            Client_MFI.Block_Size = MFIData[4].ToString();
                            Client_MFI.Case_Size = MFIData[5].ToString();
                            Client_MFI.Pallet_Size = MFIData[6].ToString();
                            Client_MFI.SanLuong = MFIData[7].ToString();
                            Client_MFI.Pallet_QR_Type = MFIData[8].ToString();
                            Client_MFI.Operator = MFIData[9].ToString();

                            //lấy thông tin cũ


                            //Kiểm tra xem có lưu thông tin trong CSDL chưa
                            string Last_Save_MFI_ID = Get_Last_MFI_ID();
                            if(Last_Save_MFI_ID == Globalvariable.MFI_ID || Last_Save_MFI_ID == string.Empty)
                            {
                                //Không lưu
                            }
                            else
                            {
                                //lưu vào SQLite
                                string connectionString = $@"Data Source=Client_Database/MF_Data.db;Version=3;";
                                string query = @"INSERT INTO MFI_Table 
                            (MFI_ID, ProductBarcode, CaseBarcode, LOT, BatchCode, BlockSize, CaseSize, PalletSize, SanLuong, PalletQRType, OperatorName, TimeStamp)
                            VALUES 
                            (@MFI_ID, @ProductBarcode, @CaseBarcode, @LOT, @BatchCode, @BlockSize, @CaseSize, @PalletSize, @SanLuong, @PalletQRType, @OperatorName, @TimeStamp)"; ;

                                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                                {
                                    conn.Open();
                                    using (SQLiteCommand command = new SQLiteCommand(query, conn))
                                    {
                                        // Gán giá trị tham số
                                        command.Parameters.AddWithValue("@MFI_ID", Globalvariable.MFI_ID);
                                        command.Parameters.AddWithValue("@ProductBarcode", Client_MFI.Product_Barcode);
                                        command.Parameters.AddWithValue("@CaseBarcode", Client_MFI.Case_Barcode);
                                        command.Parameters.AddWithValue("@LOT", Client_MFI.Case_LOT);
                                        command.Parameters.AddWithValue("@BatchCode", Client_MFI.Batch_Code);
                                        command.Parameters.AddWithValue("@BlockSize", Client_MFI.Block_Size);
                                        command.Parameters.AddWithValue("@CaseSize", Client_MFI.Case_Size);
                                        command.Parameters.AddWithValue("@PalletSize", Client_MFI.Pallet_Size);
                                        command.Parameters.AddWithValue("@SanLuong", Client_MFI.SanLuong);
                                        command.Parameters.AddWithValue("@PalletQRType", Client_MFI.Pallet_QR_Type);
                                        command.Parameters.AddWithValue("@OperatorName", Client_MFI.Operator);
                                        command.Parameters.AddWithValue("@TimeStamp", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                                        command.ExecuteNonQuery();
                                    }
                                    conn.Close();
                                }
                            }

                            Globalvariable.Data_Status = e_Data_Status.CREATE;

                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Thông tin sản xuất có lỗi không xác định, vui lòng thử lại phía máy chủ ");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));

                            Globalvariable.Data_Status = e_Data_Status.NODATA;
                        }
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi: {response.StatusCode}");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));
                        Globalvariable.Data_Status = e_Data_Status.NODATA;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi máy chủ: {ex.Message}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                Globalvariable.Data_Status = e_Data_Status.NODATA;
            }
        }
        public void Push_Data_To_Dic()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
           // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
            string connectionString = $@"Data Source={Client_MFI.QRCode_Folder + Client_MFI.QRCode_FileName};Version=3;";
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
                    string timeStamp = "0"; // TimeStamp

                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                  Globalvariable.ProductQR_Dictionary[ProductQR] = new ProductData
                    {
                        ProductID = ProductID,
                        Active = active,
                        TimeStamp = timeStamp
                    };
                }

                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu thành công, số lượng: {dataTable.Rows.Count} ");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                connection.Close();
            }
        }
        public void Gen_QR()
        {
            //kiểm tra thư mục cần lưu
            Client_MFI.QRCode_Folder = $@"Client_Database/{Client_MFI.Case_LOT.Split("-")[2].ToString()}/{Client_MFI.Case_LOT.Split("-")[1].ToString()}/";
            if (Directory.Exists(Client_MFI.QRCode_Folder))
            {

            }
            else
            {
                Directory.CreateDirectory(Client_MFI.QRCode_Folder);
            }
            
            //kiểm tra và tạo file DB
            //tạo file dữ liệu CaseQR_Date_Random file lưu theo ngày, table là số lô sản xuất

            Client_MFI.QRCode_FileName = $"ProductCode_{Client_MFI.Case_LOT}_{Client_MFI.Batch_Code}_{Globalvariable.MFI_ID}.db";

            if (!File.Exists(Client_MFI.QRCode_Folder + Client_MFI.QRCode_FileName))
            {
                SQLiteConnection.CreateFile(Client_MFI.QRCode_Folder + Client_MFI.QRCode_FileName);
                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={Client_MFI.QRCode_Folder + Client_MFI.QRCode_FileName};Version=3;"))
                {
                    conn.Open();

                    // Tạo bảng mẫu
                    string a = $@"INSERT INTO `ProductDetails` (BatchCode ,CaseCode ,ProductCode ,BlockSize ,CaseSize ,PalletSize ,OperatorName ,TimeStamp ) 
                                  VALUES ('{Client_MFI.Batch_Code}','{Client_MFI.Case_Barcode}','{Client_MFI.Product_Barcode}','{Client_MFI.Block_Size}','{Client_MFI.Case_Size}','{Client_MFI.Pallet_Size}','Operator','{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}');";
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
                Globalvariable.Data_Status = e_Data_Status.CREATING;

                //tạo mã từ file CSV
                WK_CsV_To_Sqlite.RunWorkerAsync();
            }
            else
            {
                //nếu file đã có thì không làm gì cả, đẩy trạng thái lên push
                string [] DATELOT = Client_MFI.Case_LOT.Split('-');
                Globalvariable.QRgoc = $"i.tcx.com.vn/{Client_MFI.Product_Barcode}0A509{DATELOT[0]}{DATELOT[1]}{Client_MFI.Case_LOT.Last()}";
                Globalvariable.Data_Status = e_Data_Status.PUSH;
            }

        }

        private string Get_MFI_ID()
        {
            string connectionString = $@"Data Source=Server_Database/MF_Data.db;Version=3;";
            string query = @"
                CREATE TABLE IF NOT EXISTS `MFI_Table` (
                    `ID` INTEGER NOT NULL UNIQUE,
                    `MFI_ID` TEXT NOT NULL DEFAULT 'Server',
                    `ProductBarcode` TEXT NOT NULL,
                    `CaseBarcode` TEXT NOT NULL,
                    `LOT` TEXT NOT NULL,
                    `BatchCode` TEXT NOT NULL,
                    `BlockSize` TEXT NOT NULL,
                    `CaseSize` TEXT NOT NULL,
                    `PalletSize` TEXT,
                    `SanLuong` TEXT NOT NULL,
                    `PalletQRType` TEXT NOT NULL,
                    `OperatorName` TEXT NOT NULL,
                    `TimeStamp` TEXT NOT NULL,
                    PRIMARY KEY(`ID` AUTOINCREMENT)
                );
                SELECT `MFI_ID` FROM `MFI_Table` ORDER BY `ID` DESC LIMIT 1;
                ";

            if (!Directory.Exists("Server_Database"))
            {
                Directory.CreateDirectory("Server_Database");
            }

            if (!File.Exists("Server_Database/MF_Data.db"))
            {
                SQLiteConnection.CreateFile("Server_Database/MF_Data.db");
            }
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
                            return dt.Rows[0]["MFI_ID"].ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        public string Get_Last_MFI_ID()
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=Client_Database/MF_Data.db;Version=3;"))
            {
                connection.Open();
                string query = "SELECT `MFI_ID` FROM MFI_Table ORDER BY 'ID' DESC LIMIT 1;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            return dt.Rows[0]["MFI_ID"].ToString();

                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
            }
        }

        public int Get_MaxID_QR()
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={Client_MFI.QRCode_Folder + Client_MFI.QRCode_FileName};Version=3;"))
            {
                connection.Open();
                string query = "SELECT `seq` FROM sqlite_sequence WHERE name = 'QRContent';";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            int.TryParse(dt.Rows[0]["seq"].ToString(), out int Value);
                            return Value;

                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
        }

        private void WK_Push_Data_To_Dic_DoWork(object sender, DoWorkEventArgs e)
        {
            Push_Data_To_Dic();
        }

        private void WK_Push_Data_To_Dic_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            //đẩy dữ liệu vào máy in
            this.Invoke(new Action(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu máy in... ");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            }));
            Globalvariable.Data_Status = e_Data_Status.PRINTER_PUSH;
        }
        #endregion  

        //Load lần đầu
        private void FDashboard_Initialize(object sender, EventArgs e)
        {
            WK_Server_check.RunWorkerAsync();
            WK_Update.RunWorkerAsync();
            WK_120Update.RunWorkerAsync();
            Globalvariable.MFI_ID = Get_MFI_ID();
            Camera.Connect();
            PLC.InitPLC();
        }

        private void WK_CsV_To_Sqlite_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> filedata = File.ReadAllLines($@"Client_Database/QR_L3_1.csv").ToList();

            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={Client_MFI.QRCode_Folder + Client_MFI.QRCode_FileName};Version=3;"))
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
                        this.ShowStatusForm(100, "Tạo mã QR chai", 0);
                        for (var i = 0; i < totalItems; i++)
                        {
                            string[] DATELOT = Client_MFI.Case_LOT.Split("-");
                            string CODE = $"i.tcx.com.vn/{Client_MFI.Product_Barcode}0A509{DATELOT[0]}{DATELOT[1]}{Client_MFI.Case_LOT.Last()}{filedata[i]}";
                            
                            cmd.Parameters.AddWithValue("@ProductQR", CODE);
                            cmd.ExecuteNonQuery();


                            if (i % percentStep == 0)
                            {
                                int progressValue = (int)((i / (float)totalItems) * 100);
                                this.SetStatusFormDescription("Tạo mã QR chai" + "(" + i + ")");
                                this.SetStatusFormStepIt(5);
                            }
                        }

                        transaction.Commit();
                    }
                    conn.Close();
                    this.HideStatusForm();
                }


            }
        }
        private void WK_CsV_To_Sqlite_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Globalvariable.Data_Status = e_Data_Status.PUSH;
        }

        public string QR_Content = "Ver 18972";
        public string QR_Content_His = "";
        public  int QRContentCount = 0;
        public bool ISPass = false;

        private void Camera_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status == e_Camera_Status.DISCONNECTED)
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
                    if (GCamera.Camera_Status == e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.DISCONNECTED;
                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Mất kết nối";
                        }));
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:

                    Counter.Camera120Count++;
                    try
                    {
                        if (!WK_CMR1.IsBusy)
                        {
                            Invoke(new Action(() =>
                            {
                                opWK1.FillColor = Color.Red;
                            }));
                            WK_CMR1.RunWorkerAsync(_strData);
                        }
                        else if (!WK_CMR2.IsBusy)
                        {
                            Invoke(new Action(() =>
                            {
                                opWK2.FillColor = Color.Red;
                            }));
                            WK_CMR2.RunWorkerAsync(_strData);
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

                    break;
                case SPMS1.enumClient.RECONNECT:

                        Invoke(new Action(() =>
                        {
                            opCamera.Text = "Kết nối lại";
                            opCamera.FillColor = Color.Red;
                        }));
                    
                    break;
            }
        }

        double TimeSendPLC = 0;
        public void WhenDataRecive(string _strData)
        {
            //Khi nhận được tín hiệu
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!_strData.IsNullOrEmpty() && _strData.Contains(";"))
            {
                string[] codes = _strData.Split(';');
                if (codes.Length > 1)
                {
                    string cleanedCode = codes[0].Trim().Replace("\n", "").Replace("\r", "");
                    //tăng bộ đếm QR
                    Counter.QRCount++;
                    if (cleanedCode.Length > 3)
                    {
                        QR_Content = cleanedCode;
                        // Ví dụ: Tìm kiếm dữ liệu bằng QR
                        string searchQR = cleanedCode; // QR cần tìm
                        if (cleanedCode.Contains("FAIL"))
                        {
                            QR_Content = "Không đọc được";
                            Counter.Fail++;
                            ISPass = false;
                            OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                            if (write.IsSuccess)
                            {
                                Counter.Send0ToPLC_OK++;
                            }
                            else
                            {
                                Counter.Send0ToPLC_Fail++;
                            }

                            stopwatch.Stop();
                            TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);

                        }
                        else
                        {
                            if (Globalvariable.ProductQR_Dictionary.TryGetValue(searchQR, out ProductData ProductInfo))
                            {
                                if (ProductInfo.Active != 1)
                                {
                                    ISPass = true;
                                    OperateResult write = PLC.plc.Write("D10", short.Parse("1"));
                                    if (write.IsSuccess)
                                    {
                                        Counter.Send1ToPLC_OK++;
                                    }
                                    else
                                    {
                                        Counter.Send1ToPLC_Fail++;
                                    }
                                    TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                                    ProductInfo.Active = 1;
                                    ProductInfo.TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                    Globalvariable.UpdateQueue120.Enqueue(ProductInfo.ProductID);
                                }
                                else
                                {
                                    if (Globalvariable.ISRerun)
                                    {
                                        OperateResult write = PLC.plc.Write("D10", short.Parse("1"));
                                        if (write.IsSuccess)
                                        {
                                            Counter.Send1ToPLC_OK++;
                                        }
                                        else
                                        {
                                            Counter.Send1ToPLC_Fail++;
                                        }
                                        TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                                        Counter.Rerun++;
                                        ISPass = true;
                                        QR_Content = "Thả lại :" + QR_Content;
                                        //chế độ thả lại, không đá chai đã đọc
                                    }
                                    else
                                    {
                                        //đá chai trùng ra ngoài
                                        OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                                        if (write.IsSuccess)
                                        {
                                            Counter.Send0ToPLC_OK++;

                                        }
                                        else
                                        {
                                            Counter.Send0ToPLC_Fail++;
                                        }
                                        TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                                        ISPass = false;
                                        QR_Content = "Mã đã kích hoạt (trùng)";
                                        Counter.Diff++;
                                    }

                                }

                            }
                            else
                            {
                                //đá chai không có trong data
                                if (Globalvariable.APPMODE != e_Mode.OLDMode)
                                {
                                    //đá ra những mã không có trong máy
                                    OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                                    if (write.IsSuccess)
                                    {
                                        Counter.Send0ToPLC_OK++;

                                    }
                                    else
                                    {
                                        Counter.Send0ToPLC_Fail++;
                                    }
                                    TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                                    ISPass = false;
                                    Counter.Empty++;
                                    QR_Content = "Không có trong CSDL";

                                }
                                else
                                {
                                    //kiểm tra xem có khác barcode hay không
                                    try
                                    {
                                        string[] bCode = cleanedCode.Split("/");
                                        if (bCode[bCode.Length - 1].Contains(Client_MFI.Product_Barcode))
                                        {
                                            OperateResult write = PLC.plc.Write("D10", short.Parse("1"));
                                            if (write.IsSuccess)
                                            {
                                                Counter.Send1ToPLC_OK++;

                                            }
                                            else
                                            {
                                                Counter.Send1ToPLC_Fail++;
                                            }
                                            TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                                            ISPass = true;
                                            //ở chế độ cũ, không đá, thêm vào CSDL
                                            Globalvariable.AddQueue120.Enqueue(cleanedCode);
                                        }
                                        else
                                        {
                                            QR_Content = "Mã Sai Cấu Trúc";
                                            Alarm.Alarm1 = true;
                                            Alarm.Alarm1_Count++;
                                            OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                                            if (write.IsSuccess)
                                            {
                                                Counter.Send0ToPLC_OK++;

                                            }
                                            else
                                            {
                                                Counter.Send0ToPLC_Fail++;
                                            }
                                            TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                                            ISPass = false;
                                            Counter.StructERR++;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        QR_Content = "Mã Sai Cấu Trúc: " + ex.Message;
                                        Alarm.Alarm1 = true;
                                        Alarm.Alarm1_Count++;
                                        OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                                        if (write.IsSuccess)
                                        {
                                            Counter.Send0ToPLC_OK++;

                                        }
                                        else
                                        {
                                            Counter.Send0ToPLC_Fail++;
                                        }
                                        TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                                        ISPass = false;
                                        Counter.StructERR++;
                                    }

                                }
                            }
                        }

                    }
                    else
                    {
                        QR_Content = "Mã Rỗng";
                        OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                        if (write.IsSuccess)
                        {
                            Counter.Send0ToPLC_OK++;

                        }
                        else
                        {
                            Counter.Send0ToPLC_Fail++;
                        }
                        TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                        ISPass = false;
                        Counter.Empty++;
                    }
                    QRContentCount++;
                    QR_Content_His = QR_Content;

                    //

                }
                else
                {
                    OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                    if (write.IsSuccess)
                    {
                        Counter.Send0ToPLC_OK++;

                    }
                    else
                    {
                        Counter.Send0ToPLC_Fail++;
                    }
                    TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                    ISPass = false;
                    QR_Content = "Camera trả về rỗng : " + _strData;
                    Counter.Empty++;
                    QRContentCount++;
                    QR_Content_His = QR_Content;
                }
            }
            else
            {
                OperateResult write = PLC.plc.Write("D10", short.Parse("0"));
                if (write.IsSuccess)
                {
                    Counter.Send0ToPLC_OK++;

                }
                else
                {
                    Counter.Send0ToPLC_Fail++;
                }
                TimeSendPLC = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4);
                QR_Content = "SAI CÚ PHÁP";
                ISPass = false;
                Counter.Empty++;
                QRContentCount++;
                QR_Content_His = QR_Content;
            }

        }

        public void UpdateActiveStatus(int rowId)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={Client_MFI.QRCode_Folder + Client_MFI.QRCode_FileName};Version=3;"))
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
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={Client_MFI.QRCode_Folder + Client_MFI.QRCode_FileName};Version=3;"))
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

                if (Globalvariable.APPMODE == e_Mode.OLDMode)
                {
                    swMode.Active = true;
                }
                else
                {
                    swMode.Active = false;
                }

                //Cập nhật HMI
                if (Client_MFI.Product_Barcode != opBarcode.Text || Client_MFI.Case_Barcode != opCaseBarcode.Text || Client_MFI.Case_LOT != opDateM.Text || Client_MFI.Batch_Code != opBatch.Text)
                {
                    this.Invoke(new Action(() =>
                    {
                        opBarcode.Text = Client_MFI.Product_Barcode;
                        opCaseBarcode.Text = Client_MFI.Case_Barcode;
                        opDateM.Text = Client_MFI.Case_LOT;
                        opBatch.Text = Client_MFI.Batch_Code;
                    }));
                }

                //Ready
                if (GCamera.Camera_Status == e_Camera_Status.CONNECTED && Globalvariable.Data_Status == e_Data_Status.READY && Globalvariable.PLCConnect)
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
                        this.Invoke(new Action(() =>
                        {
                            opStatus.Text = "Đang hoạt động";
                            opStatus.FillColor = Color.Green;
                        }));
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        opStatus.FillColor = Globalvariable.WB_Color;
                    }));

                    if (!Globalvariable.AllReady)
                    {

                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            opStatus.Text = "Đang dừng";

                        }));
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

        private void opCasePrinter_Click(object sender, EventArgs e)
        {

        }
        public int QRconten_His_Last = 0;

        //Cập nhật mã vừa đọc lên màn hình
        private void WK_120Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_120Update.CancellationPending)
            {
                this.Invoke((Action)(() => {
                    opQRContent.Text = QR_Content;
                    if (ISPass)
                    {
                        opPass.Text = "TỐT";
                        opPass.FillColor = Color.Green;
                    }
                    else
                    {
                        opPass.Text = "LỖI";
                        opPass.FillColor = Color.Red;
                    }

                    if(QRconten_His_Last <= QRContentCount)
                    {
                        QRconten_His_Last++;
                        opHisConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: #{QRconten_His_Last} {QR_Content_His} ");
                        opHisConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        if (opHisConsole.Count > 10)
                        {
                            opHisConsole.Items.RemoveAt(0);
                        }
                    }

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

        private void ipConsole_DoubleClick_1(object sender, EventArgs e)
        {
            this.ShowInfoDialog(ipConsole.SelectedItem as string);
        }

        private void WK_AddSQLite_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_AddSQLite.CancellationPending)
            {

                Thread.Sleep(1000000000);
            }
        }

        private void WK_CMR1_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            string inputString = e.Argument as string;
            WhenDataRecive(inputString);
            opWK1.FillColor = Color.Green;
        }

        private void WK_Update120_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_120Update.CancellationPending)
            {

                if (Globalvariable.UpdateQueue120.Count > 0)
                {
                    UpdateActiveStatus(Globalvariable.UpdateQueue120.Dequeue());
                }

                if(Globalvariable.AddQueue120.Count > 0)
                {
                    string code = Globalvariable.AddQueue120.Dequeue();
                    Add_QR_To_SQLite(code);
                    Globalvariable.MaxID_QR++;
                    Globalvariable.ProductQR_Dictionary[code] = new ProductData
                    {
                        ProductID = Globalvariable.MaxID_QR,
                        Active = 1,
                        TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                    };
                }
                Thread.Sleep(100);
            }
        }

        private void WK_CMR2_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            string inputString = e.Argument as string;
            WhenDataRecive(inputString);
            opWK2.FillColor = Color.Green;
        }

        private void WK_Server_check_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Server_check.CancellationPending)
            {
                switch (Globalvariable.Data_Status)
                {
                    case e_Data_Status.READY:
                        Get_Server_MFI_ID();
                        break;
                    case e_Data_Status.NODATA:
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đang lấy thông tin máy chủ, vui lòng chờ... ");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));
                        Get_Server_MFI_ID();
                        break;
                    case e_Data_Status.PUSH:
                        //gửi mã vào dictionary
                        Globalvariable.MaxID_QR = Get_MaxID_QR();
                        if (!WK_Push_Data_To_Dic.IsBusy)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu camera... ");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                            WK_Push_Data_To_Dic.RunWorkerAsync();
                        }

                        break;
                    case e_Data_Status.CREATE:
                        Gen_QR();
                        break;
                    case e_Data_Status.UNKNOWN:
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Có vấn đề xảy ra trong quá trình đẩy dữ liệu, vui lòng kiểm tra lại. ");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));

                        break;
                    case e_Data_Status.GET:
                        Get_Server_MFI_DATA();
                        break;
                    case e_Data_Status.PRINTER_PUSH:
                        //gửi thông tin qua máy in
                        //if(Globalvariable.CSW_APPMODE == "NEWMode")
                        //{

                        //}
                        //else
                        //{
                        //    //Globalvariable.Data_Status = e_Data_Status.PUSHOK;
                        //}

                        break;
                    case e_Data_Status.PUSHOK:
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Dữ liệu máy QR số 1 hoàn tất ");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));
                        Globalvariable.Data_Status = e_Data_Status.READY;

                        break;
                    case e_Data_Status.CREATING:
                        break;
                }

                Thread.Sleep(2000); // Chờ 2 giây trước khi gửi request tiếp theo
            }
        }


        private void uiPanel27_Click(object sender, EventArgs e)
        {
            this.ShowInfoDialog("Số lần mà Camera trả một gói tin TCP về máy tính");
        }

        private void uiPanel25_Click(object sender, EventArgs e)
        {
            this.ShowInfoDialog("Tổng số mã QR mà máy tính nhận được");
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

        private void swMode_ValueChanged(object sender, bool value)
        {
            if (swMode.Active)
            {
                Globalvariable.APPMODE = e_Mode.OLDMode;
            }
            else
            {
                Globalvariable.APPMODE = e_Mode.NEWMode;
            }
        }
    }
}