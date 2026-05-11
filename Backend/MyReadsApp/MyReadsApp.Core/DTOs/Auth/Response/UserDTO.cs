namespace MyReadsApp.Core.DTOs.Auth.Response
{
    public record UserDTO
    (
        Guid Id,
        string Email,
        string UserName,
        string Role
    );
}
