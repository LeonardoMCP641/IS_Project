using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ResourceName { get; set; }   

        [Required]
        public int Evt { get; set; }               

        [Required]
        public string Endpoint { get; set; }      

        [Required]
        public DateTime CreationDateTime { get; set; }

        public int ContainerId { get; set; }
    }
}
