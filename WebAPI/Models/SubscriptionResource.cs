using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class SubscriptionResource
    {
        public int Id { get; set; }
        public string res_type { get; set; } = "subscription";
        public string resource_name { get; set; }
        public int evt { get; set; }            // 1 – creation; 2 – deletion; 3 – ambos
        public string endpoint { get; set; }    // "mqtt://..." ou "http://..."

        public DateTime creation_datetime { get; set; }

        public int ContainerId { get; set; }
        public ContainerResource Container { get; set; }
    }

}