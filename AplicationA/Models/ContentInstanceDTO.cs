using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AplicationA.Models
{
    internal class ContentInstanceDTO
    {
        [JsonPropertyName("res-type")]
        public string ResType { get; set; } = "content-instance";

        [JsonPropertyName("resource-name")]
        public string ResourceName { get; set; }

        [JsonPropertyName("content-type")]
        public string ContentType { get; set; } // ex: "text/plain" ou "application/json"

        [JsonPropertyName("content")]
        public string Content { get; set; } // O valor da promoção (ex: "10€")
    }
}
