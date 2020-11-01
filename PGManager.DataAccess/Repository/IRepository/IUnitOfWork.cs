using System;
using System.Collections.Generic;
using System.Text;

namespace PGManager.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository ApplicationUser { get; }

        ICityRepository City { get; }

        IFacilityRepository Facility { get; }

        IPGRepository PG { get; }

        IPhotoRepository Photo { get; }

        IPriceTierRepository PriceTier { get; }

        IRoomRepository Room { get; }

        IBedRepository Bed { get; }

        IUserDocumentRepository UserDocument { get;}

        IPGRequestRepository PGRequest { get; }

        IStayRepository Stay { get; }

        void Save();
    }
}
