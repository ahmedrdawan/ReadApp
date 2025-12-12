using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.API.Controllers
{
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

        [HttpGet("{postId}/likes")]
        public async Task<IActionResult> GetCount(Guid postId)
        {
            var result = await _likeServices.CountLikeAsync(postId);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            return Ok(result);
        }
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
