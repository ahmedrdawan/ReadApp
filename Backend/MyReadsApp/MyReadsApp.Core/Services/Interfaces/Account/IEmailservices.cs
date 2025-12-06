using MyReadsApp.API.DTOs;
using MyReadsApp.Core.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces.Account
{
    public interface IEmailservices
    {

        Task<Response> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
