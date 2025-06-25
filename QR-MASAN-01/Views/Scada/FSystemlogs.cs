using DocumentFormat.OpenXml.Spreadsheet;
using QR_MASAN_01.Report;
using SpT;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QR_MASAN_01.SystemLogs;

namespace QR_MASAN_01.Views.Scada
{
    public partial class FSystemlogs : UIPage
    {
        public FSystemlogs()
        {
            InitializeComponent();
        }
        int size = 2; // Số lượng bản ghi mỗi trang
        public override void Init()
        {
            base.Init();
            if (!WK_AutoLog.IsBusy)
            {
                WK_AutoLog.RunWorkerAsync();
            }

            ipLoginType.DataSource = Enum.GetValues(typeof(SystemLogs.e_LogType)).Cast<SystemLogs.e_LogType>().ToList();
            uiPagination1.ActivePage = 1;
            size = Convert.ToInt32(ipSize.Text);
            //gán cho ipDateFrom -30 ngày, ipDateTo hiện tại
            ipDateFrom.Value = DateTime.Now.AddDays(-30);
            ipDateTo.Value = DateTime.Now;
        }

        private void uiPagination1_PageChanged(object sender, object pagingSource, int pageIndex, int count)
        {
            if (!WK_Getlogs.IsBusy)
            {
                WK_Getlogs.RunWorkerAsync();
            }
        }

        private void WK_AutoLog_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_AutoLog.CancellationPending)
            {
                //
            }
        }

        e_LogType LogType = e_LogType.SYSTEM_EVENT;

        int LogCount = 0;
        private void btnGetLogs_Click(object sender, EventArgs e)
        {
            //ghi nhận log người dùng nhấn nút lấy nhật ký hệ thống
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.USER_ACTION, "Lấy nhật ký hệ thống", Globalvariable.CurrentUser.Username, "Người dùng đã nhấn nút lấy nhật ký hệ thống");
            LogQueue.Enqueue(systemLogs);

            btnGetLogs.Enabled = false;
            
            if (!WK_Getlogs.IsBusy)
            {
                WK_Getlogs.RunWorkerAsync();
            }
        }

        DataTable LogsData;
        long DateFrom = 0;
        long DateTo = 0;
        bool getALL = false; // Biến này có thể dùng để xác định có lấy tất cả dữ liệu hay không
        private void WK_Getlogs_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    btnGetLogs.Enabled = false;
                    ipSize.Enabled = false;
                    LogType = (e_LogType)ipLoginType.SelectedIndex;
                    DateFrom = new DateTimeOffset(ipDateFrom.Value).ToUnixTimeSeconds();
                    DateTo = new DateTimeOffset(ipDateTo.Value).ToUnixTimeSeconds();

                }));

                LogCount = Get_Log_Count(LogType);
                // Lấy dữ liệu từ SQLite
                if (getALL)
                {
                    // Nếu getALL là true, lấy tất cả dữ liệu
                    LogsData = Get_Logs_From_SQLite(LogType, 0, LogCount, DateFrom, DateTo, true);
                    getALL = false; // Đặt lại biến getALL để không lấy tất cả dữ liệu trong lần tiếp theo  
                }
                else
                {
                    // Lấy dữ liệu theo phân trang
                    LogsData = Get_Logs_From_SQLite(LogType, uiPagination1.ActivePage - 1, size, DateFrom, DateTo);
                }
                    
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    btnGetLogs.Enabled = true;
                    ipSize.Enabled = true;
                    this.ShowErrorTip($"Lỗi khi lấy nhật ký hệ thống: {ex.Message}", 2000);
                }));
                return;
            }

        }

        //tạo hàm lấy log đăng nhập

        private void WK_Getlogs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.ShowSuccessTip("Lấy nhật ký hệ thống thành công", 2000);
            
            opTotalCount.Text = LogCount.ToString();
            btnGetLogs.Enabled = true;
            ipSize.Enabled = true;
            opDataG.DataSource = LogsData;
            uiPagination1.TotalCount = LogCount;
            uiPagination1.PageSize = size;
        }

        private void FSystemlogs_Initialize(object sender, EventArgs e)
        {
            //ghi nhận log người dùng bật form
            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.USER_ACTION, "Mở trang nhật ký hệ thống", Globalvariable.CurrentUser.Username, "Người dùng đã mở trang nhật ký hệ thống");
            // Ghi log sự kiện mở trang nhật ký hệ thống
            LogQueue.Enqueue(systemLogs);
        }
        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                string savePath = FileHelper.Get_Save_File_Path();
                if (!string.IsNullOrEmpty(savePath))
                {
                    // TODO: Gọi hàm xuất PDF với savePath
                   var ex_pdf = ReportClass.ExportReportToPDF(opDataG.DataSource as DataTable, savePath, ipLoginType.SelectedText);

                    if (ex_pdf.IsSucces)
                    {
                        //ghi nhận log người dùng xuất báo cáo thành công
                        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.USER_ACTION, "Xuất báo cáo nhật ký hệ thống thành công", Globalvariable.CurrentUser.Username, $"Người dùng đã xuất báo cáo nhật ký hệ thống thành công. Mã xác thực: {ex_pdf.HashCode}, Vị trí: {ex_pdf.FilePath}");
                        LogQueue.Enqueue(systemLogs);
                        //Lưu thông tin báo cáo
                        SystemLogs systemLogs2 = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.SAVE_LOG_EXPORT, "Xuất báo cáo pdf", Globalvariable.CurrentUser.Username, $"Mã xác thực: {ex_pdf.HashCode}, Vị trí: {ex_pdf.FilePath}");
                        LogQueue.Enqueue(systemLogs2);
                        // Hiển thị thông báo thành công
                        this.ShowSuccessTip($"Xuất báo cáo thành công! {ex_pdf.FilePath}", 2000);
                    }
                    else
                    {
                        this.ShowErrorTip("Xuất báo cáo thất bại!", 2000);
                    }
                 
                }
                else
                {
                    this.ShowErrorTip("Bạn đã hủy xuất báo cáo", 2000);
                    // Ghi nhận log người dùng hủy xuất báo cáo
                    SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.USER_ACTION, "Hủy xuất báo cáo nhật ký hệ thống", Globalvariable.CurrentUser.Username, "Người dùng đã hủy xuất báo cáo nhật ký hệ thống");
                    LogQueue.Enqueue(systemLogs);
                }
            }
            catch (Exception ex)
            {
                // Ghi nhận log lỗi khi xuất báo cáo
                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.ERROR, "REPORT - Lỗi xuất báo cáo nhật ký hệ thống", Globalvariable.CurrentUser.Username, ex.Message);
                LogQueue.Enqueue(systemLogs);
                this.ShowErrorTip($"Lỗi xuất báo cáo: {ex.Message}", 2000);
            }

        }

        private void ipSize_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {                 // Ghi nhận log người dùng thay đổi kích thước trang
                size = Convert.ToInt32(ipSize.Text);

                if (!WK_Getlogs.IsBusy)
                {
                    WK_Getlogs.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorTip($"Lỗi khi thay đổi kích thước trang: {ex.Message}", 2000);
            }
            
        }

        private void ipLoginType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {//xuất dữ liệu ra file csv
                string savePath = FileHelper.Get_Save_File_Path_CSV();
                if (!string.IsNullOrEmpty(savePath))
                {
                    var ex_csv = CsvHelper.ExportDataTableToCsv(opDataG.DataSource as DataTable, savePath);
                    if (ex_csv.IsSucces)
                    {
                        //ghi nhận log người dùng xuất báo cáo thành công
                        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.USER_ACTION, "Xuất tệp csv báo cáo", Globalvariable.CurrentUser.Username, $"Người dùng đã xuất báo cáo nhật ký hệ thống dạng csv thành công. Vị trí: {savePath}");
                        LogQueue.Enqueue(systemLogs);
                        //Lưu thông tin báo cáo
                        SystemLogs systemLogs2 = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.SAVE_LOG_EXPORT, "Xuất báo cáo csv", Globalvariable.CurrentUser.Username, $"Vị trí: {savePath}");
                        LogQueue.Enqueue(systemLogs2);
                        // Hiển thị thông báo thành công
                        this.ShowSuccessTip($"Xuất tệp csv thành công! {savePath}", 2000);
                    }
                    else
                    {
                        this.ShowErrorTip("Xuất CSV thất bại!", 2000);
                    }

                }
                else
                {
                    // Ghi nhận log người dùng hủy xuất báo cáo
                    this.ShowErrorTip("Bạn đã hủy xuất báo cáo", 2000);
                    // Ghi nhận log người dùng hủy xuất báo cáo
                    SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.USER_ACTION, "Hủy xuất báo cáo nhật ký hệ thống", Globalvariable.CurrentUser.Username, "Người dùng đã hủy xuất báo cáo nhật ký hệ thống");
                    LogQueue.Enqueue(systemLogs);
                }
            }
            catch (Exception ex)
            {
                // Ghi nhận log lỗi khi xuất báo cáo
                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_LogType.ERROR, "CSV - Lỗi xuất báo cáo nhật ký hệ thống", Globalvariable.CurrentUser.Username, ex.Message);
                LogQueue.Enqueue(systemLogs);
                this.ShowErrorTip($"Lỗi xuất báo cáo: {ex.Message}", 2000);
            }

        }

        private void btnGetAll_Click(object sender, EventArgs e)
        {
            //hiện bảng confirm lại đúng sai

            if (this.ShowAskDialog2("Bạn có chắc chắn tải hết dữ liệu? Máy sẽ có nguy cơ bị treo vài phút", true))
            {
                getALL = true; // Đặt biến getALL để lấy tất cả dữ liệu
                if (!WK_Getlogs.IsBusy)
                {
                    WK_Getlogs.RunWorkerAsync();
                }
            }
            else
            {

            }

        }
    }
}
