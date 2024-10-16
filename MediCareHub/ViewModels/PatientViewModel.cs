using System.ComponentModel.DataAnnotations;

namespace MediCareHub.ViewModels
{
    public class PatientViewModel
    {
        [Required]
        public int PatientId { get; set; }

        [Display(Name = "Medical History")]
        public string MedicalHistory { get; set; }

        [Display(Name = "Allergies")]
        public string Allergies { get; set; }

        [Display(Name = "Current Medications")]
        public string CurrentMedications { get; set; }

        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; }
    }
}
