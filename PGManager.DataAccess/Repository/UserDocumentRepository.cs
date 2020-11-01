using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PGManager.DataAccess.Repository
{
    public class UserDocumentRepository:Repository<UserDocument>,IUserDocumentRepository
    {
        private readonly ApplicationDbContext _db;

        public UserDocumentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
