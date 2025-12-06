using MyReadsApp.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Book.BookRequest
{
    public class CreatedBookRequest
    {
        [Required]
        [MaxLength(100)]
        public string? BookImage { get; set; }
        [Required]
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Title { get; set; }
        [Required]
        [MaxLength(2000)]
        public string? Content { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
    }
}
