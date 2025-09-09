using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100)]
        public string? Name { get; set; }

        // Initialized to avoid null refs
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
