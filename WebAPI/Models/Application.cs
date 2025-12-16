using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    /// <summary>
    /// Database model for Application resource.
    /// Reflects the Applications table.
    /// </summary>
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ResourceName { get; set; }

        [Required]
        public DateTime CreationDateTime { get; set; }
    }
}