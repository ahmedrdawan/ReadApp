using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.DTOs.User.Response;
using System;

namespace MyReadsApp.Core.DTOs.Post.Response
{
    public class PostFeedItem
    {
        public Guid Id { get; set; }
        public UserProfileResponse User { get; set; }
        public BookAuthorResponse Book { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Action { get; set; }
        public int? Rating { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; }
    }
}
