using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for email confirmation containing user ID and confirmation code.
    /// </summary>
    public record ConfirmEmailDto(
        /// <summary>
        /// Gets the user identifier for email confirmation.
        /// </summary>
        string UserId, 

        /// <summary>
        /// Gets the email confirmation code.
        /// </summary>
        string code);
}
