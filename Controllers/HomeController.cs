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
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the application home page
        /// </summary>
        public IActionResult Index()
        {
            // Check if user is authenticated
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            ViewData["WelcomeMessage"] = "Contract Monthly Claim System";
            ViewData["SystemDescription"] = "Streamlined claim submission and approval";
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page
        /// </summary>
        public IActionResult Privacy()
        {
            // Check if user is authenticated
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            return View();
        }

        /// <summary>
        /// Displays the error page
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}