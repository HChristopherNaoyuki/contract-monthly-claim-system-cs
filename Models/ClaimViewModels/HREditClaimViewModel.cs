using System;
using System.ComponentModel.DataAnnotations;
using contract_monthly_claim_system_cs.Models.DataModels;

namespace contract_monthly_claim_system_cs.Models.ClaimViewModels
{
    /// <summary>
    /// ViewModel for HR claim editing functionality
    /// Part 3 POE requirement: HR can edit submissions
    /// </summary>
    public class HREditClaimViewModel
    {
        /// <summary>
        /// Gets or sets the claim ID
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the lecturer ID
        /// </summary>
        public int LecturerId { get; set; }

        /// <summary>
        /// Gets or sets the lecturer name
        /// </summary>
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the original hours worked
        /// </summary>
        [Display(Name = "Original Hours")]
        public decimal OriginalHoursWorked { get; set; }

        /// <summary>
        /// Gets or sets the edited hours worked
        /// </summary>
        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0, 744, ErrorMessage = "Hours must be between 0 and 744")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        /// <summary>
        /// Gets or sets the original hourly rate
        /// </summary>
        [Display(Name = "Original Hourly Rate")]
        public decimal OriginalHourlyRate { get; set; }

        /// <summary>
        /// Gets or sets the edited hourly rate
        /// </summary>
        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0, 999.99, ErrorMessage = "Hourly rate must be between 0 and 999.99")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        /// <summary>
        /// Gets or sets the original amount
        /// </summary>
        [Display(Name = "Original Amount")]
        public decimal OriginalAmount { get; set; }

        /// <summary>
        /// Gets or sets the edited amount
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0, 999999.99, ErrorMessage = "Amount must be between 0 and 999999.99")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the claim status
        /// </summary>
        [Display(Name = "Status")]
        public ClaimStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the submission comments
        /// </summary>
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        [Display(Name = "Submission Comments")]
        public string SubmissionComments { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the month and year
        /// </summary>
        [Display(Name = "Month/Year")]
        public string MonthYear { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the claim date
        /// </summary>
        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; }

        /// <summary>
        /// Gets the calculated difference in hours
        /// </summary>
        [Display(Name = "Hours Difference")]
        public decimal HoursDifference
        {
            get
            {
                return HoursWorked - OriginalHoursWorked;
            }
        }

        /// <summary>
        /// Gets the calculated difference in rate
        /// </summary>
        [Display(Name = "Rate Difference")]
        public decimal RateDifference
        {
            get
            {
                return HourlyRate - OriginalHourlyRate;
            }
        }

        /// <summary>
        /// Gets the calculated difference in amount
        /// </summary>
        [Display(Name = "Amount Difference")]
        public decimal AmountDifference
        {
            get
            {
                return Amount - OriginalAmount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether any changes were made
        /// </summary>
        [Display(Name = "Has Changes")]
        public bool HasChanges
        {
            get
            {
                return HoursWorked != OriginalHoursWorked ||
                       HourlyRate != OriginalHourlyRate ||
                       Amount != OriginalAmount ||
                       !string.Equals(SubmissionComments, GetOriginalComments());
            }
        }

        /// <summary>
        /// Gets the original comments for comparison
        /// </summary>
        /// <returns>Original submission comments</returns>
        private string GetOriginalComments()
        {
            // This would typically retrieve the original comments from the claim
            // For now, we'll use the current SubmissionComments as a placeholder
            return SubmissionComments;
        }
    }
}