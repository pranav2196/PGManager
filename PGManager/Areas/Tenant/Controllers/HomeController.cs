using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PGManager.Models.ViewModels;
using PGManager.Utilities;

namespace PGManager.Controllers
{
	[Area("Tenant")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly SignInManager<IdentityUser> _signInManager;

		public HomeController(ILogger<HomeController> logger,
			SignInManager<IdentityUser> signInManager)
		{
			_logger = logger;
			_signInManager = signInManager;

		}

		public IActionResult Index()
		{
			if (_signInManager.IsSignedIn(User))
			{
				if (User.IsInRole(SD.Role_Admin))
					return RedirectToAction("Index", "User", new { Area = "Admin" });
				else if (User.IsInRole(SD.Role_User_Owner))
					return RedirectToAction("Index", "PG", new { Area = "Owner" });
				else
					return RedirectToAction("Index", "PG", new { Area = "Tenant" });
			}

			return RedirectToPage("/Account/Login", new { Area = "Identity" });
		}
	}
}
