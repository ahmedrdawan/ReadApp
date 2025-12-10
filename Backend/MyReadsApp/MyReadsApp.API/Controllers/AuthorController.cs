using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.AuthorRequest;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;

namespace MyReadsApp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorServices _authorServices;

        public AuthorController(IAuthorServices authorServices)
        {
            _authorServices = authorServices;
        }

        [HttpGet("{AuthorId}")]
        public async Task<IActionResult> GetAuthor(Guid AuthorId)
        {
            var result = await _authorServices.GetAsync(AuthorId);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] CreatedAuthorRequest request)
        {
            var author = new Author
            {
                Id = Guid.NewGuid(),
                AuthorName = request.AuthorName,
                AuthorImage = request.AuthorImage,
                Bio = request.Bio,
            };

            var result = await _authorServices.CreateAsync(author);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(
                    actionName: "GetAuthor",
                    routeValues: new { AuthorId = author.Id },
                    value: result.Value
                );
        }

        [HttpPut("{AuthorId}")]
        public async Task<IActionResult> UpdateAuthor(Guid AuthorId, UpdatedAuthorRequest request)
        {
            var newAuthor = new Author
            {
                AuthorName = request.AuthorName,
                AuthorImage = request.AuthorImage,
                Bio = request.Bio,
            };

            var result = await _authorServices.UpdateAsync(AuthorId, newAuthor);
            if (!result.IsSuccess)
                return BadRequest(result);

            return NoContent();
        }

        [HttpDelete("{AuthorId}")]
        public async Task<IActionResult> DeleteAuthor(Guid AuthorId)
        {
            var result = await _authorServices.DeleteAsync(AuthorId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return NoContent();
        }
    }
}
