using Microsoft.EntityFrameworkCore;
using AutonetProjectMVCASP.Models;

namespace TutorialWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Category> Category { get; set; }
    }
}
