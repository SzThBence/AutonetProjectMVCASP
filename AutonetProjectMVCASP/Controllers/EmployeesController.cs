using AutonetProjectMVCASP.Data;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;
using NToastNotify;
using Microsoft.Extensions.Logging;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace AutonetProjectMVCASP.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EmployeesController> _logger;
        private readonly INotyfService _toastNotification;


        public EmployeesController(ApplicationDbContext db, ILogger<EmployeesController> logger, INotyfService toastNotification)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
        }
        [HttpGet]
        public IActionResult Index()
        {
            bool LoggedIn = (User != null) && (User.Identity.IsAuthenticated);
            if (!LoggedIn)
            {
                _toastNotification.Information("You need to be logged in to make changes to this page", 5);
            }

            IEnumerable<Models.Employees> obj = _db.Employees;
            return View(obj);
        }

        
        [HttpGet]
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
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [HttpGet]
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
                _toastNotification.Success("Edit Successful!", 3);
                return RedirectToAction("Index");
            }

            return View(obj);
        }
        [HttpGet]
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

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(Employees obj)
        {
            _db.Employees.Remove(obj);
            _db.SaveChanges();
            _toastNotification.Success("Removal Successful!", 3);
            return RedirectToAction("Index");
        }

    }
}
