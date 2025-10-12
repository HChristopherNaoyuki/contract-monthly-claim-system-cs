using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ClaimViewModels
{
    public class ClaimApprovalViewModel
    {
        public int ClaimId { get; set; }

        public string LecturerName { get; set; } = string.Empty;

        public DateTime ClaimDate { get; set; }

        public decimal HoursWorked { get; set; }

        public decimal HourlyRate { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<string> DocumentNames { get; set; } = new List<string>();

        public string SubmissionComments { get; set; } = string.Empty;

        public string ApprovalComments { get; set; } = string.Empty;

        public int DaysPending
        {
            get
            {
                return (int)(DateTime.Now - ClaimDate).TotalDays;
            }
        }
    }
}