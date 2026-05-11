
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth;
using MyReadsApp.Core.DTOs.Auth.Response;


namespace MyReadsApp.Core.Services.Interfaces.Account
{
    public interface IAuthServices
    {
        Task<Response> RegisterAsync(RegisterDto request);
        Task<Response<AuthResponse>> LoginAsync(LoginDto request);
        Task<Response> ConfirmEmailAsync(string email, string token);
        Task<Response<AuthResponse>> RefreshTokenAsync();
        Task<Response<AuthResponse>> GoogleLoginAsync(string email, string? name);
        Task<Response> ForgotPasswordAsync(string email);
        Task<Response> ResetPasswordAsync(ResetPasswordDto request);
    }
}
