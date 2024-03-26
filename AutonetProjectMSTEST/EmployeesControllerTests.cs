﻿using AspNetCoreHero.ToastNotification.Abstractions;
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

namespace AutonetProjectMVCASP.Tests
{
    


    [TestClass]
    public class EmployeesControllerTests
    {
        private EmployeesController _controller;
        private Mock<ApplicationDbContext> _mockDb;
        private Mock<ILogger<EmployeesController>> _mockLogger;
        private Mock<INotyfService> _mockToastNotification;
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;

        [TestInitialize]
        public void Setup()
        {
            // Create mock data
            var employees = new List<Employees>
            {
                new Employees { Id = 1, Name = "John", Surname = "Doe" },
                new Employees { Id = 2, Name = "Jane", Surname = "Doe" }
            }.AsQueryable();

            // Create mock DbSet
            var mockSet = new Mock<DbSet<Employees>>();
            mockSet.As<IQueryable<Employees>>().Setup(m => m.Provider).Returns(employees.Provider);
            mockSet.As<IQueryable<Employees>>().Setup(m => m.Expression).Returns(employees.Expression);
            mockSet.As<IQueryable<Employees>>().Setup(m => m.ElementType).Returns(employees.ElementType);
            mockSet.As<IQueryable<Employees>>().Setup(m => m.GetEnumerator()).Returns(() => employees.GetEnumerator());

            // Create mock DbContext
            _mockDb = new Mock<ApplicationDbContext>();
            _mockDb.Setup(db => db.Employees).Returns(mockSet.Object);

            _mockLogger = new Mock<ILogger<EmployeesController>>();
            _mockToastNotification = new Mock<INotyfService>();

            _mockHostingEnvironment = new Mock<IWebHostEnvironment>();
            _mockHostingEnvironment.Setup(m => m.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));


            _controller = new EmployeesController(
                _mockDb.Object,
                _mockLogger.Object,
                _mockToastNotification.Object,
                _mockHostingEnvironment.Object);
        }

        [TestMethod]
        public void Index_ReturnsViewResult_WithListOfEmployees()
        {
            //// Arrange
            //var employees = new List<Employees>
            //{
            //    new Employees { Id = 1, Name = "John", Surname = "Doe" },
            //    new Employees { Id = 2, Name = "Jane", Surname = "Doe" }
            //}.AsQueryable();

            //HelpFunctions<Employees>.ReturnsDbSet(_mockDb, employees);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult.Model);

            var model = (IEnumerable<Employees>)viewResult.Model;
            Assert.AreEqual(2, model.Count());

        }

        //[TestMethod]
        //public void Create_WithValidModelAndImageFile_CreatesEmployeeAndRedirectsToIndex()
        //{
        //    // Arrange
        //    var employee = new Employees { Name = "John", Surname = "Doe" };
        //    var imageFile = new Mock<IFormFile>();
        //    imageFile.Setup(f => f.Length).Returns(1);
        //    imageFile.Setup(f => f.FileName).Returns("test.jpg");

        //    // Act
        //    var result = _controller.Create(employee, imageFile.Object);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

        //    var redirectToActionResult = result as RedirectToActionResult;
        //    Assert.AreEqual("Index", redirectToActionResult.ActionName);

        //    _mockDb.Verify(db => db.Employees.Add(It.IsAny<Employees>()), Times.Once);
        //    _mockDb.Verify(db => db.SaveChanges(), Times.Once);
        //    _mockToastNotification.Verify(toast => toast.Success(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        //}

        
    }
}
