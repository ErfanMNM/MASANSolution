using Newtonsoft.Json;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class AwsIotClientHelper : IDisposable
{
    private MqttClient client;
    private string clientId;
    private string host;
    private string topic;
    private X509Certificate caCert;
    private X509Certificate2 clientCert;
    private CancellationTokenSource healthCheckCts;
    private readonly object lifecycleSync = new object();
    private readonly SemaphoreSlim connectGate = new SemaphoreSlim(1, 1);
    private Task healthCheckTask;
    private bool disposed;

    public event EventHandler<AWSStatusEventArgs> AWSStatus_OnChange;
    public event EventHandler<AWSStatusReceiveEventArgs> AWSStatus_OnReceive;

    public enum e_awsIot_status
    {
        Connected,
        Disconnected,
        Connecting,
        Error,
        Subscribed,
        Unsubscribed,
        Published,
        Unpublished
    }

    public class AWSStatusEventArgs : EventArgs
    {
        public e_awsIot_status Status { get; }
        public string Message { get; }

        public AWSStatusEventArgs(e_awsIot_status status, string message)
        {
            Status = status;
            Message = message;
        }
    }

    public class AWSStatusReceiveEventArgs : EventArgs
    {
        public string Topic { get; }
        public string Payload { get; }
        public string Message { get; }

        public AWSStatusReceiveEventArgs(string topic, string payload, string message)
        {
            Topic = topic;
            Payload = payload;
            Message = message;
        }
    }

    public AwsIotClientHelper(
        string host,
        string clientId,
        string rootCAPath,
        string clientCertPathOrEmpty,
        string pfxPath,
        string pfxPassword,
        string topic = "")
    {
        this.host = host;
        this.clientId = clientId;
        this.topic = topic;

        caCert = X509Certificate.CreateFromCertFile(rootCAPath);
        clientCert = new X509Certificate2(pfxPath, pfxPassword);
    }

    public async Task ConnectAsync()
    {
        CancellationTokenSource lifetime;
        lock (lifecycleSync)
        {
            if (disposed) throw new ObjectDisposedException(nameof(AwsIotClientHelper));
            if (healthCheckCts == null) healthCheckCts = new CancellationTokenSource();
            lifetime = healthCheckCts;
        }
        await ConnectCoreAsync(lifetime);
        StartHealthCheck(lifetime);
    }

    private bool IsCurrent(CancellationTokenSource lifetime, MqttClient candidate)
    {
        lock (lifecycleSync)
        {
            return !disposed && lifetime != null && ReferenceEquals(healthCheckCts, lifetime)
                && !lifetime.IsCancellationRequested && ReferenceEquals(client, candidate);
        }
    }

    private void CloseClient(MqttClient candidate)
    {
        if (candidate == null) return;
        candidate.MqttMsgPublishReceived -= Client_MqttMsgPublishReceived;
        candidate.ConnectionClosed -= Client_ConnectionClosed;
        try { if (candidate.IsConnected) candidate.Disconnect(); }
        catch { /* Cleanup must not revive a stopped lifetime. */ }
    }

    private bool IsCurrentClient(MqttClient target)
    {
        lock (lifecycleSync) { return !disposed && target != null && ReferenceEquals(client, target); }
    }

    private async Task ConnectCoreAsync(CancellationTokenSource lifetime)
    {
        var token = lifetime.Token;
        bool entered = false;
        MqttClient candidate = null;
        try
        {
            await connectGate.WaitAsync(token);
            entered = true;
            MqttClient previous;
            lock (lifecycleSync)
            {
                if (disposed || !ReferenceEquals(healthCheckCts, lifetime) || token.IsCancellationRequested) return;
                if (client != null && client.IsConnected) return;
                previous = client;
                candidate = new MqttClient(host, 8883, true, caCert, clientCert, MqttSslProtocols.TLSv1_2);
                candidate.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                candidate.ConnectionClosed += Client_ConnectionClosed;
                client = candidate;
            }
            CloseClient(previous);
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Connecting, $"🔄 [{Now()}] Đang kết nối..."));
            await Task.Run(() =>
            {
                if (!token.IsCancellationRequested) candidate.Connect(clientId);
            });

            if (!IsCurrent(lifetime, candidate)) return;
            if (candidate.IsConnected)
            {
                AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Connected, $"✅ [{Now()}] Kết nối thành công."));
                if (!string.IsNullOrEmpty(topic))
                {
                    Subscribe(topic);
                }
            }
            else
            {
                AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Error, $"❌ [{Now()}] Kết nối thất bại."));
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception ex)
        {
            if (IsCurrent(lifetime, candidate))
                AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Error, $"❌ [{Now()}] Lỗi: {ex.Message}"));
        }
        finally
        {
            if (candidate != null && !IsCurrent(lifetime, candidate)) CloseClient(candidate);
            if (entered) connectGate.Release();
        }
    }

    public void Subscribe(string topic, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE)
    {
        this.topic = topic;
        MqttClient target;
        lock (lifecycleSync) { target = client; }
        if (target != null && target.IsConnected)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!IsCurrentClient(target)) return;
                    target.Subscribe(new[] { topic }, new[] { qos });
                    AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Subscribed, $"✅ [{Now()}] Đã subscribe topic: {topic}"));
                }
                catch (Exception ex)
                {
                    AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Unsubscribed, $"⚠️ [{Now()}] Lỗi subscribe: {ex.Message}"));
                }
            });
        }
        else
        {
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Unsubscribed, $"⚠️ [{Now()}] Không thể subscribe, chưa kết nối."));
        }
    }

    public void SubscribeMultiple(string[] topics, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE)
    {
        MqttClient target;
        lock (lifecycleSync) { target = client; }
        if (IsCurrentClient(target) && target.IsConnected)
        {
            try
            {
            byte[] qosLevels = new byte[topics.Length];
            for (int i = 0; i < qosLevels.Length; i++) qosLevels[i] = qos;

            target.Subscribe(topics, qosLevels);
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Subscribed, $"✅ [{Now()}] Đã subscribe các topic: {string.Join(", ", topics)}"));
            }
            catch (Exception ex)
            {
                AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Unsubscribed, $"⚠️ [{Now()}] Lỗi subscribe: {ex.Message}"));
            }
        }
        else
        {
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Unsubscribed, $"⚠️ [{Now()}] Không thể subscribe, chưa kết nối."));
        }
    }

    public void Publish(string topic, string payload, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, bool retained = false)
    {
        MqttClient target;
        lock (lifecycleSync) { target = client; }
        if (target != null && target.IsConnected)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!IsCurrentClient(target)) return;
                    target.Publish(topic, Encoding.UTF8.GetBytes(payload), qos, retained);
                }
                catch (Exception ex)
                {
                    AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Unpublished, $"⚠️ [{Now()}] Lỗi publish: {ex.Message}"));
                }
            });
        }
        else
        {
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Unpublished, $"⚠️ [{Now()}] Không thể publish, chưa kết nối."));
        }
    }

    public (string msg,bool Issuccess) Publish_V2(string topic, string payload, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, bool retained = false)
    {
        var rt = (string.Empty, false);
        MqttClient target;
        lock (lifecycleSync) { target = client; }
        if (IsCurrentClient(target) && target.IsConnected)
        {

            try
            {
                target.Publish(topic, Encoding.UTF8.GetBytes(payload), qos, retained);
                rt = ($"✅ [{Now()}] Đã publish thành công topic: {topic}", true);
            }
            catch (Exception ex)
            {
                rt = ($"⚠️ [{Now()}] Lỗi publish: {ex.Message}", false);
            }
        }
        else
        {
            rt = ($"⚠️ [{Now()}] Không thể publish, chưa kết nối.", false);
        }
        return rt;
    }

    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        lock (lifecycleSync) { if (disposed || !ReferenceEquals(sender, client)) return; }
        string payload = Encoding.UTF8.GetString(e.Message);
        string msg = $"📩 [{DateTime.Now.ToString("o")}] Topic: {e.Topic}, Payload: {payload}";
        AWSStatus_OnReceive?.Invoke(this, new AWSStatusReceiveEventArgs(e.Topic, payload, msg));
    }

    private void Client_ConnectionClosed(object sender, EventArgs e)
    {
        lock (lifecycleSync) { if (disposed || !ReferenceEquals(sender, client)) return; }
        AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Disconnected, $"❌ [{Now()}] Mất kết nối, đang thử reconnect..."));
    }

    private void StartHealthCheck(CancellationTokenSource lifetime)
    {
        lock (lifecycleSync)
        {
            if (disposed || !ReferenceEquals(healthCheckCts, lifetime) || lifetime.IsCancellationRequested
                || healthCheckTask != null || client == null || !client.IsConnected) return;
            var token = lifetime.Token;
            healthCheckTask = Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(5000, token).ConfigureAwait(false);
                        await ConnectCoreAsync(lifetime).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            });
        }
    }

    public void StopHealthCheck()
    {
        lock (lifecycleSync)
        {
            healthCheckCts?.Cancel();
            healthCheckCts = null;
            healthCheckTask = null;
        }
    }

    public void Disconnect()
    {
        MqttClient previous;
        lock (lifecycleSync)
        {
            StopHealthCheck();
            previous = client;
            client = null;
        }
        try
        {
            if (previous != null)
            {
                CloseClient(previous);
                AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Disconnected, $"⚠️ [{Now()}] Đã ngắt kết nối AWS IoT Core."));
            }
        }
        catch (Exception ex)
        {
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Error, $"❌ [{Now()}] Lỗi disconnect: {ex.Message}"));
        }
    }

    public void Dispose()
    {
        lock (lifecycleSync) { disposed = true; }
        Disconnect();
    }

    private string Now()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }


    public class AWS_Response
    {
        public string status { get; set; }
        public string message_id { get; set; }
        public string error_message { get; set; }
    }

    public class AWSSendPayload
    {
        [JsonProperty(Order = 1)]
        public string message_id { get; set; }

        [JsonProperty(Order = 2)]
        public string orderNo { get; set; }

        [JsonProperty(Order = 3)]
        public string uniqueCode { get; set; }

        

        [JsonProperty(Order = 5)]
        public string cartonCode { get; set; }

        [JsonProperty(Order = 6)]
        public int status { get; set; }

        [JsonProperty(Order = 7)]
        public string activate_datetime { get; set; }

        [JsonProperty(Order = 8)]
        public string production_date { get; set; }
        [JsonProperty(Order = 4)]
        public string gtin { get; set; }

        [JsonProperty(Order = 9)]
        public string thing_name { get; set; }
    }
}
