using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateIssued { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        // Foreign Key to Patient
        [Required]
        [ForeignKey(nameof(PatientId))]
        public int PatientId { get; set; }

   
        public Patient Patient { get; set; }

        // Foreign Key to Appointment
        [Required]
        public int AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; }
    }
}