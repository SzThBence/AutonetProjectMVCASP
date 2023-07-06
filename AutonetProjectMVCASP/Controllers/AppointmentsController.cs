using AutonetProjectMVCASP.Data;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;

namespace AutonetProjectMVCASP.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Appointments> obj = _db.Appointments;
            return View(obj);
        }

        public IActionResult Appointments()
        {
            return View();
        }
    }
}
