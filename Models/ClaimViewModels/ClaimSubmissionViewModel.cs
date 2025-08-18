// Models/ClaimViewModels/ClaimSubmissionViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ClaimViewModels
{
    /// <summary>
    /// ViewModel for claim submission form
    /// </summary>
    public class ClaimSubmissionViewModel
    {
        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0, 744, ErrorMessage = "Hours must be between 0 and 744")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Supporting Documents")]
        public List<IFormFile> Documents { get; set; } = new List<IFormFile>();
    }
}