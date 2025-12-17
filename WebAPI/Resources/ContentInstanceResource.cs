using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebAPI.Resources
{
    [XmlRoot("content-instance")]
    public class ContentInstanceResource
    {
        public string res_type { get; set; } = "content-instance";

        public string resource_name { get; set; }

        public string content_type { get; set; }

        public string content { get; set; }

        public string creation_datetime { get; set; }
    }
}
