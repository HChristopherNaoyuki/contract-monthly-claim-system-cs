using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ClaimViewModels
{
    /// <summary>
    /// Enhanced ViewModel for claim approval with automation features
    /// Part 3 POE requirement: Automated verification and approval processes
    /// Provides comprehensive data for claim approval workflow
    /// </summary>
    public class ClaimApprovalViewModel
    {
        /// <summary>
        /// Gets or sets the unique claim identifier
        /// Used for claim tracking and processing
        /// </summary>
        [Display(Name = "Claim ID")]
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the lecturer who submitted the claim
        /// Display property for user identification
        /// </summary>
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the claim was submitted
        /// Used for tracking and aging analysis
        /// </summary>
        [Display(Name = "Claim Date")]
        [DataType(DataType.Date)]
        public DateTime ClaimDate { get; set; }

        /// <summary>
        /// Gets or sets the number of hours worked by the lecturer
        /// Primary input for claim calculation
        /// </summary>
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        /// <summary>
        /// Gets or sets the hourly rate of the lecturer
        /// Used for amount calculation and validation
        /// </summary>
        [Display(Name = "Hourly Rate")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }

        /// <summary>
        /// Gets or sets the calculated claim amount
        /// Result of hours worked multiplied by hourly rate
        /// </summary>
        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the current status of the claim
        /// Tracks workflow progress through submission, approval, rejection, or payment
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of supporting document names
        /// Part 3 requirement: Document management for claims
        /// </summary>
        [Display(Name = "Documents")]
        public List<string> DocumentNames { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the comments provided during claim submission
        /// Additional context from the lecturer
        /// </summary>
        [Display(Name = "Submission Comments")]
        public string SubmissionComments { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the comments from approvers
        /// Audit trail for approval decisions
        /// </summary>
        [Display(Name = "Approval Comments")]
        public string ApprovalComments { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the claim has excessive hours (over 160)
        /// Part 3 automation: Automated verification flag
        /// </summary>
        [Display(Name = "Excessive Hours")]
        public bool HasExcessiveHours { get; set; }

        /// <summary>
        /// Gets or sets whether the claim amount is unusually high (over 10,000)
        /// Part 3 automation: Automated verification flag
        /// </summary>
        [Display(Name = "Unusual Amount")]
        public bool HasUnusualAmount { get; set; }

        /// <summary>
        /// Gets or sets whether the claim requires manager approval
        /// Part 3 automation: Multi-level approval workflow
        /// </summary>
        [Display(Name = "Requires Manager Approval")]
        public bool RequiresManagerApproval { get; set; }

        /// <summary>
        /// Gets the number of days the claim has been pending
        /// Calculated property for workflow efficiency tracking
        /// Part 3 requirement: Transparent status tracking
        /// </summary>
        [Display(Name = "Days Pending")]
        public int DaysPending
        {
            get
            {
                return (int)(DateTime.Now - ClaimDate).TotalDays;
            }
        }

        /// <summary>
        /// Gets the priority level based on amount and pending days
        /// Part 3 automation: Intelligent workflow prioritization
        /// </summary>
        [Display(Name = "Priority")]
        public string Priority
        {
            get
            {
                if (Amount > 10000 || DaysPending > 14)
                    return "High";
                if (Amount > 5000 || DaysPending > 7)
                    return "Medium";
                return "Low";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the claim requires attention
        /// Part 3 automation: Automated alert system
        /// </summary>
        [Display(Name = "Requires Attention")]
        public bool RequiresAttention
        {
            get
            {
                return HasExcessiveHours || HasUnusualAmount || DaysPending > 21;
            }
        }
    }
}