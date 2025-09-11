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

        // Fixed ForeignKey attribute: it should reference the navigation property name
        [ForeignKey(nameof(Department))]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<Appointment> Appointments { get; set; } //get doctor by appointments
                                                                   // Patient.cs
        public string IdentityUserId { get; set; }  // store IdentityUser.Id (string)

    }

}
