using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Utilities;

namespace PGManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class FacilitiesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacilitiesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.Facility.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Facility facility;
            if (id == null || id == 0)
                facility = new Facility();
            else
                facility = await _unitOfWork.Facility.GetAsync(id.GetValueOrDefault());

            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Facility facility)
        {
            if (ModelState.IsValid)
            {
                if (facility.Id == 0)
                    await _unitOfWork.Facility.AddAsync(facility);
                else
                {
                    var facilityFromDB = await _unitOfWork.Facility.GetAsync(facility.Id);
                    if (facilityFromDB == null)
                        await _unitOfWork.Facility.AddAsync(facility);
                    else
                    {
                        await _unitOfWork.Facility.Update(facility);
                    }
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(facility);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _unitOfWork.Facility.RemoveAsync(id);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
