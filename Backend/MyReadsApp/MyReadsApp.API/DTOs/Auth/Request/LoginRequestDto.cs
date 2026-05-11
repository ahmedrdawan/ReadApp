using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Auth.Request
{
    public record LoginRequestDto
    (
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        string Email,
        [Required]
        [MaxLength(50)]
        [MinLength(6)]
         string Password
    );
}
