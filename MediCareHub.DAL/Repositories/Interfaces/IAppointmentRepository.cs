using MediCareHub.DAL.Models;

namespace MediCareHub.DAL.Repositories.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetTodayAppointmentsForDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetPendingAppointmentsForDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAvailableAppointmentsForDoctor(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsForDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsForPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsForPatientAsync(int patientId);
        Task<Appointment> GetallAsync(int appointmentId);

    }
}
