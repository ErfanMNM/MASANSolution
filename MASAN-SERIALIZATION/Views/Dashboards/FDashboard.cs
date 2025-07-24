using MASAN_SERIALIZATION.Enums;
using SpT.Communications.TCP;
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

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public partial class FDashboard : UIPage
    {
        public FDashboard()
        {
            InitializeComponent();
        }

        private void uiTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #region Sự kiện camera trả về

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
                    // Xử lý dữ liệu nhận được từ camera chính nếu cần
                    CameraMain_Process(data);
                    break;
                case enumClient.RECONNECT:

                    if (Globals.CameraMain_State != e_Camera_State.RECONNECTING)
                    {
                        Globals.CameraMain_State = e_Camera_State.RECONNECTING;
                    }
                    break;
            }
        }

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
        #endregion

        #region Xử lý dữ liệu camera

        private void CameraMain_Process(string _data)
        {
            // Xử lý dữ liệu từ camera chính
            // Ví dụ: Cập nhật giao diện, lưu trữ dữ liệu, v.v.
        }

        private void CameraSub_Process(string _data)
        {
            // Xử lý dữ liệu từ camera phụ
            // Ví dụ: Cập nhật giao diện, lưu trữ dữ liệu, v.v.
        }

        #endregion
    }
}
