using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class Stay
    {
        [Key]
        public int Id { get; set; }

        public int PGId { get; set; }

        [ForeignKey("PGId")]
        public PG PG { get; set; }

        public string TenantId { get; set; }

        [ForeignKey("TenantId")]
        public ApplicationUser Tenant { get; set; }

        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        public int BedId { get; set; }

        [ForeignKey("BedId")]
        public Bed Bed { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int JoinRequestId { get; set; }

        [ForeignKey("JoinRequestId")]
        public PGRequest JoinRequest { get; set; }

        public int? LeaveRequestId { get; set; }

        [ForeignKey("LeaveRequestId")]
        public PGRequest LeaveRequest { get; set; }

        [NotMapped]
        public bool IsActive => (StartDate.Date <= DateTime.Now.Date && (EndDate == null || EndDate?.Date >= DateTime.Now.Date));

        [NotMapped]
        public bool LeaveRequestPlaced { get; set; }

        [NotMapped]
        public bool LeaveRequestAccepted { get; set; }
    }
}
