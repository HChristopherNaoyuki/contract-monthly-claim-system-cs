using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using contract_monthly_claim_system_cs.Models.ViewModels;
using contract_monthly_claim_system_cs.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace contract_monthly_claim_system_cs.Controllers
{
    public class AuthController : Controller
    {
        private static readonly List<User> _users = new List<User>
        {
            new User
            {
                UserId = 1,
                Name = "Admin",
                Surname = "System",
                Username = "admin",
                Password = "admin123",
                Role = UserRole.AcademicManager
            },
            new User
            {
                UserId = 2,
                Name = "John",
                Surname = "Smith",
                Username = "lecturer",
                Password = "lecturer123",
                Role = UserRole.Lecturer
            },
            new User
            {
                UserId = 3,
                Name = "Sarah",
                Surname = "Johnson",
                Username = "coordinator",
                Password = "coordinator123",
                Role = UserRole.ProgrammeCoordinator
            }
        };

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
                var user = _users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Store user information in session using extension methods
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Name", $"{user.Name} {user.Surname}");
                    HttpContext.Session.SetString("Role", user.Role.ToString());

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View("Index", model);
                }

                var user = new User
                {
                    UserId = _users.Count + 1,
                    Name = model.Name,
                    Surname = model.Surname,
                    Username = model.Username,
                    Password = model.Password,
                    Role = model.Role
                };

                _users.Add(user);

                // Auto-login after registration
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Name", $"{user.Name} {user.Surname}");
                HttpContext.Session.SetString("Role", user.Role.ToString());

                return RedirectToAction("Index", "Home");
            }

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
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
            return View();
        }
    }
}