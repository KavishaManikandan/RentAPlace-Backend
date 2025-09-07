using MailKit.Net.Smtp;
using MimeKit;

namespace RentAPlace.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                emailSettings["SenderName"], 
                emailSettings["SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), false);
            await smtp.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
