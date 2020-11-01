using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Utilities;

namespace PGManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CitiesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CitiesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var cities = await _unitOfWork.City.GetAllAsync();
            cities = cities.OrderBy(c => c.Name);
            return View(cities);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            City city;
            if (id == null || id == 0)
                city = new City();
            else
                city = await _unitOfWork.City.GetAsync(id.GetValueOrDefault());

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(City city)
        {
            if (ModelState.IsValid)
            {
                if (city.Id == 0)
                    await _unitOfWork.City.AddAsync(city);
                else
                {
                    var cityFromDB = await _unitOfWork.City.GetAsync(city.Id);
                    if (cityFromDB == null)
                        await _unitOfWork.City.AddAsync(city);
                    else
                    {
                        await _unitOfWork.City.Update(city);
                    }
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(city);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _unitOfWork.City.RemoveAsync(id);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
