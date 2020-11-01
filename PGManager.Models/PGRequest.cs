using PGManager.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class PGRequest
    {
        [Key]
        public int Id { get; set; }

        public int PGId { get; set; }

        [ForeignKey("PGId")]
        public PG PG { get; set; }

        public string ApplicantUserId { get; set; }

        [ForeignKey("ApplicantUserId")]
        public ApplicationUser Applicant { get; set; }

        [DisplayName("Price Category")]
        [Required(ErrorMessage ="Price category is required")]
        public int PriceTierId { get; set; }

        [ForeignKey("PriceTierId")]
        public PriceTier PriceTier { get; set; }

        [DisplayName("Request Type")]
        public RequestType RequestType { get; set; }

        [DisplayName("Request Status")]
        public RequestStatus RequestStatus { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [DisplayName("Request On")]
        public DateTime RequestOn { get; set; }

        [DisplayName("Last Action On")]
        public DateTime LastActionOn { get; set; }

        public string Description { get; set; }

        public int? StayId { get; set; }
    }
}
