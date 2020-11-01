using PGManager.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PGManager.Models.ViewModels
{
    public class PGRequestViewModel
    {
        public PGRequest Request { get; set; }

        [Required(ErrorMessage = "Select Action")]
        [StringRange(AllowableValues = new[] { "A", "R" }, ErrorMessage = "Invalid action")]
        public string Action { get; set; }

        [DisplayName("Room")]
        public int? RoomId { get; set; }

        [DisplayName("Bed")]
        public int? BedId { get; set; }
    }
}
