using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Author.Response;
using MyReadsApp.Core.DTOs.Post.Response;
using MyReadsApp.Core.DTOs.Post.Request;
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
    /// Provides operations for managing posts, including CRUD and feed retrieval.
    /// </summary>
    public interface IPostServices
    {
        /// <summary>
        /// Creates a new post asynchronously.
        /// </summary>
        /// <param name="post">Post entity to create.</param>
        /// <returns>
        /// A task returning a Response containing the created PostResponse.
        /// </returns>
        Task<Response<PostResponse>> CreateAsync(CreatePostRequest request);

        /// <summary>
        /// Deletes a post by its identifier.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post to delete.</param>
        /// <returns>
        /// A task returning a Response containing the deleted PostResponse.
        /// </returns>
        Task<Response<PostResponse>> DeleteAsync(Guid PostId);

        /// <summary>
        /// Updates an existing post.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post to update.</param>
        /// <param name="newPost">Updated post data.</param>
        /// <returns>
        /// A task returning a Response containing the updated PostResponse.
        /// </returns>
        Task<Response<PostResponse>> UpdateAsync(Guid PostId, UpdatePostRequest request);

        /// <summary>
        /// Retrieves a post by its identifier.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post.</param>
        /// <returns>
        /// A task returning a Response containing the PostResponse.
        /// </returns>
        Task<Response<PostResponse>> GetAsync(Guid PostId);

        /// <summary>
        /// Retrieves a paginated feed of posts, optionally including context for a specific user.
        /// </summary>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="currentUserId">Optional current user identifier to include user-specific context.</param>
        /// <returns>
        /// A task returning a Response containing a paged result of PostFeedItem.
        /// </returns>
        Task<Response<MyReadsApp.Core.Common.PagedResult<MyReadsApp.Core.DTOs.Post.Response.PostFeedItem>>> GetFeedAsync(int pageNumber = 1, int pageSize = 10, Guid? currentUserId = null);
    }
}
