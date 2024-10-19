using MediCareHub.DAL.Models;

namespace MediCareHub.DAL.Repositories.Interfaces
{
    public interface IDoctorAvailabilityRepository
    {
        Task<IEnumerable<DoctorAvailability>> GetAvailableSlotsForDoctor(int doctorId);
    }
}
