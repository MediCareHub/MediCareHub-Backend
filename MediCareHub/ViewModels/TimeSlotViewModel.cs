namespace MediCareHub.ViewModels
{
    public class TimeSlotViewModel
    {
        public string Day { get; set; } // Day of the week (e.g., "Monday")
        public TimeSpan SlotTime { get; set; } // Slot time (e.g., 09:00 AM)
    }

}
