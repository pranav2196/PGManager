using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PGManager.DataAccess.Data;

namespace PGManager.Areas.Identity.Pages.Account.Manage
{
	public partial class IndexModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly ApplicationDbContext _db;

		public IndexModel(
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			ApplicationDbContext db)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_db = db;
		}

		public string Username { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			public string Name { get; set; }
			[Required]
			[Display(Name = "Street Address")]
			public string StreetAddress { get; set; }
			[Required]
			public string City { get; set; }
			[Required]
			public string State { get; set; }
			[Required]
			[Display(Name = "Postal Code")]
			public string PostalCode { get; set; }
			[Required]
			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }
		}

		private async Task LoadAsync(IdentityUser user)
		{
			var appUser = await _db.ApplicationUsers.FindAsync(user.Id);

			Username = appUser.Email;

			Input = new InputModel
			{
				PhoneNumber = appUser.PhoneNumber,
				Name = appUser.Name,
				StreetAddress = appUser.StreetAddress,
				City = appUser.City,
				State = appUser.State,
				PostalCode = appUser.PostalCode
			};
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var user = await _db.ApplicationUsers.FindAsync(claim.Value);

			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			user.PhoneNumber = Input.PhoneNumber;
			user.Name = Input.Name;
			user.StreetAddress = Input.StreetAddress;
			user.City = Input.City;
			user.State = Input.State;
			user.PostalCode = Input.PostalCode;

			await _db.SaveChangesAsync();

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}
	}
}
