using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using PGManager.DataAccess.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using PGManager.Utility;
using PGManager.DataAccess.Repository.IRepository;
using PGManager.DataAccess.Repository;
using PGManager.Utilities;

namespace PGManager
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			//SQLite
			//services.AddDbContext<ApplicationDbContext>(options =>
			//	options.UseSqlite(
			//		Configuration.GetConnectionString("DefaultConnection")));

			//MS SQL Server
			//services.AddDbContext<ApplicationDbContext>(options =>
			//	options.UseSqlServer(
			//		Configuration.GetConnectionString("DefaultConnection")));

			//PostGreSQL
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

				string connStr;
				// Depending on if in development or production, use either Heroku-provided
				// connection string, or development connection string from env var.
				if (env == "Development")
				{
					// Use connection string from file.
					connStr = Configuration.GetConnectionString("DefaultConnection");
				}
				else
				{
					// Use connection string provided at runtime by Heroku.
					var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

					// Parse connection URL to connection string for Npgsql
					connUrl = connUrl.Replace("postgres://", string.Empty);
					var pgUserPass = connUrl.Split("@")[0];
					var pgHostPortDb = connUrl.Split("@")[1];
					var pgHostPort = pgHostPortDb.Split("/")[0];
					var pgDb = pgHostPortDb.Split("/")[1];
					var pgUser = pgUserPass.Split(":")[0];
					var pgPass = pgUserPass.Split(":")[1];
					var pgHost = pgHostPort.Split(":")[0];
					var pgPort = pgHostPort.Split(":")[1];

					connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}";
				}

				options.UseNpgsql(connStr);
			});

			services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			services.AddSingleton<IEmailSender, EmailSender>();
			services.Configure<EmailOptions>(Configuration);
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddControllersWithViews();
			services.AddRazorPages();
			services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

			var googleAuth = Configuration.GetSection("GoogleOAuth").Get<GoogleAuth>();
			var facebookAuth = Configuration.GetSection("FacebookOAuth").Get<FacebookAuth>();

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = $"/Identity/Account/Login";
				options.LogoutPath = $"/Identity/Account/Logout";
				options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
			});
			services.AddAuthentication()
				.AddFacebook(options =>
			{
				options.AppId = facebookAuth.AppId;
				options.AppSecret = facebookAuth.AppSecret;
			})
				.AddGoogle(options =>
			{
				options.ClientId = googleAuth.ClientId;
				options.ClientSecret = googleAuth.ClientSecret;
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();
			app.UseFileServer(enableDirectoryBrowsing: false);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{area=Tenant}/{controller=Home}/{action=Index}/{id?}");
				endpoints.MapRazorPages();
			});
		}
	}
}
