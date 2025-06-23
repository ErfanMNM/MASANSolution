using QR_MASAN_01.Views.Scada;
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

namespace QR_MASAN_01.Views
{
    public partial class FStatistics : UIPage
    {
        public FStatistics()
        {
            InitializeComponent();
            
        }

        public void INIT()
        {
            if(!WK_Update.IsBusy)
            {
                WK_Update.CancelAsync();
            }
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            uiTabControl1.SelectedTab = uiTabControl1.TabPages["tabPage2"];
        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
