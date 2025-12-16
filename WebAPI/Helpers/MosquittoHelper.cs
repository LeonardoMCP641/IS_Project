using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using uPLibrary.Networking.M2Mqtt;
using WebAPI.Resources;

namespace WebAPI.Helpers
{
    /// <summary>
    /// Helper class to publish SOMIOD notifications using MQTT (Mosquitto).
    /// </summary>
    public class MosquittoHelper
    {
        /// <summary>
        /// Publishes a ContentInstance to an MQTT broker.
        /// </summary>
        /// <param name="endpoint">MQTT broker address (e.g. localhost or test.mosquitto.org)</param>
        /// <param name="topic">MQTT topic (application/container)</param>
        /// <param name="contentInstance">ContentInstance resource to publish</param>
        public static void PublishContentInstance(
            string endpoint,
            string topic,
            ContentInstanceResource contentInstance)
        {
            try
            {
                var client = new MqttClient(endpoint);
                client.Connect(Guid.NewGuid().ToString());

                if (!client.IsConnected)
                    throw new Exception("Could not connect to MQTT broker.");

                var payload = Encoding.UTF8.GetBytes(SerializeToXml(contentInstance));
                client.Publish(topic, payload);

                client.Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception("Error publishing MQTT message.", ex);
            }
        }

        // ---------------- PRIVATE ----------------

        private static string SerializeToXml(object obj)
        {
            try
            {
                var serializer = new XmlSerializer(obj.GetType());
                using (var ms = new MemoryStream())
                {
                    serializer.Serialize(ms, obj);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                throw new Exception("Error serializing object to XML.");
            }
        }
    }
}
