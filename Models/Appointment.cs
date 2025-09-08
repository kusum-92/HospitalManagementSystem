using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HospitalManagementSystem.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        [ForeignKey(nameof(PatientId))]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Invoice? Invoice { get; set; }

    }
}
