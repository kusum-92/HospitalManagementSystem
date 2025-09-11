using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IAppointmentRepository _appointmentRepo; // ✅ add repo
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPatientRepository _patientRepo;

        public DoctorController(
            IDoctorRepository doctorRepo,
            IDepartmentRepository departmentRepo,
            IAppointmentRepository appointmentRepo,
            IPatientRepository patientRepo,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _userManager = userManager;
            _roleManager = roleManager;
        }

       
        // 🔹 Doctor Dashboard (only for Doctors)
        [Authorize(Roles = "Doctor")]
        public IActionResult Dashboard()
        {
            return View();
        }

        // 🔹 Manage Doctors (only Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            return View(doctors);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentRepo.GetAllAsync() ?? new List<Department>();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Doctor doctor, string email, string password)
        {
            if (ModelState.IsValid)
            {
                var departments = await _departmentRepo.GetAllAsync() ?? new List<Department>();
                ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name", doctor.DepartmentId);
                return View(doctor);
            }

            // 1️⃣ Create Identity account first
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(doctor);
            }

            // 2️⃣ Ensure role exists
            if (!await _roleManager.RoleExistsAsync("Doctor"))
                await _roleManager.CreateAsync(new IdentityRole("Doctor"));
            await _userManager.AddToRoleAsync(user, "Doctor");

            // 3️⃣ Link doctor to Identity user
            doctor.IdentityUserId = user.Id;

            // 4️⃣ Save doctor in DB
            await _doctorRepo.AddAsync(doctor);
            await _doctorRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            var departments = await _departmentRepo.GetAllAsync() ?? new List<Department>();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name", doctor.DepartmentId);
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Doctor doctor)
        {
            if (id != doctor.DoctorId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var departments = await _departmentRepo.GetAllAsync() ?? new List<Department>();
                ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name", doctor.DepartmentId);
                return View(doctor);
            }

            _doctorRepo.Update(doctor);
            await _doctorRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            _doctorRepo.Delete(doctor);
            await _doctorRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 🔹 NEW: Doctor-only actions
        // ============================================================

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _doctorRepo.GetByIdentityUserIdAsync(userId);
            if (doctor == null) return Forbid();

            var appointments = await _appointmentRepo.GetAppointmentsByDoctorIdAsync(doctor.DoctorId);
            return View("MyAppointments", appointments); // reuse existing view
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyPatients()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _doctorRepo.GetByIdentityUserIdAsync(userId);
            if (doctor == null) return Forbid();

            var appointments = await _appointmentRepo.GetAppointmentsByDoctorIdAsync(doctor.DoctorId);
            var patients = appointments
                .Where(a => a.Patient != null)
                .Select(a => a.Patient)
                .Distinct()
                .ToList();

            return View("~/Views/Patient/Index.cshtml", patients); // reuse existing view
        }

        private async Task<Doctor> GetLoggedInDoctorAsync()
        {
            var identityUserId = _userManager.GetUserId(User); // string
            if (identityUserId == null) return null;
            return await _doctorRepo.GetByIdentityUserIdAsync(identityUserId);
        }

    }
}
