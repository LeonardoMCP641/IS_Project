using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class ContentInstance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ResourceName { get; set; }    

        [Required]
        public string ContentType { get; set; }     

        [Required]
        public string Content { get; set; }         

        [Required]
        public DateTime CreationDateTime { get; set; }

        public int ContainerId { get; set; }
    }
}
