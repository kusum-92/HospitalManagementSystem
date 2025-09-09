using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        [Required]
        public string? Name { get; set; }

        // Initialized to avoid null refs
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
