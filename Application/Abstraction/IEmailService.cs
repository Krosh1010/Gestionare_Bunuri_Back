namespace Application.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendNotificationEmailAsync(string toEmail, string userName, List<NotificationEmailItem> notifications);
        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken);
        Task SendEmailVerificationAsync(string toEmail, string userName, string verificationToken);
    }

    public class NotificationEmailItem
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsExpired { get; set; }
    }
}
