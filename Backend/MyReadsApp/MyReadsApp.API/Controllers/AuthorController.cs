using MyReadsApp.API.DTOs.AuthorRequest;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyReadsApp.API.Controllers
{
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
            return result is null ? NotFound(result) : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] CreatedAuthorRequest request)
        {
            var auhtor = new Author
            {
                Id = Guid.NewGuid(),
                AuthorName = request.AuthorName,
                AuthorImage = request.AuthorImage,
                Bio = request.Bio,
            };
            var result = await _authorServices.CreateAsync(auhtor);
            return result <= 0 ? BadRequest(request) : CreatedAtAction(
                    actionName: "GetAuthor",
                    routeValues: new { AuthorId = auhtor.Id },
                    value: auhtor
                );
        }

        [HttpPut("{AuthorId}")]
        public async Task<IActionResult> UpdateAuthor(Guid AuthorId, UpdatedAuthorRequest request)
        {
            var NewAuthor = new Author
            {
                AuthorImage = request.AuthorImage,
                Bio = request.Bio,
            };

            var result = await _authorServices.UpdateAsync(AuthorId, NewAuthor);
            return result <= 0 ? BadRequest(AuthorId) : NoContent();
        }

        [HttpDelete("{AuthorId}")]
        public async Task<IActionResult> DeleteAuthor(Guid AuthorId)
        {
            var result = await _authorServices.DeleteAsync(AuthorId);
            return result <= 0 ? BadRequest(AuthorId) : NoContent();
        }
    }
}
