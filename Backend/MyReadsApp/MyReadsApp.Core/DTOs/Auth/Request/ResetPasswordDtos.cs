namespace MyReadsApp.Core.DTOs.Auth.Request
{
    public record ResetPasswordDtos(string Email, string Token, string NewPassword);
}
