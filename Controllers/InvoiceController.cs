using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        // GET: Invoice
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return View(invoices);
        }

        // GET: Invoice/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // GET: Invoice/ByPatient/5
        public async Task<IActionResult> ByPatient(int patientId)
        {
            var invoices = await _invoiceRepository.GetByPatientIdAsync(patientId);
            if (invoices == null)
                return NotFound();

            return View(invoices);
        }

        // GET: Invoice/Unpaid
        public async Task<IActionResult> Unpaid()
        {
            var unpaidInvoices = await _invoiceRepository.GetUnpaidInvoicesAsync();
            return View(unpaidInvoices);
        }

        // GET: Invoice/MarkAsPaid/5
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // POST: Invoice/MarkAsPaid/5
        [HttpPost, ActionName("MarkAsPaid")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaidConfirmed(int id)
        {
            await _invoiceRepository.MarkAsPaidAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
