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
        private readonly object serverSync = new object();
        private bool serverDisposed;
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
        public void StartServer()
        {
            lock (serverSync)
            {
                if (serverDisposed) throw new ObjectDisposedException(nameof(Socket_Server_TCP));
                if (m_socListener != null) return;
                try
                {
                    m_socListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress ipAddress = IPAddress.Parse(IP);
                    IPEndPoint ipLocal = new IPEndPoint(ipAddress, Port);
                    m_socListener.Bind(ipLocal);
                    m_socListener.Listen(4);
                    m_socListener.BeginAccept(new AsyncCallback(OnClientConnect), m_socListener);
                }
                catch (SocketException)
                {
                    m_socListener?.Close();
                    m_socListener = null;
                    return;
                }
                catch
                {
                    m_socListener?.Close();
                    m_socListener = null;
                    throw;
                }
            }
            SockServer?.Invoke(Enum_SocketServer.StartServer, Port.ToString());
        }

        public void StopServer()
        {
            CloseServer();
            if (SockServer != null)
            {
                SockServer.Invoke(Enum_SocketServer.StopServer, Port.ToString());
            }
        }

        private void CloseServer()
        {
            lock (serverSync)
            {
                // Finish closing before a concurrent Start can rebind this port.
                m_socListener?.Close();
                m_socWorker?.Close();
                m_socListener = null;
                m_socWorker = null;
            }
        }

        private void DisposeServer()
        {
            lock (serverSync) { serverDisposed = true; }
            CloseServer();
        }

        public void OnClientConnect(IAsyncResult asyn)
        {
            Socket listener = (Socket)asyn.AsyncState;
            Socket accepted = null;
            try
            {
                accepted = listener.EndAccept(asyn);
                lock (serverSync)
                {
                    if (serverDisposed || !ReferenceEquals(listener, m_socListener))
                    {
                        accepted.Close();
                        return;
                    }
                    m_socWorker = accepted;
                }
                WaitForData(accepted);
                if (SockServer != null)
                {
                    SockServer.Invoke(Enum_SocketServer.ClientConnect, accepted.RemoteEndPoint.ToString());
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
        private void OnClientDisconnect(Socket connection)
        {
            lock (serverSync)
            {
                if (!ReferenceEquals(connection, m_socWorker)) return;
                m_socWorker = null;
                connection.Close();
                // A callback from a stopped lifetime must never re-open accept.
                if (m_socListener != null)
                    m_socListener.BeginAccept(OnClientConnect, m_socListener);
            }

            // báo sự kiện client disconnect
            if (SockServer != null)
            {
                SockServer.Invoke(Enum_SocketServer.ClientDisconnect, "");
            }
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
                OnClientDisconnect(soc);
            }
            catch (ObjectDisposedException) { }
        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            CSocketPacket theSockId = (CSocketPacket)asyn.AsyncState;
            try
            {
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
                    WaitForData(theSockId.thisSocket);
                }
                else
                {
                    OnClientDisconnect(theSockId.thisSocket);
                }
            }
            catch
            {
                OnClientDisconnect(theSockId.thisSocket);
            }

        }
        public bool Send(string Data)
        {
            Socket connection;
            lock (serverSync) { connection = m_socWorker; }
            try
            {
                if (!String.IsNullOrEmpty(Data))
                {
                    if (connection == null) return false;
                    Object objData = Data;
                    byte[] byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                    connection.Send(byData);
                }
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            catch (ObjectDisposedException) { return false; }
        }
    }
}
