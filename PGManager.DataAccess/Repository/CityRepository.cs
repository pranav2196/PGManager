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
    public class CityRepository : Repository<City>, ICityRepository
    {
        private readonly ApplicationDbContext _db;

        public CityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task Update(City city)
        {
            var objFromDb = await _db.Cities.FirstOrDefaultAsync(s => s.Id == city.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = city.Name;

            }
        }
    }
}
