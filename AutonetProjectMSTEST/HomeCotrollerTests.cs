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
    public class HomeCotrollerTests
    {
        private HomeController _controller;
        private Mock<ApplicationDbContext> _mockDb;
        private Mock<ILogger<HomeController>> _mockLogger;
        private Mock<INotyfService> _mockToastNotification;
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;
    }
}
