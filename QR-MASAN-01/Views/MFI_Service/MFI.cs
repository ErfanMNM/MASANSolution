using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using Sunny.UI;
using static SPMS1.GoogleService;


namespace Service
{
    public static class Server_MFI
    {
        public static string Product_Barcode { get; set; }
        public static string Case_Barcode { get; set; }
        public static string Case_LOT { get; set; }
        public static string Batch_Code { get; set; }
        public static string Block_Size { get; set; }
        public static string Case_Size { get; set; }
        public static string Pallet_Size { get; set; }
        public static string SanLuong { get; set; }
        public static string Pallet_QR_Type { get; set; }
        public static string MFI_ID { get; set; }
        public static string Operator { get; set; }
        public static e_MFI_Status MFI_Status { get; set; } = e_MFI_Status.STARTUP;
    }

    public enum e_MFI_Status
    {
        STARTUP,
        SQLite_LOAD,
        PUSHSERVER,
        FREE,
        SQLite_SAVE
    }
    public partial class MFI : UIPage
    {

        public MFI()
        {
            InitializeComponent();
            LoadExcelToProductList($@"C:/Phan_Mem/MS_Product.xlsx");
        }

        private void FCasePrinter_Load(object sender, EventArgs e)
        {
            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Khởi động quá trình đồng bộ thông tin sản xuất - Phiên bản MFI - 8.5.5a");
        }
        private void WK_MFI_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_MFI.CancellationPending)
            {
                switch (Server_MFI.MFI_Status)
                {
                    case e_MFI_Status.STARTUP:
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Bắt đầu quá trình đồng bộ dữ liệu sản xuất");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));
                        //lấy dữ liệu lần đầu
                        if (LoadMFI().Issucces)
                        {
                            this.Invoke(new Action(() =>
                            {
                                MFI_Update_HMI();
                                
                            }));

                            Server_MFI.MFI_Status = e_MFI_Status.PUSHSERVER;
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Lấy dữ liệu trong cơ sở dữ liệu thất bại");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                        }
                        break;
                    case e_MFI_Status.SQLite_LOAD:
                        if (LoadMFI().Issucces)
                        {
                            this.Invoke(new Action(() =>
                            {
                                MFI_Update_HMI();
                                this.Invoke(new Action(() =>
                                {
                                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Kiểm tra dữ liệu");
                                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                                }));
                            }));

                            Server_MFI.MFI_Status = e_MFI_Status.PUSHSERVER;
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Lấy dữ liệu trong cơ sở dữ liệu thất bại");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                        }
                        break;
                    case e_MFI_Status.PUSHSERVER:
                        //Đẩy dữ liệu lên máy chủ
                        if (Set_Server_MFI_ID().IsSuccess)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Đẩy lên máy chủ thành công");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                            if (Set_Server_MFI_DATA().IsSuccess)
                            {
                                Server_MFI.MFI_Status = e_MFI_Status.FREE;
                            }
                            
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Đã xảy ra lỗi trong quá trình đẩy dữ liệu : Lỗi không xác định");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));
                        }
                        break;
                    case e_MFI_Status.FREE:
                        break;
                    case e_MFI_Status.SQLite_SAVE:
                        if(MFI_To_SQLite().Issucces)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Lưu dữ liệu sản xuất mới thành công");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));

                            Server_MFI.MFI_Status = e_MFI_Status.SQLite_LOAD;
                        }
                        break;
                }
                Thread.Sleep(1000);
            }
        }
        //tải thông tin sản xuất từ máy chủ
        public (bool Issucces, string message) LoadMFI()
        {
            try
            {
                string connectionString = "Data Source=Server_Database/MF_Data.db;Version=3;";
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
                                Server_MFI.Product_Barcode = dt.Rows[0]["ProductBarcode"].ToString();
                                Server_MFI.Case_Barcode = dt.Rows[0]["CaseBarcode"].ToString();
                                Server_MFI.Case_LOT = dt.Rows[0]["LOT"].ToString();
                                Server_MFI.Batch_Code = dt.Rows[0]["BatchCode"].ToString();
                                Server_MFI.Block_Size = dt.Rows[0]["BlockSize"].ToString();
                                Server_MFI.Case_Size = dt.Rows[0]["CaseSize"].ToString();
                                Server_MFI.Pallet_Size = dt.Rows[0]["PalletSize"].ToString();
                                Server_MFI.SanLuong = dt.Rows[0]["SanLuong"].ToString();
                                Server_MFI.Pallet_QR_Type = dt.Rows[0]["PalletQRType"].ToString();
                                Server_MFI.MFI_ID = dt.Rows[0]["ID"].ToString();
                                Server_MFI.Operator = dt.Rows[0]["OperatorName"].ToString();
                                return (true, "Tải thông tin MFI thành công");
                            }
                            else
                            {
                                return (false, "Không có thông tin MFI");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {

                return (false, ex.Message);
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
            ipBatchCode.Items.Add(Server_MFI.Batch_Code);

            ipCaseBarcode.Text = Server_MFI.Case_Barcode;
            ipBatchCode.SelectedItem = Server_MFI.Batch_Code;

            ipLOT.Text = Server_MFI.Case_LOT;
            ipProductBarcode.Text = Server_MFI.Product_Barcode;
            ipBlock_Size.Text = Server_MFI.Block_Size;
            ipCase_Size.Text = Server_MFI.Case_Size;
            ipPallet_Size.Text = Server_MFI.Pallet_Size;
            ipSanLuong.Text = Server_MFI.SanLuong;
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
        public (bool IsSuccess, string message) Set_Server_MFI_ID()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync($"{GlobalVariable.Server_Url}sv1/SET/MFI_ID/"+Server_MFI.MFI_ID).Result; // Chạy đồng bộ

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
                       return(false,$"Lỗi máy chủ: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"{DateTime.Now:HH:mm:ss}: Lỗi phía máy chủ: {ex.Message}");
            }
        }
        public (bool IsSuccess, string message) Set_Server_MFI_DATA()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string MFI_DataString =$"{Server_MFI.Product_Barcode},{Server_MFI.Case_Barcode},{Server_MFI.Case_LOT},{Server_MFI.Batch_Code},{Server_MFI.Block_Size},{Server_MFI.Case_Size},{Server_MFI.Pallet_Size},{Server_MFI.SanLuong},{Server_MFI.Pallet_QR_Type},{Server_MFI.Operator}";

                    HttpResponseMessage response = client.GetAsync($"{GlobalVariable.Server_Url}sv1/SET/MFI_{Server_MFI.MFI_ID}/"+ MFI_DataString).Result; // Chạy đồng bộ
                    
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
                    if (ipCaseBarcode.Text != Server_MFI.Case_Barcode ||
                        ipProductBarcode.Text != Server_MFI.Product_Barcode ||
                        ipBatchCode.Text != Server_MFI.Batch_Code ||
                        ipSanLuong.Text != Server_MFI.SanLuong ||
                        ipBlock_Size.Text != Server_MFI.Block_Size ||
                        ipCase_Size.Text != Server_MFI.Case_Size ||
                        ipPallet_Size.Text != Server_MFI.Pallet_Size ||
                        ipLOT.Text != Server_MFI.Case_LOT)
                    {
                        Invoke(new Action(() =>
                        {
                            btnLoad_Code.Enabled = true;
                            btnUndo.Enabled = true;
                        }));

                        // Kiểm tra từng trường và thay đổi màu FillColor nếu khác
                        if (ipCaseBarcode.Text != Server_MFI.Case_Barcode)
                        {
                            ipCaseBarcode.FillColor = color;
                        }
                        if (ipProductBarcode.Text != Server_MFI.Product_Barcode)
                        {
                            ipProductBarcode.FillColor = color;
                        }
                        if (ipBatchCode.Text != Server_MFI.Batch_Code)
                        {
                            ipBatchCode.FillColor = color;
                        }
                        if (ipSanLuong.Text != Server_MFI.SanLuong)
                        {
                            ipSanLuong.FillColor = color;
                        }
                        if (ipBlock_Size.Text != Server_MFI.Block_Size)
                        {
                            ipBlock_Size.FillColor = color;
                        }
                        if (ipCase_Size.Text != Server_MFI.Case_Size)
                        {
                            ipCase_Size.FillColor = color;
                        }
                        if (ipPallet_Size.Text != Server_MFI.Pallet_Size)
                        {
                            ipPallet_Size.FillColor = color;
                        }
                        if (ipLOT.Text != Server_MFI.Case_LOT)
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
                            if (Server_MFI.MFI_Status != e_MFI_Status.FREE)
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
                    lblServerStatus.FillColor = GlobalVariable.WB_Color;
                }
                //product
                if (GServer.Client_QR01 == e_Server_Status.DISCONNECTED)
                {
                    opProduct_QR_Status.FillColor = GlobalVariable.WB_Color;
                }
                //case
                if (GServer.Client_QR02 == e_Server_Status.DISCONNECTED)
                {
                    opCase_QR_Status.FillColor = GlobalVariable.WB_Color;
                }
                //pallet
                if (GServer.Client_QR03 == e_Server_Status.DISCONNECTED)
                {
                    opPallet_QR_Status.FillColor = GlobalVariable.WB_Color;
                }
                Thread.Sleep(500);
            }
        }
        public static string _BatchCode {get; set;}
        private void btnLoad_Code_Click_2(object sender, EventArgs e)
        {
                btnLoad_Code.Enabled = false;
                if (PrinterStatus.G_Pringting)
                {
                    this.ShowErrorDialog("Vui lòng tắt máy in trước khi đổ dữ liệu mới!!!");
                    btnLoad_Code.Enabled = true;
                }
                else
                {

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
                catch (Exception ex) {
                    ipConsole.Items.Add("1 + " +ex.Message);
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
                        Server_MFI.MFI_Status = e_MFI_Status.SQLite_SAVE;
                    }
                    else
                    {
                        btnLoad_Code.Enabled = true;
                    }
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
                        HttpResponseMessage response = client.GetAsync($"{GlobalVariable.Server_Url}sv1/L3/QRA/Status").Result; // Chạy đồng bộ

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
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
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

                //tải thông tin sản phẩm 
                this.Invoke(new Action(() => {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}:Tải dữ liệu sản phẩm");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                LoadExcelToProductList($@"C:/Phan_Mem/MS_Product.xlsx");

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
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            }));
            btnCloudMS.Enabled = true;
            btnCloudMS.Text = "Tải ERP";
        }
        // Danh sách toàn cục
        List<ProductInfo> productList = new List<ProductInfo>();

        // Hàm load dữ liệu từ Excel
        private void LoadExcelToProductList(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    var dataSet = reader.AsDataSet(conf);
                    var dataTable = dataSet.Tables[0]; // lấy sheet đầu tiên

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string itemCode = row["Item code"]?.ToString().Trim();
                        string barcode = row["Barcode nhãn"]?.ToString().Trim();
                        

                        if (!string.IsNullOrEmpty(itemCode))
                        {
                            productList.Add(new ProductInfo
                            {
                                ItemCode = itemCode,
                                BarcodeNhan = row["Barcode nhãn"]?.ToString().Trim(),
                                BarcodeThung = row["Barcode thùng"]?.ToString().Trim()
                            });
                        }
                    }
                }
            }
        }

        //Phần xử lý chọn mã vạch
        public class ProductInfo
        {
            public string ItemCode { get; set; }
            public string BarcodeNhan { get; set; }
            public string BarcodeThung { get; set; }
        }


        private void ipBatchCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ipBatchCode.SelectedItem != null)
            {
                try
                {
                    var a = ipBatchCode.SelectedItem as string;
                    string[] b = a.Split('-');
                    DateTime date = DateTime.ParseExact(b[1], ("ddMMyy"), System.Globalization.CultureInfo.InvariantCulture);
                    ipLOT.Value = date;

                    //phần auto bacthcode
                    string selectedBatch = ipBatchCode.SelectedItem?.ToString();
                    if (string.IsNullOrEmpty(selectedBatch))
                    {
                        ipProductBarcode.Text = "0";
                        ipCaseBarcode.Text = "0";
                        return;
                    }

                    // Tách itemCode từ batch (ví dụ: 03OT00362-xxxxxx)
                    string[] parts = selectedBatch.Split('-');
                    string itemCode = parts.Length > 0 ? parts[0] : "";

                    // Tìm sản phẩm
                    var product = productList.FirstOrDefault(p => p.ItemCode == itemCode);

                    ipProductBarcode.Text = product?.BarcodeNhan ?? "0";
                    ipCaseBarcode.Text = product?.BarcodeThung ?? "0";

                }
                catch (Exception ex)
                {
                    
                }  
            }
             
        }
    }
}

