using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using contract_monthly_claim_system_cs.Models.ClaimViewModels;
using contract_monthly_claim_system_cs.Models.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Controller for handling claim-related operations with full Part 2 functionality
    /// </summary>
    public class ClaimsController : Controller
    {
        private static List<ClaimApprovalViewModel> _claims = new List<ClaimApprovalViewModel>();
        private static List<Document> _documents = new List<Document>();
        private static int _nextClaimId = 1;
        private static int _nextDocumentId = 1;

        private const int MaxFileSize = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedFileTypes = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

        /// <summary>
        /// Displays the claim submission form with enhanced validation
        /// </summary>
        [HttpGet]
        public IActionResult Submit()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    return RedirectToAction("Index", "Auth");
                }

                var viewModel = new ClaimSubmissionViewModel
                {
                    HourlyRate = 150.00m
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in Submit GET: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading the form.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Handles claim submission with file upload and validation
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ClaimSubmissionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Calculate amount
                model.Amount = model.HoursWorked * model.HourlyRate;

                var documentNames = new List<string>();
                var savedDocuments = new List<Document>();

                // Process file uploads
                if (model.Documents != null && model.Documents.Count > 0)
                {
                    foreach (var file in model.Documents)
                    {
                        if (file.Length > 0)
                        {
                            // Validate file
                            var validationResult = ValidateFile(file);
                            if (!validationResult.IsValid)
                            {
                                ModelState.AddModelError("Documents", validationResult.ErrorMessage);
                                return View(model);
                            }

                            // Secure file name
                            var fileName = Path.GetFileName(file.FileName);
                            var safeFileName = $"{Guid.NewGuid()}_{fileName}";

                            // In production, save to secure location
                            // For prototype, store in memory
                            var document = new Document
                            {
                                DocumentId = _nextDocumentId++,
                                FileName = fileName,
                                FilePath = safeFileName,
                                UploadDate = DateTime.Now,
                                FileSize = file.Length
                            };

                            _documents.Add(document);
                            savedDocuments.Add(document);
                            documentNames.Add(fileName);
                        }
                    }
                }

                // Create and store claim
                var claim = new ClaimApprovalViewModel
                {
                    ClaimId = _nextClaimId++,
                    LecturerName = HttpContext.Session.GetString("Name") ?? "Unknown Lecturer",
                    ClaimDate = DateTime.Now,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Amount = model.Amount,
                    Status = "Submitted",
                    DocumentNames = documentNames,
                    SubmissionComments = model.Comments,
                    Documents = savedDocuments,
                    LecturerId = HttpContext.Session.GetInt32("UserId") ?? 0
                };

                _claims.Add(claim);

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Status", new { claimId = claim.ClaimId });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in Submit POST: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while submitting your claim. Please try again.");
                return View(model);
            }
        }

        /// <summary>
        /// Enhanced approval interface with better organization
        /// </summary>
        [HttpGet]
        public IActionResult Approve()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    return RedirectToAction("Index", "Auth");
                }

                var userRole = HttpContext.Session.GetString("Role");
                var submittedClaims = _claims.Where(c => c.Status == "Submitted").ToList();

                ViewBag.UserRole = userRole;
                return View(submittedClaims);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Approve GET: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading claims.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Handles claim approval with comments
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveClaim(int claimId, bool isApproved, string approvalComments)
        {
            try
            {
                var claim = _claims.FirstOrDefault(c => c.ClaimId == claimId);
                if (claim != null)
                {
                    claim.Status = isApproved ? "Approved" : "Rejected";
                    claim.ApprovalComments = approvalComments;
                    claim.ApprovalDate = DateTime.Now;
                    claim.ApprovedBy = HttpContext.Session.GetString("Name") ?? "Unknown Approver";
                    claim.ApproverRole = HttpContext.Session.GetString("Role") ?? "Unknown";

                    TempData["SuccessMessage"] = $"Claim #{claimId} has been {claim.Status.ToLower()}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ApproveClaim: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while processing the claim.";
            }

            return RedirectToAction("Approve");
        }

        /// <summary>
        /// Enhanced status tracking with real-time updates
        /// </summary>
        [HttpGet]
        public IActionResult Status(int claimId)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    return RedirectToAction("Index", "Auth");
                }

                var claim = _claims.FirstOrDefault(c => c.ClaimId == claimId);
                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                    return RedirectToAction("Track");
                }

                // Check if user has permission to view this claim
                var userRole = HttpContext.Session.GetString("Role");
                var userName = HttpContext.Session.GetString("Name");
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userRole != "Lecturer" || claim.LecturerName == userName || claim.LecturerId == userId)
                {
                    return View(claim);
                }
                else
                {
                    TempData["ErrorMessage"] = "You don't have permission to view this claim.";
                    return RedirectToAction("Track");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Status: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading claim details.";
                return RedirectToAction("Track");
            }
        }

        /// <summary>
        /// Comprehensive claim tracking for all users
        /// </summary>
        [HttpGet]
        public IActionResult Track()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    return RedirectToAction("Index", "Auth");
                }

                var userRole = HttpContext.Session.GetString("Role");
                IEnumerable<ClaimApprovalViewModel> filteredClaims;

                if (userRole == "Lecturer")
                {
                    var lecturerName = HttpContext.Session.GetString("Name");
                    var lecturerId = HttpContext.Session.GetInt32("UserId");
                    filteredClaims = _claims.Where(c =>
                        c.LecturerName == lecturerName || c.LecturerId == lecturerId);
                }
                else
                {
                    filteredClaims = _claims;
                }

                ViewBag.UserRole = userRole;
                return View(filteredClaims.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Track: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading claims.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// File validation helper method
        /// </summary>
        private FileValidationResult ValidateFile(IFormFile file)
        {
            // Check file size
            if (file.Length > MaxFileSize)
            {
                return new FileValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"File {file.FileName} is too large. Maximum size is 5MB."
                };
            }

            // Check file type
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedFileTypes.Contains(extension))
            {
                return new FileValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"File {file.FileName} is not a supported file type. Please upload PDF, DOC, DOCX, JPG, or PNG files."
                };
            }

            return new FileValidationResult { IsValid = true };
        }

        /// <summary>
        /// Download document functionality
        /// </summary>
        [HttpGet]
        public IActionResult DownloadDocument(int documentId)
        {
            try
            {
                var document = _documents.FirstOrDefault(d => d.DocumentId == documentId);
                if (document == null)
                {
                    return NotFound();
                }

                // In production, return the actual file from secure storage
                // For prototype, return a placeholder
                return Content($"This would download: {document.FileName} (Size: {document.FileSize} bytes)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DownloadDocument: {ex.Message}");
                return StatusCode(500, "An error occurred while downloading the document.");
            }
        }
    }

    /// <summary>
    /// File validation result structure
    /// </summary>
    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}