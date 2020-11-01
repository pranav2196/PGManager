using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository.IRepository
{
    public interface IRoomRepository: IRepository<Room>
    {
        Task UpdateAsync(Room room);
    }
}
