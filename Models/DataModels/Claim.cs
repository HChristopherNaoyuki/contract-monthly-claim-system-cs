// Models/DataModels/Claim.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents a monthly claim submitted by a lecturer
    /// </summary>
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [ForeignKey("Lecturer")]
        public int LecturerId { get; set; }
        public virtual Lecturer? Lecturer { get; set; }

        [Required(ErrorMessage = "Claim date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0, 744, ErrorMessage = "Hours must be between 0 and 744")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Display(Name = "Claim Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Claim Status")]
        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    }

    /// <summary>
    /// Enumeration for claim status values
    /// </summary>
    public enum ClaimStatus
    {
        Submitted,
        UnderReview,
        Approved,
        Rejected,
        Paid
    }
}