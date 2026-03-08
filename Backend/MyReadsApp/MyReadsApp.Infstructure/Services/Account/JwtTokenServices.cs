using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyReadsApp.Core.AppSetting;
using MyReadsApp.Core.DTOs.Auth.Response;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Services.Interfaces.Account;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyReadsApp.Infstructure.Services
{
    public class JwtTokenServices : IJwtTokenServices
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenServices(IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor)
        {
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _httpContextAccessor = httpContextAccessor;
        }


        public Task<TokenResult> GenerateJwtTokenAsync(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            string jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(new TokenResult(jwtString, expiresAt));
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpireAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiration),
                CreatedAt = DateTime.UtcNow
            };
        }

        public Task SetRefreshTokenInCookies(string Token, DateTime ExpireAt)
        {
            if (string.IsNullOrEmpty(Token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(Token));

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                Expires = ExpireAt
            };

            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                throw new InvalidOperationException("HttpContext is not available.");

            context.Response.Cookies.Append("refreshToken", Token, cookieOptions);
            return Task.CompletedTask;
        }

        public Task<string?> GetRefreshTokenFromCookies()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
                throw new InvalidOperationException("HttpContext is not available.");

            return Task.FromResult<string?>(context.Request.Cookies["refreshToken"]);
        }
    }
}
