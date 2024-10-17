using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCareHub.DAL.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }


        public DateTime AppointmentDate { get; set; }

        [Required]
        [EnumDataType(typeof(AppointmentStatus))]
        public string Status { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        // Navigation property

        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual MedicalRecord MedicalRecord { get; set; }


    }

    public enum AppointmentStatus
    {
        Pending,
        Completed,
        Canceled
    }
    
}
