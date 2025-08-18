// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using contract_monthly_claim_system_cs.Models;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Controller for handling home page and privacy page requests
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the HomeController class
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the home page
        /// </summary>
        /// <returns>The view result</returns>
        public IActionResult Index()
        {
            ViewData["WelcomeMessage"] = "Welcome to the Contract Monthly Claim System";
            ViewData["SystemDescription"] = "Streamline your monthly claim submissions and approvals";
            return View();
        }

        /// <summary>
        /// Displays the privacy page
        /// </summary>
        /// <returns>The view result</returns>
        public IActionResult Privacy()
        {
            ViewData["PrivacyTitle"] = "Privacy Policy";
            ViewData["PrivacyContent"] = "Your data is securely stored and only used for claim processing purposes.";
            return View();
        }

        /// <summary>
        /// Displays the error page
        /// </summary>
        /// <returns>The view result</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}