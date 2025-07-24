using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Utils;
using SpT.Auth;
using SpT.Communications.TCP;
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
using static SPMS1.OmronPLC_Hsl;

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public partial class FDashboard : UIPage
    {
        public TaskManagerHelper Dashboard_taskManager = new TaskManagerHelper();

        public FDashboard()
        {
            InitializeComponent();
        }

        #region Sự kiện khởi tạo giao diện

        public void STARTUP()
        {
            InitializeDevices();
            InitializeTasks();
        }
        #endregion


        #region Sự kiện camera trả về của thiết bị
        //fix lỗi dội dữ liệu từ camera chính
        long lastReciveCM = 0;
        string lastDataCM = "";
        private void CameraMain_ClientCallBack(enumClient state, string data)
        {
            switch (state)
            {
                case enumClient.CONNECTED:

                    if (Globals.CameraMain_State != e_Camera_State.CONNECTED)
                    {
                        Globals.CameraMain_State = e_Camera_State.CONNECTED;
                    }
                    break;
                case enumClient.DISCONNECTED:

                    if (Globals.CameraMain_State != e_Camera_State.DISCONNECTED)
                    {
                        Globals.CameraMain_State = e_Camera_State.DISCONNECTED;
                    }
                    break;
                case enumClient.RECEIVED:
                    Task.Run(() =>
                    {
                        CameraMain_Process(data);
                    });
                    
                    break;
                case enumClient.RECONNECT:

                    if (Globals.CameraMain_State != e_Camera_State.RECONNECTING)
                    {
                        Globals.CameraMain_State = e_Camera_State.RECONNECTING;
                    }
                    break;
            }
        }

        //fix lỗi dội dữ liệu từ camera phụ
        long lastReciveCS = 0;
        string lastDataCS = "";

        private void CameraSub_ClientCallBack(enumClient state, string data)
        {
            switch (state)
            {
                case enumClient.CONNECTED:
                    if (Globals.CameraSub_State != e_Camera_State.CONNECTED)
                    {
                        Globals.CameraSub_State = e_Camera_State.CONNECTED;
                    }
                    break;
                case enumClient.DISCONNECTED:
                    if (Globals.CameraSub_State != e_Camera_State.DISCONNECTED)
                    {
                        Globals.CameraSub_State = e_Camera_State.DISCONNECTED;
                    }
                    break;
                case enumClient.RECEIVED:
                    //fix double call
                    var a = DateTime.Now.Ticks - lastReciveCS;
                    var b = TimeSpan.TicksPerMillisecond * 100;
                    //tránh gửi trùng liên tiếp 2 lần thời gian hiện tại - lastRecive < 100ms nếu code mới nhận về giống với lần trước
                    if (a < b)
                    {
                        //nếu dữ liệu nhận về giống với lần trước thì không xử lý
                        if (data == lastDataCS)
                        {
                            this.InvokeIfRequired(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Chống dội Camerta sau");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            });
                            break;
                        }
                    }
                    CameraSub_Process(data);
                    break;
                case enumClient.RECONNECT:
                    if (Globals.CameraSub_State != e_Camera_State.RECONNECTING)
                    {
                        Globals.CameraSub_State = e_Camera_State.RECONNECTING;
                    }
                    break;
            }
        }

        private void OMRON_PLC_PLCStatus_OnChange(object sender, PLCStatusEventArgs e)
        {
            switch (e.Status)
            {
                case PLCStatus.Connecting:
                    if (!Globals.PLC_Connected)
                    {
                        Globals.PLC_Connected = true;
                    }
                    break;
                case PLCStatus.Disconnect:
                    if (Globals.PLC_Connected)
                    {
                        Globals.PLC_Connected = false;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Xử lý dữ liệu camera
        int ID = 0; // Biến toàn cục để lưu ID của camera chính
        private void CameraMain_Process(string _data)
        {

            this.InvokeIfRequired(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: #{ID} Camera chính nhận dữ liệu: {_data}");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                });

            //xử lý dữ liệu từ camera chính
            //tăng tổng số sản phẩm đã nhận
            Globals.ProductionData.counter.totalCount++;

            if (_data.IsNullOrEmpty())
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C2(e_Content_Result.EMPTY, "MÃ RỖNG");
                return;
            }
            if (_data == "FAIL")
            {
                //loại sản phẩm ngay lập tức
                Send_Result_Content_C2(e_Content_Result.FAIL, "KHÔNG ĐỌC ĐƯỢC");
                return;
            }

        }

        private void CameraSub_Process(string _data)
        {
            // Xử lý dữ liệu từ camera phụ
            // Ví dụ: Cập nhật giao diện, lưu trữ dữ liệu, v.v.
        }

        #endregion

        #region Các hàm khởi tạo

        public void InitializeDevices()
        {
            try
            {
                // Khởi tạo camera chính
                Camera_Main.IP = AppConfigs.Current.Camera_Main_IP;
                Camera_Main.Port = AppConfigs.Current.Camera_Main_Port;
                Camera_Main.Connect();
                // Khởi tạo camera phụ
                Camera_Sub.IP = AppConfigs.Current.Camera_Sub_IP;
                Camera_Sub.Port = AppConfigs.Current.Camera_Sub_Port;
                Camera_Sub.Connect();

                // PLC
                OMRON_PLC.PLC_IP = PLCAddress.Get("PLC_IP");
                OMRON_PLC.PLC_PORT = PLCAddress.Get("PLC_PORT").ToInt32();
                OMRON_PLC.PLC_Ready_DM = PLCAddress.Get("PLC_Ready_DM");
                OMRON_PLC.InitPLC();

            }
            catch (Exception ex)
            {
               this.ShowErrorNotifier("Lỗi D001 khi khởi tạo thiết bị: " + ex.Message);
            }
        }

        //khởi tạo task
        public void InitializeTasks()
        {
            try
            {
                // Khởi tạo các task cần thiết
                Dashboard_taskManager.Start("Device_State", async (cancellationToken, progress) =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        // Thực hiện công việc của task
                        UpdateDeviceState();
                        //delay 1 giây trước khi lặp lại
                        await Task.Delay(1000, cancellationToken);
                    }
                });

                //hàm tốc độ cao
                Dashboard_taskManager.Start("Update_Counter", async (cancellationToken, progress) =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        // Thực hiện công việc của task
                        await Task.Delay(100, cancellationToken);
                    }
                });

            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier("Lỗi D002 khi khởi tạo task: " + ex.Message);
            }
        }


        #endregion

        #region Các hàm cập nhật giao diện
        public void UpdateDeviceState()
        {
            UpdateCameraMainState();
            UpdateCameraSubState();
            UpdatePLCState();
        }

        public void UpdateCameraMainState()
        {
            this.InvokeIfRequired(() =>
            {
                if (Globals.CameraMain_State == e_Camera_State.CONNECTED)
                {
                    if (opCameraMain.Value != "Đã kết nối")
                    {
                        opCameraMain.IsBlinking = false;
                        opCameraMain.Value = "Đã kết nối";
                        opCameraMain.ONcolor = Color.Green;
                        opCameraMain.IsOn = true; // Đặt trạng thái ON cho opCameraMain
                    }

                }
                else if (Globals.CameraMain_State == e_Camera_State.DISCONNECTED)
                {
                    if (opCameraMain.Value != "Mất kết nối")
                    {
                        opCameraMain.IsBlinking = true;
                        opCameraMain.Value = "Mất kết nối";
                        opCameraMain.ONcolor = Color.Red;
                    }
                }
                else if (Globals.CameraMain_State == e_Camera_State.RECONNECTING)
                {
                    if (opCameraMain.Value != "Kết nối lại")
                    {
                        opCameraMain.IsBlinking = true;
                        opCameraMain.Value = "Kết nối lại";
                        opCameraMain.ONcolor = Color.Orange;
                    }
                }
            });
        }

        public void UpdateCameraSubState()
        {
            this.InvokeIfRequired(() =>
            {
                if (Globals.CameraSub_State == e_Camera_State.CONNECTED)
                {
                    if (opCameraSub.Value != "Đã kết nối")
                    {
                        opCameraSub.IsBlinking = false;
                        opCameraSub.Value = "Đã kết nối";
                        opCameraSub.ONcolor = Color.Green;
                        opCameraSub.IsOn = true; // Đặt trạng thái ON cho opCameraSub
                    }
                }
                else if (Globals.CameraSub_State == e_Camera_State.DISCONNECTED)
                {
                    if (opCameraSub.Value != "Mất kết nối")
                    {
                        opCameraSub.IsBlinking = true;
                        opCameraSub.Value = "Mất kết nối";
                        opCameraSub.ONcolor = Color.Red;
                    }
                }
                else if (Globals.CameraSub_State == e_Camera_State.RECONNECTING)
                {
                    if (opCameraSub.Value != "Kết nối lại")
                    {
                        opCameraSub.IsBlinking = true;
                        opCameraSub.Value = "Kết nối lại";
                        opCameraSub.ONcolor = Color.Orange;
                    }
                }
            });
        }

        public void UpdatePLCState() {
            this.InvokeIfRequired(() => {
                if (Globals.PLC_Connected)
                {
                    if (opPLCState.IsBlinking)
                    {
                        opPLCState.IsBlinking = false;
                        opPLCState.Value = "Đã kết nối";
                        opPLCState.ONcolor = Color.Green;
                        opPLCState.IsOn = true; // Đặt trạng thái ON cho opPLCState
                    }
                }
                else
                {
                    if (!opPLCState.IsBlinking)
                    {
                        opPLCState.IsBlinking = true;
                        opPLCState.Value = "Mất kết nối";
                        opPLCState.ONcolor = Color.Red;
                    }
                }
            });
        }

        #endregion

        
    }
}
