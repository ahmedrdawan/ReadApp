using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.API.DTOs.Auth.Request
{
    /// <summary>
    /// Request DTO for user registration containing credentials.
    /// </summary>
    public record RegisterRequestDto
    (
        /// <summary>
        /// Gets the username for the new account.
        /// </summary>
        [Required]
        [MaxLength(250)]
        string UserName,

        /// <summary>
        /// Gets the email address for the new account.
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        string Email ,

        /// <summary>
        /// Gets the password for the new account.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [MinLength(6)]
        string Password 
    );
}
