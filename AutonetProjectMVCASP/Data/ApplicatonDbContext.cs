using Microsoft.EntityFrameworkCore;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AutonetProjectMVCASP.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<Appointments> Appointments { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
    }
}
