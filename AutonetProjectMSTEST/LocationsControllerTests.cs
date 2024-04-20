using AspNetCoreHero.ToastNotification.Abstractions;
using AutonetProjectMVCASP.Controllers;
using AutonetProjectMVCASP.Data;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutonetProjectMSTEST;

namespace AutonetProjectMSTEST
{
    

    [TestClass]
    public class LocationsControllerTests
    {
        private LocationsController _controller;
        private Mock<ApplicationDbContext> _mockDb;
        private Mock<ILogger<LocationsController>> _mockLogger;
        private Mock<INotyfService> _mockToastNotification;
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;

        [TestInitialize]
        public void Setup()
        {
            _mockDb = new Mock<ApplicationDbContext>();

            var locations = new List<Locations>
            {
                new Locations { Place = "Location1", Title = "tmp1" },
                new Locations { Place = "Location2", Title = "tmp2" }
            }.AsQueryable();

            var locemp = new List<LocationEmployee>
            {
                new LocationEmployee { LocationPlace = "Location1", Employee = new Employees { Name = "John", Surname = "Doe" } }
            }.AsQueryable();

            var mockSetLoc = new Mock<DbSet<Locations>>();
            mockSetLoc.As<IQueryable<Locations>>().Setup(m => m.Provider).Returns(locations.Provider);
            mockSetLoc.As<IQueryable<Locations>>().Setup(m => m.Expression).Returns(locations.Expression);
            mockSetLoc.As<IQueryable<Locations>>().Setup(m => m.ElementType).Returns(locations.ElementType);
            mockSetLoc.As<IQueryable<Locations>>().Setup(m => m.GetEnumerator()).Returns(() => locations.GetEnumerator());

            _mockDb.Setup(db => db.Locations).Returns(mockSetLoc.Object);

            _mockLogger = new Mock<ILogger<LocationsController>>();
            _mockToastNotification = new Mock<INotyfService>();

            _controller = new LocationsController(
                _mockDb.Object,
                _mockLogger.Object,
                _mockToastNotification.Object);
        }

        [TestMethod]
        public void Index_ReturnsViewResult_WithListOfLocations()
        {
            

            

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Locations>));
            Assert.AreEqual(2, ((IEnumerable<Locations>)result.Model).Count());
        }

        //[TestMethod]
        //public void Details_WithValidPlace_ReturnsViewResult_WithLocationAndEmployees()
        //{
        //    // Arrange
        //    string place = "Location1";
        //    var location = new Locations { Place = place, Title = "tmp1" };
        //    var employees = new List<LocationEmployee>
        //    {
        //        new LocationEmployee { LocationPlace = place, Employee = new Employees { Name = "John", Surname = "Doe" } }
        //    };

        //    _mockDb.Setup(db => db.Locations.Find(place)).Returns(location);
        //    //_mockDb.Setup(db => db.LocationEmployees).Returns(employees.AsQueryable());

        //    // Act
        //    var result = _controller.Details(place) as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(location, result.Model);
        //    //Assert.IsNotNull(result.ViewBag.LocationEmployees);
        //}

        [TestMethod]
        public void Create_ReturnsViewResult_WithEmployeesInViewBag()
        {
            // Arrange
            var employees = new List<Employees>
            {
                new Employees { Id = 1, Name = "John", Surname = "Doe" },
                new Employees { Id = 2, Name = "Jane", Surname = "Doe" }
            };

            //_mockDb.Setup(db => db.Employees).Returns(employees.AsQueryable());

            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsNotNull(result.ViewBag.Employees);
            //Assert.AreEqual(employees.Count, ((IEnumerable<Employees>)result.ViewBag.Employees).Count());
        }


    }


}
