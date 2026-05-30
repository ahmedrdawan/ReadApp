using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles user follow management endpoints including following/unfollowing users and retrieving followers/following lists.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserFollowController : ControllerBase
    {
        private readonly IUserfollowServices _userfollowServices;
        private readonly IUserAuthServices _userAuthServices;

        public UserFollowController(IUserfollowServices userfollowServices, IUserAuthServices userauthServices)
        {
            _userfollowServices = userfollowServices;
            _userAuthServices = userauthServices;
        }

        /// <summary>
        /// Follows a user by their identifier.
        /// </summary>
        /// <param name="FollowingId">The unique identifier of the user to follow.</param>
        /// <returns>
        /// HTTP response indicating success or failure of following the user.
        /// </returns>
        [HttpPost("{FollowingId}")]
        public async Task<IActionResult> FollowUser(Guid FollowingId)
        {
            var userFollow = new UserFollow
            {
                Id = Guid.NewGuid(),
                FollowingId = FollowingId,
                FollowerId = _userAuthServices.GetCurrentUser(),
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userfollowServices.CreateAsync(userFollow);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            return Ok(result);
        }

        /// <summary>
        /// Unfollows a user by their identifier.
        /// </summary>
        /// <param name="FollowingId">The unique identifier of the user to unfollow.</param>
        /// <returns>
        /// HTTP response indicating success or failure of unfollowing the user.
        /// </returns>
        [HttpDelete("{FollowingId}")]
        public async Task<IActionResult> UnFollowUser(Guid FollowingId)
        {
            var result = await _userfollowServices.DeleteAsync(_userAuthServices.GetCurrentUser(), FollowingId);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            return NoContent();
        }

        /// <summary>
        /// Retrieves the list of followers for the current user.
        /// </summary>
        /// <returns>
        /// HTTP response containing collection of followers.
        /// </returns>
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowers()
        {
            var result = await _userfollowServices.GetFollowersAsync(_userAuthServices.GetCurrentUser());
            return Ok(result);

        }

        /// <summary>
        /// Retrieves the list of users the current user is following.
        /// </summary>
        /// <returns>
        /// HTTP response containing collection of users being followed.
        /// </returns>
        [HttpGet("following")]
        public async Task<IActionResult> GetFollowing()
        {
            var result = await _userfollowServices.GetFollowingsAsync(_userAuthServices.GetCurrentUser());
            return Ok(result);

        }
    }
}
