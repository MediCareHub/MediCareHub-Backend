using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediCareHub.DAL.Repositories
{
    public class DoctorAvailabilityRepository : IDoctorAvailabilityRepository
    {
        private readonly AppDbContext _context;

        public DoctorAvailabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        // Method to fetch the availability slots for a specific doctor
        public async Task<IEnumerable<DoctorAvailability>> GetAvailableSlotsForDoctor(int doctorId)
        {
            return await _context.DoctorAvailability
                .Where(da => da.DoctorId == doctorId)
                .ToListAsync() ?? new List<DoctorAvailability>(); // Return empty list if null
        }

    }
}
