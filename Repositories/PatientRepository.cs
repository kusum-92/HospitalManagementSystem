using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Repository.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<Patient> GetByIdentityUserIdAsync(string identityUserId)
        //{
        //    return await _context.Patients
        //        .Include(p => p.Appointments)
        //        .Include(p => p.Invoices)
        //        .FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);
        //}

        //public async Task<Patient> GetByIdentityUserIdAsync(string identityUserId)
        //{
        //    return await _context.Patients
        //        .FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);
        //}

        public async Task<Patient> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Patients
                .Include(p => p.Appointments)
                .Include(p => p.Invoices)
                .FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Include(p => p.Appointments)
                .Include(p => p.Invoices)
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Appointments)
                .Include(p => p.Invoices)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
        }

        public void Update(Patient patient)
        {
            _context.Patients.Update(patient);
        }

        public void Delete(Patient patient)
        {
            _context.Patients.Remove(patient);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

      
    }
}
