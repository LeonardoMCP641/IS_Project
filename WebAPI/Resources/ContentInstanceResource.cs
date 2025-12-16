using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebAPI.Resources
{
    [XmlRoot("contentInstance")]
    public class ContentInstanceResource
    {
        public string res_type { get; set; } = "contentInstance";
        public string content { get; set; }
        public string creation_datetime { get; set; }
    }
}