using System;

namespace MyReadsApp.Core.DTOs.Comment.Request
{
    /// <summary>
    /// Request DTO for creating a comment in the core layer.
    /// </summary>
    public record CreateCommentRequest(
        /// <summary>
        /// Identifier of the post being commented on.
        /// </summary>
        Guid PostId,

        /// <summary>
        /// Identifier of the user creating the comment.
        /// </summary>
        Guid UserId,

        /// <summary>
        /// Comment content text.
        /// </summary>
        string Content
    );
}