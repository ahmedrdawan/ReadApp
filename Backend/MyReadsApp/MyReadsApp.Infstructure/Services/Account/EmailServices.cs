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

namespace MyReadsApp.Infstructure.Services
{
    public class EmailServices : IEmailservices
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public EmailServices(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }



        public async Task<Response> ConfirmEmailAsync(ConfirmEmailRequest request)
        {

            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                    return Response.Failure("Invalid User", 401);

                var token = WebEncoders.Base64UrlDecode(request.code);
                string normalToken = Encoding.UTF8.GetString(token);

                var result = await _userManager.ConfirmEmailAsync(user, normalToken);
                if (!result.Succeeded)
                    return Response.Failure("Invalid User", 401);

                return Response.Success();
            }
            catch
            {
                return Response.Failure("Server Error", 500);
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            try
            {
                var apiKey = _config["SenderGridApiKey"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(_config["SenderEmail"],content );
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

            }
            catch
            {
                throw;
            }
        }
    }

}
