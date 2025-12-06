using MyReadsApp.API.DTOs;
using MyReadsApp.API.DTOs.Account;
using MyReadsApp.Core.DTOs.Auth.Request;
using MyReadsApp.Core.Services.Interfaces.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyReadsApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthServices _authServices;
        private readonly IEmailservices _emailservices;
        private readonly IConfiguration _Configration;

        public AuthController(IAuthServices authServices, IEmailservices emailservices, IConfiguration configration)
        {
            _authServices = authServices;
            _emailservices = emailservices;
            _Configration = configration;
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
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return NotFound();

            var result = await _emailservices.ConfirmEmailAsync(new ConfirmEmailRequest(userId, code));
            if (!result.IsSuccess)
                return BadRequest(result);

            return Redirect($"{_Configration["appURL"]}/confirmemail.html");
        }

    }
}
