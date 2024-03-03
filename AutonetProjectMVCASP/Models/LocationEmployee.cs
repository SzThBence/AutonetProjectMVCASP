using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace AutonetProjectMVCASP.Models
{
    [Keyless]
    public class LocationEmployee
    {
        public string LocationPlace { get; set; }

        public int EmployeeId { get; set; }

        // Navigation properties
        public Locations Location { get; set; }

        public Employees Employee { get; set; }
    }
}
