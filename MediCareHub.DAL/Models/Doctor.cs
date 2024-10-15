using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCareHub.DAL.Models
{
    public class Doctor
    {
        [Key]
        [ForeignKey("User")]
        public int DoctorId { get; set; }

        public virtual User User { get; set; }

        [Required]
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public int ExperienceYears { get; set; }

        [StringLength(150)]
        public string Qualification { get; set; }

        public string Description { get; set; }
    }

}
