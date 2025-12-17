using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class ContentInstance
    {

        [JsonProperty("res-type")]
        public string ResType => "content-instance";

        [Key]
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        
        [JsonProperty("resource-name")]
        public string ResourceName { get; set; }

        [Required]
        [JsonProperty("content-type")]
        public string ContentType { get; set; }

        [Required]
        [JsonProperty("content")]
        public string Content { get; set; }


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
