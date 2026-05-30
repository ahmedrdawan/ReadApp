using System;

namespace MyReadsApp.Core.DTOs.Comment.Request
{
    /// <summary>
    /// Request DTO for updating a comment in the core layer.
    /// </summary>
    public record UpdateCommentRequest(
        /// <summary>
        /// New content for the comment.
        /// </summary>
        string Content
    );
}