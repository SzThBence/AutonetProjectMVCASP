using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutonetProjectMVCASP.Models
{
    public class Appointments
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public DateTime Time { get; set; }
        public string? Location { get; set; }
        [ForeignKey("AspNetUsers")]
        public string? UserId { get; set; } //this will be the email, not the user Id, just keep that in mind

        [ForeignKey("Employees")]
        public int EmployeeId { get; set; }

        public string? JobId {  get; set; }
        public override string ToString() 
        {
            return Name + " " + Time + " " + Location + " " + UserId + " " + EmployeeId;
        }
    }
}
