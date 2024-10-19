using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCareHub.DAL.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(AppDbContext context) : base(context)
        {
        }

        // Fetch today's appointments for a specific doctor, including User details
        public async Task<IEnumerable<Appointment>> GetTodayAppointmentsForDoctorAsync(int doctorId)
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == today)
                .Include(a => a.Patient) // Include Patient entity
                .ThenInclude(p => p.User) // Include associated User of the Patient
                .Include(a => a.Doctor) // Include Doctor entity
                .ThenInclude(d => d.User) // Include associated User of the Doctor
                .ToListAsync() ?? new List<Appointment>(); ;
        }

        // Fetch pending appointments for a specific doctor, including User details
        public async Task<IEnumerable<Appointment>> GetPendingAppointmentsForDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status == "Pending")
                .Include(a => a.Patient) // Include Patient entity
                .ThenInclude(p => p.User) // Include associated User of the Patient
                .Include(a => a.Doctor) // Include Doctor entity
                .ThenInclude(d => d.User) // Include associated User of the Doctor
                .ToListAsync() ?? new List<Appointment>(); ;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Patient) // Assuming there's a relationship with the Patient
                    .ThenInclude(p => p.User) // Assuming Patient has a User object with a name
                .Include(a => a.Doctor) // Assuming there's a relationship with the Doctor
                    .ThenInclude(d => d.User) // Assuming Doctor has a User object with a name
                .ToListAsync();
        }


        public async Task<Appointment> GetallAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient) // Assuming there's a relationship with the Patient
                    .ThenInclude(p => p.User) // Assuming Patient has a User object with a name
                .Include(a => a.Doctor) // Assuming there's a relationship with the Doctor
                    .ThenInclude(d => d.User) // Assuming Doctor has a User object with a name
                .Include(a => a.MedicalRecord) // Assuming there's a relationship with the MedicalRecord
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
                                

        }
    }
}
