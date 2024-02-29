using AutonetProjectMVCASP.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using NToastNotify;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.Extensions.Logging;

namespace AutonetProjectMVCASP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() // Add Role Management
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
            {
                ProgressBar = true,
                Timeout = 5000
            });

            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 5;
                config.IsDismissable = true;
                config.Position = NotyfPosition.BottomRight;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            using var scope = app.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles if they don't exist
            var roles = new List<string> { "Admin", "User" };
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


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseNToastNotify();
            app.UseNotyf();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }

}