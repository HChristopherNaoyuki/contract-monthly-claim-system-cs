using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using contract_monthly_claim_system_cs.Models.ClaimViewModels;
using contract_monthly_claim_system_cs.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Claims controller for claim submission, approval, and tracking
    /// </summary>
    public class ClaimsController : Controller
    {
        private readonly TextFileDataService _dataService;
        private readonly ILogger<ClaimsController> _logger;

        /// <summary>
        /// Initializes a new instance of ClaimsController
        /// </summary>
        /// <param name="dataService">Data service for claim operations</param>
        /// <param name="logger">Logger instance</param>
        public ClaimsController(TextFileDataService dataService, ILogger<ClaimsController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        /// <summary>
        /// Displays claim submission form
        /// Requires user authentication
        /// </summary>
        /// <returns>Claim submission view or redirect to auth</returns>
        public IActionResult Submit()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var lecturer = _dataService.GetLecturerById(userId);

            var viewModel = new ClaimSubmissionViewModel
            {
                HourlyRate = lecturer?.HourlyRate ?? 150.00m
            };
            return View(viewModel);
        }

        /// <summary>
        /// Handles claim submission with validation
        /// </summary>
        /// <param name="model">Claim submission view model</param>
        /// <returns>Redirect to status page or error view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(ClaimSubmissionViewModel model)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                model.Amount = model.HoursWorked * model.HourlyRate;

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

                _dataService.SaveClaim(claim);

                // Handle document uploads
                if (model.Documents != null && model.Documents.Count > 0)
                {
                    foreach (var file in model.Documents)
                    {
                        if (file.Length > 0)
                        {
                            var document = new Document
                            {
                                DocumentId = _dataService.GetNextId("documents"),
                                ClaimId = claim.ClaimId,
                                FileName = file.FileName,
                                FilePath = $"/uploads/{claim.ClaimId}_{file.FileName}",
                                FileSize = file.Length,
                                FileType = System.IO.Path.GetExtension(file.FileName),
                                UploadDate = DateTime.Now,
                                IsActive = true
                            };

                            _dataService.SaveDocument(document);
                        }
                    }
                }

                _logger.LogInformation("New claim submitted by user {UserId}, Claim ID: {ClaimId}", userId, claim.ClaimId);

                return RedirectToAction("Status", new { claimId = claim.ClaimId });
            }

            return View(model);
        }

        /// <summary>
        /// Disclaims pending approval for coordinators and managers
        /// </summary>
        /// <returns>Claims approval view or redirect to auth</returns>
        public IActionResult Approve()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var submittedClaims = _dataService.GetAllClaims()
                .Where(c => c.Status == ClaimStatus.Submitted)
                .Select(c => new ClaimApprovalViewModel
                {
                    ClaimId = c.ClaimId,
                    LecturerName = GetLecturerName(c.LecturerId),
                    ClaimDate = c.ClaimDate,
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    Amount = c.Amount,
                    Status = c.Status.ToString(),
                    DocumentNames = _dataService.GetDocumentsByClaimId(c.ClaimId)
                        .Select(d => d.FileName)
                        .ToList(),
                    SubmissionComments = c.SubmissionComments
                })
                .ToList();

            return View(submittedClaims);
        }

        /// <summary>
        /// Handles claim approval or rejection
        /// </summary>
        /// <param name="claimId">ID of the claim to process</param>
        /// <param name="isApproved">True for approval, false for rejection</param>
        /// <param name="comments">Approval comments</param>
        /// <returns>Redirect to approval page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveClaim(int claimId, bool isApproved, string comments)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var claim = _dataService.GetClaimById(claimId);
            if (claim != null)
            {
                claim.Status = isApproved ? ClaimStatus.Approved : ClaimStatus.Rejected;
                claim.ModifiedDate = DateTime.Now;
                _dataService.SaveClaim(claim);

                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userRole = HttpContext.Session.GetString("Role") ?? "Unknown";

                var approval = new Approval
                {
                    ApprovalId = _dataService.GetNextId("approvals"),
                    ClaimId = claimId,
                    ApproverUserId = userId,
                    ApproverRole = userRole,
                    ApprovalDate = DateTime.Now,
                    IsApproved = isApproved,
                    Comments = comments,
                    ApprovalOrder = 1
                };

                _dataService.SaveApproval(approval);

                var action = isApproved ? "approved" : "rejected";
                _logger.LogInformation("Claim {ClaimId} {Action} by user {UserId}", claimId, action, approval.ApproverUserId);
            }

            return RedirectToAction("Approve");
        }

        /// <summary>
        /// Displays claim status details
        /// </summary>
        /// <param name="claimId">ID of the claim to display</param>
        /// <returns>Claim status view</returns>
        public IActionResult Status(int claimId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var claim = _dataService.GetClaimById(claimId);
            if (claim == null)
            {
                // Return a not found claim for demonstration
                claim = new Claim
                {
                    ClaimId = claimId,
                    LecturerId = 2,
                    ClaimDate = DateTime.Now.AddDays(-2),
                    HoursWorked = 40,
                    HourlyRate = 175.00m,
                    Amount = 7000.00m,
                    Status = ClaimStatus.Submitted,
                    SubmissionComments = "Sample comment for demonstration"
                };
            }

            var viewModel = new ClaimApprovalViewModel
            {
                ClaimId = claim.ClaimId,
                LecturerName = GetLecturerName(claim.LecturerId),
                ClaimDate = claim.ClaimDate,
                HoursWorked = claim.HoursWorked,
                HourlyRate = claim.HourlyRate,
                Amount = claim.Amount,
                Status = claim.Status.ToString(),
                DocumentNames = _dataService.GetDocumentsByClaimId(claimId)
                    .Select(d => d.FileName)
                    .ToList(),
                SubmissionComments = claim.SubmissionComments
            };

            // Get approval comments if any
            var approvals = _dataService.GetApprovalsByClaimId(claimId);
            if (approvals.Any())
            {
                viewModel.ApprovalComments = string.Join("; ", approvals.Select(a => a.Comments));
            }

            return View(viewModel);
        }

        /// <summary>
        /// Displays claim tracking for users
        /// </summary>
        /// <returns>Claim tracking view</returns>
        public IActionResult Track()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var userRole = HttpContext.Session.GetString("Role");

            List<Claim> claims;
            if (userRole == UserRole.Lecturer.ToString())
            {
                claims = _dataService.GetClaimsByLecturerId(userId);
            }
            else
            {
                claims = _dataService.GetAllClaims();
            }

            var viewModels = claims.Select(c => new ClaimApprovalViewModel
            {
                ClaimId = c.ClaimId,
                LecturerName = GetLecturerName(c.LecturerId),
                ClaimDate = c.ClaimDate,
                HoursWorked = c.HoursWorked,
                HourlyRate = c.HourlyRate,
                Amount = c.Amount,
                Status = c.Status.ToString(),
                SubmissionComments = c.SubmissionComments,
                ApprovalComments = GetApprovalComments(c.ClaimId)
            }).ToList();

            return View(viewModels);
        }

        /// <summary>
        /// Gets lecturer name by ID
        /// </summary>
        /// <param name="lecturerId">Lecturer ID</param>
        /// <returns>Lecturer full name</returns>
        private string GetLecturerName(int lecturerId)
        {
            var user = _dataService.GetUserById(lecturerId);
            return user != null ? $"{user.Name} {user.Surname}" : "Unknown Lecturer";
        }

        /// <summary>
        /// Gets approval comments for a claim
        /// </summary>
        /// <param name="claimId">Claim ID</param>
        /// <returns>Concatenated approval comments</returns>
        private string GetApprovalComments(int claimId)
        {
            var approvals = _dataService.GetApprovalsByClaimId(claimId);
            return string.Join("; ", approvals.Where(a => !string.IsNullOrEmpty(a.Comments)).Select(a => a.Comments));
        }
    }
}