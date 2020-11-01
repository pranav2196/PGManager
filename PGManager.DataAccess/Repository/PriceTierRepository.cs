using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository
{
    public class PriceTierRepository:Repository<PriceTier>, IPriceTierRepository
    {
        private readonly ApplicationDbContext _db;
        public PriceTierRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(PriceTier priceTier)
        {
            var objToUpdate = await _db.PriceTiers.FindAsync(priceTier.Id);

            objToUpdate.Name = priceTier.Name;
            objToUpdate.Rent = priceTier.Rent;
            objToUpdate.Active = priceTier.Active;
        }
    }
}
