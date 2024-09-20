using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using static Apptivate_UQMS_WebApp.Models.Account;
using Microsoft.Extensions.Logging;
using Apptivate_UQMS_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;


namespace Apptivate_UQMS_WebApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly FirebaseAuthService _firebaseAuthService;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserProfileService _userProfileService;

        public AccountController(FirebaseAuthService firebaseAuthService, ILogger<AccountController> logger, ApplicationDbContext context,IUserRegistrationService userRegistrationService, IUserProfileService userProfileService)
        {
            _firebaseAuthService = firebaseAuthService;
            _logger = logger;
            _context = context; // Assign the injected context
            _userRegistrationService = userRegistrationService;
            _userProfileService = userProfileService;
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

            var positions = _context.Positions.Select(p => new SelectListItem
            {
                Value = p.PositionID.ToString(),
                Text = p.PositionName
            }).ToList();

            // Pass the selected role to the view
            var selectedRole = TempData["SelectedRole"]?.ToString();
            var model = new RegisterViewModel
            {
                Role = selectedRole,
                Departments = departments, // Populate departments in the ViewModel
                Courses = courses,
                Positions = positions
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




        // Method to automatically log in the user if a valid Firebase token exists
        private async Task<bool> TryAutoLoginAsync()
        {
            // Check if the Firebase token is stored in cookies
            if (Request.Cookies.TryGetValue("FirebaseToken", out var token))
            {
                try
                {
                    // Validate the Firebase token
                    var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                    var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserAsync(decodedToken.Uid);

                    // Find the user in your SQL database by Firebase UID
                    var user = await _context.Users.SingleOrDefaultAsync(u => u.FirebaseUID == firebaseUser.Uid);

                    if (user != null)
                    {
                        // Sign the user in by setting the session
                        HttpContext.Session.SetString("FirebaseUID", user.FirebaseUID);
                        _logger.LogInformation($"Auto-login successful for user {user.Email}.");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during auto-login: {ex.Message}");
                }
            }

            return false;
        }


        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (await TryAutoLoginAsync())
            {
                // If auto-login is successful, redirect to home
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Firebase login
                    var token = await _firebaseAuthService.LoginUser(model.Email, model.Password);

                    // Fetch Firebase user info
                    var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(model.Email);

                    // Find the user in the database by Firebase UID
                    var user = await _context.Users.SingleOrDefaultAsync(u => u.FirebaseUID == firebaseUser.Uid);

                    if (user == null)
                    {
                        _logger.LogError("User not found in the database.");
                        ModelState.AddModelError(string.Empty, "User not found.");
                        return View(model);
                    }

                    // Store the Firebase token in a cookie (for persistent login)
                    Response.Cookies.Append("FirebaseToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTimeOffset.UtcNow.AddDays(7), // Persistent for 7 days
                        SameSite = SameSiteMode.Strict
                    });

                    // Store FirebaseUID in session
                    HttpContext.Session.SetString("FirebaseUID", user.FirebaseUID);

                    _logger.LogInformation($"User {user.Email} logged in successfully.");

                    // Role-based redirection
                    switch (user.Role)
                    {
                        case "Student":
                            return RedirectToAction("StudentDashboard", "Dashboard");
                        case "Staff":
                            return RedirectToAction("StaffDashboard", "Dashboard");
                        case "Admin":
                            return RedirectToAction("AdminDashboard", "Dashboard");
                        default:
                            _logger.LogError($"Unknown role: {user.Role}");
                            ModelState.AddModelError(string.Empty, "Unknown role.");
                            return View(model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during login.");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }




        [HttpPost]
        public IActionResult Logout()
        {
            // Log information about logout
            _logger.LogInformation("User logged out.");

            // Delete the FirebaseToken cookie
            Response.Cookies.Delete("FirebaseToken");

            // Clear session
            HttpContext.Session.Clear();


            // Redirect to the login page or another public page
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return RedirectToAction("Login");
            }

            var userProfile = await _userProfileService.GetUserProfileAsync(firebaseUid);
            if (userProfile == null)
            {
                return NotFound("User not found.");
            }

            return View(userProfile);
        }


        /*
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Retrieve the FirebaseUID or user email for logging purposes
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid != null)
            {
                _logger.LogInformation($"User with FirebaseUID: {firebaseUid} is logging out.");
            }
            else
            {
                _logger.LogWarning("Unknown user is attempting to log out.");
            }

            // Clear the session and remove the FirebaseToken cookie
            HttpContext.Session.Clear();
            Response.Cookies.Delete("FirebaseToken");

            _logger.LogInformation("Session cleared and FirebaseToken cookie deleted.");
            _logger.LogInformation("User logged out successfully.");

            // Redirect to the login page
            return RedirectToAction("Login");
        }

        */

    }
}


