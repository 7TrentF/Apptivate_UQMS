using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.Models.Account;

namespace Apptivate_UQMS_WebApp.Services
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

        public async Task<User> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                _logger.LogInformation($"Attempting to register user with email: {model.Email}");

                // Register the user in Firebase and retrieve the userId
               // var firebaseUserId = await _firebaseAuthService.RegisterUser(model.Email, model.Password, model.Role);
                // Call Firebase to create the user and get Firebase UID
                string firebaseUID = await _firebaseAuthService.RegisterUser(model.Email, model.Password, model.Role);

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
                throw;
            }
        }
    }
}
