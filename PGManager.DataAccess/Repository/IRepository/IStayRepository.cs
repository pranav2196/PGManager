using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository.IRepository
{
    public interface IStayRepository:IRepository<Stay>
    {
        Task StartStay(PGRequest request, int roomId, int bedId);

        Task EndStay(PGRequest request);
    }
}
