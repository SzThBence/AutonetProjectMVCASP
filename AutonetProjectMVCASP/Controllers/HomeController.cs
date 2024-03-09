using AspNetCoreHero.ToastNotification.Abstractions;
using AutonetProjectMVCASP.Data;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AutonetProjectMVCASP.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _toastNotification;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger, INotyfService toastNotification, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Privacy()
        {
            IEnumerable<Models.Locations> loc = _db.Locations;
            return View(loc);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> AddRolesAndUsers()
        {
            // Create roles if they don't exist
            var roles = new List<string> { "Admin", "User", "Boss", "Architect" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Assign roles to users
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }

            var adminUser = await _userManager.FindByEmailAsync("admin@admin.com");
            if (adminUser != null && !(await _userManager.IsInRoleAsync(adminUser, "Admin")))
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            return RedirectToAction("Index"); // Redirect to a specific action after completing the role assignment
        }
    }
}
