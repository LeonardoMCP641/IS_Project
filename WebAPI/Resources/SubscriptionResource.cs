using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebAPI.Resources
{
    [XmlRoot("subscription")]
    public class SubscriptionResource
    {
        public string res_type { get; set; } = "subscription";
        public string endpoint { get; set; }
        public string creation_datetime { get; set; }
    }
}