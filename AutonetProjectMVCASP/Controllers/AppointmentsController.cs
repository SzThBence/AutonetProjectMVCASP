using AutonetProjectMVCASP.Data;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using AspNetCoreHero.ToastNotification.Abstractions;


namespace AutonetProjectMVCASP.Controllers
{
    public class AppointmentsData
    {
        public string Location { get; set; }
        public IEnumerable<Appointments> Obj { get; set; }
        public AppointmentsData(string location, IEnumerable<Appointments> obj)
        {
            Location = location;
            Obj = obj;
        }

        public AppointmentsData(string location, ApplicationDbContext db)
        {
            Location = location;
            Obj = db.Appointments.Where(a => a.Location == location).ToList();
        }
    }

    public class LocDateData
    {
        public string Location { get; set; } = "default";
        public DateTime Date { get; set; } = DateTime.Now;

        public LocDateData(string location, DateTime date)
        {
            Location = location;
            Date = date;
        }

        public LocDateData()
        {

        }
    }



    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly INotyfService _toastNotification;



        public AppointmentsController(ApplicationDbContext db, ILogger<AppointmentsController> logger, INotyfService toastNotification)
        {
            _db = db;
            _logger = logger;
            _toastNotification = toastNotification;

        }



        [HttpGet]
        public IActionResult Select()
        {
            IEnumerable<Models.Locations> loc = _db.Locations;
            return View(loc);
        }
        [HttpGet]
        public IActionResult Index(string location)
        {
            bool LoggedIn = (User != null) && (User.Identity.IsAuthenticated);
            if (!LoggedIn)
            {
                _toastNotification.Information("You need to be logged in to make an appointment", 5);
            }

            if ((location == null))
            {
                ModelState.AddModelError("Location", "The location must be a valid location");
            }

            ViewData["Location"] = location;
            ViewData["ActualLocation"] = _db?.Locations.Find(location);


            AppointmentsData obj = new AppointmentsData(location, _db);
            IEnumerable<Models.Appointments> loc = obj.Obj;
            return View(loc);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateWithData(LocDateData info)
        {
            //ViewBag.DateData = date;
            var model = new Appointments
            {
                Location = info.Location,
                Time = info.Date
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointments obj)
        {
            IEnumerable<Models.Locations> loc = _db.Locations;
            if (obj.Time <= DateTime.Now)
            {
                ModelState.AddModelError("Time", "The date and time must be in the future");
            }

            if (obj.Time.DayOfWeek == DayOfWeek.Saturday || obj.Time.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("Time", "The date and time must be a weekday");
            }

            if (obj.Time.Hour < _db.Locations.Find(obj.Location).StaryTime.Hour || obj.Time.Hour > _db.Locations.Find(obj.Location).EndTime.Hour)
            {
                ModelState.AddModelError("Time", "The date and time must be between 9am and 5pm");
            }

            bool isLocation = false;
            foreach (var item in loc)
            {
                if (item.Place == obj.Location)
                {
                    isLocation = true;
                }
            }

            if (!isLocation)
            {
                ModelState.AddModelError("Location", "The location must be a valid location");
            }


            if (ModelState.IsValid)
            {
                _db.Appointments.Add(obj);
                _db.SaveChanges();
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Select");
            }



            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateWithData(Appointments obj)
        {
            if (ModelState.IsValid)
            {
                _db.Appointments.Add(obj);
                _db.SaveChanges();
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Index", new RouteValueDictionary { { "location", obj.Location } });
            }

            TempData["success"] = "Task completed!";
            return View(obj);
        }

        [HttpGet]
        public IActionResult Remove(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Appointments.Find(id);

            if (obj == null)
            {
                return NotFound();
            }



            //_db.Appointments.Remove(obj);
            //_db.SaveChanges();

            //TempData["success"] = "Task completed!";



            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(Appointments obj)
        {

            string loc = obj.Location;

            _db.Appointments.Remove(obj);
            _db.SaveChanges();

            _toastNotification.Success("Removal Successful!", 3);

            return RedirectToAction("Index", new RouteValueDictionary { { "location", obj.Location } });




        }

        [HttpGet]
        public IActionResult Person()
        {
            IEnumerable<Models.Appointments> app = _db.Appointments;

            if (User.Identity.IsAuthenticated)
            {
                return View(app);
            }
            else
            {
                _toastNotification.Error("Unexpected Login Error", 3);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
