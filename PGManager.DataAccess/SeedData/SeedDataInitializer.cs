using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using PGManager.DataAccess.Data;
using PGManager.Models;
using PGManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManager.DataAccess.SeedData
{
	public class SeedDataInitializer
	{
		public async static Task SeedData(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signinManager, RoleManager<IdentityRole> roleManager)
		{
			if (roleManager.Roles.Count() == 0)
			{
				if (!await roleManager.RoleExistsAsync(SD.Role_Admin))
				{
					await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
				}
				if (!await roleManager.RoleExistsAsync(SD.Role_User_Customer))
				{
					await roleManager.CreateAsync(new IdentityRole(SD.Role_User_Customer));
				}
				if (!await roleManager.RoleExistsAsync(SD.Role_User_Owner))
				{
					await roleManager.CreateAsync(new IdentityRole(SD.Role_User_Owner));
				}

				if (!db.ApplicationUsers.Any())
				{
					var userData = System.IO.File.ReadAllText("SeedData/AdminUsers.json");

					var users = JsonConvert.DeserializeObject<List<ApplicationUser>>(userData);
					foreach (var user in users)
					{
						var userToAdd = new ApplicationUser
						{
							UserName = user.Email,
							Email = user.Email,
							StreetAddress = user.StreetAddress,
							City = user.City,
							State = user.State,
							PostalCode = user.PostalCode,
							Name = user.Name,
							PhoneNumber = user.PhoneNumber,
							Gender = user.Gender
						};
						var result = await userManager.CreateAsync(userToAdd, user.Pwd);
						if (result.Succeeded)
							await userManager.AddToRoleAsync(userToAdd, SD.Role_Admin);
					}
				}

				var cityData = System.IO.File.ReadAllText("SeedData/Cities.json");
				var cities = JsonConvert.DeserializeObject<List<City>>(cityData);
				foreach(var city in cities)
				{
					await db.Cities.AddAsync(city);
				}

				var facilityData = System.IO.File.ReadAllText("SeedData/Facilities.json");
				var facilities = JsonConvert.DeserializeObject<List<Facility>>(facilityData);
				foreach (var facility in facilities)
				{
					await db.Facilities.AddAsync(facility);
				}

				await db.SaveChangesAsync();
			}
		}
	}
}
