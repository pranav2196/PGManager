using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PGManager.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "City Name")]
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
    }
}
