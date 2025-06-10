using MQTTnet;
using MQTTnet.Client;

using MQTTnet.Protocol;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace SPMS1.MQTT
{
    public enum enumMqttEvent
    {
        CONNECTED,
        DISCONNECTED,
        RECEIVED
    }

    public partial class MQTT_Client : Component
    {
        private IMqttClient _mqttClient;

        // ====================
        // Config từ ngoài Form
        // ====================
        public string brokerHost { get; set; } = "localhost";
        public int brokerPort { get; set; } = 1883;
        public string clientId { get; set; } = Guid.NewGuid().ToString();
        public string username { get; set; } = null;
        public string password { get; set; } = null;

        // ===========================
        // Callback báo về cho Form chính
        // ===========================
        public Action <enumMqttEvent, string, string> ClientCallback;

        // ===========================
        // Constructors
        // ===========================
        public MQTT_Client(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitClient();
        }

        public MQTT_Client()
        {
            InitializeComponent();
            InitClient();
        }

        private void InitClient()
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // Gán event
            _mqttClient.ConnectedAsync += e =>
            {
                ClientCallback?.Invoke(enumMqttEvent.CONNECTED, "", "");
                return Task.CompletedTask;
            };

            _mqttClient.DisconnectedAsync += e =>
            {
                ClientCallback?.Invoke(enumMqttEvent.DISCONNECTED, "", "");
                return Task.CompletedTask;
            };

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                string topic = e.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? new byte[0]);
                ClientCallback?.Invoke(enumMqttEvent.RECEIVED, topic, payload);
                return Task.CompletedTask;
            };
        }

        // ===========================
        // Kết nối
        // ===========================
        public async Task ConnectAsync()
        {
            if (_mqttClient.IsConnected) return;

            // Build options tại thời điểm gọi, không dùng giá trị cũ từ constructor
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerHost, brokerPort)
                .WithClientId(string.IsNullOrEmpty(clientId) ? Guid.NewGuid().ToString() : clientId)
                .WithCleanSession();

            if (!string.IsNullOrEmpty(username))
                optionsBuilder.WithCredentials(username, password);

            var options = optionsBuilder.Build();

            await _mqttClient.ConnectAsync(options);
        }

        // ===========================
        // Ngắt kết nối
        // ===========================
        public async Task DisconnectAsync()
        {
            if (_mqttClient.IsConnected)
                await _mqttClient.DisconnectAsync();
        }

        // ===========================
        // Subscribe
        // ===========================
        public async Task SubscribeAsync(string topic, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic(topic).WithQualityOfServiceLevel(qos))
                    .Build());
            }
        }

        // ===========================
        // Publish
        // ===========================
        public async Task PublishAsync(string topic, string message, bool retain = false, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
        {
            if (_mqttClient.IsConnected)
            {
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(message)
                    .WithRetainFlag(retain)
                    .WithQualityOfServiceLevel(qos)
                    .Build();

                await _mqttClient.PublishAsync(msg);
            }
        }
    }
}
