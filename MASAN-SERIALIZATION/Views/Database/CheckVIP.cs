using DATALOGIC_SCAN;
using MASAN_SERIALIZATION.Configs;
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

        Connection _ScanConection01 = new Connection();
        public CheckVIP()
        {
            InitializeComponent();
        }

        public void START()
        {
            // Khởi tạo kết nối với thiết bị quét mã vạch
            _ScanConection01.SERIALPORT = serialPort1;
            _ScanConection01.EVENT += _ScanConection01_EVENT;
            _ScanConection01.LOAD();
            _ScanConection01.CONNECT(AppConfigs.Current.HandScanCOMMain);
        }
        int scan = 0;
        private void _ScanConection01_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    break;
                case e_Serial.Disconnected:

                    break;
                case e_Serial.Recive:
                    if (!string.IsNullOrEmpty(s))
                    {
                        string scannedCode = s.Trim();
                        if(scan ==1)
                        {
                            return;
                        }

                        ProcessScannedCode(scannedCode);
                    }
                    break;
            }
        }

        
        public void ProcessScannedCode(string Code)
        {
            Task.Run(() =>
            {
                scan = 1;
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


                scan = 0;
            });
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
