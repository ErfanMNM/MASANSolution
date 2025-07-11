using Dialogs;
using MainClass.Enum;
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
using static QR_MASAN_01.SystemLogs;

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

        private void ipOrderNO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ipOrderNO.SelectedItem != null)
            {
                Globalvariable.Seleted_PO_Data = poService.Get_PO_Info_By_OrderNo(ipOrderNO.SelectedText);
                if(Globalvariable.Seleted_PO_Data.Rows.Count > 0)
                {
                    // Cập nhật các thông tin PO đã chọn
                    poService.Check_Run_File(GV.Selected_PO.orderNo.ToString()); // Kiểm
                }
  
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
                    opCZRunCount.Text = poService.Get_ID_RUN(GV.Selected_PO.orderNo.ToString()).ToString();
                    GV.ID = opCZRunCount.Text.ToInt(); // Cập nhật ID từ opCZRunCount
                    opPassCount.Text = poService.get.Get_Record_Product_Count(GV.Selected_PO.orderNo.ToString(), e_Content_Result.PASS).ToString();
                    opFailCount.Text = "Đang tải...";
                    opDuplicateCount.Text = poService.get.Get_Record_Product_Count(GV.Selected_PO.orderNo.ToString(), e_Content_Result.DUPLICATE).ToString();
                    // Cập nhật thông tin vào biến Selected_PO

                    if(opOrderQty.Text.ToInt() <= opPassCount.Text.ToInt32())
                    {
                        opTer.Text = $"PO đang chọn đã hoàn thành\r\n>> Hãy chọn PO khác";
                        opTer.ForeColor = Color.Red; // Đổi màu chữ của ô thông báo
                        opTer.Font = new Font(opTer.Font, FontStyle.Bold); // Đổi chữ đậm
                    }
                    else
                    {
                        opTer.Text = $"Nhấn LƯU LẠI để áp dụng PO";
                        opTer.ForeColor = Color.Green; // Đổi màu chữ của ô thông báo
                        opTer.Font = new Font(opTer.Font, FontStyle.Regular); // Đổi chữ thường
                    }

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
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Nhấn nút chỉnh sửa Date", "ProductDate", $"Người dùng {Globalvariable.CurrentUser.Username} nhấn nút chỉnh sửa Date");
            btnPO.Enabled = false; // ẩn nút chỉnh PO để tránh nhấn nhiều lần
            //ghi logs vào hàng đợi
            if (GV.Production_Status != e_Production_Status.DATE_EDITING)
            {
                GV.Production_Status = e_Production_Status.DATE_EDITING; // Chuyển sang trạng thái chỉnh sửa ngày sản xuất
                ipProductionDate.ReadOnly = false; // Cho phép chỉnh sửa ngày sản xuất
                ipProductionDate.FillColor = Color.Yellow; // Đổi màu nền của ô nhập ngày sản xuất
                ipProductionDate.ForeColor = Color.Black; // Đổi màu chữ của ô nhập ngày sản xuất
                btnProductionDate.Text = "LƯU NGÀY"; // Đặt lại văn bản nút
                btnProductionDate.Symbol = 61468; // Đặt lại biểu tượng nút
                btnProductionDate.FillColor = Color.Green; // Đổi màu nền của nút btnProductionDate
                btnProductionDate.Enabled = true; // Hiển thị nút chỉnh Date
                // Cập nhật thông báo
                SafeInvoke(() =>
                {
                    opTer.Text = $"Chỉnh ngày sản xuất cho PO: {ipOrderNO.Text} \r\n>> Nhấn LƯU NGÀY để áp dụng thay đổi";
                    opTer.ForeColor = Color.Green; // Đổi màu chữ của ô thông báo
                    opTer.Font = new Font(opTer.Font, FontStyle.Regular); // Đổi chữ thường
                });



            }
            else if (GV.Production_Status == e_Production_Status.DATE_EDITING)
            {
                //kiểm tra xem ngày sản xuất có hợp lệ hay không
                if (DateTime.TryParse(ipProductionDate.Text, out DateTime productionDate))
                {
                    //nếu hợp lệ thì lưu lại
                    poService.RunPO(ipOrderNO.Text, productionDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                    //ghi logs đổi ngày sản xuất thành công
                    SystemLogs systemLogsSuccess = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, "Đổi ngày sản xuất thành công", "ProductDate", $"Người dùng {Globalvariable.CurrentUser.Username} đã đổi ngày sản xuất thành công: {ipProductionDate.Text}");
                    LogQueue.Enqueue(systemLogsSuccess);
                    //hiển thị thông báo thành công
                    this.ShowSuccessNotifier("Ngày sản xuất đã được lưu thành công.", false, 3000);
                    //đổi màu lại
                   // GV.Production_Status = e_Production_Status.; // Trở về trạng thái READY
                    
                }
                else
                {
                    this.ShowErrorNotifier("Ngày sản xuất không hợp lệ. Vui lòng nhập lại.", false, 3000);
                }

                //chuyển sang trạng thái Check
                GV.Production_Status = e_Production_Status.CHECKING; // Trạng thái kiểm tra, có thể thực hiện các kiểm tra cần thiết
            }
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            opCZRunCount.Text = poService.Get_ID_RUN(GV.Selected_PO.orderNo.ToString()).ToString();
            GV.Pass_Product_Count = poService.get.Get_Record_Product_Count(GV.Selected_PO.orderNo.ToString(), e_Content_Result.PASS);
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
                        //tắt hết các nút khác ngoại trừ nút chỉnh sửa
                        
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
                        if(btnRUN.Enabled == false)
                        {
                            btnRUN.Enabled = true; // Hiển thị nút chạy
                            btnRUN.Text = "BẮT ĐẦU SẢN XUẤT"; // Đặt lại văn bản nút
                            btnRUN.Symbol = 61515; // Đặt lại biểu tượng nút
                            btnRUN.FillColor = Color.Green; // Đổi màu nền của nút btnRUN về màu CornflowerBlue
                            btnRUN.ForeColor = Color.White; // Đổi màu chữ của nút btnRUN về màu trắng
                        }
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
                        DataTable lastUsedPO_Data = poService.Get_PO_Info_By_OrderNo(lastUsedPO["orderNO"].ToString());

                        //kiểm tra xem tồn tại hay chưa
                        if (lastUsedPO_Data.Rows.Count <= 0)
                        {
                            //nếu không có PO này thì chuyển sang chế độ NO PO
                            GV.Production_Status = e_Production_Status.NOPO;
                        }
                        //lấy thông tin vào biến Selected_PO
                        GV.Selected_PO.orderNo = lastUsedPO_Data.Rows[0]["orderNO"].ToString();
                        GV.Selected_PO.productionDate = lastUsedPO_Data.Rows[0]["productionDate"].ToString();
                        GV.Selected_PO.productName = lastUsedPO_Data.Rows[0]["productName"].ToString();
                        GV.Selected_PO.productCode = lastUsedPO_Data.Rows[0]["productCode"].ToString();
                        GV.Selected_PO.lotNumber = lastUsedPO_Data.Rows[0]["lotNumber"].ToString();
                        GV.Selected_PO.GTIN = lastUsedPO_Data.Rows[0]["GTIN"].ToString();
                        GV.Selected_PO.shift = lastUsedPO_Data.Rows[0]["shift"].ToString();
                        GV.Selected_PO.factory = lastUsedPO_Data.Rows[0]["factory"].ToString();
                        GV.Selected_PO.site = lastUsedPO_Data.Rows[0]["site"].ToString();
                        GV.Selected_PO.orderQty = lastUsedPO_Data.Rows[0]["orderQty"].ToString();
                        GV.Selected_PO.CodeCount = lastUsedPO_Data.Rows[0]["UniqueCodeCount"].ToString();
                        GV.Selected_PO.customerOrderNo = lastUsedPO_Data.Rows[0]["customerOrderNo"].ToString();
                        GV.Selected_PO.runInfo.total = poService.Get_ID_RUN(lastUsedPO_Data.Rows[0]["orderNO"].ToString());
                        GV.Selected_PO.runInfo.pass = poService.get.Get_Record_Product_Count(lastUsedPO_Data.Rows[0]["orderNO"].ToString(), e_Content_Result.PASS);
                        GV.Selected_PO.runInfo.fail = GV.Selected_PO.runInfo.total - GV.Selected_PO.runInfo.pass;
                        GV.Selected_PO.runInfo.duplicate = poService.get.Get_Record_Product_Count(lastUsedPO_Data.Rows[0]["orderNO"].ToString(), e_Content_Result.DUPLICATE);

                        //lấy các thông tin gửi AWS
                        GV.Selected_PO.awsInfo.sent = poService.get.Get_AWS_Sent_Count(lastUsedPO_Data.Rows[0]["orderNO"].ToString());

                        //kiểm tra PO đã chạy hay chưa
                        int runPassCount = poService.get.Get_Record_Product_Count(lastUsedPO_Data.Rows[0]["orderNO"].ToString(), e_Content_Result.PASS);

                        //nếu đã chạy rồi, ẩn nút chỉnh PO
                        if (runPassCount > 0)
                        {
                            btnProductionDate.Enabled = false; // ẩn nút chỉnh PO
                            GV.Production_Status = e_Production_Status.LOAD; // chuyển sang trạng thái load
                        }
                        else
                        {
                            btnProductionDate.Enabled = true; // hiển thị nút chỉnh PO
                            btnProductionDate.Text = "Đổi ngày sản xuất"; // Đặt lại văn bản nút
                            btnPO.Enabled = true; // Hiển thị nút chỉnh PO
                            btnPO.Text = "ĐỔI PO"; // Đặt lại văn bản nút
                            btnPO.Symbol = 61508; // Đặt lại biểu tượng nút
                            btnPO.FillColor = Color.Yellow; // Đổi màu nền của nút btnPO về màu CornflowerBlue
                            btnPO.ForeColor = Color.Black; // Đổi màu chữ của nút btnPO về màu đen
                            GV.Production_Status = e_Production_Status.LOAD; // chuyển sang trạng thái sẵn sàng
                        }


                        break;
                    case e_Production_Status.LOAD:
                        // Trạng thái load, hiển thị thông tin PO đã chọn
                        // Cập nhật các thông tin PO đã chọn
                        SafeInvoke(() =>
                        {
                            // Cập nhật các thông tin PO đã chọn
                            ipOrderNO.Text = GV.Selected_PO.orderNo;
                            ipProductionDate.Text = GV.Selected_PO.productionDate;
                            opProductName.Text = GV.Selected_PO.productName;
                            opProductCode.Text = GV.Selected_PO.productCode;
                            opLotNumber.Text = GV.Selected_PO.lotNumber;
                            opGTIN.Text = GV.Selected_PO.GTIN;
                            opShift.Text = GV.Selected_PO.shift;
                            opFactory.Text = GV.Selected_PO.factory;
                            opSite.Text = GV.Selected_PO.site;
                            opOrderQty.Text = GV.Selected_PO.orderQty;
                            opCZCodeCount.Text = GV.Selected_PO.CodeCount;
                            opCZRunCount.Text = GV.Selected_PO.runInfo.total.ToString();
                            opPassCount.Text = GV.Selected_PO.runInfo.pass.ToString();
                            opFailCount.Text = GV.Selected_PO.runInfo.fail.ToString();
                            opDuplicateCount.Text = GV.Selected_PO.runInfo.duplicate.ToString();
                        });

                        //chuyển sang trạng thái CHECKING
                        GV.Production_Status = e_Production_Status.CHECKING;
                        break;
                    case e_Production_Status.CHECKING:
                        // Trạng thái kiểm tra, có thể thực hiện các kiểm tra cần thiết
                        // Kiểm tra xem PO đã đủ số lượng hay chưa
                        if (GV.Selected_PO.runInfo.pass >= GV.Selected_PO.orderQty.ToInt())
                        {
                            // Nếu đã đủ số lượng, chuyển sang trạng thái Completed
                            GV.Production_Status = e_Production_Status.COMPLETE;
                        }
                        else
                        {
                            //kiểm tra xem đã chạy mã nào hay chưa
                            if(GV.Selected_PO.runInfo.pass <= 0)
                            {
                                // Nếu chưa chạy mã nào, cho phép chỉnh sửa PO
                                SafeInvoke(() =>
                                {
                                    btnPO.Enabled = true; // Hiển thị nút chỉnh PO
                                    btnPO.Text = "ĐỔI PO"; // Đặt lại văn bản nút
                                    btnPO.Symbol = 61508; // Đặt lại biểu tượng nút
                                    btnPO.FillColor = Color.CornflowerBlue; // Đổi màu nền của nút btnPO về màu CornflowerBlue
                                    btnPO.ForeColor = Color.White; // Đổi màu chữ của nút btnPO về màu trắng
                                });
                            }
                            //cho phép chỉnh date
                            SafeInvoke(() =>
                            {
                                btnProductionDate.Enabled = true; // Hiển thị nút chỉnh Date
                                btnProductionDate.Text = "Đổi ngày sản xuất"; // Đặt lại văn bản nút
                                btnProductionDate.Symbol = 61508; // Đặt lại biểu tượng nút

                                ipProductionDate.ReadOnly = true; // Không cho phép chỉnh sửa ngày sản xuất
                                ipProductionDate.FillColor = Color.CornflowerBlue; // Đổi màu nền của ô nhập ngày sản xuất về màu CornflowerBlue
                                ipProductionDate.ForeColor = Color.Black;
                                ipOrderNO.ReadOnly = true; // Không cho phép chỉnh sửa Order No
                                ipOrderNO.FillColor = Color.CornflowerBlue; // Đổi màu nền của ô nhập Order No về màu CornflowerBlue
                                ipOrderNO.ForeColor = Color.White; // Đổi màu chữ của ô nhập Order No về màu trắng

                            });
                            // Nếu chưa đủ số lượng, chuyển sang READY tiếp tục chạy
                            GV.Production_Status = e_Production_Status.READY;

                        }
                        break;
                    case e_Production_Status.COMPLETE:
                        // Trạng thái hoàn thành, có thể thực hiện các hành động cần thiết
                        //cập nhật thông báo ra richtextbox
                        SafeInvoke(() =>
                        {
                            if (!opTer.Text.Contains("Đã"))
                            {
                                opTer.Text = $"Đã hoàn thành PO: {GV.Selected_PO.orderNo} với số lượng: {GV.Selected_PO.runInfo.pass} \r\nVui lòng chọn PO khác để chạy";
                                opTer.ForeColor = Color.Green; // Đổi màu chữ của ô thông báo
                                opTer.Font = new Font(opTer.Font, FontStyle.Bold); // Đổi chữ đậm
                            }

                        });
                        //Cho phép chỉnh sửa PO
                        SafeInvoke(() =>
                        {
                            btnPO.Enabled = true; // Hiển thị nút chỉnh PO
                            btnPO.Text = "ĐỔI PO"; // Đặt lại văn bản nút
                            btnPO.Symbol = 61508; // Đặt lại biểu tượng nút
                        });

                        break;
                    case e_Production_Status.DATE_EDITING:
                        // Trạng thái chỉnh sửa ngày sản xuất, có thể thực hiện các hành động cần thiết
                        break;
                }
                Thread.Sleep(100); // Đợi 0.1 giây trước khi kiểm tra lại
            }
        }

        //hàm save invoke
        void SafeInvoke(Action action)
        {
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }

        public void Push_Data_To_Dic()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
            // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
            //Đẩy vào dic chính
            string connectionString = $@"Data Source=C:/.ABC/{GV.Selected_PO.orderNo}.db;Version=3;";

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
                        orderNo = GV.Selected_PO.orderNo.ToString(),
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

        private void btnPO_Click_1(object sender, EventArgs e)
        {
            //ghi logs người dùng nhấn nút chỉnh sửa PO
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Nhấn nút chỉnh sửa PO", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} nhấn nút chỉnh sửa PO");
            //ghi logs vào hàng đợi
            SystemLogs.LogQueue.Enqueue(systemLogs);

            switch (GV.Production_Status)
            {
                case e_Production_Status.EDITING:
                    //kiểm tra PO trước khi lưu
                    if (ipOrderNO.Text == string.Empty || ipProductionDate.Text == string.Empty || ipOrderNO.Text == "Chọn orderNO")
                    {
                        this.ShowErrorNotifier("Vui lòng nhập đầy đủ thông tin Order No và Production Date.",false,3000);
                        return;
                    }

                    //kiểm tra nếu PO đã hoàn thành thì không cho lưu
                    if (opOrderQty.Text.ToInt() <= opPassCount.Text.ToInt32())
                    {
                        this.ShowErrorNotifier("PO vừa chọn đã chạy hoàn tất, vui lòng chọn PO khác", false, 3000);
                        return;
                    }

                    DataTable usedPO = new DataTable();
                    usedPO = poService.Get_PO_Info_By_OrderNo(ipOrderNO.Text);
                    if (usedPO.Rows.Count <= 0)
                    {
                        //báo lỗi không tìm thấy PO
                        this.ShowErrorNotifier("Không tìm thấy PO đã lưu, vui lòng kiểm tra lại", false, 3000);
                        return;
                    }
                    //kiểm PO đủ mã hay chưa
                    if (opOrderQty.Text.ToInt() > opCZCodeCount.Text.ToInt())
                    {
                        this.ShowErrorNotifier("Số lượng mã đã nhận không đủ cho PO này. Vui lòng chờ MES gửi đủ", false, 3000);
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
                    LogQueue.Enqueue(systemLogsSuccess);

                    // Hiển thị thông báo thành công
                    this.ShowSuccessNotifier("Thông tin PO đã được lưu thành công.", false, 3000);

                    //cập nhật GV.Selected_PO_Data
                    GV.Selected_PO.orderNo = ipOrderNO.Text;
                    GV.Selected_PO.productionDate = usedPO.Rows[0]["productionDate"].ToString();
                    GV.Selected_PO.productName = usedPO.Rows[0]["productName"].ToString();
                    GV.Selected_PO.productCode = usedPO.Rows[0]["productCode"].ToString();
                    GV.Selected_PO.lotNumber = usedPO.Rows[0]["lotNumber"].ToString();
                    GV.Selected_PO.GTIN = usedPO.Rows[0]["gtin"].ToString();
                    GV.Selected_PO.shift = usedPO.Rows[0]["shift"].ToString();
                    GV.Selected_PO.factory = usedPO.Rows[0]["factory"].ToString();
                    GV.Selected_PO.site = usedPO.Rows[0]["site"].ToString();
                    GV.Selected_PO.orderQty = usedPO.Rows[0]["orderQty"].ToString();
                    GV.Selected_PO.CodeCount = usedPO.Rows[0]["UniqueCodeCount"].ToString();
                    GV.Selected_PO.customerOrderNo = usedPO.Rows[0]["customerOrderNo"].ToString();
                    GV.Selected_PO.runInfo.total = poService.Get_ID_RUN(usedPO.Rows[0]["orderNO"].ToString());
                    GV.Selected_PO.runInfo.pass = poService.get.Get_Record_Product_Count(usedPO.Rows[0]["orderNO"].ToString(), e_Content_Result.PASS);
                    GV.Selected_PO.runInfo.fail = GV.Selected_PO.runInfo.total - GV.Selected_PO.runInfo.pass;
                    GV.Selected_PO.runInfo.duplicate = poService.get.Get_Record_Product_Count(usedPO.Rows[0]["orderNO"].ToString(), e_Content_Result.DUPLICATE);
                    //lấy các thông tin gửi AWS
                    GV.Selected_PO.awsInfo.sent = poService.get.Get_AWS_Sent_Count(usedPO.Rows[0]["orderNO"].ToString());
                    // Đặt lại trạng thái
                    ipOrderNO.ReadOnly = true; // Không cho phép chỉnh sửa Order No
                    ipProductionDate.ReadOnly = true; // Không cho phép chỉnh sửa Production Date
                    GV.Production_Status = e_Production_Status.READY; // Trạng thái không có PO hoặc đang chỉnh sửa
                    btnProductionDate.Text = "Chỉnh thông tin"; // Đặt lại văn bản nút
                    btnProductionDate.Symbol = 61508; // Thay đổi biểu tượng của nút btnPO
                    btnPO.Text = "ĐỔI PO"; // Đặt lại văn bản nút
                    btnPO.Symbol = 61508; // Đặt lại biểu tượng nút
                    btnPO.FillColor = Color.CornflowerBlue; // Đổi màu nền của nút btnPO về màu CornflowerBlue
                    ipOrderNO.FillColor = Color.CornflowerBlue; // Đổi màu nền của ô nhập Order No về màu CornflowerBlue
                    ipProductionDate.FillColor = Color.CornflowerBlue; // Đổi màu nền của ô nhập Production Date về màu CornflowerBlue
                    ipOrderNO.ForeColor = Color.White; // Đổi màu chữ của ô nhập Order No về màu trắng
                    ipProductionDate.ForeColor = Color.White; // Đổi màu chữ của ô nhập Production Date về màu trắng
                    // Cập nhật các thông tin PO đã chọn
                    SafeInvoke(() =>
                    {
                        opProductName.Text = GV.Selected_PO.productName;
                        opProductCode.Text = GV.Selected_PO.productCode;
                        opLotNumber.Text = GV.Selected_PO.lotNumber;
                        opGTIN.Text = GV.Selected_PO.GTIN;
                        opShift.Text = GV.Selected_PO.shift;
                        opFactory.Text = GV.Selected_PO.factory;
                        opSite.Text = GV.Selected_PO.site;
                        opOrderQty.Text = GV.Selected_PO.orderQty;
                        opCZCodeCount.Text = GV.Selected_PO.CodeCount;
                        opCZRunCount.Text = GV.Selected_PO.runInfo.total.ToString();
                        opPassCount.Text = GV.Selected_PO.runInfo.pass.ToString();
                        opFailCount.Text = (GV.Selected_PO.runInfo.total - GV.Selected_PO.runInfo.pass).ToString();
                        opDuplicateCount.Text = GV.Selected_PO.runInfo.duplicate.ToString();
                        //đặt trạng thái các nút
                        if (GV.Selected_PO.runInfo.pass <= 0)
                        {
                            btnPO.Enabled = true; // Hiển thị nút chỉnh PO
                            btnProductionDate.Enabled = true; // Hiển thị nút chỉnh Date

                        }
                        else
                        {
                            btnPO.Enabled = false; // ẩn nút chỉnh PO
                            btnProductionDate.Enabled = true; // ẩn nút chỉnh Date
                        }

                        opTer.Text = $"-";
                    });
                    //chuyển sang trạng thái READY
                    GV.Production_Status = e_Production_Status.CHECKING; // Trạng thái không có PO hoặc đang chỉnh sửa
                    break;

                case e_Production_Status.PUSHING:
                    break;
                case e_Production_Status.STOPPED:
                    break;
                case e_Production_Status.RUNNING:
                    break;
                case e_Production_Status.PAUSED:
                    break;
                case e_Production_Status.READY:
                    //kiểm tra xem đã PO đang dùng đã chạy hay chưa
                    GV.Selected_PO.runInfo.pass = poService.get.Get_Record_Product_Count(GV.Selected_PO.orderNo, e_Content_Result.PASS);
                    if (GV.Selected_PO.runInfo.pass > 0)
                    {
                        this.ShowErrorNotifier("PO đã chạy, không thể chỉnh sửa", false, 3000);
                        return;
                    }

                    //nếu chưa chạy thì cho phép chỉnh sửa
                    btnPO.Text = "LƯU LẠI"; // Đặt lại văn bản nút
                    btnPO.Symbol = 61468; // Đặt lại biểu tượng nút
                    btnPO.FillColor = Color.Green; // Đổi màu nền của nút btnPO
                    btnProductionDate.Enabled = false; // Hiển thị nút chỉnh Date
                    btnRUN.Enabled = false; // ẩn nút chạy
                    btnReLoad.Enabled = false; // ẩn nút tải lại
                    ipOrderNO.ReadOnly = false; //cho phép chỉnh sửa Order No
                    ipProductionDate.ReadOnly = false; //cho phép chỉnh sửa Production Date
                    ipProductionDate.FillColor = Color.Yellow; // Đổi màu nền của ô nhập ngày sản xuất
                    ipProductionDate.ForeColor = Color.Black; // Đổi màu chữ của ô nhập ngày sản xuất
                    ipOrderNO.FillColor = Color.Yellow; // Đổi màu nền của ô nhập Order No
                    ipOrderNO.ForeColor = Color.Black; // Đổi màu chữ của ô nhập Order No

                    //chuyển sang trạng thái chỉnh sửa PO
                    GV.Production_Status = e_Production_Status.EDITING; // Đặt trạng thái là đang chỉnh sửa

                    SafeInvoke(() =>
                    {
                        opTer.Text = $"Chọn orderNo, chỉnh ngày sản xuất và kiểm tra kỹ\r\n>> Nhấn LƯU LẠI để áp dụng PO";
                        poService.MES_Load_OrderNo_ToComboBox(ipOrderNO);
                        ipOrderNO.SelectedIndex = 0; // Chọn dòng đầu tiên (dòng rỗng)
                    });

                    break;
                case e_Production_Status.UNKNOWN:
                    break;
                case e_Production_Status.NOPO:
                    break;
                case e_Production_Status.STARTUP:
                    break;
                case e_Production_Status.LOAD:
                    break;
                case e_Production_Status.CHECKING:
                    break;
                case e_Production_Status.COMPLETE:

                    GV.Production_Status = e_Production_Status.EDITING; // Đặt trạng thái là đang chỉnh sửa
                    SafeInvoke(() =>
                    {
                        btnPO.Enabled = true; // ẩn nút chỉnh PO
                        btnProductionDate.Enabled = false; // Hiển thị nút chỉnh Date
                        btnRUN.Enabled = false;
                        btnReLoad.Enabled = false; // ẩn nút tải lại
                        btnPO.Text = "LƯU LẠI"; // Đặt lại văn bản nút
                        btnPO.Symbol = 61468; // Đặt lại biểu tượng nút
                        btnPO.FillColor = Color.Green; // Đổi màu nền của nút btnPO
                        ipOrderNO.ReadOnly = false; //cho phép chỉnh sửa Order No
                        ipProductionDate.ReadOnly = false; //cho phép chỉnh sửa Production Date
                        ipProductionDate.FillColor = Color.Yellow; // Đổi màu nền của ô nhập ngày sản xuất
                        ipProductionDate.ForeColor = Color.Black; // Đổi màu chữ của ô nhập ngày sản xuất
                        ipOrderNO.FillColor = Color.Yellow; // Đổi màu nền của ô nhập Order No
                        ipOrderNO.ForeColor = Color.Black; // Đổi màu chữ của ô nhập Order No
                        ipOrderNO.SelectedIndex = 0; // Chọn dòng đầu tiên (dòng rỗng)

                        opTer.Text = $"Chọn orderNo, chỉnh ngày sản xuất và kiểm tra kỹ\r\n>> Nhấn LƯU LẠI để áp dụng PO";
                        poService.MES_Load_OrderNo_ToComboBox(ipOrderNO);
                    });
                    // Chuyển sang trạng thái chỉnh sửa PO
                    
                    break;
            }
        }

        private void btnRUN_Click(object sender, EventArgs e)
        {
            //khởi động PO  
            //ghi logs người dùng nhấn nút chạy sản xuất
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Nhấn nút chạy sản xuất", "RUN", $"Người dùng {Globalvariable.CurrentUser.Username} nhấn nút chạy sản xuất ");
            //ghi logs vào hàng đợi
            LogQueue.Enqueue(systemLogs);
            //kiểm tra trạng thái hiện tại
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
                    //ghi logs người dùng dừng sản xuất
                    SystemLogs stopLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Nhấn nút dừng sản xuất", "RUN", $"Người dùng {Globalvariable.CurrentUser.Username} nhấn nút dừng sản xuất");
                    //ghi logs vào hàng đợi
                    LogQueue.Enqueue(stopLogs);
                    //tắt hết các nút
                   // btnRUN.Enabled = false; // ẩn nút chạy
                    //GV.Production_Status = e_Production_Status.READY;
                    break;
                case e_Production_Status.PAUSED:
                    break;
                case e_Production_Status.READY:

                    //kiểm tra hệ thống ready chưa
                    if (!Globalvariable.All_Ready)
                    {
                        this.ShowErrorNotifier("Hệ thống chưa Sẵn sàng, vui lòng kiểm tra các thiết bị", false, 3000);
                        return;
                    }
                    Push_Data_To_Dic();

                    btnPO.Enabled = false; // ẩn nút chỉnh PO
                    btnProductionDate.Enabled = false; // ẩn nút chỉnh Date
                    btnRUN.Text = "DỪNG SẢN XUẤT"; // Đặt lại văn bản nút
                    btnRUN.Symbol = 61516; // Đặt lại biểu tượng nút
                    btnRUN.FillColor = Color.Red; // Đổi màu nền của nút btnRUN về màu Red
                    btnRUN.ForeColor = Color.White; // Đổi màu chữ của nút btnRUN về màu trắng


                    //chuyển lên trạng thái runnung
                    GV.Production_Status = e_Production_Status.RUNNING;
                    break;
                case e_Production_Status.UNKNOWN:
                    break;
            }
        }
    }
}
