﻿using AutonetProjectMVCASP.Data;
//using AutonetProjectMVCASP.Migrations;
using Microsoft.AspNetCore.Mvc;
using AutonetProjectMVCASP.Models;
using Microsoft.Extensions.Logging;
using NToastNotify;
using AspNetCoreHero.ToastNotification.Abstractions;


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

            return View(obj);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.Locations obj)
        {
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
                _toastNotification.Success("Creation Successful!", 3);
                return RedirectToAction("Index");
            }



            return View(obj);
        }

    }
}
