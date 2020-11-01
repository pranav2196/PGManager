using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PGManager.DataAccess.Repository
{
    public class PGRequestRepository : Repository<PGRequest>, IPGRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public PGRequestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
