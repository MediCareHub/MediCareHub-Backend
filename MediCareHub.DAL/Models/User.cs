using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCareHub.DAL.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(10)]
        [EnumDataType(typeof(RoleEnum))]
        public string Role { get; set; }

        [StringLength(15)]
        [Phone]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        [EnumDataType(typeof(GenderEnum))]
        public string Gender { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum RoleEnum
    {
        Admin,
        Doctor,
        Patient
    }

    public enum GenderEnum
    {
        Male,
        Female,
        Other
    }

}
