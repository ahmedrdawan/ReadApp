using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Auth
{
    public record RegisterDto
    (
        string UserName ,
        string Email,
        string Password 
    );
}
