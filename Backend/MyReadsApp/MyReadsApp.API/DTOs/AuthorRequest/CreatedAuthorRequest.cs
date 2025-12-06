using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.AuthorRequest
{
    public class CreatedAuthorRequest
    {
        [Required]
        [MaxLength(100)]
        public string? AuthorImage { get; set; }
        [Required]
        [MaxLength(50)]
        public string? AuthorName { get; set; }
        [Required]
        [MaxLength(500)]
        public string? Bio { get; set; }
    }
}
