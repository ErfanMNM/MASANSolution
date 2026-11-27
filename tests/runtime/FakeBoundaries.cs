using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

// No SDK DLLs are referenced by RuntimeRegression. These fakes have no sockets.
namespace uPLibrary.Networking.M2Mqtt.Messages
{
    public class MqttMsgBase { public const byte QOS_LEVEL_AT_LEAST_ONCE = 1; }
    public class MqttMsgPublishEventArgs : EventArgs { public byte[] Message; public string Topic; }
}
namespace uPLibrary.Networking.M2Mqtt
{
    using Messages;
    public enum MqttSslProtocols { TLSv1_2 }
    public class MqttClient
    {
        public static readonly ConcurrentQueue<MqttClient> Instances = new ConcurrentQueue<MqttClient>();
        public static Action<MqttClient> BeforeConnect;
        public static int ConnectCalls, SubscribeCalls, PublishCalls;
        public volatile bool IsConnected;
        public event EventHandler<MqttMsgPublishEventArgs> MqttMsgPublishReceived;
        public event EventHandler ConnectionClosed;
        public MqttClient(string host, int port, bool ssl, X509Certificate ca, X509Certificate cert, MqttSslProtocols protocol)
        { Instances.Enqueue(this); }
        public byte Connect(string id) { Interlocked.Increment(ref ConnectCalls); BeforeConnect?.Invoke(this); IsConnected=true; return 0; }
        public void Disconnect() { IsConnected=false; ConnectionClosed?.Invoke(this,EventArgs.Empty); }
        public ushort Subscribe(string[] topics, byte[] qos) { Interlocked.Increment(ref SubscribeCalls); return 1; }
        public ushort Publish(string topic, byte[] data, byte qos, bool retained) { Interlocked.Increment(ref PublishCalls); return 1; }
        public void Drop() { IsConnected=false; ConnectionClosed?.Invoke(this,EventArgs.Empty); }
        public void Receive(string topic, byte[] bytes) { MqttMsgPublishReceived?.Invoke(this,new MqttMsgPublishEventArgs { Topic=topic,Message=bytes }); }
        public static void Reset() { MqttClient ignored; while(Instances.TryDequeue(out ignored)){} BeforeConnect=null; ConnectCalls=SubscribeCalls=PublishCalls=0; }
    }
}
namespace HslCommunication
{
    public class OperateResult { public bool IsSuccess=true; }
}
namespace HslCommunication.Core
{
    public enum DataFormat { CDAB }
    public class Transform { public DataFormat DataFormat; public bool IsStringReverseByteWord; }
}
namespace HslCommunication.Core.Pipe
{
    public class PipeUdpNet
    {
        public int ReceiveTimeOut,SleepTime,SocketKeepAliveTime; public bool IsPersistentConnection;
        public PipeUdpNet(string ip,int port) { }
    }
}
namespace HslCommunication.Profinet.Omron
{
    public enum OmronPlcType { CSCJ }
    public class OmronFinsUdp
    {
        public Core.Pipe.PipeUdpNet CommunicationPipe; public OmronPlcType PlcType;
        public byte SA1,GCT,DA1,SID; public Core.Transform ByteTransform=new Core.Transform();
        public int Writes; public Action BeforeWrite;
        public OperateResult Write(string address,short value)
        { Interlocked.Increment(ref Writes); BeforeWrite?.Invoke(); return new OperateResult(); }
    }
}
namespace Google.Apis.Auth.OAuth2
{
    public class GoogleCredential { public static GoogleCredential FromFile(string path) { return new GoogleCredential(); } }
}
namespace Google.Cloud.Storage { internal class NamespaceMarker {} }
namespace Google.Cloud.Storage.V1
{
    public class StorageClient
    {
        public static byte[] DownloadBytes = new byte[] {1,2,3}; public static bool FailDownload;
        public static StorageClient Create(Google.Apis.Auth.OAuth2.GoogleCredential credential) { return new StorageClient(); }
        public object UploadObject(string bucket,string name,string contentType,Stream stream) { return new object(); }
        public void DownloadObject(string bucket,string name,Stream stream)
        { if(FailDownload) throw new IOException("injected download error"); stream.Write(DownloadBytes,0,DownloadBytes.Length); }
    }
}
namespace Google.Cloud.BigQuery.V2
{
    public class BigQueryTable { }
    public class Field { public string Name; }
    public class Schema { public List<Field> Fields=new List<Field> {new Field {Name="code"}}; }
    public class BigQueryRow { public object this[string field] {get {return "fake-code";}} }
    public class BigQueryResults : IEnumerable<BigQueryRow>
    {
        public Schema Schema=new Schema();
        public IEnumerator<BigQueryRow> GetEnumerator() { yield return new BigQueryRow(); }
        IEnumerator IEnumerable.GetEnumerator() {return GetEnumerator();}
    }
    public class BigQueryClient
    {
        public static bool FailQuery;
        public static BigQueryClient Create(string project,Google.Apis.Auth.OAuth2.GoogleCredential credential) {return new BigQueryClient();}
        public BigQueryTable GetTable(string project,string dataset,string table) {return new BigQueryTable();}
        public BigQueryResults ExecuteQuery(string query,object parameters)
        {if(FailQuery) throw new InvalidOperationException("injected query error"); return new BigQueryResults();}
    }
}
namespace RuntimeFixtures
{
    internal class TextControl { public string Text=""; public bool Enabled=true; }
    internal enum LoginAction { UpdateProfile }
    internal class LoginActionEventArgs : EventArgs { public bool Status; public string Message; }
    internal class UserData
    {
        public static UserData LookupResult;
        public string Username,Role,Password,Salt,Key2FA;
        public static UserData GetUserByUsername(string user,string path) {return LookupResult;}
    }
    internal static class UserHelper
    {
        public static int ValidationCalls;
        public static bool ValidateCredentials(string user,string password,string path) {ValidationCalls++;return true;}
        public static bool UpdatePassword(string user,string password,string path) {return true;}
        public static string HashPassword(string password,string salt) {return "fixture-hash";}
    }
    internal static class TwoFAHelper { public static bool VerifyOTP(string key,string otp,int digits) {return true;} }
    internal class LogHelper<T>
    {
        public static int Writes;
        public LogHelper(string path) { }
        public Task WriteLogAsync(string user,T action,string message) {Interlocked.Increment(ref Writes);return Task.CompletedTask;}
    }
}
