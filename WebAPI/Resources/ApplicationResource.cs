using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebAPI.Resources
{
    [XmlRoot("application")]
    public class ApplicationResource
    {
        public string res_type { get; set; } = "application";
        public string resource_name { get; set; }
        public string creation_datetime { get; set; }
    }
}
