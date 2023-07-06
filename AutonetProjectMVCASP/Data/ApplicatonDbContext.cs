using Microsoft.EntityFrameworkCore;
using AutonetProjectMVCASP.Models;

namespace AutonetProjectMVCASP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<Appointments> Appointments { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
    }
}
