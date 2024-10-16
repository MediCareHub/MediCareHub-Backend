namespace MediCareHub.ViewModels
{
    public class PatientProfileViewModel
    {
        public int UserId { get; set; } // This should hold the UserId of the logged-in user
        public string MedicalHistory { get; set; }
        public string Allergies { get; set; }
        public string CurrentMedications { get; set; }
        public string EmergencyContact { get; set; }
    }
}
