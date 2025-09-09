using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
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
            ViewBag.Departments = await _departmentRepo.GetAllAsync();
            return View();
        }

        // POST: /Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepo.GetAllAsync();
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

            ViewBag.Departments = await _departmentRepo.GetAllAsync();
            return View(doctor);
        }

        // POST: /Doctor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctor doctor)
        {
            if (id != doctor.DoctorId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepo.GetAllAsync();
                return View(doctor);
            }

            _doctorRepo.Update(doctor);  // synchronous update
            await _doctorRepo.SaveAsync();  // async save

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
