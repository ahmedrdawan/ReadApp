using System;

namespace MyReadsApp.Core.DTOs.Author
{
    /// <summary>
    /// Request DTO for updating an author in the core layer.
    /// </summary>
    public record UpdateAuthorRequest(
        /// <summary>
        /// Author's display name.
        /// </summary>
        string AuthorName,

        /// <summary>
        /// Optional image URL or path for the author.
        /// </summary>
        string? AuthorImage,

        /// <summary>
        /// Biography or description for the author.
        /// </summary>
        string? Bio
    );
}