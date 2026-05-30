using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Auth.Response
{
    /// <summary>
    /// DTO for authenticated user information containing user details and JWT token.
    /// </summary>
    public record AuthResponse
    (
        /// <summary>
        /// Gets the authenticated user information.
        /// </summary>
        UserDTO User,

        /// <summary>
        /// Gets the JWT token for the authenticated session.
        /// </summary>
        TokenDto? JwtToken
    );
}
