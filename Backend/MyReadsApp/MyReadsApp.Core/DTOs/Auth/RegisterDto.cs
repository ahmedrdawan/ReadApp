using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for user registration containing credentials for account creation.
    /// </summary>
    public record RegisterDto
    (
        /// <summary>
        /// Gets the username for the new account.
        /// </summary>
        string UserName ,

        /// <summary>
        /// Gets the email address for the new account.
        /// </summary>
        string Email,

        /// <summary>
        /// Gets the password for the new account.
        /// </summary>
        string Password 
    );
}
