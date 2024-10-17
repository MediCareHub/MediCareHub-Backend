using MediCareHub.DAL.Models;

namespace MediCareHub.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public IEnumerable<Appointment> TodayAppointments { get; set; }
        public IEnumerable<Appointment> PendingAppointments { get; set; }
        public string DoctorFullName { get; set; } // Add property for the doctor's full name

    }

}
