using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Auth.Response
{
    public record AuthResponse
    (
        UserDTO User,
        TokenResult JwtToken
    );

    public record UserDTO
    (
        Guid Id,
        string Email,
        string UserName,
        string Role
    );

    public record TokenResult
    (
        string Token,
        DateTime ExpiresAt
    );
}
