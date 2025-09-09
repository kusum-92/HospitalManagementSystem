using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Repository.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Doctors)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Doctors)
                .FirstOrDefaultAsync(d => d.DepartmentID == id);
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public void Update(Department department)
        {
            _context.Departments.Update(department);
        }

        public void Delete(Department department)
        {
            _context.Departments.Remove(department);
        }

        //public async Task SaveAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}
    }
}
