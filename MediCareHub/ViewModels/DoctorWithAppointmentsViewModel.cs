using MediCareHub.ViewModels;

public class DoctorWithAppointmentsViewModel
{
    public int DoctorId { get; set; }
    public string DoctorFullName { get; set; }
    public string Specialty { get; set; }

    // Add this property for doctor availability
    public List<DoctorAvailabilityViewModel> AvailableTimeSlots { get; set; }

    public List<AppointmentViewModel> AvailableAppointments { get; set; }
}

public class DoctorAvailabilityViewModel
{
    public string DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
