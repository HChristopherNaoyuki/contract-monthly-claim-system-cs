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
    /// Part 3 POE requirement: Enhanced role system with Human Resource role
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
                    // Set session variables
                    HttpContext.Session.SetSessionInt("UserId", user.UserId);
                    HttpContext.Session.SetSessionString("Username", user.Username);
                    HttpContext.Session.SetSessionString("Name", $"{user.Name} {user.Surname}");
                    HttpContext.Session.SetSessionString("Role", user.Role.ToString());

                    _logger.LogInformation("User {Username} logged in successfully with role {Role}", user.Username, user.Role);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
                _logger.LogWarning("Failed login attempt for username: {Username}", model.Username);
            }

            return View("Index", model);
        }

        /// <summary>
        /// Handles new user registration with enhanced role system
        /// Part 3 POE requirement: Added Human Resource role option
        /// </summary>
        /// <param name="model">Registration view model with user details</param>
        /// <returns>Redirect to home or error view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_dataService.GetUserByUsername(model.Username) != null)
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View("Index", new LoginViewModel());
                }

                // Create new user
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

                // Save user to text file storage
                _dataService.SaveUser(user);

                // If user is a lecturer, create lecturer record
                if (model.Role == UserRole.Lecturer)
                {
                    var lecturer = new Lecturer
                    {
                        LecturerId = user.UserId,
                        EmployeeNumber = $"EMP{user.UserId:000}",
                        Department = "General",
                        HourlyRate = 150.00m,
                        ContractStartDate = System.DateTime.Now,
                        ContractEndDate = System.DateTime.Now.AddYears(1)
                    };
                    _dataService.SaveLecturer(lecturer);
                }

                // Automatically log in the new user
                HttpContext.Session.SetSessionInt("UserId", user.UserId);
                HttpContext.Session.SetSessionString("Username", user.Username);
                HttpContext.Session.SetSessionString("Name", $"{user.Name} {user.Surname}");
                HttpContext.Session.SetSessionString("Role", user.Role.ToString());

                _logger.LogInformation("New user registered: {Username} with role {Role}", user.Username, user.Role);

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
            var role = HttpContext.Session.GetSessionString("Role");
            HttpContext.Session.Clear();

            if (!string.IsNullOrEmpty(username))
            {
                _logger.LogInformation("User {Username} with role {Role} logged out", username, role);
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

        /// <summary>
        /// Initializes sample data for demonstration purposes
        /// Part 3 POE requirement: Pre-populated sample users for testing
        /// </summary>
        private void InitializeSampleData()
        {
            var existingUsers = _dataService.GetAllUsers();
            if (existingUsers.Count == 0)
            {
                // Create comprehensive sample data for all roles
                var sampleUsers = new[]
                {
                    new User
                    {
                        UserId = 1,
                        Name = "System",
                        Surname = "Administrator",
                        Username = "admin",
                        Password = "admin123",
                        Role = UserRole.AcademicManager,
                        Email = "admin@cmcs.com",
                        IsActive = true,
                        CreatedDate = System.DateTime.UtcNow
                    },
                    new User
                    {
                        UserId = 2,
                        Name = "John",
                        Surname = "Smith",
                        Username = "lecturer",
                        Password = "lecturer123",
                        Role = UserRole.Lecturer,
                        Email = "john.smith@university.com",
                        IsActive = true,
                        CreatedDate = System.DateTime.UtcNow
                    },
                    new User
                    {
                        UserId = 3,
                        Name = "Sarah",
                        Surname = "Johnson",
                        Username = "coordinator",
                        Password = "coordinator123",
                        Role = UserRole.ProgrammeCoordinator,
                        Email = "sarah.johnson@university.com",
                        IsActive = true,
                        CreatedDate = System.DateTime.UtcNow
                    },
                    new User
                    {
                        UserId = 4,
                        Name = "Michael",
                        Surname = "Brown",
                        Username = "hr",
                        Password = "hr123",
                        Role = UserRole.HumanResource,
                        Email = "michael.brown@university.com",
                        IsActive = true,
                        CreatedDate = System.DateTime.UtcNow
                    }
                };

                foreach (var user in sampleUsers)
                {
                    _dataService.SaveUser(user);
                }

                // Create sample lecturer details
                var lecturer = new Lecturer
                {
                    LecturerId = 2,
                    EmployeeNumber = "EMP001",
                    Department = "Computer Science",
                    HourlyRate = 150.00m,
                    ContractStartDate = System.DateTime.Now.AddYears(-1),
                    ContractEndDate = System.DateTime.Now.AddYears(1),
                    BankAccountNumber = "123456789",
                    BankName = "Sample Bank",
                    TaxNumber = "TAX001"
                };

                _dataService.SaveLecturer(lecturer);

                _logger.LogInformation("Sample data initialization completed with all roles");
            }
        }
    }
}