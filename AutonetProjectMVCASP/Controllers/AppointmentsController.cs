using AutonetProjectMVCASP.Data;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;



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

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Select()
        {
            IEnumerable<Models.Locations> loc = _db.Locations;
            return View(loc);
        }

        public IActionResult Index(string location)
        {
            if ((location == null))
            {
                ModelState.AddModelError("Location", "The location must be a valid location");
            }

            ViewData["Location"] = location;


            AppointmentsData obj = new AppointmentsData(location, _db);
            IEnumerable<Models.Appointments> loc = obj.Obj;
            return View(loc);
        }

        public IActionResult Appointments()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

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

            if (obj.Time.Hour < 8 || obj.Time.Hour > 19)
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
                return RedirectToAction("Index");
            }

            TempData["success"] = "Task completed!";

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
                return RedirectToAction("Index", new RouteValueDictionary { { "location", obj.Location } });
            }

            TempData["success"] = "Task completed!";
            return View(obj);
        }


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



            _db.Appointments.Remove(obj);
            _db.SaveChanges();

            TempData["success"] = "Task completed!";

            return RedirectToAction("Index");


        }
    }
}
