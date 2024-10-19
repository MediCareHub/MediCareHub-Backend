using System.ComponentModel.DataAnnotations;

namespace MediCareHub.ViewModels
{
    public class CompleteDoctorProfileViewModel
    {


        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50.")]
        public int ExperienceYears { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        public string Qualification { get; set; }

        public string Description { get; set; }


    }
}
