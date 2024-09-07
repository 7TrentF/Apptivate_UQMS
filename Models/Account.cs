﻿using Google.Apis.Util;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static Apptivate_UQMS_WebApp.Controllers.AccountController;

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

            // Selected department ID
            [Required]
            public int? SelectedDepartmentID { get; set; }

            [Required]
            public int? SelectedCourseID { get; set; }

            [Required]
            public int? Year { get; set; }

            public string? Position { get; set; }


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

        public class User
        {
            [Key]
            public int UserID { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public string Role { get; set; }
            public DateTime RegistrationDate { get; set; } = DateTime.Now;


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

            // Navigation property
            public User User { get; set; } 
        }

        public class StaffDetail
        {
            [Key]
            public int StaffID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }

            public string? Position { get; set; }
            public int? YearGroupTeaching { get; set; }

            // Navigation property
            public User User { get; set; }
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
        }

        public class Course
        {
            [Key]
            public int CourseID { get; set; }
            public string? CourseCode { get; set; }
            public string?  CourseName { get; set; }

            public ICollection<DepartmentCourse> DepartmentCourses { get; set; } = new HashSet<DepartmentCourse>();
            public ICollection<CourseModule> CourseModules { get; set; } = new HashSet<CourseModule>();
        }

        public class Module
        {
            public int ModuleID { get; set; }
            public string ModuleCode { get; set; }
            public string ModuleName { get; set; }

            public ICollection<CourseModule> CourseModules { get; set; }
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
