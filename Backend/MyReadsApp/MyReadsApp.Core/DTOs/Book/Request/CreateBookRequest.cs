using System;

namespace MyReadsApp.Core.DTOs.Book.Request
{
    /// <summary>
    /// Request DTO for creating a new book in the core layer.
    /// </summary>
    public record CreateBookRequest(
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
        /// Main content of the book.
        /// </summary>
        string? Content,

        /// <summary>
        /// Author identifier for the book.
        /// </summary>
        Guid AuthorId
    );
}