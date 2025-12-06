using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.AuthorRequest
{
    public class UpdatedAuthorRequest
    {
        [Required]
        [MaxLength(100)]
        public string? AuthorImage { get; set; }
        [Required]
        [MaxLength(500)]
        public string? Bio { get; set; }
    }
}
