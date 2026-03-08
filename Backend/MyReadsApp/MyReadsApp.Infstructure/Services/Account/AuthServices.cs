using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth.Request;
using MyReadsApp.Core.DTOs.Auth.Response;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.Infstructure.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenServices _jwtTokenServices;
        private readonly IEmailservices _emailservices;
        private readonly IConfiguration _configration;

        public AuthServices(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenServices jwtTokenServices,
            IEmailservices emailservices,
            IConfiguration configration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenServices = jwtTokenServices;
            _emailservices = emailservices;
            _configration = configration;
        }

        public async Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var existUser = await _userManager.FindByEmailAsync(request.Email);
            if (existUser != null)
                return Response<AuthResponse>.Failure("User with this email already exists.", 409);

            existUser = await _userManager.FindByNameAsync(request.UserName);
            if (existUser != null)
                return Response<AuthResponse>.Failure("User with this username already exists.", 409);

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                Role = "User"
            };

            var resultUser = await _userManager.CreateAsync(user, request.Password);

            if (!resultUser.Succeeded)
                return Response<AuthResponse>.Failure(
                    resultUser.Errors.Select(e => e.Description).ToList(), 500);

            var resultRole = await _userManager.AddToRoleAsync(user, user.Role);

            if (!resultRole.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return Response<AuthResponse>.Failure("Failed to assign default role.", 500);
            }

            TokenResult token = await _jwtTokenServices.GenerateJwtTokenAsync(user);

            var refreshToken = await _jwtTokenServices.GenerateRefreshTokenAsync();

            user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateAsync(user);

            await _jwtTokenServices.SetRefreshTokenInCookies(
                refreshToken.Token,
                refreshToken.ExpireAt
            );

            var response = BuildAuthResponse(user, token);

            return Response<AuthResponse>.Success(response, 201);
        }

        public async Task<Response<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Response<AuthResponse>.Failure("Invalid email or password.", 401);

            bool isValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isValid)
                return Response<AuthResponse>.Failure("Invalid email or password.", 401);

            TokenResult token = await _jwtTokenServices.GenerateJwtTokenAsync(user);

            var refreshToken = await _jwtTokenServices.GenerateRefreshTokenAsync();

            user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateAsync(user);

            await _jwtTokenServices.SetRefreshTokenInCookies(
                refreshToken.Token,
                refreshToken.ExpireAt
            );

            var response = BuildAuthResponse(user, token);

            return Response<AuthResponse>.Success(response);
        }

        public async Task<Response<AuthResponse>> RefreshTokenAsync()
        {
            string? refreshTokenValue = await _jwtTokenServices.GetRefreshTokenFromCookies();

            if (refreshTokenValue == null)
                return Response<AuthResponse>.Failure("Refresh token not found.", 401);

            var storedToken = await _userManager.Users
                .SelectMany(u => u.RefreshTokens)
                .FirstOrDefaultAsync(t => t.Token == refreshTokenValue);

            if (storedToken == null || !storedToken.IsActive)
                return Response<AuthResponse>.Failure("Invalid refresh token.", 401);

            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());

            if (user == null)
                return Response<AuthResponse>.Failure("User not found.", 404);

            storedToken.CreatedAt = DateTime.UtcNow;

            var newRefreshToken = await _jwtTokenServices.GenerateRefreshTokenAsync();

            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateAsync(user);

            await _jwtTokenServices.SetRefreshTokenInCookies(
                newRefreshToken.Token,
                newRefreshToken.ExpireAt
            );

            var newJwt = await _jwtTokenServices.GenerateJwtTokenAsync(user);

            var response = BuildAuthResponse(user, newJwt);

            return Response<AuthResponse>.Success(response);
        }

        #region BuildResponse

        private static AuthResponse BuildAuthResponse(User user, TokenResult tokenResult)
        {
            return new AuthResponse(
                new UserDTO(
                    user.Id,
                    user.Email,
                    user.UserName,
                    user.Role),
                new TokenResult(
                    tokenResult.Token,
                    tokenResult.ExpiresAt));
        }

        #endregion
    }
}
