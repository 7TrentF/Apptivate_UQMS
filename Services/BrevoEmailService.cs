using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Apptivate_UQMS_WebApp.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
        Task SendTemplateEmailAsync(string to, long templateId, Dictionary<string, object> templateData);
    }

    public class BrevoEmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly TransactionalEmailsApi _emailApi;
        private readonly ILogger<BrevoEmailService> _logger;

        public BrevoEmailService(IConfiguration configuration, ILogger<BrevoEmailService> logger)
        {
            _apiKey = configuration["Brevo:ApiKey"];
            Configuration.Default.ApiKey["api-key"] = _apiKey;
            _emailApi = new TransactionalEmailsApi();
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {
                _logger.LogInformation("Attempting to send email to {Recipient} with subject '{Subject}'", to, subject);

                var sendSmtpEmail = new SendSmtpEmail
                {
                    Subject = subject,
                    HtmlContent = htmlContent,
                    Sender = new SendSmtpEmailSender
                    {
                        Name = "Apptivate",
                        Email = "trentfisher017@gmail.com"
                    },
                    To = new List<SendSmtpEmailTo>
                {
                    new SendSmtpEmailTo { Email = to, Name = to.Split('@')[0] }
                }
                };

                var result = await _emailApi.SendTransacEmailAsync(sendSmtpEmail);

                _logger.LogInformation("Email sent successfully to {Recipient} with subject '{Subject}'", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient} with subject '{Subject}'", to, subject);
                throw new ApplicationException("Failed to send email", ex);
            }
        }

        public async Task SendTemplateEmailAsync(string to, long templateId, Dictionary<string, object> templateData)
        {
            try
            {
                _logger.LogInformation("Attempting to send template email to {Recipient} with template ID {TemplateId}", to, templateId);

                // Create sender info
                var sender = new SendSmtpEmailSender
                {
                    Name = "Apptivate",
                    Email = "trentfisher017@gmail.com"
                };

                // Create recipient info - proper way to construct
                var recipients = new List<SendSmtpEmailTo>
        {
            new SendSmtpEmailTo(to, to.Split('@')[0])  // Using constructor instead of object initializer
        };

                var sendSmtpEmail = new SendSmtpEmail
                {
                    TemplateId = templateId,
                    Sender = sender,
                    To = recipients,
                    Params = templateData
                };

                var result = await _emailApi.SendTransacEmailAsync(sendSmtpEmail);
                _logger.LogInformation("Template email sent successfully to {Recipient} with template ID {TemplateId}", to, templateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send template email to {Recipient} with template ID {TemplateId}", to, templateId);
                throw new ApplicationException("Failed to send template email", ex);
            }
        }
    }

}

