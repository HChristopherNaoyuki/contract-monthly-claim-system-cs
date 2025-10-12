using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using contract_monthly_claim_system_cs.Models.ClaimViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace contract_monthly_claim_system_cs.Controllers
{
    public class ClaimsController : Controller
    {
        private static List<ClaimApprovalViewModel> _claims = new List<ClaimApprovalViewModel>();
        private static int _nextClaimId = 1;

        public IActionResult Submit()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(ClaimSubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Amount = model.HoursWorked * model.HourlyRate;

                var documentNames = new List<string>();
                if (model.Documents != null && model.Documents.Count > 0)
                {
                    foreach (var file in model.Documents)
                    {
                        if (file.Length > 0)
                        {
                            documentNames.Add(file.FileName);
                        }
                    }
                }

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
                    SubmissionComments = model.Comments
                };

                _claims.Add(claim);

                return RedirectToAction("Status", new { claimId = claim.ClaimId });
            }

            return View(model);
        }

        public IActionResult Approve()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var submittedClaims = _claims.Where(c => c.Status == "Submitted").ToList();
            return View(submittedClaims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveClaim(int claimId, bool isApproved, string comments)
        {
            var claim = _claims.Find(c => c.ClaimId == claimId);
            if (claim != null)
            {
                claim.Status = isApproved ? "Approved" : "Rejected";
                claim.ApprovalComments = comments;
            }

            return RedirectToAction("Approve");
        }

        public IActionResult Status(int claimId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            var claim = _claims.Find(c => c.ClaimId == claimId);
            if (claim == null)
            {
                claim = new ClaimApprovalViewModel
                {
                    ClaimId = claimId,
                    LecturerName = "John Smith",
                    ClaimDate = DateTime.Now.AddDays(-2),
                    HoursWorked = 40,
                    HourlyRate = 175.00m,
                    Amount = 7000.00m,
                    Status = "Not Found",
                    DocumentNames = new List<string>(),
                    SubmissionComments = "Sample comment for demonstration"
                };
            }

            return View(claim);
        }

        public IActionResult Track()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            return View(_claims);
        }
    }
}