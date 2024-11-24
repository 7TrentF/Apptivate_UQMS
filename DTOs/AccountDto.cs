using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.DTOs.QueryModelDto;

namespace Apptivate_UQMS_WebApp.DTOs
{
    public class AccountDto
    {
        public class RegisterViewModelDto
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

            // Selected department ID
            [Required]
            public int? SelectedDepartmentID { get; set; }

            [Required]
            public int? SelectedCourseID { get; set; }

            [Required]
            public int? Year { get; set; }

            
            public int? SelectedPositionID { get; set; }

        }

        public class DropdownOptionDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }


        public class LoginViewModelDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public class UserDto
        {
            [Key]
            public int UserID { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public string Role { get; set; }
            public DateTime RegistrationDate { get; set; } = DateTime.Now;

            public string? FirebaseUID { get; set; } // Add this field
                                                     // New LastSeen property
            public DateTime? LastSeen { get; set; } // Nullable to allow for uninitialized values


            // Navigation properties
         //   public ICollection<StudentDetailDto> StudentDetails { get; set; }
           // public ICollection<StaffDetailDto> StaffDetails { get; set; }
          //  public ICollection<AdminDetailDto> AdminDetails { get; set; }
        }

        public class UserProfileViewModelDto
        {
            public AccountDto.UserDto User { get; set; }
            public AccountDto.StudentDetailDto StudentDetail { get; set; }
            public AccountDto.StaffDetailDto StaffDetail { get; set; }
        }

        public class UserResponseDto
        {
            public int UserID { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string FirebaseUID { get; set; }
            public DateTime? LastSeen { get; set; }
            // Exclude navigation properties like StaffDetails, StudentDetails, etc.
        }


        public class StudentDetailDto
        {
            [Key]
            public int StudentID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }
            public string Course { get; set; }
            public int? Year { get; set; }

            // Navigation property for User
           // public UserDto User { get; set; }

            // Navigation property for related queries
           // public ICollection<QueryDto> Queries { get; set; }
        }

        public class StaffDetailDto
        {
            [Key]
            public int StaffID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }

            public int? YearGroupTeaching { get; set; }

            // Foreign Key for Position
            public int? PositionID { get; set; }
            public PositionDto Position { get; set; }

            // Navigation property for User
            public UserDto User { get; set; }

            // Navigation property for query assignments
            public ICollection<QueryAssignmentDto> QueryAssignments { get; set; }
        }

        public class PositionDto
        {
            [Key]
            public int PositionID { get; set; }  // Primary Key
            public string PositionName { get; set; }  // Name of the Position

            // Navigation property to link staff details to positions
            public ICollection<StaffDetailDto> StaffDetails { get; set; }
        }

        public class AdminDetailDto
        {
            [Key]
            public int AdminID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }

            // Navigation property
            public UserDto User { get; set; }
        }

        public class DepartmentDto
        {
            [Key]
            public int DepartmentID { get; set; }
            public string? DepartmentName { get; set; }

            public ICollection<DepartmentCourseDto> DepartmentCourses { get; set; } = new HashSet<DepartmentCourseDto>();

            // Navigation property for queries
            public ICollection<QueryDto> Queries { get; set; }
        }

        public class CourseDto
        {
            [Key]
            public int CourseID { get; set; }
            public string? CourseCode { get; set; }
            public string? CourseName { get; set; }

            public ICollection<DepartmentCourseDto> DepartmentCourses { get; set; } = new HashSet<DepartmentCourseDto>();
            public ICollection<CourseModuleDto> CourseModules { get; set; } = new HashSet<CourseModuleDto>();

            // Navigation property for queries
            public ICollection<QueryDto> Queries { get; set; }
        }

        public class CourseDTO
        {
            public int CourseID { get; set; }
            public string CourseCode { get; set; }
        }

        public class ModuleDto
        {
            public int ModuleID { get; set; }
            public string ModuleCode { get; set; }
            public string ModuleName { get; set; }

          //  public ICollection<CourseModuleDto> CourseModules { get; set; }

            // Navigation property for queries
         //   public ICollection<QueryDto> Queries { get; set; }
        }

        public class DepartmentCourseDto
        {
            public int DepartmentCourseID { get; set; }
            public int DepartmentID { get; set; }
            public DepartmentDto Department { get; set; }

            public int CourseID { get; set; }
            public CourseDto Course { get; set; }

            // Add this property to hold CourseCode
            public string CourseCode { get; set; }
        }

        public class CourseModuleDto
        {
            public int CourseModuleID { get; set; }
            public int CourseID { get; set; }
            public CourseDto Course { get; set; }

            public int ModuleID { get; set; }
            public ModuleDto Module { get; set; }

        }




    }
}
