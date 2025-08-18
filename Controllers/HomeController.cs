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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the minimalist home page
        /// </summary>
        public IActionResult Index()
        {
            ViewData["WelcomeMessage"] = "Contract Monthly Claim System";
            ViewData["SystemDescription"] = "Streamlined claim submission and approval";
            return View();
        }

        /// <summary>
        /// Displays the privacy page
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}