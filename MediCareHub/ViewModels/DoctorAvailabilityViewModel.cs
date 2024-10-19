using MediCareHub.DAL.Models;

namespace MediCareHub.ViewModels
{
    public class DoctorAvailabilityViewModel
    {
        public int DoctorId { get; set; }
        public string DayOfWeek { get; set; }
        public List<TimeSpan> SelectedSlots { get; set; }
        public List<TimeSpan> AllTimeSlots { get; set; }

        public DoctorAvailabilityViewModel()
        {
            AllTimeSlots = new List<TimeSpan>();
            for (int i = 0; i < 24 * 4; i++) // 45-minute slots in a 24-hour day
            {
                AllTimeSlots.Add(TimeSpan.FromMinutes(i * 45));
            }
        }
    }
}
