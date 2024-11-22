using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Hubs;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Query = Apptivate_UQMS_WebApp.Models.QueryModel.Query;

using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Models.AssignQuery;
using QueryStatus = Apptivate_UQMS_WebApp.Models.QueryModel.QueryStatus;

namespace Apptivate_UQMS_WebApp.Services.QueryServices
{

    public interface IAssignQueryService
    {
        Task<string> ReassignQueryAsync(int queryId, int newStaffId, string firebaseUid);

        Task<List<TeamMemberModel>> GetTeamMembersAsync(int currentUserId);
    }


    public class AssignQueryService : IAssignQueryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<AssignQueryService> _logger;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public AssignQueryService(
            ApplicationDbContext context,
            ILogger<AssignQueryService> logger,
            IEmailService emailService,
            INotificationService notificationService,
            IHubContext<NotificationHub> notificationHubContext)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _notificationService = notificationService;
            _notificationHubContext = notificationHubContext;
        }


        public async Task<string> ReassignQueryAsync(int queryId, int newStaffId, string firebaseUid)
        {
            try
            {
                // Find the current staff member
                var currentStaff = await _context.StaffDetails
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.User.FirebaseUID == firebaseUid);

                if (currentStaff == null)
                    return "Staff not found for the provided Firebase UID.";

                // Find the query to be reassigned
                var queryAssignment = await _context.QueryAssignments
                    .FirstOrDefaultAsync(qa => qa.QueryID == queryId && qa.StaffID == currentStaff.StaffID);

                if (queryAssignment == null)
                    return "Query not found or not assigned to the current user.";

                // Find the new staff member
                var newStaff = await _context.StaffDetails
                    .FirstOrDefaultAsync(s => s.StaffID == newStaffId);

                if (newStaff == null)
                    return "New staff member not found.";

                // Update query assignment
                queryAssignment.StaffID = newStaffId;
                queryAssignment.AssignedDate = DateTime.Now;

                var query = await _context.Queries
                    .Where(q => q.QueryID == queryId)
                    .FirstOrDefaultAsync(q => q.QueryID == queryId);

                var ticketNumber = await _context.Queries
                    .Where(q => q.QueryID == queryId)
                    .Select(q => q.TicketNumber)
                    .FirstOrDefaultAsync();



                if (query != null)
                    query.Status = QueryStatus.Reassigned;

                await _context.SaveChangesAsync();

                // Notifications
                await _notificationHubContext.Clients.Group(newStaffId.ToString())
                    .SendAsync("ReceiveNotification", $"Ticket Number #{ticketNumber} has been reassigned to you.");

                await _notificationService.NotifyStaffMember(newStaffId, $"Ticket Number #{ticketNumber} has been reassigned to you.");

                await SendQueryReassignmentNotificationEmailAsync(query, newStaffId);

                _logger.LogInformation("Query {QueryID} reassigned from staff {OldStaffID} to staff {NewStaffID}.",
                    queryId, currentStaff.StaffID, newStaffId);

                return "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reassigning query {QueryID}.", queryId);
                return "An error occurred while reassigning the query.";
            }
        }

 




        private async Task SendQueryReassignmentNotificationEmailAsync(Query query, int staffId)
        {
            _logger.LogInformation($"SendQuerySubmissionNotificationEmailStaff with staffId: {staffId}");


            // Fetch the user's email based on the Firebase UID
            var userId = await _context.StaffDetails
                 .Where(u => u.StaffID == staffId) // Replace with the actual column name for Firebase UID
                .Select(u => u.UserID)
                .FirstOrDefaultAsync();


            if (userId == null)
            {
                _logger.LogError("UserId not found.");
                throw new Exception("UserId not found.");
            }


            var staffEmail = await _context.Users
                .Where(u => u.UserID == userId) // Replace with the actual column name for Firebase UID
                .Select(u => u.Email)
                .FirstOrDefaultAsync();


            if (staffEmail == null)
            {
                _logger.LogError("Staff email not found.");
                throw new Exception("Staff email  details not found.");
            }

            _logger.LogInformation("This is the email you require {UserEmail}", staffEmail);


            try
            {
                var emailData = new Dictionary<string, object>
        {
            { "user_name", staffEmail.Split('@')[0] },
            { "query_description", query.Description.Substring(0, Math.Min(50, query.Description.Length)) },
            { "ticket_number", query.TicketNumber },
            { "submitted_date", query.SubmissionDate?.ToString("f") }
        };

                // Replace '1' with your actual email template ID
                await _emailService.SendTemplateEmailAsync(staffEmail, 2, emailData);
                _logger.LogInformation("Notification email sent to user {UserEmail}", staffEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send query submission notification email to {UserEmail}: {Message}", staffEmail, ex.Message);
                throw new ApplicationException("Failed to send notification email", ex);
            }
        }




        public async Task<List<TeamMemberModel>> GetTeamMembersAsync(int currentUserId)
        {
            // Fetch the current user's details, including their department
            var currentUser = await _context.StaffDetails
                .Include(sd => sd.User)
                .FirstOrDefaultAsync(sd => sd.UserID == currentUserId);

            if (currentUser == null)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            var currentUserRole = currentUser.User.Role;
            var currentUserDepartment = currentUser.Department;

            // Ensure only authorized users can view team members
            if (currentUserRole != "Staff" && currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("Only Staff or Admin users can view team members.");
            }

            // Query staff in the same department, excluding the current user
            var teamMembers = await _context.StaffDetails
                .Where(sd => sd.Department == currentUserDepartment && sd.UserID != currentUserId)
                .Select(sd => new TeamMemberModel
                {
                    StaffID = sd.StaffID,
                    StaffName = sd.User.Name + " " + sd.User.Surname,
                    Position = sd.Position.PositionName, // Assuming PositionName is in the Position entity
                    Department = sd.Department
                })
                .ToListAsync();

            return teamMembers;
        }







    }
}
