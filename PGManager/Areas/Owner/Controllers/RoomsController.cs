using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Utilities;

namespace PGManager.Areas.Owner.Controllers
{
	[Area("Owner")]
	[Authorize(Roles = SD.Role_User_Owner)]
	public class RoomsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public RoomsController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			PG pg;
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			pg = await _unitOfWork.PG.GetAsync(claim.Value, IncludeRooms: true);

			return View(pg.Rooms);
		}

		[HttpGet]
		public async Task<IActionResult> Upsert(int id)
		{
			Room room;
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			PG pg = await _unitOfWork.PG.GetAsync(claim.Value, IncludePriceTiers: true);

			IList<SelectListItem> tiers = pg.PriceTiers.Select(pt => new SelectListItem()
			{
				Text = $"{pt.Name} (₹{String.Format("{0:n2}", pt.Rent)})",
				Value = pt.Id.ToString()
			}).ToList();

			ViewBag.Tiers = tiers;
			ViewBag.Male = pg.IsMale;
			ViewBag.Female = pg.IsFemale;

			if (id == 0)
				room = new Room();
			else
				room = await _unitOfWork.Room.GetAsync(id);

			return View(room);
		}

		[HttpPost]
		public async Task<IActionResult> Upsert(Room room)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			PG pg = await _unitOfWork.PG.GetAsync(claim.Value, IncludePriceTiers: true);

			if (ModelState.IsValid)
			{

				if (pg.PriceTiers.Any(pt => pt.Id == room.PriceTierId))
				{
					if (room.Id == 0)
					{
						room.PGId = pg.Id;
						await _unitOfWork.Room.AddAsync(room);
					}
					else
					{
						await _unitOfWork.Room.UpdateAsync(room);
					}
					_unitOfWork.Save();
				}
				else
				{
					IList<SelectListItem> tiers = pg.PriceTiers.Select(pt => new SelectListItem()
					{
						Text = $"{pt.Name} (₹{String.Format("{0:n2}", pt.Rent)})",
						Value = pt.Id.ToString()
					}).ToList();

					ViewBag.Tiers = tiers;
					ViewBag.Male = pg.IsMale;
					ViewBag.Female = pg.IsFemale;

					return View(room);
				}
			}
			else
			{
				IList<SelectListItem> tiers = pg.PriceTiers.Select(pt => new SelectListItem()
				{
					Text = $"{pt.Name} (₹{String.Format("{0:n2}", pt.Rent)})",
					Value = pt.Id.ToString()
				}).ToList();

				ViewBag.Tiers = tiers;
				ViewBag.Male = pg.IsMale;
				ViewBag.Female = pg.IsFemale;

				return View(room);
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> EditBed(int id, int roomId)
		{
			Bed bed;
			if (id == 0)
			{
				bed = new Bed()
				{
					RoomId = roomId
				};
			}
			else
			{
				bed = await _unitOfWork.Bed.GetAsync(id);
			}

			return View(bed);
		}

		[HttpPost]
		public async Task<IActionResult> EditBed(Bed bed)
		{
			if (ModelState.IsValid)
			{
				if (bed.Id == 0)
					await _unitOfWork.Bed.AddAsync(bed);
				else
					await _unitOfWork.Bed.UpdateAsync(bed);
				_unitOfWork.Save();

				return RedirectToAction(nameof(Details), new { id = bed.RoomId });
			}
			else
				return View(bed);

		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			Room room = await _unitOfWork.Room.GetAsync((r => r.Id == id), includeProperties: "PriceTier,Beds");

			return View(room);
		}

		[HttpGet]
		public async Task<IActionResult> StayHistory(int id)
		{
			if (id == 0)
				return RedirectToAction(nameof(Index));

			var bed = await _unitOfWork.Bed.GetAsync(bed => bed.Id == id, includeProperties: "Room.PG");

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (bed == null || bed.Room.PG.ApplicationUserId != claim.Value)
				return RedirectToAction(nameof(Index));

			var stays = await _unitOfWork.Stay.GetAllAsync(stay => stay.BedId == bed.Id,
															includeProperties: "Room,Bed,PG,Tenant",
															orderBy: stay => stay.OrderByDescending(s => s.StartDate));

			return View(stays);
		}

	}
}


