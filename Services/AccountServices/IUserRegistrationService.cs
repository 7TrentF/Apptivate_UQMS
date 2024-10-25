using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.DTOs.AccountDto;

namespace Apptivate_UQMS_WebApp.Services.AccountServices
{
    public interface IUserRegistrationService
    {
        Task<User> RegisterUserAsync(RegisterViewModel model);


        // Add the following methods
        // Add these methods
        Task<IEnumerable<SelectListItem>> GetDepartmentsAsync();
        Task<IEnumerable<SelectListItem>> GetCoursesAsync();
        Task<IEnumerable<SelectListItem>> GetPositionsAsync();

        Task<List<CourseDto>> GetDepartmentWithCoursesAsync(int departmentId);


        // New method to get user by Firebase UID
        Task<User> GetUserByFirebaseUidAsync(string firebaseUid);

        Task UpdateUserAsync(User user);


    }
}
