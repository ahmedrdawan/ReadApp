using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyReadsApp.Core.AppSetting;
using MyReadsApp.Core.Common;
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
    /// <summary>
    /// Provides JWT and refresh token generation, validation, and cookie management.
    /// Responsible for authentication token lifecycle in the application.
    /// </summary>
    public class JwtTokenServices : IJwtTokenServices
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public JwtTokenServices(IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }


        /// <summary>
        /// Generates a signed JWT token for an authenticated user.
        /// </summary>
        /// <param name="user">Authenticated user entity.</param>
        /// <returns>
        /// JWT token string and expiration date.
        /// </returns>
        public async Task<TokenDto> GenerateJwtTokenAsync(User user)
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

            return new TokenDto(jwtString, expiresAt);
        }


        /// <summary>
        /// Generates a secure refresh token using cryptographically strong random bytes.
        /// The token is hashed before being stored in the database.
        /// </summary>
        /// <returns>
        /// A hashed refresh token with expiration and creation time.
        /// </returns>
        public async Task<RefreshToken> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];

            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            var token = Convert.ToBase64String(randomNumber);

            return new RefreshToken
            {
                Token = HashToken(token), 
                ExpireAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiration),
                CreatedAt = DateTime.UtcNow
            };
        }


        /// <summary>
        /// Stores the refresh token in an HTTP-only secure cookie.
        /// </summary>
        /// <param name="token">Raw refresh token value.</param>
        /// <param name="expireAt">Expiration date of the cookie.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when HttpContext is not available.
        /// </exception>
        public async Task SetRefreshTokenInCookies(string token, DateTime expireAt)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
                throw new InvalidOperationException("HttpContext is not available.");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Expires = expireAt
            };

            context.Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        /// <summary>
        /// Retrieves the refresh token from HTTP cookies.
        /// </summary>
        /// <returns>
        /// The refresh token string if exists; otherwise null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when HttpContext is not available.
        /// </exception>
        public string? GetRefreshTokenFromCookies()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
                throw new InvalidOperationException("HttpContext is not available.");

            var token = context.Request.Cookies["refreshToken"];

            return token != null ? token : null;
        }


        #region Private Helpers

        /// <summary>
        /// Hashes a token using SHA256 for secure storage in the database.
        /// </summary>
        /// <param name="token">Plain token value.</param>
        /// <returns>Hashed token string.</returns>
        private string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
        #endregion
    }
}
