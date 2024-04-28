using AspNetCoreHero.ToastNotification.Abstractions;
using AutonetProjectMVCASP.Data;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


namespace AutonetProjectMVCASP.Controllers
{
    public class UsersAndRolesManagers
    {
        public UserManager<IdentityUser> _userManager;
        public RoleManager<IdentityRole> _roleManager;


        public UsersAndRolesManagers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
    }

    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _toastNotification;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;


        public HomeController(ApplicationDbContext db,
            ILogger<HomeController> logger,
            INotyfService toastNotification,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await AddRolesAndUsers();

            //// Sign out the current user
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //// Authenticate the user again (example: using a predefined user)
            //var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            //{
            //    new Claim(ClaimTypes.Name, "username"),
            //    // Add other claims as needed
            //}, CookieAuthenticationDefaults.AuthenticationScheme));

            //// Sign in the user again
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

            // Signing out, so it can enter with the new roles if needed
            await HttpContext.SignOutAsync();
            if (_signInManager.IsSignedIn(User))
            {
                // Signing in again
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var claims = await _userManager.GetClaimsAsync(user); // You may need to retrieve claims if necessary

                // Create a ClaimsIdentity for the user
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create a ClaimsPrincipal using the ClaimsIdentity
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in the user
                await HttpContext.SignInAsync(claimsPrincipal);
            }
            

            return View();
        }
        //Repurposed privacy page for locations, now it show base data with marketing pictures for locations
        [HttpGet]
        public IActionResult Privacy()
        {
            IEnumerable<Models.Locations> loc = _db.Locations;
            return View(loc);
        }
        //Role management page, lists all users and their roles, can change roles
        [HttpGet]
        public IActionResult Users()
        {
            UsersAndRolesManagers usersAndRoles = new UsersAndRolesManagers(_userManager, _roleManager);
            return View(usersAndRoles);
        }
        //Role changer, changes roles for users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleChanger(Dictionary<string, List<string>> roles)
        {
            if (roles != null)
            {
                foreach (var userId in roles.Keys)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        // Clear existing roles for the user
                        var userRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, userRoles);

                        // Add selected roles for the user
                        foreach (var roleName in roles[userId])
                        {
                            if (await _roleManager.RoleExistsAsync(roleName))
                            {
                                await _userManager.AddToRoleAsync(user, roleName);
                            }
                        }
                    }
                    await _userManager.UpdateAsync(user);
                }
            }
            _toastNotification.Success("Roles updated successfully",3);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Helper for adding roles and users, run every time we get to index, so new employees will have roles
        [HttpGet]
        public async Task<Boolean> AddRolesAndUsers()
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

            return true;
        }
    }
}
