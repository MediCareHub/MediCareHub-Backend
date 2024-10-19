namespace MediCareHub.ViewModels
{
    public class PatientProfileViewModel
    {
        public int UserId { get; set; }
        public string MedicalHistory { get; set; }
        public string Allergies { get; set; }
        public string CurrentMedications { get; set; }
        public string EmergencyContact { get; set; }
    }
}