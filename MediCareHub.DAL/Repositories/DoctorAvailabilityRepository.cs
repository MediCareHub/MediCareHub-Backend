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
        {
            _context.DoctorAvailability.Add(availability);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
