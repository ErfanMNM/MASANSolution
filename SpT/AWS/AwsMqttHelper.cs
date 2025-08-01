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
        try
        {
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Connecting, $"🔄 [{Now()}] Đang kết nối..."));

            client = new MqttClient(
                host,
                8883,
                true,
                caCert,
                clientCert,
                MqttSslProtocols.TLSv1_2
            );

            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.ConnectionClosed += Client_ConnectionClosed;

            await Task.Run(() =>
            {
                try
                {
                    client.Connect(clientId);
                }
                catch (Exception ex)
                {
                    AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Error, $"❌ [{Now()}] Lỗi kết nối: {ex.Message}"));
                }
            });

            if (client.IsConnected)
            {
                AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Connected, $"✅ [{Now()}] Kết nối thành công."));
                if (!string.IsNullOrEmpty(topic))
                {
                    Subscribe(topic);
                }
                StartHealthCheck();
            }
            else
            {
                AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Error, $"❌ [{Now()}] Kết nối thất bại."));
            }
        }
        catch (Exception ex)
        {
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Error, $"❌ [{Now()}] Lỗi: {ex.Message}"));
        }
    }

    public void Subscribe(string topic, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE)
    {
        this.topic = topic;
        if (client != null && client.IsConnected)
        {
            Task.Run(() =>
            {
                try
                {
                    client.Subscribe(new[] { topic }, new[] { qos });
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
        if (client != null && client.IsConnected)
        {
            byte[] qosLevels = new byte[topics.Length];
            for (int i = 0; i < qosLevels.Length; i++) qosLevels[i] = qos;

            client.Subscribe(topics, qosLevels);
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Subscribed, $"✅ [{Now()}] Đã subscribe các topic: {string.Join(", ", topics)}"));
        }
        else
        {
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Unsubscribed, $"⚠️ [{Now()}] Không thể subscribe, chưa kết nối."));
        }
    }

    public void Publish(string topic, string payload, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, bool retained = false)
    {
        if (client != null && client.IsConnected)
        {
            Task.Run(() =>
            {
                try
                {
                    client.Publish(topic, Encoding.UTF8.GetBytes(payload), qos, retained);
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
        if (client != null && client.IsConnected)
        {

            try
            {
                client.Publish(topic, Encoding.UTF8.GetBytes(payload), qos, retained);
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
        string payload = Encoding.UTF8.GetString(e.Message);
        string msg = $"📩 [{DateTime.Now.ToString("o")}] Topic: {e.Topic}, Payload: {payload}";
        AWSStatus_OnReceive?.Invoke(this, new AWSStatusReceiveEventArgs(e.Topic, payload, msg));
    }

    private void Client_ConnectionClosed(object sender, EventArgs e)
    {
        AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Disconnected, $"❌ [{Now()}] Mất kết nối, đang thử reconnect..."));
    }

    private void StartHealthCheck()
    {
        healthCheckCts = new CancellationTokenSource();
        var token = healthCheckCts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (client == null || !client.IsConnected)
                    {
                        
                        AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Disconnected, $"❌ [{Now()}] Mất kết nối, đang reconnect..."));
                        await ConnectAsync();
                    }
                }
                catch (Exception ex)
                {
                    AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Error, $"⚠️ [{Now()}] Lỗi healthcheck: {ex.Message}"));
                }
                await Task.Delay(5000, token);
            }
        }, token);
    }

    public void StopHealthCheck()
    {
        healthCheckCts?.Cancel();
    }

    public void Disconnect()
    {
        try
        {
            if (client != null && client.IsConnected)
            {
                client.Disconnect();
                AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Disconnected, $"⚠️ [{Now()}] Đã ngắt kết nối AWS IoT Core."));
            }
        }
        catch (Exception ex)
        {
            AWSStatus_OnChange?.Invoke(this, new AWSStatusEventArgs(e_awsIot_status.Error, $"❌ [{Now()}] Lỗi disconnect: {ex.Message}"));
        }
        StopHealthCheck();
    }

    public void Dispose()
    {
        Disconnect();
        client = null;
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
       public string  message_id { get; set; }
        public string orderNo { get; set; }
        public string uniqueCode { get; set; }
        public string cartonCode { get; set; }
        public int status { get; set; }
        public string activate_datetime { get; set; }
        public string production_date { get; set; }
        public string thing_name { get; set; }

    }
}
