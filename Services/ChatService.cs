using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Apptivate_UQMS_WebApp.Services
{
    // IChatService.cs
    public interface IChatService
    {
        Task<List<ChatUserViewModel>> GetChatUsersAsync(int currentUserId);
    }

    // ChatService.cs
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatUserViewModel>> GetChatUsersAsync(int currentUserId)
        {
            // Fetch the current user's role from the database
            var currentUser = await _context.Users.FindAsync(currentUserId);
            var currentUserRole = currentUser?.Role;

            // Query users based on role
            var users = await _context.Users
                .Where(u => u.UserID != currentUserId) // Exclude current user
                .Where(u => currentUserRole == "Admin" || currentUserRole == "Staff" ||
                            (currentUserRole == "Student" && u.Role == "Staff"))
                .GroupJoin(_context.Messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead),
                           u => u.UserID,
                           m => m.SenderId,
                           (user, messages) => new ChatUserViewModel
                           {
                               UserID = user.UserID,
                               Name = user.Name,
                               Surname = user.Surname,
                               IsOnline = user.IsOnline,
                               LastSeen = user.LastSeen,
                               UnreadCount = messages.Count() // Count unread messages for each user
                           })
                .ToListAsync();

            return users;
        }
    }

}
