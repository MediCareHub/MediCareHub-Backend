using MediCareHub.DAL.Models;

namespace MediCareHub.DAL.Repositories.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<Doctor> GetByUserId(int userId);
        Task<IEnumerable<Doctor>> GetAllAsync();
    }

}
