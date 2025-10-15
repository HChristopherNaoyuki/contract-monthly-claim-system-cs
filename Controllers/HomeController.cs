using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using contract_monthly_claim_system_cs.Models;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Home controller for public pages and system navigation
    /// Allows access to all users without authentication
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
        /// Displays the home page with system overview
        /// Accessible to all users without authentication
        /// </summary>
        /// <returns>Home page view</returns>
        public IActionResult Index()
        {
            // Allow access to all users without authentication
            ViewData["WelcomeMessage"] = "Contract Monthly Claim System";
            ViewData["SystemDescription"] = "Streamlined claim submission and approval for independent contractor lecturers";

            // Check if user is logged in to show personalized content
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                ViewData["IsLoggedIn"] = true;
                ViewData["UserName"] = HttpContext.Session.GetString("Name");
                ViewData["UserRole"] = HttpContext.Session.GetString("Role");
            }
            else
            {
                ViewData["IsLoggedIn"] = false;
            }

            return View();
        }

        /// <summary>
        /// Displays the privacy policy page
        /// Accessible to all users without authentication
        /// </summary>
        /// <returns>Privacy policy view</returns>
        public IActionResult Privacy()
        {
            // Allow access to all users without authentication
            return View();
        }

        /// <summary>
        /// Displays system features and capabilities
        /// Accessible to all users without authentication
        /// </summary>
        /// <returns>Features view</returns>
        public IActionResult Features()
        {
            // Allow access to all users without authentication
            return View();
        }

        /// <summary>
        /// Displays system documentation and help
        /// Accessible to all users without authentication
        /// </summary>
        /// <returns>Help view</returns>
        public IActionResult Help()
        {
            // Allow access to all users without authentication
            return View();
        }

        /// <summary>
        /// Handles application errors and displays error page
        /// Accessible to all users without authentication
        /// </summary>
        /// <returns>Error page view</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}