using MediCareHub.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCareHub.DAL.Repositories.Interfaces
{
    public interface IDoctorAvailabilityRepository
    {
        void AddAvailability(DoctorAvailability availability);
        void Save();
    }
}
