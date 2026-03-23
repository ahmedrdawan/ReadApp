using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.Account;
using MyReadsApp.API.Extentions;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth.Request;
using MyReadsApp.Core.DTOs.Auth.Response;
using MyReadsApp.Core.Services.Interfaces.Account;
using System.Security.Claims;

namespace MyReadsApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthServices _authServices;
        private readonly IConfiguration _Configration;
        private readonly IJwtTokenServices _jwtTokenServices;

        public AuthController(IAuthServices authServices, IConfiguration configration, IJwtTokenServices jwtTokenServices)
        {
            _authServices = authServices;
            _Configration = configration;
            _jwtTokenServices = jwtTokenServices;
        }

        [HttpPost("Sign-Up")]
        public async Task<IActionResult> Register([FromBody] SignUpDtos request)
        {
            Response<AuthResponse> result = await _authServices.RegisterAsync(new RegisterRequest
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
            });

            return this.ActionResult(result.StatusCode, result);
        }

        [HttpPost("Sign-In")]
        public async Task<IActionResult> Login([FromBody] SignInDtos request)
        {
            var result = await _authServices.LoginAsync(new LoginRequest
            {
                Email = request.Email,
                Password = request.Password,
            });

            return this.ActionResult(result.StatusCode, result);
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery]string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Invalid email confirmation request.");

            var result = await _authServices.ConfirmEmailAsync(email, token);
            
            if (!result.IsSuccess)
                return BadRequest(result);
            return this.ActionResult(result.StatusCode, result);
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDtos request)
        {
            var result = await _authServices.ForgotPasswordAsync(request.Email);
            if (!result.IsSuccess)
                return BadRequest(result);
            return this.ActionResult(result.StatusCode, result);
        }

        [HttpGet("Verify-reset-token")]
        public async Task<IActionResult> VerifyResetToken([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Invalid password reset token.");
            var result = await _authServices.VerfiyReseTokenAsync(new VerfyResetTokenDtos
            (
                email,
                token
            ));
            if (!result.IsSuccess)
                return BadRequest(result);
            return this.ActionResult(result.StatusCode, result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDtos request)
        {
            var result = await _authServices.ResetPasswordAsync(new ResetPasswordDtos
            (
                request.Email,
                request.Token,
                request.NewPassword
            ));
            if (!result.IsSuccess)
                return BadRequest(result);
            return this.ActionResult(result.StatusCode, result);
        }
        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _jwtTokenServices.RefreshTokenAsync();
            return this.ActionResult(result.StatusCode, result);
        }

        #region External Login
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

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
