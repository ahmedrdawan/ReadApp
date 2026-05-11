using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth.Response;
using MyReadsApp.Core.Entities.Identity;

namespace MyReadsApp.Core.Services.Interfaces.Account
{
    public interface IJwtTokenServices
    {
        Task<TokenDto> GenerateJwtTokenAsync(User user);
        Task<RefreshToken> GenerateRefreshTokenAsync();
        Task SetRefreshTokenInCookies(string Token, DateTime ExpireAt);
        string? GetRefreshTokenFromCookies();
    }
}
