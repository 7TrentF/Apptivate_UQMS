using Microsoft.AspNetCore.SignalR;
using Apptivate_UQMS_WebApp.Hubs;


namespace Apptivate_UQMS_WebApp.Services
{
    // Create a notification service (Services/NotificationService.cs)
    public interface INotificationService
    {
        Task NotifyStaffMember(int staffId, string queryDetails);
    }

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyStaffMember(int staffId, string queryDetails)
        {
            try
            {
                _logger.LogInformation("Attempting to send notification to staff ID: {StaffId}", staffId);
                await _hubContext.Clients.User(staffId.ToString())
                    .SendAsync("ReceiveNotification", $"New query assigned: {queryDetails}");
                _logger.LogInformation("Notification sent successfully to staff ID: {StaffId}", staffId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to staff ID: {StaffId}", staffId);
                throw;
            }
        }
    }

    
}
