using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class Subscription
    {

        [JsonProperty("res-type")]
        public string ResType => "subscription";

        [Key]
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        
        [JsonProperty("resource-name")]
        public string ResourceName { get; set; }

        [Required]
        [JsonProperty("evt")]
        [Range(1,3)]
        public int Evt { get; set; }

        [Required]
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("creation-datetime")]
        [Newtonsoft.Json.JsonIgnore]
        public string CreationDateTime { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

        [ForeignKey("Container")]
        [Newtonsoft.Json.JsonIgnore]
        public int ContainerId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual Container Container { get; set; }


    }
}
