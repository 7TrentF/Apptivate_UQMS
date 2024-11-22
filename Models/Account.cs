using Google.Apis.Util;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static Apptivate_UQMS_WebApp.Controllers.AccountController;
using static Apptivate_UQMS_WebApp.Models.QueryModel;

namespace Apptivate_UQMS_WebApp.Models
{
    public class Account
    {
        public class RegisterViewModel
        {
            [Required]
            public string? Name { get; set; }

            [Required]
            public string? Surname { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string? Email { get; set; }

            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
            [Required]
            public string? Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string? ConfirmPassword { get; set; }

            public string Role { get; set; }

            public string? PhoneNumber { get; set; }


            // Selected department ID
            [Required]
            public int? SelectedDepartmentID { get; set; }

            [Required]
            public int? SelectedCourseID { get; set; }

            [Required]
            public int? Year { get; set; }

           public string ? studentNumber { get; set; }

            public int? SelectedPositionID { get; set; }

            public IEnumerable<SelectListItem> Positions { get; set; } = new HashSet<SelectListItem>();


            // List of departments for dropdown
            public IEnumerable<SelectListItem> Departments { get; set; } = new HashSet<SelectListItem>();
            public IEnumerable<SelectListItem> Courses { get; set; } = new HashSet<SelectListItem>();
        }

        public class LoginViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public class UserProfileViewModel
        {
            public Account.User User { get; set; }
            public Account.StudentDetail StudentDetail { get; set; }
            public Account.StaffDetail StaffDetail { get; set; }
        }

        public class User
        {
            [Key]
            public int UserID { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public string Role { get; set; }
            public string? PhoneNumber { get; set; }

            public DateTime RegistrationDate { get; set; } = DateTime.Now;

            public string? FirebaseUID { get; set; } // Add this field
                                                     // New LastSeen property
            public DateTime? LastSeen { get; set; } // Nullable to allow for uninitialized values
            public string? ConnectionId { get; set; }
            public bool IsOnline { get; set; }



            // Navigation properties
            public ICollection<StudentDetail> StudentDetails { get; set; }
            public ICollection<StaffDetail> StaffDetails { get; set; }  
            public ICollection<AdminDetail> AdminDetails { get; set; } 
        }


        public class StudentDetail
        {
            [Key]
            public int StudentID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }
            public string Course { get; set; }
            public int? Year { get; set; }

            public string? studentNumber { get; set; }


            // Navigation property for User
            public User User { get; set; }

            // Navigation property for related queries
            public ICollection<Query> Queries { get; set; }
        }

        public class StaffDetail
        {
            [Key]
            public int StaffID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }
          
            public int? YearGroupTeaching { get; set; }

            // Foreign Key for Position
            public int? PositionID { get; set; }
            public Position Position { get; set; }

            // Navigation property for User
            public User User { get; set; }



            // Navigation property for query assignments
            public ICollection<QueryAssignment> QueryAssignments { get; set; }
        }

        public class Position
        {
            [Key]
            public int PositionID { get; set; }  // Primary Key
            public string PositionName { get; set; }  // Name of the Position

            // Navigation property to link staff details to positions
            public ICollection<StaffDetail> StaffDetails { get; set; }
        }

        public class AdminDetail
        {
            [Key]
            public int AdminID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }

            // Navigation property
            public User User { get; set; }
        }

        public class Department
        {
            [Key]
            public int DepartmentID { get; set; }
            public string? DepartmentName { get; set; }

            public ICollection<DepartmentCourse> DepartmentCourses { get; set; } = new HashSet<DepartmentCourse>();

            // Navigation property for queries
            public ICollection<Query> Queries { get; set; }
        }

        public class Course
        {
            [Key]
            public int CourseID { get; set; }
            public string? CourseCode { get; set; }
            public string? CourseName { get; set; }

            public ICollection<DepartmentCourse> DepartmentCourses { get; set; } = new HashSet<DepartmentCourse>();
            public ICollection<CourseModule> CourseModules { get; set; } = new HashSet<CourseModule>();

            // Navigation property for queries
            public ICollection<Query> Queries { get; set; }
        }

        public class Module
        {
            public int ModuleID { get; set; }
            public string ModuleCode { get; set; }
            public string ModuleName { get; set; }

            public ICollection<CourseModule> CourseModules { get; set; }

            // Navigation property for queries
            public ICollection<Query> Queries { get; set; }
        }


        public class DepartmentCourse
        {
            public int DepartmentCourseID { get; set; }
            public int DepartmentID { get; set; }
            public Department Department { get; set; }

            public int CourseID { get; set; }
            public Course Course { get; set; }
        }

        public class CourseModule
        {
            public int CourseModuleID { get; set; }
            public int CourseID { get; set; }
            public Course Course { get; set; }

            public int ModuleID { get; set; }
            public Module Module { get; set; }

        }




    }
}
