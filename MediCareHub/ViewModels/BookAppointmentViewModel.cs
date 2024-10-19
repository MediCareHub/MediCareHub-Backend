namespace MediCareHub.ViewModels
{
    public class BookAppointmentViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorFullName { get; set; }
        public List<DateTime> AvailableDates { get; set; } // Dates that the doctor is available
        public Dictionary<string, List<string>> AvailableSlots { get; set; } // Change DateTime to string for compatibility
    }
}
