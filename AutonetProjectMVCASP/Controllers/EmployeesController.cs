using Microsoft.AspNetCore.Mvc;

namespace AutonetProjectMVCASP.Controllers
{
    public class EmployeesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Controllers()
        {
            return View();
        }
    }
}
