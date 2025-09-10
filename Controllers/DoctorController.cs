using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoctorController : Controller
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public DoctorController(IDoctorRepository doctorRepo, IDepartmentRepository departmentRepo)
        {
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
        }

        // GET: /Doctor
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            return View(doctors);
        }

        // GET: /Doctor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // GET: /Doctor/Create
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentRepo.GetAllAsync() ?? new List<Department>();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name");
            return View();
        }

        // POST: /Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                var departments = await _departmentRepo.GetAllAsync() ?? new List<Department>();
                ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name", doctor.DepartmentId);
                return View(doctor);
            }

            await _doctorRepo.AddAsync(doctor);
            await _doctorRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Doctor/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            var departments = await _departmentRepo.GetAllAsync() ?? new List<Department>();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name", doctor.DepartmentId);
            return View(doctor);
        }

        // POST: /Doctor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctor doctor)
        {
            if (id != doctor.DoctorId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var departments = await _departmentRepo.GetAllAsync() ?? new List<Department>();
                ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name", doctor.DepartmentId);
                return View(doctor);
            }

            _doctorRepo.Update(doctor);
            await _doctorRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Doctor/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // POST: /Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            _doctorRepo.Delete(doctor);  // synchronous delete
            await _doctorRepo.SaveAsync();  // async save

            return RedirectToAction(nameof(Index));
        }
    }
}
