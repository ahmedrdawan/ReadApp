using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.Core.DTOs.Auth.Request
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
