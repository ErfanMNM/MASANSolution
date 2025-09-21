using DATALOGIC_SCAN;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public partial class PCartonDashboard : UIPage
    {
        Connection _ScanConection01 = new Connection();
        Connection _ScanConection02 = new Connection();

        BackgroundWorker _bw_update_ui = new BackgroundWorker();

        //tạo file log 
        private LogHelper<e_LogType> PCLog;

        public PCartonDashboard()
        {
            InitializeComponent();
            PCLog = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "Pages", "PClog.ptl"));
        }

        #region Các hàm khởi tạo

        public void INIT()
        {
            if(AppConfigs.Current.cartonScanerMode != 1)
            {
                _ScanConection01.SERIALPORT = serialPort1;
                _ScanConection01.EVENT += _ScanConection01_EVENT;
                _ScanConection01.LOAD();
                _ScanConection01.CONNECT(AppConfigs.Current.HandScanCOM01);

                _ScanConection02.SERIALPORT = serialPort2;
                _ScanConection02.EVENT += _ScanConection02_EVENT;
                _ScanConection02.LOAD();
                _ScanConection02.CONNECT(AppConfigs.Current.HandScanCOM02);
            }
            else
            {
                tcpClient1.IP = AppConfigs.Current.cartonScanerTCP_IP;
                tcpClient1.Port = AppConfigs.Current.cartonScanerTCP_Port;

                tcpClient1.Connect();
            }
            _ScanConection01.SERIALPORT = serialPort1;
            _ScanConection01.EVENT += _ScanConection01_EVENT;
            _ScanConection01.LOAD();
            _ScanConection01.CONNECT(AppConfigs.Current.HandScanCOM01);

            _ScanConection02.SERIALPORT = serialPort2;
            _ScanConection02.EVENT += _ScanConection02_EVENT;
            _ScanConection02.LOAD();
            _ScanConection02.CONNECT(AppConfigs.Current.HandScanCOM02);

            _bw_update_ui.WorkerSupportsCancellation = true;
            _bw_update_ui.DoWork += bw_update_ui;

            if (!_bw_update_ui.IsBusy)
            {
                _bw_update_ui.RunWorkerAsync();
            }
            else
            {
                this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
            }


        }

        string lastWarning = string.Empty;
        private void bw_update_ui(object sender, DoWorkEventArgs e)
        {
            while (!_bw_update_ui.CancellationPending)
            {
                try
                {
                    int lastID = Globals.ProductionData.counter.cartonID - 1;
                    int nextID = Globals.ProductionData.counter.cartonID + 1;
                    this.InvokeIfRequired(() =>
                    {
                        uiLabel1.Text = Globals.test.ToString();
                        uiLabel2.Text = Globals.test2.ToString();

                        opCartonMaxID.Text = Globals.ProductionData.counter.cartonID.ToString();
                        opCartonCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData) ? cartonData.cartonCode : "Chưa có mã thùng";
                        opcartonPackCount.Text = Globals.ProductionData.counter.carton_Packing_Count.ToString();


                        opLastActive.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData1) ? cartonData1.Activate_Datetime : "Chưa có thời gian kích hoạt";
                        opLastID.Text = (Globals.ProductionData.counter.cartonID - 1).ToString();
                        opLastCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData2) ? cartonData2.cartonCode : "Chưa có mã thùng";

                        opnextID.Text = (Globals.ProductionData.counter.cartonID + 1).ToString();
                        opnextCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData3) ? cartonData3.cartonCode : "Chưa có mã thùng";
                        opnextStart.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData4) ? cartonData4.Start_Datetime : "Chưa có thời gian kích hoạt";

                        uiLabel3.Text = Globals.HandScan01_Connected.ToString() + "/" + Globals.HandScan02_Connected;

                        if (Globals.Canhbao != lastWarning)
                        {
                            lastWarning = Globals.Canhbao;
                            opWarning.Items.Insert(0, Globals.Canhbao);
                        }
                    });

                }
                catch (Exception ex)
                {
                    PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, " Lỗi PC01 :" + ex.Message);
                }

                Thread.Sleep(500); // Đợi 1 giây trước khi lặp lại
            }
        }

        #endregion

        //scan chẵn
        private void _ScanConection02_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    if (!Globals.HandScan02_Connected)
                    {
                        Globals.HandScan02_Connected = true;
                    }
                    break;
                case e_Serial.Disconnected:
                    if (Globals.HandScan02_Connected)
                    {
                        Globals.HandScan02_Connected = false;
                    }
                    break;
                case e_Serial.Recive:

                    this.InvokeIfRequired(() =>
                    {
                        opLane02.Items.Insert(0, s.Trim());
                    });

                    if (AppConfigs.Current.cartonScaner_Only_Once)
                    { 
                        break;
                    }
                    HandScan02_Process(s);
                    break;
            }
        }

        string saveLine = AppConfigs.Current.cartonCode_Line01;
        private void _ScanConection01_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    if (!Globals.HandScan01_Connected)
                    {
                        Globals.HandScan01_Connected = true;
                    }
                    break;
                case e_Serial.Disconnected:

                    if (Globals.HandScan01_Connected)
                    {
                        Globals.HandScan01_Connected = false;
                    }
                    break;
                case e_Serial.Recive:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, s.Trim());
                        opLane01.Items.Insert(0, saveLine);
                    });

                    if (AppConfigs.Current.cartonScaner_Only_Once)
                    {
                        if (s.Contains(AppConfigs.Current.cartonCode_Line01) || s.Contains(AppConfigs.Current.cartonCode_Line02))
                        {
                            
                            saveLine = s.Trim();
                            this.InvokeIfRequired(() =>
                            {

                                opLane01.Items.Insert(0, "Đổi Line " + saveLine);
                            });
                        }
                        else
                        {
                            if(saveLine.Contains(AppConfigs.Current.cartonCode_Line01))
                            {
                                this.InvokeIfRequired(() =>
                                {

                                    opLane01.Items.Insert(0, "Xử lý Lane 01" + saveLine);
                                });
                                HandScan01_Process(s.Trim());
                            }
                            else if (saveLine.Contains(AppConfigs.Current.cartonCode_Line02))
                            {
                                this.InvokeIfRequired(() =>
                                {

                                    opLane01.Items.Insert(0, "Xử lý Lane 02" + saveLine);
                                });
                                HandScan02_Process(s.Trim());
                            }
                        }
                    }
                    else
                    {
                        HandScan01_Process(s);
                    }
                    
                    break;

            }


        }

        private void btnSend1_Click(object sender, EventArgs e)
        {
            HandScan01_Process(ipTest1.Text);
        }

        private void HandScan02_Process(string s)
        {
            //lưu log scan
            PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.DataChange, "Scan 02" + s.Trim(), s.Trim());
            //kiểm tra xem thùng đang chạy chẵn hay lẻ
            if (Globals.ProductionData.counter.cartonID % 2 != 0)
            {
                //thùng lẻ
                //kiểm tra xem thùng tiếp theo có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData))
                {
                    //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                    if (cartonData.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonData.cartonCode = s.Trim();
                            cartonData.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }

            }
            else
            {

                //thùng chẵn tức là thùng đang chạy
                //kiểm tra xem thùng chẵn có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDataL))
                {
                    //nếu thùng đã có mã thì không làm gì cả
                    if (cartonDataL.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);
                        }
                    }
                }
            }
        }

        private void HandScan01_Process(string s)
        {
            PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.DataChange, "Scan 01" + s.Trim(), s.Trim());

            this.InvokeIfRequired(() =>
            {
                opLane01.Items.Insert(0, s.Trim());
            });
            //kiểm tra xem thùng đang chạy chẵn hay lẻ
            if (Globals.ProductionData.counter.cartonID % 2 == 0)
            {
                //thùng chẵn
                //kiểm tra xem thùng tiếp theo có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData))
                {
                    //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                    if (cartonData.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonData.cartonCode = s.Trim();
                            cartonData.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }
            }
            else
            {
                //thùng lẻ tức là thùng đang chạy
                //kiểm tra xem thùng lẻ có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDataL))
                {
                    //nếu thùng đã có mã thì không làm gì cả
                    if (cartonDataL.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);

                            ////kết thúc thùng chẵn cũ
                            //cartonData.cartonCode = s.Trim();
                            //cartonData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }
            }
        }

        private void btnSend2_Click(object sender, EventArgs e)
        {
            HandScan02_Process(ipTest2.Text);
        }

        private void tcpClient1_ClientCallBack(SpT.Communications.TCP.enumClient state, string data)
        {
            switch (state)
            {
                case SpT.Communications.TCP.enumClient.CONNECTED:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Connected");
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.CONNECTED;
                    break;
                case SpT.Communications.TCP.enumClient.DISCONNECTED:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Disconnected" );
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.DISCONNECTED;
                    break;
                case SpT.Communications.TCP.enumClient.RECEIVED:
                    //phân tách ký tự <p> để lấy thùng bên nào ví dụ SC1<p>123456789 là thùng bên 1

                    string lane = data.Split(new string[] { "<p>" }, StringSplitOptions.None)[0];
                    string codeContent = data.Split(new string[] { "<p>" }, StringSplitOptions.None)[1];

                    if (lane != "SC1")
                    {
                        HandScan01_Process(codeContent);
                    }
                    else
                    {
                        HandScan02_Process(codeContent);
                    }

                    break;
                case SpT.Communications.TCP.enumClient.RECONNECT:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Reconnect");
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.RECONNECTING;
                    break;
            }
        }

        private void btnNextCarton_Click(object sender, EventArgs e)
        {

            if(ipTest1.Text != "secrettantien512")
            {
                this.ShowErrorDialog("Vui lòng không nhấn thử");
                return;
            }
            //kiểm tra thùng hiện tại đã có mã chưa
            if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData))
            {
                if (cartonData.cartonCode == "0")
                {
                    this.ShowErrorDialog("Thùng hiện tại chưa có mã thùng. Vui lòng quét mã thùng trước khi chuyển sang thùng mới.");
                    return;
                }
            }


            Globals.ProductionData.counter.cartonID += 1;
            Globals.ProductionData.counter.carton_Packing_Count = 0;

            //thêm thùng mới vào csdl

        }
    }
}