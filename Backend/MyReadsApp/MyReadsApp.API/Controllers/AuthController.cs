using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.Auth.Request;
using MyReadsApp.API.Extentions;
using MyReadsApp.Core.DTOs.Auth;
using MyReadsApp.Core.Services.Interfaces.Account;
using System.Security.Claims;

namespace MyReadsApp.API.Controllers
{

    /// <summary>
    /// Handles authentication-related endpoints including registration, login,
    /// email confirmation, password reset, refresh token, and external login (Google).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }


        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="request">User registration data (username, email, password).</param>
        /// <returns>
        /// HTTP response indicating success or failure of registration.
        /// </returns>
        [HttpPost("Sign-Up")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authServices.RegisterAsync(new RegisterDto
            (
                request.UserName,
                request.Email,
                request.Password
            ));

            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Authenticates a user and returns JWT + refresh token.
        /// </summary>
        /// <param name="request">Login credentials (email and password).</param>
        /// <returns>
        /// HTTP response containing authentication result or error.
        /// </returns>
        [HttpPost("Sign-In")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authServices.LoginAsync(new LoginDto
            (
                request.Email,
                request.Password
            ));

            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Confirms a user's email address using a verification token.
        /// </summary>
        /// <param name="email">User email address.</param>
        /// <param name="token">Email confirmation token.</param>
        /// <returns>
        /// HTTP response indicating success or failure of email confirmation.
        /// </returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery]string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Invalid email confirmation request.");

            var result = await _authServices.ConfirmEmailAsync(email, token);
            
            return StatusCode(result.StatusCode, result);
        }



        /// <summary>
        /// Sends a password reset token to the user's email.
        /// </summary>
        /// <param name="email">User email address.</param>
        /// <returns>
        /// HTTP response indicating whether reset email was sent.
        /// </returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            var result = await _authServices.ForgotPasswordAsync(email);
            if (!result.IsSuccess)
                return BadRequest(result);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Resets user password using a valid reset token.
        /// </summary>
        /// <param name="request">Reset password data (email, token, new password).</param>
        /// <returns>
        /// HTTP response indicating success or failure of password reset.
        /// </returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] API.DTOs.
            Auth.Request.ResetPasswordDto request)
        {
            var result = await _authServices.ResetPasswordAsync(new Core.DTOs.Auth.ResetPasswordDto
            (
                request.Email,
                request.Token,
                request.NewPassword
            ));
            if (!result.IsSuccess)
                return BadRequest(result);
            return StatusCode(result.StatusCode, result);
        }



        /// <summary>
        /// Generates a new JWT using a valid refresh token stored in cookies.
        /// </summary>
        /// <returns>
        /// HTTP response containing new authentication tokens.
        /// </returns>
        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _authServices.RefreshTokenAsync();
            return StatusCode(result.StatusCode, result);
        }

        #region External Login

        /// <summary>
        /// Initiates Google authentication flow.
        /// </summary>
        /// <returns>
        /// Redirects user to Google login page.
        /// </returns>
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        /// <summary>
        /// Handles Google authentication callback and logs user into the system.
        /// </summary>
        /// <returns>
        /// HTTP response containing authentication result or failure message.
        /// </returns>
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest("Google login failed.");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest("Google login did not provide an email.");

            var response = await _authServices.GoogleLoginAsync(email, name);
            return this.ActionResult(response.StatusCode, response);
        }
        #endregion
    }
}
