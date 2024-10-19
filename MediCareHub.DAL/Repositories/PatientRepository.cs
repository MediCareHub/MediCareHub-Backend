using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediCareHub.DAL.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(AppDbContext context) : base(context) { }

        public async Task<Patient> GetByUserId(int userId)
        {
            return await _context.Patients
                .Include(p => p.User) // Ensure you include the User entity
                .FirstOrDefaultAsync(p => p.PatientId == userId);
        }

    }
}
