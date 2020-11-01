using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository
{
    public class BedRepository : Repository<Bed>, IBedRepository
    {
        private readonly ApplicationDbContext _db;
        public BedRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Bed bed)
        {
            Bed bedToUpdate = await _db.Beds.FindAsync(bed.Id);
            
            bedToUpdate.BedNo = bed.BedNo;
        }
    }
}
