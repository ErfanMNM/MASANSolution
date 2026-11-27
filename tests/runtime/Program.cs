using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using HslCommunication.Profinet.Omron;
using RuntimeFixtures;

internal static class Program
{
    private static int passed,failed;
    private static void Check(bool ok,string message) {if(!ok) throw new Exception(message);}
    private static void Until(Func<bool> predicate,string message,int timeout=3000)
    {Check(SpinWait.SpinUntil(predicate,timeout),message);}
    private static object Field(object target,string name)
    {return target.GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Instance).GetValue(target);}
    private static void Run(string name,Action test)
    {try {test();passed++;Console.WriteLine("PASS "+name);} catch(Exception ex) {failed++;Console.WriteLine("FAIL "+name+": "+ex);}}
    public static int Main()
    {
        Run("MQTT concurrent Connect and repeated health reconnect use one loop",MqttHealth);
        Run("MQTT stale pending connect cannot revive disconnect/new lifetime",MqttPending);
        Run("MQTT Dispose during pending connect and failure retry",MqttDispose);
        Run("SPMS1 PLC cancellation, repeated Init, container and in-flight disposal",()=>Plc(typeof(SPMS1.OmronPLC_Hsl)));
        Run("SpT PLC cancellation, repeated Init, container and in-flight disposal",()=>Plc(typeof(SpT.OmronPLC_Hsl)));
        Run("PLC callbacks run outside disposal gate",PlcCallback);
        Run("Google shorter download truncates and failure reports false",GoogleDownload);
        Run("BigQuery failure reports false and success returns rows",GoogleQuery);
        Run("Auth missing user INIT and Save report failure without validation",Auth);
        Run("TCP stop before start/accept, restart, EOF, connected Stop and container Dispose",Tcp);
        Console.WriteLine("RESULT "+passed+"/"+(passed+failed)+" groups; SDKs fake; only TCP IPv4 loopback; no production app");
        return failed==0?0:1;
    }
    private static AwsIotClientHelper NewMqtt()
    {return new AwsIotClientHelper("fixture.invalid","fixture","not-read","","not-read","","");}
    private static void Finish(Task task,string message,int timeout=3000)
    {Check(task!=null && task.Wait(timeout),message);Check(!task.IsFaulted,message+" faulted");}
    private static void MqttHealth()
    {
        MqttClient.Reset();
        using(var helper=NewMqtt())
        {
            Finish(Task.WhenAll(Enumerable.Range(0,12).Select(_=>helper.ConnectAsync())),"concurrent connect");
            Check(MqttClient.ConnectCalls==1,"concurrent Connect made duplicate SDK clients");
            var lifetime=Field(helper,"healthCheckCts");
            var worker=(Task)Field(helper,"healthCheckTask");
            Check(worker!=null,"health loop absent");
            for(int cycle=0;cycle<2;cycle++)
            {
                int expected=MqttClient.ConnectCalls+1;
                MqttClient.Instances.Last().Drop();
                Until(()=>MqttClient.ConnectCalls==expected,"health reconnect missing",7000);
                Until(()=>MqttClient.Instances.Last().IsConnected,"fake connect incomplete");
                Check(ReferenceEquals(lifetime,Field(helper,"healthCheckCts")),"reconnect replaced CTS");
                Check(ReferenceEquals(worker,Field(helper,"healthCheckTask")),"reconnect multiplied health workers");
            }
            helper.Disconnect();
            Finish(worker,"cancelled health loop did not exit");
            int calls=MqttClient.ConnectCalls;
            Thread.Sleep(5250); // cross an entire unchanged production 5s interval
            Check(MqttClient.ConnectCalls==calls,"automatic reconnect after Disconnect");
            Finish(helper.ConnectAsync(),"explicit restart");
            Check(MqttClient.ConnectCalls==calls+1,"explicit restart not connected");
            var next=(Task)Field(helper,"healthCheckTask");
            helper.StopHealthCheck();Finish(next,"StopHealthCheck completion");
            int publishes=MqttClient.PublishCalls,subscribes=MqttClient.SubscribeCalls;
            helper.Publish("fixture","fixture");helper.Subscribe("fixture");
            Until(()=>MqttClient.PublishCalls==publishes+1&&MqttClient.SubscribeCalls==subscribes+1,"StopHealthCheck disabled connected Publish/Subscribe");
            helper.SubscribeMultiple(new[]{"fixture-a","fixture-b"});
            Check(helper.Publish_V2("fixture","fixture").Issuccess,"stopped monitor sync publish failed");
            Check(MqttClient.SubscribeCalls==subscribes+2&&MqttClient.PublishCalls==publishes+2,"sync MQTT operations not sent");
        }
    }
    private static void MqttPending()
    {
        MqttClient.Reset();
        using(var entered=new ManualResetEventSlim())
        using(var release=new ManualResetEventSlim())
        using(var helper=NewMqtt())
        {
            int connected=0;
            helper.AWSStatus_OnChange+=(s,e)=>{if(e.Status==AwsIotClientHelper.e_awsIot_status.Connected)Interlocked.Increment(ref connected);};
            MqttClient.BeforeConnect=c=>{entered.Set();Check(release.Wait(3000),"test release timeout");};
            var old=helper.ConnectAsync();
            Check(entered.Wait(2000),"old connect not blocked");
            helper.Disconnect();
            MqttClient.BeforeConnect=null;
            var fresh=helper.ConnectAsync();
            release.Set();Finish(Task.WhenAll(old,fresh),"pending/restart tasks");
            var all=MqttClient.Instances.ToArray();
            Check(all.Length==2 && !all[0].IsConnected && all[1].IsConnected,"stale completion closed/replaced wrong client");
            Check(connected==1,"stale completion emitted Connected");
            Check(ReferenceEquals(Field(helper,"client"),all[1]),"stale client republished");
            var worker=(Task)Field(helper,"healthCheckTask");
            helper.Dispose();helper.Dispose();Finish(worker,"new worker dispose");
        }
    }
    private static void MqttDispose()
    {
        MqttClient.Reset();
        using(var entered=new ManualResetEventSlim())
        using(var release=new ManualResetEventSlim())
        {
            var helper=NewMqtt();
            MqttClient.BeforeConnect=c=>{entered.Set();Check(release.Wait(3000),"release timeout");};
            var pending=helper.ConnectAsync();Check(entered.Wait(2000),"pending connect");
            helper.Dispose();release.Set();Finish(pending,"disposed connect completion");
            Check(MqttClient.Instances.All(c=>!c.IsConnected),"disposed helper revived connection");
            Check(Field(helper,"healthCheckTask")==null,"disposed helper started health loop");
            bool rejected=false;
            try {helper.ConnectAsync().GetAwaiter().GetResult();} catch(ObjectDisposedException){rejected=true;}
            Check(rejected,"Connect after Dispose accepted");
        }
        MqttClient.Reset();
        using(var helper=NewMqtt())
        {
            MqttClient.BeforeConnect=c=>{throw new IOException("injected MQTT failure");};
            Finish(helper.ConnectAsync(),"failed connect completion");
            Check(Field(helper,"healthCheckTask")==null,"failed initial connect started loop");
            MqttClient.BeforeConnect=null;Finish(helper.ConnectAsync(),"manual retry");
            Check(MqttClient.Instances.Last().IsConnected,"manual retry not connected");
        }
        MqttClient.Reset();
        using(var helper=NewMqtt())
        {
            helper.AWSStatus_OnChange+=(s,e)=>{if(e.Status==AwsIotClientHelper.e_awsIot_status.Connected)helper.Disconnect();};
            Finish(helper.ConnectAsync(),"reentrant Disconnect");
            Check(MqttClient.Instances.All(c=>!c.IsConnected)&&Field(helper,"healthCheckTask")==null,"reentrant Disconnect revived lifetime");
        }
    }
    private static Component NewPlc(Type type,int period,out BackgroundWorker worker,out OmronFinsUdp fake)
    {
        var plc=(Component)Activator.CreateInstance(type);
        type.GetProperty("Time_Update").SetValue(plc,period);
        worker=(BackgroundWorker)Field(plc,"WK_Update");
        fake=(OmronFinsUdp)type.GetField("plc").GetValue(plc);
        return plc;
    }
    private static void Init(Component c) {c.GetType().GetMethod("InitPLC").Invoke(c,null);}
    private static void Plc(Type type)
    {
        BackgroundWorker worker;OmronFinsUdp fake;
        var c=NewPlc(type,180,out worker,out fake);
        Init(c);Init(c);Thread.Sleep(30);c.Dispose();c.Dispose();
        Until(()=>!worker.IsBusy,"worker did not exit after Dispose");Thread.Sleep(220);
        Check(fake.Writes==0,"PLC write after Dispose during sleep");
        bool rejected=false;try {Init(c);}catch(TargetInvocationException e){rejected=e.InnerException is ObjectDisposedException;}
        Check(rejected,"Init after Dispose accepted");
        using(var container=new Container())
        {
            c=(Component)Activator.CreateInstance(type,new object[] {container});
            type.GetProperty("Time_Update").SetValue(c,180);
            worker=(BackgroundWorker)Field(c,"WK_Update");fake=(OmronFinsUdp)type.GetField("plc").GetValue(c);
            Init(c);Thread.Sleep(30);container.Dispose();
            Until(()=>!worker.IsBusy,"container Dispose worker alive");Thread.Sleep(220);
            Check(fake.Writes==0,"container Dispose still writes");
        }
        using(var entered=new ManualResetEventSlim())
        using(var release=new ManualResetEventSlim())
        {
            c=NewPlc(type,10,out worker,out fake);
            fake.BeforeWrite=()=>{entered.Set();Check(release.Wait(3000),"in-flight release timeout");};
            Init(c);Check(entered.Wait(2000),"PLC write not entered");
            var disposing=Task.Run(()=>c.Dispose());
            Thread.Sleep(40);Check(!disposing.IsCompleted,"Dispose escaped in-flight write gate");
            release.Set();Finish(disposing,"Dispose in-flight write");
            Until(()=>!worker.IsBusy,"worker alive after in-flight Dispose");
            int writes=fake.Writes;Thread.Sleep(50);Check(writes==1&&fake.Writes==writes,"extra PLC writes after Dispose");
        }
    }
    private static void PlcCallback()
    {
        for(int variant=0;variant<2;variant++)
        using(var entered=new ManualResetEventSlim())
        using(var release=new ManualResetEventSlim())
        {
            BackgroundWorker worker;OmronFinsUdp fake;
            var c=NewPlc(variant==0?typeof(SPMS1.OmronPLC_Hsl):typeof(SpT.OmronPLC_Hsl),10,out worker,out fake);
            if(variant==0)((SPMS1.OmronPLC_Hsl)c).PLCStatus_OnChange+=(s,e)=>{entered.Set();release.Wait(3000);};
            else ((SpT.OmronPLC_Hsl)c).PLCStatus_OnChange+=(s,e)=>{entered.Set();release.Wait(3000);};
            Init(c);Check(entered.Wait(2000),"callback not entered");
            try {Finish(Task.Run(()=>c.Dispose()),"Dispose blocked on UI-style callback",1000);}
            finally {release.Set();}
            Until(()=>!worker.IsBusy,"callback worker not stopped");
        }
    }
    private static void GoogleDownload()
    {
        string root=Path.Combine(Path.GetTempPath(),"RuntimeRegression-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            string path=Path.Combine(root,"download.bin");File.WriteAllBytes(path,new byte[8192]);
            using(var google=new SPMS1.GoogleService())
            {
                Google.Cloud.Storage.V1.StorageClient.FailDownload=false;
                var result=google.Download(path);Check(result.success,"download failed");
                Check(File.ReadAllBytes(path).SequenceEqual(new byte[]{1,2,3}),"old trailing bytes retained");
                Google.Cloud.Storage.V1.StorageClient.FailDownload=true;
                Check(!google.Download(path).success,"download failure reports success");
            }
        } finally {Google.Cloud.Storage.V1.StorageClient.FailDownload=false;Directory.Delete(root,true);}
    }
    private static void GoogleQuery()
    {
        using(var google=new SPMS1.GoogleService())
        {
            Google.Cloud.BigQuery.V2.BigQueryClient.FailQuery=true;
            Check(!google.BigQuery("fixture").Success,"query catch reports success");
            Google.Cloud.BigQuery.V2.BigQueryClient.FailQuery=false;
            var result=google.BigQuery("fixture");Check(result.Success&&result.DataTable.Rows.Count==1,"query success regression");
        }
    }
    private static void Auth()
    {
        string root=Path.Combine(Path.GetTempPath(),"RuntimeAuth-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var auth=new AuthFixture {CurrentUserName="missing-fixture",data_file_path=Path.Combine(root,"fixture.db")};
            int errors=0;string last="";
            auth.OnUserAction+=(s,e)=>{if(!e.Status){errors++;last=e.Message;}};
            auth.opRole.Text="stale-role";UserData.LookupResult=null;UserHelper.ValidationCalls=0;
            auth.INIT();Check(errors==1&&auth.opRole.Text=="","missing user INIT did not fail/clear role");
            auth.Save();Check(errors==2&&UserHelper.ValidationCalls==0,"missing user Save touched validator");
            Check(last=="Không tìm thấy thông tin người dùng."&&auth.btnSave.Enabled&&auth.btnSave.Text=="Lưu lại","save failure/finally mismatch");
            UserData.LookupResult=new UserData {Username="fixture",Role="fixture-role",Salt="fixture-salt"};
            auth.INIT();Check(auth.ipUserName.Text=="fixture"&&auth.opRole.Text=="fixture-role","existing user INIT regression");
            Check(!File.Exists(auth.data_file_path),"fixture touched DB");
        } finally {Directory.Delete(root,true);}
    }
    private static int ListenerPort(SPMS1.Socket_Server_TCP server)
    {return ((IPEndPoint)((Socket)Field(server,"m_socListener")).LocalEndPoint).Port;}
    private static void BindAvailable(int port)
    {using(var s=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp)){s.Bind(new IPEndPoint(IPAddress.Loopback,port));s.Listen(1);}}
    private static void Tcp()
    {
        using(var server=new SPMS1.Socket_Server_TCP {IP="127.0.0.1",Port=0})
        {
            server.StopServer();server.StopServer();
            for(int i=0;i<12;i++)
            {
                server.StartServer();int port=ListenerPort(server);server.StartServer();
                server.StopServer();Check(Field(server,"m_socListener")==null,"listener retained");BindAvailable(port);
                server.Port=port; // repeat sequential stop/start against the same fixed port
            }
            server.Port=0;
            int received=0,connected=0;
            server.SockServer+=(e,s)=>{if(e==SPMS1.Enum_SocketServer.ReceiveData&&s=="fixture")Interlocked.Increment(ref received);if(e==SPMS1.Enum_SocketServer.ClientConnect)Interlocked.Increment(ref connected);};
            server.StartServer();int live=ListenerPort(server);
            using(var client=new TcpClient())
            {client.Connect(IPAddress.Loopback,live);Until(()=>connected==1,"accept missing");var bytes=Encoding.ASCII.GetBytes("fixture");client.GetStream().Write(bytes,0,bytes.Length);Until(()=>received==1,"loopback receive");}
            Until(()=>Field(server,"m_socWorker")==null,"EOF did not release client");
            using(var client=new TcpClient())
            {
                client.Connect(IPAddress.Loopback,live);Until(()=>connected==2,"second accept");
                using(var sendStarted=new ManualResetEventSlim())
                {
                    int stopReturned=0;
                    var sending=Task.Run(()=>
                    {
                        Check(server.Send("first-send"),"first send before Stop failed");sendStarted.Set();
                        while(Volatile.Read(ref stopReturned)==0)server.Send("race");
                        for(int n=0;n<50;n++)server.Send("after-stop");
                    });
                    Check(sendStarted.Wait(1000),"sender did not start before Stop");
                    try {server.StopServer();}
                    finally {Volatile.Write(ref stopReturned,1);}
                    Check(Field(server,"m_socListener")==null&&Field(server,"m_socWorker")==null,"connected Stop retains sockets");
                    Finish(sending,"Send/Stop race fault");Check(!server.Send("stopped"),"Send accepted without socket");
                }
                client.ReceiveTimeout=1000;
                try {while(client.GetStream().ReadByte()!=-1){}} // drain any data sent before Stop
                catch(IOException ex)
                {
                    var socketError=ex.InnerException as SocketException;
                    Check(socketError!=null&&socketError.SocketErrorCode==SocketError.ConnectionReset,
                        "Stop neither returned EOF nor reset the peer: "+ex.Message);
                }
            }
            server.StartServer();int restarted=ListenerPort(server);server.Dispose();BindAvailable(restarted);
        }
        var container=new Container();var owned=new SPMS1.Socket_Server_TCP(container){IP="127.0.0.1",Port=0};
        owned.StartServer();int ownedPort=ListenerPort(owned);container.Dispose();BindAvailable(ownedPort);
    }
}
