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


                        opLastActive.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID -1, out ProductionCartonData cartonData1) ? cartonData1.Activate_Datetime : "Chưa có thời gian kích hoạt";
                        opLastID.Text = (Globals.ProductionData.counter.cartonID - 1).ToString();
                        opLastCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData2) ? cartonData2.cartonCode : "Chưa có mã thùng";

                        opnextID.Text = (Globals.ProductionData.counter.cartonID + 1).ToString();
                        opnextCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData3) ? cartonData3.cartonCode : "Chưa có mã thùng";
                        opnextStart.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData4) ? cartonData4.Start_Datetime : "Chưa có thời gian kích hoạt";

                        uiLabel3.Text = Globals.HandScan01_Connected.ToString() +"/" + Globals.HandScan02_Connected;

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
                    HandScan02_Process(s);
                    break;
            }
        }

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

                    HandScan01_Process(s);


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
                //kiểm tra xem thùng xếp trước đó kết thúc chưa (thùng chẵn cũ)
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData))
                {
                    //nếu thùng chưa kết thúc thì kết thúc
                    if (cartonData.Activate_Datetime == "0")
                    {

                        //nếu ở chế độ auto start thì kết thúc thùng luôn
                        if (AppConfigs.Current.cartonAutoStart)
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

                            cartonData.cartonCode = s.Trim();
                            cartonData.Activate_Datetime = DateTime.Now.ToString("o");
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            Globals_Database.Activate_Carton.Enqueue(s.Trim());
                            return;
                        }

                        //kiểm tra mã có trùng không
                        if (s.Trim() != cartonData.cartonCode)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng không trùng với mã thùng đang xếp. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }

                        //kết thúc thùng chẵn cũ
                        cartonData.cartonCode = s.Trim();
                        cartonData.Activate_Datetime = DateTime.Now.ToString("o");
                        Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                        Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        return;

                    }
                    else
                    {
                        //ở chế độ auto start thì không làm gì cả chỉ chửi
                        if (AppConfigs.Current.cartonAutoStart)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("02 Thùng đang xếp, không thể kích hoạt", false, 5000);
                            });
                            return;
                        }

                        //nếu là thùng 24 thì kích hoạt


                        //nếu thùng đã kết thúc thì kiểm tra thùng tiếp theo có mã chưa
                        if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData nextCartonData))
                        {
                            //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                            if (nextCartonData.Start_Datetime != "0")
                            {
                                //không làm gì cả
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
                                    nextCartonData.cartonCode = s.Trim();
                                    nextCartonData.Start_Datetime = DateTime.Now.ToString("o");
                                    //thêm vào hàng chờ cập nhật
                                    Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(nextCartonData);
                                }
                            }
                        }

                    }
                }
                else
                {
                    //nếu thùng đã kết thúc thì kiểm tra thùng tiếp theo có mã chưa
                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData nextCartonData))
                    {
                        //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                        if (nextCartonData.Start_Datetime != "0")
                        {
                            //không làm gì cả
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
                                nextCartonData.cartonCode = s.Trim();
                                nextCartonData.Start_Datetime = DateTime.Now.ToString("o");
                                //thêm vào hàng chờ cập nhật
                                Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(nextCartonData);
                            }
                        }
                    }
                }
            }
            else
            {
                //nếu ở chế độ auto start thì không làm gì cả chỉ chửi
                if (AppConfigs.Current.cartonAutoStart)
                {
                    this.InvokeIfRequired(() =>
                    {
                        this.ShowErrorNotifier("02 Thùng đang xếp, không thể kích hoạt", false, 5000);
                    });
                    return;
                }
                //nếu thùng lẻ thì có nghĩa là đang chạy, chỉ kiểm tra có mã start chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDataL))
                {
                    //nếu thùng đã có mã start thì không làm gì cả, gửi thông báo chửi vì thùng phải kết thúc mới được quét mã
                    if (cartonDataL.Start_Datetime != "0")
                    {

                        if (Globals.Production_State == e_Production_State.Waiting_Stop)
                        {
                            //kiểm tra mã có trùng không
                            if (s.Trim() != cartonDataL.cartonCode)
                            {
                                this.InvokeIfRequired(() =>
                                {
                                    this.ShowErrorNotifier("Mã thùng không trùng với mã thùng đang xếp. Vui lòng kiểm tra lại!", false, 5000);
                                });
                                return;
                            }

                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Activate_Datetime = DateTime.Now.ToString("o");
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);
                            Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }

                        this.InvokeIfRequired(() =>
                        {
                            this.ShowErrorNotifier("02 Thùng đã kích hoạt và đang trong thời gian xếp, vui lòng thử lại", false, 5000);
                        });
                        return;

                    }
                    else
                    {
                        //nếu thùng chưa có mã start thì cập nhật mã thùng mới
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
                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Start_Datetime = DateTime.Now.ToString("o");
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
                //thùng đang xếp là thùng chẵn
                //kiểm tra xem thùng xếp trước đó kết thúc chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData))
                {
                    //nếu thùng chưa chốt
                    if (cartonData.Activate_Datetime == "0")
                    {
                        //nếu ở chế độ auto start thì kết thúc thùng luôn
                        if (AppConfigs.Current.cartonAutoStart)
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

                            cartonData.cartonCode = s.Trim();
                            cartonData.Activate_Datetime = DateTime.Now.ToString("o");
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            Globals_Database.Activate_Carton.Enqueue(s.Trim());
                            return;
                        }

                        //kiểm tra mã có trùng không
                        if (s.Trim() != cartonData.cartonCode)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng không trùng với mã thùng đang xếp. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }

                        cartonData.cartonCode = s.Trim();
                        cartonData.Activate_Datetime = DateTime.Now.ToString("o");
                        Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                        Globals_Database.Activate_Carton.Enqueue(s.Trim());
                    }
                    else
                    {
                        //không ở chế độ auto start thì không làm gì cả chỉ chửi
                        if (AppConfigs.Current.cartonAutoStart)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("01 Thùng đang xếp, không thể kích hoạt", false, 5000);
                            });
                            return;
                        }
                        //nếu thùng đã kết thúc thì kiểm tra thùng tiếp theo có mã chưa
                        if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData nextCartonData))
                        {
                            //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                            if (nextCartonData.Start_Datetime != "0")
                            {
                                //không làm gì cả
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
                                    nextCartonData.cartonCode = s.Trim();
                                    nextCartonData.Start_Datetime = DateTime.Now.ToString("o");
                                    //thêm vào hàng chờ cập nhật
                                    Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(nextCartonData);
                                }
                            }
                        }

                    }
                }
                else
                {
                    //ở chế độ auto start thì không làm gì cả chỉ chửi
                    if (AppConfigs.Current.cartonAutoStart)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            this.ShowErrorNotifier("01 Thùng đang xếp, không thể kích hoạt", false, 5000);
                        });
                        return;
                    }
                    //nếu thùng đã kết thúc thì kiểm tra thùng tiếp theo có mã chưa
                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData nextCartonData))
                    {
                        //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                        if (nextCartonData.Start_Datetime != "0")
                        {
                            //không làm gì cả
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
                                nextCartonData.cartonCode = s.Trim();
                                nextCartonData.Start_Datetime = DateTime.Now.ToString("o");
                                //thêm vào hàng chờ cập nhật
                                Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(nextCartonData);
                            }
                        }
                    }
                }
            }
            else
            {
                //nếu ở chế độ auto start thì không làm gì cả chỉ chửi
                if (AppConfigs.Current.cartonAutoStart)
                {
                    this.InvokeIfRequired(() =>
                    {
                        this.ShowErrorNotifier("01 Thùng đang xếp, không thể kích hoạt", false, 5000);
                    });
                    return;
                }

                //nếu thùng lẻ thì có nghĩa là đang chạy, chỉ kiểm tra có mã start chưa

                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDataL))
                {
                    //nếu thùng đã có mã start thì không làm gì cả, gửi thông báo chửi vì thùng phải kết thúc mới được quét mã
                    if (cartonDataL.Start_Datetime != "0")
                    {
                        //nếu là thùng cuối thì kích hoạt mã

                        if (Globals.Production_State == e_Production_State.Waiting_Stop)
                        {
                            //kiểm tra mã có trùng không
                            if (s.Trim() != cartonDataL.cartonCode)
                            {
                                this.InvokeIfRequired(() =>
                                {
                                    this.ShowErrorNotifier("Mã thùng không trùng với mã thùng đang xếp. Vui lòng kiểm tra lại!", false, 5000);
                                });
                                return;
                            }

                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Activate_Datetime = DateTime.Now.ToString("o");
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);
                            Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }

                        this.InvokeIfRequired(() =>
                        {
                            this.ShowErrorNotifier("Thùng đã kích hoạt và đang trong thời gian xếp, vui lòng thử lại", false, 5000);
                        });
                        return;

                    }
                    else
                    {
                        //nếu thùng chưa có mã start thì cập nhật mã thùng mới
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
                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Start_Datetime = DateTime.Now.ToString("o");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);
                        }
                    }
                }

            }
        }

        private void btnSend2_Click(object sender, EventArgs e)
        {
            HandScan02_Process(ipTest2.Text);
        }
    }
}