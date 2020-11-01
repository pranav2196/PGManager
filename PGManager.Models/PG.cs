using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class PG
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage ="Invalid email")]
        [Required(ErrorMessage = "Email is required")]
        [DisplayName("Email")]
        public string EmailAddress { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(?<!\d)\d{10}(?!\d)", ErrorMessage = "Enter a valid phone number")]
        [Required(ErrorMessage ="Phone number is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage ="City is required")]
        [DisplayName("City")]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsMale { get; set; }

        public bool IsFemale { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual IList<AvailableFacility> Facilities { get; set; }

        public virtual IList<Photo> Photos { get; set; }

        public virtual IList<PriceTier> PriceTiers { get; set; }

        public virtual IList<Room> Rooms { get; set; }

        public virtual IList<Stay> Stays { get; set; }
    }
}
