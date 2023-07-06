using System.ComponentModel.DataAnnotations;

namespace AutonetProjectMVCASP.Models
{
    public class Locations
    {
        [Key]
        public string Place { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime StaryTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
