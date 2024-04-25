using AspNetCoreHero.ToastNotification.Abstractions;
using AutonetProjectMVCASP.Controllers;
using AutonetProjectMVCASP.Data;
using AutonetProjectMVCASP.Models;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Hangfire;

namespace AutonetProjectMSTEST
{
    [TestClass]
    public class AppointmentsControllerTests
    {
        

        private AppointmentsController _controller;
        private Mock<ApplicationDbContext> _mockDb;
        private Mock<ILogger<AppointmentsController>> _mockLogger;
        private Mock<INotyfService> _mockToastNotification;
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;
        private Mock<IConfiguration> _mockConfiguration;

        [TestInitialize]
        public void Setup()
        {


            _mockDb = new Mock<ApplicationDbContext>();

            var specificDate = new DateTime(2024, 4, 20);

            var startTime = specificDate.AddHours(10);
            var endTime = specificDate.AddHours(16);

            var locations = new List<Locations>
            {
                new Locations { Place = "Location1", Title = "tmp1", StartTime=startTime, EndTime=endTime },
                new Locations { Place = "Location2", Title = "tmp2", StartTime=startTime, EndTime=endTime}
            }.AsQueryable();

            var mockSetLoc = new Mock<DbSet<Locations>>();
            mockSetLoc.As<IQueryable<Locations>>().Setup(m => m.Provider).Returns(locations.Provider);
            mockSetLoc.As<IQueryable<Locations>>().Setup(m => m.Expression).Returns(locations.Expression);
            mockSetLoc.As<IQueryable<Locations>>().Setup(m => m.ElementType).Returns(locations.ElementType);
            mockSetLoc.As<IQueryable<Locations>>().Setup(m => m.GetEnumerator()).Returns(() => locations.GetEnumerator());

            // Setup Find method for Locations DbSet
            mockSetLoc.Setup(m => m.Find(It.IsAny<object[]>()))
                .Returns((object[] ids) =>
                {
                    var id = (string)ids[0];
                    return locations.FirstOrDefault(l => l.Place == id);
                });

            _mockDb.Setup(db => db.Locations).Returns(mockSetLoc.Object);

            var location = "Location1";
            var appointments = new List<Appointments>
            {
                new Appointments { Id = 1, Location = location, Name = "ex1", Time = DateTime.Today },
                new Appointments { Id = 2, Location = location, Name = "ex2", Time = DateTime.Today }
            }.AsQueryable();

            var mockSetAp = new Mock<DbSet<Appointments>>();
            mockSetAp.As<IQueryable<Appointments>>().Setup(m => m.Provider).Returns(appointments.Provider);
            mockSetAp.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointments.Expression);
            mockSetAp.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointments.ElementType);
            mockSetAp.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(() => appointments.GetEnumerator());

            _mockDb.Setup(db => db.Appointments).Returns(mockSetAp.Object);
            _mockDb.Setup(db => db.Appointments.Find(It.IsAny<object[]>()))
                   .Returns((object[] ids) =>
                   {
                       var id = (int)ids[0];
                       return appointments.FirstOrDefault(a => a.Id == id);
                   });

            var employees = new List<Employees>
            {
                new Employees { Id = 1, Name = "John", Surname = "Doe" },
                new Employees { Id = 2, Name = "Jane", Surname = "Doe" }
            }.AsQueryable();

            var mockSetEmp = new Mock<DbSet<Employees>>();
            mockSetEmp.As<IQueryable<Employees>>().Setup(m => m.Provider).Returns(employees.Provider);
            mockSetEmp.As<IQueryable<Employees>>().Setup(m => m.Expression).Returns(employees.Expression);
            mockSetEmp.As<IQueryable<Employees>>().Setup(m => m.ElementType).Returns(employees.ElementType);
            mockSetEmp.As<IQueryable<Employees>>().Setup(m => m.GetEnumerator()).Returns(() => employees.GetEnumerator());

            _mockDb.Setup(db => db.Employees).Returns(mockSetEmp.Object);


            // Sample Users data
            var users = new List<IdentityUser>
            {
                new IdentityUser { Id = "1", UserName = "user1" },
                new IdentityUser { Id = "2", UserName = "user2" }
            }.AsQueryable();

            // Mock DbSet for Users
            var mockUserSet = new Mock<DbSet<IdentityUser>>();
            mockUserSet.As<IQueryable<IdentityUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<IdentityUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<IdentityUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<IdentityUser>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

            _mockDb.Setup(db => db.Users).Returns(mockUserSet.Object);


            _mockLogger = new Mock<ILogger<AppointmentsController>>();
            _mockToastNotification = new Mock<INotyfService>();

            _mockHostingEnvironment = new Mock<IWebHostEnvironment>();
            _mockHostingEnvironment.Setup(m => m.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

            string _testpath = Path.Combine(Directory.GetCurrentDirectory());
            _testpath = Directory.GetParent(_testpath).FullName;
            _testpath = Directory.GetParent(_testpath).FullName;
            _testpath = Directory.GetParent(_testpath).FullName;
            _testpath = Path.Combine(_testpath, "wwwroot");

            _mockHostingEnvironment.Setup(m => m.WebRootPath).Returns(_testpath);



            // Setup IConfiguration mock with default values for SmtpSettings
            var mockConfigurationSection = new Mock<IConfigurationSection>();
            mockConfigurationSection.Setup(a => a["Server"]).Returns("smtp.gmail.com");
            mockConfigurationSection.Setup(a => a["Port"]).Returns("465");

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(a => a.GetSection("SmtpSettings")).Returns(mockConfigurationSection.Object);

            //ConfigureHangfire();

            _controller = new AppointmentsController(
                _mockDb.Object,
                _mockLogger.Object,
                _mockToastNotification.Object,
                _mockConfiguration.Object);
        }

        [TestMethod]
        public void Select_ReturnsViewResult_WithListOfLocations()
        {
            

            // Act
            var result = _controller.Select() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Locations>));
            var model = result.Model as IEnumerable<Locations>;
            Assert.AreEqual(2, model.Count());
        }


        // Example for Index action
        [TestMethod]
        public void Index_ReturnsViewResult_WithAppointments()
        {
            //Arrange
            var location = "Location1";


            // Act
            var result = _controller.Index(location) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Appointments>));
            var model = result.Model as IEnumerable<Appointments>;
            Assert.AreEqual(2, model.Count());
        }

        [TestMethod]
        public void Create_ReturnsViewResult_WithNoParameters()
        {
           
            

            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreatePost_WithValidModel_RedirectsToSelectAction()
        {
            
            var time = DateTime.Now.AddDays(1);
            while (time.Hour < 10 || time.Hour > 16)
            {
                time = time.AddHours(1);
            }

            while (time.Date.DayOfWeek == DayOfWeek.Saturday || time.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                time = time.AddDays(1);
            }

            var validAppointment = new Appointments
            {
                Id = 3,
                Location = "Location1",
                Time = time,

            };



            // Act
            var result = _controller.CreateNoHangfire(validAppointment) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Select", result.ActionName);
        }

        [TestMethod]
        public void CreatePost_WithInvalidModel_ReturnsViewResult()
        {
            // Arrange
            
            _controller.ModelState.AddModelError("Time", "The date and time must be in the future");
            var invalidAppointment = new Appointments
            {
                Id = 3,
                Location = "Location1",
                Time = DateTime.Now.AddDays(-1),  // invalid time
                                               
            };

            // Act
            var result = _controller.Create(invalidAppointment) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);

            _controller.ModelState.Clear();
        }

        //[TestMethod]
        //public void CreateWithData_WithValidData_ReturnsViewResult()
        //{
            

        //    var validData = new LocDateData
        //    {
        //        Location = "Location1",
        //        Date = DateTime.Now.AddDays(1)
        //    };

        //    // Act
        //    var result = _controller.CreateWithDataTesting(validData) as ViewResult;

            

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("", result.ViewName); // Update this with the actual view name if known
        //}

        [TestMethod]
        public void Remove_WithValidId_ReturnsViewResult()
        {
            
            //Arrange
            int validId = 1; // Assuming 1 is a valid ID
            _mockDb.Setup(db => db.Appointments.Find(It.IsAny<object[]>()))
                .Returns((object[] ids) =>
                {
                    var id = (int)ids[0];
                    return new Appointments
                    {
                        Id = id,
                        EmployeeId = 1 // Replace with a valid EmployeeId for testing purposes
                        
                    };
                });


            // Act
            var result = _controller.Remove(validId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            //Assert.AreEqual("", result.ViewName); // Update this with the actual view name if known
        }

        [TestMethod]
        public void GeneratePdf_ReturnsFileResult()
        {
            //Arrange
            //_mockDb.Setup(_mockDb => _mockDb.Appointments).Returns(new Mock<DbSet<Appointments>>().Object);
            

            // Act
            var result = _controller.GeneratePdf();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            //Assert.AreEqual("application/pdf", result.ContentType);
        }

        [TestMethod]
        public void GenerateExcel_ReturnsFileResult()
        {
            

            // Act
            var result = _controller.GenerateExcel();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            //Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
        }


        

    }
}
