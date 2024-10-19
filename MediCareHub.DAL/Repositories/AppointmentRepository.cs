using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IEnumerable<Appointment>> GetAvailableAppointmentsForDoctor(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User) // Include doctor details
                .ToListAsync();
        }

        public async Task<Appointment> GetByDoctorAndDateTime(int doctorId, DateTime appointmentDate)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(a => a.DoctorId == doctorId && a.AppointmentDate == appointmentDate);
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

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsForPatientAsync(int patientId)
        {
            var currentDate = DateTime.Now;

            return await _context.Appointments
                .Where(a => a.PatientId == patientId && a.AppointmentDate > currentDate && a.Status != "Canceled")
                .Include(a => a.Doctor)   // Assuming there's a relationship with Doctor
                    .ThenInclude(d => d.User)  // Fetch the doctor's user information
                .Include(a => a.Patient)  // Include the Patient
                    .ThenInclude(p => p.User)  // Fetch the patient’s user information
                .ToListAsync() ?? new List<Appointment>();
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsForPatientAsync(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User) // Assuming Doctor has a User entity
                .Include(a => a.Patient)
                .ThenInclude(p => p.User) // Assuming Patient has a User entity
                .ToListAsync();
        }

    }
}
