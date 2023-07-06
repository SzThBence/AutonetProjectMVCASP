using Microsoft.AspNetCore.Mvc;

namespace AutonetProjectMVCASP.Controllers
{
    public class AppointmentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Appointments()
        {
            return View();
        }
    }
}
