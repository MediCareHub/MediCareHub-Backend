using System.ComponentModel.DataAnnotations;

namespace MediCareHub.ViewModels
{
    public class MedicalRecordViewModel
    {
        [Required]
        public int RecordId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; }

        [Required]
        [Display(Name = "Medication")]
        public string Medication { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
