using Dialogs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public POService poService = new POService("C:/Users/THUC/source/repos/ErfanMNM/MASANSolution/Server_Service/po.db");

        //hàm khởi động Page
        public void INIT()
        {
            WK_Update.RunWorkerAsync(); // Bắt đầu chạy worker để cập nhật số lượng mã vạch đã chạy
            poService.Check_PO_Log_File();//Kiểm tra xem file PG log có tồn tại hay không, nếu không thì tạo mới
            poService.MES_Load_OrderNo_ToComboBox(ipOrderNO);

            //lấy PO dùng trước đó
            DataRow lastPO_Row = poService.Get_Last_Used_PO();
            string lastPO_string = lastPO_Row["orderNO"].ToString();

            if (lastPO_string != null)
            {
                bool found = ipOrderNO.Items.Cast<DataRowView>().Any(item =>
                                                            item["orderNO"].ToString() == lastPO_string);

                if (!found)
                {
                    ipOrderNO.SelectedIndex = 0; // Chọn dòng đầu tiên (dòng rỗng)
                }
                else
                {

                    ipOrderNO.SelectedValue = lastPO_Row["orderNO"].ToString();
                    ipProductionDate.Text = lastPO_Row["productionDate"].ToString();

                    //ghi logs 
                    //khởi động phần mềm
                    SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, "Khởi động PO", "PO", $"Bắt đầu khởi động {lastPO_Row["orderNO"]}");

                    //ghi logs vào hàng đợi
                    SystemLogs.LogQueue.Enqueue(systemLogs);

                    Globalvariable.PI_Status = e_PI_Status.READY; // Trạng thái không có PO hoặc đang chỉnh sửa
                }

            }
            else
            {
                ipOrderNO.SelectedIndex = 0; // Chọn dòng đầu tiên (dòng rỗng) 
            }

            //61508
            btnPO.Text = "Chỉnh thông tin";
            btnPO.Symbol = 61508; // Thay đổi biểu tượng của nút btnPO
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
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Nhấn nút chỉnh sửa PO", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} nhấn nút chỉnh sửa PO");
            //ghi logs vào hàng đợi


            if (Globalvariable.PI_Status != e_PI_Status.EDITING)
            {
                using (var dialog = new Pom_dialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        btnPO.Text = "Lưu thông tin";
                        btnPO.Symbol = 61468; // Thay đổi biểu tượng của nút btnPO
                        Globalvariable.PI_Status = e_PI_Status.EDITING; // Đặt trạng thái là đang chỉnh sửa
                        if(Globalvariable.Product_Active_Count < Globalvariable.Seleted_PO_Data.Rows[0]["orderQty"].ToString().ToInt() && Globalvariable.Product_Active_Count > 0)
                        {
                            ipProductionDate.ReadOnly = false; //cho phép chỉnh sửa date
                            ipProductionDate.FillColor = Color.Yellow; // Đổi màu nền của ô nhập ngày sản xuất
                            ipProductionDate.ForeColor = Color.Black; // Đổi màu chữ của ô nhập ngày sản xuất

                            //ghi logs chỉ chỉnh được ngày sản xuất
                            SystemLogs systemLogsEdit = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, "Chỉnh ProductionDate", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} chỉ chỉnh sửa ngày sản xuất của PO: {ipOrderNO.Text}");
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
                            SystemLogs systemLogsEdit = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, "Chỉnh sửa PO", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} người dùng bắt đầu chỉnh: {ipOrderNO.Text}");

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

                //ghi logs đổi PO thành công
                SystemLogs systemLogsSuccess = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.PO, "Đổi PO thành công", "PO", $"Người dùng {Globalvariable.CurrentUser.Username} đã đổi PO thành công: {ipOrderNO.Text}");
                //ghi logs vào hàng đợi
                SystemLogs.LogQueue.Enqueue(systemLogsSuccess);

                // Hiển thị thông báo thành công
                this.ShowSuccessTip("Thông tin PO đã được lưu thành công.");
                Globalvariable.Seleted_PO_Data = poService.Get_PO_Info_By_OrderNo(ipOrderNO.Text);
                Globalvariable.Seleted_PO_Data.Rows[0]["productionDate"] = ipProductionDate.Text;// Cập nhật ngày sản xuất trong theo user chỉnh sửa

                // Đặt lại trạng thái
                ipOrderNO.ReadOnly = true; // Không cho phép chỉnh sửa Order No
                ipProductionDate.ReadOnly = true; // Không cho phép chỉnh sửa Production Date

                Globalvariable.PI_Status = e_PI_Status.READY; // Trạng thái không có PO hoặc đang chỉnh sửa
                btnPO.Text = "Chỉnh thông tin";
                btnPO.Symbol = 61508; // Thay đổi biểu tượng của nút btnPO
            }


        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            //opCZCodeActiveCount.Text = poService.GetCZRunCount(Globalvariable.Seleted_PO_Data.Rows[0]["orderNO"].ToString()).ToString();
        }

        private void uiTableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_Update.CancellationPending)
            {
                if(Globalvariable.PI_Status == e_PI_Status.READY)
                {
                    this.Invoke(new Action(() =>
                    {
                        // Cập nhật số lượng mã vạch đã chạy
                        opCZRunCount.Text = Globalvariable.Product_Active_Count.ToString();
                    }));
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        // Cập nhật số lượng mã vạch đã chạy
                        opCZRunCount.Text = "-";
                    }));
                }
                Thread.Sleep(100); // Đợi 0.1 giây trước khi kiểm tra lại
            }
        }

        private void btnStopPO_Click(object sender, EventArgs e)
        {

        }

        private void FPI_Service_Initialize(object sender, EventArgs e)
        {
            //ghi logs người dùng khởi động trang FPI_Service
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffK"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.USER_ACTION, "Khởi động trang FPI_Service", "FPI_Service", $"Người dùng {Globalvariable.CurrentUser.Username} đã khởi động trang FPI_Service");
            //ghi logs vào hàng đợi
            SystemLogs.LogQueue.Enqueue(systemLogs);
        }
    }
}
