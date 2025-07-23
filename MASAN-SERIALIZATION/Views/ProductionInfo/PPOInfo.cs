using MASAN_SERIALIZATION.Enums;
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

namespace MASAN_SERIALIZATION.Views.ProductionInfo
{
    public partial class PPOInfo : UIPage
    {
        public PPOInfo()
        {
            InitializeComponent();
        }

        #region Sự kiện giao diện
        private void PPOInfo_Initialize(object sender, EventArgs e)
        {
            //ghi log khởi tạo giao diện
            Globals.Log.WriteLogAsync(Globals.CurrentUser.Username,e_LogType.UserAction,"Người dùng mở giao diện chỉnh thông tin sản xuất");
        }

        private void PPOInfo_Finalize(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
