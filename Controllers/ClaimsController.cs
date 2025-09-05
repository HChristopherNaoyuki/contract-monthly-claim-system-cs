using Microsoft.AspNetCore.Mvc;
using contract_monthly_claim_system_cs.Models.ClaimViewModels;
using System;
using System.Collections.Generic;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Controller for handling claim-related operations
    /// </summary>
    public class ClaimsController : Controller
    {
        // Static list to store claims in memory for prototype
        private static List<ClaimApprovalViewModel> _claims = new List<ClaimApprovalViewModel>();
        private static int _nextClaimId = 1;

        /// <summary>
        /// Displays the claim submission form with editable hourly rate
        /// </summary>
        public IActionResult Submit()
        {
            // Check if user is authenticated
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var viewModel = new ClaimSubmissionViewModel
            {
                HourlyRate = 150.00m // Default value, now editable
            };
            return View(viewModel);
        }

        /// <summary>
        /// Handles claim submission with editable hourly rate
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(ClaimSubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Calculate amount based on user-input hours and rate
                model.Amount = model.HoursWorked * model.HourlyRate;

                // Create and store the claim for prototype
                var claim = new ClaimApprovalViewModel
                {
                    ClaimId = _nextClaimId++,
                    LecturerName = HttpContext.Session.GetString("Name") ?? "Unknown Lecturer",
                    ClaimDate = DateTime.Now,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Amount = model.Amount,
                    Status = "Submitted",
                    DocumentNames = new List<string>()
                };

                _claims.Add(claim);

                // Redirect to status page with the new claim ID
                return RedirectToAction("Status", new { claimId = claim.ClaimId });
            }

            return View(model);
        }

        /// <summary>
        /// Displays the claim approval interface
        /// </summary>
        public IActionResult Approve()
        {
            // Check if user is authenticated
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            // Return all claims for approval
            return View(_claims);
        }

        /// <summary>
        /// Handles claim approval
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveClaim(int claimId, bool isApproved, string comments)
        {
            var claim = _claims.Find(c => c.ClaimId == claimId);
            if (claim != null)
            {
                claim.Status = isApproved ? "Approved" : "Rejected";
                claim.Comments = comments;
            }

            return RedirectToAction("Approve");
        }

        /// <summary>
        /// Displays claim status in table format
        /// </summary>
        public IActionResult Status(int claimId)
        {
            // Check if user is authenticated
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var claim = _claims.Find(c => c.ClaimId == claimId);
            if (claim == null)
            {
                // Return sample data if claim not found
                claim = new ClaimApprovalViewModel
                {
                    ClaimId = claimId,
                    LecturerName = "John Smith",
                    ClaimDate = DateTime.Now.AddDays(-2),
                    HoursWorked = 40,
                    HourlyRate = 175.00m,
                    Amount = 7000.00m,
                    Status = "Submitted",
                    DocumentNames = new List<string> { "Timesheet.pdf", "Contract.pdf" }
                };
            }

            return View(claim);
        }
    }
}