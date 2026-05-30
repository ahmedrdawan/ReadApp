namespace MyReadsApp.API.DTOs.Post
{
    /// <summary>
    /// Base request DTO for post operations containing book identifier.
    /// </summary>
    public class BasePostRequest
    {
        /// <summary>
        /// Gets or sets the identifier of the book associated with the post.
        /// </summary>
        public Guid BookId { get; set; }
    }

}
