using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HospitalManagementSystem.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Patient> Patients => Set<Patient>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Doctors)
                .WithOne(doc => doc.Department)
                .HasForeignKey(doc => doc.DepartmentId);

            modelBuilder.Entity<Doctor>()
                .HasMany(doc => doc.Appointments)
                .WithOne(app => app.Doctor)
                .HasForeignKey(app => app.DoctorId);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Appointments)
                .WithOne(app => app.Patient)
                .HasForeignKey(app => app.PatientId)
                .OnDelete(DeleteBehavior.Cascade);  // keep cascade here

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Invoices)
                .WithOne(inv => inv.Patient)
                .HasForeignKey(inv => inv.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 break cascade path here

            modelBuilder.Entity<Appointment>()
                .HasOne(app => app.Invoice)
                .WithOne(inv => inv.Appointment)
                .HasForeignKey<Invoice>(inv => inv.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);  // cascade from appointment → invoice
        }

    }
}
