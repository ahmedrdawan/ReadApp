using MyReadsApp.API.DTOs;
using MyReadsApp.API.DTOs.Account;
using MyReadsApp.Core.DTOs.Auth.Request;
using MyReadsApp.Core.Services.Interfaces.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Infstructure.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;

namespace MyReadsApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthServices _authServices;
        private readonly IConfiguration _Configration;
        private readonly JwtTokenServices _jwtTokenServices;

        public AuthController(IAuthServices authServices, IConfiguration configration, JwtTokenServices jwtTokenServices)
        {
            _authServices = authServices;
            _Configration = configration;
            _jwtTokenServices = jwtTokenServices;
        }

        [HttpPost("Sign-Up")]
        public async Task<IActionResult> Register([FromBody] SignUpDtos request)
        {
            var result = await _authServices.RegisterAsync(new RegisterRequest
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
            });
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Sign-In")]
        public async Task<IActionResult> Login([FromBody] SignInDtos request)
        {
            var result = await _authServices.LoginAsync(new LoginRequest
            {
                Email = request.Email,
                Password = request.Password,
            });

            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery]string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Invalid email confirmation request.");

            var result = await _authServices.ConfirmEmailAsync(userId, token);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _jwtTokenServices.RefreshTokenAsync();
            return StatusCode(result.StatusCode, result);
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
            return StatusCode(response.StatusCode, response);
        }
        #endregion
    }
}
