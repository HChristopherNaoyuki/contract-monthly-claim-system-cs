using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ViewModels
{
    /// <summary>
    /// ViewModel for HR report generation with comprehensive data
    /// Part 3 POE requirement: PDF report generation for HR
    /// </summary>
    public class HRReportViewModel
    {
        /// <summary>
        /// Gets or sets the report title
        /// </summary>
        [Display(Name = "Report Title")]
        public string ReportTitle { get; set; } = "HR Analytics Report";

        /// <summary>
        /// Gets or sets the generation date and time
        /// </summary>
        [Display(Name = "Generated At")]
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the report period
        /// </summary>
        [Display(Name = "Report Period")]
        public string ReportPeriod { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of claims
        /// </summary>
        [Display(Name = "Total Claims")]
        public int TotalClaims { get; set; }

        /// <summary>
        /// Gets or sets the number of approved claims
        /// </summary>
        [Display(Name = "Approved Claims")]
        public int ApprovedClaims { get; set; }

        /// <summary>
        /// Gets or sets the number of rejected claims
        /// </summary>
        [Display(Name = "Rejected Claims")]
        public int RejectedClaims { get; set; }

        /// <summary>
        /// Gets or sets the number of pending claims
        /// </summary>
        [Display(Name = "Pending Claims")]
        public int PendingClaims { get; set; }

        /// <summary>
        /// Gets or sets the total amount approved
        /// </summary>
        [Display(Name = "Total Amount Approved")]
        [DataType(DataType.Currency)]
        public decimal TotalAmountApproved { get; set; }

        /// <summary>
        /// Gets or sets the total amount paid
        /// </summary>
        [Display(Name = "Total Amount Paid")]
        [DataType(DataType.Currency)]
        public decimal TotalAmountPaid { get; set; }

        /// <summary>
        /// Gets or sets the approval rate percentage
        /// </summary>
        [Display(Name = "Approval Rate")]
        public decimal ApprovalRate { get; set; }

        /// <summary>
        /// Gets or sets the top performing lecturers
        /// </summary>
        [Display(Name = "Top Performing Lecturers")]
        public List<TopLecturerViewModel> TopLecturers { get; set; } = new List<TopLecturerViewModel>();

        /// <summary>
        /// Gets or sets the monthly breakdown
        /// </summary>
        [Display(Name = "Monthly Breakdown")]
        public List<MonthlyBreakdownViewModel> MonthlyBreakdown { get; set; } = new List<MonthlyBreakdownViewModel>();

        /// <summary>
        /// Gets or sets the department statistics
        /// </summary>
        [Display(Name = "Department Statistics")]
        public List<DepartmentStatViewModel> DepartmentStats { get; set; } = new List<DepartmentStatViewModel>();

        /// <summary>
        /// Gets the formatted generation date
        /// </summary>
        [Display(Name = "Formatted Date")]
        public string FormattedDate
        {
            get
            {
                return GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// Gets the report summary
        /// </summary>
        [Display(Name = "Report Summary")]
        public string ReportSummary
        {
            get
            {
                return $"This report covers {TotalClaims} total claims with an approval rate of {ApprovalRate}% and total approved amount of {TotalAmountApproved:C}.";
            }
        }
    }

    /// <summary>
    /// ViewModel for department statistics in HR reports
    /// Part 3 POE requirement: Enhanced departmental analytics
    /// </summary>
    public class DepartmentStatViewModel
    {
        /// <summary>
        /// Gets or sets the department name
        /// </summary>
        [Display(Name = "Department")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of lecturers
        /// </summary>
        [Display(Name = "Lecturer Count")]
        public int LecturerCount { get; set; }

        /// <summary>
        /// Gets or sets the total claims
        /// </summary>
        [Display(Name = "Total Claims")]
        public int TotalClaims { get; set; }

        /// <summary>
        /// Gets or sets the total amount
        /// </summary>
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the average claim amount
        /// </summary>
        [Display(Name = "Average Claim")]
        [DataType(DataType.Currency)]
        public decimal AverageClaimAmount
        {
            get
            {
                return TotalClaims > 0 ? TotalAmount / TotalClaims : 0;
            }
        }
    }
}