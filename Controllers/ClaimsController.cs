using Microsoft.AspNetCore.Mvc;
using contract_monthly_claim_system_cs.Models.ViewModels;
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
        /// Displays the claim submission form
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
                HourlyRate = 150.00m // Default value
            };
            return View(viewModel);
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
                    HourlyRate = 150.00m,
                    Amount = 6000.00m,
                    Status = "Submitted",
                    DocumentNames = new List<string> { "Timesheet.pdf" }
                }
            };
            return View(viewModel);
        }

        /// <summary>
        /// Displays claim status
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
                HourlyRate = 150.00m,
                Amount = 6000.00m,
                Status = "Submitted",
                DocumentNames = new List<string> { "Timesheet.pdf" }
            };
            return View(viewModel);
        }
    }
}