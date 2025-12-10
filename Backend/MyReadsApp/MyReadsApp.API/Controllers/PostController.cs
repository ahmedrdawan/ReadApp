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

        [HttpGet("{PostId}")]
        public async Task<IActionResult> GetPost(Guid PostId)
        {
            var result = await _PostServices.GetAsync(PostId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatedPostRequest request)
        {
            var Post = new Post
            {
                Id = Guid.NewGuid(),
                BookId = request.BookId,
                UserId = _userAuthServices.GetCurrentUser(),
                CreatedAt = DateTime.UtcNow,
            };

            var result = await _PostServices.CreateAsync(Post);
            if (!result.IsSuccess)
                return BadRequest(result);
            return 
                CreatedAtAction(
                    actionName: "GetPost",
                    routeValues: new { PostId = Post.Id },
                    value: result.Value);
        }

        [HttpPut("{PostId}")]
        public async Task<IActionResult> UpdatePost(Guid PostId, UpdatePostRequest request)
        {
            var NewPost = new Post
            {
                BookId = request.BookId,
                UpdatedAt = DateTime.UtcNow,
            };

            var result = await _PostServices.UpdateAsync(PostId, NewPost);
            if (!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }

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

