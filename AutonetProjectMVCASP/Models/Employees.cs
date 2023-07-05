using System.ComponentModel.DataAnnotations;

namespace AutonetProjectMVCASP.Models
{
    public class Employees
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Job { get; set; }
    }
}
