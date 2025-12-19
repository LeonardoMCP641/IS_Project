using System;
using System.Linq;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class MqttNotificationService
    {
        private readonly SomiodDbContext db = new SomiodDbContext();

        private MqttClient client;

        public MqttNotificationService()
        {
            // Inicializa o cliente MQTT
            client = new MqttClient("127.0.0.1"); // IP do broker
            client.Connect(Guid.NewGuid().ToString());
        }

        public void NotifyContentInstanceEvent(string appRn, string contRn, string ciRn, string contentType, string content, int evt)
        {
            var subscriptions = db.Subscriptions
                .Where(s => s.Container.ResourceName == contRn &&
                            s.Container.Application.ResourceName == appRn &&
                            (s.Evt == evt || s.Evt == 3)) // evt 3 = both
                .ToList();

            foreach (var sub in subscriptions)
            {
                if (sub.Endpoint.StartsWith("mqtt://"))
                {
                    var topic = $"api/somiod/{appRn}/{contRn}";
                    var payload = new
                    {
                        AppRn = appRn,
                        ContRn = contRn,
                        CiRn = ciRn,
                        ContentType = contentType,
                        Content = content,
                        Event = evt == 1 ? "creation" : "deletion"
                    };
                    var msg = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(payload));
                    client.Publish(topic, msg, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                }
                else if (sub.Endpoint.StartsWith("http://") || sub.Endpoint.StartsWith("https://"))
                {
                    using (var wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                        var payload = new
                        {
                            AppRn = appRn,
                            ContRn = contRn,
                            CiRn = ciRn,
                            ContentType = contentType,
                            Content = content,
                            Event = evt == 1 ? "creation" : "deletion"
                        };
                        wc.UploadString(sub.Endpoint, System.Text.Json.JsonSerializer.Serialize(payload));
                    }
                }
            }
        }
    }
}

