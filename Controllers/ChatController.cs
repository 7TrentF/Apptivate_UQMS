using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        var users = await _context.Users
            .Where(u => u.UserID != currentUserId)
            .Select(u => new ChatUserViewModel
            {
                UserID = u.UserID,
                Name = u.Name,
                Surname = u.Surname,
                IsOnline = u.IsOnline,
                LastSeen = u.LastSeen,
                UnreadCount = _context.Messages.Count(m => m.SenderId == u.UserID && m.ReceiverId == currentUserId && !m.IsRead)
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