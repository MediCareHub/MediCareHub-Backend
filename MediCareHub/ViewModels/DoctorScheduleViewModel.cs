namespace MediCareHub.ViewModels
{
    public class DoctorScheduleViewModel
    {
        public int DoctorId { get; set; }          // Unique ID for the doctor
        public string DoctorFullName { get; set; } // Full name of the doctor
        public string Specialty { get; set; }      // Doctor's specialty (optional)

        // List of available dates (you can expand this based on your needs)
        public List<DateTime> AvailableDates { get; set; }

        // List of time slots for the selected date
        public List<TimeSlotViewModel> AvailableTimeSlots { get; set; }

        // Constructor to initialize the lists
        public DoctorScheduleViewModel()
        {
            AvailableDates = new List<DateTime>();
            AvailableTimeSlots = new List<TimeSlotViewModel>();
        }
    }


}
