using AspNetCoreHero.ToastNotification.Abstractions;
using AutonetProjectMVCASP.Controllers;
using AutonetProjectMVCASP.Data;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AutonetProjectMSTEST
{
    public class FakeSignInManager : SignInManager<IdentityUser>
    {
        public FakeSignInManager(UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<IdentityUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<IdentityUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<IdentityUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public override Task SignInAsync(IdentityUser user, bool isPersistent, string authenticationMethod = null)
        {
            
            return Task.CompletedTask;
        }

        public override Task SignInAsync(IdentityUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            
            return Task.CompletedTask;
        }
    }

    [TestClass]
    public class HomeControllerTests
    {
        //helper for setting up UserManager
        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        private HomeController _controller;
        private Mock<ApplicationDbContext> _mockDb;
        private Mock<ILogger<HomeController>> _mockLogger;
        private Mock<ILogger<SignInManager<IdentityUser>>> _mockUserLogger;
        private Mock<INotyfService> _mockToastNotification;
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private FakeSignInManager _fakeSignInManager;

        [TestInitialize]
        public void Setup()
        {
            // Mock DbContext and DbSet
            var users = new List<IdentityUser>
            {
                new IdentityUser { Id = "1", UserName = "user1" },
                new IdentityUser { Id = "2", UserName = "user2" }
            }.AsQueryable();

            var mockUserSet = new Mock<DbSet<IdentityUser>>();
            mockUserSet.As<IQueryable<IdentityUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<IdentityUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<IdentityUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<IdentityUser>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

            _mockDb = new Mock<ApplicationDbContext>();
            _mockDb.Setup(db => db.Users).Returns(mockUserSet.Object);

            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockToastNotification = new Mock<INotyfService>();

            // Setup UserManager Mock
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object,
                null, null, null, null, null, null, null, null);

            _mockUserManager = new Mock<UserManager<IdentityUser>>(new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string userName) => users.FirstOrDefault(u => u.UserName == userName));


            // Setup RoleManager Mock
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);

            // Setup FakeSignInManager
            _mockUserLogger = new Mock<ILogger<SignInManager<IdentityUser>>>();
            _fakeSignInManager = new FakeSignInManager(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                Options.Create(new IdentityOptions()),
                _mockUserLogger.Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<IdentityUser>>().Object);

            // Instantiate HomeController
            _controller = new HomeController(
                _mockDb.Object,
                _mockLogger.Object,
                _mockToastNotification.Object,
                _mockUserManager.Object,
                _mockRoleManager.Object,
                _fakeSignInManager);
        }


        [TestMethod]
        public async Task Index_ReturnsViewResult()
        {

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Privacy_ReturnsViewResult_WithListOfLocations()
        {
            // Arrange
            var locations = new List<AutonetProjectMVCASP.Models.Locations>
        {
            new AutonetProjectMVCASP.Models.Locations { Place = "Location1", Address = "tmp1" },
            new AutonetProjectMVCASP.Models.Locations { Place = "Location2", Address = "tmp2" }
        }.AsQueryable();

            var mockLocationSet = new Mock<DbSet<AutonetProjectMVCASP.Models.Locations>>();
            mockLocationSet.As<IQueryable<AutonetProjectMVCASP.Models.Locations>>().Setup(m => m.Provider).Returns(locations.Provider);
            mockLocationSet.As<IQueryable<AutonetProjectMVCASP.Models.Locations>>().Setup(m => m.Expression).Returns(locations.Expression);
            mockLocationSet.As<IQueryable<AutonetProjectMVCASP.Models.Locations>>().Setup(m => m.ElementType).Returns(locations.ElementType);
            mockLocationSet.As<IQueryable<AutonetProjectMVCASP.Models.Locations>>().Setup(m => m.GetEnumerator()).Returns(() => locations.GetEnumerator());

            _mockDb.Setup(db => db.Locations).Returns(mockLocationSet.Object);

            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult.Model);

            var model = viewResult.Model as IEnumerable<AutonetProjectMVCASP.Models.Locations>;
            Assert.AreEqual(2, model.Count());
        }

        [TestMethod]
        public void Users_ReturnsViewResult_WithUsersAndRolesManager()
        {
            // Arrange

            // Act
            var result = _controller.Users();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult.Model);

            Assert.IsInstanceOfType(viewResult.Model, typeof(UsersAndRolesManagers));
        }

        [TestMethod]
        public async Task RoleChanger_ReturnsRedirectToAction()
        {
            // Arrange
            var roles = new Dictionary<string, List<string>>
        {
            { "1", new List<string> { "Admin", "User" } },
            { "2", new List<string> { "User" } }
        };

            // Act
            var result = await _controller.RoleChanger(roles);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task AddRolesAndUsers_ReturnsTrue()
        {
            // Arrange

            // Act
            var result = await _controller.AddRolesAndUsers();

            // Assert
            Assert.IsTrue(result);
        }

    }
}
