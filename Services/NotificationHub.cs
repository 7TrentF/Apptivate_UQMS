using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Apptivate_UQMS_WebApp.Services
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string staffId, string message)
        {
            // Notify only the specific staff member based on their connection ID or group
            await Clients.Group(staffId).SendAsync("ReceiveNotification", message);
        }

        public override Task OnConnectedAsync()
        {
            var staffId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(staffId))
            {
                Groups.AddToGroupAsync(Context.ConnectionId, staffId);
            }

            return base.OnConnectedAsync();
        }

        public override  Task OnDisconnectedAsync(Exception exception)
        {
            var staffId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(staffId))
            {
                Groups.RemoveFromGroupAsync(Context.ConnectionId, staffId);
            }
          //  _logger.LogInformation("Connected: StaffID {StaffID} is subscribed to group.", staffId);

            return base.OnDisconnectedAsync(exception);
        }


    }

}
