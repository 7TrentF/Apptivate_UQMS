using Apptivate_UQMS_WebApp.Data;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Apptivate_UQMS_WebApp.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly Dictionary<string, string> StaffConnections = new();
        private static readonly Dictionary<string, string> StudentConnections = new();

        private readonly ILogger<NotificationHub> _logger;
        private readonly ApplicationDbContext _context;

        public NotificationHub(ApplicationDbContext context, ILogger<NotificationHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var staffId = Context.GetHttpContext()?.Request.Query["staffId"].ToString();
                var studentId = Context.GetHttpContext()?.Request.Query["studentId"].ToString();

                if (!string.IsNullOrEmpty(staffId))
                {
                    _logger.LogInformation("Staff member with ID {StaffId} is connecting. Connection ID: {ConnectionId}", staffId, Context.ConnectionId);

                    // Add connection to the staff member's group
                    await Groups.AddToGroupAsync(Context.ConnectionId, staffId);

                    // Store in dictionary to track
                    StaffConnections[Context.ConnectionId] = staffId;

                    _logger.LogInformation("Staff member with ID {StaffId} connected and added to group. Connection ID: {ConnectionId}", staffId, Context.ConnectionId);
                }
                else if (!string.IsNullOrEmpty(studentId))
                {
                    _logger.LogInformation("Student with ID {StudentId} is connecting. Connection ID: {ConnectionId}", studentId, Context.ConnectionId);

                    // Add connection to the student's group
                    await Groups.AddToGroupAsync(Context.ConnectionId, studentId);

                    // Store in dictionary to track
                    StudentConnections[Context.ConnectionId] = studentId;

                    _logger.LogInformation("Student with ID {StudentId} connected and added to group. Connection ID: {ConnectionId}", studentId, Context.ConnectionId);
                }
                else
                {
                    _logger.LogWarning("Connection attempt failed. No valid staffId or studentId in query string. Connection ID: {ConnectionId}", Context.ConnectionId);
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing OnConnectedAsync for Connection ID: {ConnectionId}", Context.ConnectionId);
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var staffId = StaffConnections.GetValueOrDefault(Context.ConnectionId);
                if (!string.IsNullOrEmpty(staffId))
                {
                    _logger.LogInformation("Staff member with ID {StaffId} is disconnecting. Connection ID: {ConnectionId}", staffId, Context.ConnectionId);

                    // Remove from group when disconnecting
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, staffId);
                    StaffConnections.Remove(Context.ConnectionId);

                    _logger.LogInformation("Staff member with ID {StaffId} removed from group and connection removed. Connection ID: {ConnectionId}", staffId, Context.ConnectionId);
                }

                var studentId = StudentConnections.GetValueOrDefault(Context.ConnectionId);
                if (!string.IsNullOrEmpty(studentId))
                {
                    _logger.LogInformation("Student with ID {StudentId} is disconnecting. Connection ID: {ConnectionId}", studentId, Context.ConnectionId);

                    // Remove from group when disconnecting
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, studentId);
                    StudentConnections.Remove(Context.ConnectionId);

                    _logger.LogInformation("Student with ID {StudentId} removed from group and connection removed. Connection ID: {ConnectionId}", studentId, Context.ConnectionId);
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing OnDisconnectedAsync for Connection ID: {ConnectionId}", Context.ConnectionId);
                throw;
            }
        }

        // Notify staff members via their group
        public async Task NotifyStaff(string staffId, string message)
        {
            try
            {
                _logger.LogInformation("Sending notification to staff with ID {StaffId}. Message: {Message}", staffId, message);

                await Clients.Group(staffId).SendAsync("ReceiveNotification", message);

                _logger.LogInformation("Notification sent to staff with ID {StaffId}.", staffId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending notification to staff with ID {StaffId}.", staffId);
                throw;
            }
        }

        // Notify students via their group
        public async Task NotifyStudent(string studentId, string message)
        {
            try
            {
                _logger.LogInformation("Sending notification to student with ID {StudentId}. Message: {Message}", studentId, message);

                await Clients.Group(studentId).SendAsync("ReceiveNotification", message);

                _logger.LogInformation("Notification sent to student with ID {StudentId}.", studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending notification to student with ID {StudentId}.", studentId);
                throw;
            }
        }
    }

}
