using System.ComponentModel.DataAnnotations;

namespace MediCareHub.ViewModels
{
    public class AppointmentViewModel
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }  // "Pending", "Completed", "Canceled"

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
