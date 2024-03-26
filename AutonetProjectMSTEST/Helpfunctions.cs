using AutonetProjectMVCASP.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutonetProjectMSTEST
{
    public class HelpFunctions<T> where T : class
    {
        public static DbSet<T> ReturnsDbSet<T>(Mock<ApplicationDbContext> mock, IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock.Setup(db => db.Set<T>()).Returns(mockSet.Object);
            return mockSet.Object;
        }
    }
}
