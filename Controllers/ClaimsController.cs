using contract_monthly_claim_system_cs.Models.ClaimViewModels;
using Microsoft.AspNetCore.Mvc;

// Controllers/ClaimsController.cs
namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Controller for handling claim-related operations
    /// </summary>
    public class ClaimsController : Controller
    {
        private readonly ClaimSystemContext _context;

        public ClaimsController(ClaimSystemContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the claim submission form
        /// </summary>
        [HttpGet]
        public IActionResult Submit()
        {
            // In a real implementation, we would get the current lecturer's details
            var viewModel = new ClaimSubmissionViewModel
            {
                HourlyRate = 150.00m // Default value for prototype
            };

            return View(viewModel);
        }

        /// <summary>
        /// Handles the claim submission form post
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ClaimSubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // In a real implementation, we would save to database
                // For prototype, we just redirect to status page
                return RedirectToAction("Status", new { claimId = 1 });
            }

            return View(model);
        }

        /// <summary>
        /// Displays the approval interface
        /// </summary>
        [HttpGet]
        public IActionResult Approve()
        {
            // For prototype, we create sample data
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
                    DocumentNames = new List<string> { "Timesheet.pdf", "Invoice.docx" }
                }
            };

            return View(viewModel);
        }

        /// <summary>
        /// Displays claim status
        /// </summary>
        [HttpGet]
        public IActionResult Status(int claimId)
        {
            // For prototype, we create sample data
            var viewModel = new ClaimApprovalViewModel
            {
                ClaimId = claimId,
                LecturerName = "John Smith",
                ClaimDate = DateTime.Now.AddDays(-2),
                HoursWorked = 40,
                HourlyRate = 150.00m,
                Amount = 6000.00m,
                Status = "Submitted",
                DocumentNames = new List<string> { "Timesheet.pdf", "Invoice.docx" }
            };

            return View(viewModel);
        }
    }
}