using Application.Abstraction;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _templatesPath;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            var filePath = Path.Combine(_templatesPath, templateName);
            return await File.ReadAllTextAsync(filePath);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["Email:SenderName"] ?? "AssetGuard",
                    _configuration["Email:SenderEmail"] ?? "noreply@assetguard.com"));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();

                var host = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var port = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var username = _configuration["Email:SmtpUsername"] ?? "";
                var password = _configuration["Email:SmtpPassword"] ?? "";

                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(username, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email trimis cu succes către {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la trimiterea email-ului către {Email}", toEmail);
                throw;
            }
        }

        public async Task SendNotificationEmailAsync(string toEmail, string userName, List<NotificationEmailItem> notifications)
        {
            var expiredItems = notifications.Where(n => n.IsExpired).ToList();
            var expiringItems = notifications.Where(n => !n.IsExpired).ToList();

            var template = await LoadTemplateAsync("notification-email.html");
            var itemTemplate = await LoadTemplateAsync("notification-item.html");

            var expiredHtml = "";
            if (expiredItems.Any())
            {
                expiredHtml = "<div class='section-title expired'>❌ Expirate</div>";
                foreach (var item in expiredItems)
                {
                    expiredHtml += itemTemplate
                        .Replace("{{CSS_CLASS}}", "expired")
                        .Replace("{{TITLE}}", item.Title)
                        .Replace("{{MESSAGE}}", item.Message);
                }
            }

            var expiringHtml = "";
            if (expiringItems.Any())
            {
                expiringHtml = "<div class='section-title expiring'>⚠️ Expiră curând</div>";
                foreach (var item in expiringItems)
                {
                    expiringHtml += itemTemplate
                        .Replace("{{CSS_CLASS}}", "expiring")
                        .Replace("{{TITLE}}", item.Title)
                        .Replace("{{MESSAGE}}", item.Message);
                }
            }

            var htmlBody = template
                .Replace("{{USER_NAME}}", userName)
                .Replace("{{EXPIRED_SECTION}}", expiredHtml)
                .Replace("{{EXPIRING_SECTION}}", expiringHtml);

            var subject = $"AssetGuard - {notifications.Count} notificări noi";
            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken)
        {
            var template = await LoadTemplateAsync("password-reset.html");

            var htmlBody = template
                .Replace("{{USER_NAME}}", userName)
                .Replace("{{RESET_TOKEN}}", resetToken);

            await SendEmailAsync(toEmail, "AssetGuard - Resetare Parolă", htmlBody);
        }

        public async Task SendEmailVerificationAsync(string toEmail, string userName, string verificationToken)
        {
            var template = await LoadTemplateAsync("email-verification.html");

            var htmlBody = template
                .Replace("{{USER_NAME}}", userName)
                .Replace("{{VERIFICATION_TOKEN}}", verificationToken);

            await SendEmailAsync(toEmail, "AssetGuard - Verificare Email", htmlBody);
        }
    }
}
