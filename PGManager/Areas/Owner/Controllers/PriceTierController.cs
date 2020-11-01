using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Utilities;

namespace PGManager.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = SD.Role_User_Owner)]
    public class PriceTierController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PriceTierController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            PG pg;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            pg = await _unitOfWork.PG.GetAsync(claim.Value, IncludePriceTiers: true);

            return View(pg.PriceTiers);
        }

        [HttpGet]
        public async Task<ActionResult> Upsert(int id)
        {
            PriceTier priceTier;
            if (id == 0)
                priceTier = new PriceTier();
            else
                priceTier = await _unitOfWork.PriceTier.GetAsync(id);

            return View(priceTier);
        }

        [HttpPost]
        public async Task<ActionResult> Upsert(PriceTier priceTier)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                int? pgId = (await _unitOfWork.PG.GetAsync(claim.Value, false, false, false))?.Id;
                if (pgId != null)
                {
                    if (priceTier.Id == 0)
                    {
                        priceTier.PGId = (int)pgId;
                        await _unitOfWork.PriceTier.AddAsync(priceTier);
                    }
                    else
                    {
                        await _unitOfWork.PriceTier.UpdateAsync(priceTier);
                    }
                    _unitOfWork.Save();
                }
            }
            else
            {
                return View(priceTier);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            PG pg = await _unitOfWork.PG.GetAsync(claim.Value, IncludePriceTiers:true);

            if (pg.PriceTiers != null)
            {
                var priceTier = pg.PriceTiers.Where(pt => pt.Id == id).SingleOrDefault();
                if (priceTier != null)
                {
                    _unitOfWork.PriceTier.Remove(priceTier);
                    _unitOfWork.Save();
                }
            }

            return true;
        }
    }
}
