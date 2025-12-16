using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Endpoint { get; set; }

        [Required]
        public DateTime CreationDateTime { get; set; }

        public int ContainerId { get; set; }
    }
}