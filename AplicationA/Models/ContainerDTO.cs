using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AplicationA.Models
{
    public class ContainerDTO
    {
        [JsonPropertyName("res-type")]
        public string ResType { get; set; } = "container";

        [JsonPropertyName("resource-name")]
        public string ResourceName { get; set; }
    }
}
