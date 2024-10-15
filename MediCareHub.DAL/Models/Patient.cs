using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCareHub.DAL.Models
{
    public class Patient
    {
        [Key]
        [ForeignKey("User")]
        public int PatientId { get; set; }

        public virtual User User { get; set; }

        public string MedicalHistory { get; set; }

        public string Allergies { get; set; }

        public string CurrentMedications { get; set; }

        [StringLength(100)]
        public string EmergencyContact { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
