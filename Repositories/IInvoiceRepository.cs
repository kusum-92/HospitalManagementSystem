using HospitalManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Repository.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();   // New method
        Task AddAsync(Invoice invoice);
        Task MarkAsPaidAsync(int invoiceId);
        Task SaveAsync();
    }
}
