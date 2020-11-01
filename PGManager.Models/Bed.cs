using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class Bed
    {
        [Key]
        public int Id { get; set; }

        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        [DisplayName("Bed No")]
        [Required(ErrorMessage = "Bed No is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Bed No")]
        public int BedNo { get; set; }

        public DateTime? LeaveDate { get; set; }

        public DateTime? JoinDate { get; set; }

        [NotMapped]
        public bool Occupied => (JoinDate != null && ((JoinDate?.Date <= DateTime.Now.Date && (LeaveDate == null || LeaveDate?.Date >= DateTime.Now.Date)) 
								|| JoinDate?.Date >= DateTime.Now.Date));

        public virtual IList<Stay> Stays { get; set; }
    }
}
