// Models/DataModels/Lecturer.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents an Independent Contractor Lecturer
    /// </summary>
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0, 999.99, ErrorMessage = "Hourly rate must be between 0 and 999.99")]
        public decimal HourlyRate { get; set; }

        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}