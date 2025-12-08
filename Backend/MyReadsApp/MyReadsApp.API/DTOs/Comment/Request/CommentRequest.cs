using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.Comment.Request
{
    public class CommentRequest
    {
        [Required]
        public string content { get; set; }
    }
}
