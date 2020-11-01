using Microsoft.EntityFrameworkCore;
using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository
{
    public class FacilityRepository:Repository<Facility>,IFacilityRepository
    {
        private readonly ApplicationDbContext _db;

        public FacilityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task Update(Facility facility)
        {
            var objFromDb = await _db.Facilities.FirstOrDefaultAsync(s => s.Id == facility.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = facility.Name;
                objFromDb.Icon = facility.Icon;
            }
        }
    }
}
