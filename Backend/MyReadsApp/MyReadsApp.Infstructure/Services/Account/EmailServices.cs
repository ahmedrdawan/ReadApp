using Microsoft.Extensions.Options;
using MyReadsApp.Core.AppSetting;
using MyReadsApp.Core.Services.Interfaces.Account;
using System.Net;
using System.Net.Mail;

namespace MyReadsApp.Infrastructure.Services
{

    /// <summary>
    /// Provides functionality to send emails using SMTP configuration.
    /// Used for account-related emails such as confirmation and password reset.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value
                ?? throw new ArgumentNullException(nameof(smtpSettings));
        }


        /// <summary>
        /// Sends an email to a specified recipient using SMTP settings from configuration.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="subject">Email subject line.</param>
        /// <param name="content">Email body content (supports HTML).</param>
        /// <returns>A task representing the asynchronous send operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when sender email is not configured.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when recipient email is null or empty.
        /// </exception>
        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string content)
        {
            if (string.IsNullOrWhiteSpace(_smtpSettings.SenderEmail))
                throw new InvalidOperationException("Sender email is not configured.");

            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentNullException(nameof(toEmail));

            using var smtp = new SmtpClient(
                _smtpSettings.SmtpHost,
                _smtpSettings.SmtpPort);

            smtp.Credentials = new NetworkCredential(
                _smtpSettings.SenderEmail,
                _smtpSettings.SmtpPassword);

            smtp.EnableSsl = _smtpSettings.EnableSsl;

            using var mail = new MailMessage
            {
                From = new MailAddress(
                    _smtpSettings.SenderEmail,
                    "MyReadsApp"),

                Subject = subject,
                Body = content,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await smtp.SendMailAsync(mail);
        }
    }
}