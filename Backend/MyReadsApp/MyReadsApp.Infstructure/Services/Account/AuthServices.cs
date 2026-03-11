using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyReadsApp.Core.AppSetting;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth.Request;
using MyReadsApp.Core.DTOs.Auth.Response;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Services.Interfaces.Account;
using System.Text;

namespace MyReadsApp.Infstructure.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenServices _jwtTokenServices;
        private readonly IEmailservices _emailservices;
        private readonly IConfiguration _configration;
        private readonly BaseAppSetting _baseAppSetting;

        public AuthServices(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenServices jwtTokenServices,
            IEmailservices emailservices,
            IConfiguration configration,
            IOptions<BaseAppSetting> baseAppSetting)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenServices = jwtTokenServices;
            _emailservices = emailservices;
            _configration = configration;
            _baseAppSetting = baseAppSetting.Value ?? throw new ArgumentNullException(nameof(baseAppSetting));
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

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(emailConfirmationToken)
            );
            var confirmationLink = $"{_baseAppSetting.appURL}/confirm-email?userId={user.Id}&token={encodedToken}";
            var isSend = await _emailservices.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $@" <h3>Email Confirmation</h3>
                    <p>Please confirm your email by clicking the link below:</p>
                    <a href='{confirmationLink}'>Confirm Email</a>
                    "
            );

            if (!isSend)
                return Response<AuthResponse>.Failure("Failed to send confirmation email.", 500);


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


            var isValid = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (isValid.IsNotAllowed)
                return Response<AuthResponse>.Failure("Please confirm your email before logging in.", 403);

            if (!isValid.Succeeded)
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

        
        public async Task<Response> ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return Response.Failure("User not found", 404);

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return Response.Failure("Email confirmation failed", 400);

            return Response.Success();
        }


        #region ExternalLogin
        #region ExternalLogin
        public async Task<Response<AuthResponse>> GoogleLoginAsync(string email, string name = null)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                
                user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = name != null ? name.Replace(" ", "") : email.Split('@')[0],
                    Email = email,
                    EmailConfirmed = true, 
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return Response<AuthResponse>.Failure(
                        createResult.Errors.Select(e => e.Description).ToList(), 500);

                var roleResult = await _userManager.AddToRoleAsync(user, user.Role);
                if (!roleResult.Succeeded)
                    return Response<AuthResponse>.Failure("Failed to assign default role.", 500);
            }

            var jwtToken = await _jwtTokenServices.GenerateJwtTokenAsync(user);

            var refreshToken = await _jwtTokenServices.GenerateRefreshTokenAsync();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            await _jwtTokenServices.SetRefreshTokenInCookies(refreshToken.Token, refreshToken.ExpireAt);

            var response = BuildAuthResponse(user, jwtToken);

            return Response<AuthResponse>.Success(response);
        }
        #endregion
        #endregion


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
