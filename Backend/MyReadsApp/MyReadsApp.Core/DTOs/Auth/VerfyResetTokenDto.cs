using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for password reset token verification containing email and token.
    /// </summary>
    public record VerfyResetTokenDto
    (
        /// <summary>
        /// Gets the email address for token verification.
        /// </summary>
        string Email, 

        /// <summary>
        /// Gets the reset token to verify.
        /// </summary>
        string Token);
}
