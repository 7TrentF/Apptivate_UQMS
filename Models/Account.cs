using Google.Apis.Util;
using System.ComponentModel.DataAnnotations;
using static Apptivate_UQMS_WebApp.Controllers.AccountController;

namespace Apptivate_UQMS_WebApp.Models
{
    public class Account
    {
      public class RegisterStudentViewModel
{
            [Required]
            public string? Name { get; set; }

            [Required]
            public string Surname { get; set; }

            
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            public string Role { get; set; }
            [Required]
            public string Department { get; set; }  

            [Required]
            public string Course { get; set; }  

            public string? Position { get; set; }

        }

        public class RegisterStaffViewModel
        {
            public string Name { get; set; }
            public string Surname { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            public string Role { get; set; }
            public string Department { get; set; }  
            public string Course { get; set; }  

            public string Position { get; set; }  
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

        public class StaffLoginViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }



        public class StudentDetails
        {
            public int StudentDetailsID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }
            public string Course { get; set; }
        }

        public class StaffDetails
        {
            public int StaffDetailsID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }
            public string Position { get; set; }
        }

        public class AdminDetails
        {
            public int AdminDetailsID { get; set; }
            public int UserID { get; set; }
            public string Department { get; set; }
        }

    }
}
