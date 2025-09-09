using HospitalManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Repository.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task AddAsync(Department department);
        void Update(Department department);
        void Delete(Department department);
        Task SaveAsync();
    }
}
