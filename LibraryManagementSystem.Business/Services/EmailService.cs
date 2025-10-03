using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LibraryManagementSystem.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptionsMonitor<EmailSettings> emailSettingsOptions)
        {
            _emailSettings = emailSettingsOptions.CurrentValue;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Reset your password";

            var builder = new BodyBuilder();
            builder.HtmlBody = 
            $@"
            <p>Hello,</p>
            <p>You recently requested to reset your password for your account. Click the link below to reset it:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>If you did not request a password reset, please ignore this email.</p>
            <p>Thanks,<br>The Library Management Team</p>"; 

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_emailSettings.SmtpServer
                    ,_emailSettings.SmtpPort,SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_emailSettings.Username,_emailSettings.Password);

                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }

        }
    }
}
