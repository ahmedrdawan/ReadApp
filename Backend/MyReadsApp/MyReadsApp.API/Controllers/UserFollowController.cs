using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.API.Controllers
{
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
        [HttpDelete("{FollowingId}")]
        public async Task<IActionResult> UnFollowUser(Guid FollowingId)
        {
            var result = await _userfollowServices.DeleteAsync(_userAuthServices.GetCurrentUser(), FollowingId);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            return NoContent();
        }

        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowers()
        {
            var result = await _userfollowServices.GetFollowersAsync(_userAuthServices.GetCurrentUser());
            return Ok(result);

        }
        [HttpGet("following")]
        public async Task<IActionResult> GetFollowing()
        {
            var result = await _userfollowServices.GetFollowingsAsync(_userAuthServices.GetCurrentUser());
            return Ok(result);

        }
    }
}
