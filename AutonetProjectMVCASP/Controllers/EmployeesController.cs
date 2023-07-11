using AutonetProjectMVCASP.Data;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;
using NToastNotify;
using Microsoft.Extensions.Logging;

namespace AutonetProjectMVCASP.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IToastNotification _toastNotification;


        public EmployeesController(ApplicationDbContext db, ILogger<EmployeesController> logger, IToastNotification toastNotification)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            IEnumerable<Models.Employees> obj = _db.Employees;
            return View(obj);
        }

        public IActionResult Controllers()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employees obj)
        {
            if (obj.Name == null)
            {
                ModelState.AddModelError("Name", "The name field is required");
            }

            if (obj.Surname == null)
            {
                ModelState.AddModelError("Surname", "The surname field is required");
            }



            if (ModelState.IsValid)
            {
                _db.Employees.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Employees.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employees obj)
        {
            if (obj.Name == null)
            {
                ModelState.AddModelError("Name", "The name field is required");
            }

            if (obj.Surname == null)
            {
                ModelState.AddModelError("Surname", "The surname field is required");
            }

            if (ModelState.IsValid)
            {
                _db.Employees.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Remove(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Employees.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(Employees obj)
        {
            _db.Employees.Remove(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
