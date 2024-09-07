using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using static Apptivate_UQMS_WebApp.Models.Account;
using Microsoft.Extensions.Logging;
using Apptivate_UQMS_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Apptivate_UQMS_WebApp.Controllers
{

    public class AccountController : Controller
    {
        private readonly FirebaseAuthService _firebaseAuthService;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUserRegistrationService _userRegistrationService;

        public AccountController(FirebaseAuthService firebaseAuthService, ILogger<AccountController> logger, ApplicationDbContext context,IUserRegistrationService userRegistrationService)
        {
            _firebaseAuthService = firebaseAuthService;
            _logger = logger;
            _context = context; // Assign the injected context
            _userRegistrationService = userRegistrationService;
        }

        [HttpGet]
        public IActionResult SelectRole()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SelectRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError(string.Empty, "Please select a role.");
                return View();
            }

            TempData["SelectedRole"] = role;
            return RedirectToAction("Register");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User already authenticated, redirecting to home.");
                return RedirectToAction("Index", "Home");
            }

            _logger.LogInformation("Register page loaded.");

            // Fetch departments from the database
            var departments = _context.Departments.Select(d => new SelectListItem
            {
                Value = d.DepartmentID.ToString(),
                Text = d.DepartmentName
            }).ToList();

            var courses = _context.Courses.Select(d => new SelectListItem
            {
                Value = d.CourseID.ToString(),
                Text = d.CourseName
            }).ToList();


            // Pass the selected role to the view
            var selectedRole = TempData["SelectedRole"]?.ToString();
            var model = new RegisterViewModel
            {
                Role = selectedRole,
                Departments = departments, // Populate departments in the ViewModel
                Courses = courses
            };



            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Use the service to register the user
                    var user = await _userRegistrationService.RegisterUserAsync(model);

                    _logger.LogInformation($"User registered successfully with email: {user.Email}");
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while registering user: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                }
            }

            
            // Repopulate departments list if validation fails
            model.Departments = _context.Departments.Select(d => new SelectListItem
            {
                Value = d.DepartmentID.ToString(),
                Text = d.DepartmentName
            }).ToList();

            // Repopulate departments list if validation fails
            model.Courses = _context.Courses.Select(d => new SelectListItem
            {
                Value = d.CourseID.ToString(),
                Text = d.CourseName
            }).ToList();
            

            _logger.LogWarning($"Registration failed for user with email: {model.Email}");
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User already authenticated, redirecting to home.");
                return RedirectToAction("Index", "Home");
            }

            _logger.LogInformation("Login page loaded.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = await _firebaseAuthService.LoginUser(model.Email, model.Password);
                    _logger.LogInformation($"User logged in successfully with token: {token}");

                    Response.Cookies.Append("FirebaseToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during login.");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            _logger.LogWarning("Invalid model state during login.");
            return View(model);
        }


        [HttpPost]
        public IActionResult Logout()
        {
            // Log information about logout
            _logger.LogInformation("User logged out.");

            // Delete the FirebaseToken cookie
            Response.Cookies.Delete("FirebaseToken");

            // Redirect to the login page or another public page
            return RedirectToAction("Login", "Account");
        }

    }
}


