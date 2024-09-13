using Apptivate_UQMS_WebApp.Models;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.Models.Account;

namespace Apptivate_UQMS_WebApp.Services
{
    public interface IUserRegistrationService
    {
        Task<User> RegisterUserAsync(RegisterViewModel model);
    }
}
