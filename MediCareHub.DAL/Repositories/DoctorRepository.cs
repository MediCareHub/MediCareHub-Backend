using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediCareHub.DAL.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<Doctor> GetByUserId(int userId)
        {
            return await _context.Doctors
    .Include(d => d.User)
    .Include(d => d.Department)
    .FirstOrDefaultAsync(d => d.DoctorId == userId);  // This is looking for a doctor by DoctorId, not UserId

        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)      // Include User entity
                .Include(d => d.Department) // Include Department for specialty
                .ToListAsync();
        }



    }
}
