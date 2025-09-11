using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly UserManager<IdentityUser> _userManager;
        public AppointmentController(
            IAppointmentRepository appointmentRepo,
            IInvoiceRepository invoiceRepo,
            IPatientRepository patientRepo,
            IDoctorRepository doctorRepo,
            UserManager<IdentityUser> userManager)
        {
            _appointmentRepo = appointmentRepo;
            _invoiceRepo = invoiceRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
            _userManager = userManager;
        }

        // GET: /Appointment
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentRepo.GetAllAsync();
            return View(appointments);
        }

        // GET: /Appointment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // GET: /Appointment/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _patientRepo.GetAllAsync();
            ViewBag.Doctors = await _doctorRepo.GetAllAsync();
            return View();
        }

        // POST: /Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Patients = await _patientRepo.GetAllAsync();
                ViewBag.Doctors = await _doctorRepo.GetAllAsync();
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
                DateIssued = DateTime.Now,
                Amount = 500m, // Replace with real calculation
                IsPaid = false
            };

            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

       
        // GET: /Appointment/Edit/5
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            if (User.IsInRole("Patient"))
            {
                var userId = _userManager.GetUserId(User);
                var patient = await _patientRepo.GetByIdentityUserIdAsync(userId);

                if (appointment.PatientId != patient.Id)
                    return Forbid();

                // Patients cannot select other patients
                ViewBag.Doctors = await _doctorRepo.GetAllAsync();
                return View(appointment);
            }

            // Admin flow
            ViewBag.Patients = await _patientRepo.GetAllAsync();
            ViewBag.Doctors = await _doctorRepo.GetAllAsync();
            return View(appointment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                ViewBag.Patients = await _patientRepo.GetAllAsync();
                ViewBag.Doctors = await _doctorRepo.GetAllAsync();
                return View(appointment);
            }

            _appointmentRepo.Update(appointment);
            await _appointmentRepo.SaveAsync();

            // Update invoice if it exists
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

            // 🔹 Redirect based on role
            if (User.IsInRole("Patient"))
                return RedirectToAction("MyAppointments", "Patient");

            return RedirectToAction(nameof(Index)); // Admin
        }

        // GET: /Appointment/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            _appointmentRepo.Delete(appointment);
            await _appointmentRepo.SaveAsync();

            // 🔹 Redirect based on role
            if (User.IsInRole("Patient"))
                return RedirectToAction("MyAppointments", "Patient");

            return RedirectToAction(nameof(Index)); // Admin
        }
    }
}
