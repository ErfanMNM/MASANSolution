using Dialogs;
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
            poService.CheckPOLog();
            poService.LoadOrderNoToComboBox(ipOrderNO);
            
            //lấy PO dùng trước đó
            DataRow lastPO = poService.GetLastUsedPO();
            string lastPOs = lastPO["orderNO"].ToString();
            if (lastPO != null)
            {
                //nếu không có trong cbb thì chọn dòng 1
                if (!ipOrderNO.Items.Contains(lastPO["orderNO"].ToString()))
                {
                    ipOrderNO.SelectedIndex = 0; // Chọn dòng đầu tiên (dòng rỗng)
                }
                else
                {
                    ipOrderNO.SelectedValue = lastPO["orderNO"].ToString();
                    ipProductionDate.Text = lastPO["productionDate"].ToString();
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
                Globalvariable.Seleted_PO_Data = poService.GetPOByOrderNo(ipOrderNO.SelectedText);
                if (Globalvariable.Seleted_PO_Data.Rows.Count > 0)
                {
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
                //
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

            if (Globalvariable.PI_Status != e_PI_Status.EDITING)
            {
                using (var dialog = new Pom_dialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        btnPO.Text = "Lưu thông tin";
                        btnPO.Symbol = 61468; // Thay đổi biểu tượng của nút btnPO
                        Globalvariable.PI_Status = e_PI_Status.EDITING; // Đặt trạng thái là đang chỉnh sửa
                                                                        //load lại dữ liệu PO
                        poService.LoadOrderNoToComboBox(ipOrderNO);
                        ipOrderNO.SelectedIndex = 0; // Chọn dòng đầu tiên (dòng rỗng)
                        ipOrderNO.ReadOnly = false; //cho phép chỉnh sửa
                        ipProductionDate.ReadOnly = false; //cho phép chỉnh sửa
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(dialog.Message))
                        {
                            opNotiBoard.Items.Add(dialog.Message);
                        }
                        else
                        {

                            return;
                        }

                    }
                }
            }
            else
            {

                // Cập nhật dữ liệu PO
                poService.CreateRunPO(ipOrderNO.Text, ipProductionDate.Text);

                // Hiển thị thông báo thành công
                this.ShowSuccessTip("Thông tin PO đã được lưu thành công.");

                Globalvariable.Seleted_PO_Data = poService.GetPOByOrderNo(ipOrderNO.Text);

                // Đặt lại trạng thái
                ipOrderNO.ReadOnly = true; // Không cho phép chỉnh sửa Order No
                ipProductionDate.ReadOnly = true; // Không cho phép chỉnh sửa Production Date

                Globalvariable.PI_Status = e_PI_Status.READY; // Trạng thái không có PO hoặc đang chỉnh sửa
                btnPO.Text = "Chỉnh thông tin";
                btnPO.Symbol = 61508; // Thay đổi biểu tượng của nút btnPO
            }
            
            
        }
    }
}
