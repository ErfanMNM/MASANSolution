using HslCommunication;
using HslCommunication.Profinet.Omron;
using MainClass;
using QR_MASAN_01.Views.Scada;
using QR_MASAN_01.Views.Settings;
using SpT.Static;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QR_MASAN_01.Views
{
    public partial class FStatistics : UIPage
    {

        Dictionary<PropertyInfo, panelS> bindings = new Dictionary<PropertyInfo, panelS>();
        public OmronFinsUdp plc = new OmronFinsUdp();
        public FStatistics()
        {
            InitializeComponent();
            
        }
        private List<int> baseAddresses = new List<int> { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150 };

        private void FillRows()
        {
            uiDataGridView1.Rows.Clear();
            foreach (var baseAddr in baseAddresses)
            {
                uiDataGridView1.Rows.Add(baseAddr.ToString());
            }
        }


        public void INIT()
        {
            if(!WK_Update.IsBusy)
            {
                WK_Update.RunWorkerAsync();
            }

            Type type = typeof(Counter_Info);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                panelS uc = new panelS() { LabelName = prop.Name };
                flowLayoutPanel1.Controls.Add(uc);
                bindings[prop] = uc;
            }
            uiDataGridView1.Columns.Add("Address", "Address");
            uiDataGridView1.Columns[0].ReadOnly = true; // Cột Address không cho edit
            //đổi màu cột Address
            uiDataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.Cyan;
            uiDataGridView1.Columns[0].Width = 100; // Cột Address rộng 100px
            uiDataGridView1.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            uiDataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            for (int offset = 0; offset <= 8; offset += 2)
            {
                uiDataGridView1.Columns.Add($"Col_{offset}", offset.ToString());
            }
            FillRows();

            plc.CommunicationPipe = new HslCommunication.Core.Pipe.PipeUdpNet(PLCAddress.Get("PLC_IP"), Convert.ToInt32(PLCAddress.Get("PLC_PORT")))
            {
                ReceiveTimeOut = 1000,
                SleepTime = 0,
                SocketKeepAliveTime = -1,
                IsPersistentConnection = true,
            };
            plc.PlcType = OmronPlcType.CSCJ;
            plc.SA1 = 1;
            plc.GCT = 2;
            plc.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;
            plc.ByteTransform.IsStringReverseByteWord = true;
        }

        //hàm invoke an toàn
        private void SafeInvoke(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Update.CancellationPending)
            {
                //hiện các thành phần trong Globalvariale.GCounter.xxx lên màn hình
                SafeInvoke(() =>
                {
                    foreach (var kv in bindings)
                    {
                        var prop = kv.Key;
                        var uc = kv.Value;

                        object value = prop.GetValue(Globalvariable.GCounter);
                        uc.LabelValue = value?.ToString() ?? "";
                    }
                });

                Thread.Sleep(1000); // Cập nhật mỗi giây
            }
        }

        bool isLoad = false;
        private void btnReloadPLC_Click(object sender, EventArgs e)
        {
            try
            {
                isLoad = true;
                OperateResult<int[]> read = plc.ReadInt32("D0", 160);
                if (read.IsSuccess)
                {
                    int[] values = read.Content;

                    for (int row = 0; row < baseAddresses.Count; row++)
                    {
                        int baseAddr = baseAddresses[row];
                        for (int col = 1; col <= 5; col++)
                        {
                            int offset = (col - 1) * 2;
                            int addr = baseAddr + offset;
                            int index = addr; // index trong mảng values

                            if (index >= 0 && index < values.Length)
                            {
                                uiDataGridView1.Rows[row].Cells[col].Value = values[index];
                            }
                            else
                            {
                                uiDataGridView1.Rows[row].Cells[col].Value = "N/A";
                            }
                        }
                    }
                }
                else
                {
                    this.ShowErrorNotifier($"Lỗi đọc dữ liệu từ PLC: {read.Message}");
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu có ngoại lệ
                this.ShowErrorNotifier($"Lỗi khi đọc dữ liệu từ PLC: {ex.Message}");
            }
            finally
            {
                isLoad = false;
            }
        }

        private void uiDataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isLoad) return; // Bỏ qua nếu đang load dữ liệu

            if (e.RowIndex >= 0 && e.ColumnIndex > 0) // Bỏ qua cột Address
            {
                try
                {
                    int baseAddr = Convert.ToInt32(uiDataGridView1.Rows[e.RowIndex].Cells[0].Value);
                    int offset = (e.ColumnIndex - 1) * 2; // Cột đầu tiên là Address, bắt đầu từ cột 1
                    int addr = baseAddr + offset;
                    if (int.TryParse(uiDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out int value))
                    {
                        //kiểm tra value phải là int32 
                        if (value < int.MinValue || value > int.MaxValue)
                        {
                            this.ShowErrorNotifier("Giá trị phải nằm trong khoảng int32.");
                            return;
                        }
                        OperateResult write = plc.Write("D" + addr.ToString(), value);
                        if (write.IsSuccess)
                        {
                            //báo thành công
                            this.ShowSuccessTip($"Ghi D{addr} thành công: {value}");
                        }
                        else
                        {
                            //báo lỗi
                            this.ShowErrorNotifier($"Lỗi ghi dữ liệu vào PLC: {write.Message} (D{addr})");
                        }
                    }
                    else
                    {
                        this.ShowErrorNotifier("Giá trị nhập vào không hợp lệ.");
                    }
                }
                catch (Exception ex)
                {
                    this.ShowErrorNotifier($"Lỗi ghi dữ liệu vào PLC: {ex.Message}");
                }
            }

        }
    }
}
