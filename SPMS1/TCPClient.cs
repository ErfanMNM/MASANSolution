using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SPMS1
{
    public enum enumClient
    {
        CONNECTED,
        DISCONNECTED,
        RECEIVED,
        RECONNECT
    }
    public partial class TCPClient : Component
    {
        
        public TCPClient()
        {
            InitializeComponent();
        }

        public TCPClient(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        BackgroundWorker worker = new BackgroundWorker();
        bool STATUP = true;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!worker.CancellationPending)
            {
                while (!Connected)
                {
                    //ClientCallBack.Invoke(enumClient.RECONNECT, "RECONNECT");
                    Thread.Sleep(3000);
                    Connect();

                }
                
            }
            
        }

        public delegate void EventForClient(enumClient eAE, string _strData);

        public event EventForClient ClientCallBack;
        public string IP { get; set; }
        public int Port { get; set; }

        byte[] m_DataBuffer = new byte[512];
        IAsyncResult m_asynResult;
        private AsyncCallback pfnCallBack;
        private Socket client;
        public bool Connected { get; private set; } = false;
        public void Connect()
        {

            if (STATUP)
            {
                STATUP = false;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += Worker_DoWork;
                worker.RunWorkerAsync();
            }
            try
            {
                Ping pinger = new Ping();
                PingReply reply = pinger.Send(IP);
                if (reply.Status == IPStatus.Success)
                {
                    try
                    {
                        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPAddress Ip = IPAddress.Parse(IP);
                        int iPortNo = Convert.ToInt32(Port);
                        IPEndPoint ipEnd = new IPEndPoint(Ip, iPortNo);
                        client.Connect(ipEnd);
                        WaitForData();
                        if (ClientCallBack != null)
                        {
                            ClientCallBack.Invoke(enumClient.CONNECTED, "Connected");
                            Connected = true;
                        }
                    }
                    catch
                    {
                        Connected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                if(Connected != false)
                {
                    ClientCallBack.Invoke(enumClient.DISCONNECTED, $"Error: {ex.Message}");
                    Connected = false;
                }

            }

           
        }
        private void WaitForData()
        {
            try
            {
                if (pfnCallBack == null)
                {
                    pfnCallBack = new AsyncCallback(OnDataReceived);
                }
                CSocketPacket theSocPkt = new CSocketPacket();
                theSocPkt.thisSocket = client;
                m_asynResult = client.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnCallBack, theSocPkt);
            }
            catch (Exception ex)
            {
                ClientCallBack.Invoke(enumClient.DISCONNECTED, $"Error: {ex.Message}");
            }
        }
        private class CSocketPacket
        {
            public Socket thisSocket;
            public byte[] dataBuffer = new byte[256];
        }
        public void Disconnect()
        {
            try
            {
                if (Connected)
                {
                    if (client != null)
                    {
                        client.Disconnect(false);
                        Connected = false;
                        
                        if (ClientCallBack != null)
                        {
                            ClientCallBack.Invoke(enumClient.DISCONNECTED, "Disconnected");
                            Connected = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClientCallBack.Invoke(enumClient.DISCONNECTED, $"Error: {ex.Message}");
                Connected = false;
                
            }
        }
        public void Send(string data)
        {
            if (client != null && data != null)
            {
                try
                {
                    Object objData = data;
                    byte[] byData = Encoding.ASCII.GetBytes(objData.ToString());
                    client.Send(byData);
                }
                catch (SocketException ex)
                {
                    client = null;
                    if (ClientCallBack != null)
                    {
                        ClientCallBack.Invoke(enumClient.DISCONNECTED, $"Disconnected: {ex.Message}");
                        Connected = false;
                    }
                }
            }
        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                CSocketPacket theSockId = (CSocketPacket)asyn.AsyncState;
                int iRx = 0;
                iRx = theSockId.thisSocket.EndReceive(asyn);
                char[] chars = new char[iRx];
                Decoder d = Encoding.ASCII.GetDecoder();
                int charLen = d.GetChars(theSockId.dataBuffer, 0, iRx, chars, 0);
                String szData = new String(chars);
                if (ClientCallBack != null && !string.IsNullOrEmpty(szData))
                {
                    ClientCallBack.Invoke(enumClient.RECEIVED, szData);
                    Connected = true;

                }
                else if (ClientCallBack != null)
                {
                    Disconnect();
                    if (!worker.IsBusy)
                    {
                        worker.RunWorkerAsync();
                    }
                }
                WaitForData();
            }
            catch (Exception ex)
            {
                ClientCallBack.Invoke(enumClient.DISCONNECTED, $"Error: {ex.Message}");
            }
        }   
    }
}
