using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;


namespace Apptivate_UQMS_WebApp.Hubs
{

    public class NotificationHub : Hub
    {

        private readonly ILogger<NotificationHub> _logger;
        private static readonly Dictionary<string, string> _connections = new();


        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }


        public async Task SendNotification(int userId, string message)
        {
            try
            {
                await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
                _logger.LogInformation($"Notification sent to user {userId}: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send notification to user {userId}: {message}");
            }
        }

        public async Task TestConnection()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Test connection received from user: {UserId}", userId);

            // Send a test message back to the calling client
            await Clients.Caller.SendAsync("ReceiveNotification", "Test notification - Connection working!");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Log connection details
            _logger.LogInformation(
                "User attempting to connect. Connection ID: {ConnectionId}, User ID: {UserId}",
                Context.ConnectionId,
                userId ?? "null"
            );

            if (!string.IsNullOrEmpty(userId))
            {
                _connections[Context.ConnectionId] = userId;
                _logger.LogInformation("User {UserId} connected with connection ID {ConnectionId}",
                    userId, Context.ConnectionId);
            }
            else
            {
                _logger.LogWarning("User connected but no user ID found. Connection ID: {ConnectionId}",
                    Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connections.Remove(Context.ConnectionId);
            _logger.LogInformation("User disconnected. Connection ID: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

    }

    

}
