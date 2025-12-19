using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Somiod.AppA.Promocoes.Models // Confirma se o namespace bate com o teu projeto
{
    public class ApplicationDTO
    {
        [JsonPropertyName("res-type")]
        public string ResType { get; set; } = "application";

        [JsonPropertyName("resource-name")]
        public string ResourceName { get; set; }
    }
    
}
