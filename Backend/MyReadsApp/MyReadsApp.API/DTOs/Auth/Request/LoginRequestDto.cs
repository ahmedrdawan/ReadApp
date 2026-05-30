using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Auth.Request
{
    /// <summary>
    /// Request DTO for user login containing credentials.
    /// </summary>
    public record LoginRequestDto
    (
        /// <summary>
        /// Gets the email address for login.
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        string Email,

        /// <summary>
        /// Gets the password for login.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [MinLength(6)]
        string Password
    );
}
