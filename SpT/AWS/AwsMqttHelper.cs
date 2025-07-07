using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using HslCommunication.MQTT;
using Newtonsoft.Json;

public static class AwsMqttHelper
{
    private static MqttClient mqttClient;

    //public static async Task PublishAsync(object payloadObj, string topic)
    //{
    //    // Nếu payloadObj là object, tự serialize sang JSON
    //    string jsonPayload = payloadObj is string str ? str : JsonConvert.SerializeObject(payloadObj);

    //    var factory = new MqttFactory();
    //    using (var mqttClient = factory.CreateMqttClient())
    //    {
    //        var options = new MqttClientOptionsBuilder()
    //            .WithClientId("GenZPublisher_" + Guid.NewGuid().ToString("N"))
    //            .WithTcpServer("your-endpoint-ats.iot.ap-southeast-1.amazonaws.com", 8883)
    //            .WithTls(new MqttClientOptionsBuilderTlsParameters
    //            {
    //                UseTls = true,
    //                Certificates = new List<X509Certificate>
    //                {
    //                    new X509Certificate2(@"C:\AWS\device-certificate.pem.crt"),
    //                    new X509Certificate2(@"C:\AWS\private.pem.key"),
    //                    new X509Certificate2(@"C:\AWS\AmazonRootCA1.pem")
    //                },
    //                AllowUntrustedCertificates = false,
    //                IgnoreCertificateChainErrors = false,
    //                IgnoreCertificateRevocationErrors = false
    //            })
    //            .Build();

    //        mqttClient.ConnectedAsync += (s, e) => Console.WriteLine("✅ AWS MQTT Connected");
    //        mqttClient.Disconnected += (s, e) => Console.WriteLine("❌ AWS MQTT Disconnected");


    //        try
    //        {
    //            await mqttClient.ConnectAsync(options);
    //            Console.WriteLine("⏳ Đang publish JSON...");

    //            var message = new MqttApplicationMessageBuilder()
    //                .WithTopic(topic)
    //                .WithPayload(jsonPayload)
    //                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
    //                .Build();

    //            await mqttClient.PublishAsync(message);
    //            Console.WriteLine("✅ JSON đã publish lên AWS IoT Core");

    //            await mqttClient.DisconnectAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine("❌ Lỗi khi publish AWS MQTT: " + ex.Message);
    //        }
    //    }
    //}

    public static async Task PulishAsync(string payloadObj, string topic)
    {
        // Nếu payloadObj là object, tự serialize sang JSON
        string jsonPayload = payloadObj is string str ? str : JsonConvert.SerializeObject(payloadObj);

        mqttClient = new MqttClient(new MqttConnectionOptions()
        {
            ClientId = "ABC",
            IpAddress = "127.0.0.1",
            Credentials = new MqttCredential("admin", "123456"),   // 设置了用户名和密码
        });
    }
}
