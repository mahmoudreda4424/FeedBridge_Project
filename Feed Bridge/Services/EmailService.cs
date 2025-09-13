using Feed_Bridge.IServices;
using System.Net;
using System.Net.Mail;

namespace Feed_Bridge.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"]);
            var smtpEmail = _config["Smtp:Email"];
            var smtpPassword = _config["Smtp:Password"];
            var fromEmail = _config["Smtp:From"] ?? smtpEmail; // لو مش متحددة، هيستخدم نفس الـ Email

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpEmail, smtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "FeedBridge Support"), // الاسم اللي يبان
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
