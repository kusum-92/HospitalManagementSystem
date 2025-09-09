using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartment _departmentRepository;

        public DepartmentController(IDepartment departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return View(departments);
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return NotFound();
            return View(department);
        }

        // GET: Department/Create
        public IActionResult Create() => View();

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Department department)
        {
            if (!ModelState.IsValid)
                return View(department);

            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return NotFound();
            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,Name")] Department department)
        {
            if (id != department.DepartmentID)
                return NotFound();

            if (!ModelState.IsValid)
                return View(department);

            _departmentRepository.Update(department);
            await _departmentRepository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return NotFound();
            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department != null)
            {
                _departmentRepository.Delete(department);
                await _departmentRepository.SaveAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
