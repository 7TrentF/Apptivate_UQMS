using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Apptivate_UQMS_WebApp.Controllers
{
    // Example Controller Usage
    public class NotificationController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(IEmailService emailService, ILogger<NotificationController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IActionResult> SendNotification(string userEmail)
        {
            _logger.LogInformation("Sending standard notification email to {UserEmail}", userEmail);

            try
            {
                await _emailService.SendEmailAsync(
                    userEmail,
                    "Important Update",
                    "<h1>Your action is required</h1><p>Please review the latest changes...</p>"
                );

                _logger.LogInformation("Notification email sent successfully to {UserEmail}", userEmail);
                return Ok("Notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification email to {UserEmail}", userEmail);
                return BadRequest($"Failed to send notification: {ex.Message}");
            }
        }

        public async Task<IActionResult> SendTemplateNotification(string userEmail)
        {
            _logger.LogInformation("Sending template notification email to {UserEmail}", userEmail);

            try
            {
                var templateData = new Dictionary<string, object>
            {
                { "user_name", userEmail.Split('@')[0] },
                { "action_required", "Please review recent changes" },
                { "deadline", DateTime.Now.AddDays(3).ToString("d") }
            };

                await _emailService.SendTemplateEmailAsync(
                    userEmail,
                    1, // Your template ID from Brevo
                    templateData
                );

                _logger.LogInformation("Template notification email sent successfully to {UserEmail}", userEmail);
                return Ok("Template notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send template notification email to {UserEmail}", userEmail);
                return BadRequest($"Failed to send template notification: {ex.Message}");
            }
        }
    }
}
