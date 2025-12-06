using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Account
{
    public class SignInDtos
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
