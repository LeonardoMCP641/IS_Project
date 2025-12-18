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

        private const string Broker = "127.0.0.1";
        private const int Port = 1883;

        private IMqttClient GetClient()
        {
            if (_client?.IsConnected == true)
                return _client;

            lock (_lock)
            {
                if (_client?.IsConnected == true)
                    return _client;

                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(Broker, Port)
                    .WithClientId(Guid.NewGuid().ToString())
                    .WithCleanSession()
                    .Build();

                _client.ConnectAsync(options).Wait();
                return _client;
            }
        }

        /// <summary>
        /// Sends a SOMIOD-compliant MQTT notification
        /// evt: 1 = creation, 2 = deletion
        /// </summary>
        public void NotifyContentInstanceEvent(
            string appRn,
            string containerRn,
            string ciRn,
            string contentType,
            string content,
            int evt)
        {
            var topic = $"api/somiod/{appRn}/{containerRn}";

            var payload = new
            {
                evt = evt,
                resource = new
                {
                    res_type = "content-instance",
                    resource_name = ciRn,
                    content_type = contentType,
                    content = content,
                    path = $"/api/somiod/{appRn}/{containerRn}/{ciRn}",
                    creation_datetime = DateTime.UtcNow
                        .ToString("yyyy-MM-ddTHH:mm:ss")
                }
            };

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(JsonConvert.SerializeObject(payload))
                .WithAtMostOnceQoS()
                .Build();

            GetClient().PublishAsync(message).Wait();
        }
    }
}
