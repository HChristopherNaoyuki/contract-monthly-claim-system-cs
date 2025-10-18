using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using contract_monthly_claim_system_cs.Models.ViewModels;
using contract_monthly_claim_system_cs.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using contract_monthly_claim_system_cs.Extensions;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Authentication controller for user login, registration, and session management
    /// </summary>
    public class AuthController : Controller
    {
        private readonly TextFileDataService _dataService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of AuthController
        /// </summary>
        /// <param name="dataService">Data service for user operations</param>
        /// <param name="logger">Logger instance</param>
        public AuthController(TextFileDataService dataService, ILogger<AuthController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        /// <summary>
        /// Displays the login/registration page
        /// </summary>
        /// <returns>Authentication view</returns>
        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            var model = new LoginViewModel();
            return View(model);
        }

        /// <summary>
        /// Handles user login with credentials validation
        /// </summary>
        /// <param name="model">Login view model with credentials</param>
        /// <returns>Redirect to home or error view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dataService.GetUserByUsername(model.Username);

                if (user != null && user.Password == model.Password && user.IsActive)
                {
                    HttpContext.Session.SetSessionInt("UserId", user.UserId);
                    HttpContext.Session.SetSessionString("Username", user.Username);
                    HttpContext.Session.SetSessionString("Name", $"{user.Name} {user.Surname}");
                    HttpContext.Session.SetSessionString("Role", user.Role.ToString());

                    _logger.LogInformation("User {Username} logged in successfully", user.Username);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
                _logger.LogWarning("Failed login attempt for username: {Username}", model.Username);
            }

            return View("Index", model);
        }

        /// <summary>
        /// Handles new user registration
        /// </summary>
        /// <param name="model">Registration view model with user details</param>
        /// <returns>Redirect to home or error view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_dataService.GetUserByUsername(model.Username) != null)
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View("Index", new LoginViewModel());
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

                HttpContext.Session.SetSessionInt("UserId", user.UserId);
                HttpContext.Session.SetSessionString("Username", user.Username);
                HttpContext.Session.SetSessionString("Name", $"{user.Name} {user.Surname}");
                HttpContext.Session.SetSessionString("Role", user.Role.ToString());

                _logger.LogInformation("New user registered: {Username}", user.Username);

                return RedirectToAction("Index", "Home");
            }

            return View("Index", new LoginViewModel());
        }

        /// <summary>
        /// Handles user logout and session cleanup
        /// </summary>
        /// <returns>Redirect to authentication page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            var username = HttpContext.Session.GetSessionString("Username");
            HttpContext.Session.Clear();

            if (!string.IsNullOrEmpty(username))
            {
                _logger.LogInformation("User {Username} logged out", username);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays forgot password page
        /// </summary>
        /// <returns>Forgot password view</returns>
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Handles password reset request
        /// </summary>
        /// <param name="username">Username for password reset</param>
        /// <returns>Confirmation view</returns>
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