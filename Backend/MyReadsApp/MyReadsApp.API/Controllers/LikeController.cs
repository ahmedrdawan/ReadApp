using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles like management endpoints for posts including creating, deleting, and counting likes.
    /// </summary>
    [Authorize]
    [Route("api/post")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeServices _likeServices;
        private readonly IUserAuthServices _userAuthServices;

        public LikeController(ILikeServices likeServices, IUserAuthServices userAuthServices)
        {
            _likeServices = likeServices;
            _userAuthServices = userAuthServices;
        }

        /// <summary>
        /// Retrieves the count of likes for a post.
        /// </summary>
        /// <param name="postId">The unique identifier of the post.</param>
        /// <returns>
        /// HTTP response containing the count of likes for the post.
        /// </returns>
        [HttpGet("{postId}/likes")]
        public async Task<IActionResult> GetCount(Guid postId)
        {
            var result = await _likeServices.CountLikeAsync(postId);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            return Ok(result);
        }

        /// <summary>
        /// Adds a like to a post by the current user.
        /// </summary>
        /// <param name="postId">The unique identifier of the post to like.</param>
        /// <returns>
        /// HTTP response indicating success or failure of adding the like.
        /// </returns>
        [HttpPost("{postId}/likes")]
        public async Task<IActionResult> CreateLike(Guid postId)
        {
            var like = new Like
            {
                PostId = postId,
                UserId = _userAuthServices.GetCurrentUser(),
                CreatedAt = DateTime.UtcNow
            };

            var result = await _likeServices.CreateAsync(like);
            
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode,result);

            return Ok(result);
        }

        /// <summary>
        /// Removes a like from a post by the current user.
        /// </summary>
        /// <param name="postId">The unique identifier of the post to unlike.</param>
        /// <returns>
        /// HTTP response indicating success or failure of removing the like.
        /// </returns>
        [HttpDelete("{postId}/likes")]
        public async Task<IActionResult> DeleteLike(Guid postId)
        {
            var result = await _likeServices.DeleteAsync(postId, _userAuthServices.GetCurrentUser());
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);

            return NoContent();
        }

    }
}
