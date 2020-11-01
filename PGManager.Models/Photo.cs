using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PGManager.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }

        public int PGId { get; set; }

        [ForeignKey("PGId")]
        public PG PG { get; set; }

    }
}
