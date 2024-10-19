using MediCareHub.DAL.Data.Configurations;
using MediCareHub.DAL.Models;
using MediCareHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Include(d => d.User)  // Include the related User entity
                .Include(d =>d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == userId);
        }
    }
}
