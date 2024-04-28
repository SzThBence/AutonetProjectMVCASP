using System.ComponentModel.DataAnnotations;

namespace AutonetProjectMVCASP.Models
{
    public class Employees
    {
        [Key]
        public int Id { get; set; } //Main identification method
        [Required]
        public string? Name { get; set; }  //Name of the employee
        [Required]
        public string? Surname { get; set; } //Surname of the employee
        public string? Job { get; set; } //Job title of the employee

        public string? ImagePath { get; set; }  // Property to store the path of the uploaded image

        // Navigation property for many-to-many relationship
        public ICollection<LocationEmployee>? LocationEmployees { get; set; }

    }
}
