using AutonetProjectMVCASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutonetProjectMSTEST
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void AppointmentNameIsRequired()
        {
            // Arrange
            var appointment = new Appointments
            {
                Time = DateTime.Now,
                Location = "Test Location",
                UserId = "test@example.com",
                EmployeeId = 1
            };

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => ValidateModel(appointment));
        }

        [TestMethod]
        public void AppointmentTimeIsNotRequired()
        {
            // Arrange
            var appointment = new Appointments
            {
                Name = "Test Appointment",
                Location = "Test Location",
                UserId = "test@example.com",
                EmployeeId = 1
            };

            // Act & Assert
            ValidateModel(appointment);
        }

        [TestMethod]
        public void AppointmentValidModelIsValid()
        {
            // Arrange
            var appointment = new Appointments
            {
                Name = "Test Appointment",
                Time = DateTime.Now,
                Location = "Test Location",
                UserId = "test@example.com",
                EmployeeId = 1
            };

            // Act & Assert
            ValidateModel(appointment);
        }

        [TestMethod]
        public void AppointmentToStringReturnsExpectedFormat()
        {
            // Arrange
            var appointment = new Appointments
            {
                Name = "Test Appointment",
                Time = DateTime.Now,
                Location = "Test Location",
                UserId = "test@example.com",
                EmployeeId = 1
            };

            string expected = $"{appointment.Name} {appointment.Time} {appointment.Location} {appointment.UserId} {appointment.EmployeeId}";

            // Act
            string result = appointment.ToString();

            // Assert
            Assert.AreEqual(expected, result);
        }

        private void ValidateModel(Appointments appointment)
        {
            // Using DataAnnotations for validation
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(appointment, null, null);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();

            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(appointment, validationContext, results, true);

            if (!isValid)
            {
                throw new InvalidOperationException(string.Join(", ", results));
            }
        }

        //Employees
        [TestMethod]
        public void EmployeeNameIsRequired()
        {
            // Arrange
            var employee = new Employees
            {
                Surname = "TestSurname",
                Job = "TestJob",
                ImagePath = "testImagePath"
            };

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => ValidateModel(employee));
        }

        [TestMethod]
        public void EmployeeSurnameIsRequired()
        {
            // Arrange
            var employee = new Employees
            {
                Name = "TestName",
                Job = "TestJob",
                ImagePath = "testImagePath"
            };

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => ValidateModel(employee));
        }

        [TestMethod]
        public void EmployeeValidModelIsValid()
        {
            // Arrange
            var employee = new Employees
            {
                Name = "TestName",
                Surname = "TestSurname",
                Job = "TestJob",
                ImagePath = "testImagePath"
            };

            // Act & Assert
            ValidateModel(employee);
        }

        

        private void ValidateModel(Employees employee)
        {
            // Using DataAnnotations for validation
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(employee, null, null);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();

            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(employee, validationContext, results, true);

            if (!isValid)
            {
                throw new InvalidOperationException(string.Join(", ", results));
            }
        }

        //Locations


        [TestMethod]
        public void LocationAddressIsRequired()
        {
            // Arrange
            var location = new Locations
            {
                Place = "TestPlace",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Latitude = 0,
                Longitude = 0,
                ImagePath = "testImagePath"
            };

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => ValidateModel(location));
        }

        [TestMethod]
        public void LocationValidModelIsValid()
        {
            // Arrange
            var location = new Locations
            {
                Place = "TestPlace",
                Address = "TestAddress",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Latitude = 0,
                Longitude = 0,
                ImagePath = "testImagePath"
            };

            // Act & Assert
            ValidateModel(location);
        }


        private void ValidateModel(Locations location)
        {
            // Using DataAnnotations for validation
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(location, null, null);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();

            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(location, validationContext, results, true);

            if (!isValid)
            {
                throw new InvalidOperationException(string.Join(", ", results));
            }
        }
    }
}

