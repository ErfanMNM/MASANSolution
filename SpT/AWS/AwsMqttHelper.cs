using System;
using System.Timers;
using System.Security.Cryptography.X509Certificates;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class AwsIotClientHelper
{
    private MqttClient client;
    private string clientId;
    private string host;
    private string topic;
    private X509Certificate caCert;
    private X509Certificate2 clientCert;
    private Timer healthCheckTimer;

    public Action<string> OnStatusChanged;
    public Action<string> OnMessageReceived;

    public AwsIotClientHelper(
        string host,
        string clientId,
        string rootCAPath,
        string clientCertPathOrEmpty,
        string pfxPath,
        string pfxPassword,
        string topic = ""
    )
    {
        this.clientId = clientId;
        this.host = host;
        this.topic = topic;

        caCert = X509Certificate.CreateFromCertFile(rootCAPath);
        clientCert = new X509Certificate2(pfxPath, pfxPassword);

        healthCheckTimer = new Timer(5000);
        healthCheckTimer.Elapsed += HealthCheckTimer_Elapsed;
        healthCheckTimer.AutoReset = true;
    }

    public void Connect()
    {
        try
        {
            if (client != null && client.IsConnected)
            {
                OnStatusChanged?.Invoke($"⚠️ [{Now()}] Đã kết nối, bỏ qua connect.");
                return;
            }

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

            client.Connect(clientId);

            if (client.IsConnected)
            {
                OnStatusChanged?.Invoke($"✅ [{Now()}] Kết nối AWS IoT Core thành công.");
                if (!string.IsNullOrEmpty(topic))
                {
                    client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    OnStatusChanged?.Invoke($"✅ [{Now()}] Đã subscribe topic: {topic}");
                }
                healthCheckTimer.Start();
            }
            else
            {
                OnStatusChanged?.Invoke($"❌ [{Now()}] Kết nối thất bại, kiểm tra cert/host/network.");
            }
        }
        catch (Exception ex)
        {
            OnStatusChanged?.Invoke($"❌ [{Now()}] Lỗi kết nối: {ex.Message}");
        }
    }

    public void Subscribe(string topic, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE)
    {
        this.topic = topic;
        if (client != null && client.IsConnected)
        {
            client.Subscribe(new[] { topic }, new[] { qos });
            OnStatusChanged?.Invoke($"✅ [{Now()}] Đã subscribe topic: {topic}");
        }
        else
        {
            OnStatusChanged?.Invoke($"⚠️ [{Now()}] Không thể subscribe, chưa kết nối.");
        }
    }

    public void Publish(string topic, string payload, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, bool retained = false)
    {
        if (client != null && client.IsConnected)
        {
            client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(payload), qos, retained);
            OnStatusChanged?.Invoke($"📤 [{Now()}] Published to {topic}: {payload}");
        }
        else
        {
            OnStatusChanged?.Invoke($"⚠️ [{Now()}] Không thể publish, chưa kết nối.");
        }
    }

    public void Disconnect()
    {
        if (client != null && client.IsConnected)
        {
            client.Disconnect();
            OnStatusChanged?.Invoke($"⚠️ [{Now()}] Đã ngắt kết nối AWS IoT Core.");
        }
        healthCheckTimer.Stop();
    }

    private void Client_ConnectionClosed(object sender, EventArgs e)
    {
        OnStatusChanged?.Invoke($"❌ [{Now()}] Mất kết nối, sẽ thử reconnect sau 5s...");
    }

    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string payload = System.Text.Encoding.UTF8.GetString(e.Message);
        string msg = $"📩 [{Now()}] Topic: {e.Topic}, Payload: {payload}";
        OnMessageReceived?.Invoke(msg);
    }

    private void HealthCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (client == null || !client.IsConnected)
        {
            OnStatusChanged?.Invoke($"⚠️ [{Now()}] Mất kết nối, đang reconnect...");
            Connect();
        }
    }

    public void SubscribeMultiple(string[] topics, byte qos = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE)
    {
        if (client != null && client.IsConnected)
        {
            byte[] qosLevels = new byte[topics.Length];
            for (int i = 0; i < qosLevels.Length; i++) qosLevels[i] = qos;

            client.Subscribe(topics, qosLevels);
            OnStatusChanged?.Invoke($"✅ [{Now()}] Đã subscribe các topic: {string.Join(", ", topics)}");
        }
        else
        {
            OnStatusChanged?.Invoke($"⚠️ [{Now()}] Không thể subscribe, chưa kết nối.");
        }
    }


    private string Now()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}
