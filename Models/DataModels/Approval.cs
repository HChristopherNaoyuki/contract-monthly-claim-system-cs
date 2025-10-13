using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents an approval record for a claim
    /// </summary>
    [Table("Approvals")]
    public class Approval
    {
        /// <summary>
        /// Gets or sets the unique approval identifier
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApprovalId { get; set; }

        /// <summary>
        /// Gets or sets the associated claim ID
        /// </summary>
        [Required(ErrorMessage = "Claim ID is required")]
        [ForeignKey("Claim")]
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the approver user ID
        /// </summary>
        [Required(ErrorMessage = "Approver user ID is required")]
        [ForeignKey("ApproverUser")]
        public int ApproverUserId { get; set; }

        /// <summary>
        /// Gets or sets the approver role
        /// </summary>
        [Required(ErrorMessage = "Approver role is required")]
        [Column("ApproverRole")]
        public string ApproverRole { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the approval date
        /// </summary>
        [DataType(DataType.DateTime)]
        [Column("ApprovalDate")]
        public DateTime ApprovalDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets whether the claim was approved
        /// </summary>
        [Column("IsApproved")]
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the approval comments
        /// </summary>
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        [Column("Comments")]
        public string Comments { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the approval order (1: first approval, 2: final approval)
        /// </summary>
        [Range(1, 2, ErrorMessage = "Approval order must be 1 or 2")]
        [Column("ApprovalOrder")]
        public int ApprovalOrder { get; set; } = 1;

        /// <summary>
        /// Gets or sets the associated claim
        /// </summary>
        public virtual Claim? Claim { get; set; }

        /// <summary>
        /// Gets or sets the approver user
        /// </summary>
        public virtual User? ApproverUser { get; set; }
    }
}