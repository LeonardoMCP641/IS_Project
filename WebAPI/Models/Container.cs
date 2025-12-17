using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebAPI.Models
{
    public class Container
    {
        [JsonProperty("res-type")]
        public string ResType => "container";
        [Key]
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        
        [JsonProperty("resource-name")]
        public string ResourceName { get; set; }

        [JsonProperty("creation-datetime")]
        [Newtonsoft.Json.JsonIgnore]
        public string CreationDateTime { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");


        [ForeignKey("Application")]
        [Newtonsoft.Json.JsonIgnore]
        public int ApplicationId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Application Application { get; set; }


    }
}