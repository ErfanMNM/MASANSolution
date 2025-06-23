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
                WK_Update.RunWorkerAsync();
            }
        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
