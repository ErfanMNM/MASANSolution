using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace TestApp
{
    public class TCPCOMBridge : IDisposable
    {
        private sealed class Session
        {
            internal readonly object Gate = new object();
            internal readonly TcpListener[] Listeners = new TcpListener[2];
            internal readonly TcpClient[] Clients = new TcpClient[2];
            internal bool Closed;
            internal void Close()
            {
                lock (Gate)
                {
                    if (Closed) return; Closed = true;
                    foreach (var listener in Listeners) listener?.Stop();
                    foreach (var client in Clients) client?.Close();
                }
            }
        }
        private readonly object gate = new object(); private Session current; private bool disposed;
        public event EventHandler<string> StatusChanged;
        public event EventHandler<string> DataTransferred;
        public bool IsConnected { get { lock(gate) return current != null && !current.Closed && current.Clients[0]?.Connected == true && current.Clients[1]?.Connected == true; } }
        public int Port1 {get;private set;} public int Port2 {get;private set;}
        public Task Completion {get;private set;} = Task.CompletedTask;
        public bool CreateBridge(int tcpPort1,int tcpPort2)
        {
            var session = new Session();
            try
            {
                lock(gate)
                {
                    if(disposed) return false;
                    var previous = current; current = null; previous?.Close();
                    session.Listeners[0]=new TcpListener(IPAddress.Any,tcpPort1);
                    session.Listeners[1]=new TcpListener(IPAddress.Any,tcpPort2);
                    session.Listeners[0].ExclusiveAddressUse=true;
                    session.Listeners[1].ExclusiveAddressUse=true;
                    session.Listeners[0].Start(); session.Listeners[1].Start();
                    Port1=((IPEndPoint)session.Listeners[0].LocalEndpoint).Port;
                    Port2=((IPEndPoint)session.Listeners[1].LocalEndpoint).Port;
                    current=session; Completion=Task.Run(()=>Run(session));
                }
                OnStatusChanged("TCP bridge opened"); return true;
            }
            catch(Exception ex) {session.Close();OnStatusChanged(ex.Message);return false;}
        }
        private async Task<TcpClient> Accept(Session session,int side)
        {
            var client=await session.Listeners[side].AcceptTcpClientAsync().ConfigureAwait(false);
            lock(session.Gate)
            {
                if(session.Closed){client.Close();throw new ObjectDisposedException(nameof(TCPCOMBridge));}
                session.Clients[side]=client;
            }
            return client;
        }
        private async Task Run(Session session)
        {
            Task first=null,second=null;
            try
            {
                var accept1=Accept(session,0);var accept2=Accept(session,1);
                try { await Task.WhenAll(accept1,accept2).ConfigureAwait(false); }
                catch {session.Close();throw;}
                var one=accept1.Result.GetStream();var two=accept2.Result.GetStream();
                first=Forward(one,two,"App1 → App2"); second=Forward(two,one,"App2 → App1");
                await Task.WhenAny(first,second).ConfigureAwait(false);
            }
            catch(Exception ex) {if(!session.Closed)OnStatusChanged(ex.Message);}
            finally
            {
                session.Close();
                if(first!=null && second!=null)try {await Task.WhenAll(first,second).ConfigureAwait(false);}catch(Exception){}
                lock(gate){if(ReferenceEquals(current,session))current=null;}
            }
        }
        private async Task Forward(NetworkStream source,NetworkStream target,string direction)
        {
            var buffer=new byte[1024];
            while(true)
            {
                int size=await source.ReadAsync(buffer,0,buffer.Length).ConfigureAwait(false);if(size==0)return;
                await target.WriteAsync(buffer,0,size).ConfigureAwait(false);
                OnDataTransferred(direction+": "+Encoding.UTF8.GetString(buffer,0,size));
            }
        }
        public void SendTestData(string data,bool toPort1=true)
        {
            Session session;lock(gate)session=current;if(session==null)return;
            try {var bytes=Encoding.UTF8.GetBytes(data+"\r\n");var stream=session.Clients[toPort1?0:1]?.GetStream();stream?.Write(bytes,0,bytes.Length);}
            catch(Exception ex){OnStatusChanged(ex.Message);}
        }
        public void CloseBridge(){Session old;lock(gate){old=current;current=null;}old?.Close();}
        protected virtual void OnStatusChanged(string status)
        {
            var observers=StatusChanged;if(observers==null)return;
            foreach(EventHandler<string> observer in observers.GetInvocationList())try{observer(this,status);}catch(Exception){}
        }
        protected virtual void OnDataTransferred(string data)
        {
            var observers=DataTransferred;if(observers==null)return;
            foreach(EventHandler<string> observer in observers.GetInvocationList())try{observer(this,data);}catch(Exception){}
        }
        public void Dispose(){lock(gate)disposed=true;CloseBridge();}
        public static List<int> GetAvailableTCPPorts(){return new List<int>{8001,8002,8003,8004,8005,8006,9001,9002,9003,9004,9005,9006,10001,10002,10003,10004,10005,10006};}
    }
}
