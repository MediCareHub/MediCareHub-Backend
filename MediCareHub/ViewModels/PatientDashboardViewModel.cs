using MediCareHub.DAL.Models;

namespace MediCareHub.ViewModels
{
    public class PatientDashboardViewModel
    {
        public string PatientFullName { get; set; }
        public IEnumerable<Appointment> UpcomingAppointments { get; set; }
    }
}
