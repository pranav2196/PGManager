using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository
{
    public class RoomRepository: Repository<Room>,IRoomRepository
    {
        private readonly ApplicationDbContext _db;
        public RoomRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Room room)
        {
            Room roomToUpdate = await _db.Rooms.FindAsync(room.Id);

            if (roomToUpdate != null)
            {
                roomToUpdate.Gender = room.Gender;
                roomToUpdate.Floor = room.Floor;
                roomToUpdate.RoomNumber = room.RoomNumber;
                roomToUpdate.PriceTierId = room.PriceTierId;
            }
        }
    }
}
