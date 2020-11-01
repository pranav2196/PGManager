using PGManager.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        public int PGId { get; set; }

        [ForeignKey("PGId")]
        public PG PG { get; set; }

        [Required(ErrorMessage ="Price Tier is required")]
        public int PriceTierId { get; set; }

        [ForeignKey("PriceTierId")]
        [DisplayName("Price Tier")]
        public PriceTier PriceTier { get; set; }

        [DisplayName("Room No")]
        [Required(ErrorMessage ="Room Number is required")]
        [MinLength(1)]
        public string RoomNumber { get; set; }

        [DisplayName("Floor No")]
        [Required(ErrorMessage = "Floor is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Floor number")]
        public int Floor { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [StringRange(AllowableValues = new[] { "M", "F" }, ErrorMessage = "Gender must be either Male or Female.")]
        public char Gender { get; set; }

        public virtual IList<Bed> Beds { get; set; }

        public virtual IList<Stay> Stays { get; set; }
    }
}
