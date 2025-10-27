using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using contract_monthly_claim_system_cs.Models;
using Microsoft.AspNetCore.Http;
using contract_monthly_claim_system_cs.Extensions;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Home controller for public pages and system navigation
    /// Enhanced with HR Dashboard access and Part 3 POE features
    /// Allows access to all users without authentication for public pages
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of HomeController
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the home page with system overview and role-based navigation
        /// Part 3 POE requirement: Enhanced user experience with role-based features
        /// Accessible to all users without authentication
        /// </summary>
        /// <returns>Home page view with dynamic content based on user role</returns>
        public IActionResult Index()
        {
            ViewData["WelcomeMessage"] = "Contract Monthly Claim System";
            ViewData["SystemDescription"] = "Streamlined claim submission and approval for independent contractor lecturers";

            // Check user authentication and role for dynamic content
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                ViewData["IsLoggedIn"] = true;
                ViewData["UserName"] = HttpContext.Session.GetString("Name");
                ViewData["UserRole"] = HttpContext.Session.GetString("Role");

                // Part 3 POE: Role-based feature visibility
                var userRole = HttpContext.Session.GetString("Role");
                if (userRole == "AcademicManager")
                {
                    ViewData["ShowHRAnalytics"] = true;
                    ViewData["HRDashboardMessage"] = "Access comprehensive HR analytics and automated reporting";
                }
                else
                {
                    ViewData["ShowHRAnalytics"] = false;
                }
            }
            else
            {
                ViewData["IsLoggedIn"] = false;
                ViewData["ShowHRAnalytics"] = false;
            }

            _logger.LogInformation("Home page accessed with role-based content display");
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page
        /// Part 3 POE requirement: System documentation and compliance
        /// Accessible to all users without authentication
        /// </summary>
        /// <returns>Privacy policy view</returns>
        public IActionResult Privacy()
        {
            _logger.LogInformation("Privacy policy page accessed");
            return View();
        }

        /// <summary>
        /// Handles application errors and displays error page
        /// Part 3 POE requirement: Robust error handling and user experience
        /// Accessible to all users without authentication
        /// </summary>
        /// <returns>Error page view</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("Error page displayed for request: {RequestId}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}