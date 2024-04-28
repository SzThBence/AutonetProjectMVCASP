using AutonetProjectMVCASP.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using NToastNotify;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using Hangfire;
//using NToastNotify.MessageContainers;
using AspNetCoreHero.ToastNotification.Containers;
using NToastNotify.MessageContainers;

namespace AutonetProjectMVCASP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));
            //Identity
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() // Add Role Management
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //NToastNotify
            builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
            {
                ProgressBar = true,
                Timeout = 5000
            });

            //Notyf
            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 5;
                config.IsDismissable = true;
                config.Position = NotyfPosition.BottomRight;
            });

            

            //Hangfire
            builder.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddHangfireServer();
            builder.Services.AddControllersWithViews();

            //Start configuration
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //Roles, scope
            using var scope = app.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles if they don't exist
            var roles = new List<string> { "Admin", "User", "Boss", "Architect" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var users = await userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                if (!await userManager.IsInRoleAsync(user, "User"))
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            var adminUser = await userManager.FindByEmailAsync("admin@admin.com");
            if (adminUser != null && !(await userManager.IsInRoleAsync(adminUser, "Admin")))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }


            //Configure services
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //roles and users
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //Notyf
            app.UseNToastNotify();
            app.UseNotyf();

            //Hangfire
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            //Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }

}