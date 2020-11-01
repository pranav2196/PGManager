using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class PriceTier
    {
        [Key]
        public int Id { get; set; }

        public int PGId { get; set; }

        [ForeignKey("PGId")]
        public PG PG { get; set; }

        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Rent is mandatory")]
        [Range(1, int.MaxValue, ErrorMessage = "Rent cannot be zero")]
        [DisplayName("Monthly Rent")]
        public double Rent { get; set; }

        public bool Active { get; set; }

        public virtual IList<Room> Rooms { get; set; }

        public virtual IList<PGRequest> PGRequests { get; set; }
    }
}
