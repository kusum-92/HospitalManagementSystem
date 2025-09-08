using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HospitalManagementSystem.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Specialization { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }

}
