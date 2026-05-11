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
        TokenDto? JwtToken
    );
}
