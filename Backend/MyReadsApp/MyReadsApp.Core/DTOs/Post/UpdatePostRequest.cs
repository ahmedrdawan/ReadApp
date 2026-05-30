using System;

namespace MyReadsApp.Core.DTOs.Post.Request
{
    /// <summary>
    /// Request DTO for updating an existing post in the core layer.
    /// </summary>
    public record UpdatePostRequest(
        /// <summary>
        /// Identifier of the related book for the post.
        /// </summary>
        Guid BookId
    );
}