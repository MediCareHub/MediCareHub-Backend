using MediCareHub.DAL.Models;

namespace MediCareHub.ViewModels
{
    public class DoctorProfileViewModel
    {
        public int UserId { get; set; } // This should hold the UserId of the logged-in user
        public int DepartmentId { get; set; }
        public int ExperienceYears { get; set; }
        public string Qualification { get; set; }
        public string Description { get; set; }

    }
}
