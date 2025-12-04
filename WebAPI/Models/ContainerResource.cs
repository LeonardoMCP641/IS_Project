using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class ContainerResource
    {
        public int Id { get; set; }
        public string res_type { get; set; } = "container";
        public string resource_name { get; set; }
        public DateTime creation_datetime { get; set; }

        public int ApplicationId { get; set; }
        public ApplicationResource Application { get; set; }
    }

}