using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class ApplicationCreateDto
    {
        [JsonProperty("res-type")]
        public string ResType { get; set; } // optional, but validated if present

        [JsonProperty("resource-name")]
        public string ResourceName { get; set; } // optional per enunciado
    }

    public class ApplicationUpdateDto
    {
        [JsonProperty("res-type")]
        public string ResType { get; set; }

        [JsonProperty("resource-name")]
        public string ResourceName { get; set; } // allow rename (still must be unique)
    }

    public class ContainerCreateDto
    {
        [JsonProperty("res-type")]
        public string ResType { get; set; }

        [JsonProperty("resource-name")]
        public string ResourceName { get; set; } // optional
    }

    public class ContainerUpdateDto
    {
        [JsonProperty("res-type")]
        public string ResType { get; set; }

        [JsonProperty("resource-name")]
        public string ResourceName { get; set; } // allow rename (unique within app)
    }

    public class ContentInstanceCreateDto
    {
        [JsonProperty("res-type")]
        public string ResType { get; set; }

        [JsonProperty("resource-name")]
        public string ResourceName { get; set; } // optional


        [Required]
        [JsonProperty("content-type")]
        public string ContentType { get; set; }

        [Required]
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class SubscriptionCreateDto
    {
        [JsonProperty("res-type")]
        public string ResType { get; set; }

        [JsonProperty("resource-name")]
        public string ResourceName { get; set; } // optional

        [Range(1, 2)]
        [JsonProperty("evt")]
        public int Evt { get; set; }

        [Required]
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }
    }
}
