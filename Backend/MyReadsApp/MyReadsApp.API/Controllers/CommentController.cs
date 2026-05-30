using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.Book.BookRequest;
using MyReadsApp.API.DTOs.Comment.Request;
using MyReadsApp.Core.DTOs.Comment.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Services;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles comment management endpoints including retrieving, creating, updating, and deleting post comments.
    /// </summary>
    [Authorize(Roles = "User")]
    [Route("api/")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentServises _commentServises;
        private readonly IUserAuthServices _userAuthServices;

        public CommentController(ICommentServises commentServises, IUserAuthServices userAuthServices)
        {
            _commentServises = commentServises;
            _userAuthServices = userAuthServices;
        }

        /// <summary>
        /// Retrieves a specific comment by its identifier.
        /// </summary>
        /// <param name="CommentId">The unique identifier of the comment.</param>
        /// <returns>
        /// HTTP response containing comment details or not found error.
        /// </returns>
        [HttpGet("Post/Comment/{CommentId}")]
        public async Task<IActionResult> GetComment(Guid CommentId)
        {
            var result = await _commentServises.GetAsync(CommentId);
            if(!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }

        /// <summary>
        /// Creates a new comment on a post.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post being commented on.</param>
        /// <param name="request">Comment creation data including content.</param>
        /// <returns>
        /// HTTP response indicating success or failure of comment creation.
        /// </returns>
        [HttpPost("Post/{PostId}/Comment")]
        public async Task<IActionResult> CreateComment(Guid PostId, [FromBody] CreatedCommentRequest request)
        {
            // Map API DTO to Core DTO (include current user and post)
            var coreRequest = new MyReadsApp.Core.DTOs.Comment.Request.CreateCommentRequest(PostId, _userAuthServices.GetCurrentUser(), request.content);

            var result = await _commentServises.CreateAsync(coreRequest);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(
                    actionName: "GetComment",
                    routeValues: new { CommentId = result.Value.Id },
                    value: result.Value);
        }

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post containing the comment.</param>
        /// <param name="CommentId">The unique identifier of the comment to update.</param>
        /// <param name="request">Comment update data including new content.</param>
        /// <returns>
        /// HTTP response indicating success or failure of the update.
        /// </returns>
        [HttpPut("Post/{PostId}/Comment/{CommentId}")]
        public async Task<IActionResult> UpdateComment(Guid PostId, Guid CommentId, UpdateCommentRequest request)
        {
            // Map API DTO to Core DTO
            var coreUpdate = new MyReadsApp.Core.DTOs.Comment.Request.UpdateCommentRequest(request.content);

            var result = await _commentServises.UpdateAsync(CommentId, coreUpdate);
            if (!result.IsSuccess)
                return BadRequest(result);
            return NoContent();
        }

        /// <summary>
        /// Deletes a comment by its identifier.
        /// </summary>
        /// <param name="CommentId">The unique identifier of the comment to delete.</param>
        /// <returns>
        /// HTTP response indicating success or failure of deletion.
        /// </returns>
        [HttpDelete("Post/Comment/{CommentId}")]
        public async Task<IActionResult> DeleteComment(Guid CommentId)
        {
            var result = await _commentServises.DeleteAsync(CommentId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a paginated collection of comments for a specific post.
        /// </summary>
        /// <param name="PostId">The unique identifier of the post.</param>
        /// <param name="pageNumber">The page number for pagination (default: 1).</param>
        /// <param name="pageSize">The number of comments per page (default: 10).</param>
        /// <returns>
        /// HTTP response containing collection of comments for the post.
        /// </returns>
        [HttpGet("Post/{PostId}/Comments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(Guid PostId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var res = await _commentServises.GetListAsync(PostId, pageNumber, pageSize);
            return StatusCode(res.StatusCode, res);
        }
    }
}
