// Models/DataModels/Approval.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents an approval record for a claim
    /// </summary>
    public class Approval
    {
        [Key]
        public int ApprovalId { get; set; }

        [ForeignKey("Claim")]
        public int ClaimId { get; set; }
        public virtual Claim? Claim { get; set; }

        [Required(ErrorMessage = "Approver role is required")]
        [Display(Name = "Approver Role")]
        public string ApproverRole { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [Display(Name = "Approval Date")]
        public DateTime ApprovalDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        public string? Comments { get; set; }
    }
}