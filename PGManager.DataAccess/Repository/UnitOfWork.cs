using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PGManager.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        IApplicationUserRepository _applicationUser;
        ICityRepository _city;
        IFacilityRepository _facility;
        IPGRepository _pg;
        IPhotoRepository _photo;
        IPriceTierRepository _priceTier;
        IRoomRepository _room;
        IBedRepository _bed;
        IUserDocumentRepository _userDocument;
        IPGRequestRepository _pgRequest;
        IStayRepository _stay;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
        }

        public IApplicationUserRepository ApplicationUser
        {
            get
            {
                if (_applicationUser == null)
                    _applicationUser = new ApplicationUserRepository(_db);
                return _applicationUser;
            }
        }
        public ICityRepository City
        {
            get
            {
                if (_city == null)
                    _city = new CityRepository(_db);
                return _city;
            }
        }
        public IFacilityRepository Facility
        {
            get
            {
                if (_facility == null)
                    _facility = new FacilityRepository(_db);
                return _facility;
            }
        }

        public IPGRepository PG
        {
            get
            {
                if (_pg == null)
                    _pg = new PGRepository(_db);
                return _pg;
            }
        }

        public IPhotoRepository Photo
        {
            get
            {
                if (_photo == null)
                    _photo = new PhotoRepository(_db);
                return _photo;
            }
        }

        public IPriceTierRepository PriceTier
        {
            get
            {
                if (_priceTier == null)
                    _priceTier = new PriceTierRepository(_db);
                return _priceTier;
            }
        }

        public IRoomRepository Room
        {
            get
            {
                if (_room == null)
                    _room = new RoomRepository(_db);
                return _room;
            }
        }

        public IBedRepository Bed
        {
            get
            {
                if (_bed == null)
                    _bed = new BedRepository(_db);
                return _bed;
            }
        }

        public IUserDocumentRepository UserDocument
        {
            get
            {
                if (_userDocument == null)
                    _userDocument = new UserDocumentRepository(_db);
                return _userDocument;
            }
        }

        public IPGRequestRepository PGRequest
        {
            get
            {
                if (_pgRequest == null)
                    _pgRequest = new PGRequestRepository(_db);
                return _pgRequest;
            }
        }


        public IStayRepository Stay
        {
            get
            {
                if (_stay == null)
                    _stay = new StayRepository(_db);
                return _stay;
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
