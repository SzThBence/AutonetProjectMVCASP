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


        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger, INotyfService toastNotification, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
            _userManager = userManager;
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

        /*[HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }*/

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}