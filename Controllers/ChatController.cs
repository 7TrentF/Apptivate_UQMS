using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.DTOs.AccountDto;

namespace Apptivate_UQMS_WebApp.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly ApplicationDbContext _context;

    public ChatController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Chats()
    {
        var currentUserId = GetCurrentUserId();

        // Fetch users based on role in a single query
        var users = await _context.Users
            .Where(u => u.UserID != currentUserId && // Exclude current user
                       (u.Role == "Admin" || u.Role == "Staff" ||
                       (u.Role == "Student" && u.Role == "Staff")))
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

        return View(users);
    }






    [HttpGet]
    public async Task<IActionResult> GetMessages(int userId)
    {
        var currentUserId = GetCurrentUserId();
        var messages = await _context.Messages
            .Where(m =>
                (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                (m.SenderId == userId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.Timestamp)
            .Select(m => new
            {
                m.MessageId,
                m.Content,
                m.Timestamp,
                IsFromCurrentUser = m.SenderId == currentUserId
            })
            .ToListAsync();
        return Json(messages);
    }

    [HttpGet("api/users/statuses")]
    public async Task<IActionResult> GetUserStatuses()
    {
        var statuses = await _context.Users
            .Select(u => new { userId = u.UserID, isOnline = u.IsOnline })
            .ToListAsync();
        return Ok(statuses);
    }



    private int GetCurrentUserId()
    {
        var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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