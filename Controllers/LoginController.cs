using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginController> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // ---------------- DOCTOR ----------------
        [HttpGet]
        public IActionResult LoginDoctor() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginDoctor(string fullName, string specialization)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.FullName == fullName && d.Specialization == specialization);

            if (doctor == null)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View();
            }

            _httpContextAccessor.HttpContext!.Session.SetInt32("DoctorId", doctor.DoctorId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, doctor.DoctorId.ToString()),
                new Claim(ClaimTypes.Name, doctor.FullName),
                new Claim(ClaimTypes.Role, "Doctor")
            };

            var identity = new ClaimsIdentity(claims, "DoctorPatientCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                "DoctorPatientCookie",
                principal,
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) }
            );

            return RedirectToAction("DashboardDoctor");
        }

        [HttpGet]
        public async Task<IActionResult> RegisterDoctor()
        {
            var departments = await _context.Departments.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterDoctor([Bind("FullName,Specialization,DepartmentId")] Doctor doctor)
        {
            // CORRECT logic: save when ModelState.IsValid
            if (ModelState.IsValid)
            {
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
                // redirect to doctor login (they can then login)
                return RedirectToAction("LoginDoctor");
            }

            // if invalid, reload departments and show view with validation messages
            var departments = await _context.Departments.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "Name");
            return View(doctor);
        }

        public async Task<IActionResult> DashboardDoctor()
        {
            int? doctorId = _httpContextAccessor.HttpContext!.Session.GetInt32("DoctorId");
            if (doctorId == null)
            {
                // if they somehow are authenticated via cookie but session missing, you can still try to read claim
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idClaim, out var tmp)) return RedirectToAction("LoginDoctor");
                doctorId = tmp;
                _httpContextAccessor.HttpContext.Session.SetInt32("DoctorId", tmp);
            }

            var doctor = await _context.Doctors.Include(d => d.Department)
                                               .FirstOrDefaultAsync(d => d.DoctorId == doctorId.Value);
            if (doctor == null) return RedirectToAction("LoginDoctor");

            return View(doctor);
        }

        // ---------------- PATIENT ----------------
        [HttpGet]
        public IActionResult LoginPatient() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPatient(string fullName, string contactNumber)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.FullName == fullName && p.ContactNumber == contactNumber);

            if (patient == null)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View();
            }

            _httpContextAccessor.HttpContext!.Session.SetInt32("PatientId", patient.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
                new Claim(ClaimTypes.Name, patient.FullName),
                new Claim(ClaimTypes.Role, "Patient")
            };

            var identity = new ClaimsIdentity(claims, "DoctorPatientCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                "DoctorPatientCookie",
                principal,
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) }
            );

            return RedirectToAction("DashboardPatient");
        }

        [HttpGet]
        public IActionResult RegisterPatient() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPatient([Bind("FullName,DOB,Gender,ContactNumber")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction("LoginPatient");
            }
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogWarning("Validation error in field {Field}: {ErrorMessage}",
                                       state.Key, error.ErrorMessage);
                }
            }
            return View(patient);
        }

        public async Task<IActionResult> DashboardPatient()
        {
            int? patientId = _httpContextAccessor.HttpContext!.Session.GetInt32("PatientId");
            if (patientId == null)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idClaim, out var tmp)) return RedirectToAction("LoginPatient");
                patientId = tmp;
                _httpContextAccessor.HttpContext.Session.SetInt32("PatientId", tmp);
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == patientId.Value);
            if (patient == null) return RedirectToAction("LoginPatient");

            return View(patient);
        }

        // ---------------- ADMIN (Identity) ----------------
        [HttpGet]
        public IActionResult LoginAdmin() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAdmin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Provide email and password.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded)
                    return RedirectToAction("DashboardAdmin");
            }

            ModelState.AddModelError("", "Invalid admin credentials.");
            return View();
        }

        public IActionResult DashboardAdmin()
        {
            return View();
        }

        // ---------------- LOGOUT ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out Doctor/Patient cookie authentication
            await HttpContext.SignOutAsync("DoctorPatientCookie");  //CookieAuthenticationDefaults.AuthenticationScheme

            // Sign out Identity (for Admin)
            await _signInManager.SignOutAsync();

            // Redirect to Logout page
            return RedirectToAction("LoggedOut");
        }

        [HttpGet]
        public IActionResult LoggedOut()
        {
            return View();
        }
    }
}
