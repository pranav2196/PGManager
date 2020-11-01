using Microsoft.EntityFrameworkCore;
using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository
{
    public class PGRepository : Repository<PG>, IPGRepository
    {
        private readonly ApplicationDbContext _db;

        public PGRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<PG> GetAsync(string ApplicationUserID, bool IncludeFacilities = false, 
            bool IncludeCity = false, bool IncludePhotos = false, bool IncludePriceTiers = false, bool IncludeRooms=false)
        {
            var query = _db.PG.Where(p => p.ApplicationUserId == ApplicationUserID);
            if (IncludeFacilities)
                query = query.Include(f => f.Facilities).ThenInclude(af => af.Facility);
            if (IncludeCity)
                query = query.Include(c => c.City);
            if (IncludePhotos)
                query = query.Include(p => p.Photos);
            if (IncludePriceTiers)
                query = query.Include(pt => pt.PriceTiers);
            if (IncludeRooms)
                query = query.Include(r => r.Rooms);
            return await query.SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Facility>> GetFacilitiesForPG(int PGId)
        {
            IEnumerable<Facility> facilities = await (from f in _db.Facilities
                                                      join af in _db.AvailableFacilities
                                                      on new { p1 = f.Id, p2 = PGId } equals new { p1 = af.FacilityID, p2 = af.PGId } into Mapping
                                                      from m in Mapping.DefaultIfEmpty()
                                                      select new Facility()
                                                      {
                                                          Id = f.Id,
                                                          Icon = f.Icon,
                                                          Name = f.Name,
                                                          IsActive = m != null
                                                      }).ToListAsync();
            return facilities;
        }

        public async Task Update(PG pg, IList<Facility> facilities)
        {
            List<AvailableFacility> newFacilities = null;
            if (pg.Id == 0)
            {
                await _db.PG.AddAsync(pg);
                await _db.SaveChangesAsync();
                newFacilities = facilities.Where(f => f.IsActive).Select(f => new AvailableFacility()
                {
                    FacilityID = f.Id,
                    PGId = pg.Id
                }).ToList();
            }
            else
            {
                var pgToEdit = await _db.PG.FindAsync(pg.Id);
                pgToEdit.Name = pg.Name;
                pgToEdit.EmailAddress = pg.EmailAddress;
                pgToEdit.Address = pg.Address;
                pgToEdit.CityId = pg.CityId;
                pgToEdit.Description = pg.Description;
                pgToEdit.Phone = pg.Phone;
                pgToEdit.IsMale = pg.IsMale;
                pgToEdit.IsFemale = pg.IsFemale;
                pgToEdit.IsActive = pg.IsActive;

                var activeFacilities = facilities.Where(f => f.IsActive).Select(f => f.Id).ToArray();

                var facilitiesToRemove = await (from af in _db.AvailableFacilities.Where(f => !activeFacilities.Contains(f.FacilityID) && f.PGId == pgToEdit.Id)
                                                select new AvailableFacility()
                                                {
                                                    Id = af.Id
                                                }
                                                ).ToListAsync();

                _db.AvailableFacilities.RemoveRange(facilitiesToRemove);

                var availableFacilities = await (from af in _db.AvailableFacilities.Where(f => activeFacilities.Contains(f.FacilityID) && f.PGId == pgToEdit.Id)
                                                 select af.FacilityID
                                                ).ToArrayAsync();

                newFacilities = facilities.Where(f => !availableFacilities.Contains(f.Id) && f.IsActive).Select(f => new AvailableFacility()
                {
                    FacilityID = f.Id,
                    PGId = pgToEdit.Id
                }).ToList();
            }

            if (newFacilities != null)
                await _db.AvailableFacilities.AddRangeAsync(newFacilities);

            await _db.SaveChangesAsync();
        }
    }
}
