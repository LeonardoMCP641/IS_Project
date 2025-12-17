using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace WebAPI.Models
{
   
    public class Application
    {
        [JsonProperty("res-type")]
        public string ResType => "application";

        [Key]
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("resource-name")]
        public string ResourceName { get; set; }
        [JsonProperty("creation-datetime")]
        [Newtonsoft.Json.JsonIgnore]
        public string CreationDateTime { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");


    }

}