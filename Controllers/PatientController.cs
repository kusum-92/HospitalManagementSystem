using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDoctorRepository _doctorRepo;

        public PatientController(IPatientRepository patientRepo, UserManager<IdentityUser> usermanager, IAppointmentRepository appointmentRepo,
            IInvoiceRepository invoiceRepo, RoleManager<IdentityRole> roleManagar, IDoctorRepository doctorRepository)
        {
            _patientRepo = patientRepo;
            _userManager = usermanager;
            _appointmentRepo = appointmentRepo;
            _invoiceRepo = invoiceRepo;
            _roleManager = roleManagar;
            _doctorRepo = doctorRepository;
        }

        // 🔹 Patient Dashboard (only for Patients)
        [Authorize(Roles = "Patient")]
        public IActionResult Dashboard()
        {
            return View();
        }

        // 🔹 Manage Patients (only Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var patients = await _patientRepo.GetAllAsync();
            return View(patients);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // get available doctors to choose from
            var doctors = await _appointmentRepo.GetAvailableDoctorsAsync();
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName");
            return View();
        }

        [Authorize(Roles = "Patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            var patient = await GetLoggedInPatientAsync();
            if (patient == null) return Forbid();

            if (ModelState.IsValid)
            {
                // force appointment to belong to logged-in patient
                appointment.PatientId = patient.Id;

                await _appointmentRepo.AddAsync(appointment);
                await _appointmentRepo.SaveAsync();

                return RedirectToAction(nameof(MyAppointments));
            }

            // reload doctors if validation fails
            var doctors = await _appointmentRepo.GetAvailableDoctorsAsync();
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName");
            return View(appointment);
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(patient);

            _patientRepo.Update(patient);
            await _patientRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            _patientRepo.Delete(patient);
            await _patientRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
        private async Task<Patient> GetLoggedInPatientAsync()
        {
            var identityUserId = _userManager.GetUserId(User);
            if (identityUserId == null) return null;
            return await _patientRepo.GetByIdentityUserIdAsync(identityUserId);
        }

        // 🔹 My Appointments
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _patientRepo.GetByIdentityUserIdAsync(userId);
            if (patient == null) return Forbid();

            var appointments = await _appointmentRepo.GetAppointmentsByPatientIdAsync(patient.Id);
            return View("MyAppointments", appointments); // reuse your existing view
        }

        // 🔹 My Invoices
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyInvoices()
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _patientRepo.GetByIdentityUserIdAsync(userId);
            if (patient == null) return Forbid();

            var invoices = await _invoiceRepo.GetInvoicesByPatientIdAsync(patient.Id);
            return View("MyInvoices", invoices); // reuse your existing view
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreateAppointment()
        {
            // Get logged-in patient
            var userId = _userManager.GetUserId(User);
            var patient = await _patientRepo.GetByIdentityUserIdAsync(userId);
            if (patient == null) return Forbid();

            // Load doctors
            var doctors = await _doctorRepo.GetAllAsync();
            ViewBag.Doctors = doctors;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _patientRepo.GetByIdentityUserIdAsync(userId);
            if (patient == null) return Forbid();

            if (ModelState.IsValid)
            {
                ViewBag.Doctors = await _doctorRepo.GetAllAsync();
                return View(appointment);
            }

            // Force appointment to belong to this patient
            appointment.PatientId = patient.Id;

            await _appointmentRepo.AddAsync(appointment);
            await _appointmentRepo.SaveAsync();

            // Create invoice automatically
            var invoice = new Invoice
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = patient.Id,
                DateIssued = DateTime.Now,
                Amount = 500m,
                IsPaid = false
            };

            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveAsync();

            return RedirectToAction("MyAppointments");
        }

    }
}
