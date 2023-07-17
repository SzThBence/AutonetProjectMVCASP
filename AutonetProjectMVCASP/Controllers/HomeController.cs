using AspNetCoreHero.ToastNotification.Abstractions;
using AutonetProjectMVCASP.Data;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Diagnostics;

namespace AutonetProjectMVCASP.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _toastNotification;


        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger, INotyfService toastNotification)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}