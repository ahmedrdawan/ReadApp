using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.DTOs.Comment.Response;
using MyReadsApp.Core.DTOs.Comment.Request;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces
{
    /// <summary>
    /// Provides operations for managing comments on posts.
    /// </summary>
    public interface ICommentServises
    {
        /// <summary>
        /// Creates a new comment asynchronously.
        /// </summary>
        /// <param name="comment">Comment entity to create.</param>
        /// <returns>
        /// A task returning a Response containing the created CommentResponse.
        /// </returns>
        Task<Response<CommentResponse>> CreateAsync(CreateCommentRequest request);

        /// <summary>
        /// Deletes a comment by its identifier.
        /// </summary>
        /// <param name="CommentId">The unique identifier of the comment to delete.</param>
        /// <returns>
        /// A task returning a Response containing the deleted CommentResponse.
        /// </returns>
        Task<Response<CommentResponse>> DeleteAsync(Guid CommentId);

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="CommentId">The unique identifier of the comment to update.</param>
        /// <param name="newComment">Updated comment data.</param>
        /// <returns>
        /// A task returning a Response containing the updated CommentResponse.
        /// </returns>
        Task<Response<CommentResponse>> UpdateAsync(Guid CommentId, UpdateCommentRequest request);

        /// <summary>
        /// Retrieves a comment by its identifier.
        /// </summary>
        /// <param name="CommentId">The unique identifier of the comment.</param>
        /// <returns>
        /// A task returning a Response containing the CommentResponse.
        /// </returns>
        Task<Response<CommentResponse>> GetAsync(Guid CommentId);

        /// <summary>
        /// Retrieves a paginated list of comments for a post.
        /// </summary>
        /// <param name="postId">The post identifier for which to retrieve comments.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>
        /// A task returning a Response containing a paged result of CommentResponse.
        /// </returns>
        Task<Response<MyReadsApp.Core.Common.PagedResult<CommentResponse>>> GetListAsync(Guid postId, int pageNumber = 1, int pageSize = 10);
    }
}
