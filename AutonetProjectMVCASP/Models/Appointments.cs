using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutonetProjectMVCASP.Models
{
    public class Appointments
    {
        [Key]
        public int Id { get; set; } //main identification method
        [Required]
        public string? Name { get; set; }   //name of the appointment
        public DateTime Time { get; set; } //start time of the apointment, in the current system each one represents one hour
        public string? Location { get; set; }   //location of the appointment
        [ForeignKey("AspNetUsers")]
        public string? UserId { get; set; } //this will be the email, not the user Id, just keep that in mind
                                            //user that created the appointment
        [ForeignKey("Employees")]
        public int EmployeeId { get; set; } //employee chosen for the appointment, if none is chosen, it is -1

        public string? JobId {  get; set; } //job that the appointment is related to, used for email schedueling
        public override string ToString() //simple ToString method, easier text generation
        {
            return Name + " " + Time + " " + Location + " " + UserId + " " + EmployeeId;
        }
    }
}
