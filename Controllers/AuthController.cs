using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using contract_monthly_claim_system_cs.Models.ViewModels;
using contract_monthly_claim_system_cs.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace contract_monthly_claim_system_cs.Controllers
{
    public class AuthController : Controller
    {
        private readonly TextFileDataService _dataService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(TextFileDataService dataService, ILogger<AuthController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dataService.GetUserByUsername(model.Username);

                if (user != null && user.Password == model.Password && user.IsActive)
                {
                    // Store user information in session using extension methods
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Name", $"{user.Name} {user.Surname}");
                    HttpContext.Session.SetString("Role", user.Role.ToString());

                    _logger.LogInformation("User {Username} logged in successfully", user.Username);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
                _logger.LogWarning("Failed login attempt for username: {Username}", model.Username);
            }

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_dataService.GetUserByUsername(model.Username) != null)
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View("Index", model);
                }

                var user = new User
                {
                    UserId = _dataService.GetNextId("users"),
                    Name = model.Name,
                    Surname = model.Surname,
                    Username = model.Username,
                    Password = model.Password,
                    Role = model.Role,
                    Email = $"{model.Username}@cmcs.com",
                    IsActive = true,
                    CreatedDate = System.DateTime.UtcNow
                };

                _dataService.SaveUser(user);

                // Auto-login after registration
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Name", $"{user.Name} {user.Surname}");
                HttpContext.Session.SetString("Role", user.Role.ToString());

                _logger.LogInformation("New user registered: {Username}", user.Username);

                return RedirectToAction("Index", "Home");
            }

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            var username = HttpContext.Session.GetString("Username");
            HttpContext.Session.Clear();

            if (!string.IsNullOrEmpty(username))
            {
                _logger.LogInformation("User {Username} logged out", username);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string username)
        {
            ViewBag.Message = "If the username exists, a password reset link has been sent to the associated email.";
            _logger.LogInformation("Password reset requested for username: {Username}", username);
            return View();
        }
    }
}