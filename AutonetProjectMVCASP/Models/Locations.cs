using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutonetProjectMVCASP.Models
{
    public class Locations
    {
        [Key]
        public string Place { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public float Latitude { get; set; } // Latitude and Longitude for the location on a map
        public float Longitude { get; set; }
        public string? Title { get; set; }
        public string? ImagePath { get; set; }  // Property to store the path of the promotional image
        // Navigation property for many-to-many relationship
        public ICollection<LocationEmployee>? LocationEmployees { get; set; }

    }

    public class EmployeeId
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
