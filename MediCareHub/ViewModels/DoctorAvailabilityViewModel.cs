namespace MediCareHub.ViewModels
{
    public class DoctorAvailabilityViewModel
    {
        public string DayOfWeek { get; set; } // e.g., Monday, Tuesday
        public TimeSpan StartTime { get; set; } // Start time for the slot
        public TimeSpan EndTime { get; set; } // End time for the slot
    }
}
