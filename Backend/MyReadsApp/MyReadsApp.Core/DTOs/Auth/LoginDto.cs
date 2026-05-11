using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.Core.DTOs.Auth
{
    public record LoginDto
    (
        string Email ,
        string Password 
    );
}
