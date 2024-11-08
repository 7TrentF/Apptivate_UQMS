using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Apptivate_UQMS_WebApp.Services;
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
    private readonly IChatService _chatService;
    public ChatController(ApplicationDbContext context, IChatService chatService)
    {
        _context = context;
        _chatService = chatService;

    }

    public async Task<IActionResult> Chats()
    {
        var currentUserId = GetCurrentUserId();
        var users = await _chatService.GetChatUsersAsync(currentUserId);
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