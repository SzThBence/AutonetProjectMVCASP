using Microsoft.EntityFrameworkCore;
using AutonetProjectMVCASP.Models;

namespace AutonetProjectMVCASP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Appointments> Appointments { get; set; }

        public DbSet<Employees> Employees { get; set; }
    }
}
