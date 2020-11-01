using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PGManager.DataAccess.Repository
{
    public class PhotoRepository: Repository<Photo>, IPhotoRepository
    {
        private readonly ApplicationDbContext _db;

        public PhotoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
