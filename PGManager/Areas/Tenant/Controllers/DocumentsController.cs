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
using PGManager.Utilities;

namespace PGManager.Areas.Tenant.Controllers
{
    [Area("Tenant")]
    [Authorize(Roles = SD.Role_User_Customer)]
    public class DocumentsController : Controller
    {
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IUnitOfWork _unitOfWork;
        private Cloudinary _cloudinary;

        public DocumentsController(IOptions<CloudinarySettings> cloudinaryConfig, IUnitOfWork unitOfWork)
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

            var documents = await _unitOfWork.UserDocument.GetAllAsync(ud => ud.ApplicationUserId == claim.Value);

            return View(documents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(IFormFile fileToUpload)
        {
            if (fileToUpload.Length > 0)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                var uploadResult = new ImageUploadResult();
                using (var stream = fileToUpload.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(fileToUpload.Name, stream)
                    };

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
                UserDocument document = new UserDocument()
                {
                    DateAdded = DateTime.Now,
                    ApplicationUserId = claim.Value,
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url.ToString()
                };

                await _unitOfWork.UserDocument.AddAsync(document);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var document = await _unitOfWork.UserDocument.GetAsync(id);
            var deletionParams = new DeletionParams(document.PublicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
            if (deletionResult.StatusCode == HttpStatusCode.OK)
            {
                _unitOfWork.UserDocument.Remove(document);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
