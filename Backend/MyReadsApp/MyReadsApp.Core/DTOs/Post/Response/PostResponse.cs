using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Post.Response
{
    /// <summary>
    /// Response DTO for post information containing basic post details.
    /// </summary>
    public class PostResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the post.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the post.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the book associated with the post.
        /// </summary>
        public Guid BookId { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the post.
        /// </summary>
        public DateTime? CreatedAt { get; set; }
    }
}
