using MyReadsApp.API.DTOs.Book.BookRequest;
using MyReadsApp.API.DTOs.Post;
using MyReadsApp.Core.DTOs.Post.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.Services.Interfaces.Account;
using Microsoft.AspNetCore.Authorization;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles post management endpoints including retrieving, creating, updating, and deleting posts.
    /// </summary>
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostServices _PostServices;
        private readonly IUserAuthServices _userAuthServices;

        public PostController(IPostServices PostServices, IUserAuthServices userAuthServices)
        {
            _PostServices = PostServices;
            _userAuthServices = userAuthServices;
        }

        /// <summary>
        /// Retrieves a paginated feed of posts.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination (default: 1).</param>
        /// <param name="pageSize">The number of posts per page (default: 10).</param>
        /// <returns>
        /// HTTP response containing paginated collection of posts.
        /// </returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            Guid? currentUser = null;
            if (User?.Identity?.IsAuthenticated == true)
            {
                try { currentUser = _userAuthServices.GetCurrentUser(); } catch { currentUser = null; }
            }

            var result = await _PostServices.GetFeedAsync(pageNumber, pageSize, currentUser);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves a specific post by its identifier.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post.</param>
        /// <returns>
        /// HTTP response containing post details or not found error.
        /// </returns>
        [HttpGet("{PostId}")]
        public async Task<IActionResult> GetPost(Guid PostId)
        {
            var result = await _PostServices.GetAsync(PostId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }

        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="request">Post creation data including book identifier.</param>
        /// <returns>
        /// HTTP response indicating success or failure of post creation.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatedPostRequest request)
        {
            // Map API DTO to Core DTO (include current user)
            var coreRequest = new MyReadsApp.Core.DTOs.Post.Request.CreatePostRequest(request.BookId, _userAuthServices.GetCurrentUser());

            var result = await _PostServices.CreateAsync(coreRequest);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(
                actionName: "GetPost",
                routeValues: new { PostId = result.Value.Id },
                value: result.Value);
        }

        /// <summary>
        /// Updates an existing post.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post to update.</param>
        /// <param name="request">Post update data.</param>
        /// <returns>
        /// HTTP response indicating success or failure of the update.
        /// </returns>
        [HttpPut("{PostId}")]
        public async Task<IActionResult> UpdatePost(Guid PostId, UpdatePostRequest request)
        {
            // Map API DTO to Core DTO
            var coreUpdate = new MyReadsApp.Core.DTOs.Post.Request.UpdatePostRequest(request.BookId);

            var result = await _PostServices.UpdateAsync(PostId, coreUpdate);
            if (!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }

        /// <summary>
        /// Deletes a post by its identifier.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post to delete.</param>
        /// <returns>
        /// HTTP response indicating success or failure of deletion.
        /// </returns>
        [HttpDelete("{PostId}")]
        public async Task<IActionResult> DeletePost(Guid PostId)
        {
            var result = await _PostServices.DeleteAsync(PostId);
            if (!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }
    }
}

