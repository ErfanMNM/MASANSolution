using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPMS1
{
    public enum Enum_SocketServer
    {
        StartServer,
        StopServer,
        ClientConnect,
        ClientDisconnect,
        ReceiveData
    }
    public partial class Socket_Server_TCP : Component
    {
        public Socket_Server_TCP()
        {
            InitializeComponent();
        }

        public Socket_Server_TCP(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }



        private AsyncCallback pfnWorkerCallBack;
        private Socket m_socListener;
        private Socket m_socWorker;
        public Int32 Port { get; set; }
        string IPNAME = "127.0.0.1";
        public string IP
        {
            get { return IPNAME; }
            set { IPNAME = value; }
        }
        public string ReceiveData = "";

        public delegate void SocketServerHandler(Enum_SocketServer e, string s);

        public event SocketServerHandler SockServer;
        private BackgroundWorker worker = new BackgroundWorker();
        bool statup = true;
        private void CheckConnection()
        {
            worker.DoWork += Worker_DoWork;
        }
        bool a = true;
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            while (a)
            {
                Thread.Sleep(1000); // kiểm tra 1s/lần
                if (!m_socWorker.Connected)
                {
                    OnClientDisconnect();

                    return;
                }
            }
        }

        public void StartServer()
        {
            try
            {
                m_socListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(IP);
                IPEndPoint ipLocal = new IPEndPoint(ipAddress, Port);
                m_socListener.Bind(ipLocal);
                m_socListener.Listen(4);
                m_socListener.BeginAccept(new AsyncCallback(OnClientConnect), null);
                if (SockServer != null)
                {
                    SockServer.Invoke(Enum_SocketServer.StartServer, Port.ToString());
                }
            }
            catch (SocketException)
            {
            }
        }

        public void StopServer()
        {
            m_socWorker.Close();

            if (SockServer != null)
            {
                SockServer.Invoke(Enum_SocketServer.StopServer, Port.ToString());
            }
        }

        public void OnClientConnect(IAsyncResult asyn)
        {
            if (statup)
            {
                CheckConnection();
                statup = false;
            }
            if (!worker.IsBusy) { worker.RunWorkerAsync(); }
            try
            {
                m_socWorker = m_socListener.EndAccept(asyn);

                WaitForData(m_socWorker);
                if (SockServer != null)
                {
                    SockServer.Invoke(Enum_SocketServer.ClientConnect, m_socWorker.RemoteEndPoint.ToString());
                }
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\n OnClientConnection: Socket has been closed\n");
            }
            catch (SocketException)
            {
            }
        }
        private void OnClientDisconnect()
        {
            // đóng socket 
            m_socWorker.Close();
            a = false;

            // báo sự kiện client disconnect
            if (SockServer != null)
            {
                SockServer.Invoke(Enum_SocketServer.ClientDisconnect, "");
            }
            // bắt đầu nhận kết nối mới
            m_socListener.BeginAccept(OnClientConnect, null);
        }
        public class CSocketPacket
        {
            public System.Net.Sockets.Socket thisSocket;
            public byte[] dataBuffer = new byte[256];
        }

        public void WaitForData(System.Net.Sockets.Socket soc)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }
                CSocketPacket theSocPkt = new CSocketPacket();
                theSocPkt.thisSocket = soc;
                soc.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnWorkerCallBack, theSocPkt);

            }
            catch (SocketException)
            {
            }
        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                CSocketPacket theSockId = (CSocketPacket)asyn.AsyncState;
                int iRx = 0;
                iRx = theSockId.thisSocket.EndReceive(asyn);
                char[] chars = new char[iRx];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(theSockId.dataBuffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);
                ReceiveData = szData;
                if (!String.IsNullOrEmpty(szData))
                {
                    if (SockServer != null)
                    {
                        SockServer.Invoke(Enum_SocketServer.ReceiveData, szData);
                    }
                    WaitForData(m_socWorker);
                }
                else
                {
                    OnClientDisconnect();
                }
            }
            catch
            {
                // System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }

        }
        public bool Send(string Data)
        {
            try
            {
                if (!String.IsNullOrEmpty(Data) && m_socWorker != null)
                {
                    Object objData = Data;
                    byte[] byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                    m_socWorker.Send(byData);
                }
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}