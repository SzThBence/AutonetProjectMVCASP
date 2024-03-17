using AutonetProjectMVCASP.Data;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;
using NToastNotify;
using Microsoft.Extensions.Logging;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Hosting;

namespace AutonetProjectMVCASP.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EmployeesController> _logger;
        private readonly INotyfService _toastNotification;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EmployeesController(ApplicationDbContext db,
            ILogger<EmployeesController> logger,
            INotyfService toastNotification,
            IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;
            _hostingEnvironment = hostingEnvironment;
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
        public IActionResult Create(Employees obj, IFormFile imageFile)
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
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Save the uploaded image file to a directory or store it in the database
                    var imagePath = Path.Combine("images", imageFile.FileName);
                    using (var stream = new FileStream(Path.Combine(_hostingEnvironment.WebRootPath, imagePath), FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    obj.ImagePath = imagePath;

                    // Add the new employee to the database
                    _db.Employees.Add(obj);
                    _db.SaveChanges();

                    // Show success notification
                    _toastNotification.Success("Creation Successful!", 3);

                    // Redirect to the Index action
                    return RedirectToAction("Index");
                }
                else
                {
                    // If no image file is uploaded, show an error
                    ModelState.AddModelError("ImageFile", "Please select an image file");
                }
            }

            _toastNotification.Error("Creation Failed!", 3);
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
