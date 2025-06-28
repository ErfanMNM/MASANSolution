using QR_MASAN_01.Views.Scada;
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
            while (!WK_Update.CancellationPending)
            {
                //hiện các thành phần trong Globalvariale.GCounter.xxx lên màn hình
                this.Invoke(new Action(() =>
                {
                    // Cập nhật các thành phần UI với giá trị từ Globavariale.GCounter
                    opTotalC1.Text = Globalvariable.GCounter.Total_C1.ToString();
                    opFailC1.Text = Globalvariable.GCounter.Total_Failed_C1.ToString();
                    opPassC1.Text = Globalvariable.GCounter.Total_Pass_C1.ToString();

                    opReadFailC1.Text = Globalvariable.GCounter.Total_Failed_C1.ToString();
                    opFormatC1.Text = Globalvariable.GCounter.Format_C1.ToString();
                    opDuplicateC1.Text = Globalvariable.GCounter.Duplicate_C1.ToString();

                    opReworkC1.Text = Globalvariable.GCounter.Rework_C1.ToString();
                    opNotFoundC1.Text = Globalvariable.GCounter.NotFound_C1.ToString();
                    opUnknowC1.Text = Globalvariable.GCounter.Unknown_C2.ToString();

                    opPLCSend0OKC1.Text = Globalvariable.GCounter.PLC_0_Pass_C1.ToString();
                    opPLCSend1OKC1.Text = Globalvariable.GCounter.PLC_1_Pass_C1.ToString();
                    opPLCSend0FailC1.Text = Globalvariable.GCounter.PLC_0_Fail_C1.ToString();

                    
                }));

                Thread.Sleep(1000); // Cập nhật mỗi giây
            }
        }
    }
}
