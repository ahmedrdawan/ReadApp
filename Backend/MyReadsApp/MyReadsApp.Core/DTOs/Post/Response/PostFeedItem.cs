using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.DTOs.User.Response;
using System;

namespace MyReadsApp.Core.DTOs.Post.Response
{
    /// <summary>
    /// Response DTO for post feed item containing comprehensive post and related data for feed display.
    /// </summary>
    public class PostFeedItem
    {
        /// <summary>
        /// Gets or sets the unique identifier of the post.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user who created the post.
        /// </summary>
        public UserProfileResponse User { get; set; }

        /// <summary>
        /// Gets or sets the book associated with the post.
        /// </summary>
        public BookAuthorResponse Book { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the post.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the action or activity type (e.g., "added to library", "rated").
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the user's rating for the book.
        /// </summary>
        public int? Rating { get; set; }

        /// <summary>
        /// Gets or sets the total number of likes on the post.
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of comments on the post.
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user has liked this post.
        /// </summary>
        public bool IsLikedByCurrentUser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user has favorited this book.
        /// </summary>
        public bool IsFavoritedByCurrentUser { get; set; }
    }
}
