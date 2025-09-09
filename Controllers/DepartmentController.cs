using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepo;

        public DepartmentController(IDepartmentRepository departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        // GET: /Department
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentRepo.GetAllAsync();
            return View(departments);
        }

        // GET: /Department/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentRepo.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // GET: /Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (!ModelState.IsValid)
                return View(department);

            await _departmentRepo.AddAsync(department);
            

            return RedirectToAction(nameof(Index));
        }

        // GET: /Department/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentRepo.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: /Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.DepartmentID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(department);

            _departmentRepo.Update(department);
            await _departmentRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Department/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentRepo.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: /Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _departmentRepo.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            _departmentRepo.Delete(department);
            await _departmentRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
