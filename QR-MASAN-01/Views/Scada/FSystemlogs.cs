using DocumentFormat.OpenXml.Spreadsheet;
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
                // Kiểm tra hàng đợi và thêm bản ghi vào SQLite
                if (LogQueue.Count > 0)
                {
                    InsertToSQLite(LogQueue.Dequeue());
                }
                // Đợi một khoảng thời gian trước khi kiểm tra lại
                System.Threading.Thread.Sleep(100); // 1 giây
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
        private void WK_Getlogs_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                btnGetLogs.Enabled = false;
                ipSize.Enabled = false;
                LogType = (e_LogType)ipLoginType.SelectedIndex;
            }));
            
            LogCount = Get_Log_Count(LogType);
            LogsData = Get_Logs_From_SQLite(LogType, uiPagination1.ActivePage - 1, size);
        }

        //tạo hàm lấy log đăng nhập

        private void WK_Getlogs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.ShowSuccessTip("Lấy nhật ký hệ thống thành công", 2000);
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

        private void ipSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            size = Convert.ToInt32(ipSize.Text);
            if (!WK_Getlogs.IsBusy)
            {
                WK_Getlogs.RunWorkerAsync();
            }
        }
    }
}
