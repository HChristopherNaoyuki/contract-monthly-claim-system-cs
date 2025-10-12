using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents an Independent Contractor Lecturer with extended information
    /// </summary>
    [Table("Lecturers")]
    public class Lecturer
    {
        /// <summary>
        /// Gets or sets the lecturer ID (matches UserId)
        /// </summary>
        [Key]
        [ForeignKey("User")]
        public int LecturerId { get; set; }

        /// <summary>
        /// Gets or sets the employee number
        /// </summary>
        [StringLength(20, ErrorMessage = "Employee number cannot exceed 20 characters")]
        [Column("EmployeeNumber")]
        public string EmployeeNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the department name
        /// </summary>
        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
        [Column("Department")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hourly rate for claims
        /// </summary>
        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0, 9999.99, ErrorMessage = "Hourly rate must be between 0 and 9999.99")]
        [Column("HourlyRate")]
        public decimal HourlyRate { get; set; }

        /// <summary>
        /// Gets or sets the contract start date
        /// </summary>
        [Column("ContractStartDate")]
        public DateTime? ContractStartDate { get; set; }

        /// <summary>
        /// Gets or sets the contract end date
        /// </summary>
        [Column("ContractEndDate")]
        public DateTime? ContractEndDate { get; set; }

        /// <summary>
        /// Gets or sets the bank account number
        /// </summary>
        [StringLength(50, ErrorMessage = "Bank account number cannot exceed 50 characters")]
        [Column("BankAccountNumber")]
        public string BankAccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the bank name
        /// </summary>
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters")]
        [Column("BankName")]
        public string BankName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the tax number
        /// </summary>
        [StringLength(50, ErrorMessage = "Tax number cannot exceed 50 characters")]
        [Column("TaxNumber")]
        public string TaxNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the associated user
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the claims submitted by this lecturer
        /// </summary>
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}