using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class Container
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ResourceName { get; set; }

        [Required]
        public DateTime CreationDateTime { get; set; }

        public int ApplicationId { get; set; }
    }
}