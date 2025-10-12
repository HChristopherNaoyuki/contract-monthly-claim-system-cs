using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents a monthly claim submitted by a lecturer
    /// </summary>
    [Table("Claims")]
    public class Claim
    {
        /// <summary>
        /// Gets or sets the unique claim identifier
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the lecturer ID who submitted the claim
        /// </summary>
        [Required(ErrorMessage = "Lecturer ID is required")]
        [ForeignKey("Lecturer")]
        public int LecturerId { get; set; }

        /// <summary>
        /// Gets or sets the claim submission date
        /// </summary>
        [Required(ErrorMessage = "Claim date is required")]
        [DataType(DataType.DateTime)]
        [Column("ClaimDate")]
        public DateTime ClaimDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the month and year for the claim (format: YYYY-MM)
        /// </summary>
        [Required(ErrorMessage = "Month year is required")]
        [StringLength(7, ErrorMessage = "Month year must be in YYYY-MM format")]
        [Column("MonthYear")]
        public string MonthYear { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hours worked
        /// </summary>
        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0, 744, ErrorMessage = "Hours must be between 0 and 744")]
        [Column("HoursWorked")]
        public decimal HoursWorked { get; set; }

        /// <summary>
        /// Gets or sets the hourly rate
        /// </summary>
        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0, 9999.99, ErrorMessage = "Hourly rate must be between 0 and 9999.99")]
        [Column("HourlyRate")]
        public decimal HourlyRate { get; set; }

        /// <summary>
        /// Gets or sets the calculated claim amount
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0, 999999.99, ErrorMessage = "Amount must be between 0 and 999999.99")]
        [Column("Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the claim status
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [Column("Status")]
        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;

        /// <summary>
        /// Gets or sets the submission comments
        /// </summary>
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        [Column("SubmissionComments")]
        public string SubmissionComments { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the last modification date
        /// </summary>
        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the associated lecturer
        /// </summary>
        public virtual Lecturer Lecturer { get; set; }

        /// <summary>
        /// Gets or sets the supporting documents
        /// </summary>
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        /// <summary>
        /// Gets or sets the approval records
        /// </summary>
        public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    }

    /// <summary>
    /// Defines the possible status values for claims
    /// </summary>
    public enum ClaimStatus
    {
        /// <summary>
        /// Claim has been submitted but not reviewed
        /// </summary>
        Submitted = 0,

        /// <summary>
        /// Claim is under review by coordinators/managers
        /// </summary>
        UnderReview = 1,

        /// <summary>
        /// Claim has been approved for payment
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Claim has been rejected
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Claim has been paid
        /// </summary>
        Paid = 4
    }
}