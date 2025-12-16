using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebAPI.Resources
{
    [XmlRoot("container")]
    public class ContainerResource
    {
        public string res_type { get; set; } = "container";
        public string resource_name { get; set; }
        public string creation_datetime { get; set; }
    }
}
