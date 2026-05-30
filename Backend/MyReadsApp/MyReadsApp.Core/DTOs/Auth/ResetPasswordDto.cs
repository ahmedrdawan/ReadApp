namespace MyReadsApp.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for password reset containing email, reset token, and new password.
    /// </summary>
    public record ResetPasswordDto(
        /// <summary>
        /// Gets the email address for password reset.
        /// </summary>
        string Email, 

        /// <summary>
        /// Gets the password reset token.
        /// </summary>
        string Token, 

        /// <summary>
        /// Gets the new password.
        /// </summary>
        string NewPassword);
}
