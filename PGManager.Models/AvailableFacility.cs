using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class AvailableFacility
    {
        [Key]
        public int Id { get; set; }

        public int FacilityID { get; set; }

        [ForeignKey("FacilityID")]
        public Facility Facility { get; set; }

        public int PGId { get; set; }

        [ForeignKey("PGId")]
        public PG PG { get; set; }


    }
}
