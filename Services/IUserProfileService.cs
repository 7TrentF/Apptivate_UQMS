using Apptivate_UQMS_WebApp.Data;
using static Apptivate_UQMS_WebApp.Models.Account;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using FirebaseAdmin;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using Google.Apis.Auth.OAuth2;
using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Apptivate_UQMS_WebApp.Extentions; // Add this using directive


namespace Apptivate_UQMS_WebApp.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileViewModel> GetUserProfileAsync(string firebaseUid);
    }

    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(ApplicationDbContext context, ILogger<UserProfileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserProfileViewModel> GetUserProfileAsync(string firebaseUid)
        {
            if (string.IsNullOrEmpty(firebaseUid))
            {
                _logger.LogError("Firebase UID is null or empty.");
                return null;
            }

            // Fetch user by Firebase UID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            if (user == null)
            {
                _logger.LogError("User with Firebase UID {FirebaseUID} not found.", firebaseUid);
                return null;
            }


            // Fetch student or staff details
            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);
            var staffDetail = await _context.StaffDetails
                                            .Include(s => s.Position)
                                            .FirstOrDefaultAsync(s => s.UserID == user.UserID);

            var userProfile = new UserProfileViewModel
            {
                User = user,
                StudentDetail = studentDetail,
                StaffDetail = staffDetail
            };

            return userProfile;
        }
    }



}
