using System;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;

namespace WebAPI.Services
{
    public class MqttNotificationService
    {
        private static IMqttClient _client;
        private static readonly object _lock = new object();

        private const string Broker = "localhost";
        private const int Port = 1883;

        private IMqttClient GetClient()
        {
            if (_client != null && _client.IsConnected)
                return _client;

            lock (_lock)
            {
                if (_client != null && _client.IsConnected)
                    return _client;

                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(Broker, Port)
                    .WithClientId("somiod-middleware")
                    .WithCleanSession()
                    .Build();

                _client.ConnectAsync(options).Wait();
                return _client;
            }
        }

        public void PublishContentInstanceEvent(
            string appRn,
            string contRn,
            string ciRn,
            int evt)
        {
            var topic = $"api/somiod/{appRn}/{contRn}";

            var payload = new
            {
                evt = evt,
                resource = new
                {
                    res_type = "content-instance",
                    resource_name = ciRn,
                    path = $"/api/somiod/{appRn}/{contRn}/{ciRn}"
                },
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            var json = JsonConvert.SerializeObject(payload);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(json))
                .WithAtMostOnceQoS()
                .Build();

            GetClient().PublishAsync(message).Wait();
        }
    }
}
