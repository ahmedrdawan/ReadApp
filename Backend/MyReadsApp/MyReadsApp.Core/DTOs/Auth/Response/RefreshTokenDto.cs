namespace MyReadsApp.Core.DTOs.Auth.Response
{
    /// <summary>
    /// DTO for refresh token containing token value and expiration time.
    /// </summary>
    public record RefreshTokenDto
    (
        /// <summary>
        /// Gets the refresh token value.
        /// </summary>
        string RefreshToken,

        /// <summary>
        /// Gets the refresh token expiration date and time.
        /// </summary>
        DateTime ExpiresAt
    );
}
