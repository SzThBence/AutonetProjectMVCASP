using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutonetProjectMVCASP.Models
{
    public class Locations
    {
        [Key]
        public string Place { get; set; }   //main identification method, name of the location
        [Required]
        public string Address { get; set; } //address of the location
        public DateTime StartTime { get; set; } //first working hour
        public DateTime EndTime { get; set; }   //last working hour
        public float Latitude { get; set; } // Latitude and Longitude for the location on a map
        public float Longitude { get; set; }
        public string? Title { get; set; }  //short descriptor
        public string? ImagePath { get; set; }  // Property to store the path of the promotional image

        // Navigation property for many-to-many relationship
        public ICollection<LocationEmployee>? LocationEmployees { get; set; }

    }

    //old code, went with the many/many relationship instead, kept, if we want to try a different approach
    public class EmployeeId
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
