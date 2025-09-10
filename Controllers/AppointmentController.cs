using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository _doctorRepo;

        public AppointmentController(
            IAppointmentRepository appointmentRepo,
            IInvoiceRepository invoiceRepo,
            IPatientRepository patientRepo,
            IDoctorRepository doctorRepo)
        {
            _appointmentRepo = appointmentRepo;
            _invoiceRepo = invoiceRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
        }

        // GET: /Appointment
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Patient"))
            {
                var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(patientIdClaim, out var patientId))
                    return Unauthorized();

                var appointments = await _appointmentRepo.GetByPatientIdAsync(patientId);
                return View(appointments);
            }

            // Admin & Doctor see all appointments
            var allAppointments = await _appointmentRepo.GetAllAsync();
            return View(allAppointments);
        }

        // GET: /Appointment/Create
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Patient"))
            {
                ViewBag.Doctors = await _doctorRepo.GetAllAsync();
            }
            else if (User.IsInRole("Admin"))
            {
                ViewBag.Patients = await _patientRepo.GetAllAsync();
                ViewBag.Doctors = await _doctorRepo.GetAllAsync();
            }
            return View();
        }

        // POST: /Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // Assign PatientId based on role
            if (User.IsInRole("Patient"))
            {
                var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(patientIdClaim, out var patientId))
                    return Unauthorized();

                appointment.PatientId = patientId;
            }

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Patient"))
                    ViewBag.Doctors = await _doctorRepo.GetAllAsync();
                else if (User.IsInRole("Admin"))
                {
                    ViewBag.Patients = await _patientRepo.GetAllAsync();
                    ViewBag.Doctors = await _doctorRepo.GetAllAsync();
                }
                return View(appointment);
            }

            // Save appointment
            await _appointmentRepo.AddAsync(appointment);
            await _appointmentRepo.SaveAsync();

            // Create invoice automatically
            var invoice = new Invoice
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DateIssued = System.DateTime.Now,
                Amount = 500m,
                IsPaid = false
            };

            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Appointment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            // Patients can only see their own appointments
            if (User.IsInRole("Patient"))
            {
                var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(patientIdClaim, out var patientId) || appointment.PatientId != patientId)
                    return Unauthorized();
            }

            return View(appointment);
        }

        // GET: /Appointment/Edit/5 (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            ViewBag.Patients = await _patientRepo.GetAllAsync();
            ViewBag.Doctors = await _doctorRepo.GetAllAsync();
            return View(appointment);
        }

        // POST: /Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _patientRepo.GetAllAsync();
                ViewBag.Doctors = await _doctorRepo.GetAllAsync();
                return View(appointment);
            }

            _appointmentRepo.Update(appointment);
            await _appointmentRepo.SaveAsync();

            // Update invoice if exists
            if (appointment.Invoice != null)
            {
                var invoice = await _invoiceRepo.GetByIdAsync(appointment.Invoice.InvoiceId);
                if (invoice != null)
                {
                    invoice.PatientId = appointment.PatientId;
                    invoice.AppointmentId = appointment.AppointmentId;
                    invoice.Amount = 500m;
                    await _invoiceRepo.SaveAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Appointment/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // POST: /Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            _appointmentRepo.Delete(appointment);
            await _appointmentRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
