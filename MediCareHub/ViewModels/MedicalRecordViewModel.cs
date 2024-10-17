using System.ComponentModel.DataAnnotations;

namespace MediCareHub.ViewModels
{
    public class MedicalRecordViewModel
    {
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; }

        [Display(Name = "Medication")]
        public string Medication { get; set; }
    }

}
