using AutonetProjectMVCASP.Data;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;
using NToastNotify;
using Microsoft.Extensions.Logging;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

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
        //Lists all employees in cards
        [HttpGet]
        public IActionResult Index()
        {
            //check if user is logged in
            bool LoggedIn = (User != null) && (User.Identity.IsAuthenticated);
            

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
            //state validation
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
            //base data check
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Employees.Find(id);

            //found data check
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employees updatedEmployee, IFormFile imageFile)
        {
            //check for id corruption
            if (id != updatedEmployee.Id)
            {
                return NotFound();
            }

            if (imageFile != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        //get base employee
                        var existingEmployee = _db.Employees.Find(id);

                        if (existingEmployee == null)
                        {
                            return NotFound();
                        }

                        //update data
                        existingEmployee.Name = updatedEmployee.Name;
                        existingEmployee.Surname = updatedEmployee.Surname;
                        existingEmployee.Job = updatedEmployee.Job;

                        if (imageFile != null && imageFile.Length > 0)
                        {
                            // Save the uploaded image file to a directory or store it in the database, then update employee's image path
                            var imagePath = Path.Combine("images", imageFile.FileName);
                            using (var stream = new FileStream(Path.Combine(_hostingEnvironment.WebRootPath, imagePath), FileMode.Create))
                            {
                                imageFile.CopyTo(stream);
                            }
                            existingEmployee.ImagePath = imagePath;
                        }
                        else
                        {
                            //no image
                            _toastNotification.Error("Please select an image file", 3);
                            ModelState.AddModelError("ImageFile", "Please select an image file");
                        }

                        _db.SaveChanges();
                        _toastNotification.Success("Edit Successful!", 3);
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // Handle concurrency exception if necessary
                        _toastNotification.Error("Concurrency error occurred!", 3);
                        return View(updatedEmployee);
                    }
                }
                else
                {
                    _toastNotification.Error("Edit Failed!", 3);
                    // If ModelState is not valid, return to the edit view with errors
                    return View(updatedEmployee);
                }

                
            }
            _toastNotification.Error("Please select an image file", 3);
            // If ModelState is not valid, return to the edit view with errors
            return View(updatedEmployee);
        }
        [HttpGet]
        public IActionResult Remove(int? id)
        {
            //base check
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Employees.Find(id);

            //found check
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
            _toastNotification.Success("Removal Successful!", 3);
            return RedirectToAction("Index");
        }

    }
}
