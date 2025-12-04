using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class ApplicationResource
    {
        public int Id { get; set; }            // interno BD
        public string res_type { get; set; } = "application";
        public string resource_name { get; set; }
        public DateTime creation_datetime { get; set; }
    }
}