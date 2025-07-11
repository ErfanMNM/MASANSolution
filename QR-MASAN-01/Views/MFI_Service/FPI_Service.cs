using Dialogs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QR_MASAN_01.Views.MFI_Service
{
    public partial class FPI_Service : UIPage
    {
        public FPI_Service()
        {
            InitializeComponent();
        }

        public POService poService;
        
        //Phân ra từng bước cho dễ thao tác



        //hàm khởi động Page
        public void INIT()
        {
           
            WK_Update.RunWorkerAsync(); // Bắt đầu chạy worker để cập nhật số lượng mã vạch đã chạy
             }
        //thêm văn bản vào listbox opTerminal
        public void AddTerminal (string text)
        {
            if (opTerminal.InvokeRequired)
            {
                opTerminal.Invoke(new Action(() =>
                {
                    opTerminal.Items.Add (text);
                    // Tự động cuộn xuống cuối danh sách
                    opTerminal.SelectedIndex = opTerminal.Items.Count - 1; // Chọn mục cuối cùng để cuộn xuống
                    if (opTerminal.Items.Count > 50) // Giới hạn số lượng mục hiển thị
                    {
                        opTerminal.Items.RemoveAt(0); // Xóa mục đầu tiên nếu vượt quá 1000 mục
                    }
                }

                ));
            }
            else
            {
                opTerminal.Items.Add(text);
                // Tự động cuộn xuống cuối danh sách
                opTerminal.SelectedIndex = opTerminal.Items.Count - 1; // Chọn mục cuối cùng để cuộn xuống
                if (opTerminal.Items.Count > 50) // Giới hạn số lượng mục hiển thị
                {
                    opTerminal.Items.RemoveAt(0); // Xóa mục đầu tiên nếu vượt quá 1000 mục
                }
            }
        }

        private void ipOrderNO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ipOrderNO.SelectedItem != null)
            {
                Globalvariable.Seleted_PO_Data = poService.Get_PO_Info_By_OrderNo(ipOrderNO.SelectedText);
                if (Globalvariable.Seleted_PO_Data.Rows.Count > 0)
                {
                    opCZCodeCount.Text = Globalvariable.Seleted_PO_Data.Rows[0]["UniqueCodeCount"].ToString();
                    opProductionLine.Text = Globalvariable.Seleted_PO_Data.Rows[0]["productionLine"].ToString();
                    opOrderQty.Text = Globalvariable.Seleted_PO_Data.Rows[0]["orderQty"].ToString();
                    opCustomerOrderNO.Text = Globalvariable.Seleted_PO_Data.Rows[0]["customerOrderNo"].ToString();
                    opProductName.Text = Globalvariable.Seleted_PO_Data.Rows[0]["productName"].ToString();
                    opProductCode.Text = Globalvariable.Seleted_PO_Data.Rows[0]["productCode"].ToString();
                    opLotNumber.Text = Globalvariable.Seleted_PO_Data.Rows[0]["lotNumber"].ToString();
                    opGTIN.Text = Globalvariable.Seleted_PO_Data.Rows[0]["gtin"].ToString();
                    opShift.Text = Globalvariable.Seleted_PO_Data.Rows[0]["shift"].ToString();
                    opFactory.Text = Globalvariable.Seleted_PO_Data.Rows[0]["factory"].ToString();
                    opSite.Text = Globalvariable.Seleted_PO_Data.Rows[0]["site"].ToString();
                    opCZCodeCount.Text = Globalvariable.Seleted_PO_Data.Rows[0]["UniqueCodeCount"].ToString();
                    opCZRunCount.Text = poService.Get_ID_RUN(Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString()).ToString();
                    GV.ID = opCZRunCount.Text.ToInt(); // Cập nhật ID từ opCZRunCount
                }
                else
                {
                    opProductionLine.Text = string.Empty;
                    opOrderQty.Text = string.Empty;
                    opCustomerOrderNO.Text = string.Empty;
                    opProductName.Text = string.Empty;
                    opProductCode.Text = string.Empty;
                    opLotNumber.Text = string.Empty;
                    opGTIN.Text = string.Empty;
                    opShift.Text = string.Empty;
                    opFactory.Text = string.Empty;
                    opSite.Text = string.Empty;
                    opCZCodeCount.Text = "0";
                }

            }
        }

        private void btnPO_Click(object sender, EventArgs e)
        {
            //ghi logs người dùng nhấn nút
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Nhấn nút chỉnh sửa PO", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} nhấn nút chỉnh sửa PO");
            //ghi logs vào hàng đợi
            if (GV.Production_Status != e_Production_Status.EDITING)
            {
                using (var dialog = new Pom_dialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        btnPO.Text = "Lưu thông tin";
                        btnPO.Symbol = 61468; // Thay đổi biểu tượng của nút btnPO
                        GV.Production_Status = e_Production_Status.EDITING; // Đặt trạng thái là đang chỉnh sửa
                        if(GV.Pass_Product_Count < Globalvariable.Seleted_PO_Data.Rows[0]["orderQty"].ToString().ToInt())
                        {
                            ipProductionDate.ReadOnly = false; //cho phép chỉnh sửa date
                            ipProductionDate.FillColor = Color.Yellow; // Đổi màu nền của ô nhập ngày sản xuất
                            ipProductionDate.ForeColor = Color.Black; // Đổi màu chữ của ô nhập ngày sản xuất

                            //ghi logs chỉ chỉnh được ngày sản xuất
                            SystemLogs systemLogsEdit = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, $"Chỉnh ProductionDate = {ipProductionDate.Text}", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} chỉ chỉnh sửa ngày sản xuất của PO: {ipOrderNO.Text}");
                            //ghi logs vào hàng đợi
                            SystemLogs.LogQueue.Enqueue(systemLogsEdit);

                           
                        }
                        else
                        {
                            poService.MES_Load_OrderNo_ToComboBox(ipOrderNO);

                            ipOrderNO.SelectedIndex = 0; // Chọn dòng đầu tiên (dòng rỗng)
                            ipOrderNO.ReadOnly = false; //cho phép chỉnh sửa
                            ipProductionDate.ReadOnly = false; //cho phép chỉnh sửa

                            ipProductionDate.FillColor = Color.Yellow; // Đổi màu nền của ô nhập ngày sản xuất
                            ipProductionDate.ForeColor = Color.Black; // Đổi màu chữ của ô nhập ngày sản xuất

                            //tương tự ipOrderNO
                            ipOrderNO.FillColor = Color.Yellow; // Đổi màu nền của ô nhập Order No
                            ipOrderNO.ForeColor = Color.Black; // Đổi màu chữ của ô nhập Order No

                            //ghi logs cho chỉnh hết
                            SystemLogs systemLogsEdit = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, "Chỉnh sửa PO", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} người dùng bắt đầu chỉnh: {ipOrderNO.Text}");

                        }

                    }
                }
            }
            else
            {
                if (ipOrderNO.Text == string.Empty || ipProductionDate.Text == string.Empty || ipOrderNO.Text == "Chọn orderNO")
                {
                    this.ShowErrorTip("Vui lòng nhập đầy đủ thông tin Order No và Production Date.");
                    return;
                }
                if(opOrderQty.Text.ToString().ToInt() > opCZCodeCount.Text.ToString().ToInt())
                {
                    this.ShowErrorTip("Số lượng mã đã nhận không đủ cho PO này. Vui lòng chờ MES gửi đủ");
                    return;
                }
                // Cập nhật dữ liệu PO

                poService.RunPO(ipOrderNO.Text, ipProductionDate.Text);

                //đổi màu lại
                ipOrderNO.FillColor = Color.CornflowerBlue; // Đổi màu nền của ô nhập Order No về màu CornflowerBlue
                ipProductionDate.FillColor = Color.CornflowerBlue; // Đổi màu nền của ô nhập Production Date về màu CornflowerBlue

                //ghi logs đổi PO thành công
                SystemLogs systemLogsSuccess = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, "Đổi PO thành công", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} đã đổi PO thành công: {ipOrderNO.Text}");
                //ghi logs vào hàng đợi
                SystemLogs.LogQueue.Enqueue(systemLogsSuccess);

                // Hiển thị thông báo thành công
                this.ShowSuccessTip("Thông tin PO đã được lưu thành công.");
                DateTime localTime = ipProductionDate.Value;
                //DateTime utcTime = localTime.ToUniversalTime();
                string isoString = localTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                Globalvariable.Seleted_PO_Data = poService.Get_PO_Info_By_OrderNo(ipOrderNO.Text);
                Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"] = isoString;// Cập nhật ngày sản xuất trong theo user chỉnh sửa

                // Đặt lại trạng thái
                ipOrderNO.ReadOnly = true; // Không cho phép chỉnh sửa Order No
                ipProductionDate.ReadOnly = true; // Không cho phép chỉnh sửa Production Date
                GV.Production_Status = e_Production_Status.READY; // Trạng thái không có PO hoặc đang chỉnh sửa
                
                btnPO.Text = "Chỉnh thông tin";
                btnPO.Symbol = 61508; // Thay đổi biểu tượng của nút btnPO
            }


        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            opCZRunCount.Text = poService.Get_ID_RUN(Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString()).ToString();
            GV.Pass_Product_Count = poService.Get_Pass_Product_Count(Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString());
            opPassCount.Text = GV.Pass_Product_Count.ToString();
        }

        private void uiTableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_Update.CancellationPending)
            {
                switch (GV.Production_Status)
                {
                    case e_Production_Status.EDITING:
                       
                        break;
                    case e_Production_Status.PUSHING:
                        break;
                    case e_Production_Status.STOPPED:

                        break;
                    case e_Production_Status.RUNNING:
                        
                        break;
                    case e_Production_Status.PAUSED:
                        break;
                    case e_Production_Status.UNKNOWN:
                        break;
                    case e_Production_Status.READY:
                        break;
                    case e_Production_Status.NOPO:
                        break;
                    case e_Production_Status.STARTUP:
                        //trạng thái khởi động.
                        poService = new POService();
                        poService.Check_PO_Log_File();//Kiểm tra xem file PG log có tồn tại hay không, nếu không thì tạo mới
                        // Lấy PO đã sử dụng lần cuối
                        DataRow lastUsedPO = poService.Get_Last_Used_PO();
                        //lấy các thông tin cơ bản

                        break;
                    case e_Production_Status.LOAD:
                        break;
                    case e_Production_Status.CHECKING:
                        break;
                }
                Thread.Sleep(100); // Đợi 0.1 giây trước khi kiểm tra lại
            }
        }

        private void btnStopPO_Click(object sender, EventArgs e)
        {
            //ghi logs người dùng nhấn nút dừng sản xuất
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Người dùng nhấn nút Sản Xuất", GV.Production_Status.ToString(), $"Người dùng {Globalvariable.CurrentUser.Username} nhấn nút sản xuất");

            switch (GV.Production_Status)
            {
                case e_Production_Status.EDITING:
                    break;
                case e_Production_Status.PUSHING:
                    break;
                case e_Production_Status.STOPPED:
                    break;
                case e_Production_Status.RUNNING:
                    //dừng sản xuất
                    GV.Production_Status = e_Production_Status.READY;
                    break;
                case e_Production_Status.PAUSED:
                    break;
                case e_Production_Status.READY:
                    //khởi động chạy
                    //đẩy dữ liệu vào Dic
                    //kiểm tra PO đã đủ số hay chưa
                    
                    Push_Data_To_Dic();
                    //chuyển lên trạng thái runnung
                    GV.Production_Status = e_Production_Status.RUNNING;
                    break;
                case e_Production_Status.UNKNOWN:
                    break;
            }
        }

        public void Push_Data_To_Dic()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
            // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
            //Đẩy vào dic chính
            string connectionString = $@"Data Source=C:/.ABC/{Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"]}.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Câu lệnh SQL để đọc một cột (ví dụ: cột 'Name')
                string query = $"SELECT * FROM `UniqueCodes`;";

                // Sử dụng SQLiteDataAdapter để đổ dữ liệu vào DataTable
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }
                // Duyệt qua các hàng trong DataTable và thêm vào List<string>
                foreach (DataRow row in dataTable.Rows)
                {
                    // Đọc dữ liệu từ SQL Server
                    int ID = Convert.ToInt32(row["ID"]); // CaseID
                    string Code = row["Code"].ToString(); // CaseQR
                    string Status = row["Status"].ToString(); // Active
                    string ActivateDate = row["ActivateDate"].ToString();
                    string ProductionDate = row["ProductionDate"].ToString();


                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                    GV.C2_CodeData_Dictionary[Code] = new CodeData
                    {
                        ID = ID,
                        Status = Status,
                        Activate_Datetime = ActivateDate,
                        Production_Datetime = ProductionDate,
                        orderNo = Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString(),
                    };

                }

                connection.Close();
            }
        }


        private void FPI_Service_Initialize(object sender, EventArgs e)
        {
            //ghi logs người dùng khởi động trang FPI_Service
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Khởi động trang FPI_Service", "FPI_Service", $"Người dùng {Globalvariable.CurrentUser.Username} đã khởi động trang FPI_Service");
            //ghi logs vào hàng đợi
            SystemLogs.LogQueue.Enqueue(systemLogs);
        }
    }
}
