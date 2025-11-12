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
using System.Text;
using contract_monthly_claim_system_cs.Extensions;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Enhanced Claims Controller with comprehensive automation features for Part 3 POE requirements
    /// Handles claim submission, approval workflow, HR analytics, automated processing, and PDF report generation
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
            try
            {
                // Check if user is authenticated
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _logger.LogWarning("Unauthorized access attempt to claim submission");
                    TempData["ErrorMessage"] = "Please log in to submit claims.";
                    return RedirectToAction("Index", "Auth");
                }

                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;

                // Only lecturers can submit claims
                if (userRole != UserRole.Lecturer.ToString())
                {
                    _logger.LogWarning("User {UserId} with role {Role} attempted to submit claims", userId, userRole);
                    TempData["ErrorMessage"] = "Only lecturers can submit claims.";
                    return RedirectToAction("Index", "Home");
                }

                var lecturer = _dataService.GetLecturerById(userId);

                // Create view model with pre-populated data
                var viewModel = new ClaimSubmissionViewModel
                {
                    HourlyRate = lecturer?.HourlyRate ?? 150.00m
                };

                _logger.LogInformation("User {UserId} accessed claim submission form", userId);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claim submission form");
                TempData["ErrorMessage"] = "An error occurred while loading the claim submission form.";
                return RedirectToAction("Index", "Home");
            }
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
            try
            {
                // Validate user authentication
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _logger.LogWarning("Unauthorized POST attempt to claim submission");
                    TempData["ErrorMessage"] = "Please log in to submit claims.";
                    return RedirectToAction("Index", "Auth");
                }

                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;

                // Only lecturers can submit claims
                if (userRole != UserRole.Lecturer.ToString())
                {
                    _logger.LogWarning("User {UserId} with role {Role} attempted to submit claims", userId, userRole);
                    TempData["ErrorMessage"] = "Only lecturers can submit claims.";
                    return RedirectToAction("Index", "Home");
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Claim submission model validation failed");
                    return View(model);
                }

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

                TempData["SuccessMessage"] = $"Claim #{claim.ClaimId} submitted successfully! Amount: {claim.Amount:C}";
                return RedirectToAction("Status", new { claimId = claim.ClaimId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Automated claim submission failed for user {UserId}",
                    HttpContext.Session.GetInt32("UserId"));
                ModelState.AddModelError("", "An error occurred during claim submission. Please try again.");
                return View(model);
            }
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
            try
            {
                // Validate authentication and authorization
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _logger.LogWarning("Unauthorized access attempt to approval page");
                    TempData["ErrorMessage"] = "Please log in to access approval features.";
                    return RedirectToAction("Index", "Auth");
                }

                var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;
                if (userRole != UserRole.ProgrammeCoordinator.ToString() &&
                    userRole != UserRole.AcademicManager.ToString() &&
                    userRole != UserRole.HumanResource.ToString())
                {
                    _logger.LogWarning("User {UserId} with role {Role} attempted to access approval page",
                        HttpContext.Session.GetInt32("UserId"), userRole);
                    TempData["ErrorMessage"] = "Access denied. Approval features are only available for Programme Coordinators, Academic Managers, and Human Resources.";
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading approval page");
                TempData["ErrorMessage"] = "An error occurred while loading the approval page.";
                return RedirectToAction("Index", "Home");
            }
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
            try
            {
                // Validate authentication
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _logger.LogWarning("Unauthorized approval attempt for claim {ClaimId}", claimId);
                    TempData["ErrorMessage"] = "Please log in to approve claims.";
                    return RedirectToAction("Index", "Auth");
                }

                // Retrieve claim from text file storage
                var claim = _dataService.GetClaimById(claimId);
                if (claim == null)
                {
                    _logger.LogWarning("Claim {ClaimId} not found for approval processing", claimId);
                    TempData["ErrorMessage"] = $"Claim #{claimId} not found.";
                    return RedirectToAction("Approve");
                }

                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

                // Part 3 Automation: Automated status update
                claim.Status = isApproved ? ClaimStatus.Approved : ClaimStatus.Rejected;
                claim.ModifiedDate = DateTime.Now;
                _dataService.SaveClaim(claim);

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
                if (isApproved && (userRole == UserRole.AcademicManager.ToString() || userRole == UserRole.HumanResource.ToString()))
                {
                    await GenerateHRReport(claim);
                }

                var action = isApproved ? "approved" : "rejected";
                _logger.LogInformation("Automated claim {Action}: Claim {ClaimId} by user {UserId} with role {Role}",
                    action, claimId, userId, userRole);

                TempData["SuccessMessage"] = $"Claim #{claimId} has been {action} successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing claim approval for claim {ClaimId}", claimId);
                TempData["ErrorMessage"] = $"An error occurred while processing claim #{claimId}.";
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
            try
            {
                // Validate authentication and authorization (Academic Managers and Human Resources only)
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _logger.LogWarning("Unauthenticated access attempt to HR dashboard");
                    TempData["ErrorMessage"] = "Please log in to access HR analytics.";
                    return RedirectToAction("Index", "Auth");
                }

                var userRole = HttpContext.Session.GetString("Role");
                if (userRole != UserRole.AcademicManager.ToString() && userRole != UserRole.HumanResource.ToString())
                {
                    _logger.LogWarning("Unauthorized access attempt to HR dashboard by user with role: {UserRole}", userRole);
                    TempData["ErrorMessage"] = "Access denied. HR Dashboard is only available for Academic Managers and Human Resources.";
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating HR dashboard");
                TempData["ErrorMessage"] = "An error occurred while generating the HR dashboard. Please try again.";
                return RedirectToAction("Index", "Home");
            }
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
            try
            {
                // Validate authentication
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    TempData["ErrorMessage"] = "Please log in to view claim status.";
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

                // Validate user access to this claim
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;

                if (userRole == UserRole.Lecturer.ToString() && claim.LecturerId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access claim {ClaimId} belonging to another lecturer", userId, claimId);
                    TempData["ErrorMessage"] = "Access denied. You can only view your own claims.";
                    return RedirectToAction("Track");
                }

                // Create detailed status view model
                var viewModel = CreateClaimApprovalViewModel(claim, userRole);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claim status for claim {ClaimId}", claimId);
                TempData["ErrorMessage"] = "An error occurred while loading claim status.";
                return RedirectToAction("Track");
            }
        }

        /// <summary>
        /// Displays claim tracking for users with role-based filtering
        /// Part 3 requirement: Transparent claim status tracking
        /// Fixed: Lecturers can now see their own claims in Track view
        /// </summary>
        /// <returns>Claim tracking view with filtered claims</returns>
        [HttpGet]
        public IActionResult Track()
        {
            try
            {
                // Validate authentication
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    TempData["ErrorMessage"] = "Please log in to track claims.";
                    return RedirectToAction("Index", "Auth");
                }

                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;

                List<Claim> claims;

                // Part 3 Automation: Role-based data filtering
                if (userRole == UserRole.Lecturer.ToString())
                {
                    // Fixed: Lecturers should only see their own claims
                    claims = _dataService.GetClaimsByLecturerId(userId);
                    _logger.LogInformation("Lecturer {UserId} viewing their {ClaimCount} claims", userId, claims.Count);
                }
                else if (userRole == UserRole.ProgrammeCoordinator.ToString() ||
                         userRole == UserRole.AcademicManager.ToString() ||
                         userRole == UserRole.HumanResource.ToString())
                {
                    // Coordinators, Managers, and HR can see all claims
                    claims = _dataService.GetAllClaims();
                    _logger.LogInformation("{UserRole} {UserId} viewing all {ClaimCount} claims", userRole, userId, claims.Count);
                }
                else
                {
                    // Default to user's own claims for unknown roles
                    claims = _dataService.GetClaimsByLecturerId(userId);
                    _logger.LogWarning("Unknown role {UserRole} for user {UserId}, defaulting to own claims", userRole, userId);
                }

                // Create tracking view models
                var viewModels = claims.Select(c => CreateClaimApprovalViewModel(c, userRole))
                                      .OrderByDescending(c => c.ClaimDate)
                                      .ToList();

                _logger.LogInformation("Tracking view generated for user {UserId} with role {Role} - {ClaimCount} claims",
                    userId, userRole, claims.Count);

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claim tracking page");
                TempData["ErrorMessage"] = "An error occurred while loading claim tracking.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// HR Edit Claim - Allows Human Resources to edit claim submissions
        /// Part 3 POE requirement: HR can edit submissions
        /// </summary>
        /// <param name="claimId">ID of the claim to edit</param>
        /// <returns>Edit view or redirect</returns>
        [HttpGet]
        public IActionResult HREditClaim(int claimId)
        {
            try
            {
                // Validate authentication and authorization
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _logger.LogWarning("Unauthorized access attempt to HR edit claim");
                    TempData["ErrorMessage"] = "Please log in to access HR features.";
                    return RedirectToAction("Index", "Auth");
                }

                var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;
                if (userRole != UserRole.HumanResource.ToString())
                {
                    _logger.LogWarning("User {UserId} with role {Role} attempted to access HR edit claim",
                        HttpContext.Session.GetInt32("UserId"), userRole);
                    TempData["ErrorMessage"] = "Access denied. HR edit features are only available for Human Resources.";
                    return RedirectToAction("Index", "Home");
                }

                // Retrieve claim from text file storage
                var claim = _dataService.GetClaimById(claimId);
                if (claim == null)
                {
                    _logger.LogWarning("Claim {ClaimId} not found for HR editing", claimId);
                    TempData["ErrorMessage"] = $"Claim #{claimId} not found.";
                    return RedirectToAction("HRDashboard");
                }

                // Create edit view model
                var viewModel = new HREditClaimViewModel
                {
                    ClaimId = claim.ClaimId,
                    LecturerId = claim.LecturerId,
                    LecturerName = GetLecturerName(claim.LecturerId),
                    OriginalHoursWorked = claim.HoursWorked,
                    HoursWorked = claim.HoursWorked,
                    OriginalHourlyRate = claim.HourlyRate,
                    HourlyRate = claim.HourlyRate,
                    OriginalAmount = claim.Amount,
                    Amount = claim.Amount,
                    Status = claim.Status,
                    SubmissionComments = claim.SubmissionComments,
                    MonthYear = claim.MonthYear,
                    ClaimDate = claim.ClaimDate
                };

                _logger.LogInformation("HR editing claim {ClaimId} for lecturer {LecturerId}",
                    claimId, claim.LecturerId);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading HR edit claim view for claim {ClaimId}", claimId);
                TempData["ErrorMessage"] = "An error occurred while loading the claim for editing.";
                return RedirectToAction("HRDashboard");
            }
        }

        /// <summary>
        /// HR Edit Claim - POST handler for saving HR edits
        /// Part 3 POE requirement: HR can edit and update submissions
        /// </summary>
        /// <param name="model">Edited claim data</param>
        /// <returns>Redirect to HR dashboard or error view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HREditClaim(HREditClaimViewModel model)
        {
            try
            {
                // Validate authentication and authorization
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _logger.LogWarning("Unauthorized POST attempt to HR edit claim");
                    TempData["ErrorMessage"] = "Please log in to access HR features.";
                    return RedirectToAction("Index", "Auth");
                }

                var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;
                if (userRole != UserRole.HumanResource.ToString())
                {
                    _logger.LogWarning("User {UserId} with role {Role} attempted to POST HR edit claim",
                        HttpContext.Session.GetInt32("UserId"), userRole);
                    TempData["ErrorMessage"] = "Access denied. HR edit features are only available for Human Resources.";
                    return RedirectToAction("Index", "Home");
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("HR edit claim model validation failed for claim {ClaimId}", model.ClaimId);
                    return View(model);
                }

                // Retrieve original claim
                var claim = _dataService.GetClaimById(model.ClaimId);
                if (claim == null)
                {
                    _logger.LogWarning("Claim {ClaimId} not found during HR edit submission", model.ClaimId);
                    TempData["ErrorMessage"] = $"Claim #{model.ClaimId} not found.";
                    return RedirectToAction("HRDashboard");
                }

                // Update claim with HR edits
                claim.HoursWorked = model.HoursWorked;
                claim.HourlyRate = model.HourlyRate;
                claim.Amount = model.Amount;
                claim.SubmissionComments = model.SubmissionComments;
                claim.ModifiedDate = DateTime.Now;

                // Save updated claim
                _dataService.SaveClaim(claim);

                // Log HR edit activity
                LogHREditActivity(claim, model);

                _logger.LogInformation("HR successfully edited claim {ClaimId}. Changes: Hours {OldHours}->{NewHours}, Rate {OldRate}->{NewRate}, Amount {OldAmount}->{NewAmount}",
                    model.ClaimId, model.OriginalHoursWorked, model.HoursWorked,
                    model.OriginalHourlyRate, model.HourlyRate,
                    model.OriginalAmount, model.Amount);

                TempData["SuccessMessage"] = $"Claim #{model.ClaimId} has been successfully updated.";
                return RedirectToAction("HRDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing HR edit claim for claim {ClaimId}", model.ClaimId);
                ModelState.AddModelError("", "An error occurred while updating the claim. Please try again.");
                return View(model);
            }
        }

        /// <summary>
        /// Generates PDF report for HR analytics
        /// Part 3 POE requirement: PDF report generation with date and time
        /// </summary>
        /// <param name="reportType">Type of report to generate</param>
        /// <returns>PDF file download</returns>
        [HttpGet]
        public IActionResult GenerateHRReport(string reportType = "comprehensive")
        {
            try
            {
                // Validate authentication and authorization
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _logger.LogWarning("Unauthorized access attempt to generate HR report");
                    TempData["ErrorMessage"] = "Please log in to access HR features.";
                    return RedirectToAction("Index", "Auth");
                }

                var userRole = HttpContext.Session.GetString("Role") ?? string.Empty;
                if (userRole != UserRole.HumanResource.ToString() && userRole != UserRole.AcademicManager.ToString())
                {
                    _logger.LogWarning("User {UserId} with role {Role} attempted to generate HR report",
                        HttpContext.Session.GetInt32("UserId"), userRole);
                    TempData["ErrorMessage"] = "Access denied. Report generation is only available for Human Resources and Academic Managers.";
                    return RedirectToAction("Index", "Home");
                }

                // Generate report data
                var reportData = GenerateReportData(reportType);

                // Generate PDF content
                var pdfContent = GeneratePdfReport(reportData);

                // Return PDF file
                var fileName = $"HR_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                _logger.LogInformation("HR report generated successfully: {FileName}", fileName);

                return File(Encoding.UTF8.GetBytes(pdfContent), "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating HR report");
                TempData["ErrorMessage"] = "An error occurred while generating the report. Please try again.";
                return RedirectToAction("HRDashboard");
            }
        }

        #region Part 3 Automation Methods

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
                    .Where(u => (u.Role == UserRole.ProgrammeCoordinator ||
                                u.Role == UserRole.AcademicManager ||
                                u.Role == UserRole.HumanResource) && u.IsActive)
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

        #region HR Edit and Report Generation Methods

        /// <summary>
        /// Logs HR edit activity for audit trail
        /// Part 3 POE requirement: Audit trail for HR edits
        /// </summary>
        /// <param name="claim">Updated claim</param>
        /// <param name="model">Edit view model with original values</param>
        private void LogHREditActivity(Claim claim, HREditClaimViewModel model)
        {
            try
            {
                var editLog = new
                {
                    ClaimId = claim.ClaimId,
                    EditedByUserId = HttpContext.Session.GetInt32("UserId"),
                    EditedByRole = HttpContext.Session.GetString("Role"),
                    EditTimestamp = DateTime.Now,
                    Changes = new
                    {
                        HoursWorked = new { From = model.OriginalHoursWorked, To = model.HoursWorked },
                        HourlyRate = new { From = model.OriginalHourlyRate, To = model.HourlyRate },
                        Amount = new { From = model.OriginalAmount, To = model.Amount }
                    }
                };

                // In a real system, this would be saved to an audit log
                _logger.LogInformation("HR Edit Activity: {@EditLog}", editLog);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log HR edit activity for claim {ClaimId}", claim.ClaimId);
            }
        }

        /// <summary>
        /// Generates comprehensive report data for PDF generation
        /// Part 3 POE requirement: Comprehensive reporting data
        /// </summary>
        /// <param name="reportType">Type of report</param>
        /// <returns>HR report view model</returns>
        private HRReportViewModel GenerateReportData(string reportType)
        {
            var allClaims = _dataService.GetAllClaims();
            var approvedClaims = allClaims.Where(c => c.Status == ClaimStatus.Approved).ToList();
            var rejectedClaims = allClaims.Where(c => c.Status == ClaimStatus.Rejected).ToList();
            var pendingClaims = allClaims.Where(c => c.Status == ClaimStatus.Submitted).ToList();
            var paidClaims = allClaims.Where(c => c.Status == ClaimStatus.Paid).ToList();

            var lecturers = _dataService.GetAllLecturers();
            var departmentStats = lecturers.GroupBy(l => l.Department)
                .Select(g => new DepartmentStatViewModel
                {
                    DepartmentName = g.Key,
                    LecturerCount = g.Count(),
                    TotalClaims = allClaims.Count(c => lecturers.Any(l => l.LecturerId == c.LecturerId && l.Department == g.Key)),
                    TotalAmount = allClaims.Where(c => lecturers.Any(l => l.LecturerId == c.LecturerId && l.Department == g.Key))
                                         .Sum(c => c.Amount)
                })
                .Where(d => d.TotalClaims > 0)
                .ToList();

            return new HRReportViewModel
            {
                ReportTitle = reportType == "comprehensive" ? "Comprehensive HR Analytics Report" : "HR Summary Report",
                GeneratedAt = DateTime.Now,
                ReportPeriod = DateTime.Now.ToString("MMMM yyyy"),
                TotalClaims = allClaims.Count,
                ApprovedClaims = approvedClaims.Count,
                RejectedClaims = rejectedClaims.Count,
                PendingClaims = pendingClaims.Count,
                TotalAmountApproved = approvedClaims.Sum(c => c.Amount),
                TotalAmountPaid = paidClaims.Sum(c => c.Amount),
                ApprovalRate = allClaims.Count > 0 ? Math.Round((decimal)approvedClaims.Count / allClaims.Count * 100, 2) : 0,
                TopLecturers = GetTopPerformingLecturers(approvedClaims),
                MonthlyBreakdown = GetMonthlyClaimBreakdown(approvedClaims),
                DepartmentStats = departmentStats
            };
        }

        /// <summary>
        /// Generates PDF report content using simple text-based PDF structure
        /// Part 3 POE requirement: PDF document generation
        /// </summary>
        /// <param name="reportData">Report data to include</param>
        /// <returns>PDF content as string</returns>
        private string GeneratePdfReport(HRReportViewModel reportData)
        {
            var pdfContent = new StringBuilder();

            // PDF header
            pdfContent.AppendLine("%PDF-1.4");
            pdfContent.AppendLine("1 0 obj");
            pdfContent.AppendLine("<< /Type /Catalog /Pages 2 0 R >>");
            pdfContent.AppendLine("endobj");

            // PDF pages
            pdfContent.AppendLine("2 0 obj");
            pdfContent.AppendLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
            pdfContent.AppendLine("endobj");

            // PDF content
            pdfContent.AppendLine("3 0 obj");
            pdfContent.AppendLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R >>");
            pdfContent.AppendLine("endobj");

            // PDF stream content
            var content = GeneratePdfContent(reportData);
            pdfContent.AppendLine("4 0 obj");
            pdfContent.AppendLine("<< /Length " + content.Length + " >>");
            pdfContent.AppendLine("stream");
            pdfContent.AppendLine(content);
            pdfContent.AppendLine("endstream");
            pdfContent.AppendLine("endobj");

            // PDF trailer
            pdfContent.AppendLine("xref");
            pdfContent.AppendLine("0 5");
            pdfContent.AppendLine("0000000000 65535 f ");
            pdfContent.AppendLine("0000000010 00000 n ");
            pdfContent.AppendLine("0000000079 00000 n ");
            pdfContent.AppendLine("0000000178 00000 n ");
            pdfContent.AppendLine("0000000309 00000 n ");
            pdfContent.AppendLine("trailer");
            pdfContent.AppendLine("<< /Size 5 /Root 1 0 R >>");
            pdfContent.AppendLine("startxref");
            pdfContent.AppendLine("0");
            pdfContent.AppendLine("%%EOF");

            return pdfContent.ToString();
        }

        /// <summary>
        /// Generates PDF content with report data
        /// Part 3 POE requirement: Professional PDF formatting
        /// </summary>
        /// <param name="reportData">Report data to include</param>
        /// <returns>Formatted PDF content</returns>
        private string GeneratePdfContent(HRReportViewModel reportData)
        {
            var content = new StringBuilder();

            // Simple PDF content generation
            content.AppendLine("BT");
            content.AppendLine("/F1 12 Tf");
            content.AppendLine("50 750 Td");
            content.AppendLine("(" + reportData.ReportTitle + ") Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Generated: " + reportData.FormattedDate + ") Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Report Period: " + reportData.ReportPeriod + ") Tj");
            content.AppendLine("0 -40 Td");
            content.AppendLine("(Summary Statistics:) Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Total Claims: " + reportData.TotalClaims + ") Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Approved Claims: " + reportData.ApprovedClaims + ") Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Rejected Claims: " + reportData.RejectedClaims + ") Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Pending Claims: " + reportData.PendingClaims + ") Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Total Amount Approved: " + reportData.TotalAmountApproved.ToString("C") + ") Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Total Amount Paid: " + reportData.TotalAmountPaid.ToString("C") + ") Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Approval Rate: " + reportData.ApprovalRate + "%) Tj");
            content.AppendLine("0 -40 Td");
            content.AppendLine("(Top Performing Lecturers:) Tj");

            // Add top lecturers
            foreach (var lecturer in reportData.TopLecturers.Take(5))
            {
                content.AppendLine("0 -20 Td");
                content.AppendLine("(" + lecturer.LecturerName + " - " + lecturer.TotalAmount.ToString("C") + " - " + lecturer.ClaimCount + " claims) Tj");
            }

            content.AppendLine("0 -40 Td");
            content.AppendLine("(Department Statistics:) Tj");

            // Add department stats
            foreach (var dept in reportData.DepartmentStats)
            {
                content.AppendLine("0 -20 Td");
                content.AppendLine("(" + dept.DepartmentName + ": " + dept.LecturerCount + " lecturers, " + dept.TotalClaims + " claims, " + dept.TotalAmount.ToString("C") + ") Tj");
            }

            content.AppendLine("0 -40 Td");
            content.AppendLine("(Report generated by Contract Monthly Claim System) Tj");
            content.AppendLine("0 -20 Td");
            content.AppendLine("(Part 3 POE Automation - Human Resource Analytics) Tj");
            content.AppendLine("ET");

            return content.ToString();
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