using System;

namespace MyReadsApp.Core.DTOs.Post.Request
{
    /// <summary>
    /// Request DTO for creating a new post in the core layer.
    /// </summary>
    public record CreatePostRequest(
        /// <summary>
        /// Identifier of the related book for the post.
        /// </summary>
        Guid BookId,
        /// <summary>
        /// Identifier of the user creating the post.
        /// </summary>
        Guid UserId
    );
}