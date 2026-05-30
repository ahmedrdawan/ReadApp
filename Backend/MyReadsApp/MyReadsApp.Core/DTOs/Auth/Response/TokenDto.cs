namespace MyReadsApp.Core.DTOs.Auth.Response
{
    /// <summary>
    /// DTO for JWT token containing access token and expiration time.
    /// </summary>
    public record TokenDto
    (
        /// <summary>
        /// Gets the JWT access token.
        /// </summary>
        string AccessToken,

        /// <summary>
        /// Gets the token expiration date and time.
        /// </summary>
        DateTime ExpiresAt
    );
}
