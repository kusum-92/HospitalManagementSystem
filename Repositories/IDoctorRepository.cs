using HospitalManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Repository.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task AddAsync(Doctor doctor);
        void Update(Doctor doctor);
        void Delete(Doctor doctor);
        Task SaveAsync();
    }
}
