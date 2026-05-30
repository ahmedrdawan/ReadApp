using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Auth.Request
{
    /// <summary>
    /// Request DTO for password reset containing email, reset token, and new password.
    /// </summary>
    public record ResetPasswordDto
    (
        /// <summary>
        /// Gets the email address for password reset.
        /// </summary>
        [Required]
        [EmailAddress]
        string Email ,

        /// <summary>
        /// Gets the password reset token.
        /// </summary>
        [Required]
        [MaxLength(255)]
        string Token ,

        /// <summary>
        /// Gets the new password.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [MinLength(6)]
        string NewPassword
    );
}
