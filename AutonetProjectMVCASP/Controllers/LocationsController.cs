using AutonetProjectMVCASP.Data;
//using AutonetProjectMVCASP.Migrations;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;
using Microsoft.Extensions.Logging;
using NToastNotify;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Microsoft.EntityFrameworkCore;


namespace AutonetProjectMVCASP.Controllers
{
    public class LocationsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LocationsController> _logger;
        private readonly INotyfService _toastNotification;


        public LocationsController(ApplicationDbContext db, ILogger<LocationsController> logger, INotyfService toastNotification)
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
            IEnumerable<Models.Locations> obj = _db.Locations;
            return View(obj);
        }


        [HttpGet]
        public IActionResult Details(string? place)
        {
            if (place == null)
            {
                return NotFound();
            }

            var obj = _db.Locations.Find(place);

            if (obj == null)
            {
                return NotFound();
            }

            var locationEmployees = _db.LocationEmployees
                                    .Where(le => le.LocationPlace == place)
                                    .Select(le => new
                                    {
                                        // Select only the properties you need
                                        Name = le.Employee.Name,
                                        Surname = le.Employee.Surname
                                    })
                                    .ToList();

            ViewBag.LocationEmployees = locationEmployees;

            return View(obj);
        }
        [HttpGet]
        public IActionResult Create()
        {
            // Retrieve the list of Employees from the database
            var employees = _db.Employees?.ToList();

            // Ensure ViewBag.Employees is initialized
            ViewBag.Employees = employees ?? new List<Employees>();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.Locations obj, List<int> employeeIds)
        {
            obj.StartTime = obj.StartTime.Date.AddHours(obj.StartTime.Hour);
            obj.EndTime = obj.EndTime.Date.AddHours(obj.EndTime.Hour);

            obj.LocationEmployees = new List<LocationEmployee>();
            if (obj.Equals(null))
            {
                return NotFound();
            }
            if (Math.Abs(obj.Latitude) > 90)
            {
                ModelState.AddModelError("Latitude", "The latitude must be valid");
            }
            if (Math.Abs(obj.Longitude) > 180)
            {
                ModelState.AddModelError("Longitude", "The longitude must be valid");
            }




            if (ModelState.IsValid)
            {            
                _db.Locations.Add(obj);
                _db.SaveChanges();
                // Add the selected Employees to the Location
                if (employeeIds != null && employeeIds.Any())
                {
                    foreach (int employeeId in employeeIds)
                    {
                        var locationEmployee = new LocationEmployee
                        {
                            LocationPlace = obj.Place, // Assuming Place is the primary key of Locations
                            EmployeeId = employeeId
                        };
                        //obj.LocationEmployees.Add(locationEmployee);
                        _db.LocationEmployees.Add(locationEmployee);
                        
                    }
                    _db.SaveChanges();
                }
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Index");
            }



            return View(obj);
        }
        [HttpGet]
        public IActionResult BigMap()
        {
            IEnumerable<Models.Locations> obj = _db.Locations;
            return View(obj);
        }

        [HttpGet]
        public IActionResult Edit(string? id)
        {

            if (id == null || id == "")
            {
                return NotFound();
            }

            var obj = _db.Locations.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            // Retrieve the list of Employees from the database
            var employees = _db.Employees?.ToList();

            // Ensure ViewBag.Employees is initialized
            ViewBag.Employees = employees ?? new List<Employees>();


            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Locations obj, List<int> employeeIds)
        {
            obj.StartTime = obj.StartTime.Date.AddHours(obj.StartTime.Hour);
            obj.EndTime = obj.EndTime.Date.AddHours(obj.EndTime.Hour);

            if (obj == null)
            {
                return NotFound();
            }

            if (Math.Abs(obj.Latitude) > 90)
            {
                ModelState.AddModelError("Latitude", "The latitude must be valid");
            }

            if (Math.Abs(obj.Longitude) > 180)
            {
                ModelState.AddModelError("Longitude", "The longitude must be valid");
            }

            if (ModelState.IsValid)
            {
                // Update the existing Location object
                _db.Entry(obj).State = EntityState.Modified;

                // Remove existing LocationEmployees for the Location
                var existingLocationEmployees = _db.LocationEmployees.Where(le => le.LocationPlace == obj.Place).ToList();
                _db.LocationEmployees.RemoveRange(existingLocationEmployees); //first make the menu option for it

                // Add the selected Employees to the Location
                if (employeeIds != null && employeeIds.Any())
                {
                    foreach (int employeeId in employeeIds)
                    {
                        var locationEmployee = new LocationEmployee
                        {
                            LocationPlace = obj.Place,
                            EmployeeId = employeeId
                        };
                        _db.LocationEmployees.Add(locationEmployee);
                    }
                }

                _db.SaveChanges();
                _toastNotification.Success("Editing Successful!", 3);
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        [HttpGet]
        public IActionResult Remove(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.Locations.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(Locations obj)
        {
            _db.Locations.Remove(obj);
            _db.SaveChanges();
            _toastNotification.Success("Removal Successful!", 3);
            return RedirectToAction("Index");
        }

    }
}
