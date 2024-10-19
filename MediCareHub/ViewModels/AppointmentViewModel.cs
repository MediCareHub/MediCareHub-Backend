using System.ComponentModel.DataAnnotations;

namespace MediCareHub.ViewModels
{
    public class AppointmentViewModel
    {
        [Required]
        public int AppointmentId { get; set; }

        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } // "Pending", "Completed", "Canceled"

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Doctor Name")]
        public string DoctorFullName { get; set; } // To display the doctor's name

        [Display(Name = "Patient Name")]
        public string PatientFullName { get; set; } // To display the patient's name

        public MedicalRecordViewModel MedicalRecord { get; set; } // Add this line
    }
}
