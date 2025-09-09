using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctor _doctorRepository;
        private readonly IDepartment _departmentRepository;

        public DoctorController(IDoctor doctorRepository, IDepartment departmentRepository)
        {
            _doctorRepository = doctorRepository;
            _departmentRepository = departmentRepository;
        }

        // GET: Doctor
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            return View(doctors);
        }

        // GET: Doctor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();
            return View(doctor);
        }

        // GET: Doctor/ByAppointment/5
        public async Task<IActionResult> ByAppointment(int appointmentId)
        {
            var doctor = await _doctorRepository.GetByAppointmentIdAsync(appointmentId);
            if (doctor == null)
                return NotFound();
            return View("Details", doctor);
        }

        // Helper to populate departments dropdown
        private async Task PopulateDepartmentsAsync(int? selected = null)
        {
            var departments = await _departmentRepository.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name", selected);
        }

        // GET: Doctor/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsAsync();   
            return View();
        }

        // POST: Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Specialization,DepartmentId")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                await PopulateDepartmentsAsync(doctor.DepartmentId);
                return View(doctor);
            }

            await _doctorRepository.AddAsync(doctor);
            await _doctorRepository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctor/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();
            await PopulateDepartmentsAsync(doctor.DepartmentId);
            return View(doctor);
        }

        // POST: Doctor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,FullName,Specialization,DepartmentId")] Doctor doctor)
        {
            if (id != doctor.DoctorId)
                return NotFound();

            if (ModelState.IsValid)
            {
                await PopulateDepartmentsAsync(doctor.DepartmentId);
                return View(doctor);
            }

            _doctorRepository.Update(doctor);
            await _doctorRepository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctor/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();
            return View(doctor);
        }

        // POST: Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor != null)
            {
                _doctorRepository.Delete(doctor);
                await _doctorRepository.SaveAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
