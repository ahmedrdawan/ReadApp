using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.API.DTOs.Auth.Request
{
    public record RegisterRequestDto
    (
        [Required]
        [MaxLength(250)]
         string UserName,

        [Required]
        [EmailAddress]
        [MaxLength(255)]
         string Email ,

        [Required]
        [MaxLength(50)]
        [MinLength(6)]
         string Password 
    );
}
