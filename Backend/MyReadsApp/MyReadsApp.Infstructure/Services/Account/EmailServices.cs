using Microsoft.Extensions.Options;
using MyReadsApp.API.DTOs;
using MyReadsApp.Core.AppSetting;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Services.Interfaces.Account;
using System.Net;
using System.Net.Mail;
using Response = MyReadsApp.Core.Common.Response;

namespace MyReadsApp.Infstructure.Services
{
    public class EmailServices : IEmailservices
    {
        private readonly StmpSetting _stmpSetting;

        public EmailServices(IOptions<StmpSetting> stmpSetting)
        {
            _stmpSetting = stmpSetting.Value ?? throw new ArgumentNullException(nameof(stmpSetting));
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string content)
        {
            if (string.IsNullOrEmpty(_stmpSetting.SenderEmail))
                throw new Exception("SenderEmail is NULL");

            if (string.IsNullOrEmpty(toEmail))
                throw new Exception("Receiver email is NULL");
            try
            {
                using (var smtp = new SmtpClient(_stmpSetting.SmtpHost, _stmpSetting.SmtpPort))
                {
                    smtp.Credentials = new NetworkCredential(_stmpSetting.SenderEmail, _stmpSetting.SmtpPassword);
                    smtp.EnableSsl = _stmpSetting.EnableSsl;

                    var mail = new MailMessage
                    {
                        From = new MailAddress(_stmpSetting.SenderEmail, "MyReadsApp"),
                        Subject = subject,
                        Body = content,
                        IsBodyHtml = true
                    };

                    mail.To.Add(toEmail);

                    await smtp.SendMailAsync(mail);

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}