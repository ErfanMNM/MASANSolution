using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Profinet.Omron;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SPMS1
{
    public partial class OmronPLC_Hsl : Component
    {
        public string PLC_IP { get; set; } = "127.0.0.1";
        public int PLC_PORT { get; set; } = 9600;
        public string PLC_Ready_DM { get; set; } = "D16";
        public PLCStatus PLC_STATUS { get; set; } = PLCStatus.Disconnect;
        public int Ready { get; set; } = 0;
        public int Time_Update { get; set; } = 300;
        public OmronFinsUdp plc = new OmronFinsUdp();
        public event EventHandler<PLCStatusEventArgs> PLCStatus_OnChange;
        private readonly object updateSync = new object();
        private volatile bool updateDisposed;

        public OmronPLC_Hsl()
        {
            InitializeComponent();
        }

        public OmronPLC_Hsl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void InitPLC()
        {
            lock (updateSync)
            {
                if (updateDisposed) throw new ObjectDisposedException(nameof(OmronPLC_Hsl));
                if (WK_Update.IsBusy) return;
                plc.CommunicationPipe = new HslCommunication.Core.Pipe.PipeUdpNet(PLC_IP, PLC_PORT)
                {
                    ReceiveTimeOut = 1000,
                    SleepTime = 0,
                    SocketKeepAliveTime = -1,
                    IsPersistentConnection = true,
                };
                plc.PlcType = OmronPlcType.CSCJ;
                plc.SA1 = 1;
                plc.GCT = 2;
                plc.DA1 = 0;
                plc.SID = 0;
                plc.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;
                plc.ByteTransform.IsStringReverseByteWord = true;

                WK_Update.RunWorkerAsync();
            }
        }

        private void StopUpdate()
        {
            lock (updateSync)
            {
                updateDisposed = true;
                WK_Update.CancelAsync();
                Monitor.PulseAll(updateSync);
            }
        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                OperateResult write;
                lock (updateSync)
                {
                    if (updateDisposed || WK_Update.CancellationPending) return;
                    Monitor.Wait(updateSync, Time_Update);
                    if (updateDisposed || WK_Update.CancellationPending) return;
                    // Dispose can wait for an in-flight SDK write, never a UI callback.
                    write = plc.Write(PLC_Ready_DM, short.Parse(Ready.ToString()));
                }
                if (updateDisposed || WK_Update.CancellationPending) return;

                if (write.IsSuccess)
                {
                    if(PLC_STATUS == PLCStatus.Connecting)
                    {
                        
                    }
                    else
                    {
                        PLC_STATUS = PLCStatus.Connecting;
                        PLCStatus_OnChange?.Invoke(this, new PLCStatusEventArgs(PLCStatus.Connecting, "PLC đã kết nối"));
                    }
                }
                else
                {
                    if (PLC_STATUS == PLCStatus.Disconnect)
                    {

                    }
                    else
                    {
                        PLC_STATUS = PLCStatus.Disconnect;
                        PLCStatus_OnChange?.Invoke(this, new PLCStatusEventArgs(PLCStatus.Disconnect, "PLC mất kết nối"));
                    }
                }
            }
        }

        public enum PLCStatus
        {
            Connecting = 0,
            Disconnect = 1,
        }

        public class PLCStatusEventArgs : EventArgs
        {
            public PLCStatus Status { get; set; }
            public string Message { get; set; }

            public PLCStatusEventArgs(PLCStatus status, string message)
            {
                Status = status;
                Message = message;
            }
        }
    }
}
