using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class Facility
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Facility Name")]
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        [Display(Name = "Icon Name")]
        public string Icon { get; set; }

        [NotMapped]
        public bool IsActive { get; set; }
    }
}
