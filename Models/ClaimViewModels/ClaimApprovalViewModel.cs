using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ClaimViewModels
{
    /// <summary>
    /// Enhanced ViewModel for claim approval interface with Part 2 features
    /// </summary>
    public class ClaimApprovalViewModel
    {
        public int ClaimId { get; set; }

        public int LecturerId { get; set; }

        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; } = string.Empty;

        [Display(Name = "Claim Date")]
        [DataType(DataType.DateTime)]
        public DateTime ClaimDate { get; set; }

        [Display(Name = "Hours Worked")]
        [Range(0.1, 744, ErrorMessage = "Hours must be between 0.1 and 744")]
        public decimal HoursWorked { get; set; }

        [Display(Name = "Hourly Rate")]
        [Range(0.1, 9999.99, ErrorMessage = "Hourly rate must be between 0.1 and 9999.99")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Claim Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Submitted";

        [Display(Name = "Documents")]
        public List<string> DocumentNames { get; set; } = new List<string>();

        public List<DataModels.Document> Documents { get; set; } = new List<DataModels.Document>();

        [Display(Name = "Submission Comments")]
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        public string SubmissionComments { get; set; } = string.Empty;

        [Display(Name = "Approval Comments")]
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        public string ApprovalComments { get; set; } = string.Empty;

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; } = string.Empty;

        [Display(Name = "Approver Role")]
        public string ApproverRole { get; set; } = string.Empty;

        [Display(Name = "Approval Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ApprovalDate { get; set; }

        [Display(Name = "Days Pending")]
        public int DaysPending
        {
            get
            {
                return (int)(DateTime.Now - ClaimDate).TotalDays;
            }
        }

        [Display(Name = "Is Urgent")]
        public bool IsUrgent
        {
            get
            {
                return DaysPending > 7 && Status == "Submitted";
            }
        }
    }
}