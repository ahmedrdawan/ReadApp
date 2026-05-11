using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyReadsApp.Core.AppSetting;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth;
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
        private readonly IEmailService _emailservices;
        private readonly BaseAppSetting _baseAppSetting;

        public AuthServices(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenServices jwtTokenServices,
            IEmailService emailservices,
            IOptions<BaseAppSetting> baseAppSetting)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenServices = jwtTokenServices;
            _emailservices = emailservices;
            _baseAppSetting = baseAppSetting.Value ?? throw new ArgumentNullException(nameof(baseAppSetting));
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">User registration data (username, email, password).</param>
        /// <returns>
        /// Success message if user is created and email confirmation is sent,
        /// otherwise failure response with error details.
        /// </returns>
        public async Task<Response> RegisterAsync(RegisterDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
                return Response.Failure(
                    "User with this email already exists.",
                    409);

            existingUser = await _userManager.FindByNameAsync(request.UserName);

            if (existingUser != null)
                return Response.Failure(
                    "User with this username already exists.",
                    409);

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                Role = "User"
            };

            var createResult = await _userManager.CreateAsync(
                user,
                request.Password
            );

            if (!createResult.Succeeded)
                return Response.Failure(
                    createResult.Errors
                        .Select(e => e.Description)
                        .FirstOrDefault() ?? "User creation failed.",
                    500
                );

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                user.Role
            );

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return Response.Failure(
                    "Failed to assign default role.",
                    500
                );
            }


            await SendEmailConfirmTokenAsync(user);

            return Response.Success(
                message: "Please confirm your email."
            );
        }

        /// <summary>
        /// Generates an email confirmation token and sends it to the user's email.
        /// </summary>
        /// <param name="user">The user who needs email confirmation.</param>
        /// <returns>
        /// Success if email was sent successfully,
        /// otherwise failure response.
        /// </returns>
        private async Task<Response<bool>> SendEmailConfirmTokenAsync(User user)
        {
            if (user.EmailConfirmed)
                return Response<bool>.Failure(
                    "Email is already confirmed.",
                    400
                );

            var emailConfirmationToken =
                await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(emailConfirmationToken)
            );

                var htmlBody = $@"
                        <h2>Email Confirmation</h2>

                        <p>
                            Please confirm your email.
                        </p>

                        <p>
                            {encodedToken}
                        </p>

                            <p>
                                If you did not create this account,
                                you can safely ignore this email.
                            </p>
                        ";

            await _emailservices.SendEmailAsync(
                user.Email!,
                "Confirm Your Email",
                htmlBody
            );

            return Response<bool>.Success(true);
        }


        /// <summary>
        /// Confirms a user's email using the provided token.
        /// </summary>
        /// <param name="email">User email address.</param>
        /// <param name="token">Encoded email confirmation token.</param>
        /// <returns>
        /// Success if email is confirmed,
        /// otherwise failure response.
        /// </returns>
        public async Task<Response> ConfirmEmailAsync(string email,string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Response.Failure(
                    "Invalid email confirmation request.",
                    400
                );
            

            if (user.EmailConfirmed)
                return Response.Failure(
                    "Email is already confirmed.",
                    400
                );
            

            try
            {
                var decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(token)
                );

                var result = await _userManager.ConfirmEmailAsync(
                    user,
                    decodedToken
                );

                if (!result.Succeeded)
                    return Response.Failure(
                        result.Errors
                            .Select(e => e.Description)
                            .FirstOrDefault() ?? "Email confirmation failed.",
                        400
                    );
                

                return Response.Success();
            }
            catch (FormatException)
            {
                return Response.Failure(
                    "Invalid token format.",
                    400
                );
            }
        }


        /// <summary>
        /// Authenticates a user and generates JWT + refresh token.
        /// </summary>
        /// <param name="request">Login credentials (email and password).</param>
        /// <returns>
        /// Authentication response containing user data and JWT token,
        /// or failure if credentials are invalid.
        /// </returns>
        public async Task<Response<AuthResponse>> LoginAsync(LoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Response<AuthResponse>.Failure("Invalid email or password.", 401);

            var isValid = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (isValid.IsNotAllowed)
                return Response<AuthResponse>.Failure("Please confirm your email before logging in.", 403);

            if (!isValid.Succeeded)
                return Response<AuthResponse>.Failure("Invalid email or password.", 401);

            var tokenResult = await _jwtTokenServices.GenerateJwtTokenAsync(user);

            RefreshToken refreshToken = await _jwtTokenServices.GenerateRefreshTokenAsync();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            await _jwtTokenServices.SetRefreshTokenInCookies(
                refreshToken.Token,
                refreshToken.ExpireAt
            );

            var response = BuildAuthResponse(user, tokenResult);

            return Response<AuthResponse>.Success(response);
        }


        /// <summary>
        /// Generates a new JWT token using a valid refresh token from cookies.
        /// Implements refresh token rotation for security.
        /// </summary>
        /// <returns>
        /// New authentication response with updated JWT and refresh token,
        /// or failure if refresh token is invalid or expired.
        /// </returns>
        public async Task<Response<AuthResponse>> RefreshTokenAsync()
        {
            string? hashedToken =  _jwtTokenServices.GetRefreshTokenFromCookies();

            if (hashedToken == null)
                return Response<AuthResponse>.Failure("Refresh token not found.", 401);

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u =>
                    u.RefreshTokens.Any(t => t.Token == hashedToken));

            if (user == null)
                return Response<AuthResponse>.Failure("Invalid refresh token.", 401);

            var oldToken = user.RefreshTokens.First(t => t.Token == hashedToken);

            if (!oldToken.IsActive)
                return Response<AuthResponse>.Failure("Expired or revoked token.", 401);

            oldToken.RevokedAt = DateTime.UtcNow;

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

        #region ExternalLogin
        /// <summary>
        /// Logs in or registers a user using Google authentication.
        /// </summary>
        /// <param name="email">Google user email.</param>
        /// <param name="name">Google display name (optional).</param>
        /// <returns>
        /// Authentication response with JWT and refresh token.
        /// </returns>
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
                        createResult.Errors.Select(e => e.Description).FirstOrDefault() ?? "User creation failed.", 500);

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



        #region BuildResponse

        /// <summary>
        /// Builds a unified authentication response containing user data and JWT token.
        /// </summary>
        /// <param name="user">Authenticated user entity.</param>
        /// <param name="tokenResult">Generated JWT token result.</param>
        /// <returns>Structured authentication response.</returns>
        private static AuthResponse BuildAuthResponse(User user, TokenDto tokenResult)
        {
            return new AuthResponse(
                new UserDTO(
                    user.Id,
                    user.Email,
                    user.UserName,
                    user.Role),
                tokenResult
                );
        }
        #endregion


        /// <summary>
        /// Generates a password reset token and sends it to the user's email.
        /// </summary>
        /// <param name="email">User email address.</param>
        /// <returns>
        /// Always returns success to avoid email enumeration attacks.
        /// </returns>
        public async Task<Response> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Response.Success(); 

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));


            await _emailservices.SendEmailAsync(
                email,
                "Reset your Password",
                $@"
                    <h3>Reset your Password</h3>
                    <p>{encodedToken}</p>
                    <p>This link will expire soon.</p>
                    "
            );

            return Response.Success();
        }

        /// <summary>
        /// Resets the user's password using a valid reset token.
        /// </summary>
        /// <param name="request">Reset password data (email, token, new password).</param>
        /// <returns>
        /// Success if password is updated,
        /// otherwise failure if token is invalid or expired.
        /// </returns>
        public async Task<Response> ResetPasswordAsync(ResetPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Response.Failure("Invalid request", 400);

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(request.Token)
            );

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!result.Succeeded)
                return Response.Failure("Invalid or expired token", 400);

            return Response.Success();
        }
    }
}
