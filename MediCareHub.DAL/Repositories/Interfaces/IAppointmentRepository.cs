using MediCareHub.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCareHub.DAL.Repositories.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetTodayAppointmentsForDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetPendingAppointmentsForDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsForDoctorAsync(int doctorId);
        Task<Appointment> GetallAsync(int appointmentId);

        // Other methods for Appointment management
    }
}
