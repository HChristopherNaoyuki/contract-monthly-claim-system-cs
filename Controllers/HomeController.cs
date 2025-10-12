using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using contract_monthly_claim_system_cs.Models;

namespace contract_monthly_claim_system_cs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            ViewData["WelcomeMessage"] = "Contract Monthly Claim System";
            ViewData["SystemDescription"] = "Streamlined claim submission and approval";

            return View();
        }

        public IActionResult Privacy()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Auth");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}