using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepo;

        public PatientController(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        // GET: /Patient
        public async Task<IActionResult> Index()
        {
            var patients = await _patientRepo.GetAllAsync();
            return View(patients);
        }

        // GET: /Patient/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // GET: /Patient/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (!ModelState.IsValid)
                return View(patient);

            await _patientRepo.AddAsync(patient);
            await _patientRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Patient/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // POST: /Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: /Patient/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // POST: /Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            _patientRepo.Delete(patient);
            await _patientRepo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
