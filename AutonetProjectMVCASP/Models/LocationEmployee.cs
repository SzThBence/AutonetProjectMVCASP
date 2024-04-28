using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace AutonetProjectMVCASP.Models
{
    [Keyless]
    public class LocationEmployee
    {   
        //Identifier properties
        public string LocationPlace { get; set; }   //location

        public int EmployeeId { get; set; }         //employee

        // Navigation properties
        public Locations Location { get; set; } //location

        public Employees Employee { get; set; } //employee
    }
}
