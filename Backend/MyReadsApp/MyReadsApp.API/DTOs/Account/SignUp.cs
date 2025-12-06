using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Account
{
    public class SignUpDtos
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
