namespace MyReadsApp.Core.DTOs.Auth.Response
{
    public record TokenDto
    (
        string AccessToken,
        DateTime ExpiresAt
    );
}
