using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        [Required]
        public string? Name { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
    }
}
