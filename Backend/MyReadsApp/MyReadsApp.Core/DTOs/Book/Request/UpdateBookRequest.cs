using System;

namespace MyReadsApp.Core.DTOs.Book.Request
{
    /// <summary>
    /// Request DTO for updating an existing book in the core layer.
    /// </summary>
    public record UpdateBookRequest(
        /// <summary>
        /// Image URL or path for the book.
        /// </summary>
        string? BookImage,

        /// <summary>
        /// Description of the book.
        /// </summary>
        string? Description,

        /// <summary>
        /// Title of the book.
        /// </summary>
        string? Title,

        /// <summary>
        /// Author identifier for the book.
        /// </summary>
        Guid AuthorId
    );
}