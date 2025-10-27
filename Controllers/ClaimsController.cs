using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using contract_monthly_claim_system_cs.Models.ClaimViewModels;
using contract_monthly_claim_system_cs.Models.ViewModels;
using contract_monthly_claim_system_cs.Models.DataModels;
using contract_monthly_claim_system_cs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Enhanced Claims Controller with comprehensive automation features for Part 3 POE requirements
    /// Handles claim submission, approval workflow, HR analytics, and automated processing
    /// Uses text file storage instead of database as per assignment requirements
    /// </summary>
    public class ClaimsController : Controller
    {
        private readonly TextFileDataService _dataService;
        private readonly ILogger<ClaimsController> _logger;

        /// <summary>
        /// Initializes a new instance of ClaimsController with dependency injection
        /// </summary>
        /// <param name="dataService">Data service for text file operations</param>
        /// <param name="logger">Logger instance for tracking operations and debugging</param>
        public ClaimsController(TextFileDataService dataService, ILogger<ClaimsController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        /// <summary>
        /// Displays enhanced claim submission form with auto-calculation features
        /// Part 3 requirement: Automated claim submission with validation
        /// Requires user authentication and validates lecturer status
        /// </summary>
        /// <returns>Claim submission view or redirect to authentication</returns>
        [HttpGet]
        public IActionResult Submit()
        {
            // Check if user is authenticated
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                _logger.LogWarning("Unauthorized access attempt to claim submission");
                return RedirectToAction("Index", "Auth");
            }

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var lecturer = _dataService.GetLecturerById(userId);

            // Create view model with pre-populated data
            var viewModel = new ClaimSubmissionViewModel
            {
                HourlyRate = lecturer?.HourlyRate ?? 150.00m
            };

            _logger.LogInformation("User {UserId} accessed claim submission form", userId);
            return View(viewModel);
        }

        /// <summary>
        /// Handles automated claim submission with enhanced validation and auto-calculation
        /// Part 3 automation requirement: Auto-calculation feature and validation checks
        /// Implements automated workflow for claim processing
        /// </summary>
        /// <param name="model">Claim submission data with validation</param>
        /// <returns>Redirect to status page or error view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ClaimSubmissionViewModel model)
        {
            // Validate user authentication
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                _logger.LogWarning("Unauthorized POST attempt to claim submission");
                return RedirectToAction("Index", "Auth");
            }

            // Validate model state
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                    // Part 3 Automation: Auto-calculation feature
                    model.Amount = AutoCalculateClaimAmount(model.HoursWorked, model.HourlyRate);

                    // Part 3 Automation: Enhanced validation checks
                    var validationResult = ValidateClaimSubmission(model, userId);
                    if (!validationResult.IsValid)
                    {
                        ModelState.AddModelError("", validationResult.ErrorMessage);
                        _logger.LogWarning("Claim validation failed for user {UserId}: {Error}",
                            userId, validationResult.ErrorMessage);
                        return View(model);
                    }

                    // Create new claim entity
                    var claim = new Claim
                    {
                        ClaimId = _dataService.GetNextId("claims"),
                        LecturerId = userId,
                        ClaimDate = DateTime.Now,
                        MonthYear = DateTime.Now.ToString("yyyy-MM"),
                        HoursWorked = model.HoursWorked,
                        HourlyRate = model.HourlyRate,
                        Amount = model.Amount,
                        Status = ClaimStatus.Submitted,
                        SubmissionComments = model.Comments,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    // Save claim to text file storage
                    _dataService.SaveClaim(claim);

                    // Part 3 Automation: Automated document processing
                    await ProcessUploadedDocuments(model.Documents, claim.ClaimId);

                    // Part 3 Automation: Automated notification system
                    await NotifyCoordinators(claim);

                    _logger.LogInformation("Automated claim submission successful - User {UserId}, Claim {ClaimId}, Amount {Amount}",
                        userId, claim.ClaimId, claim.Amount);

                    return RedirectToAction("Status", new { claimId = claim.ClaimId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Automated claim submission failed for user {UserId}",
                        HttpContext.Session.GetInt32("UserId"));
                    ModelState.AddModelError("", "An error occurred during claim submission. Please try again.");
                }
            }

            // Return to view with validation errors
            return View(model);
        }

        /// <summary>
        /// Enhanced approval view with automated claim verification for coordinators and managers
        /// Part 3 automation requirement: Automated verification and approval processes
        /// Provides intelligent claim analysis and workflow management
        /// </summary>
        /// <returns>Approval view with automated claim analysis</returns>
        [HttpGet]
        public IActionResult Approve()
        {
            // Validate authentication and authorization
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                _logger.LogWarning("Unauthorized access attempt to approval page");
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;
            if (userRole != UserRole.ProgrammeCoordinator.ToString() &&
                userRole != UserRole.AcademicManager.ToString())
            {
                _logger.LogWarning("User {UserId} with role {Role} attempted to access approval page",
                    HttpContext.Session.GetInt32("UserId"), userRole);
                return RedirectToAction("Index", "Home");
            }

            // Retrieve and process pending claims
            var submittedClaims = _dataService.GetAllClaims()
                .Where(c => c.Status == ClaimStatus.Submitted)
                .Select(c => CreateClaimApprovalViewModel(c, userRole))
                .OrderByDescending(c => c.RequiresManagerApproval)
                .ThenByDescending(c => c.Amount)
                .ThenBy(c => c.ClaimDate)
                .ToList();

            _logger.LogInformation("Automated approval view generated for {UserRole} - {ClaimCount} claims pending",
                userRole, submittedClaims.Count);

            return View(submittedClaims);
        }

        /// <summary>
        /// Creates a ClaimApprovalViewModel with automated verification flags
        /// Part 3 automation: Automated claim analysis for approvers
        /// </summary>
        /// <param name="claim">Claim entity to convert</param>
        /// <param name="userRole">Current user's role for permission checks</param>
        /// <returns>Populated ClaimApprovalViewModel</returns>
        private ClaimApprovalViewModel CreateClaimApprovalViewModel(Claim claim, string userRole)
        {
            return new ClaimApprovalViewModel
            {
                ClaimId = claim.ClaimId,
                LecturerName = GetLecturerName(claim.LecturerId),
                ClaimDate = claim.ClaimDate,
                HoursWorked = claim.HoursWorked,
                HourlyRate = claim.HourlyRate,
                Amount = claim.Amount,
                Status = claim.Status.ToString(),
                DocumentNames = _dataService.GetDocumentsByClaimId(claim.ClaimId)
                    .Select(d => d.FileName)
                    .ToList(),
                SubmissionComments = claim.SubmissionComments,
                // Part 3 Automation: Automated verification flags
                HasExcessiveHours = claim.HoursWorked > 160,
                HasUnusualAmount = claim.Amount > 10000,
                RequiresManagerApproval = claim.Amount > 5000 && userRole == UserRole.ProgrammeCoordinator.ToString()
            };
        }

        /// <summary>
        /// Automated claim approval/rejection with workflow management
        /// Part 3 automation requirement: Approval workflows and automated processing
        /// Handles multi-level approval system with notifications
        /// </summary>
        /// <param name="claimId">ID of claim to process</param>
        /// <param name="isApproved">Approval decision (true for approve, false for reject)</param>
        /// <param name="comments">Approval comments for audit trail</param>
        /// <returns>Redirect to approval page with status message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveClaim(int claimId, bool isApproved, string comments)
        {
            // Validate authentication
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                _logger.LogWarning("Unauthorized approval attempt for claim {ClaimId}", claimId);
                return RedirectToAction("Index", "Auth");
            }

            // Retrieve claim from text file storage
            var claim = _dataService.GetClaimById(claimId);
            if (claim != null)
            {
                // Part 3 Automation: Automated status update
                claim.Status = isApproved ? ClaimStatus.Approved : ClaimStatus.Rejected;
                claim.ModifiedDate = DateTime.Now;
                _dataService.SaveClaim(claim);

                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

                // Create approval record for audit trail
                var approval = new Approval
                {
                    ApprovalId = _dataService.GetNextId("approvals"),
                    ClaimId = claimId,
                    ApproverUserId = userId,
                    ApproverRole = userRole,
                    ApprovalDate = DateTime.Now,
                    IsApproved = isApproved,
                    Comments = comments,
                    ApprovalOrder = GetNextApprovalOrder(claimId)
                };

                _dataService.SaveApproval(approval);

                // Part 3 Automation: Automated notification to lecturer
                await NotifyLecturerOfDecision(claim, isApproved, comments);

                // Part 3 Automation: Automated reporting for HR on final approval
                if (isApproved && userRole == UserRole.AcademicManager.ToString())
                {
                    await GenerateHRReport(claim);
                }

                var action = isApproved ? "approved" : "rejected";
                _logger.LogInformation("Automated claim {Action}: Claim {ClaimId} by user {UserId} with role {Role}",
                    action, claimId, userId, userRole);

                TempData["SuccessMessage"] = $"Claim #{claimId} has been {action} successfully.";
            }
            else
            {
                _logger.LogWarning("Claim {ClaimId} not found for approval processing", claimId);
                TempData["ErrorMessage"] = $"Claim #{claimId} not found.";
            }

            return RedirectToAction("Approve");
        }

        /// <summary>
        /// Enhanced HR dashboard with automated data management and reporting
        /// Part 3 automation requirement: HR view automation and data management
        /// Provides comprehensive analytics and reporting features
        /// </summary>
        /// <returns>HR dashboard view with automated analytics</returns>
        [HttpGet]
        public IActionResult HRDashboard()
        {
            // Validate authentication and authorization (Academic Managers only)
            if (HttpContext.Session.GetInt32("UserId") == null ||
                HttpContext.Session.GetString("Role") != UserRole.AcademicManager.ToString())
            {
                _logger.LogWarning("Unauthorized access attempt to HR dashboard");
                return RedirectToAction("Index", "Home");
            }

            // Retrieve all claims for analytics
            var allClaims = _dataService.GetAllClaims();
            var approvedClaims = allClaims.Where(c => c.Status == ClaimStatus.Approved).ToList();
            var paidClaims = allClaims.Where(c => c.Status == ClaimStatus.Paid).ToList();
            var submittedClaims = allClaims.Where(c => c.Status == ClaimStatus.Submitted).ToList();

            // Part 3 Automation: Create comprehensive HR dashboard view model
            var hrViewModel = new HRDashboardViewModel
            {
                TotalClaims = allClaims.Count,
                ApprovedClaims = approvedClaims.Count,
                PaidClaims = paidClaims.Count,
                TotalAmountApproved = approvedClaims.Sum(c => c.Amount),
                TotalAmountPaid = paidClaims.Sum(c => c.Amount),
                PendingApprovalCount = submittedClaims.Count,
                AverageClaimAmount = approvedClaims.Any() ? approvedClaims.Average(c => c.Amount) : 0,
                // Part 3 Automation: Advanced analytics
                TopLecturers = GetTopPerformingLecturers(approvedClaims),
                MonthlyBreakdown = GetMonthlyClaimBreakdown(approvedClaims),
                GeneratedAt = DateTime.Now
            };

            _logger.LogInformation("HR dashboard generated with automated analytics - {TotalClaims} total claims",
                allClaims.Count);

            return View(hrViewModel);
        }

        /// <summary>
        /// Displays detailed claim status with tracking information
        /// Part 3 requirement: Claim status tracking transparency
        /// </summary>
        /// <param name="claimId">ID of the claim to display</param>
        /// <returns>Claim status view with detailed information</returns>
        [HttpGet]
        public IActionResult Status(int claimId)
        {
            // Validate authentication
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            // Retrieve claim from text file storage
            var claim = _dataService.GetClaimById(claimId);
            if (claim == null)
            {
                _logger.LogWarning("Claim {ClaimId} not found for status display", claimId);
                TempData["ErrorMessage"] = $"Claim #{claimId} not found.";
                return RedirectToAction("Track");
            }

            // Create detailed status view model
            var viewModel = CreateClaimApprovalViewModel(claim, HttpContext.Session.GetString("Role") ?? string.Empty);

            // Get approval history and comments
            var approvals = _dataService.GetApprovalsByClaimId(claimId);
            if (approvals.Any())
            {
                viewModel.ApprovalComments = string.Join("; ",
                    approvals.Where(a => !string.IsNullOrEmpty(a.Comments))
                            .Select(a => $"{a.ApproverRole}: {a.Comments}"));
            }

            _logger.LogInformation("Status displayed for claim {ClaimId} with status {Status}",
                claimId, claim.Status);

            return View(viewModel);
        }

        /// <summary>
        /// Displays claim tracking for users with role-based filtering
        /// Part 3 requirement: Transparent claim status tracking
        /// </summary>
        /// <returns>Claim tracking view with filtered claims</returns>
        [HttpGet]
        public IActionResult Track()
        {
            // Validate authentication
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;

            List<Claim> claims;

            // Part 3 Automation: Role-based data filtering
            if (userRole == UserRole.Lecturer.ToString())
            {
                claims = _dataService.GetClaimsByLecturerId(userId);
            }
            else
            {
                claims = _dataService.GetAllClaims();
            }

            // Create tracking view models
            var viewModels = claims.Select(c => CreateClaimApprovalViewModel(c, userRole)).ToList();

            _logger.LogInformation("Tracking view generated for user {UserId} with role {Role} - {ClaimCount} claims",
                userId, userRole, claims.Count);

            return View(viewModels);
        }

        #region Part 3 Automation Methods

        /// <summary>
        /// Automated claim amount calculation with overtime consideration
        /// Part 3 automation requirement: Auto-calculation feature
        /// </summary>
        /// <param name="hoursWorked">Hours worked by lecturer</param>
        /// <param name="hourlyRate">Lecturer's hourly rate</param>
        /// <returns>Calculated claim amount with overtime if applicable</returns>
        private decimal AutoCalculateClaimAmount(decimal hoursWorked, decimal hourlyRate)
        {
            // Basic calculation
            var amount = hoursWorked * hourlyRate;

            // Part 3 Automation: Overtime calculation
            if (hoursWorked > 160) // More than 160 hours per month (standard full-time)
            {
                var overtimeHours = hoursWorked - 160;
                var overtimeRate = hourlyRate * 1.5m; // Time and a half for overtime
                amount = (160 * hourlyRate) + (overtimeHours * overtimeRate);

                _logger.LogDebug("Overtime calculated: {RegularHours} regular + {OvertimeHours} overtime",
                    160, overtimeHours);
            }

            return Math.Round(amount, 2);
        }

        /// <summary>
        /// Enhanced claim validation with automated business rules
        /// Part 3 automation requirement: Validation checks for accurate data entry
        /// </summary>
        /// <param name="model">Claim submission data</param>
        /// <param name="userId">User ID for validation context</param>
        /// <returns>Validation result with error message if invalid</returns>
        private (bool IsValid, string ErrorMessage) ValidateClaimSubmission(ClaimSubmissionViewModel model, int userId)
        {
            // Maximum hours validation (744 hours in a 31-day month)
            if (model.HoursWorked > 744)
            {
                return (false, "Hours worked cannot exceed 744 hours per month.");
            }

            // Minimum hours validation
            if (model.HoursWorked <= 0)
            {
                return (false, "Hours worked must be greater than 0.");
            }

            // Rate validation
            if (model.HourlyRate > 500) // Maximum hourly rate constraint
            {
                return (false, "Hourly rate exceeds maximum allowed amount.");
            }

            if (model.HourlyRate <= 0)
            {
                return (false, "Hourly rate must be greater than 0.");
            }

            // Monthly submission limit validation
            var currentMonthClaims = _dataService.GetAllClaims()
                .Where(c => c.LecturerId == userId &&
                           c.MonthYear == DateTime.Now.ToString("yyyy-MM"))
                .Count();

            if (currentMonthClaims >= 3) // Maximum 3 claims per month
            {
                return (false, "Maximum of 3 claims allowed per month.");
            }

            // Amount validation
            var calculatedAmount = AutoCalculateClaimAmount(model.HoursWorked, model.HourlyRate);
            if (calculatedAmount > 50000) // Maximum claim amount
            {
                return (false, "Claim amount exceeds maximum allowed limit.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Automated document processing with validation and secure storage
        /// Part 3 automation requirement: Document upload and management
        /// </summary>
        /// <param name="documents">Uploaded document files</param>
        /// <param name="claimId">Claim ID for document association</param>
        private async Task ProcessUploadedDocuments(List<IFormFile> documents, int claimId)
        {
            if (documents != null && documents.Count > 0)
            {
                foreach (var file in documents)
                {
                    if (file.Length > 0)
                    {
                        // Validate file size (5MB limit)
                        if (file.Length > 5 * 1024 * 1024)
                        {
                            _logger.LogWarning("File {FileName} exceeds size limit for claim {ClaimId}",
                                file.FileName, claimId);
                            continue;
                        }

                        // Validate file type
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            _logger.LogWarning("Invalid file type {FileType} for claim {ClaimId}",
                                fileExtension, claimId);
                            continue;
                        }

                        // Create uploads directory if it doesn't exist
                        var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsDirectory))
                        {
                            Directory.CreateDirectory(uploadsDirectory);
                            _logger.LogInformation("Created uploads directory: {UploadsDirectory}", uploadsDirectory);
                        }

                        // Generate secure file name
                        var fileName = $"{claimId}_{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        var filePath = Path.Combine(uploadsDirectory, fileName);

                        // Save the file asynchronously
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create document record
                        var document = new Document
                        {
                            DocumentId = _dataService.GetNextId("documents"),
                            ClaimId = claimId,
                            FileName = file.FileName,
                            FilePath = $"/uploads/{fileName}",
                            FileSize = file.Length,
                            FileType = fileExtension,
                            UploadDate = DateTime.Now,
                            IsActive = true
                        };

                        _dataService.SaveDocument(document);
                        _logger.LogInformation("Document {FileName} processed for claim {ClaimId}",
                            file.FileName, claimId);
                    }
                }
            }
        }

        /// <summary>
        /// Automated notification system for coordinators about new claims
        /// Part 3 automation requirement: Workflow notifications
        /// </summary>
        /// <param name="claim">Newly submitted claim</param>
        private async Task NotifyCoordinators(Claim claim)
        {
            try
            {
                var coordinators = _dataService.GetAllUsers()
                    .Where(u => u.Role == UserRole.ProgrammeCoordinator && u.IsActive)
                    .ToList();

                // In a real system, this would send emails or other notifications
                foreach (var coordinator in coordinators)
                {
                    _logger.LogInformation("Notification sent to coordinator {CoordinatorName} for claim {ClaimId}",
                        coordinator.Username, claim.ClaimId);
                }

                await Task.CompletedTask; // Simulate async operation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notifications for claim {ClaimId}", claim.ClaimId);
            }
        }

        /// <summary>
        /// Automated notification to lecturer about claim decision
        /// Part 3 automation requirement: User communication and transparency
        /// </summary>
        /// <param name="claim">Processed claim</param>
        /// <param name="isApproved">Approval status</param>
        /// <param name="comments">Decision comments for context</param>
        private async Task NotifyLecturerOfDecision(Claim claim, bool isApproved, string comments)
        {
            try
            {
                var lecturer = _dataService.GetUserById(claim.LecturerId);
                if (lecturer != null)
                {
                    var status = isApproved ? "approved" : "rejected";
                    _logger.LogInformation("Decision notification sent to lecturer {LecturerName} - Claim {ClaimId} {Status}",
                        lecturer.Username, claim.ClaimId, status);
                }

                await Task.CompletedTask; // Simulate async operation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send decision notification for claim {ClaimId}", claim.ClaimId);
            }
        }

        /// <summary>
        /// Automated HR report generation for approved claims
        /// Part 3 automation requirement: Automated reporting and data management
        /// </summary>
        /// <param name="claim">Approved claim for reporting</param>
        private async Task GenerateHRReport(Claim claim)
        {
            try
            {
                var lecturer = _dataService.GetUserById(claim.LecturerId);
                var lecturerDetails = _dataService.GetLecturerById(claim.LecturerId);

                // Generate comprehensive report data
                var reportData = new
                {
                    ClaimId = claim.ClaimId,
                    LecturerName = $"{lecturer?.Name} {lecturer?.Surname}",
                    EmployeeNumber = lecturerDetails?.EmployeeNumber,
                    Department = lecturerDetails?.Department,
                    Amount = claim.Amount,
                    HoursWorked = claim.HoursWorked,
                    HourlyRate = claim.HourlyRate,
                    ApprovalDate = DateTime.Now,
                    BankDetails = new
                    {
                        AccountNumber = lecturerDetails?.BankAccountNumber,
                        BankName = lecturerDetails?.BankName
                    },
                    TaxInformation = new
                    {
                        TaxNumber = lecturerDetails?.TaxNumber
                    }
                };

                _logger.LogInformation("HR report generated for claim {ClaimId} - Amount: {Amount}",
                    claim.ClaimId, claim.Amount);

                await Task.CompletedTask; // Simulate async report generation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate HR report for claim {ClaimId}", claim.ClaimId);
            }
        }

        /// <summary>
        /// Gets the next approval order for workflow management
        /// Part 3 automation requirement: Multi-level approval workflow
        /// </summary>
        /// <param name="claimId">Claim ID for order calculation</param>
        /// <returns>Next approval order number</returns>
        private int GetNextApprovalOrder(int claimId)
        {
            var existingApprovals = _dataService.GetApprovalsByClaimId(claimId);
            return existingApprovals.Count + 1;
        }

        /// <summary>
        /// Automated analytics for top performing lecturers
        /// Part 3 automation requirement: Performance analytics and reporting
        /// </summary>
        /// <param name="approvedClaims">List of approved claims for analysis</param>
        /// <returns>List of top lecturers with performance metrics</returns>
        private List<TopLecturerViewModel> GetTopPerformingLecturers(List<Claim> approvedClaims)
        {
            return approvedClaims
                .GroupBy(c => c.LecturerId)
                .Select(g => new TopLecturerViewModel
                {
                    LecturerId = g.Key,
                    LecturerName = GetLecturerName(g.Key),
                    TotalAmount = g.Sum(c => c.Amount),
                    ClaimCount = g.Count(),
                    Department = _dataService.GetLecturerById(g.Key)?.Department ?? "Unknown"
                })
                .OrderByDescending(l => l.TotalAmount)
                .Take(5) // Top 5 performers
                .ToList();
        }

        /// <summary>
        /// Automated monthly breakdown analysis for trend reporting
        /// Part 3 automation requirement: Financial reporting and trend analysis
        /// </summary>
        /// <param name="approvedClaims">List of approved claims for breakdown</param>
        /// <returns>Monthly breakdown data for analytics</returns>
        private List<MonthlyBreakdownViewModel> GetMonthlyClaimBreakdown(List<Claim> approvedClaims)
        {
            return approvedClaims
                .GroupBy(c => c.MonthYear)
                .Select(g => new MonthlyBreakdownViewModel
                {
                    MonthYear = g.Key,
                    TotalAmount = g.Sum(c => c.Amount),
                    ClaimCount = g.Count()
                })
                .OrderBy(m => m.MonthYear)
                .ToList();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets formatted lecturer name for display purposes
        /// </summary>
        /// <param name="lecturerId">Lecturer ID for name lookup</param>
        /// <returns>Formatted lecturer full name</returns>
        private string GetLecturerName(int lecturerId)
        {
            var user = _dataService.GetUserById(lecturerId);
            return user != null ? $"{user.Name} {user.Surname}" : "Unknown Lecturer";
        }

        #endregion
    }
}