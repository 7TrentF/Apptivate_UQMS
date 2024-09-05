using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using static Apptivate_UQMS_WebApp.Models.Account;
using Microsoft.Extensions.Logging;

namespace Apptivate_UQMS_WebApp.Controllers
{
    using Microsoft.Extensions.Logging;
    using System.ComponentModel.DataAnnotations;

    public class AccountController : Controller
    {
        private readonly FirebaseAuthService _firebaseAuthService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(FirebaseAuthService firebaseAuthService, ILogger<AccountController> logger)
        {
            _firebaseAuthService = firebaseAuthService;
            _logger = logger;
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

            // Pass the selected role to the view
            var selectedRole = TempData["SelectedRole"]?.ToString();
            var model = new RegisterStudentViewModel { Role = selectedRole };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Pass the role along with email and password to RegisterUser method
                    var userId = await _firebaseAuthService.RegisterUser(model.Email, model.Password, model.Role);

                    // Role is already assigned during user registration, so no need to call AssignUserRole again
                    _logger.LogInformation($"User registered successfully with UserId: {userId}");
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during registration.");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                _logger.LogWarning(error.ErrorMessage);
            }

            _logger.LogWarning("Invalid model state during registration.");
            return View(model);
        }


        /*
        [HttpGet]
        public IActionResult RegisterStaff()
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User already authenticated, redirecting to home.");
                return RedirectToAction("Index", "Home");
            }

            _logger.LogInformation("Staff registration page loaded.");


            // Pass the selected role to the view
            var selectedRole = TempData["SelectedRole"]?.ToString();
            // Initialize the RegisterViewModel with the role as "Staff"
            var model = new RegisterStaffViewModel { Role = selectedRole };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStaff(RegisterStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Register user as staff
                    var userId = await _firebaseAuthService.RegisterUser(model.Email, model.Password, model.Role);

                    _logger.LogInformation($"Staff registered successfully with UserId: {userId}");
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during staff registration.");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            _logger.LogWarning("Invalid model state during staff registration.");
            return View(model);
        }


        */


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



        public class PositionRequiredIfStaffAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (RegisterStudentViewModel)validationContext.ObjectInstance;

                if (model.Role == "Staff" && string.IsNullOrEmpty(model.Position))
                {
                    return new ValidationResult("The Position field is required for Staff.");
                }

                return ValidationResult.Success;
            }
        }



    }
}


