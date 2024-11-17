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
                    await HandleStaffConnection(staffId);
                }
                else if (!string.IsNullOrEmpty(studentId))
                {
                    await HandleStudentConnection(studentId);
                }
                else
                {
                    _logger.LogWarning("Connection attempt failed. No staffId or studentId in query string. Connection ID: {ConnectionId}", Context.ConnectionId);
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync for Connection ID: {ConnectionId}", Context.ConnectionId);
                throw;
            }
        }

        private async Task HandleStaffConnection(string staffId)
        {
            _logger.LogInformation("Staff member connecting. ID: {StaffId}, Connection: {ConnectionId}", staffId, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"staff_{staffId}");
            StaffConnections[Context.ConnectionId] = staffId;
        }

        private async Task HandleStudentConnection(string studentId)
        {
            _logger.LogInformation("Student connecting. ID: {StudentId}, Connection: {ConnectionId}", studentId, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"student_{studentId}");
            StudentConnections[Context.ConnectionId] = studentId;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var staffId = StaffConnections.GetValueOrDefault(Context.ConnectionId);
                var studentId = StudentConnections.GetValueOrDefault(Context.ConnectionId);

                if (!string.IsNullOrEmpty(staffId))
                {
                    await HandleStaffDisconnection(staffId);
                }
                else if (!string.IsNullOrEmpty(studentId))
                {
                    await HandleStudentDisconnection(studentId);
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync for Connection ID: {ConnectionId}", Context.ConnectionId);
                throw;
            }
        }

        private async Task HandleStaffDisconnection(string staffId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"staff_{staffId}");
            StaffConnections.Remove(Context.ConnectionId);
            _logger.LogInformation("Staff member disconnected. ID: {StaffId}", staffId);
        }

        private async Task HandleStudentDisconnection(string studentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"student_{studentId}");
            StudentConnections.Remove(Context.ConnectionId);
            _logger.LogInformation("Student disconnected. ID: {StudentId}", studentId);
        }

        // Method to notify student when feedback is given
        public async Task NotifyStudentFeedback(string studentId, int queryId, string message)
        {
            try
            {
                _logger.LogInformation("Sending feedback notification to student {StudentId} for query {QueryId}", studentId, queryId);

                var notification = new
                {
                    type = "feedback",
                    queryId = queryId,
                    message = message,
                    timestamp = DateTime.UtcNow
                };

                await Clients.Group($"student_{studentId}").SendAsync("ReceiveFeedbackNotification", notification);
                _logger.LogInformation("Feedback notification sent to student {StudentId}", studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending feedback notification to student {StudentId}", studentId);
                throw;
            }
        }

        // Method to notify staff (keeping existing functionality)
        public async Task NotifyStaff(string staffId, string message)
        {
            try
            {
                _logger.LogInformation("Sending notification to staff {StaffId}", staffId);
                await Clients.Group($"staff_{staffId}").SendAsync("ReceiveNotification", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to staff {StaffId}", staffId);
                throw;
            }
        }
    }
}
