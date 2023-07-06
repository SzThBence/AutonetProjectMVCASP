using Microsoft.AspNetCore.Mvc;

namespace AutonetProjectMVCASP.Controllers
{
    public class LocationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Locations()
        {
            return View();
        }
    }
}
