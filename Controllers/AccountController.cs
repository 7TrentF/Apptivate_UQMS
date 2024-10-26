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
using Apptivate_UQMS_WebApp.Services.AccountServices;
using Azure.Core;


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
        public async Task<IActionResult> GetCoursesByDepartment(int departmentId)
        {
            _logger.LogInformation($"GetDepartmentWithCourses hit !! : {departmentId}");


            var courses = await _userRegistrationService.GetDepartmentWithCoursesAsync(departmentId);
            if (courses == null || !courses.Any())
            {
                return NotFound(); // Return 404 if no courses are found
            }
            return Ok(courses); // Return the list of courses with IDs and codes
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User already authenticated, redirecting to home.");
                return RedirectToAction("Index", "Dashboard");
            }

            _logger.LogInformation("Register page loaded.");


            var departments = await _userRegistrationService.GetDepartmentsAsync();
            var courses = await _userRegistrationService.GetCoursesAsync();
            var positions = await _userRegistrationService.GetPositionsAsync();
          

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
                    var user = await _userRegistrationService.RegisterUserAsync(model);
                    _logger.LogInformation("User registered successfully.");

                    return Json(new { success = true, message = "Registration successful! You can now login." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Registration error.");
                    return Json(new { success = false, message = ex.Message });
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

            // If ModelState is invalid, collect all error messages
            var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Combine all error messages into a single string
            string combinedErrors = string.Join(" ", errorMessages);
            _logger.LogWarning($"Registration failed: {combinedErrors}");

            return Json(new { success = false, message = combinedErrors });
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
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }


        public async Task UpdateLastSeen(string firebaseUid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            if (user != null)
            {
                user.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
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

                    // Update the LastSeen property
                    user.LastSeen = DateTime.UtcNow;
                    await _context.SaveChangesAsync(); // Ensure that changes are saved to the database


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
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginViewModel model)
        {
            _logger.LogInformation("GoogleLogin attempt initiated");

            try
            {
                // Check if IdToken is provided
                if (string.IsNullOrEmpty(model.IdToken))
                {
                    _logger.LogWarning("GoogleLogin failed: IdToken is missing from request.");
                    return BadRequest(new { error = "IdToken is required" });
                }

                _logger.LogInformation("here is your token grump {UID}", model.IdToken);


                // Step 1: Verify the Firebase ID token
                _logger.LogInformation("Verifying Firebase ID token.");
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(model.IdToken);
                string uid = decodedToken.Uid;
                _logger.LogInformation("Firebase ID token verified successfully for UID: {UID}", uid);

                // Step 2: Fetch Firebase user details
                _logger.LogInformation("Retrieving user information for UID: {UID}", uid);
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                _logger.LogInformation("User information retrieved successfully. Email: {Email}, Display Name: {DisplayName}", userRecord.Email, userRecord.DisplayName);

                // Step 3: Check if user exists in the database
                var user = await _context.Users.SingleOrDefaultAsync(u => u.FirebaseUID == uid);

                if (user == null)
                {
                    _logger.LogError("User not found in the database for UID: {UID}", uid);
                    return BadRequest(new { error = "User not found." });
                }

                // Step 4: Update the LastSeen property and save
                user.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Step 5: Store the Firebase token in a cookie for persistent login
                var token = model.IdToken; // Or retrieve an updated token if necessary
                Response.Cookies.Append("FirebaseToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    SameSite = SameSiteMode.Strict
                });

                // Step 6: Store FirebaseUID in session
                HttpContext.Session.SetString("FirebaseUID", user.FirebaseUID);

                _logger.LogInformation("GoogleLogin successful for user {Email} with role {Role}", user.Email, user.Role);
                // Instead of redirecting, return a JSON response with the redirect URL
                var redirectUrl = user.Role switch
                {
                    "Student" => Url.Action("StudentDashboard", "Dashboard"),
                    "Staff" => Url.Action("StaffDashboard", "Dashboard"),
                    "Admin" => Url.Action("AdminDashboard", "Dashboard"),
                    _ => null
                };

                if (redirectUrl == null)
                {
                    return Json(new { success = false, error = "Unknown role." });
                }

                // Set cookies and session as before
                Response.Cookies.Append("FirebaseToken", model.IdToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    SameSite = SameSiteMode.Strict
                });

                HttpContext.Session.SetString("FirebaseUID", user.FirebaseUID);

                return Json(new
                {
                    success = true,
                    redirectUrl = redirectUrl
                });
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(ex, "GoogleLogin failed during Firebase authentication");
                return Json(new { success = false, error = "Invalid or expired token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during GoogleLogin");
                return Json(new { success = false, error = "Google login failed due to an unexpected error" });
            }
        }




        // GoogleLoginViewModel.cs
        public class GoogleLoginViewModel
    {

            public string IdToken { get; set; }
            public string Provider { get; set; }


        }



        [HttpPost]
        public async Task<IActionResult> ExternalLoginCallback([FromBody] ExternalLoginRequest request)
        {
            try
            {
                // Verify the Firebase ID token
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
                string uid = decodedToken.Uid;

                // Get the user's info
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);


                // Here you can:
                // 1. Create or update user in your database
                // 2. Set up authentication cookie
                // 3. Create claims and roles

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    


    public class ExternalLoginRequest
    {
        public string IdToken { get; set; }
        public string Provider { get; set; }
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


