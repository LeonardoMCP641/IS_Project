using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class ContentInstanceResource
    {
        public int Id { get; set; }
        public string res_type { get; set; } = "content-instance";
        public string resource_name { get; set; }
        public string content_type { get; set; }  // "application/xml", "application/json", "text/plain"
        public string content { get; set; }
        public DateTime creation_datetime { get; set; }

        public int ContainerId { get; set; }
        public ContainerResource Container { get; set; }
    }

}