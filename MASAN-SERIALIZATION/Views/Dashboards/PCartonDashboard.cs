using DATALOGIC_SCAN;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
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

        private void bw_update_ui(object sender, DoWorkEventArgs e)
        {
            while(!_bw_update_ui.CancellationPending)
            {
                try
                {
                    //this.InvokeIfRequired(() =>
                    //{
                    //    if (Globals.HandScan01_Connected)
                    //    {
                    //        if (tuStatePanel1.IsBlinking)
                    //        {
                    //            tuStatePanel1.Value = "Kết nối";
                    //            tuStatePanel1.IsOn = true;
                    //            tuStatePanel1.ONcolor = Color.Green;
                    //            tuStatePanel1.IsBlinking = false;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        if (!tuStatePanel1.IsBlinking)
                    //        {
                    //            tuStatePanel1.Value = "Mất kết nối";
                    //            tuStatePanel1.IsOn = false;
                    //            tuStatePanel1.ONcolor = Color.Red;
                    //            tuStatePanel1.IsBlinking = true;
                    //        }

                    //    }

                    //    // Cập nhật trạng thái kết nối của thiết bị thứ hai
                    //    if (Globals.HandScan02_Connected)
                    //    {
                    //        if (tuStatePanel2.IsBlinking)
                    //        {
                    //            tuStatePanel2.Value = "Kết nối";
                    //            tuStatePanel2.IsOn = true;
                    //            tuStatePanel2.ONcolor = Color.Green;
                    //            tuStatePanel2.IsBlinking = false;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (!tuStatePanel2.IsBlinking)
                    //        {
                    //            tuStatePanel2.Value = "Mất kết nối";
                    //            tuStatePanel2.IsOn = false;
                    //            tuStatePanel2.ONcolor = Color.Red;
                    //            tuStatePanel2.IsBlinking = true;
                    //        }
                    //    }
                    //});

                    if (Globals.Production_State == Production.e_Production_State.Running)
                    {
                        this.InvokeIfRequired(() =>
                        {
                            opCartonMaxID.Text = Globals.ProductionData.counter.cartonID.ToString();
                        });
                    }
                }
                catch (Exception ex)
                {
                    PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, " Lỗi PC01 :" + ex.Message);
                }

                Thread.Sleep(100); // Đợi 1 giây trước khi lặp lại
            }
        }

        #endregion


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
                    //lưu log scan
                    PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.DataChange, "Scan 02" + s.Trim(), s.Trim());
                    this.InvokeIfRequired(() =>{
                        opLane02.Items.Insert(0, s.Trim());
                    });
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

                    PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.DataChange, "Scan 02" + s.Trim(), s.Trim());
                    //kiểm tra mã thùng đã tồn tại hay chưa
                    if (Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonID.ToString() == s.Trim()))
                    {
                        this.ShowErrorDialog("Mã thùng đã tồn tại trong hệ thống");
                        return;
                    }

                    //kiểm tra xem thùng đang chạy chẵn hay lẻ
                    if (Globals.ProductionData.counter.cartonID % 2 == 0)
                    {
                        //Kiểm tra là scan mới hay active

                    }
                    else
                    {
                        //nếu thùng lẻ thì thêm vào danh sách thùng lẻ
                        
                    }


                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, s.Trim());
                            
                    });
                    break;
            }
        }
    }
}