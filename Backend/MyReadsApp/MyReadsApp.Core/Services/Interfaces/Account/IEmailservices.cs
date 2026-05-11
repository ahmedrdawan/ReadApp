namespace MyReadsApp.Core.Services.Interfaces.Account
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string content);
    }
}
