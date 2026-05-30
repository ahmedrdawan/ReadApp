using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Book.BookRequest
{
    /// <summary>
    /// Request DTO for updating an existing book.
    /// </summary>
    public class UpdateBookRequest
    {
        /// <summary>
        /// Gets or sets the image URL or path for the book.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? BookImage { get; set; }

        /// <summary>
        /// Gets or sets the description of the book.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        [MaxLength(50)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the author of the book.
        /// </summary>
        [Required]
        public Guid AuthorId { get; set; }
    }
}
