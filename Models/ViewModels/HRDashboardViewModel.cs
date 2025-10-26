using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ViewModels
{
    /// <summary>
    /// ViewModel for HR Dashboard with automated analytics and reporting
    /// Part 3 POE requirement for enhanced HR functionality and automation
    /// Provides comprehensive data for the HR analytics dashboard view
    /// </summary>
    public class HRDashboardViewModel
    {
        /// <summary>
        /// Gets or sets the total number of claims submitted in the system
        /// Used for overall system metrics and reporting
        /// </summary>
        [Display(Name = "Total Claims")]
        public int TotalClaims { get; set; }

        /// <summary>
        /// Gets or sets the number of claims that have been approved
        /// Indicates workflow progress and approval rate
        /// </summary>
        [Display(Name = "Approved Claims")]
        public int ApprovedClaims { get; set; }

        /// <summary>
        /// Gets or sets the number of claims that have been marked as paid
        /// Tracks payment processing completion
        /// </summary>
        [Display(Name = "Paid Claims")]
        public int PaidClaims { get; set; }

        /// <summary>
        /// Gets or sets the total monetary amount of all approved claims
        /// Used for financial reporting and budget tracking
        /// </summary>
        [Display(Name = "Total Amount Approved")]
        [DataType(DataType.Currency)]
        public decimal TotalAmountApproved { get; set; }

        /// <summary>
        /// Gets or sets the total monetary amount of all paid claims
        /// Tracks actual expenditure vs approved amounts
        /// </summary>
        [Display(Name = "Total Amount Paid")]
        [DataType(DataType.Currency)]
        public decimal TotalAmountPaid { get; set; }

        /// <summary>
        /// Gets or sets the number of claims awaiting approval
        /// Indicates current workload for coordinators and managers
        /// </summary>
        [Display(Name = "Pending Approval")]
        public int PendingApprovalCount { get; set; }

        /// <summary>
        /// Gets or sets the average amount per approved claim
        /// Provides insights into claim patterns and amounts
        /// Part 3 automation requirement for financial analytics
        /// </summary>
        [Display(Name = "Average Claim Amount")]
        [DataType(DataType.Currency)]
        public decimal AverageClaimAmount { get; set; }

        /// <summary>
        /// Gets or sets the list of top performing lecturers based on claim amounts
        /// Part 3 automation requirement for performance analytics
        /// Used for recognition and performance management
        /// </summary>
        [Display(Name = "Top Performing Lecturers")]
        public List<TopLecturerViewModel> TopLecturers { get; set; } = new List<TopLecturerViewModel>();

        /// <summary>
        /// Gets or sets the monthly breakdown of claims and amounts
        /// Part 3 automation requirement for trend analysis and reporting
        /// Enables temporal analysis of claim patterns
        /// </summary>
        [Display(Name = "Monthly Breakdown")]
        public List<MonthlyBreakdownViewModel> MonthlyBreakdown { get; set; } = new List<MonthlyBreakdownViewModel>();

        /// <summary>
        /// Gets or sets the timestamp when this dashboard data was generated
        /// Ensures data freshness and update tracking
        /// </summary>
        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets the approval rate as a percentage
        /// Calculated property for quick performance insights
        /// Part 3 automation requirement for KPI tracking
        /// </summary>
        [Display(Name = "Approval Rate")]
        public decimal ApprovalRate
        {
            get
            {
                if (TotalClaims == 0) return 0;
                return Math.Round((decimal)ApprovedClaims / TotalClaims * 100, 2);
            }
        }

        /// <summary>
        /// Gets the payment processing rate as a percentage
        /// Tracks efficiency of payment processing workflow
        /// Part 3 automation requirement for process optimization
        /// </summary>
        [Display(Name = "Payment Processing Rate")]
        public decimal PaymentProcessingRate
        {
            get
            {
                if (ApprovedClaims == 0) return 0;
                return Math.Round((decimal)PaidClaims / ApprovedClaims * 100, 2);
            }
        }
    }

    /// <summary>
    /// ViewModel for top performing lecturers analytics
    /// Part 3 POE requirement for performance tracking and recognition
    /// Provides data for lecturer performance comparisons
    /// </summary>
    public class TopLecturerViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lecturer
        /// Used for data linking and detailed reporting
        /// </summary>
        public int LecturerId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the lecturer
        /// Display property for user identification
        /// </summary>
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total monetary amount of all claims by this lecturer
        /// Primary metric for performance ranking
        /// </summary>
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the total number of claims submitted by this lecturer
        /// Secondary metric for activity level tracking
        /// </summary>
        [Display(Name = "Claim Count")]
        public int ClaimCount { get; set; }

        /// <summary>
        /// Gets the average amount per claim for this lecturer
        /// Calculated property for performance analysis
        /// Part 3 automation requirement for detailed analytics
        /// </summary>
        [Display(Name = "Average per Claim")]
        [DataType(DataType.Currency)]
        public decimal AveragePerClaim
        {
            get
            {
                if (ClaimCount == 0) return 0;
                return Math.Round(TotalAmount / ClaimCount, 2);
            }
        }

        /// <summary>
        /// Gets or sets the department of the lecturer
        /// Enables departmental analysis and comparisons
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Gets the performance rating based on total amount and claim count
        /// Part 3 automation requirement for automated performance scoring
        /// </summary>
        [Display(Name = "Performance Rating")]
        public string PerformanceRating
        {
            get
            {
                if (TotalAmount > 20000) return "Excellent";
                if (TotalAmount > 10000) return "Very Good";
                if (TotalAmount > 5000) return "Good";
                return "Standard";
            }
        }
    }

    /// <summary>
    /// ViewModel for monthly claim breakdown analytics
    /// Part 3 POE requirement for temporal analysis and trend reporting
    /// Enables month-over-month performance tracking
    /// </summary>
    public class MonthlyBreakdownViewModel
    {
        /// <summary>
        /// Gets or sets the month and year in YYYY-MM format
        /// Standardized format for consistent sorting and display
        /// </summary>
        [Display(Name = "Month")]
        public string MonthYear { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total monetary amount for the month
        /// Primary financial metric for monthly reporting
        /// </summary>
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the number of claims processed in the month
        /// Activity level metric for workload analysis
        /// </summary>
        [Display(Name = "Claims Processed")]
        public int ClaimCount { get; set; }

        /// <summary>
        /// Gets the formatted month name for display purposes
        /// User-friendly display property
        /// Part 3 automation requirement for enhanced UX
        /// </summary>
        [Display(Name = "Month Name")]
        public string MonthName
        {
            get
            {
                if (DateTime.TryParse(MonthYear + "-01", out DateTime date))
                {
                    return date.ToString("MMMM yyyy");
                }
                return MonthYear;
            }
        }

        /// <summary>
        /// Gets the average claim amount for the month
        /// Calculated property for monthly performance analysis
        /// </summary>
        [Display(Name = "Monthly Average")]
        [DataType(DataType.Currency)]
        public decimal MonthlyAverage
        {
            get
            {
                if (ClaimCount == 0) return 0;
                return Math.Round(TotalAmount / ClaimCount, 2);
            }
        }

        /// <summary>
        /// Gets the trend indicator based on previous month comparison
        /// Part 3 automation requirement for trend analysis
        /// Simple trend calculation for dashboard display
        /// </summary>
        [Display(Name = "Trend")]
        public string Trend
        {
            get
            {
                // This would typically compare with previous month data
                // For now, using claim count as a simple indicator
                if (ClaimCount > 8) return "📈"; // Up trend
                if (ClaimCount > 4) return "➡️";  // Stable
                return "📉"; // Down trend
            }
        }
    }
}