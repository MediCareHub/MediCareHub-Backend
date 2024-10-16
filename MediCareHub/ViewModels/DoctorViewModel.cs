using System.ComponentModel.DataAnnotations;

namespace MediCareHub.ViewModels
{
    public class DoctorViewModel
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Years of Experience")]
        public int ExperienceYears { get; set; }

        [Display(Name = "Qualification")]
        public string Qualification { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
