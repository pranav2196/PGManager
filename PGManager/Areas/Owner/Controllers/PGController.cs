using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Models.ViewModels;
using PGManager.Utilities;

namespace PGManager.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = SD.Role_User_Owner)]
    public class PGController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PGController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            PG pg;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            pg = await _unitOfWork.PG.GetAsync(claim.Value, true, true, false);

            if (pg == null)
            {
                pg = new PG();
            }

            return View(pg);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert()
        {
            PGEditViewModel pgEdit = new PGEditViewModel();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            pgEdit.PG = await _unitOfWork.PG.GetAsync(claim.Value, true, true, false);


            if (pgEdit.PG == null)
            {
                pgEdit.PG = new PG();
                var facilities = await _unitOfWork.Facility.GetAllAsync();
                pgEdit.Facilities = facilities.ToList();
            }
            else
            {
                var facilities = await _unitOfWork.PG.GetFacilitiesForPG(pgEdit.PG.Id);
                pgEdit.Facilities = facilities.ToList();
            }

            IEnumerable<City> cities = await _unitOfWork.City.GetAllAsync();

            ViewBag.CityList = cities.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            return View(pgEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert([FromForm] PGEditViewModel pgVM)
        {
            PGEditViewModel pgEdit = new PGEditViewModel();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            PG pg = pgVM.PG;
            pg.ApplicationUserId = claim.Value;
            if (!pg.IsMale && !pg.IsFemale)
            {
                pg.IsMale = true;
            }

            await _unitOfWork.PG.Update(pg, pgVM.Facilities);

            return RedirectToAction(nameof(Index));
        }
    }
}
