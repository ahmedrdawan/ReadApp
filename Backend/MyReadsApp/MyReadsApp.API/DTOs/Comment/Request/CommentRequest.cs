using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Comment.Request
{
    /// <summary>
    /// Base request DTO for comment operations containing content.
    /// </summary>
    public class CommentRequest
    {
        /// <summary>
        /// Gets or sets the content of the comment.
        /// </summary>
        [Required]
        public string content { get; set; }
    }
}
