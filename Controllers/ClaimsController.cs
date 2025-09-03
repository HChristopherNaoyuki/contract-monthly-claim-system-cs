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

                // In a real application, this would save to database
                // For prototype, redirect to status page with sample data
                return RedirectToAction("Status", new { claimId = 1 });
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

            var viewModel = new List<ClaimApprovalViewModel>
            {
                new ClaimApprovalViewModel
                {
                    ClaimId = 1,
                    LecturerName = "John Smith",
                    ClaimDate = DateTime.Now.AddDays(-2),
                    HoursWorked = 40,
                    HourlyRate = 175.00m, // Updated to show different rate
                    Amount = 7000.00m, // Updated calculation
                    Status = "Submitted",
                    DocumentNames = new List<string> { "Timesheet.pdf", "Contract.pdf" }
                }
            };
            return View(viewModel);
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

            var viewModel = new ClaimApprovalViewModel
            {
                ClaimId = claimId,
                LecturerName = "John Smith",
                ClaimDate = DateTime.Now.AddDays(-2),
                HoursWorked = 40,
                HourlyRate = 175.00m, // Updated to show different rate
                Amount = 7000.00m, // Updated calculation
                Status = "Submitted",
                DocumentNames = new List<string> { "Timesheet.pdf", "Contract.pdf" }
            };
            return View(viewModel);
        }
    }
}