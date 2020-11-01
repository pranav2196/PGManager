using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Internal;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Models.Enums;
using PGManager.Utilities;

namespace PGManager.Areas.Tenant.Controllers
{
	[Area("Tenant")]
	[Authorize(Roles = SD.Role_User_Customer)]
	public class PGController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public PGController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string City, string Gender)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var currentstay = await _unitOfWork.Stay.GetAsync(s => s.TenantId == claim.Value &&
																(s.StartDate.Date >= DateTime.Now.Date ||
																(s.EndDate == null && s.StartDate.Date <= DateTime.Now.Date) ||
																(s.EndDate != null && ((DateTime)s.EndDate).Date >= DateTime.Now.Date)));

			Expression<Func<PG, bool>> filter = pg => pg.IsActive &&
												(
													(!string.IsNullOrEmpty(City) && pg.City.Name == City) ||
													string.IsNullOrEmpty(City)
												) &&
												(
													(!string.IsNullOrEmpty(Gender) &&
														(
															(Gender == "M" && pg.IsMale) || (Gender == "F" && pg.IsFemale)
														)
													) ||
													string.IsNullOrEmpty(Gender)
												) &&
												(
													currentstay == null ||
													(currentstay != null && pg.Id != currentstay.PGId)
												);

			var pgList = await _unitOfWork.PG.GetAllAsync(filter, includeProperties: "Facilities.Facility,Photos,PriceTiers,City");

			IEnumerable<City> cities = await _unitOfWork.City.GetAllAsync();

			ViewBag.CityList = cities.Select(c => new SelectListItem()
			{
				Text = c.Name,
				Value = c.Name,
				Selected = c.Name == City
			}).ToList();

			ViewBag.SelectedGender = Gender ?? string.Empty;

			return View(pgList);
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			if (id == 0)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				var pg = await _unitOfWork.PG.GetAsync(pg => pg.IsActive && pg.Id == id, includeProperties: "Facilities.Facility,Photos,PriceTiers,City");
				if (pg == null)
					return RedirectToAction(nameof(Index));
				else
				{
					var claimsIdentity = (ClaimsIdentity)User.Identity;
					var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
					ViewBag.ActiveStay = (await _unitOfWork.Stay.GetAllAsync(s => s.PGId == pg.Id && s.TenantId == claim.Value && (s.StartDate.Date >= DateTime.Now.Date ||
																		(s.StartDate.Date <= DateTime.Now.Date && (s.EndDate == null || ((DateTime)s.EndDate).Date >= DateTime.Now.Date))))).Any();
					
					ViewBag.Applied = (await _unitOfWork.PGRequest.GetAllAsync(req=>req.PGId==pg.Id && req.ApplicantUserId==claim.Value && 
																				req.RequestType==RequestType.Join && req.RequestStatus==RequestStatus.Pending)).Any();
					return View(pg);
				}
			}
		}

		[HttpGet]
		public async Task<IActionResult> Apply(int id)
		{
			if (id == 0)
				return RedirectToAction(nameof(Index));

			var priceTiers = await _unitOfWork.PriceTier.GetAllAsync(pt => pt.PGId == id);

			IList<SelectListItem> tiers = priceTiers.Select(pt => new SelectListItem()
			{
				Text = $"{pt.Name} (₹{String.Format("{0:n2}", pt.Rent)})",
				Value = pt.Id.ToString()
			}).ToList();

			ViewBag.PriceTiers = tiers;

			PGRequest request = new PGRequest()
			{
				PGId = id
			};

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			//check
			bool activeStay = (await _unitOfWork.Stay.GetAsync(s => s.TenantId == claim.Value &&
																(s.StartDate.Date >= DateTime.Now.Date ||
																(s.EndDate == null && s.StartDate.Date <= DateTime.Now.Date) ||
																(s.EndDate != null && ((DateTime)s.EndDate).Date >= DateTime.Now.Date)))) != null;
			if (activeStay)
				ModelState.AddModelError("", "You cannot apply to other PG before exiting current one");
			ViewBag.CurrentStay = activeStay;

			return View(request);
		}

		[HttpPost]
		public async Task<IActionResult> Apply(PGRequest request)
		{
			if (!ModelState.IsValid)
			{
				var priceTiers = await _unitOfWork.PriceTier.GetAllAsync(pt => pt.PGId == request.PGId);

				IList<SelectListItem> tiers = priceTiers.Select(pt => new SelectListItem()
				{
					Text = $"{pt.Name} (₹{String.Format("{0:n2}", pt.Rent)})",
					Value = pt.Id.ToString()
				}).ToList();

				ViewBag.PriceTiers = tiers;

				return View(request);
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
				PGRequest req = new PGRequest()
				{
					RequestType = RequestType.Join,
					RequestStatus = RequestStatus.Pending,
					ApplicantUserId = claim.Value,
					RequestOn = request.LastActionOn = DateTime.Now,
					Description = request.Description,
					Date = request.Date,
					PriceTierId = request.PriceTierId,
					PGId = request.PGId
				};
				await _unitOfWork.PGRequest.AddAsync(req);
				_unitOfWork.Save();

				return RedirectToAction(nameof(Index));
			}
		}

		[HttpGet]
		public async Task<IActionResult> Leave(int id)
		{
			if (id == 0)
				return RedirectToAction(nameof(MyStays));

			var stay = await _unitOfWork.Stay.GetAsync(s => s.Id == id);
			if (stay == null)
				return RedirectToAction(nameof(MyStays));

			PGRequest request = new PGRequest()
			{
				PGId = stay.PGId,
				StayId = stay.Id
			};

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			return View(request);
		}

		[HttpPost]
		public async Task<IActionResult> Leave(PGRequest request)
		{

			var stay = await _unitOfWork.Stay.GetAsync(s => s.Id == request.StayId, includeProperties: "Tenant,Room");

			if (stay != null)
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

				if (stay.TenantId != claim.Value)
					ModelState.AddModelError("", "Invalid Request");
				if (stay.StartDate >= request.Date)
					ModelState.AddModelError("Date", "Date must be greater than joining date");
				if (DateTime.Now.Date > request.Date)
					ModelState.AddModelError("Date", "Date cannot be in past");
				if (ModelState.IsValid)
				{
					PGRequest req = new PGRequest()
					{
						RequestType = RequestType.Leave,
						RequestStatus = RequestStatus.Pending,
						ApplicantUserId = claim.Value,
						RequestOn = request.LastActionOn = DateTime.Now,
						Description = request.Description,
						Date = request.Date,
						PriceTierId = stay.Room.PriceTierId,
						PGId = stay.PGId,
						StayId = stay.Id
					};

					await _unitOfWork.PGRequest.AddAsync(req);
					_unitOfWork.Save();

					return RedirectToAction(nameof(MyStays));
				}
			}

			return View(request);

		}

		[HttpGet]
		public async Task<IActionResult> MyStays()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var stays = await _unitOfWork.Stay.GetAllAsync(stay => stay.TenantId == claim.Value,
															includeProperties: "Room,Bed,PG,Tenant",
															orderBy: stay => stay.OrderByDescending(s => s.StartDate));

			var activeStay = stays.Where(s => s.IsActive || s.StartDate > DateTime.Now.Date).SingleOrDefault();
			if (activeStay != null)
			{
				var requests = await _unitOfWork.PGRequest.GetAllAsync(req => req.StayId == activeStay.Id
												&& req.RequestType == RequestType.Leave
												&& (req.RequestStatus == RequestStatus.Pending || req.RequestStatus == RequestStatus.Accepted));

				activeStay.LeaveRequestPlaced = requests.Where(req => req.RequestStatus == RequestStatus.Pending).Any();
				activeStay.LeaveRequestAccepted = requests.Where(req => req.RequestStatus == RequestStatus.Accepted).Any();
			}

			return View(stays);
		}
	}
}