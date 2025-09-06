using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ClaimViewModels
{
    /// <summary>
    /// ViewModel for claim submission form
    /// Allows editing of hourly rate and includes comments
    /// </summary>
    public class ClaimSubmissionViewModel
    {
        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0, 744, ErrorMessage = "Hours must be between 0 and 744")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0, 999.99, ErrorMessage = "Hourly rate must be between 0 and 999.99")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Comments")]
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; } = string.Empty;

        [Display(Name = "Supporting Documents")]
        public List<IFormFile> Documents { get; set; } = new List<IFormFile>();
    }
}