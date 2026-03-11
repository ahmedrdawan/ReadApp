using MyReadsApp.API.DTOs;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Services.Interfaces.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.OLE.Interop;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using Response = MyReadsApp.Core.Common.Response;
using MyReadsApp.Core.AppSetting;

namespace MyReadsApp.Infstructure.Services
{
    public class EmailServices : IEmailservices
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly GridSetting _gridSetting;

        public EmailServices(IConfiguration config, UserManager<User> userManager, GridSetting gridSetting)
        {
            _config = config;
            _userManager = userManager;
            _gridSetting = gridSetting;
        }

        
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _gridSetting.SenderGridApiKey;
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(_gridSetting.SenderEmail, "MyReadsApp");
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", content);

            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
    }

}
