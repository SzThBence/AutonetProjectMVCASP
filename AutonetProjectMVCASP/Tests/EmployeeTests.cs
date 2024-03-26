using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutonetProjectMVCASP.Tests
{
    

    [TestClass]
    public class EmployeeTests
    {
        private AutonetProjectMVCASP.Models.Employees employee;

        [TestInitialize]
        public void TestInitialize()
        {
            employee = new AutonetProjectMVCASP.Models.Employees();
        }

        [TestMethod]
        public void TestEmployeeName()
        {

            // Act
            employee.Name = "John";

            // Assert
            Assert.AreEqual("John", employee.Name);
        }

        [TestMethod]
        public void TestEmployeeSurname()
        {
            

            // Act
            employee.Surname = "Doe";

            // Assert
            Assert.AreEqual("Doe", employee.Surname);
        }

        [TestMethod]
        public void TestEmployeeJob()
        {
            

            // Act
            employee.Job = "Manager";

            // Assert
            Assert.AreEqual("Manager", employee.Job);
        }

        [TestMethod]
        public void TestEmployeeImagePath()
        {
            

            // Act
            employee.ImagePath = "images/john_doe.jpg";

            // Assert
            Assert.AreEqual("images/john_doe.jpg", employee.ImagePath);

        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Cleanup code, if any
        }
    }
}
