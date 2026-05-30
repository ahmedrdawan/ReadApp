using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for user login containing credentials.
    /// </summary>
    public record LoginDto
    (
        /// <summary>
        /// Gets the email address for login.
        /// </summary>
        string Email ,

        /// <summary>
        /// Gets the password for login.
        /// </summary>
        string Password 
    );
}
