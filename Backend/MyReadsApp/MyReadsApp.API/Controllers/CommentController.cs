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

        [HttpGet("Post/Comment/{CommentId}")]
        public async Task<IActionResult> GetComment(Guid CommentId)
        {
            var result = await _commentServises.GetAsync(CommentId);
            if(!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }

        [HttpPost("Post/{PostId}/Comment")]
        public async Task<IActionResult> CreateComment(Guid PostId, [FromBody] CreatedCommentRequest request)
        {
            var Comment = new Comment
            {
                Id = Guid.NewGuid(),
                content = request.content,
                CreatedAt = DateTime.UtcNow,
                UserId = _userAuthServices.GetCurrentUser(),
                PostId = PostId
            };


            var result = await _commentServises.CreateAsync(Comment);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(
                    actionName: "GetComment",
                    routeValues: new { CommentId = Comment.Id },
                    value: result.Value);
        }

        [HttpPut("Post/{PostId}/Comment/{CommentId}")]
        public async Task<IActionResult> UpdateComment(Guid PostId, Guid CommentId, UpdateCommentRequest request)
        {
            var NewComment = new Comment
            {
                Id = CommentId,
                UserId = _userAuthServices.GetCurrentUser(),
                CreatedAt = DateTime.UtcNow,
                PostId = PostId,
                content = request.content,
            };

            var result = await _commentServises.UpdateAsync(CommentId, NewComment);
            if (!result.IsSuccess)
                return BadRequest(result);
            return  NoContent();
        }

        [HttpDelete("Post/Comment/{CommentId}")]
        public async Task<IActionResult> DeleteComment(Guid CommentId)
        {
            var result = await _commentServises.DeleteAsync(CommentId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return NoContent();
        }
    }
}
