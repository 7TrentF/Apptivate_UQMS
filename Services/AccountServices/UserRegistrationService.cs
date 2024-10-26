using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Models.AppUsers;
using static Apptivate_UQMS_WebApp.DTOs.AccountDto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apptivate_UQMS_WebApp.Services.AccountServices
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly FirebaseAuthService _firebaseAuthService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRegistrationService> _logger;

        public UserRegistrationService(FirebaseAuthService firebaseAuthService, ApplicationDbContext context, ILogger<UserRegistrationService> logger)
        {
            _firebaseAuthService = firebaseAuthService;
            _context = context;
            _logger = logger;
        }


        // Method to check password strength
        private bool IsPasswordStrong(string password)
        {
            // Define password strength criteria
            int strength = 0;
            if (password.Length >= 8) strength++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[A-Z]")) strength++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[a-z]")) strength++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[0-9]")) strength++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[^A-Za-z0-9]")) strength++;

            return strength >= 3; // Adjust as necessary for your requirements
        }

        public async Task<User> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                _logger.LogInformation($"Attempting to register user with email: {model.Email}");

                // Validate password strength
                if (!IsPasswordStrong(model.Password))
                {
                    _logger.LogWarning("Weak password provided during registration.");
                    throw new Exception("Password does not meet strength requirements.");
                }

                // Attempt Firebase registration
                string firebaseUID;
                try
                {
                    firebaseUID = await _firebaseAuthService.RegisterUser(model.Email, model.Password, model.Role);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Firebase registration failed.");
                    throw new Exception("Registration in Firebase failed.");
                }
                // Save user data to the Users table
                var user = new User
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    PasswordHash = _firebaseAuthService.GeneratePasswordHash(model.Password),
                    Role = model.Role,
                    RegistrationDate = DateTime.Now,

                    FirebaseUID = firebaseUID // Store Firebase UID
                };

                // Add the user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Save role-specific details
                if (model.Role == "Student")
                {
                    var studentDetail = new StudentDetail
                    {
                        UserID = user.UserID,
                        Department = _context.Departments.FirstOrDefault(d => d.DepartmentID == model.SelectedDepartmentID)?.DepartmentName,
                        Course = _context.Courses.FirstOrDefault(d => d.CourseID == model.SelectedCourseID)?.CourseCode,
                        Year = model.Year,
                    };
                    _context.StudentDetails.Add(studentDetail);
                }
                else if (model.Role == "Staff")
                {
                    var staffDetail = new StaffDetail
                    {
                        UserID = user.UserID,
                        Department = _context.Departments.FirstOrDefault(d => d.DepartmentID == model.SelectedDepartmentID)?.DepartmentName,
                        PositionID = model.SelectedPositionID.Value,
                        YearGroupTeaching = model.Year,
                    };
                    _context.StaffDetails.Add(staffDetail);
                }

                // Save changes to the database for role-specific details
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User registered successfully with email: {model.Email}");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during user registration: {ex.Message}");
                throw new Exception("Registration failed. Please check your input.");

                throw;
            }
        }

        public async Task<List<CourseDto>> GetDepartmentWithCoursesAsync(int departmentId)
        {

            _logger.LogInformation($"GetDepartmentWithCoursesAsync hit : {departmentId}");

            var courses = await _context.DepartmentCourses
                                        .Where(dc => dc.DepartmentID == departmentId)
                                        .Select(dc => new CourseDto
                                        {
                                            CourseID = dc.Course.CourseID,
                                            CourseCode = dc.Course.CourseCode
                                        })
                                        .ToListAsync();

            if (!courses.Any())
            {
                _logger.LogInformation($"No courses found for DepartmentID: {departmentId}");
            }

            return courses;
        }

        // method to find a user by Firebase UID
        public async Task<User> GetUserByFirebaseUidAsync(string firebaseUid)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
        }


        // methods for fetching departments, courses, and positions
        public async Task<IEnumerable<SelectListItem>> GetDepartmentsAsync()
        {
            return await _context.Departments
                                 .Select(d => new SelectListItem
                                 {
                                     Value = d.DepartmentID.ToString(),
                                     Text = d.DepartmentName
                                 }).ToListAsync();

         
        }

        public async Task<IEnumerable<SelectListItem>> GetCoursesAsync()
        {
            return await _context.Courses
                                 .Select(c => new SelectListItem
                                 {
                                     Value = c.CourseID.ToString(),
                                     Text = c.CourseName
                                 }).ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetPositionsAsync()
        {
            return await _context.Positions
                                 .Select(p => new SelectListItem
                                 {
                                     Value = p.PositionID.ToString(),
                                     Text = p.PositionName
                                 }).ToListAsync();
        }





        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {user.Email} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {user.Email}: {ex.Message}");
                throw;
            }
        }

    }
}
