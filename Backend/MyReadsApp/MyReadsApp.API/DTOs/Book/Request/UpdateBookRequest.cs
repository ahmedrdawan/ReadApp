using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Book.BookRequest
{
    public class UpdateBookRequest
    {
        [Required]
        [MaxLength(100)]
        public string? BookImage { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(50)]
        public string? Title { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
    }
}
