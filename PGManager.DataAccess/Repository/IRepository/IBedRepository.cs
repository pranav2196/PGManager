using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository.IRepository
{
    public interface IBedRepository:IRepository<Bed>
    {
        Task UpdateAsync(Bed bed);
    }
}
