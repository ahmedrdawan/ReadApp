namespace MyReadsApp.Core.DTOs.Auth.Response
{
    public record RefreshTokenDto
    (
        string RefreshToken,
        DateTime ExpiresAt
    );
}
