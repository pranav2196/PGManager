using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Models.Enums;
using PGManager.Models.ViewModels;
using PGManager.Utilities;

namespace PGManager.Areas.Owner.Controllers
{
	[Area("Owner")]
	[Authorize(Roles = SD.Role_User_Owner)]
	public class RequestController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		#region Constructor
		public RequestController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		#endregion

		#region Actions
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			PG pg = await _unitOfWork.PG.GetAsync(claim.Value);

			var requests = await _unitOfWork.PGRequest.GetAllAsync(req => req.PGId == pg.Id && req.RequestStatus == RequestStatus.Pending,
																	includeProperties: "Applicant,PriceTier",
																	orderBy: x => x.OrderByDescending(req => req.Date));

			return View(requests);
		}

		[HttpGet]
		public async Task<IActionResult> UserDetails(string id)
		{
			var user = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == id, includeProperties: "Documents");
			if (user == null)
			{
				user = new ApplicationUser();
			}

			return View(user);
		}

		[HttpGet]
		public async Task<IActionResult> Approve(int id)
		{
			PGRequestViewModel requestVM = new PGRequestViewModel();
			requestVM.Request = await _unitOfWork.PGRequest.GetAsync(req => req.Id == id, includeProperties: "PG,PriceTier,Applicant");

			var rooms = await _unitOfWork.Room.GetAllAsync(room => room.PriceTierId == requestVM.Request.PriceTierId);


			IList<SelectListItem> roomList = rooms.Select(r => new SelectListItem()
			{
				Text = r.RoomNumber,
				Value = r.Id.ToString()
			}).ToList();

			ViewBag.RoomList = roomList;

			return View(requestVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Approve(PGRequestViewModel requestVM)
		{
			requestVM.Request = await _unitOfWork.PGRequest.GetAsync(requestVM.Request.Id);

			if (requestVM.Action == "A" && requestVM.Request.RequestType == RequestType.Join)
			{
				if (requestVM.RoomId == null || requestVM.RoomId == 0)
					ModelState.AddModelError("RoomId", "Please Select Room");
				if (requestVM.BedId == null || requestVM.BedId == 0)
					ModelState.AddModelError("BedId", "Please Select Bed");
			}
			if (ModelState.IsValid)
			{

				if (requestVM.Action == "A")
				{
					if (requestVM.Request.RequestType == RequestType.Join)
					{
						PG pg = await _unitOfWork.PG.GetAsync(pg => pg.Id == requestVM.Request.PGId, includeProperties: "Rooms.Beds");
						var room = pg.Rooms.Where(room => room.Id == requestVM.RoomId).SingleOrDefault();
						if (room != null)
						{
							var bed = room.Beds.Where(bed => bed.Id == requestVM.BedId).SingleOrDefault();
							if (bed != null)
							{
								await _unitOfWork.Stay.StartStay(requestVM.Request, room.Id, bed.Id);
								bed.JoinDate = requestVM.Request.Date;
							}
							else
								ModelState.AddModelError("BedId", "Invalid Bed");
						}
						else
							ModelState.AddModelError("RoomId", "Invalid Room");
					}
					else if (requestVM.Request.RequestType == RequestType.Leave)
					{
						if (requestVM.Request.StayId == null)
							ModelState.AddModelError("", "Invalid Request");
						else
							await _unitOfWork.Stay.EndStay(requestVM.Request);
					}
				}
				else
				{
					requestVM.Request.LastActionOn = DateTime.Now;
					requestVM.Request.RequestStatus = RequestStatus.Rejected;
				}
				if (ModelState.IsValid)
				{
					_unitOfWork.Save();
					return RedirectToAction(nameof(Index));
				}
			}

			requestVM.Request = await _unitOfWork.PGRequest.GetAsync(req => req.Id == requestVM.Request.Id, includeProperties: "PG,PriceTier,Applicant");

			var rooms = await _unitOfWork.Room.GetAllAsync(room => room.PriceTierId == requestVM.Request.PriceTierId);

			IList<SelectListItem> roomList = rooms.Select(r => new SelectListItem()
			{
				Text = r.RoomNumber,
				Value = r.Id.ToString()
			}).ToList();

			ViewBag.RoomList = roomList;

			return View(requestVM);
		}

		[HttpGet]
		public async Task<JsonResult> GetBeds(int id)
		{
			var beds = await _unitOfWork.Bed.GetAllAsync(bed => bed.RoomId == id);

			beds = beds.Where(bed => !bed.Occupied).ToList();

			return Json(beds);
		}
		#endregion
	}
}
