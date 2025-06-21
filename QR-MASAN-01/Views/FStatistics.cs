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
        FCameraS _cameraS = new FCameraS();
        public FStatistics()
        {
            InitializeComponent();
            
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            uiTabControl1.SelectedTab = uiTabControl1.TabPages["tabPage2"];
        }
    }
}
