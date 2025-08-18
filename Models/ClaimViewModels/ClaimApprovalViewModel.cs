// Models/ClaimViewModels/ClaimApprovalViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ClaimViewModels
{
    /// <summary>
    /// ViewModel for claim approval interface
    /// </summary>
    public class ClaimApprovalViewModel
    {
        public int ClaimId { get; set; }

        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; } = string.Empty;

        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; }

        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Claim Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Documents")]
        public List<string> DocumentNames { get; set; } = new List<string>();

        [Display(Name = "Approval Comments")]
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        public string? Comments { get; set; }
    }
}