using System.ComponentModel.DataAnnotations;

namespace AutonetProjectMVCASP.Models
{
    public class Employees
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Surname { get; set; }
        public string? Job { get; set; }
        // Property to store the path of the uploaded image
        public string? ImagePath { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<LocationEmployee>? LocationEmployees { get; set; }

    }
}
