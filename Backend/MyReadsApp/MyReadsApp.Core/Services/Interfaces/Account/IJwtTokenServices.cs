using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth.Response;
using MyReadsApp.Core.Entities.Identity;

namespace MyReadsApp.Core.Services.Interfaces.Account
{
    public interface IJwtTokenServices
    {
        Task<TokenResult> GenerateJwtTokenAsync(User user);
        Task<Entities.Identity.RefreshToken> GenerateRefreshTokenAsync();
        Task SetRefreshTokenInCookies(string Token, DateTime ExpireAt);
        Task<string?> GetRefreshTokenFromCookies();
        Task<Response<RefreshTokenResponse>> RefreshTokenAsync();

    }
}
