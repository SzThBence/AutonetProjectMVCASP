using Microsoft.EntityFrameworkCore;
using AutonetProjectMVCASP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AutonetProjectMVCASP.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public ApplicationDbContext() { }
        public virtual DbSet<Appointments> Appointments { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }

        public DbSet<LocationEmployee> LocationEmployees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Locations and Employees
            modelBuilder.Entity<LocationEmployee>()
                .HasKey(le => new { le.LocationPlace, le.EmployeeId });

            modelBuilder.Entity<LocationEmployee>()
                .HasOne(le => le.Location)
                .WithMany(l => l.LocationEmployees)
                .HasForeignKey(le => le.LocationPlace);

            modelBuilder.Entity<LocationEmployee>()
                .HasOne(le => le.Employee)
                .WithMany(e => e.LocationEmployees)
                .HasForeignKey(le => le.EmployeeId);
        }
    }
}
