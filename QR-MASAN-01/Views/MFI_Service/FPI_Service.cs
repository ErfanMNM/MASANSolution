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

namespace QR_MASAN_01.Views.MFI_Service
{
    public partial class FPI_Service : UIPage
    {
        public FPI_Service()
        {
            InitializeComponent();
        }

        public POService poService = new POService("C:/Users/THUC/source/repos/ErfanMNM/MASANSolution/Server_Service/po.db");
        
        public void INIT()
        {
            poService.LoadOrderNoToComboBox(ipOrderNO);
            ipOrderNO.SelectedIndex = 0;
        }

        private void ipOrderNO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ipOrderNO.SelectedItem != null)
            {
                Globalvariable.Seleted_PO_Data = poService.GetPOByOrderNo(ipOrderNO.SelectedText);
                if (Globalvariable.Seleted_PO_Data.Rows.Count > 0)
                {
                    opProductionLine.Text = Globalvariable.Seleted_PO_Data.Rows[0]["productionLine"].ToString();
                    opOrderQty.Text = Globalvariable.Seleted_PO_Data.Rows[0]["orderQty"].ToString();
                }
            }
        }
    }
}
