using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Apptivate_UQMS_WebApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(int receiverId, string message)
        {
            var senderId = GetCurrentUserId();
            var newMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, new { content = message, timestamp = newMessage.Timestamp });
            await Clients.Caller.SendAsync("ReceiveMessage", senderId, new { content = message, timestamp = newMessage.Timestamp });
        }

        public async Task MarkAsRead(int senderId)
        {
            var currentUserId = GetCurrentUserId();
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == currentUserId && !m.IsRead)
                .ToListAsync();
            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
            await Clients.User(senderId.ToString()).SendAsync("MessagesRead", currentUserId);
            await Clients.Caller.SendAsync("UnreadCountUpdated", senderId, 0);
        }

        public override async Task OnConnectedAsync()
        {
            await UpdateUserStatus(true);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await UpdateUserStatus(false);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task UpdateUserStatus(bool isOnline)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Broadcast the status change to all connected clients
                await Clients.All.SendAsync("UserStatusChanged", userId, isOnline);
            }
        }

        private int GetCurrentUserId()
        {
            var firebaseUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(firebaseUserId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var user = _context.Users.FirstOrDefault(u => u.FirebaseUID == firebaseUserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found in the database.");
            }
            return user.UserID;
        }
    }
}