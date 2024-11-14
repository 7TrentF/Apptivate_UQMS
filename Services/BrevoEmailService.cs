using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using System.Text;
using System;
using Newtonsoft.Json;


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


        public static void Main(string[] args)
        {
            // Generate OTP
            string otpCode = GenerateOTP();
            Console.WriteLine("Generated OTP: " + otpCode);

            // Send OTP to user
            string userEmail = "recipient@example.com";  // Replace with actual recipient email
            SendOTPEmail(otpCode, userEmail);

            // Get OTP input from user (in a real scenario, you'd get this from a user input form)
            Console.WriteLine("Enter the OTP sent to your email:");
            string userInputOtp = Console.ReadLine();

            // Verify OTP
            if (VerifyOTP(userInputOtp, otpCode))
            {
                Console.WriteLine("OTP verified successfully!");
            }
            else
            {
                Console.WriteLine("Invalid OTP. Please try again.");
            }
        }

        public static string GenerateOTP(int length = 6)
        {
            Random random = new Random();
            StringBuilder otpBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                otpBuilder.Append(random.Next(0, 10).ToString());
            }
            return otpBuilder.ToString();
        }

        public static void SendOTPEmail(string otpCode, string recipientEmail)
        {
            // Set up Brevo API key
            Configuration.Default.ApiKey["api-key"] = "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";  // Replace with your actual API key

            var apiInstance = new TransactionalEmailsApi();

            // Create email content
            var sendSmtpEmail = new SendSmtpEmail(
                to: new List<SendSmtpEmailTo> { new SendSmtpEmailTo(email: recipientEmail) },
                sender: new SendSmtpEmailSender(email: "sender@example.com", name: "Your Name or Company"),  // Replace sender email and name
                subject: "Your OTP Code",
                htmlContent: $"<p>Your OTP code is: <strong>{otpCode}</strong></p>"
            );

            try
            {
                // Send the email
                var result = apiInstance.SendTransacEmail(sendSmtpEmail);
                Console.WriteLine("OTP Email sent successfully: " + JsonConvert.SerializeObject(result));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception when sending OTP email: " + e.Message);
            }
        }

        public static bool VerifyOTP(string inputOtp, string generatedOtp)
        {
            return inputOtp == generatedOtp;
        }

    }


}

