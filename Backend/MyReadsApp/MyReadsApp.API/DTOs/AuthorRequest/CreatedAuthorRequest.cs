using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.AuthorRequest
{
    /// <summary>
    /// Request DTO for creating a new author.
    /// </summary>
    public class CreatedAuthorRequest
    {
        /// <summary>
        /// Gets or sets the image URL or path for the author.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? AuthorImage { get; set; }

        /// <summary>
        /// Gets or sets the name of the author.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? AuthorName { get; set; }

        /// <summary>
        /// Gets or sets the biography of the author.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string? Bio { get; set; }
    }
}
