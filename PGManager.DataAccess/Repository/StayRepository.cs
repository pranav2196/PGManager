using Microsoft.EntityFrameworkCore;
using PGManager.DataAccess.Data;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.Repository
{
	public class StayRepository : Repository<Stay>, IStayRepository
	{
		private readonly ApplicationDbContext _db;

		public StayRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public async Task StartStay(PGRequest request, int roomId, int bedId)
		{
			Stay stay = new Stay()
			{
				PGId = request.PGId,
				TenantId = request.ApplicantUserId,
				RoomId = roomId,
				BedId = bedId,
				StartDate = request.Date.Date,
				JoinRequestId = request.Id
			};

			request.LastActionOn = DateTime.Now;
			request.RequestStatus = RequestStatus.Accepted;

			var requests = await _db.PGRequests.Where(req => req.ApplicantUserId == request.ApplicantUserId && req.Id != request.Id).ToListAsync();
			if (requests != null)
			{
				foreach (var req in requests)
				{
					req.RequestStatus = RequestStatus.AutoRejected;
					req.LastActionOn = DateTime.Now;
				}
			}

			await _db.Stays.AddAsync(stay);
		}

		public async Task EndStay(PGRequest request)
		{
			Stay stay = await _db.Stays.Where(s => s.Id == request.StayId).SingleAsync();

			stay.EndDate = request.Date;
			stay.LeaveRequestId = request.Id;

			request.LastActionOn = DateTime.Now;
			request.RequestStatus = RequestStatus.Accepted;
		}
	}
}
