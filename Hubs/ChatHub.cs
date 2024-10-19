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

            // Create the new message object
            var newMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            // Save the message to the database
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Convert IDs to string as SignalR uses string identifiers
            var receiverIdString = receiverId.ToString();
            var senderIdString = senderId.ToString();

            // Send the message to the receiver if connected
            await Clients.User(receiverIdString).SendAsync("ReceiveMessage", senderId, message);

            // Also send the message to the sender for confirmation (to ensure real-time visibility)
            await Clients.User(senderIdString).SendAsync("ReceiveMessage", senderId, message);
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
                user.ConnectionId = isOnline ? Context.ConnectionId : null; // Save the connection ID
                await _context.SaveChangesAsync();
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