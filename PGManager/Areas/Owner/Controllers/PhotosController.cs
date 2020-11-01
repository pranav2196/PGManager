using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.Models;
using PGManager.Models.ViewModels;
using PGManager.Utilities;

namespace PGManager.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = SD.Role_User_Owner)]
    public class PhotosController : Controller
    {
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IUnitOfWork _unitOfWork;
        private Cloudinary _cloudinary;

        public PhotosController(IOptions<CloudinarySettings> cloudinaryConfig, IUnitOfWork unitOfWork)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _unitOfWork = unitOfWork;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var pg = await _unitOfWork.PG.GetAsync(claim.Value, false, false, true);

            return View(pg != null ? pg.Photos : null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(IFormFile fileToUpload)
        {
            if (fileToUpload.Length > 0)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                int? pgId = (await _unitOfWork.PG.GetAsync(claim.Value, false, false, false))?.Id;

                if (pgId != null)
                {
                    var uploadResult = new ImageUploadResult();
                    using (var stream = fileToUpload.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(fileToUpload.Name, stream),
                            Transformation = new Transformation().AspectRatio(1, 1).Crop("fill").Gravity("center")
                        };

                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    }
                    Photo photo = new Photo()
                    {
                        DateAdded = DateTime.Now,
                        PGId = (int)pgId,
                        PublicId = uploadResult.PublicId,
                        Url = uploadResult.Url.ToString()
                    };

                    await _unitOfWork.Photo.AddAsync(photo);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var photo = await _unitOfWork.Photo.GetAsync(id);
            var deletionParams = new DeletionParams(photo.PublicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
            if (deletionResult.StatusCode == HttpStatusCode.OK)
            {
                _unitOfWork.Photo.Remove(photo);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
