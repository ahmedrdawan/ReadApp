using MyReadsApp.Core.DTOs.Auth.Response;
using MyReadsApp.Core.Entities.Identity;

namespace MyReadsApp.Core.Services.Interfaces.Account
{
    public interface IJwtTokenServices
    {
        Task<TokenResult> GenerateJwtTokenAsync(User user);
    }
}
