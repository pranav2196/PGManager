using Microsoft.AspNetCore.Identity;
using PGManager.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        [DisplayName("Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [StringRange(AllowableValues = new[] { "M", "F" }, ErrorMessage = "Gender must be either Male or Female.")]
        public char Gender { get; set; }

        public virtual IList<UserDocument> Documents { get; set; }

        [NotMapped]
        public string Role { get; set; }

		[NotMapped]
		public string Pwd { get; set; }
    }
}
