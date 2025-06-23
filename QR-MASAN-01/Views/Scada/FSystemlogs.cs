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

namespace QR_MASAN_01.Views.Scada
{
    public partial class FSystemlogs : UIPage
    {
        public FSystemlogs()
        {
            InitializeComponent();
        }

        public void INIT()
        {
            if (!WK_AutoLog.IsBusy)
            {
                WK_AutoLog.RunWorkerAsync();
            }
        }

        private void uiPagination1_PageChanged(object sender, object pagingSource, int pageIndex, int count)
        {

        }

        private void WK_AutoLog_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_AutoLog.CancellationPending)
            {
                // Kiểm tra hàng đợi và thêm bản ghi vào SQLite
                if (SystemLogs.LogQueue.Count > 0)
                {
                    SystemLogs.InsertToSQLite(SystemLogs.LogQueue.Dequeue());
                }
                // Đợi một khoảng thời gian trước khi kiểm tra lại
                System.Threading.Thread.Sleep(100); // 1 giây
            }
        }
    }
}
