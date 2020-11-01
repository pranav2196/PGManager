using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository.IRepository
{
    public interface IPGRepository : IRepository<PG>
    {
        Task<PG> GetAsync(string ApplicationUserID, bool IncludeFacilities = false, bool IncludeCity = false,
            bool IncludePhotos = false, bool IncludePriceTiers = false, bool IncludeRooms = false);

        Task<IEnumerable<Facility>> GetFacilitiesForPG(int PGId);

        Task Update(PG pg, IList<Facility> facilities);

    }
}
