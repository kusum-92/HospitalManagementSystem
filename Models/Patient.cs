using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string ContactNumber { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
        // Patient.cs
        public string IdentityUserId { get; set; }  // store IdentityUser.Id (string)

    }

}
