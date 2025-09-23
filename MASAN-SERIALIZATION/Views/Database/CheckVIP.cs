using MASAN_SERIALIZATION.Production;
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

namespace MASAN_SERIALIZATION.Views.Database
{
    public partial class CheckVIP : UIPage
    {
        public CheckVIP()
        {
            InitializeComponent();
        }

        private void btnfind_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
               TResult resultCode = Globals.ProductionData.getDataPO.getCodeInfo(ipCode.Text.Trim(), Globals.ProductionData.orderNo);
                uiDataGridView1.DataSource = null;
                if (resultCode.issuccess)
                {
                    TResult resultVIP = Globals.ProductionData.getDataPO.getCodeInfoWithCartonCode(Globals.ProductionData.orderNo, resultCode.data.Rows[0]["cartonCode"].ToString());
                     
                    Invoke(new Action(() =>
                    {
                        if (resultVIP.issuccess)
                        {
                            uiDataGridView1.DataSource = resultVIP.data;
                        }
                        opCodeInfo.Text = $"MÃ: {resultCode.data.Rows[0]["Code"].ToString()} | Trạng thái :{resultCode.data.Rows[0]["Status"].ToString()} | Thời gian kích hoạt : {resultCode.data.Rows[0]["ActivateDate"].ToString()}";
                        opCaseCode.Text = $"MÃ THÙNG: {resultCode.data.Rows[0]["cartonCode"].ToString()}";
                    }));

                    
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        opCodeInfo.Text = $"Không tìm thấy code {ipCode.Text.Trim()} trong PO {Globals.ProductionData.orderNo}";
                        opCaseCode.Text = $"";
                    }));
                }



            });
        }

        private void btnDeleteCarton_Click(object sender, EventArgs e)
        {
            

            //ProductionCartonData cartonData = new ProductionCartonData();
            //cartonData.cartonID = Globals_Database.Dictionary_ProductionCarton_Data.Count + 1;
            //cartonData.cartonCode = "0";
            //cartonData.Activate_Datetime = "0";
            //cartonData.Activate_User = "0";
            //cartonData.Production_Datetime = "0";
            //cartonData.orderNo = Globals.ProductionData.orderNo;
            //cartonData.Start_Datetime = "0";
            //Globals_Database.Dictionary_ProductionCarton_Data.Add(cartonData.cartonID, cartonData);
            ////thêm cái thùng mới vào cuối để đủ thùng
            //Globals.ProductionData.setDB.Insert_Carton(cartonData, cartonData.orderNo);

        }
    }
}
