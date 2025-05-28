using Diaglogs;
using Dialogs;
using MainClass;
using Newtonsoft.Json;
using QR_MASAN_01;
using Sunny.UI;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using static SPMS1.GoogleService;

namespace MFI_Service
{
    public partial class MFI_Service_Form : UIPage
    {
        public MFI_Info _Server_MFI { get; set; } = new MFI_Info();
        public MFI_Service_Form()
        {
            InitializeComponent();
        }

        private void FCasePrinter_Load(object sender, EventArgs e)
        {
        }
        private void WK_MFI_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_MFI.CancellationPending)
            {
                switch (_Server_MFI.MFI_Status)
                {
                    case e_MFI_Status.STARTUP:
                        //Máy tính khởi động lần đầu
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Phiên bản giao diện chỉnh thông số sản xuất : 5.16.5c");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));

                        _Server_MFI.MFI_Status = e_MFI_Status.SQLite_LOAD; //tải từ máy tính 
                        break;


                    case e_MFI_Status.SQLite_LOAD:
                        //Lấy thông tin sản xuất từ máy tính
                        var _gmfifl = Get_Last_MFI_From_Local();
                        if (_gmfifl.Issucces)
                        {
                            this.Invoke(new Action(() =>
                            {
                                MFI_Update_HMI();//Cập nhật lên màn hình
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: {_gmfifl.message}");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));

                            _Server_MFI.MFI_Status = e_MFI_Status.PUSHSERVER;
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: {_gmfifl.message}");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                        }
                        break;
                    case e_MFI_Status.PUSHSERVER:

                        var _ptsvrt =  Push_MFI_To_Server();


                        //Đẩy dữ liệu lên máy chủ
                        if (_ptsvrt.IsSuccess)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy lên máy chủ thành công");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                            _Server_MFI.MFI_Status = e_MFI_Status.FREE;
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Đã xảy ra lỗi trong quá trình đẩy dữ liệu : {_ptsvrt.Message}");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                        }
                        break;
                    case e_MFI_Status.FREE:

                        break;
                    case e_MFI_Status.SQLite_SAVE:
                        if (MFI_To_SQLite().Issucces)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Lưu dữ liệu sản xuất mới thành công");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));

                            _Server_MFI.MFI_Status = e_MFI_Status.SQLite_LOAD;
                        }
                        break;
                }
                Thread.Sleep(1000);
            }
        }
        //tải thông tin sản xuất
        public (bool Issucces, string message) Get_Last_MFI_From_Local()
        {
            try
            {

                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");
                string dbPath = Path.Combine(folderPath, "MFI_Server_Logs.tlog");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Kiểm tra nếu file chưa tồn tại thì tạo và thêm bảng
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();

                        string createTableQuery = @"
                                                    CREATE TABLE ""MFI_Table"" (
                                                        ""ID"" INTEGER NOT NULL UNIQUE,
                                                        ""MFI_ID"" TEXT NOT NULL DEFAULT 'Server',
                                                        ""ProductBarcode"" TEXT NOT NULL,
                                                        ""CaseBarcode"" TEXT NOT NULL,
                                                        ""LOT"" TEXT NOT NULL,
                                                        ""BatchCode"" TEXT NOT NULL,
                                                        ""BlockSize"" TEXT NOT NULL,
                                                        ""CaseSize"" TEXT NOT NULL,
                                                        ""PalletSize"" TEXT,
                                                        ""SanLuong"" TEXT NOT NULL,
                                                        ""PalletQRType"" TEXT NOT NULL,
                                                        ""OperatorName"" TEXT NOT NULL,
                                                        ""TimeStamp"" TEXT NOT NULL,
                                                        PRIMARY KEY(""ID"" AUTOINCREMENT)
                                                    );";

                        using (var command = new SQLiteCommand(createTableQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }

                string connectionString = $"Data Source={dbPath};Version=3;";
                string query = "SELECT * FROM `MFI_Table` ORDER BY ROWID DESC LIMIT 1;";

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
                                _Server_MFI.Product_Barcode = dt.Rows[0]["ProductBarcode"].ToString();
                                _Server_MFI.Case_Barcode = dt.Rows[0]["CaseBarcode"].ToString();
                                _Server_MFI.Case_LOT = dt.Rows[0]["LOT"].ToString();
                                _Server_MFI.Batch_Code = dt.Rows[0]["BatchCode"].ToString();
                                _Server_MFI.Block_Size = dt.Rows[0]["BlockSize"].ToString();
                                _Server_MFI.Case_Size = dt.Rows[0]["CaseSize"].ToString();
                                _Server_MFI.Pallet_Size = dt.Rows[0]["PalletSize"].ToString();
                                _Server_MFI.SanLuong = dt.Rows[0]["SanLuong"].ToString();
                                _Server_MFI.Pallet_QR_Type = dt.Rows[0]["PalletQRType"].ToString();
                                _Server_MFI.MFI_ID = dt.Rows[0]["ID"].ToString();
                                _Server_MFI.Operator = dt.Rows[0]["OperatorName"].ToString();
                                return (true, "Tải thông tin MFI từ máy tính thành công");
                            }
                            else
                            {
                                _Server_MFI.Product_Barcode = "0";
                                _Server_MFI.Case_Barcode = "0";
                                _Server_MFI.Case_LOT = "0";
                                _Server_MFI.Batch_Code = "0";
                                _Server_MFI.Block_Size = "0";
                                _Server_MFI.Case_Size = "0";
                                _Server_MFI.Pallet_Size = "0";
                                _Server_MFI.SanLuong = "0";
                                _Server_MFI.Pallet_QR_Type = "0";
                                _Server_MFI.MFI_ID = "0";
                                _Server_MFI.Operator = "0";
                                return (true, "Không có thông tin MFI, lấy giá trị mặc định");

                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                return (false, "Lỗi khi lấy MFI từ máy tính" + ex.Message);
            }
        }
        public void FMFI_INIT()
        {
            WK_Update.RunWorkerAsync();
            WK_MFI.RunWorkerAsync();
            WK_Server_Status.RunWorkerAsync();
        }
        public void MFI_Update_HMI () {
            ipBatchCode.Items.Clear();
            ipBatchCode.Items.Add(_Server_MFI.Batch_Code);

            opMFIID.Text = _Server_MFI.MFI_ID;
            ipCaseBarcode.Text = _Server_MFI.Case_Barcode;
            ipBatchCode.SelectedItem = _Server_MFI.Batch_Code;

            ipLOT.Text = _Server_MFI.Case_LOT;
            ipProductBarcode.Text = _Server_MFI.Product_Barcode;
            ipBlock_Size.Text = _Server_MFI.Block_Size;
            ipCase_Size.Text = _Server_MFI.Case_Size;
            ipPallet_Size.Text = _Server_MFI.Pallet_Size;
            ipSanLuong.Text = _Server_MFI.SanLuong;
        }
        public (bool Issucces, string message) MFI_To_SQLite()
        {
            string connectionString = $@"Data Source=Server_Database/MF_Data.db;Version=3;";
            string query = @"INSERT INTO MFI_Table 
                            (ProductBarcode, CaseBarcode, LOT, BatchCode, BlockSize, CaseSize, PalletSize, SanLuong, PalletQRType, OperatorName, TimeStamp)
                            VALUES 
                            (@ProductBarcode, @CaseBarcode, @LOT, @BatchCode, @BlockSize, @CaseSize, @PalletSize, @SanLuong, @PalletQRType, @OperatorName, @TimeStamp)"; ;

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    
                    // Gán giá trị tham số
                    command.Parameters.AddWithValue("@ProductBarcode", ipProductBarcode.Text);
                    command.Parameters.AddWithValue("@CaseBarcode", ipCaseBarcode.Text);
                    command.Parameters.AddWithValue("@LOT", ipLOT.Text);
                    command.Parameters.AddWithValue("@BatchCode", _BatchCode);
                    command.Parameters.AddWithValue("@BlockSize", ipBlock_Size.Text);
                    command.Parameters.AddWithValue("@CaseSize", ipCase_Size.Text);
                    command.Parameters.AddWithValue("@PalletSize", ipPallet_Size.Text);
                    command.Parameters.AddWithValue("@SanLuong", ipSanLuong.Text);
                    command.Parameters.AddWithValue("@PalletQRType", "test");
                    command.Parameters.AddWithValue("@OperatorName", "Operator");
                    command.Parameters.AddWithValue("@TimeStamp", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }

            return (true, "OK");
        }
        public (bool IsSuccess, string Message) Push_MFI_To_Server()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var url = "http://localhost:3000/set";
                    // Gộp toàn bộ object thành JSON string
                    string jsonValue = JsonConvert.SerializeObject(_Server_MFI);

                    // Tạo payload cho key "MFI_Data"
                    var payload = new
                    {
                        key = "MFI_Data",
                        value = JsonConvert.DeserializeObject(jsonValue) // giữ dạng object, không double encode
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


        public (bool IsSuccess, string message) Set__Server_MFI_DATA()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string MFI_DataString =$"{_Server_MFI.Product_Barcode},{_Server_MFI.Case_Barcode},{_Server_MFI.Case_LOT},{_Server_MFI.Batch_Code},{_Server_MFI.Block_Size},{_Server_MFI.Case_Size},{_Server_MFI.Pallet_Size},{_Server_MFI.SanLuong},{_Server_MFI.Pallet_QR_Type},{_Server_MFI.Operator}";

                    HttpResponseMessage response = client.GetAsync($"{Globalvariable.Server_Url}sv1/SET/MFI_{_Server_MFI.MFI_ID}/"+ MFI_DataString).Result; // Chạy đồng bộ
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result; // Chạy đồng bộ
                        //khi có phiên làm việc khác
                        if (result == "ok")
                        {
                            return (true, null);
                        }
                        else
                        {
                            return (false, "Gửi dữ liệu thất bại, máy chủ không trả về OK");
                        }


                    }
                    else
                    {
                        return (false, $"Lỗi máy chủ: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"{DateTime.Now:HH:mm:ss}: Lỗi phía máy chủ: {ex.Message}");
            }
        }
        Color color = Color.White;
        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Update.CancellationPending)
            {
                if(color == Color.White)
                {
                    color = Color.Yellow;
                }
                else
                {
                    color = Color.White;
                }

                try
                {
                    //kiểm tra và load thông tin sản xuất
                    if (ipCaseBarcode.Text != _Server_MFI.Case_Barcode ||
                        ipProductBarcode.Text != _Server_MFI.Product_Barcode ||
                        ipBatchCode.Text != _Server_MFI.Batch_Code ||
                        ipSanLuong.Text != _Server_MFI.SanLuong ||
                        ipBlock_Size.Text != _Server_MFI.Block_Size ||
                        ipCase_Size.Text != _Server_MFI.Case_Size ||
                        ipPallet_Size.Text != _Server_MFI.Pallet_Size ||
                        ipLOT.Text != _Server_MFI.Case_LOT)
                    {
                        Invoke(new Action(() =>
                        {
                            btnLoad_Code.Enabled = true;
                            btnUndo.Enabled = true;
                        }));

                        // Kiểm tra từng trường và thay đổi màu FillColor nếu khác
                        if (ipCaseBarcode.Text != _Server_MFI.Case_Barcode)
                        {
                            ipCaseBarcode.FillColor = color;
                        }
                        if (ipProductBarcode.Text != _Server_MFI.Product_Barcode)
                        {
                            ipProductBarcode.FillColor = color;
                        }
                        if (ipBatchCode.Text != _Server_MFI.Batch_Code)
                        {
                            ipBatchCode.FillColor = color;
                        }
                        if (ipSanLuong.Text != _Server_MFI.SanLuong)
                        {
                            ipSanLuong.FillColor = color;
                        }
                        if (ipBlock_Size.Text != _Server_MFI.Block_Size)
                        {
                            ipBlock_Size.FillColor = color;
                        }
                        if (ipCase_Size.Text != _Server_MFI.Case_Size)
                        {
                            ipCase_Size.FillColor = color;
                        }
                        if (ipPallet_Size.Text != _Server_MFI.Pallet_Size)
                        {
                            ipPallet_Size.FillColor = color;
                        }
                        if (ipLOT.Text != _Server_MFI.Case_LOT)
                        {
                            ipLOT.FillColor = color;
                        }
                    }
                    else
                    {
                        // Nếu tất cả đều khớp, đặt màu FillColor về mặc định
                        ipCaseBarcode.FillColor = Color.White;
                        ipProductBarcode.FillColor = Color.White;
                        ipBatchCode.FillColor = Color.White;
                        ipSanLuong.FillColor = Color.White;
                        ipBlock_Size.FillColor = Color.White;
                        ipCase_Size.FillColor = Color.White;
                        ipPallet_Size.FillColor = Color.White;
                        ipLOT.FillColor = Color.White;
                        Invoke(new Action(() =>
                        {
                            if (_Server_MFI.MFI_Status != e_MFI_Status.FREE)
                            {
                                btnLoad_Code.Enabled = false;
                            }
                            else
                            {
                                btnLoad_Code.Enabled = false;
                            }
                            btnUndo.Enabled = false;

                        }));
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        this.ShowErrorDialog($"Lỗi : {ex.Message}");

                    }));
                }
                

                if (GServer.Server_Status == e_Server_Status.DISCONNECTED)
                {
                    lblServerStatus.FillColor = Globalvariable.WB_Color;
                }
                //product
                if (GServer.Client_QR01 == e_Server_Status.DISCONNECTED)
                {
                    opProduct_QR_Status.FillColor = Globalvariable.WB_Color;
                }
                //case
                if (GServer.Client_QR02 == e_Server_Status.DISCONNECTED)
                {
                    opCase_QR_Status.FillColor = Globalvariable.WB_Color;
                }
                //pallet
                if (GServer.Client_QR03 == e_Server_Status.DISCONNECTED)
                {
                    opPallet_QR_Status.FillColor = Globalvariable.WB_Color;
                }
                Thread.Sleep(500);
            }
        }
        public static string _BatchCode {get; set;}
        private void btnLoad_Code_Click_2(object sender, EventArgs e)
        {
            btnLoad_Code.Enabled = false;


            try
            {
                ipProductBarcode.Text = ipProductBarcode.Text.Replace("\r\n", "");
                ipCaseBarcode.Text = ipCaseBarcode.Text.Replace("\r\n", "");
                ipLOT.Text = ipLOT.Text.Replace("\r\n", "");
                ipBlock_Size.Text = ipBlock_Size.Text.Replace("\r\n", "");
                ipCase_Size.Text = ipCase_Size.Text.Replace("\r\n", "");
                ipPallet_Size.Text = ipPallet_Size.Text.Replace("\r\n", "");
                ipSanLuong.Text = ipSanLuong.Text.Replace("\r\n", "");
                _BatchCode = ipBatchCode.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                ipConsole.Items.Add("1 + " + ex.Message);
            }

            try
            {
                var a = ipBatchCode.SelectedItem as string;
                string[] b = a.Split('-');
                DateTime date = DateTime.ParseExact(b[1], ("ddMMyy"), System.Globalization.CultureInfo.InvariantCulture);
                if (ipLOT.Value != date)
                {
                    this.ShowErrorDialog($"Số lô với ngày sản xuất đang khác nhau, bạn có chắc chắn sẽ lưu????");
                }
            }
            catch (Exception ex)
            {
                ipConsole.Items.Add(ex.Message);
            }

            if (this.ShowAskDialog("Đồng ý lưu?", "Bạn có chắc chắn sẽ lưu thông tin mới? Thao tác này sẽ tạo và sử dụng bộ mã QR mới cho tất cả các máy khác. Thời gian để khởi động lại khoảng 5 phút", UIStyle.Red))
            {
                btnLoad_Code.Enabled = false;
                _Server_MFI.MFI_Status = e_MFI_Status.SQLite_SAVE;
            }
            else
            {
                btnLoad_Code.Enabled = true;
            }


        }
        private void btnUndo_Click(object sender, EventArgs e)
        {
            MFI_Update_HMI();
        }
        private void WK_Server_Status_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Server_Status.CancellationPending) {

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync($"{Globalvariable.Server_Url}sv1/L3/QRA/Status").Result; // Chạy đồng bộ

                        if (response.IsSuccessStatusCode)
                        {
                            string result = response.Content.ReadAsStringAsync().Result;

                            ClientData clientData = JsonConvert.DeserializeObject<ClientData>(result);

                            if (GServer.Server_Status == e_Server_Status.DISCONNECTED)
                            {
                                GServer.Server_Status = e_Server_Status.CONNECTED;
                                lblServerStatus.Text = "Máy chủ sẵn sàng";
                                lblServerStatus.FillColor = Color.LimeGreen;
                            }
                            //product
                            if (clientData.L3_QR01 == "1")
                            {
                                if (GServer.Client_QR01 == e_Server_Status.DISCONNECTED)
                                {
                                    GServer.Client_QR01 = e_Server_Status.CONNECTED;
                                    opProduct_QR_Status.Text = "Đang sẵn sàng";
                                    opProduct_QR_Status.FillColor = Color.White;
                                }
                            }
                            else
                            {
                                if (GServer.Client_QR01 == e_Server_Status.CONNECTED)
                                {
                                    GServer.Client_QR01 = e_Server_Status.DISCONNECTED;
                                    opProduct_QR_Status.Text = "Mất kết nối";
                                }
                            }
                            //case
                            if (clientData.L3_QR02 == "1")
                            {
                                if (GServer.Client_QR02 == e_Server_Status.DISCONNECTED)
                                {
                                    GServer.Client_QR02 = e_Server_Status.CONNECTED;
                                    opCase_QR_Status.Text = "Đang sẵn sàng";
                                    opCase_QR_Status.FillColor = Color.White;
                                }
                            }
                            else
                            {
                                if (GServer.Client_QR02 == e_Server_Status.CONNECTED)
                                {
                                    GServer.Client_QR02 = e_Server_Status.DISCONNECTED;
                                    opCase_QR_Status.Text = "Mất kết nối";
                                }
                            }
                            //pallet
                            if (clientData.L3_QR03 == "1")
                            {
                                if (GServer.Client_QR03 == e_Server_Status.DISCONNECTED)
                                {
                                    GServer.Client_QR03 = e_Server_Status.CONNECTED;
                                    opPallet_QR_Status.Text = "Đang sẵn sàng";
                                    opPallet_QR_Status.FillColor = Color.White;
                                }
                            }
                            else
                            {
                                if (GServer.Client_QR03 == e_Server_Status.CONNECTED)
                                {
                                    GServer.Client_QR03 = e_Server_Status.DISCONNECTED;
                                    opPallet_QR_Status.Text = "Mất kết nối";
                                }
                            }


                        }
                        else
                        {
                            if (GServer.Server_Status == e_Server_Status.CONNECTED)
                            {
                                GServer.Server_Status = e_Server_Status.DISCONNECTED;
                                lblServerStatus.Text = "Máy chủ mất kết nối";
                            }
                            //product
                            if (GServer.Client_QR01 == e_Server_Status.CONNECTED)
                            {
                                GServer.Client_QR01 = e_Server_Status.DISCONNECTED;
                                opProduct_QR_Status.Text = "Mất kết nối";
                            }
                            //case
                            if (GServer.Client_QR02 == e_Server_Status.CONNECTED)
                            {
                                GServer.Client_QR02 = e_Server_Status.DISCONNECTED;
                                opCase_QR_Status.Text = "Mất kết nối";
                            }
                            //pallet
                            if (GServer.Client_QR03 == e_Server_Status.CONNECTED)
                            {
                                GServer.Client_QR03 = e_Server_Status.DISCONNECTED;
                                opPallet_QR_Status.Text = "Mất kết nối";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (GServer.Server_Status == e_Server_Status.CONNECTED)
                    {
                        GServer.Server_Status = e_Server_Status.DISCONNECTED;
                        lblServerStatus.Text = "Máy chủ lỗi kết nối";
                    }

                    //product
                    if (GServer.Client_QR01 == e_Server_Status.CONNECTED)
                    {
                        GServer.Client_QR01 = e_Server_Status.DISCONNECTED;
                        opProduct_QR_Status.Text = "Mất kết nối";
                    }
                    //case
                    if (GServer.Client_QR02 == e_Server_Status.CONNECTED)
                    {
                        GServer.Client_QR02 = e_Server_Status.DISCONNECTED;
                        opCase_QR_Status.Text = "Mất kết nối";
                    }
                    //pallet
                    if (GServer.Client_QR03 == e_Server_Status.CONNECTED)
                    {
                        GServer.Client_QR03 = e_Server_Status.DISCONNECTED;
                        opPallet_QR_Status.Text = "Mất kết nối";
                    }
                }
                Thread.Sleep(2000);
            }
        }
        private void btnScanBarcode_Click(object sender, EventArgs e)
        {
            using (var dialog = new Scaner())
            {
                dialog._Title = "Quét barcode chai";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string onlyNumbers = new string(dialog.TextValue.Where(char.IsDigit).ToArray());

                    ipProductBarcode.Text = onlyNumbers;
                    ipCaseBarcode.Text = "1" + onlyNumbers;
                }
            }
        }

        private void btnCaseBarcode_Click(object sender, EventArgs e)
        {
            using (var dialog = new Scaner())
            {
                dialog._Title = "Quét barcode thùng";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string onlyNumbers = new string(dialog.TextValue.Where(char.IsDigit).ToArray());

                    ipProductBarcode.Text = onlyNumbers;
                    ipCaseBarcode.Text = "1" + onlyNumbers;
                }
            }
        }

        private void btnEnterBatch_Click(object sender, EventArgs e)
        {
            using (var dialog = new Entertext())
            {
                dialog.TextValue = ipBatchCode.SelectedText;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ipBatchCode.Items.Add(dialog.TextValue);
                    ipBatchCode.SelectedItem = dialog.TextValue;
                }
            }
        }

        private void btnCloudMS_Click(object sender, EventArgs e)
        {
            btnCloudMS.Enabled= false;
            btnCloudMS.Text = "Đang tải ERP...";
            this.Invoke(new Action(() => {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Bắt đầu tải dữ liệu từ Cloud");
                ipBatchCode.SelectedIndex = ipBatchCode.Items.Count - 1;
            }));
            WK_LoadCloud.RunWorkerAsync();
        }
        Big_Query_Result ERP { get; set; } = new Big_Query_Result();
        private void WK_LoadCloud_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ERP = googleService1.BigQuery($@"SELECT *
                FROM `sales-268504.FactoryIntegration.BatchProduction`
                WHERE `ORG_CODE` = ""MIP""
                AND `LINE` = ""Line 3""
                AND SUB_INV = ""W05""
                ORDER BY `LAST_UPDATE_DATE` DESC;
                ");
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => { 
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Lỗi khi tải dữ liệu ERP: {ex.Message}");
                }));
            }
            
        }

        private void WK_LoadCloud_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(ERP.DataTable.Rows.Count < 1)
            {
                this.ShowErrorDialog("Không có dữ liệu ERP");
            }
            foreach (DataRow row in ERP.DataTable.Rows)
            {
                string a = row[4].ToString();
                string b = row["THU_PAL"].ToString();
                ipBatchCode.Items.Add(a);
            }
            this.Invoke(new Action(() => {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Lấy dữ liệu ERP thành công có {ERP.DataTable.Rows.Count} ô dữ liệu" );
                ipBatchCode.SelectedIndex = ipBatchCode.Items.Count - 1;
            }));
            btnCloudMS.Enabled = true;
            btnCloudMS.Text = "Tải ERP";
        }

        private void ipBatchCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btnLoad_Code.Disabled)
            {
                try
                {
                    var a = ipBatchCode.SelectedItem as string;
                    string[] b = a.Split('-');
                    DateTime date = DateTime.ParseExact(b[1], ("ddMMyy"), System.Globalization.CultureInfo.InvariantCulture);
                    ipLOT.Value = date;
                }
                catch (Exception ex)
                {
                    
                }  
            }
             
        }
    }
}

