using System;
using System.Collections.Generic;
using System.Text;

namespace PGManager.Models.ViewModels
{
    public class PGEditViewModel
    {
        public PG PG { get; set; }

        public IList<Facility> Facilities { get; set; }
    }
}
