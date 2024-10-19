using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MediCareHub.DAL.Repositories
{
    public class DoctorAvailabilityRepository : GenericRepository<DoctorAvailability>, IDoctorAvailabilityRepository
    {

        public DoctorAvailabilityRepository(AppDbContext context) : base(context)
        {
        }

        public void AddAvailability(DoctorAvailability availability)
        // Method to fetch the availability slots for a specific doctor
        public async Task<IEnumerable<DoctorAvailability>> GetAvailableSlotsForDoctor(int doctorId)
        {
            return await _context.DoctorAvailability
                .Where(da => da.DoctorId == doctorId)
                .ToListAsync() ?? new List<DoctorAvailability>(); // Return empty list if null
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
