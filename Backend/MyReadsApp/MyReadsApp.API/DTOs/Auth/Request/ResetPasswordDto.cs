using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Auth.Request
{
    public record ResetPasswordDto
    (
        [Required]
        [EmailAddress]
        string Email ,
        [Required]
        [MaxLength(255)]
         string Token ,
        [Required]
        [MaxLength(50)]
        [MinLength(6)]
         string NewPassword
    );
}
